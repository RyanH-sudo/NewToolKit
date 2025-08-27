using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NetToolkit.Modules.SshTerminal.Models;
using NetToolkit.Modules.SshTerminal.Interfaces;

namespace NetToolkit.Modules.SshTerminal.Services;

/// <summary>
/// Session manager service - the keeper of digital connection memories
/// Where session states persist beyond the ephemeral nature of digital realms
/// </summary>
public class SessionManager : IDisposable
{
    private readonly ILogger<SessionManager> _logger;
    private readonly ISshEventPublisher _eventPublisher;
    private readonly string _sessionStoragePath;
    
    // In-memory session state cache
    private readonly ConcurrentDictionary<string, SessionState> _sessionStates = new();
    private readonly ConcurrentDictionary<string, DateTime> _lastSaveTimestamps = new();
    
    // Background persistence
    private readonly Timer _persistenceTimer;
    private readonly SemaphoreSlim _saveSemaphore = new(1, 1);
    
    private volatile bool _disposed;
    
    public SessionManager(ILogger<SessionManager> logger, ISshEventPublisher eventPublisher)
    {
        _logger = logger;
        _eventPublisher = eventPublisher;
        
        // Set up session storage directory
        _sessionStoragePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "NetToolkit",
            "Sessions");
        
        Directory.CreateDirectory(_sessionStoragePath);
        
