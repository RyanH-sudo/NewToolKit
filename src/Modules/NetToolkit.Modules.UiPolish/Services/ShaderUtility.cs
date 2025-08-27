using NetToolkit.Modules.UiPolish.Models;
using NetToolkit.Modules.UiPolish.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text;

namespace NetToolkit.Modules.UiPolish.Services;

/// <summary>
/// Advanced shader utility for GLSL generation and visual effects
/// Provides sophisticated shader creation for Three.js materials and cyberpunk aesthetics
/// </summary>
public class ShaderUtility : IShaderUtility
{
    private readonly ILogger<ShaderUtility> _logger;
    private readonly Dictionary<ShaderType, ShaderTemplate> _shaderTemplates;

    public ShaderUtility(ILogger<ShaderUtility> logger)
    {
        _logger = logger;
        _shaderTemplates = InitializeShaderTemplates();
    }

    public async Task<ShaderCode> GenerateGlslShaderAsync(ShaderType shaderType, Dictionary<string, object> parameters)
    {
        try
        {
            _logger.LogDebug("Generating GLSL shader: {ShaderType}", shaderType);

            if (!_shaderTemplates.TryGetValue(shaderType, out var template))
            {
                throw new ArgumentException($"Unknown shader type: {shaderType}");
            }

            var vertexShader = ProcessShaderTemplate(template.VertexShader, parameters);
            var fragmentShader = ProcessShaderTemplate(template.FragmentShader, parameters);
            var uniforms = GenerateUniforms(shaderType, parameters);

            var shaderCode = new ShaderCode
            {
                VertexShader = vertexShader,
                FragmentShader = fragmentShader,
                Uniforms = uniforms,
                ShaderType = shaderType,
                GeneratedAt = DateTime.UtcNow
            };

            _logger.LogInformation("✨ GLSL shader generated: {ShaderType} - {VertexLines} vertex lines, {FragmentLines} fragment lines", 
                shaderType, vertexShader.Split('\n').Length, fragmentShader.Split('\n').Length);

            return shaderCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate GLSL shader: {ShaderType}", shaderType);
            return GenerateFallbackShader(shaderType);
        }
    }

