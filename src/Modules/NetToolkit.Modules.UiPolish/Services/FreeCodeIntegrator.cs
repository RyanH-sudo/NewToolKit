using NetToolkit.Modules.UiPolish.Models;
using NetToolkit.Modules.UiPolish.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Text;

namespace NetToolkit.Modules.UiPolish.Services;

/// <summary>
/// Free code integrator for external JavaScript libraries
/// Provides secure integration of three-globe, xterm.js, particle.js and other enhancement libraries
/// </summary>
public class FreeCodeIntegrator : IFreeCodeIntegrator
{
    private readonly ILogger<FreeCodeIntegrator> _logger;
    private readonly HttpClient _httpClient;
    private readonly Dictionary<string, LibraryDefinition> _knownLibraries;
    private readonly Dictionary<string, string> _libraryCache;
    private readonly HashSet<string> _securityPatterns;

    public FreeCodeIntegrator(ILogger<FreeCodeIntegrator> logger, HttpClient httpClient)
    {
        _logger = logger;
        _httpClient = httpClient;
        _knownLibraries = InitializeKnownLibraries();
        _libraryCache = new Dictionary<string, string>();
        _securityPatterns = InitializeSecurityPatterns();
    }

    public async Task<IntegrationResult> IntegrateThreeGlobeAsync(GlobeConfig globeConfig)
    {
        try
        {
            _logger.LogInformation("Integrating three-globe library for 3D network visualization");

            var threeGlobeLib = await LoadExternalLibraryAsync("three-globe", "2.31.0");
            if (!threeGlobeLib.Success)
            {
                return new IntegrationResult 
                { 
                    Success = false, 
                    ErrorMessage = "Failed to load three-globe library",
                    Code = GenerateThreeGlobeFallback()
                };
            }

            var integrationCode = GenerateThreeGlobeIntegration(globeConfig);
            var validationResult = await ValidateExternalCodeAsync(integrationCode);

            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Three-globe integration code failed validation: {Issues}", 
                    string.Join(", ", validationResult.SecurityIssues));
                return new IntegrationResult 
                { 
                    Success = false, 
                    ErrorMessage = "Integration code failed security validation",
                    Code = GenerateThreeGlobeFallback()
                };
            }

            _logger.LogInformation("🌍 Globe mastery achieved: 3D network visualization ready for cosmic exploration!");

