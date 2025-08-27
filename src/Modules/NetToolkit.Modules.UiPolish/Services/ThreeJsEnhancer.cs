using NetToolkit.Modules.UiPolish.Models;
using NetToolkit.Modules.UiPolish.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace NetToolkit.Modules.UiPolish.Services;

/// <summary>
/// Advanced Three.js enhancement engine for creating immersive 3D experiences
/// Specializes in generating sophisticated shader code and particle systems
/// </summary>
public class ThreeJsEnhancer : IThreeJsEnhancer
{
    private readonly ILogger<ThreeJsEnhancer> _logger;
    private readonly IShaderUtility _shaderUtility;
    private readonly Dictionary<ComponentType, string> _enhancementTemplates;

    public ThreeJsEnhancer(ILogger<ThreeJsEnhancer> logger, IShaderUtility shaderUtility)
    {
        _logger = logger;
        _shaderUtility = shaderUtility;
        _enhancementTemplates = InitializeEnhancementTemplates();
    }

    public async Task<string> GenerateEnhancementAsync(ComponentType componentType, ThreeJsOptions options)
    {
        try
        {
            _logger.LogInformation("Generating Three.js enhancement for {ComponentType}", componentType);

            var enhancement = componentType switch
            {
                ComponentType.Terminal => await GenerateTerminalEnhancementAsync(options),
                ComponentType.Topography => await GenerateTopographyEnhancementAsync(options),
                ComponentType.Dashboard => await GenerateDashboardEnhancementAsync(options),
                ComponentType.Scanner => await GenerateScannerEnhancementAsync(options),
                ComponentType.AiOrb => await GenerateAiOrbEnhancementAsync(options),
                ComponentType.Education => await GenerateEducationEnhancementAsync(options),
                ComponentType.Admin => await GenerateAdminEnhancementAsync(options),
                _ => await GenerateGenericEnhancementAsync(options)
            };

            _logger.LogInformation("Successfully generated {Length} characters of Three.js enhancement", enhancement.Length);
            return enhancement;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate Three.js enhancement for {ComponentType}", componentType);
            return GenerateFallbackScript(componentType);
        }
    }

    private async Task<string> GenerateTerminalEnhancementAsync(ThreeJsOptions options)
    {
        var holographicShader = await _shaderUtility.CreateHolographicMaterialAsync(new HolographicConfig
        {
            Transparency = 0.8f,
            GlowIntensity = 1.2f,
            ColorShift = true,
            RimLighting = true,
            FresnelPower = 2.0f
        });

        var sb = new StringBuilder();
        sb.AppendLine("// Terminal Holographic Enhancement - Cyberpunk Console");
        sb.AppendLine("(function() {");
        sb.AppendLine("  const scene = new THREE.Scene();");
        sb.AppendLine("  const camera = new THREE.PerspectiveCamera(75, window.innerWidth / window.innerHeight, 0.1, 1000);");
        sb.AppendLine("  const renderer = new THREE.WebGLRenderer({ alpha: true, antialias: true });");
        sb.AppendLine("  renderer.setSize(window.innerWidth, window.innerHeight);");
        sb.AppendLine("  renderer.setClearColor(0x000000, 0);");
        sb.AppendLine();
        
        // Add holographic material
        sb.AppendLine("  // Holographic terminal material");
        sb.AppendLine($"  {holographicShader}");
        sb.AppendLine();
        
        // Terminal scanlines effect
        sb.AppendLine("  // Scanlines geometry");
        sb.AppendLine("  const scanlineGeometry = new THREE.PlaneGeometry(2, 0.01, 1, 1);");
        sb.AppendLine("  const scanlineMaterial = new THREE.MeshBasicMaterial({");
        sb.AppendLine("    color: 0x00ff00,");
        sb.AppendLine("    transparent: true,");
        sb.AppendLine("    opacity: 0.3");
        sb.AppendLine("  });");
        sb.AppendLine();
        
        // Animation loop
        sb.AppendLine("  const scanlines = [];");
        sb.AppendLine("  for(let i = 0; i < 20; i++) {");
        sb.AppendLine("    const scanline = new THREE.Mesh(scanlineGeometry, scanlineMaterial);");
        sb.AppendLine("    scanline.position.y = (i * 0.1) - 1;");
        sb.AppendLine("    scene.add(scanline);");
        sb.AppendLine("    scanlines.push(scanline);");
        sb.AppendLine("  }");
        sb.AppendLine();
        
        sb.AppendLine("  camera.position.z = 5;");
        sb.AppendLine();
        
        sb.AppendLine("  function animate() {");
        sb.AppendLine("    requestAnimationFrame(animate);");
        sb.AppendLine("    scanlines.forEach((line, index) => {");
        sb.AppendLine("      line.position.y += 0.01;");
        sb.AppendLine("      if(line.position.y > 1) line.position.y = -1;");
        sb.AppendLine("    });");
        sb.AppendLine("    renderer.render(scene, camera);");
        sb.AppendLine("  }");
        sb.AppendLine("  animate();");
        sb.AppendLine("})();");

        return sb.ToString();
    }