    public async Task<string> CreateHolographicMaterialAsync(HolographicConfig hologramConfig)
    {
        try
        {
            _logger.LogDebug("Creating holographic material with config: {Config}", hologramConfig);

            var sb = new StringBuilder();
            sb.AppendLine("// Holographic Material - Cyberpunk Excellence");
            sb.AppendLine("const holographicMaterial = new THREE.ShaderMaterial({");
            sb.AppendLine("  uniforms: {");
            sb.AppendLine($"    time: {{ value: 0.0 }},");
            sb.AppendLine($"    transparency: {{ value: {hologramConfig.Transparency:F2} }},");
            sb.AppendLine($"    glowIntensity: {{ value: {hologramConfig.GlowIntensity:F2} }},");
            sb.AppendLine($"    fresnelPower: {{ value: {hologramConfig.FresnelPower:F2} }},");
            sb.AppendLine($"    colorShift: {{ value: {(hologramConfig.ColorShift ? 1.0 : 0.0)} }},");
            sb.AppendLine($"    rimLighting: {{ value: {(hologramConfig.RimLighting ? 1.0 : 0.0)} }}");
            sb.AppendLine("  },");
            sb.AppendLine();

            // Vertex shader for holographic effect
            sb.AppendLine("  vertexShader: `");
            sb.AppendLine("    varying vec3 vNormal;");
            sb.AppendLine("    varying vec3 vPosition;");
            sb.AppendLine("    varying vec2 vUv;");
            sb.AppendLine("    uniform float time;");
            sb.AppendLine();
            sb.AppendLine("    void main() {");
            sb.AppendLine("      vNormal = normalize(normalMatrix * normal);");
            sb.AppendLine("      vPosition = position;");
            sb.AppendLine("      vUv = uv;");
            sb.AppendLine();
            sb.AppendLine("      // Holographic vertex displacement");
            sb.AppendLine("      vec3 displaced = position;");
            sb.AppendLine("      displaced.y += sin(position.x * 5.0 + time * 2.0) * 0.01;");
            sb.AppendLine("      displaced.x += cos(position.y * 4.0 + time * 1.5) * 0.008;");
            sb.AppendLine();
            sb.AppendLine("      gl_Position = projectionMatrix * modelViewMatrix * vec4(displaced, 1.0);");
            sb.AppendLine("    }");
            sb.AppendLine("  `,");
            sb.AppendLine();

            // Fragment shader for holographic effect
            sb.AppendLine("  fragmentShader: `");
            sb.AppendLine("    uniform float time;");
            sb.AppendLine("    uniform float transparency;");
            sb.AppendLine("    uniform float glowIntensity;");
            sb.AppendLine("    uniform float fresnelPower;");
            sb.AppendLine("    uniform float colorShift;");
            sb.AppendLine("    uniform float rimLighting;");
            sb.AppendLine();
            sb.AppendLine("    varying vec3 vNormal;");
            sb.AppendLine("    varying vec3 vPosition;");
            sb.AppendLine("    varying vec2 vUv;");
            sb.AppendLine();
            sb.AppendLine("    void main() {");
            sb.AppendLine("      // Fresnel effect calculation");
            sb.AppendLine("      vec3 viewDirection = normalize(cameraPosition - vPosition);");
            sb.AppendLine("      float fresnel = 1.0 - dot(vNormal, viewDirection);");
            sb.AppendLine("      fresnel = pow(fresnel, fresnelPower);");
            sb.AppendLine();
            
            sb.AppendLine("      // Holographic color cycling");
            sb.AppendLine("      vec3 baseColor = vec3(0.0, 0.83, 1.0); // Neon cyan");
            sb.AppendLine("      if (colorShift > 0.5) {");
            sb.AppendLine("        float cycle = sin(time * 0.5 + vPosition.y * 2.0) * 0.5 + 0.5;");
            sb.AppendLine("        baseColor = mix(baseColor, vec3(1.0, 0.0, 0.8), cycle * 0.3);");
            sb.AppendLine("      }");
            sb.AppendLine();
            
            sb.AppendLine("      // Scanline effect");
            sb.AppendLine("      float scanlines = sin(vUv.y * 100.0 + time * 5.0) * 0.1 + 0.9;");
            sb.AppendLine();
            
            sb.AppendLine("      // Rim lighting");
            sb.AppendLine("      float rim = 1.0 - dot(vNormal, viewDirection);");
            sb.AppendLine("      rim = pow(rim, 3.0) * rimLighting;");
            sb.AppendLine();
            
            sb.AppendLine("      // Holographic interference pattern");
            sb.AppendLine("      float interference = sin(vPosition.x * 20.0 + time * 3.0) *");
            sb.AppendLine("                          sin(vPosition.y * 15.0 + time * 2.0) * 0.1;");
            sb.AppendLine();
            
            sb.AppendLine("      // Combine effects");
            sb.AppendLine("      vec3 finalColor = baseColor * (fresnel + rim) * scanlines * glowIntensity;");
            sb.AppendLine("      finalColor += interference;");
            sb.AppendLine();
            
            sb.AppendLine("      float alpha = fresnel * transparency + rim * 0.3;");
            sb.AppendLine("      gl_FragColor = vec4(finalColor, alpha);");
            sb.AppendLine("    }");
            sb.AppendLine("  `,");
            sb.AppendLine();
            
            sb.AppendLine("  transparent: true,");
            sb.AppendLine("  blending: THREE.AdditiveBlending,");
            sb.AppendLine("  side: THREE.DoubleSide");
            sb.AppendLine("});");
            sb.AppendLine();
            
            sb.AppendLine("// Animate the holographic material");
            sb.AppendLine("function animateHolographic() {");
            sb.AppendLine("  holographicMaterial.uniforms.time.value = performance.now() * 0.001;");
            sb.AppendLine("  requestAnimationFrame(animateHolographic);");
            sb.AppendLine("}");
            sb.AppendLine("animateHolographic();");

            var result = sb.ToString();
            _logger.LogInformation("🌈 Holographic material created - Cyberpunk aesthetic achieved!");
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create holographic material");
            return GenerateFallbackHolographicMaterial();
        }
    }