            return new IntegrationResult
            {
                Success = true,
                Code = integrationCode,
                LibraryUrls = new[] { threeGlobeLib.LoadedUrl },
                Dependencies = new[] { "three.js", "three-globe" },
                SetupInstructions = GetThreeGlobeSetupInstructions()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to integrate three-globe library");
            return new IntegrationResult 
            { 
                Success = false, 
                ErrorMessage = ex.Message,
                Code = GenerateThreeGlobeFallback()
            };
        }
    }

    public async Task<IntegrationResult> IntegrateXtermJsAsync(TerminalConfig terminalConfig)
    {
        try
        {
            _logger.LogInformation("Integrating xterm.js for enhanced terminal experiences");

            var xtermLib = await LoadExternalLibraryAsync("xterm", "5.3.0");
            var xtermAddonFitLib = await LoadExternalLibraryAsync("xterm-addon-fit", "0.8.0");

            if (!xtermLib.Success)
            {
                return new IntegrationResult 
                { 
                    Success = false, 
                    ErrorMessage = "Failed to load xterm.js library",
                    Code = GenerateXtermFallback()
                };
            }

            var integrationCode = GenerateXtermIntegration(terminalConfig);
            var validationResult = await ValidateExternalCodeAsync(integrationCode);

            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Xterm.js integration code failed validation: {Issues}", 
                    string.Join(", ", validationResult.SecurityIssues));
                return new IntegrationResult 
                { 
                    Success = false, 
                    ErrorMessage = "Integration code failed security validation",
                    Code = GenerateXtermFallback()
                };
            }

            _logger.LogInformation("💻 Terminal transcendence: xterm.js holographic console experience activated!");

            return new IntegrationResult
            {
                Success = true,
                Code = integrationCode,
                LibraryUrls = new[] { xtermLib.LoadedUrl, xtermAddonFitLib.LoadedUrl },
                Dependencies = new[] { "xterm", "xterm-addon-fit" },
                SetupInstructions = GetXtermSetupInstructions()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to integrate xterm.js library");
            return new IntegrationResult 
            { 
                Success = false, 
                ErrorMessage = ex.Message,
                Code = GenerateXtermFallback()
            };
        }
    }

    public async Task<IntegrationResult> IntegrateParticleJsAsync(ParticleConfig particleConfig)
    {
        try
        {
            _logger.LogInformation("Integrating particle.js for ambient effects");

            var particleLib = await LoadExternalLibraryAsync("particles.js", "2.0.0");
            if (!particleLib.Success)
            {
                return new IntegrationResult 
                { 
                    Success = false, 
                    ErrorMessage = "Failed to load particles.js library",
                    Code = GenerateParticleFallback(particleConfig)
                };
            }

            var integrationCode = GenerateParticleIntegration(particleConfig);
            var validationResult = await ValidateExternalCodeAsync(integrationCode);

            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Particles.js integration code failed validation: {Issues}", 
                    string.Join(", ", validationResult.SecurityIssues));
                return new IntegrationResult 
                { 
                    Success = false, 
                    ErrorMessage = "Integration code failed security validation",
                    Code = GenerateParticleFallback(particleConfig)
                };
            }

            _logger.LogInformation("✨ Particle paradise: Ambient cosmic dust effects swirling to life!");

            return new IntegrationResult
            {
                Success = true,
                Code = integrationCode,
                LibraryUrls = new[] { particleLib.LoadedUrl },
                Dependencies = new[] { "particles.js" },
                SetupInstructions = GetParticleSetupInstructions()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to integrate particles.js library");
            return new IntegrationResult 
            { 
                Success = false, 
                ErrorMessage = ex.Message,
                Code = GenerateParticleFallback(particleConfig)
            };
        }
    }

    public async Task<CodeValidationResult> ValidateExternalCodeAsync(string jsCode)
    {
        try
        {
            var result = new CodeValidationResult { IsValid = true };
            
            // Check for security patterns
            foreach (var pattern in _securityPatterns)
            {
                if (Regex.IsMatch(jsCode, pattern, RegexOptions.IgnoreCase))
                {
                    result.IsValid = false;
                    result.SecurityIssues.Add($"Suspicious pattern detected: {pattern}");
                }
            }

            // Check for eval() usage
            if (Regex.IsMatch(jsCode, @"\beval\s*\(", RegexOptions.IgnoreCase))
            {
                result.IsValid = false;
                result.SecurityIssues.Add("Use of eval() function detected");
            }

            // Check for document.write usage
            if (Regex.IsMatch(jsCode, @"document\.write\s*\(", RegexOptions.IgnoreCase))
            {
                result.IsValid = false;
                result.SecurityIssues.Add("Use of document.write detected");
            }

            // Check for external network requests
            if (Regex.IsMatch(jsCode, @"\b(fetch|XMLHttpRequest|axios|ajax)\s*\(", RegexOptions.IgnoreCase))
            {
                result.SecurityIssues.Add("External network requests detected - verify endpoints");
            }

            // Validate syntax by attempting to parse as JSON (basic check)
            try
            {
                // This is a simplified validation - in production, you'd use a proper JS parser
                var containsBasicSyntax = jsCode.Contains("function") || jsCode.Contains("=>");
                if (!containsBasicSyntax && jsCode.Length > 50)
                {
                    result.SecurityIssues.Add("Code appears to be malformed or obfuscated");
                }
            }
            catch
            {
                result.SecurityIssues.Add("Code syntax validation failed");
            }

            _logger.LogDebug("Code validation completed: {IsValid}, Issues: {IssueCount}", 
                result.IsValid, result.SecurityIssues.Count);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to validate external code");
            return new CodeValidationResult 
            { 
                IsValid = false, 
                SecurityIssues = { "Validation process failed" } 
            };
        }
    }

    public async Task<LibraryLoadResult> LoadExternalLibraryAsync(string libraryName, string version, string? fallbackPath = null)
    {
        try
        {
            _logger.LogDebug("Loading external library: {Library} v{Version}", libraryName, version);

            var cacheKey = $"{libraryName}@{version}";
            if (_libraryCache.ContainsKey(cacheKey))
            {
                _logger.LogDebug("Library {Library} loaded from cache", libraryName);
                return new LibraryLoadResult 
                { 
                    Success = true, 
                    LoadedUrl = _libraryCache[cacheKey],
                    Source = "cache"
                };
            }

            if (_knownLibraries.TryGetValue(libraryName, out var libraryDef))
            {
                var cdnUrl = string.Format(libraryDef.CdnUrlTemplate, version);
                
                try
                {
                    // Verify library exists at CDN
                    var response = await _httpClient.HeadAsync(cdnUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        _libraryCache[cacheKey] = cdnUrl;
                        _logger.LogInformation("📦 Library loaded from CDN: {Library} v{Version}", libraryName, version);
                        
                        return new LibraryLoadResult 
                        { 
                            Success = true, 
                            LoadedUrl = cdnUrl,
                            Source = "cdn",
                            Integrity = await GenerateIntegrityHashAsync(cdnUrl)
                        };
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to load {Library} from CDN, trying fallback", libraryName);
                }

                // Try fallback path if provided
                if (!string.IsNullOrEmpty(fallbackPath) && File.Exists(fallbackPath))
                {
                    _logger.LogInformation("📁 Library loaded from local fallback: {Library}", libraryName);
                    return new LibraryLoadResult 
                    { 
                        Success = true, 
                        LoadedUrl = fallbackPath,
                        Source = "local"
                    };
                }
            }

            _logger.LogWarning("Failed to load library {Library} - not found in CDN or local paths", libraryName);
            return new LibraryLoadResult 
            { 
                Success = false, 
                ErrorMessage = $"Library {libraryName} v{version} not found"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load external library {Library}", libraryName);
            return new LibraryLoadResult 
            { 
                Success = false, 
                ErrorMessage = ex.Message
            };
        }
    }

    private string GenerateThreeGlobeIntegration(GlobeConfig config)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine("// Three.js Globe Integration - Network Visualization");
        sb.AppendLine("(function() {");
        sb.AppendLine("  if (!window.THREE || !window.ThreeGlobe) {");
        sb.AppendLine("    console.error('Three.js or ThreeGlobe not loaded');");
        sb.AppendLine("    return;");
        sb.AppendLine("  }");
        sb.AppendLine();
        
        sb.AppendLine("  const scene = new THREE.Scene();");
        sb.AppendLine("  const camera = new THREE.PerspectiveCamera(75, window.innerWidth / window.innerHeight, 0.1, 1000);");
        sb.AppendLine("  const renderer = new THREE.WebGLRenderer({ alpha: true, antialias: true });");
        sb.AppendLine("  renderer.setSize(window.innerWidth, window.innerHeight);");
        sb.AppendLine("  renderer.setClearColor(0x000000, 0);");
        sb.AppendLine();
        
        sb.AppendLine("  // Create globe");
        sb.AppendLine("  const globe = new ThreeGlobe()");
        sb.AppendLine($"    .globeImageUrl('//unpkg.com/three-globe/example/img/earth-blue-marble.jpg')");
        sb.AppendLine($"    .backgroundColor('rgba(0,0,0,0)');");
        sb.AppendLine();
        
        // Add network nodes if configured
        if (config.ShowNetworkNodes)
        {
            sb.AppendLine("  // Network nodes data");
            sb.AppendLine("  const nodesData = [");
            sb.AppendLine("    { lat: 40.7128, lng: -74.0060, name: 'New York', value: 100 },");
            sb.AppendLine("    { lat: 51.5074, lng: -0.1278, name: 'London', value: 80 },");
            sb.AppendLine("    { lat: 35.6762, lng: 139.6503, name: 'Tokyo', value: 90 },");
            sb.AppendLine("    { lat: 37.7749, lng: -122.4194, name: 'San Francisco', value: 75 }");
            sb.AppendLine("  ];");
            sb.AppendLine();
            
            sb.AppendLine("  globe.pointsData(nodesData)");
            sb.AppendLine("    .pointAltitude(d => d.value * 0.01)");
            sb.AppendLine("    .pointColor(() => '#00d4ff')");
            sb.AppendLine("    .pointRadius(0.5);");
            sb.AppendLine();
        }

        sb.AppendLine("  scene.add(globe);");
        sb.AppendLine();
        
        sb.AppendLine("  // Lighting");
        sb.AppendLine("  const ambientLight = new THREE.AmbientLight(0xffffff, 0.3);");
        sb.AppendLine("  scene.add(ambientLight);");
        sb.AppendLine("  const directionalLight = new THREE.DirectionalLight(0xffffff, 0.8);");
        sb.AppendLine("  directionalLight.position.set(1, 1, 1);");
        sb.AppendLine("  scene.add(directionalLight);");
        sb.AppendLine();
        
        sb.AppendLine("  camera.position.z = 300;");
        sb.AppendLine();
        
        sb.AppendLine("  function animate() {");
        sb.AppendLine("    requestAnimationFrame(animate);");
        sb.AppendLine("    globe.rotation.y += 0.005;");
        sb.AppendLine("    renderer.render(scene, camera);");
        sb.AppendLine("  }");
        sb.AppendLine("  animate();");
        sb.AppendLine();
        
        sb.AppendLine("  // Handle window resize");
        sb.AppendLine("  window.addEventListener('resize', () => {");
        sb.AppendLine("    camera.aspect = window.innerWidth / window.innerHeight;");
        sb.AppendLine("    camera.updateProjectionMatrix();");
        sb.AppendLine("    renderer.setSize(window.innerWidth, window.innerHeight);");
        sb.AppendLine("  });");
        sb.AppendLine("})();");

        return sb.ToString();
    }

    private string GenerateXtermIntegration(TerminalConfig config)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine("// Xterm.js Terminal Enhancement");
        sb.AppendLine("(function() {");
        sb.AppendLine("  if (!window.Terminal) {");
        sb.AppendLine("    console.error('Xterm.js not loaded');");
        sb.AppendLine("    return;");
        sb.AppendLine("  }");
        sb.AppendLine();
        
        sb.AppendLine("  const term = new Terminal({");
        sb.AppendLine($"    fontSize: {config.FontSize},");
        sb.AppendLine($"    fontFamily: '{config.FontFamily}',");
        sb.AppendLine("    theme: {");
        sb.AppendLine("      background: '#1a1a2e',");
        sb.AppendLine("      foreground: '#c4c4c4',");
        sb.AppendLine("      cursor: '#00d4ff',");
        sb.AppendLine("      selection: 'rgba(0, 212, 255, 0.3)'");
        sb.AppendLine("    },");
        sb.AppendLine("    cursorBlink: true,");
        sb.AppendLine("    cursorStyle: 'block'");
        sb.AppendLine("  });");
        sb.AppendLine();
        
        if (config.EnableHolographicEffects)
        {
            sb.AppendLine("  // Holographic terminal effects");
            sb.AppendLine("  const terminalElement = document.getElementById('terminal');");
            sb.AppendLine("  if (terminalElement) {");
            sb.AppendLine("    terminalElement.style.textShadow = '0 0 10px #00d4ff, 0 0 20px #00d4ff, 0 0 30px #00d4ff';");
            sb.AppendLine("    terminalElement.style.background = 'linear-gradient(135deg, rgba(26,26,46,0.95) 0%, rgba(22,33,62,0.95) 100%)';");
            sb.AppendLine("    terminalElement.style.backdropFilter = 'blur(10px)';");
            sb.AppendLine("    terminalElement.style.border = '1px solid rgba(0, 212, 255, 0.5)';");
            sb.AppendLine("    terminalElement.style.borderRadius = '8px';");
            sb.AppendLine("  }");
            sb.AppendLine();
        }
        
        sb.AppendLine("  // Open terminal");
        sb.AppendLine("  const terminalContainer = document.getElementById('terminal-container');");
        sb.AppendLine("  if (terminalContainer) {");
        sb.AppendLine("    term.open(terminalContainer);");
        sb.AppendLine("  }");
        sb.AppendLine();
        
        sb.AppendLine("  // Fit addon");
        sb.AppendLine("  if (window.FitAddon) {");
        sb.AppendLine("    const fitAddon = new FitAddon.FitAddon();");
        sb.AppendLine("    term.loadAddon(fitAddon);");
        sb.AppendLine("    fitAddon.fit();");
        sb.AppendLine("  }");
        sb.AppendLine();
        
        sb.AppendLine("  // Welcome message");
        sb.AppendLine("  term.writeln('\\x1b[36mNetToolkit Terminal Enhanced\\x1b[0m');");
        sb.AppendLine("  term.writeln('\\x1b[32mHolographic mode: ACTIVE\\x1b[0m');");
        sb.AppendLine("  term.writeln('');");
        sb.AppendLine("  term.write('$ ');");
        sb.AppendLine();
        
        sb.AppendLine("  // Basic input handling");
        sb.AppendLine("  let currentLine = '';");
        sb.AppendLine("  term.onData(data => {");
        sb.AppendLine("    if (data === '\\r') {");
        sb.AppendLine("      term.writeln('');");
        sb.AppendLine("      if (currentLine.trim()) {");
        sb.AppendLine("        term.writeln(`Command executed: ${currentLine}`);");
        sb.AppendLine("      }");
        sb.AppendLine("      currentLine = '';");
        sb.AppendLine("      term.write('$ ');");
        sb.AppendLine("    } else if (data === '\\x7F') {");
        sb.AppendLine("      if (currentLine.length > 0) {");
        sb.AppendLine("        currentLine = currentLine.slice(0, -1);");
        sb.AppendLine("        term.write('\\b \\b');");
        sb.AppendLine("      }");
        sb.AppendLine("    } else {");
        sb.AppendLine("      currentLine += data;");
        sb.AppendLine("      term.write(data);");
        sb.AppendLine("    }");
        sb.AppendLine("  });");
        sb.AppendLine("})();");

        return sb.ToString();
    }

    private string GenerateParticleIntegration(ParticleConfig config)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine("// Particles.js Ambient Effects");
        sb.AppendLine("(function() {");
        sb.AppendLine("  if (!window.particlesJS) {");
        sb.AppendLine("    console.error('Particles.js not loaded');");
        sb.AppendLine("    return;");
        sb.AppendLine("  }");
        sb.AppendLine();
        
        sb.AppendLine("  particlesJS('particles-js', {");
        sb.AppendLine("    particles: {");
        sb.AppendLine("      number: {");
        sb.AppendLine($"        value: {config.ParticleCount},");
        sb.AppendLine("        density: {");
        sb.AppendLine("          enable: true,");
        sb.AppendLine("          value_area: 800");
        sb.AppendLine("        }");
        sb.AppendLine("      },");
        sb.AppendLine("      color: {");
        sb.AppendLine($"        value: '#{config.Color:X6}'");
        sb.AppendLine("      },");
        sb.AppendLine("      shape: {");
        sb.AppendLine($"        type: '{config.Shape.ToString().ToLower()}',");
        sb.AppendLine("        stroke: {");
        sb.AppendLine("          width: 0,");
        sb.AppendLine("          color: '#000000'");
        sb.AppendLine("        }");
        sb.AppendLine("      },");
        sb.AppendLine("      opacity: {");
        sb.AppendLine($"        value: {config.Opacity:F2},");
        sb.AppendLine("        random: true,");
        sb.AppendLine("        anim: {");
        sb.AppendLine("          enable: true,");
        sb.AppendLine("          speed: 1,");
        sb.AppendLine("          opacity_min: 0.1,");
        sb.AppendLine("          sync: false");
        sb.AppendLine("        }");
        sb.AppendLine("      },");
        sb.AppendLine("      size: {");
        sb.AppendLine($"        value: {config.Size:F1},");
        sb.AppendLine("        random: true,");
        sb.AppendLine("        anim: {");
        sb.AppendLine("          enable: false");
        sb.AppendLine("        }");
        sb.AppendLine("      },");
        sb.AppendLine("      line_linked: {");
        sb.AppendLine($"        enable: {config.ConnectParticles.ToString().ToLower()},");
        sb.AppendLine("        distance: 150,");
        sb.AppendLine("        color: '#00d4ff',");
        sb.AppendLine("        opacity: 0.4,");
        sb.AppendLine("        width: 1");
        sb.AppendLine("      },");
        sb.AppendLine("      move: {");
        sb.AppendLine("        enable: true,");
        sb.AppendLine($"        speed: {config.Speed:F1},");
        sb.AppendLine("        direction: 'none',");
        sb.AppendLine("        random: false,");
        sb.AppendLine("        straight: false,");
        sb.AppendLine("        out_mode: 'out',");
        sb.AppendLine("        bounce: false");
        sb.AppendLine("      }");
        sb.AppendLine("    },");
        sb.AppendLine("    interactivity: {");
        sb.AppendLine("      detect_on: 'canvas',");
        sb.AppendLine("      events: {");
        sb.AppendLine("        onhover: {");
        sb.AppendLine("          enable: true,");
        sb.AppendLine("          mode: 'repulse'");
        sb.AppendLine("        },");
        sb.AppendLine("        onclick: {");
        sb.AppendLine("          enable: true,");
        sb.AppendLine("          mode: 'push'");
        sb.AppendLine("        },");
        sb.AppendLine("        resize: true");
        sb.AppendLine("      }");
        sb.AppendLine("    },");
        sb.AppendLine("    retina_detect: true");
        sb.AppendLine("  });");
        sb.AppendLine("})();");

        return sb.ToString();
    }

    private string GenerateThreeGlobeFallback()
    {
        return @"// Three-Globe Fallback - Simple 3D sphere
(function() {
  console.log('Three-Globe fallback mode activated');
  if (!window.THREE) return;
  
  const scene = new THREE.Scene();
  const camera = new THREE.PerspectiveCamera(75, window.innerWidth / window.innerHeight, 0.1, 1000);
  const renderer = new THREE.WebGLRenderer({ alpha: true });
  renderer.setSize(window.innerWidth, window.innerHeight);
  
  const geometry = new THREE.SphereGeometry(5, 32, 32);
  const material = new THREE.MeshBasicMaterial({ color: 0x00d4ff, wireframe: true });
  const sphere = new THREE.Mesh(geometry, material);
  scene.add(sphere);
  
  camera.position.z = 15;
  
  function animate() {
    requestAnimationFrame(animate);
    sphere.rotation.y += 0.005;
    renderer.render(scene, camera);
  }
  animate();
})();";
    }

    private string GenerateXtermFallback()
    {
        return @"// Xterm.js Fallback - Basic terminal simulation
(function() {
  console.log('Xterm.js fallback mode activated');
  const container = document.getElementById('terminal-container');
  if (container) {
    container.innerHTML = '<div style=""background: #1a1a2e; color: #c4c4c4; padding: 10px; font-family: monospace; border-radius: 5px;"">NetToolkit Terminal (Fallback Mode)<br/>$ Ready for commands...</div>';
  }
})();";
    }

    private string GenerateParticleFallback(ParticleConfig config)
    {
        return @"// Particles.js Fallback - CSS particles
(function() {
  console.log('Particles.js fallback mode activated');
  const container = document.getElementById('particles-js');
  if (container) {
    container.style.background = 'radial-gradient(circle, rgba(0,212,255,0.1) 0%, rgba(26,26,46,0.1) 100%)';
    container.innerHTML = '<div style=""position: absolute; top: 50%; left: 50%; transform: translate(-50%, -50%); color: rgba(0,212,255,0.7); font-size: 12px;"">Particle effects loading...</div>';
  }
})();";
    }

    private async Task<string> GenerateIntegrityHashAsync(string url)
    {
        try
        {
            var response = await _httpClient.GetAsync(url);
            var content = await response.Content.ReadAsByteArrayAsync();
            
            using var sha384 = SHA384.Create();
            var hash = sha384.ComputeHash(content);
            var base64Hash = Convert.ToBase64String(hash);
            
            return $"sha384-{base64Hash}";
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to generate integrity hash for {Url}", url);
            return string.Empty;
        }
    }

    private Dictionary<string, LibraryDefinition> InitializeKnownLibraries()
    {
        return new Dictionary<string, LibraryDefinition>
        {
            ["three"] = new LibraryDefinition
            {
                Name = "three",
                CdnUrlTemplate = "https://unpkg.com/three@{0}/build/three.min.js",
                Description = "Three.js 3D library"
            },
            ["three-globe"] = new LibraryDefinition
            {
                Name = "three-globe",
                CdnUrlTemplate = "https://unpkg.com/three-globe@{0}/dist/three-globe.min.js",
                Description = "3D globe visualization library"
            },
            ["xterm"] = new LibraryDefinition
            {
                Name = "xterm",
                CdnUrlTemplate = "https://unpkg.com/xterm@{0}/lib/xterm.js",
                Description = "Terminal emulator for web browsers"
            },
            ["xterm-addon-fit"] = new LibraryDefinition
            {
                Name = "xterm-addon-fit",
                CdnUrlTemplate = "https://unpkg.com/xterm-addon-fit@{0}/lib/xterm-addon-fit.js",
                Description = "Fit addon for xterm.js"
            },
            ["particles.js"] = new LibraryDefinition
            {
                Name = "particles.js",
                CdnUrlTemplate = "https://unpkg.com/particles.js@{0}/particles.min.js",
                Description = "Lightweight particle animation library"
            }
        };
    }

    private HashSet<string> InitializeSecurityPatterns()
    {
        return new HashSet<string>
        {
            @"<script[^>]*>.*?</script>",
            @"javascript:",
            @"data:text/html",
            @"vbscript:",
            @"onload\s*=",
            @"onerror\s*=",
            @"onclick\s*=",
            @"__proto__",
            @"constructor\s*\[",
            @"Function\s*\(",
            @"setTimeout\s*\(",
            @"setInterval\s*\(",
            @"new\s+Function\s*\(",
            @"import\s*\(",
            @"require\s*\(",
            @"process\.env",
            @"global\.",
            @"window\.location\s*=",
            @"document\.location\s*="
        };
    }

    private string GetThreeGlobeSetupInstructions()
    {
        return @"1. Ensure Three.js is loaded before three-globe
2. Create a container div with ID 'globe-container'
3. Call the integration function after DOM is loaded
4. Globe will auto-rotate and display network nodes";
    }

    private string GetXtermSetupInstructions()
    {
        return @"1. Create container div with ID 'terminal-container'
2. Load xterm.js and fit addon
3. Terminal will initialize with holographic theme
4. Supports basic input/output functionality";
    }

    private string GetParticleSetupInstructions()
    {
        return @"1. Create container div with ID 'particles-js'
2. Load particles.js library
3. Particle system will initialize automatically
4. Interactive hover and click effects enabled";
    }
}

/// <summary>
/// Library definition for known external libraries
/// </summary>
public class LibraryDefinition
{
    public required string Name { get; init; }
    public required string CdnUrlTemplate { get; init; }
    public required string Description { get; init; }
}