using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using NetToolkit.Modules.PowerShell.Interfaces;

namespace NetToolkit.Modules.PowerShell.ViewModels;

/// <summary>
/// Terminal View Model - The digital conductor orchestrating the symphony of UI and backend!
/// Where MVVM meets PowerShell in a dance of elegant separation of concerns.
/// </summary>
public class TerminalViewModel : INotifyPropertyChanged
{
    private readonly IPowerShellHost _powerShellHost;
    private readonly IScriptTemplateService _templateService;
    private string _currentCommand = string.Empty;
    private string _outputBuffer = string.Empty;
    private bool _isSshMode = false;
    private string _currentPrompt = "PS C:\\>";
    private bool _isExecuting = false;
    private PowerShellSessionState _sessionState = new();
    
    public event PropertyChangedEventHandler? PropertyChanged;
    
    // Collections for UI binding - the live data streams
    public ObservableCollection<TerminalOutputLine> OutputLines { get; } = new();
    public ObservableCollection<string> CommandHistory => _powerShellHost.GetCommandHistory();
    public ObservableCollection<ScriptTemplate> AvailableTemplates { get; } = new();
    public ObservableCollection<string> AutoCompleteItems { get; } = new();
    public ObservableCollection<TerminalTip> HoverTips { get; } = new();
    
    // Properties for UI binding - the digital puppet strings
    public string CurrentCommand
    {
        get => _currentCommand;
        set => SetProperty(ref _currentCommand, value);
    }
    
    public string OutputBuffer
    {
        get => _outputBuffer;
        set => SetProperty(ref _outputBuffer, value);
    }
    
    public bool IsSshMode
    {
        get => _isSshMode;
        set => SetProperty(ref _isSshMode, value);
    }
    
    public string CurrentPrompt
    {
        get => _currentPrompt;
        set => SetProperty(ref _currentPrompt, value);
    }
    
    public bool IsExecuting
    {
        get => _isExecuting;
        set => SetProperty(ref _isExecuting, value);
    }
    
    public PowerShellSessionState SessionState
    {
        get => _sessionState;
        set => SetProperty(ref _sessionState, value);
    }
    
    // Three.js integration properties - for the holographic terminal magic
    public string TerminalBorderEffect { get; set; } = "glow";
    public string ParticleEffectConfig { get; set; } = "{}";
    public bool EnableHolographicMode { get; set; } = true;
    
    public TerminalViewModel(IPowerShellHost powerShellHost, IScriptTemplateService templateService)
    {
        _powerShellHost = powerShellHost ?? throw new ArgumentNullException(nameof(powerShellHost));
        _templateService = templateService ?? throw new ArgumentNullException(nameof(templateService));
        
        InitializeAsync();
        InitializeHoverTips();
    }
    
    /// <summary>
    /// Initialize the view model - awakening the UI consciousness
    /// </summary>
    private async void InitializeAsync()
    {
        // Load available templates for UI
        var templates = await _templateService.LoadTemplatesAsync();
        foreach (var template in templates)
        {
            AvailableTemplates.Add(template);
        }
        
        // Start output stream processing
        _ = Task.Run(ProcessOutputStreamAsync);
        
        // Update session state
        UpdateSessionState();
        
        // Add welcome message
        AddOutputLine("Welcome to NetToolkit PowerShell Terminal - Where magic meets command line! ✨", 
                     TerminalOutputType.System, "#00FFFF");
        AddOutputLine("Type 'help' for available commands or select a template from the side panel.", 
                     TerminalOutputType.System, "#888888");
    }
    
    /// <summary>
    /// Execute the current command - launching digital missiles of productivity
    /// </summary>
    public async Task ExecuteCommandAsync()
    {
        if (string.IsNullOrWhiteSpace(CurrentCommand) || IsExecuting)
            return;
        
        var command = CurrentCommand.Trim();
        CurrentCommand = string.Empty;
        
        // Add command to output display
        AddOutputLine($"{CurrentPrompt} {command}", TerminalOutputType.Command, "#FFFF00");
        
        IsExecuting = true;
        
        try
        {
            var result = await _powerShellHost.ExecuteCommandAsync(command);
            
            // Add output to display
            if (!string.IsNullOrEmpty(result.Output))
            {
                foreach (var line in result.Output.Split('\n', StringSplitOptions.RemoveEmptyEntries))
                {
                    AddOutputLine(line, TerminalOutputType.Output, "#FFFFFF");
                }
            }
            
            // Add errors if any
            if (!string.IsNullOrEmpty(result.ErrorOutput))
            {
                foreach (var line in result.ErrorOutput.Split('\n', StringSplitOptions.RemoveEmptyEntries))
                {
                    AddOutputLine(line, TerminalOutputType.Error, "#FF4444");
                }
            }
            
            // Show execution time for performance awareness
            if (result.Duration.TotalSeconds > 1)
            {
                AddOutputLine($"Execution completed in {result.Duration.TotalSeconds:F2} seconds", 
                             TerminalOutputType.System, "#888888");
            }
        }
        catch (Exception ex)
        {
            AddOutputLine($"Command execution failed: {ex.Message}", TerminalOutputType.Error, "#FF4444");
        }
        finally
        {
            IsExecuting = false;
            UpdateSessionState();
        }
    }
    