    public async Task<GlowEffect> CreateCyberpunkGlowAsync(GlowConfig glowConfig)
    {
        try
        {
            _logger.LogDebug("Creating cyberpunk glow effect with config: {Config}", glowConfig);

            var css = GenerateGlowCss(glowConfig);
            var javascript = GenerateGlowJavaScript(glowConfig);

            var glowEffect = new GlowEffect
            {
                CssCode = css,
                JavaScriptCode = javascript,
                GlowConfig = glowConfig,
                IsAnimated = glowConfig.EnablePulsing || glowConfig.EnableColorCycling,
                CreatedAt = DateTime.UtcNow
            };

            _logger.LogInformation("✨ Cyberpunk glow effect created - Neon magnificence activated!");
            
            return glowEffect;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create cyberpunk glow effect");
            return GenerateFallbackGlowEffect(glowConfig);
        }
    }

    public async Task<ShaderValidationResult> ValidateShaderAsync(string shaderSource)
    {
        try
        {
            _logger.LogDebug("Validating shader source ({Length} characters)", shaderSource.Length);

            var result = new ShaderValidationResult
            {
                IsValid = true,
                ShaderSource = shaderSource,
                ValidationErrors = new List<string>(),
                ValidationWarnings = new List<string>(),
                ValidatedAt = DateTime.UtcNow
            };

            // Check for required GLSL elements
            if (!shaderSource.Contains("void main()"))
            {
                result.IsValid = false;
                result.ValidationErrors.Add("Missing main() function");
            }

            // Check for common GLSL syntax patterns
            if (shaderSource.Contains("gl_Position") && !shaderSource.Contains("vertex"))
            {
                // Likely a vertex shader
                if (!shaderSource.Contains("projectionMatrix") && !shaderSource.Contains("modelViewMatrix"))
                {
                    result.ValidationWarnings.Add("Vertex shader should typically use transformation matrices");
                }
            }

            if (shaderSource.Contains("gl_FragColor") || shaderSource.Contains("gl_FragData"))
            {
                // Likely a fragment shader
                if (!shaderSource.Contains("varying") && !shaderSource.Contains("uniform"))
                {
                    result.ValidationWarnings.Add("Fragment shader should typically receive varying or uniform inputs");
                }
            }

            // Check for deprecated functions
            var deprecatedFunctions = new[] { "texture2D", "texture2DProj", "textureCube" };
            foreach (var func in deprecatedFunctions)
            {
                if (shaderSource.Contains(func))
                {
                    result.ValidationWarnings.Add($"Deprecated function '{func}' detected - consider using 'texture' instead");
                }
            }

            // Check for potential performance issues
            if (shaderSource.Contains("for") && shaderSource.Contains("gl_FragColor"))
            {
                result.ValidationWarnings.Add("Fragment shader contains loops - may impact performance");
            }

            // Check precision qualifiers
            if (!shaderSource.Contains("precision"))
            {
                result.ValidationWarnings.Add("No precision qualifiers found - consider adding for better compatibility");
            }

            // Basic syntax validation
            var openBraces = shaderSource.Count(c => c == '{');
            var closeBraces = shaderSource.Count(c => c == '}');
            if (openBraces != closeBraces)
            {
                result.IsValid = false;
                result.ValidationErrors.Add($"Mismatched braces: {openBraces} opening, {closeBraces} closing");
            }

            var openParens = shaderSource.Count(c => c == '(');
            var closeParens = shaderSource.Count(c => c == ')');
            if (openParens != closeParens)
            {
                result.IsValid = false;
                result.ValidationErrors.Add($"Mismatched parentheses: {openParens} opening, {closeParens} closing");
            }

            _logger.LogInformation("🔍 Shader validation complete - Valid: {IsValid}, Errors: {ErrorCount}, Warnings: {WarningCount}", 
                result.IsValid, result.ValidationErrors.Count, result.ValidationWarnings.Count);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to validate shader");
            return new ShaderValidationResult
            {
                IsValid = false,
                ShaderSource = shaderSource,
                ValidationErrors = new List<string> { $"Validation failed: {ex.Message}" },
                ValidatedAt = DateTime.UtcNow
            };
        }
    }