    private async Task<string> GenerateTopographyEnhancementAsync(ThreeJsOptions options)
    {
        var sb = new StringBuilder();
        sb.AppendLine("// Network Topography 3D Enhancement with Glowing Nodes");
        sb.AppendLine("(function() {");
        sb.AppendLine("  const scene = new THREE.Scene();");
        sb.AppendLine("  const camera = new THREE.PerspectiveCamera(75, window.innerWidth / window.innerHeight, 0.1, 1000);");
        sb.AppendLine("  const renderer = new THREE.WebGLRenderer({ alpha: true, antialias: true });");
        sb.AppendLine("  renderer.setSize(window.innerWidth, window.innerHeight);");
        sb.AppendLine("  renderer.setClearColor(0x0a0a0a, 0.9);");
        sb.AppendLine();
        
        // Generate glowing sphere for nodes
        sb.AppendLine("  // Glowing node material");
        sb.AppendLine("  const glowVertexShader = `");
        sb.AppendLine("    varying vec3 vNormal;");
        sb.AppendLine("    void main() {");
        sb.AppendLine("      vNormal = normalize(normalMatrix * normal);");
        sb.AppendLine("      gl_Position = projectionMatrix * modelViewMatrix * vec4(position, 1.0);");
        sb.AppendLine("    }");
        sb.AppendLine("  `;");
        sb.AppendLine();
        
        sb.AppendLine("  const glowFragmentShader = `");
        sb.AppendLine("    uniform vec3 glowColor;");
        sb.AppendLine("    uniform float intensity;");
        sb.AppendLine("    varying vec3 vNormal;");
        sb.AppendLine("    void main() {");
        sb.AppendLine("      float fresnel = pow(1.0 - dot(vNormal, vec3(0.0, 0.0, 1.0)), 2.0);");
        sb.AppendLine("      gl_FragColor = vec4(glowColor * intensity * fresnel, fresnel);");
        sb.AppendLine("    }");
        sb.AppendLine("  `;");
        sb.AppendLine();
        
        sb.AppendLine("  const nodeMaterial = new THREE.ShaderMaterial({");
        sb.AppendLine("    uniforms: {");
        sb.AppendLine("      glowColor: { value: new THREE.Color(0x00d4ff) },");
        sb.AppendLine("      intensity: { value: 1.5 }");
        sb.AppendLine("    },");
        sb.AppendLine("    vertexShader: glowVertexShader,");
        sb.AppendLine("    fragmentShader: glowFragmentShader,");
        sb.AppendLine("    transparent: true,");
        sb.AppendLine("    blending: THREE.AdditiveBlending");
        sb.AppendLine("  });");
        sb.AppendLine();
        
        // Generate network nodes
        sb.AppendLine("  // Create network nodes");
        sb.AppendLine("  const nodes = [];");
        sb.AppendLine("  const connections = [];");
        sb.AppendLine("  for(let i = 0; i < 20; i++) {");
        sb.AppendLine("    const geometry = new THREE.SphereGeometry(0.1, 16, 16);");
        sb.AppendLine("    const node = new THREE.Mesh(geometry, nodeMaterial);");
        sb.AppendLine("    node.position.set(");
        sb.AppendLine("      (Math.random() - 0.5) * 10,");
        sb.AppendLine("      (Math.random() - 0.5) * 10,");
        sb.AppendLine("      (Math.random() - 0.5) * 10");
        sb.AppendLine("    );");
        sb.AppendLine("    scene.add(node);");
        sb.AppendLine("    nodes.push(node);");
        sb.AppendLine("  }");
        sb.AppendLine();
        
        sb.AppendLine("  camera.position.z = 15;");
        sb.AppendLine();
        
        sb.AppendLine("  function animate() {");
        sb.AppendLine("    requestAnimationFrame(animate);");
        sb.AppendLine("    nodes.forEach(node => {");
        sb.AppendLine("      node.rotation.x += 0.01;");
        sb.AppendLine("      node.rotation.y += 0.01;");
        sb.AppendLine("    });");
        sb.AppendLine("    renderer.render(scene, camera);");
        sb.AppendLine("  }");
        sb.AppendLine("  animate();");
        sb.AppendLine("})();");

        return sb.ToString();
    }

