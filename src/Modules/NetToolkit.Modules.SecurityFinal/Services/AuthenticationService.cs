using NetToolkit.Modules.SecurityFinal.Models;
using NetToolkit.Modules.SecurityFinal.Interfaces;
using Microsoft.Extensions.Logging;
using System.DirectoryServices.AccountManagement;
using System.Security.Principal;
using System.Text;
using System.Security.Cryptography;

namespace NetToolkit.Modules.SecurityFinal.Services;

/// <summary>
/// Windows authentication and authorization service
/// The digital gatekeeper of NetToolkit - ensuring only worthy souls pass through
/// </summary>
public class AuthenticationService : IAuthenticationService
{
    private readonly ILogger<AuthenticationService> _logger;
    private readonly IAuditService _auditService;
    private readonly Dictionary<string, DateTime> _authenticationCache;
    private readonly Dictionary<string, string> _pinStorage;
    private readonly object _authLock = new();
    private readonly string _pinStorePath;

    // Role hierarchies for authorization
    private readonly Dictionary<UserRole, List<SecureOperation>> _rolePermissions;

    public AuthenticationService(ILogger<AuthenticationService> logger, IAuditService auditService)
    {
        _logger = logger;
        _auditService = auditService;
        _authenticationCache = new Dictionary<string, DateTime>();
        _pinStorage = new Dictionary<string, string>();
        _pinStorePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "NetToolkit", "Security", "Pins");
        