    private string ProcessShaderTemplate(string template, Dictionary<string, object> parameters)
    {
        var processed = template;

        foreach (var param in parameters)
        {
            var placeholder = $"{{{param.Key}}}";
            var value = param.Value?.ToString() ?? "";
            processed = processed.Replace(placeholder, value);
        }

        return processed;
    }

    private Dictionary<string, UniformValue> GenerateUniforms(ShaderType shaderType, Dictionary<string, object> parameters)
    {
        var uniforms = new Dictionary<string, UniformValue>();

        // Add common uniforms
        uniforms["time"] = new UniformValue { Value = 0.0f, Type = "float" };
        uniforms["resolution"] = new UniformValue { Value = new[] { 1920, 1080 }, Type = "vec2" };

        // Add shader-specific uniforms based on type
        switch (shaderType)
        {
            case ShaderType.Holographic:
                uniforms["transparency"] = new UniformValue { Value = 0.8f, Type = "float" };
                uniforms["glowIntensity"] = new UniformValue { Value = 1.2f, Type = "float" };
                uniforms["fresnelPower"] = new UniformValue { Value = 2.0f, Type = "float" };
                break;

            case ShaderType.Metallic:
                uniforms["metallicFactor"] = new UniformValue { Value = 0.9f, Type = "float" };
                uniforms["roughnessFactor"] = new UniformValue { Value = 0.1f, Type = "float" };
                uniforms["baseColor"] = new UniformValue { Value = new[] { 0.8f, 0.8f, 0.9f }, Type = "vec3" };
                break;

            case ShaderType.Particle:
                uniforms["pointSize"] = new UniformValue { Value = 5.0f, Type = "float" };
                uniforms["particleColor"] = new UniformValue { Value = new[] { 0.0f, 0.8f, 1.0f }, Type = "vec3" };
                break;

            case ShaderType.Glow:
                uniforms["glowColor"] = new UniformValue { Value = new[] { 0.0f, 0.8f, 1.0f }, Type = "vec3" };
                uniforms["glowRadius"] = new UniformValue { Value = 10.0f, Type = "float" };
                break;
        }

        // Add user-specified parameters
        foreach (var param in parameters)
        {
            if (!uniforms.ContainsKey(param.Key))
            {
                var value = param.Value;
                var type = InferUniformType(value);
                uniforms[param.Key] = new UniformValue { Value = value, Type = type };
            }
        }

        return uniforms;
    }

    private string InferUniformType(object value)
    {
        return value switch
        {
            float or double => "float",
            int => "int",
            bool => "bool",
            float[] arr when arr.Length == 2 => "vec2",
            float[] arr when arr.Length == 3 => "vec3",
            float[] arr when arr.Length == 4 => "vec4",
            int[] arr when arr.Length == 2 => "ivec2",
            int[] arr when arr.Length == 3 => "ivec3",
            int[] arr when arr.Length == 4 => "ivec4",
            _ => "float"
        };
    }