    private async Task<string> GenerateDashboardEnhancementAsync(ThreeJsOptions options)
    {
        var particleConfig = new ParticleSystemConfig
        {
            ParticleCount = 1000,
            ParticleSize = 2.0f,
            SpeedRange = new Vector2(0.1f, 0.5f),
            LifetimeRange = new Vector2(3.0f, 8.0f),
            EmissionRate = 50,
            StartColor = new ColorRgba(0, 212, 255, 255),
            EndColor = new ColorRgba(0, 100, 150, 0)
        };

        var particleScript = await CreateParticleSystemAsync(particleConfig);

        var sb = new StringBuilder();
        sb.AppendLine("// Dashboard Floating Particles Enhancement");
        sb.AppendLine("(function() {");
        sb.AppendLine("  const scene = new THREE.Scene();");
        sb.AppendLine("  const camera = new THREE.PerspectiveCamera(75, window.innerWidth / window.innerHeight, 0.1, 1000);");
        sb.AppendLine("  const renderer = new THREE.WebGLRenderer({ alpha: true, antialias: true });");
        sb.AppendLine("  renderer.setSize(window.innerWidth, window.innerHeight);");
        sb.AppendLine("  renderer.setClearColor(0x000000, 0);");
        sb.AppendLine();
        
        sb.AppendLine(particleScript);
        sb.AppendLine();
        
        sb.AppendLine("  camera.position.z = 10;");
        sb.AppendLine();
        
        sb.AppendLine("  function animate() {");
        sb.AppendLine("    requestAnimationFrame(animate);");
        sb.AppendLine("    renderer.render(scene, camera);");
        sb.AppendLine("  }");
        sb.AppendLine("  animate();");
        sb.AppendLine("})();");

        return sb.ToString();
    }

    private async Task<string> GenerateScannerEnhancementAsync(ThreeJsOptions options)
    {
        var sb = new StringBuilder();
        sb.AppendLine("// Security Scanner Pulse Effect");
        sb.AppendLine("(function() {");
        sb.AppendLine("  const scene = new THREE.Scene();");
        sb.AppendLine("  const camera = new THREE.PerspectiveCamera(75, window.innerWidth / window.innerHeight, 0.1, 1000);");
        sb.AppendLine("  const renderer = new THREE.WebGLRenderer({ alpha: true, antialias: true });");
        sb.AppendLine("  renderer.setSize(window.innerWidth, window.innerHeight);");
        sb.AppendLine("  renderer.setClearColor(0x000000, 0);");
        sb.AppendLine();
        
        sb.AppendLine("  // Pulse ring geometry");
        sb.AppendLine("  const ringGeometry = new THREE.RingGeometry(1, 1.1, 32);");
        sb.AppendLine("  const ringMaterial = new THREE.MeshBasicMaterial({");
        sb.AppendLine("    color: 0xff4444,");
        sb.AppendLine("    transparent: true,");
        sb.AppendLine("    opacity: 0.7,");
        sb.AppendLine("    side: THREE.DoubleSide");
        sb.AppendLine("  });");
        sb.AppendLine();
        
        sb.AppendLine("  const pulseRings = [];");
        sb.AppendLine("  for(let i = 0; i < 5; i++) {");
        sb.AppendLine("    const ring = new THREE.Mesh(ringGeometry, ringMaterial.clone());");
        sb.AppendLine("    ring.position.z = -i * 0.5;");
        sb.AppendLine("    scene.add(ring);");
        sb.AppendLine("    pulseRings.push({ mesh: ring, scale: 1, opacity: 0.7 });");
        sb.AppendLine("  }");
        sb.AppendLine();
        
        sb.AppendLine("  camera.position.z = 5;");
        sb.AppendLine();
        
        sb.AppendLine("  function animate() {");
        sb.AppendLine("    requestAnimationFrame(animate);");
        sb.AppendLine("    pulseRings.forEach((ring, index) => {");
        sb.AppendLine("      ring.scale += 0.02;");
        sb.AppendLine("      ring.opacity -= 0.01;");
        sb.AppendLine("      ring.mesh.scale.set(ring.scale, ring.scale, 1);");
        sb.AppendLine("      ring.mesh.material.opacity = ring.opacity;");
        sb.AppendLine("      if(ring.opacity <= 0) {");
        sb.AppendLine("        ring.scale = 1;");
        sb.AppendLine("        ring.opacity = 0.7;");
        sb.AppendLine("      }");
        sb.AppendLine("    });");
        sb.AppendLine("    renderer.render(scene, camera);");
        sb.AppendLine("  }");
        sb.AppendLine("  animate();");
        sb.AppendLine("})();");

        return sb.ToString();
    }

