using Microsoft.Graph.Models;

namespace NetToolkit.Modules.PowerShell.Interfaces;

/// <summary>
/// Microsoft integration service - your gateway to the Office 365 kingdom
/// </summary>
public interface IMicrosoftIntegrationService
{
    /// <summary>
    /// Initialize Graph API connection - opening the gates to Microsoft's realm
    /// </summary>
    Task<bool> InitializeAsync(string tenantId, string clientId, string clientSecret);
    
    /// <summary>
    /// Get user by email - finding needles in the Office 365 haystack
    /// </summary>
    Task<User?> GetUserAsync(string email);
    
    /// <summary>
    /// Extend mailbox quota - because everyone needs more space
    /// </summary>
    Task<bool> ExtendMailboxAsync(string userEmail, int quotaInGB);
    
    /// <summary>
    /// Convert mailbox to shared - sharing is caring, after all
    /// </summary>
    Task<bool> ConvertToSharedMailboxAsync(string userEmail);
    
    /// <summary>
    /// Add email alias - giving identities secret alter egos
    /// </summary>
    Task<bool> AddEmailAliasAsync(string userEmail, string alias);
    
    /// <summary>
    /// Remove email alias - because sometimes alter egos outlive their usefulness
    /// </summary>
    Task<bool> RemoveEmailAliasAsync(string userEmail, string alias);
    
    /// <summary>
    /// Create distribution group - assembling the digital Avengers
    /// </summary>
    Task<Group?> CreateDistributionGroupAsync(string groupName, string description, List<string> members);
    
    /// <summary>
    /// Add user to group - welcome to the club!
    /// </summary>
    Task<bool> AddUserToGroupAsync(string groupId, string userEmail);
    
    /// <summary>
    /// Set user password - the keys to the digital kingdom
    /// </summary>
    Task<bool> SetUserPasswordAsync(string userEmail, string newPassword, bool forceChangeNextLogin = true);
    
    /// <summary>
    /// Enable/disable user account - the power of digital life and death
    /// </summary>
    Task<bool> SetUserAccountStatusAsync(string userEmail, bool enabled);
    
    /// <summary>
    /// Get user's mailbox statistics - knowledge is power
    /// </summary>
    Task<MailboxStats?> GetMailboxStatsAsync(string userEmail);
    
    /// <summary>
    /// Get tenant information - knowing thy domain
    /// </summary>
    Task<TenantInfo?> GetTenantInfoAsync();
    
    /// <summary>
    /// Test connection - is anybody out there?
    /// </summary>
    Task<bool> TestConnectionAsync();
}

/// <summary>
/// Mailbox statistics - the vital signs of digital communication
/// </summary>
public class MailboxStats
{
    public string UserEmail { get; set; } = string.Empty;
    public long TotalItemSizeInBytes { get; set; }
    public int ItemCount { get; set; }
    public long QuotaUsedInBytes { get; set; }
    public long QuotaLimitInBytes { get; set; }
    public DateTime LastLogon { get; set; }
    public List<string> Aliases { get; set; } = new();
    public bool IsSharedMailbox { get; set; }
    
    /// <summary>
    /// Get quota usage percentage - because percentages are more dramatic
    /// </summary>
    public double GetQuotaUsagePercentage()
    {
        return QuotaLimitInBytes > 0 ? (double)QuotaUsedInBytes / QuotaLimitInBytes * 100 : 0;
    }
    
    /// <summary>
    /// Get human-readable size - because bytes are for machines
    /// </summary>
    public string GetHumanReadableSize()
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        double len = TotalItemSizeInBytes;
        int order = 0;
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len = len / 1024;
        }
        return $"{len:0.##} {sizes[order]}";
    }
}

/// <summary>
/// Tenant information - the who's who of your Office 365 realm
/// </summary>
public class TenantInfo
{
    public string TenantId { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string PrimaryDomain { get; set; } = string.Empty;
    public List<string> VerifiedDomains { get; set; } = new();
    public string Country { get; set; } = string.Empty;
    public string CountryLetterCode { get; set; } = string.Empty;
    public string TenantType { get; set; } = string.Empty;
    public int UserCount { get; set; }
    public DateTime CreatedDateTime { get; set; }
}

/// <summary>
/// Microsoft Graph operation result - success or the reasons for digital despair
/// </summary>
public class GraphOperationResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string ErrorCode { get; set; } = string.Empty;
    public Dictionary<string, object> AdditionalData { get; set; } = new();
    
    /// <summary>
    /// Generate witty status message based on operation result
    /// </summary>
    public string GetWittyStatusMessage(string operation)
    {
        return Success switch
        {
            true => operation switch
            {
                "ExtendMailbox" => "Mailbox expanded! More room for digital hoarding! 📦",
                "ConvertToShared" => "Mailbox is now shared! Communism in the digital age! 🤝",
                "AddAlias" => "Alias added! Your digital alter ego is born! 🎭",
                "CreateGroup" => "Group created! The digital fellowship is formed! 👥",
                _ => $"{operation} completed with the grace of a thousand angels! ✨"
            },
            false => operation switch
            {
                "ExtendMailbox" => "Mailbox extension failed! Perhaps it's on a digital diet? 🍎",
                "ConvertToShared" => "Conversion failed! The mailbox is being antisocial! 😤",
                "AddAlias" => "Alias addition failed! Identity crisis in progress! 🤔",
                _ => $"{operation} stumbled over Microsoft's digital red tape! 📋"
            }
        };
    }
}