    private string GenerateGlowCss(GlowConfig glowConfig)
    {
        var sb = new StringBuilder();
        sb.AppendLine("/* Cyberpunk Glow Effect CSS */");
        sb.AppendLine(".cyberpunk-glow {");
        
        // Base glow effect
        var glowColor = $"rgba({glowConfig.GlowColor.R}, {glowConfig.GlowColor.G}, {glowConfig.GlowColor.B}, {glowConfig.GlowColor.A / 255.0:F2})";
        sb.AppendLine($"  color: {glowColor};");
        
        var shadows = new List<string>();
        for (int i = 1; i <= glowConfig.GlowRadius; i++)
        {
            var intensity = 1.0f - (i / (float)glowConfig.GlowRadius);
            var shadowColor = $"rgba({glowConfig.GlowColor.R}, {glowConfig.GlowColor.G}, {glowConfig.GlowColor.B}, {intensity * glowConfig.Intensity:F2})";
            shadows.Add($"0 0 {i * 2}px {shadowColor}");
        }
        
        sb.AppendLine($"  text-shadow: {string.Join(", ", shadows)};");
        sb.AppendLine($"  filter: drop-shadow(0 0 {glowConfig.GlowRadius}px {glowColor});");
        
        if (glowConfig.EnablePulsing)
        {
            sb.AppendLine("  animation: cyberpunk-pulse 2s ease-in-out infinite alternate;");
        }
        
        if (glowConfig.EnableColorCycling)
        {
            sb.AppendLine("  animation: cyberpunk-color-cycle 4s linear infinite;");
        }
        
        if (glowConfig.EnablePulsing && glowConfig.EnableColorCycling)
        {
            sb.AppendLine("  animation: cyberpunk-pulse 2s ease-in-out infinite alternate, cyberpunk-color-cycle 4s linear infinite;");
        }
        
        sb.AppendLine("}");
        sb.AppendLine();
        
        // Pulse animation
        if (glowConfig.EnablePulsing)
        {
            sb.AppendLine("@keyframes cyberpunk-pulse {");
            sb.AppendLine("  from {");
            sb.AppendLine($"    filter: drop-shadow(0 0 {glowConfig.GlowRadius}px {glowColor});");
            sb.AppendLine("  }");
            sb.AppendLine("  to {");
            sb.AppendLine($"    filter: drop-shadow(0 0 {glowConfig.GlowRadius * 2}px {glowColor});");
            sb.AppendLine("  }");
            sb.AppendLine("}");
            sb.AppendLine();
        }
        
        // Color cycling animation
        if (glowConfig.EnableColorCycling)
        {
            sb.AppendLine("@keyframes cyberpunk-color-cycle {");
            sb.AppendLine("  0% { filter: hue-rotate(0deg); }");
            sb.AppendLine("  25% { filter: hue-rotate(90deg); }");
            sb.AppendLine("  50% { filter: hue-rotate(180deg); }");
            sb.AppendLine("  75% { filter: hue-rotate(270deg); }");
            sb.AppendLine("  100% { filter: hue-rotate(360deg); }");
            sb.AppendLine("}");
        }
        
        return sb.ToString();
    }

    private string GenerateGlowJavaScript(GlowConfig glowConfig)
    {
        var sb = new StringBuilder();
        sb.AppendLine("// Cyberpunk Glow Effect JavaScript");
        sb.AppendLine("(function() {");
        sb.AppendLine("  const glowElements = document.querySelectorAll('.cyberpunk-glow');");
        sb.AppendLine();
        
        sb.AppendLine("  function applyGlowEffect(element) {");
        sb.AppendLine("    element.addEventListener('mouseenter', function() {");
        sb.AppendLine($"      this.style.filter = 'drop-shadow(0 0 {glowConfig.GlowRadius * 1.5}px rgba({glowConfig.GlowColor.R}, {glowConfig.GlowColor.G}, {glowConfig.GlowColor.B}, {glowConfig.Intensity * 1.2:F2}))';");
        sb.AppendLine("    });");
        sb.AppendLine();
        
        sb.AppendLine("    element.addEventListener('mouseleave', function() {");
        sb.AppendLine($"      this.style.filter = 'drop-shadow(0 0 {glowConfig.GlowRadius}px rgba({glowConfig.GlowColor.R}, {glowConfig.GlowColor.G}, {glowConfig.GlowColor.B}, {glowConfig.Intensity:F2}))';");
        sb.AppendLine("    });");
        sb.AppendLine("  }");
        sb.AppendLine();
        
        sb.AppendLine("  glowElements.forEach(applyGlowEffect);");
        sb.AppendLine("})();");
        
        return sb.ToString();
    }