    private async Task<string> GenerateAiOrbEnhancementAsync(ThreeJsOptions options)
    {
        var sb = new StringBuilder();
        sb.AppendLine("// AI Orb Orbital Ring Enhancement");
        sb.AppendLine("(function() {");
        sb.AppendLine("  const scene = new THREE.Scene();");
        sb.AppendLine("  const camera = new THREE.PerspectiveCamera(75, window.innerWidth / window.innerHeight, 0.1, 1000);");
        sb.AppendLine("  const renderer = new THREE.WebGLRenderer({ alpha: true, antialias: true });");
        sb.AppendLine("  renderer.setSize(window.innerWidth, window.innerHeight);");
        sb.AppendLine("  renderer.setClearColor(0x000000, 0);");
        sb.AppendLine();
        
        sb.AppendLine("  // Central orb");
        sb.AppendLine("  const orbGeometry = new THREE.SphereGeometry(0.5, 32, 32);");
        sb.AppendLine("  const orbMaterial = new THREE.MeshBasicMaterial({");
        sb.AppendLine("    color: 0x00d4ff,");
        sb.AppendLine("    transparent: true,");
        sb.AppendLine("    opacity: 0.8");
        sb.AppendLine("  });");
        sb.AppendLine("  const orb = new THREE.Mesh(orbGeometry, orbMaterial);");
        sb.AppendLine("  scene.add(orb);");
        sb.AppendLine();
        
        sb.AppendLine("  // Orbital rings");
        sb.AppendLine("  const rings = [];");
        sb.AppendLine("  for(let i = 0; i < 3; i++) {");
        sb.AppendLine("    const ringGeometry = new THREE.TorusGeometry(1 + i * 0.5, 0.02, 8, 100);");
        sb.AppendLine("    const ringMaterial = new THREE.MeshBasicMaterial({");
        sb.AppendLine("      color: 0x00ff88,");
        sb.AppendLine("      transparent: true,");
        sb.AppendLine("      opacity: 0.5");
        sb.AppendLine("    });");
        sb.AppendLine("    const ring = new THREE.Mesh(ringGeometry, ringMaterial);");
        sb.AppendLine("    ring.rotation.x = Math.PI / 2;");
        sb.AppendLine("    ring.rotation.y = i * 0.3;");
        sb.AppendLine("    scene.add(ring);");
        sb.AppendLine("    rings.push(ring);");
        sb.AppendLine("  }");
        sb.AppendLine();
        
        sb.AppendLine("  camera.position.z = 5;");
        sb.AppendLine();
        
        sb.AppendLine("  function animate() {");
        sb.AppendLine("    requestAnimationFrame(animate);");
        sb.AppendLine("    orb.rotation.y += 0.01;");
        sb.AppendLine("    rings.forEach((ring, index) => {");
        sb.AppendLine("      ring.rotation.z += 0.005 * (index + 1);");
        sb.AppendLine("    });");
        sb.AppendLine("    renderer.render(scene, camera);");
        sb.AppendLine("  }");
        sb.AppendLine("  animate();");
        sb.AppendLine("})();");

        return sb.ToString();
    }

