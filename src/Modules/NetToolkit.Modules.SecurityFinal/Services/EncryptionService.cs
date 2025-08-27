using NetToolkit.Modules.SecurityFinal.Models;
using NetToolkit.Modules.SecurityFinal.Interfaces;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;

namespace NetToolkit.Modules.SecurityFinal.Services;

/// <summary>
/// Comprehensive encryption service using AES with DPAPI key protection
/// The digital vault guardian of NetToolkit's sensitive data
/// </summary>
public class EncryptionService : IEncryptionService
{
    private readonly ILogger<EncryptionService> _logger;
    private readonly Dictionary<string, EncryptionKey> _keyCache;
    private readonly object _keyLock = new();
    private readonly string _keyStorePath;

    // AES-256 with CBC mode for maximum security
    private const int KeySizeBytes = 32; // 256 bits
    private const int IvSizeBytes = 16;  // 128 bits
    private const int SaltSizeBytes = 16; // 128 bits

    public EncryptionService(ILogger<EncryptionService> logger)
    {
        _logger = logger;
        _keyCache = new Dictionary<string, EncryptionKey>();
        _keyStorePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), 
            "NetToolkit", "Security", "Keys");
        
        Directory.CreateDirectory(_keyStorePath);
        InitializeEncryptionSystem();
    }

    public async Task<EncryptedData> EncryptAsync(byte[] data, EncryptionContext context)
    {
        try
        {
            _logger.LogDebug("Encrypting data for purpose: {Purpose}", context.Purpose);

            if (data == null || data.Length == 0)
            {
                throw new ArgumentException("Data cannot be null or empty", nameof(data));
            }

            // Generate or retrieve encryption key
            var encryptionKey = await GetOrGenerateKeyAsync(context);
            
            // Generate random salt and IV
            var salt = GenerateRandomBytes(SaltSizeBytes);
            var iv = GenerateRandomBytes(IvSizeBytes);

            // Derive key from master key using PBKDF2
            var derivedKey = DeriveKey(encryptionKey.Id, salt, KeySizeBytes);

            // Encrypt the data
            var encryptedBytes = await EncryptDataAsync(data, derivedKey, iv);

            var encryptedData = new EncryptedData
            {
                Data = encryptedBytes,
                Salt = salt,
                IV = iv,
                Algorithm = "AES-256-CBC",
                KeySize = 256,
                EncryptedAt = DateTime.UtcNow,
                KeyId = encryptionKey.Id
            };

            // Add metadata
            encryptedData.Metadata["Purpose"] = context.Purpose;
            encryptedData.Metadata["UserId"] = context.UserId;
            encryptedData.Metadata["KeyType"] = context.KeyType.ToString();

            _logger.LogInformation("🔒 Data encrypted successfully: {Purpose} - Digital fortress secured!", context.Purpose);
            return encryptedData;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to encrypt data for purpose: {Purpose}", context.Purpose);
            throw;
        }
    }

    public async Task<byte[]> DecryptAsync(EncryptedData encryptedData, EncryptionContext context)
    {
        try
        {
            _logger.LogDebug("Decrypting data with key: {KeyId}", encryptedData.KeyId);

            if (encryptedData?.Data == null || encryptedData.Data.Length == 0)
            {
                throw new ArgumentException("Encrypted data cannot be null or empty", nameof(encryptedData));
            }

            // Retrieve the encryption key
            var encryptionKey = await GetKeyAsync(encryptedData.KeyId);
            if (encryptionKey == null)
            {
                throw new InvalidOperationException($"Encryption key not found: {encryptedData.KeyId}");
            }

            // Derive the same key used for encryption
            var derivedKey = DeriveKey(encryptionKey.Id, encryptedData.Salt, KeySizeBytes);

            // Decrypt the data
            var decryptedData = await DecryptDataAsync(encryptedData.Data, derivedKey, encryptedData.IV);

            _logger.LogDebug("🔓 Data decrypted successfully for key: {KeyId}", encryptedData.KeyId);
            return decryptedData;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to decrypt data with key: {KeyId}", encryptedData?.KeyId);
            throw;
        }
    }

    public async Task EncryptConfigurationAsync(string configPath)
    {
        try
        {
            _logger.LogInformation("Encrypting configuration file: {ConfigPath}", configPath);

            if (!File.Exists(configPath))
            {
                throw new FileNotFoundException($"Configuration file not found: {configPath}");
            }

            // Read the configuration file
            var configData = await File.ReadAllBytesAsync(configPath);
            
            // Create encryption context
            var context = new EncryptionContext
            {
                Purpose = $"Config:{Path.GetFileName(configPath)}",
                KeyType = KeyType.Configuration,
                UserId = Environment.UserName
            };

            // Encrypt the configuration
            var encryptedConfig = await EncryptAsync(configData, context);

            // Create encrypted file path
            var encryptedPath = configPath + ".encrypted";
            
            // Save encrypted configuration
            await SaveEncryptedDataAsync(encryptedPath, encryptedConfig);

            // Optionally backup original and replace with encrypted version
            var backupPath = configPath + ".backup";
            File.Copy(configPath, backupPath, true);
            
            _logger.LogInformation("🛡️ Configuration encrypted and secured: {ConfigPath} - Secrets now safe from prying eyes!", 
                Path.GetFileName(configPath));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to encrypt configuration file: {ConfigPath}", configPath);
            throw;
        }
    }

    public async Task<EncryptionKey> GenerateKeyAsync(KeyType keyType)
    {
        try
        {
            _logger.LogDebug("Generating encryption key of type: {KeyType}", keyType);

            var keyId = Guid.NewGuid().ToString("N");
            var masterKey = GenerateRandomBytes(KeySizeBytes);

            // Protect the master key using DPAPI (Windows Data Protection API)
            var protectedKey = ProtectData(masterKey);

            var encryptionKey = new EncryptionKey
            {
                Id = keyId,
                Type = keyType,
                Algorithm = "AES-256",
                KeySize = 256,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddYears(1), // Keys expire after 1 year
                CreatedBy = Environment.UserName,
                IsActive = true
            };

            // Store the key securely
            await StoreKeyAsync(encryptionKey, protectedKey);

            lock (_keyLock)
            {
                _keyCache[keyId] = encryptionKey;
            }

            _logger.LogInformation("🔑 Encryption key generated: {KeyType} - Digital keys forged in the cyber forge!", keyType);
            return encryptionKey;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate encryption key of type: {KeyType}", keyType);
            throw;
        }
    }

    public async Task<EncryptionValidation> ValidateEncryptionIntegrityAsync()
    {
        try
        {
            _logger.LogInformation("Validating encryption integrity across all encrypted data");

            var validation = new EncryptionValidation
            {
                IsValid = true,
                ValidatedAt = DateTime.UtcNow
            };

            // Find all encrypted files in the application directory
            var encryptedFiles = Directory.GetFiles(
                Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? ".",
                "*.encrypted", SearchOption.AllDirectories);

            validation.TotalFilesChecked = encryptedFiles.Length;

            foreach (var filePath in encryptedFiles)
            {
                try
                {
                    // Attempt to load and validate the encrypted data structure
                    var encryptedData = await LoadEncryptedDataAsync(filePath);
                    
                    // Verify the key exists
                    var key = await GetKeyAsync(encryptedData.KeyId);
                    var isValid = key != null && key.IsActive && key.ExpiresAt > DateTime.UtcNow;
                    
                    validation.FileValidation[filePath] = isValid;
                    
                    if (isValid)
                    {
                        validation.ValidFiles++;
                    }
                    else
                    {
                        validation.InvalidFiles++;
                        validation.CorruptedFiles.Add(filePath);
                        validation.IsValid = false;
                    }
                }
                catch (Exception ex)
                {
                    validation.FileValidation[filePath] = false;
                    validation.InvalidFiles++;
                    validation.CorruptedFiles.Add(filePath);
                    validation.ValidationErrors.Add($"{filePath}: {ex.Message}");
                    validation.IsValid = false;
                }
            }

            var statusEmoji = validation.IsValid ? "✅" : "⚠️";
            _logger.LogInformation("{Emoji} Encryption validation complete: {ValidFiles}/{TotalFiles} files valid - " +
                "Digital integrity {Status}!", statusEmoji, validation.ValidFiles, validation.TotalFilesChecked,
                validation.IsValid ? "maintained" : "compromised");

            return validation;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to validate encryption integrity");
            return new EncryptionValidation
            {
                IsValid = false,
                ValidationErrors = { ex.Message },
                ValidatedAt = DateTime.UtcNow
            };
        }
    }

    private void InitializeEncryptionSystem()
    {
        try
        {
            _logger.LogDebug("Initializing encryption system");

            // Load existing keys from secure storage
            LoadExistingKeys();

            // Verify DPAPI availability
            if (!IsDataProtectionAvailable())
            {
                _logger.LogWarning("DPAPI not available - using fallback key protection");
            }

            _logger.LogInformation("🔐 Encryption system initialized - Digital vault ready for service!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize encryption system");
            throw;
        }
    }

    private void LoadExistingKeys()
    {
        try
        {
            if (!Directory.Exists(_keyStorePath))
                return;

            var keyFiles = Directory.GetFiles(_keyStorePath, "*.key");
            
            foreach (var keyFile in keyFiles)
            {
                try
                {
                    var keyData = File.ReadAllText(keyFile);
                    var keyInfo = JsonSerializer.Deserialize<EncryptionKey>(keyData);
                    
                    if (keyInfo != null && keyInfo.IsActive && keyInfo.ExpiresAt > DateTime.UtcNow)
                    {
                        lock (_keyLock)
                        {
                            _keyCache[keyInfo.Id] = keyInfo;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to load key from file: {KeyFile}", keyFile);
                }
            }

            _logger.LogDebug("Loaded {KeyCount} encryption keys from storage", _keyCache.Count);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to load existing keys");
        }
    }

    private async Task<EncryptionKey> GetOrGenerateKeyAsync(EncryptionContext context)
    {
        // Try to find an existing key for this context
        var existingKey = FindExistingKey(context);
        if (existingKey != null)
        {
            return existingKey;
        }

        // Generate a new key
        return await GenerateKeyAsync(context.KeyType);
    }

    private EncryptionKey? FindExistingKey(EncryptionContext context)
    {
        lock (_keyLock)
        {
            return _keyCache.Values.FirstOrDefault(key => 
                key.Type == context.KeyType && 
                key.IsActive && 
                key.ExpiresAt > DateTime.UtcNow);
        }
    }

    private async Task<EncryptionKey?> GetKeyAsync(string keyId)
    {
        lock (_keyLock)
        {
            if (_keyCache.TryGetValue(keyId, out var cachedKey))
            {
                return cachedKey;
            }
        }

        // Try to load from storage
        try
        {
            var keyPath = Path.Combine(_keyStorePath, $"{keyId}.key");
            if (File.Exists(keyPath))
            {
                var keyData = await File.ReadAllTextAsync(keyPath);
                var key = JsonSerializer.Deserialize<EncryptionKey>(keyData);
                
                if (key != null)
                {
                    lock (_keyLock)
                    {
                        _keyCache[keyId] = key;
                    }
                    return key;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to load key from storage: {KeyId}", keyId);
        }

        return null;
    }

    private async Task StoreKeyAsync(EncryptionKey key, byte[] protectedKeyData)
    {
        try
        {
            var keyPath = Path.Combine(_keyStorePath, $"{key.Id}.key");
            var keyDataPath = Path.Combine(_keyStorePath, $"{key.Id}.data");

            // Store key metadata as JSON
            var keyJson = JsonSerializer.Serialize(key, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(keyPath, keyJson);

            // Store protected key data separately
            await File.WriteAllBytesAsync(keyDataPath, protectedKeyData);

            _logger.LogDebug("Encryption key stored securely: {KeyId}", key.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to store encryption key: {KeyId}", key.Id);
            throw;
        }
    }

    private async Task<byte[]> GetStoredKeyDataAsync(string keyId)
    {
        try
        {
            var keyDataPath = Path.Combine(_keyStorePath, $"{keyId}.data");
            if (File.Exists(keyDataPath))
            {
                return await File.ReadAllBytesAsync(keyDataPath);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to retrieve stored key data: {KeyId}", keyId);
        }

        throw new InvalidOperationException($"Key data not found: {keyId}");
    }

    private byte[] DeriveKey(string keyId, byte[] salt, int keyLength)
    {
        try
        {
            // Get the protected master key
            var protectedKeyData = GetStoredKeyDataAsync(keyId).Result;
            var masterKey = UnprotectData(protectedKeyData);

            // Use PBKDF2 to derive the actual encryption key
            using var pbkdf2 = new Rfc2898DeriveBytes(masterKey, salt, 100000, HashAlgorithmName.SHA256);
            return pbkdf2.GetBytes(keyLength);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to derive key for: {KeyId}", keyId);
            throw;
        }
    }

    private async Task<byte[]> EncryptDataAsync(byte[] data, byte[] key, byte[] iv)
    {
        using var aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using var encryptor = aes.CreateEncryptor();
        using var msEncrypt = new MemoryStream();
        using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
        
        await csEncrypt.WriteAsync(data, 0, data.Length);
        csEncrypt.FlushFinalBlock();
        
        return msEncrypt.ToArray();
    }

    private async Task<byte[]> DecryptDataAsync(byte[] encryptedData, byte[] key, byte[] iv)
    {
        using var aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using var decryptor = aes.CreateDecryptor();
        using var msDecrypt = new MemoryStream(encryptedData);
        using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
        using var msPlain = new MemoryStream();
        
        await csDecrypt.CopyToAsync(msPlain);
        return msPlain.ToArray();
    }

    private byte[] GenerateRandomBytes(int size)
    {
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[size];
        rng.GetBytes(bytes);
        return bytes;
    }

    private byte[] ProtectData(byte[] data)
    {
        try
        {
            if (IsDataProtectionAvailable())
            {
                return ProtectedData.Protect(data, null, DataProtectionScope.CurrentUser);
            }
            else
            {
                // Fallback: Simple XOR with machine-specific key (not as secure as DPAPI)
                return XorEncrypt(data, GetMachineKey());
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to protect data using DPAPI, using fallback");
            return XorEncrypt(data, GetMachineKey());
        }
    }

    private byte[] UnprotectData(byte[] protectedData)
    {
        try
        {
            if (IsDataProtectionAvailable())
            {
                return ProtectedData.Unprotect(protectedData, null, DataProtectionScope.CurrentUser);
            }
            else
            {
                return XorEncrypt(protectedData, GetMachineKey());
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to unprotect data using DPAPI, using fallback");
            return XorEncrypt(protectedData, GetMachineKey());
        }
    }

    private bool IsDataProtectionAvailable()
    {
        try
        {
            // Test DPAPI availability
            var testData = new byte[] { 1, 2, 3, 4 };
            var protected = ProtectedData.Protect(testData, null, DataProtectionScope.CurrentUser);
            var unprotected = ProtectedData.Unprotect(protected, null, DataProtectionScope.CurrentUser);
            return testData.SequenceEqual(unprotected);
        }
        catch
        {
            return false;
        }
    }

    private byte[] XorEncrypt(byte[] data, byte[] key)
    {
        var result = new byte[data.Length];
        for (int i = 0; i < data.Length; i++)
        {
            result[i] = (byte)(data[i] ^ key[i % key.Length]);
        }
        return result;
    }

    private byte[] GetMachineKey()
    {
        // Generate a machine-specific key based on machine name and user
        var keyData = $"{Environment.MachineName}:{Environment.UserName}:NetToolkitSecurityKey";
        using var sha256 = SHA256.Create();
        return sha256.ComputeHash(Encoding.UTF8.GetBytes(keyData));
    }

    private async Task SaveEncryptedDataAsync(string filePath, EncryptedData encryptedData)
    {
        var json = JsonSerializer.Serialize(encryptedData, new JsonSerializerOptions 
        { 
            WriteIndented = true,
            Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
        });
        
        await File.WriteAllTextAsync(filePath, json);
    }

    private async Task<EncryptedData> LoadEncryptedDataAsync(string filePath)
    {
        var json = await File.ReadAllTextAsync(filePath);
        var encryptedData = JsonSerializer.Deserialize<EncryptedData>(json, new JsonSerializerOptions
        {
            Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
        });
        
        if (encryptedData == null)
        {
            throw new InvalidOperationException($"Failed to deserialize encrypted data from: {filePath}");
        }
        
        return encryptedData;
    }

    public void Dispose()
    {
        try
        {
            // Clear sensitive data from memory
            lock (_keyLock)
            {
                _keyCache.Clear();
            }
            
            _logger.LogDebug("Encryption service disposed - Digital vault sealed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during encryption service disposal");
        }
    }
}