    private Dictionary<ShaderType, ShaderTemplate> InitializeShaderTemplates()
    {
        return new Dictionary<ShaderType, ShaderTemplate>
        {
            [ShaderType.Holographic] = new ShaderTemplate
            {
                VertexShader = @"
                    varying vec3 vNormal;
                    varying vec3 vPosition;
                    uniform float time;
                    
                    void main() {
                        vNormal = normalize(normalMatrix * normal);
                        vPosition = position;
                        
                        vec3 displaced = position;
                        displaced.y += sin(position.x * 10.0 + time) * 0.05;
                        
                        gl_Position = projectionMatrix * modelViewMatrix * vec4(displaced, 1.0);
                    }
                ",
                FragmentShader = @"
                    uniform float time;
                    uniform float transparency;
                    varying vec3 vNormal;
                    varying vec3 vPosition;
                    
                    void main() {
                        vec3 viewDirection = normalize(cameraPosition - vPosition);
                        float fresnel = pow(1.0 - dot(vNormal, viewDirection), 2.0);
                        
                        vec3 color = vec3(0.0, 0.8, 1.0);
                        float alpha = fresnel * transparency;
                        
                        gl_FragColor = vec4(color * fresnel, alpha);
                    }
                "
            },
            [ShaderType.Metallic] = new ShaderTemplate
            {
                VertexShader = @"
                    varying vec3 vNormal;
                    varying vec3 vPosition;
                    
                    void main() {
                        vNormal = normalize(normalMatrix * normal);
                        vPosition = (modelViewMatrix * vec4(position, 1.0)).xyz;
                        gl_Position = projectionMatrix * modelViewMatrix * vec4(position, 1.0);
                    }
                ",
                FragmentShader = @"
                    uniform float metallicFactor;
                    uniform float roughnessFactor;
                    uniform vec3 baseColor;
                    varying vec3 vNormal;
                    varying vec3 vPosition;
                    
                    void main() {
                        vec3 normal = normalize(vNormal);
                        vec3 viewDir = normalize(-vPosition);
                        
                        float metallic = metallicFactor;
                        vec3 albedo = baseColor;
                        
                        vec3 reflection = reflect(-viewDir, normal);
                        float fresnel = pow(1.0 - max(dot(normal, viewDir), 0.0), 5.0);
                        
                        vec3 color = mix(albedo, vec3(0.9), metallic * fresnel);
                        
                        gl_FragColor = vec4(color, 1.0);
                    }
                "
            }
        };
    }

    private ShaderCode GenerateFallbackShader(ShaderType shaderType)
    {
        _logger.LogWarning("Generating fallback shader for {ShaderType}", shaderType);
        
        return new ShaderCode
        {
            VertexShader = @"
                void main() {
                    gl_Position = projectionMatrix * modelViewMatrix * vec4(position, 1.0);
                }
            ",
            FragmentShader = @"
                void main() {
                    gl_FragColor = vec4(0.0, 0.8, 1.0, 1.0);
                }
            ",
            Uniforms = new Dictionary<string, UniformValue>(),
            ShaderType = shaderType,
            GeneratedAt = DateTime.UtcNow
        };
    }

    private string GenerateFallbackHolographicMaterial()
    {
        return @"
            // Fallback Holographic Material
            const holographicMaterial = new THREE.MeshBasicMaterial({
                color: 0x00d4ff,
                transparent: true,
                opacity: 0.7,
                wireframe: false
            });
        ";
    }

    private GlowEffect GenerateFallbackGlowEffect(GlowConfig glowConfig)
    {
        return new GlowEffect
        {
            CssCode = @"
                .cyberpunk-glow {
                    color: #00d4ff;
                    text-shadow: 0 0 10px #00d4ff;
                    filter: drop-shadow(0 0 5px #00d4ff);
                }
            ",
            JavaScriptCode = "// Fallback glow effect - basic implementation",
            GlowConfig = glowConfig,
            IsAnimated = false,
            CreatedAt = DateTime.UtcNow
        };
    }
}

/// <summary>
/// Shader template containing vertex and fragment shader source
/// </summary>
public class ShaderTemplate
{
    public required string VertexShader { get; init; }
    public required string FragmentShader { get; init; }
}