        // Initialize periodic persistence timer (save every 30 seconds)
        _persistenceTimer = new Timer(async _ => await PeriodicSaveAsync(), null, 
            TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30));
        
        // Load existing sessions on startup
        _ = Task.Run(LoadExistingSessionsAsync);
        
        _logger.LogInformation("💾 Session Manager initialized - ready to preserve digital memories at {StoragePath}!",
            _sessionStoragePath);
    }
    
    #region Session State Management
    
    /// <summary>
    /// Create new session state
    /// </summary>
    public async Task<SessionState> CreateSessionStateAsync(ConnectionParams connectionParams)
    {
        var sessionState = new SessionState
        {
            SessionId = connectionParams.SessionId,
            ConnectionParams = connectionParams,
            Status = ConnectionStatus.Disconnected,
            CreatedAt = DateTime.Now
        };
        
        _sessionStates[sessionState.SessionId] = sessionState;
        
        // Schedule immediate save for new sessions
        await SaveSessionStateAsync(sessionState.SessionId);
        
        _logger.LogDebug("📝 Created new session state for {SessionId}: {DisplayName}",
            sessionState.SessionId, connectionParams.DisplayName);
        
        return sessionState;
    }
    
    /// <summary>
    /// Get session state by ID
    /// </summary>
    public async Task<SessionState?> GetSessionStateAsync(string sessionId)
    {
        await Task.CompletedTask;
        
        // Try from cache first
        if (_sessionStates.TryGetValue(sessionId, out var cachedState))
        {
            return cachedState;
        }
        
        // Try loading from disk
        var loadedState = await LoadSessionStateFromDiskAsync(sessionId);
        if (loadedState != null)
        {
            _sessionStates[sessionId] = loadedState;
            return loadedState;
        }
        
        _logger.LogDebug("❓ Session state not found for {SessionId}", sessionId);
        return null;
    }
    
    /// <summary>
    /// Update session state
    /// </summary>
    public async Task UpdateSessionStateAsync(string sessionId, Action<SessionState> updateAction)
    {
        if (!_sessionStates.TryGetValue(sessionId, out var sessionState))
        {
            _logger.LogWarning("⚠️ Attempted to update unknown session state: {SessionId}", sessionId);
            return;
        }
        
        try
        {
            updateAction(sessionState);
            sessionState.LastActivity = DateTime.Now;
            
            // Mark for save
            _lastSaveTimestamps[sessionId] = DateTime.MinValue;
            
            _logger.LogTrace("✏️ Updated session state for {SessionId}", sessionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Error updating session state for {SessionId}", sessionId);
            
            await _eventPublisher.PublishErrorEncounteredAsync(
                sessionId, "Session state update failed", ex, "SessionManager.UpdateSessionStateAsync");
        }
    }
    
    /// <summary>
    /// Add command to session history
    /// </summary>
    public async Task AddCommandToHistoryAsync(string sessionId, CommandHistoryEntry historyEntry)
    {
        await UpdateSessionStateAsync(sessionId, state =>
        {
            state.CommandHistory.Add(historyEntry);
            
            // Limit history size to prevent memory bloat
            const int maxHistorySize = 1000;
            if (state.CommandHistory.Count > maxHistorySize)
            {
                state.CommandHistory.RemoveRange(0, state.CommandHistory.Count - maxHistorySize);
            }
        });
        
        _logger.LogTrace("📜 Added command to history for session {SessionId}: {Command}",
            sessionId, historyEntry.Command);
    }
    
    /// <summary>
    /// Update session connection status
    /// </summary>
    public async Task UpdateSessionStatusAsync(string sessionId, ConnectionStatus newStatus, string? statusMessage = null)
    {
        await UpdateSessionStateAsync(sessionId, state =>
        {
            var oldStatus = state.Status;
            state.Status = newStatus;
            
            if (newStatus == ConnectionStatus.Connected && !state.ConnectedAt.HasValue)
            {
                state.ConnectedAt = DateTime.Now;
            }
            
            if (!string.IsNullOrEmpty(statusMessage))
            {
                state.Metadata["last_status_message"] = statusMessage;
            }
            
            _logger.LogDebug("🔄 Session {SessionId} status: {OldStatus} → {NewStatus}",
                sessionId, oldStatus, newStatus);
        });
    }
    
    /// <summary>
    /// Increment reconnection attempts
    /// </summary>
    public async Task IncrementReconnectAttemptsAsync(string sessionId)
    {
        await UpdateSessionStateAsync(sessionId, state =>
        {
            state.ReconnectAttempts++;
            state.Metadata["last_reconnect_attempt"] = DateTime.Now;
        });
        
        _logger.LogDebug("🔄 Incremented reconnect attempts for session {SessionId}", sessionId);
    }
    
    /// <summary>
    /// Reset reconnection attempts
    /// </summary>
    public async Task ResetReconnectAttemptsAsync(string sessionId)
    {
        await UpdateSessionStateAsync(sessionId, state =>
        {
            state.ReconnectAttempts = 0;
            state.Metadata.Remove("last_reconnect_attempt");
        });
        
        _logger.LogDebug("✅ Reset reconnect attempts for session {SessionId}", sessionId);
    }
    
    /// <summary>
    /// Get all session states
    /// </summary>
    public async Task<List<SessionState>> GetAllSessionStatesAsync()
    {
        await Task.CompletedTask;
        return _sessionStates.Values.ToList();
    }
    
    /// <summary>
    /// Get active sessions (connected or connecting)
    /// </summary>
    public async Task<List<SessionState>> GetActiveSessionStatesAsync()
    {
        var allStates = await GetAllSessionStatesAsync();
        return allStates.Where(s => s.Status == ConnectionStatus.Connected || 
                                  s.Status == ConnectionStatus.Connecting ||
                                  s.Status == ConnectionStatus.Reconnecting).ToList();
    }
    
    /// <summary>
    /// Get session statistics
    /// </summary>
    public async Task<Dictionary<string, object>> GetSessionStatisticsAsync()
    {
        var allStates = await GetAllSessionStatesAsync();
        
        var stats = new Dictionary<string, object>
        {
            ["total_sessions"] = allStates.Count,
            ["active_sessions"] = allStates.Count(s => s.Status == ConnectionStatus.Connected),
            ["failed_sessions"] = allStates.Count(s => s.Status == ConnectionStatus.Failed),
            ["total_commands"] = allStates.Sum(s => s.CommandHistory.Count),
            ["average_session_duration_minutes"] = allStates
                .Where(s => s.ConnectedAt.HasValue)
                .Select(s => (DateTime.Now - s.ConnectedAt.Value).TotalMinutes)
                .DefaultIfEmpty(0)
                .Average(),
            ["connection_types"] = allStates
                .GroupBy(s => s.ConnectionParams.Type)
                .ToDictionary(g => g.Key.ToString(), g => g.Count()),
            ["success_rate"] = allStates.Count > 0 
                ? (double)allStates.Count(s => s.Status == ConnectionStatus.Connected) / allStates.Count 
                : 0.0,
            ["most_active_session"] = allStates
                .OrderByDescending(s => s.CommandHistory.Count)
                .FirstOrDefault()?.SessionId ?? "None"
        };
        
        _logger.LogTrace("📊 Generated session statistics: {TotalSessions} total, {ActiveSessions} active",
            (object?)stats["total_sessions"], (object?)stats["active_sessions"]);
        
        return stats;
    }
    
    #endregion
    
    #region Persistence Operations
    
    /// <summary>
    /// Save session state to disk
    /// </summary>
    public async Task SaveSessionStateAsync(string sessionId)
    {
        if (!_sessionStates.TryGetValue(sessionId, out var sessionState))
        {
            return;
        }
        
        try
        {
            await _saveSemaphore.WaitAsync();
            
            var sessionFilePath = GetSessionFilePath(sessionId);
            var sessionJson = sessionState.Serialize();
            
            await File.WriteAllTextAsync(sessionFilePath, sessionJson);
            _lastSaveTimestamps[sessionId] = DateTime.Now;
            
            _logger.LogTrace("💾 Saved session state to disk: {SessionId}", sessionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Error saving session state for {SessionId}", sessionId);
        }
        finally
        {
            _saveSemaphore.Release();
        }
    }
    
    /// <summary>
    /// Load session state from disk
    /// </summary>
    private async Task<SessionState?> LoadSessionStateFromDiskAsync(string sessionId)
    {
        try
        {
            var sessionFilePath = GetSessionFilePath(sessionId);
            if (!File.Exists(sessionFilePath))
            {
                return null;
            }
            
            var sessionJson = await File.ReadAllTextAsync(sessionFilePath);
            var sessionState = SessionState.Deserialize(sessionJson);
            
            if (sessionState != null)
            {
                _logger.LogTrace("📂 Loaded session state from disk: {SessionId}", sessionId);
            }
            
            return sessionState;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Error loading session state from disk for {SessionId}", sessionId);
            return null;
        }
    }
    
    /// <summary>
    /// Load all existing sessions from disk
    /// </summary>
    private async Task LoadExistingSessionsAsync()
    {
        try
        {
            if (!Directory.Exists(_sessionStoragePath))
            {
                return;
            }
            
            var sessionFiles = Directory.GetFiles(_sessionStoragePath, "*.json");
            var loadedCount = 0;
            
            foreach (var sessionFile in sessionFiles)
            {
                try
                {
                    var fileName = Path.GetFileNameWithoutExtension(sessionFile);
                    var sessionState = await LoadSessionStateFromDiskAsync(fileName);
                    
                    if (sessionState != null)
                    {
                        _sessionStates[sessionState.SessionId] = sessionState;
                        loadedCount++;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "⚠️ Error loading session file {SessionFile}", sessionFile);
                }
            }
            
            _logger.LogInformation("📚 Loaded {LoadedCount} existing sessions from disk", loadedCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Error during bulk session loading");
        }
    }
    
    /// <summary>
    /// Periodic save operation
    /// </summary>
    private async Task PeriodicSaveAsync()
    {
        if (_disposed) return;
        
        try
        {
            var sessionsToSave = new List<string>();
            var now = DateTime.Now;
            
            // Find sessions that need saving (modified since last save)
            foreach (var sessionId in _sessionStates.Keys)
            {
                var lastSave = _lastSaveTimestamps.GetValueOrDefault(sessionId, DateTime.MinValue);
                var sessionState = _sessionStates[sessionId];
                
                if (sessionState.LastActivity > lastSave || 
                    (now - lastSave) > TimeSpan.FromMinutes(5)) // Force save every 5 minutes
                {
                    sessionsToSave.Add(sessionId);
                }
            }
            
            // Save modified sessions
            var saveTasks = sessionsToSave.Select(SaveSessionStateAsync);
            await Task.WhenAll(saveTasks);
            
            if (sessionsToSave.Count > 0)
            {
                _logger.LogTrace("💾 Periodic save completed: {SaveCount} sessions", sessionsToSave.Count);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Error during periodic session save");
        }
    }
    
    /// <summary>
    /// Save all sessions to disk
    /// </summary>
    public async Task SaveAllSessionsAsync()
    {
        try
        {
            var sessionIds = _sessionStates.Keys.ToList();
            var saveTasks = sessionIds.Select(SaveSessionStateAsync);
            
            await Task.WhenAll(saveTasks);
            
            _logger.LogInformation("💾 Saved all {SessionCount} sessions to disk", sessionIds.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Error saving all sessions");
        }
    }
    
    /// <summary>
    /// Delete session state
    /// </summary>
    public async Task DeleteSessionStateAsync(string sessionId)
    {
        try
        {
            // Remove from memory
            _sessionStates.TryRemove(sessionId, out _);
            _lastSaveTimestamps.TryRemove(sessionId, out _);
            
            // Remove from disk
            var sessionFilePath = GetSessionFilePath(sessionId);
            if (File.Exists(sessionFilePath))
            {
                await Task.Run(() => File.Delete(sessionFilePath));
            }
            
            _logger.LogDebug("🗑️ Deleted session state for {SessionId}", sessionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Error deleting session state for {SessionId}", sessionId);
        }
    }
    
    /// <summary>
    /// Clean up old sessions (older than 30 days)
    /// </summary>
    public async Task CleanupOldSessionsAsync(TimeSpan maxAge = default)
    {
        if (maxAge == default)
            maxAge = TimeSpan.FromDays(30);
        
        var cutoffDate = DateTime.Now - maxAge;
        var sessionsToDelete = new List<string>();
        
        foreach (var kvp in _sessionStates)
        {
            var sessionState = kvp.Value;
            if (sessionState.CreatedAt < cutoffDate && 
                (sessionState.LastActivity == null || sessionState.LastActivity < cutoffDate))
            {
                sessionsToDelete.Add(kvp.Key);
            }
        }
        
        foreach (var sessionId in sessionsToDelete)
        {
            await DeleteSessionStateAsync(sessionId);
        }
        
        _logger.LogInformation("🧹 Cleaned up {CleanedCount} old sessions (older than {MaxAge})",
            (object?)sessionsToDelete.Count, (object?)maxAge);
    }
    
    #endregion
    
    #region Helper Methods
    
    private string GetSessionFilePath(string sessionId)
    {
        var safeFileName = string.Join("_", sessionId.Split(Path.GetInvalidFileNameChars()));
        return Path.Combine(_sessionStoragePath, $"{safeFileName}.json");
    }
    
    #endregion
    
    #region Export/Import Operations
    
    /// <summary>
    /// Export session states to JSON
    /// </summary>
    public async Task<string> ExportSessionStatesAsync(List<string>? sessionIds = null)
    {
        try
        {
            var sessionsToExport = sessionIds != null
                ? _sessionStates.Where(kvp => sessionIds.Contains(kvp.Key)).Select(kvp => kvp.Value)
                : _sessionStates.Values;
            
            var exportData = new
            {
                export_timestamp = DateTime.UtcNow,
                version = "1.0",
                session_count = sessionsToExport.Count(),
                sessions = sessionsToExport
            };
            
            var json = JsonConvert.SerializeObject(exportData, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            
            _logger.LogInformation("📤 Exported {SessionCount} session states", exportData.session_count);
            
            return json;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Error exporting session states");
            throw;
        }
    }
    
    /// <summary>
    /// Import session states from JSON
    /// </summary>
    public async Task<int> ImportSessionStatesAsync(string jsonData, bool overwriteExisting = false)
    {
        try
        {
            var importData = JsonConvert.DeserializeObject<dynamic>(jsonData);
            var sessions = JsonConvert.DeserializeObject<List<SessionState>>(
                importData?.sessions?.ToString() ?? "[]");
            
            if (sessions == null) return 0;
            
            var importedCount = 0;
            foreach (var session in sessions)
            {
                if (session == null) continue;
                
                if (!overwriteExisting && _sessionStates.ContainsKey(session.SessionId))
                {
                    _logger.LogDebug("⏭️ Skipping existing session {SessionId}", (object?)session.SessionId);
                    continue;
                }
                
                _sessionStates[session.SessionId] = session;
                await SaveSessionStateAsync(session.SessionId);
                importedCount++;
            }
            
            _logger.LogInformation("📥 Imported {ImportedCount} session states", importedCount);
            return importedCount;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Error importing session states");
            throw;
        }
    }
    
    #endregion
    
    #region IDisposable Implementation
    
    public void Dispose()
    {
        if (_disposed) return;
        
        _disposed = true;
        
        try
        {
            // Save all sessions before disposal
            var saveTask = SaveAllSessionsAsync();
            saveTask.Wait(TimeSpan.FromSeconds(5));
            
            // Dispose resources
            _persistenceTimer?.Dispose();
            _saveSemaphore?.Dispose();
            
            _logger.LogInformation("🧹 Session Manager disposed - digital memories preserved!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Error during Session Manager disposal");
        }
    }
    
    #endregion
}