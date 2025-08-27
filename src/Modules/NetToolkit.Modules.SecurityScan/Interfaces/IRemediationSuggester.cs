using NetToolkit.Modules.SecurityScan.Models;

namespace NetToolkit.Modules.SecurityScan.Interfaces;

/// <summary>
/// Remediation suggester interface - the wise counselor of digital fortress repair
/// Where vulnerabilities transform into actionable solutions with PowerShell sorcery
/// </summary>
public interface IRemediationSuggester
{
    /// <summary>
    /// Suggest fix for vulnerability - the prescription for digital ailments
    /// </summary>
    /// <param name="vulnerability">Vulnerability requiring remediation</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Remediation suggestion with actionable steps</returns>
    Task<RemediationSuggestion> SuggestFixAsync(VulnerabilityEntry vulnerability, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Generate PowerShell remediation script - crafting the healing incantation
    /// </summary>
    /// <param name="vulnerability">Vulnerability to remediate</param>
    /// <param name="scriptType">Type of remediation script</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>PowerShell script for remediation</returns>
    Task<string> GenerateRemediationScriptAsync(VulnerabilityEntry vulnerability, RemediationScriptType scriptType, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get remediation priority based on vulnerability characteristics
    /// </summary>
    /// <param name="vulnerability">Vulnerability to prioritize</param>
    /// <returns>Priority level for remediation</returns>
    RemediationPriority GetRemediationPriority(VulnerabilityEntry vulnerability);
    
    /// <summary>
    /// Estimate remediation effort and time requirements
    /// </summary>
    /// <param name="vulnerability">Vulnerability to assess</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Effort estimation</returns>
    Task<RemediationEffort> EstimateRemediationEffortAsync(VulnerabilityEntry vulnerability, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get remediation templates for common vulnerability types
    /// </summary>
    /// <param name="category">Vulnerability category</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of applicable templates</returns>
    Task<List<RemediationTemplate>> GetRemediationTemplatesAsync(VulnerabilityCategory category, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Validate remediation script before execution - safety first in digital realm
    /// </summary>
    /// <param name="script">PowerShell script to validate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Validation result with safety assessment</returns>
    Task<ScriptValidationResult> ValidateRemediationScriptAsync(string script, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Generate batch remediation script for multiple vulnerabilities
    /// </summary>
    /// <param name="vulnerabilities">List of vulnerabilities to address</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Comprehensive remediation script</returns>
    Task<string> GenerateBatchRemediationAsync(List<VulnerabilityEntry> vulnerabilities, CancellationToken cancellationToken = default);
}