    private async Task<string> GenerateEducationEnhancementAsync(ThreeJsOptions options)
    {
        var sb = new StringBuilder();
        sb.AppendLine("// Education Module Interactive 3D Elements");
        sb.AppendLine("(function() {");
        sb.AppendLine("  const scene = new THREE.Scene();");
        sb.AppendLine("  const camera = new THREE.PerspectiveCamera(75, window.innerWidth / window.innerHeight, 0.1, 1000);");
        sb.AppendLine("  const renderer = new THREE.WebGLRenderer({ alpha: true, antialias: true });");
        sb.AppendLine("  renderer.setSize(window.innerWidth, window.innerHeight);");
        sb.AppendLine("  renderer.setClearColor(0x000000, 0);");
        sb.AppendLine();
        
        sb.AppendLine("  // Floating knowledge nodes");
        sb.AppendLine("  const nodes = [];");
        sb.AppendLine("  for(let i = 0; i < 10; i++) {");
        sb.AppendLine("    const geometry = new THREE.OctahedronGeometry(0.2);");
        sb.AppendLine("    const material = new THREE.MeshBasicMaterial({");
        sb.AppendLine("      color: new THREE.Color().setHSL(i / 10, 0.8, 0.6),");
        sb.AppendLine("      transparent: true,");
        sb.AppendLine("      opacity: 0.7");
        sb.AppendLine("    });");
        sb.AppendLine("    const node = new THREE.Mesh(geometry, material);");
        sb.AppendLine("    node.position.set(");
        sb.AppendLine("      (Math.random() - 0.5) * 5,");
        sb.AppendLine("      (Math.random() - 0.5) * 5,");
        sb.AppendLine("      (Math.random() - 0.5) * 5");
        sb.AppendLine("    );");
        sb.AppendLine("    scene.add(node);");
        sb.AppendLine("    nodes.push(node);");
        sb.AppendLine("  }");
        sb.AppendLine();
        
        sb.AppendLine("  camera.position.z = 8;");
        sb.AppendLine();
        
        sb.AppendLine("  function animate() {");
        sb.AppendLine("    requestAnimationFrame(animate);");
        sb.AppendLine("    nodes.forEach((node, index) => {");
        sb.AppendLine("      node.rotation.x += 0.01;");
        sb.AppendLine("      node.rotation.y += 0.01;");
        sb.AppendLine("      node.position.y += Math.sin(Date.now() * 0.001 + index) * 0.001;");
        sb.AppendLine("    });");
        sb.AppendLine("    renderer.render(scene, camera);");
        sb.AppendLine("  }");
        sb.AppendLine("  animate();");
        sb.AppendLine("})();");

        return sb.ToString();
    }

    private async Task<string> GenerateAdminEnhancementAsync(ThreeJsOptions options)
    {
        var sb = new StringBuilder();
        sb.AppendLine("// Admin Interface Floating Panels");
        sb.AppendLine("(function() {");
        sb.AppendLine("  const scene = new THREE.Scene();");
        sb.AppendLine("  const camera = new THREE.PerspectiveCamera(75, window.innerWidth / window.innerHeight, 0.1, 1000);");
        sb.AppendLine("  const renderer = new THREE.WebGLRenderer({ alpha: true, antialias: true });");
        sb.AppendLine("  renderer.setSize(window.innerWidth, window.innerHeight);");
        sb.AppendLine("  renderer.setClearColor(0x000000, 0);");
        sb.AppendLine();
        
        sb.AppendLine("  // Admin status indicators");
        sb.AppendLine("  const indicators = [];");
        sb.AppendLine("  const colors = [0x00ff00, 0xffff00, 0xff8800, 0xff0000];");
        sb.AppendLine("  for(let i = 0; i < 4; i++) {");
        sb.AppendLine("    const geometry = new THREE.CylinderGeometry(0.1, 0.1, 0.5, 8);");
        sb.AppendLine("    const material = new THREE.MeshBasicMaterial({");
        sb.AppendLine("      color: colors[i],");
        sb.AppendLine("      transparent: true,");
        sb.AppendLine("      opacity: 0.8");
        sb.AppendLine("    });");
        sb.AppendLine("    const indicator = new THREE.Mesh(geometry, material);");
        sb.AppendLine("    indicator.position.set(i * 1.5 - 2.25, 0, 0);");
        sb.AppendLine("    scene.add(indicator);");
        sb.AppendLine("    indicators.push(indicator);");
        sb.AppendLine("  }");
        sb.AppendLine();
        
        sb.AppendLine("  camera.position.z = 5;");
        sb.AppendLine();
        
        sb.AppendLine("  function animate() {");
        sb.AppendLine("    requestAnimationFrame(animate);");
        sb.AppendLine("    indicators.forEach((indicator, index) => {");
        sb.AppendLine("      indicator.material.opacity = 0.5 + 0.5 * Math.sin(Date.now() * 0.003 + index);");
        sb.AppendLine("    });");
        sb.AppendLine("    renderer.render(scene, camera);");
        sb.AppendLine("  }");
        sb.AppendLine("  animate();");
        sb.AppendLine("})();");

        return sb.ToString();
    }

