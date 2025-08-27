using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NetToolkit.Modules.ScannerAndTopography.Interfaces;
using NetToolkit.Modules.ScannerAndTopography.Models;

namespace NetToolkit.Modules.ScannerAndTopography.Services;

/// <summary>
/// Topography renderer service - the cosmic architect of 3D digital realms
/// Where network topology ascends to Three.js celestial visualization
/// </summary>
public class TopographyRendererService : ITopographyRenderer
{
    private readonly ILogger<TopographyRendererService> _logger;
    private readonly IScannerEventPublisher _eventPublisher;
    private readonly JsonSerializerSettings _jsonSettings;

    public TopographyRendererService(ILogger<TopographyRendererService> logger, IScannerEventPublisher eventPublisher)
    {
        _logger = logger;
        _eventPublisher = eventPublisher;
        
        // Configure JSON serialization for cosmic data exchange
        _jsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Formatting = Formatting.None,
            NullValueHandling = NullValueHandling.Ignore,
            DateFormatHandling = DateFormatHandling.IsoDateFormat
        };
    }

    /// <summary>
    /// Serialize topology to JSON - preparing data for Three.js metamorphosis
    /// </summary>
    public string SerializeToJson(TopologyResult result)
    {
        try
        {
            _logger.LogDebug("🌌 Serializing topology {ScanId} to cosmic JSON format - {NodeCount} nodes, {EdgeCount} edges",
                result.ScanId, result.Nodes.Count, result.Edges.Count);

            var serializableData = new
            {
                scanId = result.ScanId,
                timestamp = result.ScanTimestamp,
                networkRange = result.NetworkRange,
                nodes = result.Nodes.Select(n => new
                {
                    id = n.Id,
                    label = !string.IsNullOrEmpty(n.HostName) ? n.HostName : n.IpAddress,
                    ipAddress = n.IpAddress,
                    macAddress = n.MacAddress,
                    manufacturer = n.Manufacturer,
                    deviceType = n.DeviceType,
                    isOnline = n.IsOnline,
                    isDynamic = n.IsDynamic,
                    responseTime = n.ResponseTime,
                    openPorts = n.OpenPorts,
                    position = new { x = n.Position.X, y = n.Position.Y, z = n.Position.Z },
                    color = n.GetStatusColor(),
                    status = n.Status.ToString(),
                    personality = n.GetNodePersonality(),
                    size = CalculateNodeSize(n),
                    glow = n.Status == NodeStatus.Anomaly
                }).ToArray(),
                edges = result.Edges.Select(e => new
                {
                    id = e.Id,
                    source = e.FromNodeId,
                    target = e.ToNodeId,
                    type = e.ConnectionType.ToString(),
                    label = e.Label,
                    color = e.Color,
                    strength = e.Strength,
                    isActive = e.IsActive,
                    personality = e.GetConnectionPersonality()
                }).ToArray(),
                statistics = new
                {
                    totalNodes = result.Nodes.Count,
                    activeNodes = result.Nodes.Count(n => n.IsOnline),
                    totalEdges = result.Edges.Count,
                    scanDuration = result.Statistics.ScanDuration.TotalSeconds,
                    efficiency = result.Statistics.ScanEfficiency * 100,
                    commentary = result.Statistics.GetPerformanceCommentary()
                },
                anomalies = result.Anomalies.Select(a => new
                {
                    id = a.Id,
                    nodeId = a.NodeId,
                    type = a.Type.ToString(),
                    severity = a.Severity.ToString(),
                    description = a.Description,
                    wittyDescription = a.GetWittyDescription(),
                    recommendedAction = a.RecommendedAction,
                    detectedAt = a.DetectedAt
                }).ToArray(),
                summary = result.GetWittyScanSummary()
            };

            var json = JsonConvert.SerializeObject(serializableData, _jsonSettings);
            
            _logger.LogInformation("✨ Topology serialization complete - {Size} bytes of cosmic data ready for Three.js consumption!",
                Encoding.UTF8.GetByteCount(json));

            return json;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Topology serialization encountered cosmic interference - data transmission failed!");
            throw;
        }
    }

    /// <summary>
    /// Handle navigation events from 3D interface - cosmic interaction processing
    /// </summary>
    public async Task HandleNavigationEventAsync(string eventJson, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("🚀 Processing 3D navigation event - cosmic interaction detected!");
            
            var navigationEvent = JsonConvert.DeserializeObject<NavigationEvent>(eventJson, _jsonSettings);
            if (navigationEvent == null)
            {
                _logger.LogWarning("⚠️ Invalid navigation event format - cosmic static interference!");
                return;
            }

            switch (navigationEvent.Type)
            {
                case "zoom":
                    await HandleZoomEvent(navigationEvent, cancellationToken);
                    break;
                case "rotate":
                    await HandleRotateEvent(navigationEvent, cancellationToken);
                    break;
                case "nodeClick":
                    await HandleNodeClickEvent(navigationEvent, cancellationToken);
                    break;
                case "nodeHover":
                    await HandleNodeHoverEvent(navigationEvent, cancellationToken);
                    break;
                case "edgeClick":
                    await HandleEdgeClickEvent(navigationEvent, cancellationToken);
                    break;
                default:
                    _logger.LogDebug("🤔 Unknown navigation event type: {Type} - new cosmic phenomena discovered!", navigationEvent.Type);
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💫 Navigation event processing encountered dimensional anomaly!");
        }
    }

    /// <summary>
    /// Generate Three.js initialization script - birthing the digital universe
    /// </summary>
    public string GenerateThreeJsScript(TopologyResult topology, string containerElementId = "topology-container")
    {
        var script = new StringBuilder();
        
        script.AppendLine($"// 🌌 NetToolkit Three.js Topology Renderer - Digital Cosmos Generator");
        script.AppendLine($"// Generated at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
        script.AppendLine();
        
        // Initialize Three.js scene with metallic cyber aesthetic
        script.AppendLine("class NetTopologyRenderer {");
        script.AppendLine("    constructor(containerId) {");
        script.AppendLine("        this.container = document.getElementById(containerId);");
        script.AppendLine("        this.scene = new THREE.Scene();");
        script.AppendLine("        this.camera = new THREE.PerspectiveCamera(75, this.container.clientWidth / this.container.clientHeight, 0.1, 2000);");
        script.AppendLine("        this.renderer = new THREE.WebGLRenderer({ antialias: true, alpha: true });");
        script.AppendLine("        this.nodes = new Map();");
        script.AppendLine("        this.edges = new Map();");
        script.AppendLine("        this.raycaster = new THREE.Raycaster();");
        script.AppendLine("        this.mouse = new THREE.Vector2();");
        script.AppendLine("        this.controls = null;");
        script.AppendLine();
        script.AppendLine("        this.initRenderer();");
        script.AppendLine("        this.initControls();");
        script.AppendLine("        this.initLighting();");
        script.AppendLine("        this.setupEventListeners();");
        script.AppendLine("        this.animate();");
        script.AppendLine("    }");
        script.AppendLine();

        // Renderer initialization with cyber aesthetics
        script.AppendLine("    initRenderer() {");
        script.AppendLine("        this.renderer.setSize(this.container.clientWidth, this.container.clientHeight);");
        script.AppendLine("        this.renderer.setClearColor(0x0a0a0a, 1); // Deep space black");
        script.AppendLine("        this.renderer.shadowMap.enabled = true;");
        script.AppendLine("        this.renderer.shadowMap.type = THREE.PCFSoftShadowMap;");
        script.AppendLine("        this.container.appendChild(this.renderer.domElement);");
        script.AppendLine("        this.camera.position.set(0, 50, 100);");
        script.AppendLine("    }");
        script.AppendLine();

        // Orbital controls for cosmic navigation
        script.AppendLine("    initControls() {");
        script.AppendLine("        this.controls = new THREE.OrbitControls(this.camera, this.renderer.domElement);");
        script.AppendLine("        this.controls.enableDamping = true;");
        script.AppendLine("        this.controls.dampingFactor = 0.05;");
        script.AppendLine("        this.controls.maxDistance = 500;");
        script.AppendLine("        this.controls.minDistance = 10;");
        script.AppendLine("    }");
        script.AppendLine();

        // Dramatic lighting for metallic nodes
        script.AppendLine("    initLighting() {");
        script.AppendLine("        const ambientLight = new THREE.AmbientLight(0x404040, 0.3);");
        script.AppendLine("        this.scene.add(ambientLight);");
        script.AppendLine();
        script.AppendLine("        const directionalLight = new THREE.DirectionalLight(0xffffff, 0.8);");
        script.AppendLine("        directionalLight.position.set(100, 100, 50);");
        script.AppendLine("        directionalLight.castShadow = true;");
        script.AppendLine("        directionalLight.shadow.mapSize.width = 2048;");
        script.AppendLine("        directionalLight.shadow.mapSize.height = 2048;");
        script.AppendLine("        this.scene.add(directionalLight);");
        script.AppendLine();
        script.AppendLine("        const pointLight = new THREE.PointLight(0x00ffff, 0.5, 200);");
        script.AppendLine("        pointLight.position.set(0, 50, 0);");
        script.AppendLine("        this.scene.add(pointLight);");
        script.AppendLine("    }");
        script.AppendLine();

        // Node creation with metallic materials
        script.AppendLine("    createNode(nodeData) {");
        script.AppendLine("        const geometry = new THREE.SphereGeometry(nodeData.size || 2, 16, 16);");
        script.AppendLine("        const material = new THREE.MeshPhongMaterial({");
        script.AppendLine("            color: nodeData.color,");
        script.AppendLine("            shininess: 100,");
        script.AppendLine("            specular: 0x222222");
        script.AppendLine("        });");
        script.AppendLine();
        script.AppendLine("        const mesh = new THREE.Mesh(geometry, material);");
        script.AppendLine("        mesh.position.set(nodeData.position.x, nodeData.position.y, nodeData.position.z);");
        script.AppendLine("        mesh.castShadow = true;");
        script.AppendLine("        mesh.receiveShadow = true;");
        script.AppendLine("        mesh.userData = nodeData;");
        script.AppendLine();
        script.AppendLine("        // Add glow effect for anomalies");
        script.AppendLine("        if (nodeData.glow) {");
        script.AppendLine("            const glowGeometry = new THREE.SphereGeometry(nodeData.size * 1.5, 16, 16);");
        script.AppendLine("            const glowMaterial = new THREE.MeshBasicMaterial({");
        script.AppendLine("                color: 0xff4444,");
        script.AppendLine("                transparent: true,");
        script.AppendLine("                opacity: 0.3");
        script.AppendLine("            });");
        script.AppendLine("            const glowMesh = new THREE.Mesh(glowGeometry, glowMaterial);");
        script.AppendLine("            mesh.add(glowMesh);");
        script.AppendLine("        }");
        script.AppendLine();
        script.AppendLine("        this.scene.add(mesh);");
        script.AppendLine("        this.nodes.set(nodeData.id, mesh);");
        script.AppendLine("        return mesh;");
        script.AppendLine("    }");
        script.AppendLine();

        // Edge creation with laser-like connections
        script.AppendLine("    createEdge(edgeData, sourceNode, targetNode) {");
        script.AppendLine("        const points = [");
        script.AppendLine("            sourceNode.position.clone(),");
        script.AppendLine("            targetNode.position.clone()");
        script.AppendLine("        ];");
        script.AppendLine("        const geometry = new THREE.BufferGeometry().setFromPoints(points);");
        script.AppendLine("        const material = new THREE.LineBasicMaterial({");
        script.AppendLine("            color: edgeData.color,");
        script.AppendLine("            transparent: true,");
        script.AppendLine("            opacity: edgeData.isActive ? 0.8 : 0.3");
        script.AppendLine("        });");
        script.AppendLine();
        script.AppendLine("        const line = new THREE.Line(geometry, material);");
        script.AppendLine("        line.userData = edgeData;");
        script.AppendLine("        this.scene.add(line);");
        script.AppendLine("        this.edges.set(edgeData.id, line);");
        script.AppendLine("        return line;");
        script.AppendLine("    }");
        script.AppendLine();

        // Event handling for cosmic interactions
        script.AppendLine("    setupEventListeners() {");
        script.AppendLine("        this.renderer.domElement.addEventListener('click', this.onMouseClick.bind(this));");
        script.AppendLine("        this.renderer.domElement.addEventListener('mousemove', this.onMouseMove.bind(this));");
        script.AppendLine("        window.addEventListener('resize', this.onWindowResize.bind(this));");
        script.AppendLine("    }");
        script.AppendLine();

        script.AppendLine("    onMouseClick(event) {");
        script.AppendLine("        this.updateMousePosition(event);");
        script.AppendLine("        this.raycaster.setFromCamera(this.mouse, this.camera);");
        script.AppendLine("        const intersects = this.raycaster.intersectObjects([...this.nodes.values()]);");
        script.AppendLine();
        script.AppendLine("        if (intersects.length > 0) {");
        script.AppendLine("            const nodeData = intersects[0].object.userData;");
        script.AppendLine("            this.handleNodeClick(nodeData);");
        script.AppendLine("        }");
        script.AppendLine("    }");
        script.AppendLine();

        script.AppendLine("    handleNodeClick(nodeData) {");
        script.AppendLine("        const event = {");
        script.AppendLine("            type: 'nodeClick',");
        script.AppendLine("            nodeId: nodeData.id,");
        script.AppendLine("            nodeData: nodeData,");
        script.AppendLine("            timestamp: new Date().toISOString()");
        script.AppendLine("        };");
        script.AppendLine("        this.sendEventToBackend(event);");
        script.AppendLine("    }");
        script.AppendLine();

        script.AppendLine("    sendEventToBackend(event) {");
        script.AppendLine("        if (window.chrome && window.chrome.webview) {");
        script.AppendLine("            window.chrome.webview.postMessage(JSON.stringify(event));");
        script.AppendLine("        } else {");
        script.AppendLine("            console.log('Navigation Event:', event);");
        script.AppendLine("        }");
        script.AppendLine("    }");
        script.AppendLine();

        // Animation loop for cosmic motion
        script.AppendLine("    animate() {");
        script.AppendLine("        requestAnimationFrame(() => this.animate());");
        script.AppendLine("        this.controls.update();");
        script.AppendLine("        this.renderer.render(this.scene, this.camera);");
        script.AppendLine("    }");
        script.AppendLine();

        script.AppendLine("    onWindowResize() {");
        script.AppendLine("        this.camera.aspect = this.container.clientWidth / this.container.clientHeight;");
        script.AppendLine("        this.camera.updateProjectionMatrix();");
        script.AppendLine("        this.renderer.setSize(this.container.clientWidth, this.container.clientHeight);");
        script.AppendLine("    }");
        script.AppendLine();

        script.AppendLine("    updateMousePosition(event) {");
        script.AppendLine("        const rect = this.renderer.domElement.getBoundingClientRect();");
        script.AppendLine("        this.mouse.x = ((event.clientX - rect.left) / rect.width) * 2 - 1;");
        script.AppendLine("        this.mouse.y = -((event.clientY - rect.top) / rect.height) * 2 + 1;");
        script.AppendLine("    }");
        script.AppendLine();

        // Topology loading method
        script.AppendLine("    loadTopology(topologyData) {");
        script.AppendLine("        // Clear existing topology");
        script.AppendLine("        this.clearScene();");
        script.AppendLine();
        script.AppendLine("        // Create nodes first");
        script.AppendLine("        topologyData.nodes.forEach(nodeData => {");
        script.AppendLine("            this.createNode(nodeData);");
        script.AppendLine("        });");
        script.AppendLine();
        script.AppendLine("        // Create edges between nodes");
        script.AppendLine("        topologyData.edges.forEach(edgeData => {");
        script.AppendLine("            const sourceNode = this.nodes.get(edgeData.source);");
        script.AppendLine("            const targetNode = this.nodes.get(edgeData.target);");
        script.AppendLine("            if (sourceNode && targetNode) {");
        script.AppendLine("                this.createEdge(edgeData, sourceNode, targetNode);");
        script.AppendLine("            }");
        script.AppendLine("        });");
        script.AppendLine();
        script.AppendLine("        console.log(`🌌 Topology loaded: ${topologyData.nodes.length} nodes, ${topologyData.edges.length} edges`);");
        script.AppendLine("    }");
        script.AppendLine();

        script.AppendLine("    clearScene() {");
        script.AppendLine("        [...this.nodes.values()].forEach(node => this.scene.remove(node));");
        script.AppendLine("        [...this.edges.values()].forEach(edge => this.scene.remove(edge));");
        script.AppendLine("        this.nodes.clear();");
        script.AppendLine("        this.edges.clear();");
        script.AppendLine("    }");
        script.AppendLine("}");
        script.AppendLine();

        // Initialize the topology renderer
        script.AppendLine($"// Initialize NetToolkit Topology Renderer");
        script.AppendLine($"const topologyRenderer = new NetTopologyRenderer('{containerElementId}');");
        script.AppendLine();

        // Load the topology data
        var topologyJson = SerializeToJson(topology);
        script.AppendLine($"// Load topology data");
        script.AppendLine($"const topologyData = {topologyJson};");
        script.AppendLine($"topologyRenderer.loadTopology(topologyData);");

        _logger.LogInformation("🎨 Three.js initialization script generated - {Lines} lines of cosmic rendering code!",
            script.ToString().Count(c => c == '\n'));

        return script.ToString();
    }

    /// <summary>
    /// Generate update script for topology changes - evolutionary cosmos adaptation
    /// </summary>
    public string GenerateUpdateScript(TopologyResult updates)
    {
        var updateScript = new StringBuilder();
        
        updateScript.AppendLine("// 🌟 Topology Update Script - Digital Evolution in Progress");
        updateScript.AppendLine($"// Generated at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
        updateScript.AppendLine();
        
        var updatesJson = SerializeToJson(updates);
        updateScript.AppendLine("if (typeof topologyRenderer !== 'undefined') {");
        updateScript.AppendLine($"    const updateData = {updatesJson};");
        updateScript.AppendLine("    topologyRenderer.loadTopology(updateData);");
        updateScript.AppendLine("    console.log('🔄 Topology updated successfully - digital cosmos evolved!');");
        updateScript.AppendLine("} else {");
        updateScript.AppendLine("    console.warn('⚠️ Topology renderer not found - cosmic synchronization failed!');");
        updateScript.AppendLine("}");

        return updateScript.ToString();
    }

    /// <summary>
    /// Calculate optimal 3D positioning - cosmic choreography algorithms
    /// </summary>
    public List<NetworkNode> CalculateOptimalPositioning(List<NetworkNode> nodes, List<NetworkEdge> edges)
    {
        _logger.LogDebug("🎭 Calculating optimal 3D positioning for {NodeCount} nodes - cosmic choreography commencing!", nodes.Count);

        if (!nodes.Any()) return nodes;

        // Simple force-directed layout algorithm for 3D space
        var positionedNodes = nodes.ToList();
        var random = new Random();
        
        // Initialize positions if not set
        foreach (var node in positionedNodes.Where(n => n.Position.X == 0 && n.Position.Y == 0 && n.Position.Z == 0))
        {
            node.Position = Vector3D.RandomPosition(random);
        }

        // Apply force-directed positioning over multiple iterations
        for (int iteration = 0; iteration < 100; iteration++)
        {
            // Repulsion forces between all nodes
            for (int i = 0; i < positionedNodes.Count; i++)
            {
                for (int j = i + 1; j < positionedNodes.Count; j++)
                {
                    var node1 = positionedNodes[i];
                    var node2 = positionedNodes[j];
                    
                    var dx = node2.Position.X - node1.Position.X;
                    var dy = node2.Position.Y - node1.Position.Y;
                    var dz = node2.Position.Z - node1.Position.Z;
                    
                    var distance = Math.Sqrt(dx * dx + dy * dy + dz * dz);
                    if (distance < 0.1) distance = 0.1; // Prevent division by zero
                    
                    var repulsionForce = 1000.0 / (distance * distance);
                    var fx = (dx / distance) * repulsionForce;
                    var fy = (dy / distance) * repulsionForce;
                    var fz = (dz / distance) * repulsionForce;
                    
                    // Apply forces
                    node1.Position.X -= fx * 0.01;
                    node1.Position.Y -= fy * 0.01;
                    node1.Position.Z -= fz * 0.01;
                    
                    node2.Position.X += fx * 0.01;
                    node2.Position.Y += fy * 0.01;
                    node2.Position.Z += fz * 0.01;
                }
            }

            // Attraction forces for connected nodes
            foreach (var edge in edges)
            {
                var sourceNode = positionedNodes.FirstOrDefault(n => n.Id == edge.FromNodeId);
                var targetNode = positionedNodes.FirstOrDefault(n => n.Id == edge.ToNodeId);
                
                if (sourceNode != null && targetNode != null)
                {
                    var dx = targetNode.Position.X - sourceNode.Position.X;
                    var dy = targetNode.Position.Y - sourceNode.Position.Y;
                    var dz = targetNode.Position.Z - sourceNode.Position.Z;
                    
                    var distance = Math.Sqrt(dx * dx + dy * dy + dz * dz);
                    var attractionForce = distance * 0.01 * edge.Strength;
                    
                    if (distance > 0.1)
                    {
                        var fx = (dx / distance) * attractionForce;
                        var fy = (dy / distance) * attractionForce;
                        var fz = (dz / distance) * attractionForce;
                        
                        sourceNode.Position.X += fx;
                        sourceNode.Position.Y += fy;
                        sourceNode.Position.Z += fz;
                        
                        targetNode.Position.X -= fx;
                        targetNode.Position.Y -= fy;
                        targetNode.Position.Z -= fz;
                    }
                }
            }
        }

        _logger.LogInformation("✨ Optimal positioning calculated - digital constellation arranged with cosmic harmony!");
        return positionedNodes;
    }

    #region Private Helper Methods

    private async Task HandleZoomEvent(NavigationEvent navigationEvent, CancellationToken cancellationToken)
    {
        _logger.LogDebug("🔍 Zoom event: {Direction} - adjusting cosmic perspective!", navigationEvent.Data?["direction"]);
        // Zoom handling logic can be extended based on needs
    }

    private async Task HandleRotateEvent(NavigationEvent navigationEvent, CancellationToken cancellationToken)
    {
        _logger.LogDebug("🌀 Rotate event - cosmic viewpoint shifting!");
        // Rotation handling logic
    }

    private async Task HandleNodeClickEvent(NavigationEvent navigationEvent, CancellationToken cancellationToken)
    {
        var nodeId = navigationEvent.Data?["nodeId"]?.ToString();
        if (!string.IsNullOrEmpty(nodeId))
        {
            _logger.LogInformation("🖱️ Node {NodeId} clicked - cosmic interaction detected!", nodeId);
            
            // Publish node interaction event for other modules
            await _eventPublisher.PublishAsync(new
            {
                EventType = "NodeClicked",
                NodeId = nodeId,
                Timestamp = DateTime.UtcNow,
                Source = "TopographyRenderer"
            });
        }
    }

    private async Task HandleNodeHoverEvent(NavigationEvent navigationEvent, CancellationToken cancellationToken)
    {
        var nodeId = navigationEvent.Data?["nodeId"]?.ToString();
        if (!string.IsNullOrEmpty(nodeId))
        {
            _logger.LogDebug("✨ Node {NodeId} hovered - displaying cosmic tooltip!", nodeId);
            // Hover handling logic
        }
    }

    private async Task HandleEdgeClickEvent(NavigationEvent navigationEvent, CancellationToken cancellationToken)
    {
        var edgeId = navigationEvent.Data?["edgeId"]?.ToString();
        if (!string.IsNullOrEmpty(edgeId))
        {
            _logger.LogInformation("🔗 Edge {EdgeId} clicked - connection details requested!", edgeId);
            // Edge interaction logic
        }
    }

    private double CalculateNodeSize(NetworkNode node)
    {
        // Base size plus scaling based on characteristics
        double baseSize = 2.0;
        
        // Scale based on number of open ports
        baseSize += node.OpenPorts.Count * 0.1;
        
        // Special sizing for different device types
        baseSize = node.DeviceType?.ToLower() switch
        {
            "router" or "gateway" => baseSize * 1.5,
            "server" => baseSize * 1.3,
            "switch" => baseSize * 1.2,
            _ => baseSize
        };
        
        // Anomaly nodes are larger
        if (node.Status == NodeStatus.Anomaly)
        {
            baseSize *= 1.4;
        }

        return Math.Max(1.0, Math.Min(baseSize, 6.0)); // Clamp between 1 and 6
    }

    #endregion
}

/// <summary>
/// Navigation event model for 3D interface interactions
/// </summary>
public class NavigationEvent
{
    public string Type { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public Dictionary<string, object>? Data { get; set; }
}