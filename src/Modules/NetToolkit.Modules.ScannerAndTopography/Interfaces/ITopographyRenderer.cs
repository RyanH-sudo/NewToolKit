using NetToolkit.Modules.ScannerAndTopography.Models;

namespace NetToolkit.Modules.ScannerAndTopography.Interfaces;

/// <summary>
/// Topography renderer interface - transforming data into 3D digital cosmos
/// Where topology results ascend to Three.js celestial realms
/// </summary>
public interface ITopographyRenderer
{
    /// <summary>
    /// Serialize topology to JSON - preparing data for the 3D metamorphosis
    /// </summary>
    /// <param name="result">Topology result to serialize</param>
    /// <returns>JSON string ready for Three.js consumption - cosmic coordinates encoded</returns>
    string SerializeToJson(TopologyResult result);
    
    /// <summary>
    /// Handle navigation events from 3D interface - responding to cosmic interactions
    /// </summary>
    /// <param name="eventJson">Navigation event in JSON format</param>
    /// <returns>Task representing the celestial response</returns>
    Task HandleNavigationEventAsync(string eventJson, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Generate Three.js initialization script - birthing the digital universe
    /// </summary>
    /// <param name="topology">Topology data for 3D rendering</param>
    /// <param name="containerElementId">HTML element ID for Three.js canvas</param>
    /// <returns>JavaScript code to initialize the 3D scene - the cosmic genesis script</returns>
    string GenerateThreeJsScript(TopologyResult topology, string containerElementId = "topology-container");
    
    /// <summary>
    /// Update 3D scene with new data - evolutionary cosmos adaptation
    /// </summary>
    /// <param name="updates">Topology updates to apply</param>
    /// <returns>JavaScript commands for scene updates - digital evolution commands</returns>
    string GenerateUpdateScript(TopologyResult updates);
    
    /// <summary>
    /// Calculate optimal 3D positioning for nodes - cosmic choreography algorithms
    /// </summary>
    /// <param name="nodes">Network nodes requiring positioning</param>
    /// <param name="edges">Connections influencing layout</param>
    /// <returns>Nodes with computed 3D positions - harmonious digital constellation</returns>
    List<NetworkNode> CalculateOptimalPositioning(List<NetworkNode> nodes, List<NetworkEdge> edges);
}