    private async Task<string> GenerateGenericEnhancementAsync(ThreeJsOptions options)
    {
        return @"// Generic Three.js Enhancement
(function() {
  const scene = new THREE.Scene();
  const camera = new THREE.PerspectiveCamera(75, window.innerWidth / window.innerHeight, 0.1, 1000);
  const renderer = new THREE.WebGLRenderer({ alpha: true, antialias: true });
  renderer.setSize(window.innerWidth, window.innerHeight);
  renderer.setClearColor(0x000000, 0);
  
  const geometry = new THREE.BoxGeometry();
  const material = new THREE.MeshBasicMaterial({ color: 0x00d4ff, transparent: true, opacity: 0.5 });
  const cube = new THREE.Mesh(geometry, material);
  scene.add(cube);
  
  camera.position.z = 5;
  
  function animate() {
    requestAnimationFrame(animate);
    cube.rotation.x += 0.01;
    cube.rotation.y += 0.01;
    renderer.render(scene, camera);
  }
  animate();
})();";
    }

    private async Task<string> CreateParticleSystemAsync(ParticleSystemConfig config)
    {
        var sb = new StringBuilder();
        sb.AppendLine("  // Particle System");
        sb.AppendLine($"  const particleCount = {config.ParticleCount};");
        sb.AppendLine("  const particles = new THREE.BufferGeometry();");
        sb.AppendLine("  const positions = new Float32Array(particleCount * 3);");
        sb.AppendLine("  const velocities = new Float32Array(particleCount * 3);");
        sb.AppendLine();
        
        sb.AppendLine("  for(let i = 0; i < particleCount; i++) {");
        sb.AppendLine("    positions[i * 3] = (Math.random() - 0.5) * 20;");
        sb.AppendLine("    positions[i * 3 + 1] = (Math.random() - 0.5) * 20;");
        sb.AppendLine("    positions[i * 3 + 2] = (Math.random() - 0.5) * 20;");
        sb.AppendLine();
        sb.AppendLine($"    velocities[i * 3] = (Math.random() - 0.5) * {config.SpeedRange.Y};");
        sb.AppendLine($"    velocities[i * 3 + 1] = (Math.random() - 0.5) * {config.SpeedRange.Y};");
        sb.AppendLine($"    velocities[i * 3 + 2] = (Math.random() - 0.5) * {config.SpeedRange.Y};");
        sb.AppendLine("  }");
        sb.AppendLine();
        
        sb.AppendLine("  particles.setAttribute('position', new THREE.BufferAttribute(positions, 3));");
        sb.AppendLine();
        
        sb.AppendLine("  const particleMaterial = new THREE.PointsMaterial({");
        sb.AppendLine($"    color: 0x{config.StartColor.R:x2}{config.StartColor.G:x2}{config.StartColor.B:x2},");
        sb.AppendLine($"    size: {config.ParticleSize},");
        sb.AppendLine("    transparent: true,");
        sb.AppendLine($"    opacity: {config.StartColor.A / 255.0:F2}");
        sb.AppendLine("  });");
        sb.AppendLine();
        
        sb.AppendLine("  const particleSystem = new THREE.Points(particles, particleMaterial);");
        sb.AppendLine("  scene.add(particleSystem);");

        return sb.ToString();
    }

    private Dictionary<ComponentType, string> InitializeEnhancementTemplates()
    {
        return new Dictionary<ComponentType, string>
        {
            [ComponentType.Terminal] = "holographic_scanlines",
            [ComponentType.Topography] = "network_nodes_glow",
            [ComponentType.Dashboard] = "floating_particles",
            [ComponentType.Scanner] = "pulse_rings",
            [ComponentType.AiOrb] = "orbital_rings",
            [ComponentType.Education] = "knowledge_nodes",
            [ComponentType.Admin] = "status_indicators"
        };
    }

    private string GenerateFallbackScript(ComponentType componentType)
    {
        _logger.LogWarning("Generating fallback script for {ComponentType}", componentType);
        
        return @"// Fallback Enhancement - Minimalist approach for reliability
(function() {
  console.log('Three.js enhancement loaded - fallback mode');
  const container = document.body;
  container.style.background = 'linear-gradient(135deg, #1a1a2e 0%, #16213e 100%)';
  container.style.transition = 'all 0.3s ease';
})();";
    }
}

/// <summary>
/// Extended interface for Three.js enhancement capabilities
/// </summary>
public interface IThreeJsEnhancer
{
    /// <summary>
    /// Generate Three.js enhancement code for specific component types
    /// </summary>
    Task<string> GenerateEnhancementAsync(ComponentType componentType, ThreeJsOptions options);
}