    /// <summary>
    /// Generate and execute script from template - digital alchemy in action
    /// </summary>
    public async Task ExecuteTemplateAsync(string templateId, Dictionary<string, object> parameters)
    {
        try
        {
            IsExecuting = true;
            
            var template = await _templateService.GetTemplateAsync(templateId);
            if (template == null)
            {
                AddOutputLine($"Template '{templateId}' not found - The spell book is incomplete!", 
                             TerminalOutputType.Error, "#FF4444");
                return;
            }
            
            AddOutputLine($"🧙‍♂️ Generating script from template: {template.Name}", 
                         TerminalOutputType.System, "#00FFFF");
            
            var script = await _templateService.GenerateScriptAsync(templateId, parameters);
            
            AddOutputLine($"⚗️ Script generated successfully! Executing...", 
                         TerminalOutputType.System, "#00FF00");
            
            var result = await _powerShellHost.ExecuteAsync(script, parameters);
            
            // Process results same as regular command execution
            if (!string.IsNullOrEmpty(result.Output))
            {
                foreach (var line in result.Output.Split('\n', StringSplitOptions.RemoveEmptyEntries))
                {
                    AddOutputLine(line, TerminalOutputType.Output, "#FFFFFF");
                }
            }
            
            if (!string.IsNullOrEmpty(result.ErrorOutput))
            {
                foreach (var line in result.ErrorOutput.Split('\n', StringSplitOptions.RemoveEmptyEntries))
                {
                    AddOutputLine(line, TerminalOutputType.Error, "#FF4444");
                }
            }
            
            var statusEmoji = result.Success ? "✅" : "❌";
            var statusMessage = result.Success ? 
                "Template execution completed successfully - Digital magic achieved!" :
                "Template execution failed - The spell fizzled out!";
            
            AddOutputLine($"{statusEmoji} {statusMessage}", 
                         TerminalOutputType.System, result.Success ? "#00FF00" : "#FF4444");
        }
        catch (Exception ex)
        {
            AddOutputLine($"Template execution failed: {ex.Message}", TerminalOutputType.Error, "#FF4444");
        }
        finally
        {
            IsExecuting = false;
        }
    }
    
    /// <summary>
    /// Toggle SSH mode - dimensional portal management
    /// </summary>
    public async Task ToggleSshModeAsync(SshConnectionInfo? connectionInfo = null)
    {
        try
        {
            var newMode = !IsSshMode;
            var result = await _powerShellHost.ToggleSshModeAsync(newMode, connectionInfo);
            
            if (result)
            {
                IsSshMode = newMode;
                var modeText = IsSshMode ? $"SSH ({connectionInfo?.Host})" : "Local";
                var emoji = IsSshMode ? "🌐" : "🏠";
                
                CurrentPrompt = IsSshMode ? $"SSH {connectionInfo?.Host} $" : "PS C:\\>";
                
                AddOutputLine($"{emoji} Terminal mode changed to: {modeText}", 
                             TerminalOutputType.System, "#00FFFF");
                
                UpdateSessionState();
            }
            else
            {
                var errorMsg = IsSshMode ? 
                    "Failed to establish SSH connection - The portal remains closed!" :
                    "Failed to disconnect SSH - Trapped in the digital dimension!";
                
                AddOutputLine($"❌ {errorMsg}", TerminalOutputType.Error, "#FF4444");
            }
        }
        catch (Exception ex)
        {
            AddOutputLine($"SSH mode toggle failed: {ex.Message}", TerminalOutputType.Error, "#FF4444");
        }
    }
    
    /// <summary>
    /// Clear terminal output - digital purification ritual
    /// </summary>
    public async Task ClearTerminalAsync()
    {
        OutputLines.Clear();
        OutputBuffer = string.Empty;
        
        await _powerShellHost.ClearAsync();
        
        AddOutputLine("Terminal cleared - Fresh start, clean slate! ✨", 
                     TerminalOutputType.System, "#888888");
    }
    
    /// <summary>
    /// Get auto-complete suggestions - the digital crystal ball
    /// </summary>
    public async Task UpdateAutoCompleteAsync(string partialCommand)
    {
        AutoCompleteItems.Clear();
        
        if (string.IsNullOrWhiteSpace(partialCommand))
            return;
        
        try
        {
            var commands = await _powerShellHost.GetAvailableCommandsAsync();
            var suggestions = commands
                .Where(cmd => cmd.StartsWith(partialCommand, StringComparison.OrdinalIgnoreCase))
                .Take(10)
                .ToList();
            
            foreach (var suggestion in suggestions)
            {
                AutoCompleteItems.Add(suggestion);
            }
        }
        catch (Exception)
        {
            // Silently handle auto-complete failures - it's a nice-to-have feature
        }
    }
    
