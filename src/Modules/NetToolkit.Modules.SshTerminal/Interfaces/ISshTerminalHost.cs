using NetToolkit.Modules.SshTerminal.Models;

namespace NetToolkit.Modules.SshTerminal.Interfaces;

/// <summary>
/// SSH terminal host interface - the omniscient gateway to digital realms
/// Where connections transcend physical boundaries and terminals become cosmic portals
/// </summary>
public interface ISshTerminalHost
{
    /// <summary>
    /// Connect to a remote or local device - opening the digital bridge to distant lands
    /// </summary>
    /// <param name="parameters">Connection configuration containing all mystical parameters</param>
    /// <param name="cancellationToken">Cancellation token for graceful termination</param>
    /// <returns>Session handle to the established connection realm</returns>
    Task<SessionHandle> ConnectAsync(ConnectionParams parameters, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Send command to active session - dispatching digital incantations across the void
    /// </summary>
    /// <param name="sessionId">Target session identifier</param>
    /// <param name="command">Command to execute in the remote realm</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status of command dispatch</returns>
    Task<bool> SendCommandAsync(string sessionId, string command, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Send raw data to session - bypassing command processing for pure data streams
    /// </summary>
    /// <param name="sessionId">Target session identifier</param>
    /// <param name="data">Raw data bytes to transmit</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Number of bytes successfully transmitted</returns>
    Task<int> SendRawDataAsync(string sessionId, byte[] data, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get output stream from session - the flowing river of digital consciousness
    /// </summary>
    /// <param name="sessionId">Session identifier to monitor</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Async enumerable stream of colored output</returns>
    IAsyncEnumerable<ColoredOutput> GetOutputStreamAsync(string sessionId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get current terminal buffer contents - the memory palace of digital exchanges
    /// </summary>
    /// <param name="sessionId">Session identifier</param>
    /// <param name="lastLines">Number of recent lines to retrieve (0 for all)</param>
    /// <returns>Terminal buffer contents</returns>
    Task<TerminalBuffer> GetTerminalBufferAsync(string sessionId, int lastLines = 0);
    
    /// <summary>
    /// Disconnect from session - gracefully severing the digital tether
    /// </summary>
    /// <param name="sessionId">Session to terminate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if disconnection was successful</returns>
    Task<bool> DisconnectAsync(string sessionId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get all active sessions - census of living digital connections
    /// </summary>
    /// <returns>List of currently active session handles</returns>
    Task<List<SessionHandle>> GetActiveSessionsAsync();
    
    /// <summary>
    /// Get session state - detailed status of a specific digital realm
    /// </summary>
    /// <param name="sessionId">Session to query</param>
    /// <returns>Current session state or null if not found</returns>
    Task<SessionState?> GetSessionStateAsync(string sessionId);
    
    /// <summary>
    /// Reconnect session - resurrection of severed digital bonds
    /// </summary>
    /// <param name="sessionId">Session to resurrect</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status of reconnection attempt</returns>
    Task<bool> ReconnectSessionAsync(string sessionId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Close all sessions - digital apocalypse for cleanup
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Number of sessions successfully closed</returns>
    Task<int> CloseAllSessionsAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Event fired when new data arrives from any session
    /// </summary>
    event EventHandler<DataReceivedEvent>? DataReceived;
    
    /// <summary>
    /// Event fired when a connection is established
    /// </summary>
    event EventHandler<ConnectionEstablishedEvent>? ConnectionEstablished;
    
    /// <summary>
    /// Event fired when a command is executed
    /// </summary>
    event EventHandler<CommandExecutedEvent>? CommandExecuted;
    
    /// <summary>
    /// Event fired when an error occurs
    /// </summary>
    event EventHandler<ErrorEncounteredEvent>? ErrorEncountered;
}