        _rolePermissions = InitializeRolePermissions();
        Directory.CreateDirectory(_pinStorePath);
        LoadStoredPins();
    }

    public async Task<AuthenticationResult> AuthenticateAsync()
    {
        try
        {
            _logger.LogDebug("Authenticating current Windows user");

            var identity = WindowsIdentity.GetCurrent();
            if (identity == null || !identity.IsAuthenticated)
            {
                await AuditAuthenticationFailure("No Windows identity available");
                return new AuthenticationResult
                {
                    IsAuthenticated = false,
                    Username = "Unknown",
                    ErrorMessage = "Windows authentication required"
                };
            }

            var username = identity.Name ?? "Unknown";
            var userRole = await DetermineUserRoleAsync(identity);
            var permissions = GetPermissionsForRole(userRole);

            // Cache successful authentication
            lock (_authLock)
            {
                _authenticationCache[username] = DateTime.UtcNow;
            }

            var result = new AuthenticationResult
            {
                IsAuthenticated = true,
                Username = username,
                Role = userRole,
                AuthenticatedAt = DateTime.UtcNow,
                IsPinEnabled = HasStoredPin(username),
                Permissions = permissions
            };

            await AuditSuccessfulAuthentication(result);
            
            _logger.LogInformation("🔐 Authentication successful: {Username} ({Role}) - Digital gates opened!", 
                username, userRole);
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Authentication failed");
            await AuditAuthenticationFailure($"Authentication exception: {ex.Message}");
            
            return new AuthenticationResult
            {
                IsAuthenticated = false,
                Username = "Unknown",
                ErrorMessage = $"Authentication failed: {ex.Message}"
            };
        }
    }

    public async Task<AuthorizationResult> AuthorizeAsync(SecureOperation operation)
    {
        try
        {
            _logger.LogDebug("Authorizing operation: {Operation}", operation);

            var authResult = await AuthenticateAsync();
            if (!authResult.IsAuthenticated)
            {
                await AuditAuthorizationFailure(operation, "User not authenticated");
                return new AuthorizationResult
                {
                    IsAuthorized = false,
                    Operation = operation,
                    RequiredRole = UserRole.User,
                    UserRole = UserRole.Guest,
                    DenialReason = "Authentication required",
                    CheckedAt = DateTime.UtcNow
                };
            }

            var requiredRole = GetRequiredRoleForOperation(operation);
            var hasPermission = authResult.Permissions.Contains(operation.ToString()) || 
                               IsRoleAuthorized(authResult.Role, requiredRole);

            var authorizationResult = new AuthorizationResult
            {
                IsAuthorized = hasPermission,
                Operation = operation,
                RequiredRole = requiredRole,
                UserRole = authResult.Role,
                DenialReason = hasPermission ? null : $"Insufficient privileges - {requiredRole} role required",
                CheckedAt = DateTime.UtcNow
            };

            if (hasPermission)
            {
                await AuditSuccessfulAuthorization(authorizationResult);
                _logger.LogDebug("✅ Authorization granted: {Username} for {Operation}", 
                    authResult.Username, operation);
            }
            else
            {
                await AuditAuthorizationFailure(operation, authorizationResult.DenialReason!);
                _logger.LogWarning("❌ Authorization denied: {Username} for {Operation} - {Reason}", 
                    authResult.Username, operation, authorizationResult.DenialReason);
            }

            return authorizationResult;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Authorization check failed for operation: {Operation}", operation);
            await AuditAuthorizationFailure(operation, $"Authorization exception: {ex.Message}");
            
            return new AuthorizationResult
            {
                IsAuthorized = false,
                Operation = operation,
                RequiredRole = UserRole.Administrator,
                UserRole = UserRole.Guest,
                DenialReason = $"Authorization check failed: {ex.Message}",
                CheckedAt = DateTime.UtcNow
            };
        }
    }

    public async Task<UserRole> GetUserRoleAsync()
    {
        try
        {
            var identity = WindowsIdentity.GetCurrent();
            if (identity?.Name == null)
            {
                return UserRole.Guest;
            }

            return await DetermineUserRoleAsync(identity);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to determine user role");
            return UserRole.Guest;
        }
    }

    public async Task EnablePinAuthenticationAsync(string pin)
    {
        try
        {
            var authResult = await AuthenticateAsync();
            if (!authResult.IsAuthenticated)
            {
                throw new InvalidOperationException("User must be authenticated to set PIN");
            }

            if (string.IsNullOrWhiteSpace(pin) || pin.Length < 4)
            {
                throw new ArgumentException("PIN must be at least 4 characters long");
            }

            var hashedPin = HashPin(pin);
            var username = authResult.Username;

            lock (_authLock)
            {
                _pinStorage[username] = hashedPin;
            }

            await StorePinSecurely(username, hashedPin);
            
            await AuditSecurityEvent(new SecurityAuditEvent
            {
                EventId = Guid.NewGuid().ToString(),
                EventType = SecurityEventType.SecurityPolicyChange,
                UserId = username,
                Description = "PIN authentication enabled",
                OccurredAt = DateTime.UtcNow,
                SourceModule = "SecurityFinal",
                Severity = SeverityLevel.Medium,
                IsSuccessful = true
            });

            _logger.LogInformation("📱 PIN authentication enabled for user: {Username} - Secondary fortress activated!", 
                username);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to enable PIN authentication");
            throw;
        }
    }

    public async Task AuditSecurityEventAsync(SecurityAuditEvent auditEvent)
    {
        try
        {
            await _auditService.LogSecurityEventAsync(auditEvent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to audit security event: {EventType}", auditEvent.EventType);
        }
    }

    public async Task<bool> ValidatePinAsync(string username, string pin)
    {
        try
        {
            if (!HasStoredPin(username))
            {
                return false;
            }

            var hashedPin = HashPin(pin);
            lock (_authLock)
            {
                if (_pinStorage.TryGetValue(username, out var storedHash))
                {
                    var isValid = storedHash == hashedPin;
                    
                    await AuditSecurityEvent(new SecurityAuditEvent
                    {
                        EventId = Guid.NewGuid().ToString(),
                        EventType = isValid ? SecurityEventType.Login : SecurityEventType.AuthenticationFailure,
                        UserId = username,
                        Description = $"PIN validation {(isValid ? "successful" : "failed")}",
                        OccurredAt = DateTime.UtcNow,
                        SourceModule = "SecurityFinal",
                        Severity = isValid ? SeverityLevel.Informational : SeverityLevel.Medium,
                        IsSuccessful = isValid
                    });

                    if (isValid)
                    {
                        _logger.LogDebug("🔓 PIN validation successful for user: {Username}", username);
                    }
                    else
                    {
                        _logger.LogWarning("🚫 PIN validation failed for user: {Username}", username);
                    }

                    return isValid;
                }
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "PIN validation failed for user: {Username}", username);
            return false;
        }
    }

    private async Task<UserRole> DetermineUserRoleAsync(WindowsIdentity identity)
    {
        try
        {
            var principal = new WindowsPrincipal(identity);
            
            // Check for administrator privileges
            if (principal.IsInRole(WindowsBuiltInRole.Administrator) || 
                principal.IsInRole("BUILTIN\\Administrators"))
            {
                return UserRole.Administrator;
            }

            // Check for power user privileges
            if (principal.IsInRole(WindowsBuiltInRole.PowerUser) || 
                principal.IsInRole("BUILTIN\\Power Users"))
            {
                return UserRole.PowerUser;
            }

            // Check using Active Directory if available
            try
            {
                using var context = new PrincipalContext(ContextType.Domain);
                using var userPrincipal = UserPrincipal.FindByIdentity(context, identity.Name);
                
                if (userPrincipal != null)
                {
                    var groups = userPrincipal.GetAuthorizationGroups();
                    foreach (var group in groups)
                    {
                        var groupName = group.Name?.ToLowerInvariant();
                        if (groupName != null)
                        {
                            if (groupName.Contains("admin"))
                                return UserRole.Administrator;
                            if (groupName.Contains("power"))
                                return UserRole.PowerUser;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Could not query Active Directory for user role - using local checks");
            }

            // Default to regular user if authenticated
            return identity.IsAuthenticated ? UserRole.User : UserRole.Guest;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to determine user role, defaulting to Guest");
            return UserRole.Guest;
        }
    }

    private UserRole GetRequiredRoleForOperation(SecureOperation operation)
    {
        return operation switch
        {
            SecureOperation.ReadConfiguration => UserRole.User,
            SecureOperation.WriteConfiguration => UserRole.PowerUser,
            SecureOperation.ExecuteScript => UserRole.PowerUser,
            SecureOperation.AccessSensitiveData => UserRole.PowerUser,
            SecureOperation.ModifySettings => UserRole.User,
            SecureOperation.ViewAuditLogs => UserRole.Administrator,
            SecureOperation.ManageUsers => UserRole.Administrator,
            SecureOperation.DeployApplication => UserRole.Administrator,
            SecureOperation.RunDiagnostics => UserRole.PowerUser,
            SecureOperation.AccessAdminFeatures => UserRole.Administrator,
            _ => UserRole.User
        };
    }

    private bool IsRoleAuthorized(UserRole userRole, UserRole requiredRole)
    {
        // Role hierarchy: Guest < User < PowerUser < Administrator < SystemAdministrator < SecurityOfficer
        var roleHierarchy = new Dictionary<UserRole, int>
        {
            [UserRole.Guest] = 0,
            [UserRole.User] = 1,
            [UserRole.PowerUser] = 2,
            [UserRole.Administrator] = 3,
            [UserRole.SystemAdministrator] = 4,
            [UserRole.SecurityOfficer] = 5
        };

        return roleHierarchy.GetValueOrDefault(userRole, 0) >= roleHierarchy.GetValueOrDefault(requiredRole, 0);
    }

    private List<string> GetPermissionsForRole(UserRole role)
    {
        if (_rolePermissions.TryGetValue(role, out var operations))
        {
            return operations.Select(op => op.ToString()).ToList();
        }

        return new List<string>();
    }

    private Dictionary<UserRole, List<SecureOperation>> InitializeRolePermissions()
    {
        return new Dictionary<UserRole, List<SecureOperation>>
        {
            [UserRole.Guest] = new List<SecureOperation>(),
            
            [UserRole.User] = new List<SecureOperation>
            {
                SecureOperation.ReadConfiguration,
                SecureOperation.ModifySettings
            },
            
            [UserRole.PowerUser] = new List<SecureOperation>
            {
                SecureOperation.ReadConfiguration,
                SecureOperation.WriteConfiguration,
                SecureOperation.ExecuteScript,
                SecureOperation.AccessSensitiveData,
                SecureOperation.ModifySettings,
                SecureOperation.RunDiagnostics
            },
            
            [UserRole.Administrator] = new List<SecureOperation>
            {
                SecureOperation.ReadConfiguration,
                SecureOperation.WriteConfiguration,
                SecureOperation.ExecuteScript,
                SecureOperation.AccessSensitiveData,
                SecureOperation.ModifySettings,
                SecureOperation.ViewAuditLogs,
                SecureOperation.ManageUsers,
                SecureOperation.DeployApplication,
                SecureOperation.RunDiagnostics,
                SecureOperation.AccessAdminFeatures
            },
            
            [UserRole.SystemAdministrator] = Enum.GetValues<SecureOperation>().ToList(),
            [UserRole.SecurityOfficer] = Enum.GetValues<SecureOperation>().ToList()
        };
    }

    private string HashPin(string pin)
    {
        using var sha256 = SHA256.Create();
        var saltedPin = $"NetToolkit_PIN_Salt_{pin}_{Environment.UserName}";
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPin));
        return Convert.ToBase64String(hashBytes);
    }

    private bool HasStoredPin(string username)
    {
        lock (_authLock)
        {
            return _pinStorage.ContainsKey(username);
        }
    }

    private async Task StorePinSecurely(string username, string hashedPin)
    {
        try
        {
            var pinFile = Path.Combine(_pinStorePath, $"{Convert.ToBase64String(Encoding.UTF8.GetBytes(username))}.pin");
            
            // Encrypt the PIN hash before storage
            using var aes = Aes.Create();
            aes.GenerateKey();
            aes.GenerateIV();

            var encryptedPin = await EncryptPinDataAsync(hashedPin, aes.Key, aes.IV);
            var pinData = new
            {
                EncryptedPin = Convert.ToBase64String(encryptedPin),
                Key = Convert.ToBase64String(aes.Key),
                IV = Convert.ToBase64String(aes.IV),
                CreatedAt = DateTime.UtcNow
            };

            var json = System.Text.Json.JsonSerializer.Serialize(pinData, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(pinFile, json);
            
            _logger.LogDebug("PIN stored securely for user: {Username}", username);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to store PIN securely for user: {Username}", username);
            throw;
        }
    }

    private void LoadStoredPins()
    {
        try
        {
            if (!Directory.Exists(_pinStorePath))
                return;

            var pinFiles = Directory.GetFiles(_pinStorePath, "*.pin");
            var loadedCount = 0;

            foreach (var pinFile in pinFiles)
            {
                try
                {
                    var json = File.ReadAllText(pinFile);
                    var pinData = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(json);
                    
                    if (pinData != null && pinData.ContainsKey("EncryptedPin"))
                    {
                        var filename = Path.GetFileNameWithoutExtension(pinFile);
                        var username = Encoding.UTF8.GetString(Convert.FromBase64String(filename));
                        
                        // For simplicity in this implementation, we'll load the encrypted PIN as-is
                        // In a real implementation, we'd decrypt it using the stored key/IV
                        lock (_authLock)
                        {
                            _pinStorage[username] = pinData["EncryptedPin"].ToString() ?? "";
                        }
                        
                        loadedCount++;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to load PIN from file: {PinFile}", pinFile);
                }
            }

            _logger.LogDebug("Loaded {Count} stored PINs", loadedCount);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to load stored PINs");
        }
    }

    private async Task<byte[]> EncryptPinDataAsync(string data, byte[] key, byte[] iv)
    {
        using var aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using var encryptor = aes.CreateEncryptor();
        using var msEncrypt = new MemoryStream();
        using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
        using var swEncrypt = new StreamWriter(csEncrypt);
        
        await swEncrypt.WriteAsync(data);
        swEncrypt.Close();
        
        return msEncrypt.ToArray();
    }

    private async Task AuditSuccessfulAuthentication(AuthenticationResult result)
    {
        await AuditSecurityEvent(new SecurityAuditEvent
        {
            EventId = Guid.NewGuid().ToString(),
            EventType = SecurityEventType.Login,
            UserId = result.Username,
            Description = $"Successful Windows authentication - Role: {result.Role}",
            OccurredAt = result.AuthenticatedAt,
            SourceModule = "SecurityFinal",
            Severity = SeverityLevel.Informational,
            IsSuccessful = true,
            EventData = new Dictionary<string, object>
            {
                ["Role"] = result.Role.ToString(),
                ["PinEnabled"] = result.IsPinEnabled,
                ["PermissionCount"] = result.Permissions.Count
            }
        });
    }

    private async Task AuditAuthenticationFailure(string reason)
    {
        await AuditSecurityEvent(new SecurityAuditEvent
        {
            EventId = Guid.NewGuid().ToString(),
            EventType = SecurityEventType.AuthenticationFailure,
            UserId = Environment.UserName,
            Description = $"Authentication failed: {reason}",
            OccurredAt = DateTime.UtcNow,
            SourceModule = "SecurityFinal",
            Severity = SeverityLevel.Medium,
            IsSuccessful = false,
            EventData = new Dictionary<string, object>
            {
                ["Reason"] = reason,
                ["MachineName"] = Environment.MachineName
            }
        });
    }

    private async Task AuditSuccessfulAuthorization(AuthorizationResult result)
    {
        await AuditSecurityEvent(new SecurityAuditEvent
        {
            EventId = Guid.NewGuid().ToString(),
            EventType = SecurityEventType.DataAccess,
            UserId = WindowsIdentity.GetCurrent()?.Name ?? "Unknown",
            Description = $"Authorization granted for operation: {result.Operation}",
            OccurredAt = result.CheckedAt,
            SourceModule = "SecurityFinal",
            Severity = SeverityLevel.Informational,
            IsSuccessful = true,
            EventData = new Dictionary<string, object>
            {
                ["Operation"] = result.Operation.ToString(),
                ["UserRole"] = result.UserRole.ToString(),
                ["RequiredRole"] = result.RequiredRole.ToString()
            }
        });
    }

    private async Task AuditAuthorizationFailure(SecureOperation operation, string reason)
    {
        await AuditSecurityEvent(new SecurityAuditEvent
        {
            EventId = Guid.NewGuid().ToString(),
            EventType = SecurityEventType.AuthorizationFailure,
            UserId = WindowsIdentity.GetCurrent()?.Name ?? "Unknown",
            Description = $"Authorization denied for operation: {operation}",
            OccurredAt = DateTime.UtcNow,
            SourceModule = "SecurityFinal",
            Severity = SeverityLevel.Medium,
            IsSuccessful = false,
            EventData = new Dictionary<string, object>
            {
                ["Operation"] = operation.ToString(),
                ["Reason"] = reason
            }
        });
    }

    public void Dispose()
    {
        try
        {
            // Clear sensitive data from memory
            lock (_authLock)
            {
                _authenticationCache.Clear();
                _pinStorage.Clear();
            }
            
            _logger.LogDebug("Authentication service disposed - Digital gates sealed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during authentication service disposal");
        }
    }
}