    /// <summary>
    /// Process output stream from PowerShell host - real-time terminal feeding
    /// </summary>
    private async Task ProcessOutputStreamAsync()
    {
        try
        {
            await foreach (var chunk in _powerShellHost.GetOutputStreamAsync())
            {
                // Note: In a real WPF application, you would use Dispatcher.Invoke
                // For now, we'll handle this synchronously for cross-platform compatibility
                AddOutputLine(chunk.Content, 
                             ConvertOutputType(chunk.Type), 
                             chunk.Color);
            }
        }
        catch (Exception)
        {
            // Handle stream processing errors gracefully
        }
    }
    
    /// <summary>
    /// Add output line to terminal display - feeding the digital stream
    /// </summary>
    private void AddOutputLine(string content, TerminalOutputType type, string color)
    {
        var line = new TerminalOutputLine
        {
            Content = content,
            Type = type,
            Color = color,
            Timestamp = DateTime.UtcNow
        };
        
        OutputLines.Add(line);
        
        // Limit output history to prevent memory bloat
        if (OutputLines.Count > 1000)
        {
            OutputLines.RemoveAt(0);
        }
        
        // Update output buffer for Three.js integration
        UpdateOutputBuffer();
    }
    
    /// <summary>
    /// Update output buffer for Three.js rendering - digital art preparation
    /// </summary>
    private void UpdateOutputBuffer()
    {
        var buffer = string.Join("\n", OutputLines.TakeLast(50).Select(line => 
            $"[{line.Type}] {line.Content}"));
        
        OutputBuffer = buffer;
    }
    
    /// <summary>
    /// Update session state - digital introspection
    /// </summary>
    private void UpdateSessionState()
    {
        SessionState = _powerShellHost.GetSessionState();
    }
    
    /// <summary>
    /// Initialize hover tips - the digital wisdom dispensers
    /// </summary>
    private void InitializeHoverTips()
    {
        HoverTips.Add(new TerminalTip 
        { 
            Element = "ExecuteButton", 
            Text = "Execute command - Unleash the digital magic! ⚡" 
        });
        
        HoverTips.Add(new TerminalTip 
        { 
            Element = "SshToggle", 
            Text = "Toggle SSH mode - Open portals to distant realms! 🌐" 
        });
        
        HoverTips.Add(new TerminalTip 
        { 
            Element = "ClearButton", 
            Text = "Clear terminal - Fresh start for fresh minds! ✨" 
        });
        
        HoverTips.Add(new TerminalTip 
        { 
            Element = "TemplatePanel", 
            Text = "Script templates - Pre-built spells for common tasks! 📜" 
        });
        
        HoverTips.Add(new TerminalTip 
        { 
            Element = "CommandInput", 
            Text = "Command input - Where digital incantations are born! 💭" 
        });
    }
    
    /// <summary>
    /// Convert PowerShell output type to terminal output type - digital translation
    /// </summary>
    private static TerminalOutputType ConvertOutputType(PowerShellOutputType psType)
    {
        return psType switch
        {
            PowerShellOutputType.Information => TerminalOutputType.Output,
            PowerShellOutputType.Warning => TerminalOutputType.Warning,
            PowerShellOutputType.Error => TerminalOutputType.Error,
            PowerShellOutputType.Debug => TerminalOutputType.Debug,
            PowerShellOutputType.Verbose => TerminalOutputType.Verbose,
            PowerShellOutputType.Progress => TerminalOutputType.Progress,
            PowerShellOutputType.Command => TerminalOutputType.Command,
            _ => TerminalOutputType.Output
        };
    }
    
    /// <summary>
    /// Property change notification - MVVM magic
    /// </summary>
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
    /// <summary>
    /// Set property with change notification - the elegant MVVM dance
    /// </summary>
    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;
        
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}

/// <summary>
/// Terminal output line - a single line in the digital scroll
/// </summary>
public class TerminalOutputLine
{
    public string Content { get; set; } = string.Empty;
    public TerminalOutputType Type { get; set; }
    public string Color { get; set; } = "#FFFFFF";
    public DateTime Timestamp { get; set; }
}

/// <summary>
/// Terminal output types - categorizing the digital streams
/// </summary>
public enum TerminalOutputType
{
    Output,     // Regular command output
    Error,      // Error messages
    Warning,    // Warning messages
    Command,    // Commands being executed
    System,     // System messages
    Debug,      // Debug information
    Verbose,    // Verbose output
    Progress    // Progress indicators
}

/// <summary>
/// Terminal tip - wisdom nuggets for UI elements
/// </summary>
public class TerminalTip
{
    public string Element { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
}