using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NetToolkit.Core.Events;
using NetToolkit.Core.Interfaces;
using NetToolkit.Modules.Education.Data;
using NetToolkit.Modules.Education.Interfaces;
using NetToolkit.Modules.Education.Models;
using Polly;
using Polly.Retry;

namespace NetToolkit.Modules.Education.Services;

/// <summary>
/// Gamification engine service - the cosmic conductor of achievement symphonies
/// Where learning milestones transform into legendary accomplishments with digital glory
/// </summary>
public class GamificationEngineService : IGamificationEngine
{
    private readonly EducationDbContext _dbContext;
    private readonly IEventBus _eventBus;
    private readonly ILogger<GamificationEngineService> _logger;
    private readonly ResiliencePipeline _retryPolicy;

    // Badge thresholds and configurations
    private static readonly Dictionary<string, Func<string, Achievement, bool>> BadgeCriteria = new()
    {
        // Module 1 badges
        ["network_newbie"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.ContainsKey("LessonId"),
        ["cable_connoisseur"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "5" && achievement.Value >= 80,
        ["ip_investigator"] = (userId, achievement) => achievement.Type == "LessonCompleted" && (achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "6" || achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "7") && achievement.Value == 100,
        ["osi_oracle"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "10" && achievement.Value >= 95,
        ["speed_demon"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("TimeSpent", 0).ToString() == "15" && int.Parse(achievement.Context.GetValueOrDefault("TimeSpent", "0").ToString()) <= 15,
        ["perfectionist"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Value == 100,
        
        // Module 2 badges (Hardware Heroes)
        ["hardware_rookie"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "21",
        ["nic_ninja"] = (userId, achievement) => achievement.Type == "LessonCompleted" && (achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "22" || achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "23") && achievement.Value >= 80,
        ["switch_sage"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "23" && achievement.Value >= 90,
        ["router_royalty"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "24" && achievement.Value == 100,
        ["troubleshooting_titan"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "35" && achievement.Value >= 90,
        ["cosmic_connector"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.ModuleId == 2 && achievement.Value == 100,
        
        // Module 3 badges (IP Shenanigans)
        ["ip_initiate"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "41",
        ["address_architect"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "42" && achievement.Value >= 80,
        ["dhcp_diplomat"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "45" && achievement.Value >= 85,
        ["subnet_sorcerer"] = (userId, achievement) => achievement.Type == "LessonCompleted" && (achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "46" || achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "47") && achievement.Value >= 90,
        ["cidr_champion"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "47" && achievement.Value == 100,
        ["nat_negotiator"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "49" && achievement.Value >= 85,
        ["arp_detective"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "50" && achievement.Value >= 80,
        ["multicast_maestro"] = (userId, achievement) => achievement.Type == "LessonCompleted" && (achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "53" || achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "54") && achievement.Value >= 85,
        ["powershell_wizard"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "56" && achievement.Value >= 90,
        ["troubleshooting_guru"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "57" && achievement.Value >= 85,
        ["ipv6_futurist"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "58" && achievement.Value >= 90,
        ["binary_mastermind"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "59" && achievement.Value >= 95,
        ["ip_impresario"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.ModuleId == 3 && achievement.Value == 100,
        ["addressing_ace"] = (userId, achievement) => achievement.Type == "ModuleCompleted" && achievement.ModuleId == 3 && achievement.Value >= 90,

        // Module 4 badges (Scripting Sorcery)
        ["script_apprentice"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "61",
        ["variable_virtuoso"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "62" && achievement.Value >= 70,
        ["command_conjurer"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "63" && achievement.Value >= 75,
        ["logic_luminary"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "65" && achievement.Value >= 80,
        ["loop_legend"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "66" && achievement.Value >= 80,
        ["function_phoenix"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "67" && achievement.Value >= 85,
        ["error_exorcist"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "68" && achievement.Value >= 85,
        ["network_necromancer"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "69" && achievement.Value >= 90,
        ["automation_archmage"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "72" && achievement.Value >= 90,
        ["module_monarch"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "74" && achievement.Value >= 90,
        ["debug_deity"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "75" && achievement.Value >= 95,
        ["remote_ruler"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "78" && achievement.Value >= 95,
        ["certification_champion"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "79" && achievement.Value >= 95,
        ["script_sorcerer_supreme"] = (userId, achievement) => achievement.Type == "ModuleCompleted" && achievement.ModuleId == 4 && achievement.Value >= 95,

        // Module 5 badges (Routing Riddles)
        ["path_seeker"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "81",
        ["static_navigator"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "82" && achievement.Value >= 70,
        ["dynamic_discoverer"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "83" && achievement.Value >= 75,
        ["gossip_guru"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "84" && achievement.Value >= 80,
        ["ospf_oracle"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "85" && achievement.Value >= 80,
        ["bgp_border_guardian"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "86" && achievement.Value >= 85,
        ["table_tactician"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "88" && achievement.Value >= 85,
        ["metric_mastermind"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "89" && achievement.Value >= 90,
        ["convergence_conductor"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "90" && achievement.Value >= 90,
        ["loop_liberator"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "91" && achievement.Value >= 90,
        ["acl_architect"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "92" && achievement.Value >= 95,
        ["script_pathweaver"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "93" && achievement.Value >= 95,
        ["troubleshoot_tracker"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "95" && achievement.Value >= 95,
        ["routing_riddle_master"] = (userId, achievement) => achievement.Type == "ModuleCompleted" && achievement.ModuleId == 5 && achievement.Value >= 95,

        // Module 6 badges (Security Shenanigans - Fortress Building 101)
        ["security_sentinel"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "101",
        ["firewall_guardian"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "102" && achievement.Value >= 75,
        ["encryption_enigma"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "103" && achievement.Value >= 80,
        ["tunnel_engineer"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "104" && achievement.Value >= 80,
        ["intrusion_investigator"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "105" && achievement.Value >= 85,
        ["vulnerability_vanquisher"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "106" && achievement.Value >= 85,
        ["malware_hunter"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "107" && achievement.Value >= 80,
        ["phishing_phantom"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "108" && achievement.Value >= 80,
        ["access_arbiter"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "109" && achievement.Value >= 85,
        ["two_factor_titan"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "110" && achievement.Value >= 80,
        ["script_shield_master"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "111" && achievement.Value >= 90,
        ["segmentation_sovereign"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "112" && achievement.Value >= 90,
        ["logging_librarian"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "113" && achievement.Value >= 85,
        ["pentest_paladin"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "114" && achievement.Value >= 90,
        ["zero_trust_zealot"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "115" && achievement.Value >= 90,
        ["cloud_castle_keeper"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "116" && achievement.Value >= 90,
        ["incident_commander"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "117" && achievement.Value >= 95,
        ["compliance_crusader"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "118" && achievement.Value >= 85,
        ["fortress_architect"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "119" && achievement.Value >= 95,
        ["security_shenanigan_master"] = (userId, achievement) => achievement.Type == "ModuleCompleted" && achievement.ModuleId == 6 && achievement.Value >= 95,
        
        // Module 7 badges (Wireless Wonders - Invisible Highways)
        ["wave_wanderer"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "121",
        ["home_wave_hero"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "122" && achievement.Value >= 70,
        ["standard_speed_seeker"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "123" && achievement.Value >= 75,
        ["frequency_finder"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "124" && achievement.Value >= 80,
        ["beacon_broadcaster"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "125" && achievement.Value >= 75,
        ["name_tag_navigator"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "126" && achievement.Value >= 70,
        ["encryption_air_guardian"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "127" && achievement.Value >= 85,
        ["mesh_web_weaver"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "128" && achievement.Value >= 80,
        ["bluetooth_whisperer"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "129" && achievement.Value >= 75,
        ["touch_spark_specialist"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "130" && achievement.Value >= 70,
        ["interference_investigator"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "131" && achievement.Value >= 80,
        ["roaming_relay_master"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "132" && achievement.Value >= 85,
        ["air_script_sorcerer"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "133" && achievement.Value >= 90,
        ["wireless_security_warrior"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "134" && achievement.Value >= 85,
        ["air_detective"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "135" && achievement.Value >= 85,
        ["future_signal_surfer"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "136" && achievement.Value >= 80,
        ["iot_connection_conductor"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "137" && achievement.Value >= 80,
        ["topology_pattern_master"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "138" && achievement.Value >= 85,
        ["protocol_wave_wizard"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "139" && achievement.Value >= 95,
        ["wireless_wonder_wizard"] = (userId, achievement) => achievement.Type == "ModuleCompleted" && achievement.ModuleId == 7 && achievement.Value >= 95,
        
        // Module 8 badges (Cloud Conquest - Sky-High Networks)
        ["sky_explorer"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "141",
        ["infrastructure_builder"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "142" && achievement.Value >= 70,
        ["platform_pioneer"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "143" && achievement.Value >= 70,
        ["software_sky_rider"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "144" && achievement.Value >= 70,
        ["azure_apprentice"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "145" && achievement.Value >= 75,
        ["aws_adventurer"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "146" && achievement.Value >= 75,
        ["gcp_genius"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "147" && achievement.Value >= 75,
        ["vm_virtuoso"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "148" && achievement.Value >= 80,
        ["container_captain"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "149" && achievement.Value >= 80,
        ["serverless_sorcerer"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "150" && achievement.Value >= 85,
        ["storage_sovereign"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "151" && achievement.Value >= 85,
        ["network_navigator"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "152" && achievement.Value >= 85,
        ["automation_archmage"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "153" && achievement.Value >= 90,
        ["security_sentinel"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "154" && achievement.Value >= 90,
        ["fog_clearer"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "155" && achievement.Value >= 90,
        ["hybrid_hero"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "156" && achievement.Value >= 90,
        ["cost_commander"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "157" && achievement.Value >= 90,
        ["edge_emperor"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "158" && achievement.Value >= 95,
        ["cloud_architect"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "159" && achievement.Value >= 95,
        ["cloud_conqueror"] = (userId, achievement) => achievement.Type == "ModuleCompleted" && achievement.ModuleId == 8 && achievement.Value >= 95,
        
        // Module 9 badges (Advanced Alchemy - Mixing Protocols)
        ["apprentice_alchemist"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "161",
        ["web_elixir_brewer"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "162" && achievement.Value >= 75,
        ["file_potion_master"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "163" && achievement.Value >= 75,
        ["communication_sage"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "164" && achievement.Value >= 80,
        ["network_services_wizard"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "165" && achievement.Value >= 85,
        ["tunnel_architect"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "167" && achievement.Value >= 90,
        ["qos_priority_mage"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "168" && achievement.Value >= 85,
        ["voice_blend_virtuoso"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "170" && achievement.Value >= 90,
        ["monitoring_oracle"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "169" && achievement.Value >= 80,
        ["script_enchanter"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "171" && achievement.Value >= 95,
        ["multiprotocol_architect"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "172" && achievement.Value >= 95,
        ["security_guardian"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "173" && achievement.Value >= 95,
        ["troubleshooting_detective"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "174" && achievement.Value >= 90,
        ["future_protocol_visionary"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "175" && achievement.Value >= 95,
        ["iot_miniaturist"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "176" && achievement.Value >= 90,
        ["cloud_protocol_master"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "177" && achievement.Value >= 95,
        ["stack_architect_supreme"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "178" && achievement.Value >= 95,
        ["certification_alchemist"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "179" && achievement.Value >= 95,
        ["grand_protocol_alchemist"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "180" && achievement.Value >= 95,
        ["alchemy_grandmaster"] = (userId, achievement) => achievement.Type == "ModuleCompleted" && achievement.ModuleId == 9 && achievement.Value >= 95,
        
        // Module 10 badges (Mastery Mayhem - Engineer Extraordinaire)
        ["foundation_overlord"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "181",
        ["hardware_commander"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "182" && achievement.Value >= 80,
        ["ip_emperor"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "183" && achievement.Value >= 85,
        ["script_sovereign"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "184" && achievement.Value >= 90,
        ["routing_regent"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "185" && achievement.Value >= 90,
        ["security_supreme"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "186" && achievement.Value >= 95,
        ["wireless_wizard_supreme"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "187" && achievement.Value >= 95,
        ["cloud_conqueror_supreme"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "188" && achievement.Value >= 95,
        ["protocol_potentate"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "189" && achievement.Value >= 95,
        ["integration_emperor"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "190" && achievement.Value >= 95,
        ["project_overlord"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "191" && achievement.Value >= 90,
        ["design_deity"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "192" && achievement.Value >= 95,
        ["security_sentinel"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "193" && achievement.Value >= 95,
        ["hybrid_hegemon"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "194" && achievement.Value >= 90,
        ["troubleshooting_titan"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "195" && achievement.Value >= 95,
        ["certification_champion"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "196" && achievement.Value >= 95,
        ["implementation_emperor"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "197" && achievement.Value >= 95,
        ["ethics_exemplar"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "198" && achievement.Value >= 90,
        ["future_prophet"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "199" && achievement.Value >= 95,
        ["engineer_extraordinaire"] = (userId, achievement) => achievement.Type == "LessonCompleted" && achievement.Context.GetValueOrDefault("LessonId", 0).ToString() == "200" && achievement.Value >= 98,
        ["network_overlord_supreme"] = (userId, achievement) => achievement.Type == "ModuleCompleted" && achievement.ModuleId == 10 && achievement.Value >= 98
    };

    public GamificationEngineService(
        EducationDbContext dbContext, 
        IEventBus eventBus, 
        ILogger<GamificationEngineService> logger)
    {
        _dbContext = dbContext;
        _eventBus = eventBus;
        _logger = logger;
        
        // Configure resilience policy (INFERRED: Polly v8 ResiliencePipeline)
        _retryPolicy = new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions
            {
                MaxRetryAttempts = 3,
                BackoffType = DelayBackoffType.Exponential,
                Delay = TimeSpan.FromSeconds(1),
                MaxDelay = TimeSpan.FromSeconds(30)
            })
            .Build();
    }

    /// <summary>
    /// Calculate quiz score with witty feedback
    /// </summary>
    public async Task<QuizScoreResult> CalculateQuizScoreAsync(
        Dictionary<string, int> answers, 
        Dictionary<string, int> correctAnswers,
        int timeSpentSeconds = 0)
    {
        try
        {

            var totalQuestions = correctAnswers.Count;
            var correctCount = 0;
            var incorrectAnswers = new List<string>();

            // Calculate correct answers
            foreach (var question in correctAnswers)
            {
                if (answers.TryGetValue(question.Key, out var userAnswer) && userAnswer == question.Value)
                {
                    correctCount++;
                }
                else
                {
                    incorrectAnswers.Add(question.Key);
                }
            }

            var percentage = totalQuestions > 0 ? (double)correctCount / totalQuestions * 100 : 0;
            var score = (int)Math.Round(percentage);

            // Calculate bonus points for speed
            var bonusPoints = CalculateSpeedBonus(timeSpentSeconds, totalQuestions);

            var result = new QuizScoreResult
            {
                Score = correctCount,
                MaxScore = totalQuestions,
                Percentage = percentage,
                WittyFeedback = GenerateWittyFeedback(percentage, timeSpentSeconds, totalQuestions),
                IncorrectAnswers = incorrectAnswers,
                PassedThreshold = percentage >= 70, // 70% passing threshold
                TimeSpent = TimeSpan.FromSeconds(timeSpentSeconds),
                BonusPoints = bonusPoints
            };

            _logger.LogDebug("Quiz score calculated: {Score}% ({Correct}/{Total}) with {Bonus} bonus points", 
                percentage, correctCount, totalQuestions, bonusPoints, result.WittyFeedback);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Failed to calculate quiz score - cosmic calculation malfunction!");
            
            return new QuizScoreResult
            {
                Score = 0,
                MaxScore = correctAnswers.Count,
                Percentage = 0,
                WittyFeedback = "Oops! The cosmic quiz calculator had a hiccup. Don't worry, your learning journey continues!",
                PassedThreshold = false
            };
        }
    }

    /// <summary>
    /// Award badges based on achievements
    /// </summary>
    public async Task<List<Badge>> AwardBadgesAsync(
        string userId, 
        Achievement achievement, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _retryPolicy.ExecuteAsync(async (context) =>
            {

                var newBadges = new List<Badge>();
                
                // Get all available badges for this module
                var availableBadges = await _dbContext.Badges
                    .Where(b => b.ModuleId == achievement.ModuleId)
                    .ToListAsync(cancellationToken);

                // Get already earned badges to avoid duplicates
                var earnedBadgeIds = await _dbContext.UserBadges
                    .Where(ub => ub.UserId == userId)
                    .Select(ub => ub.BadgeId)
                    .ToListAsync(cancellationToken);

                // Check each badge criteria
                foreach (var badge in availableBadges.Where(b => !earnedBadgeIds.Contains(b.Id)))
                {
                    if (BadgeCriteria.TryGetValue(badge.BadgeId, out var criteria) && criteria(userId, achievement))
                    {
                        // Special case for badges that require multiple achievements
                        if (await CheckSpecialBadgeCriteriaAsync(userId, badge, achievement, cancellationToken))
                        {
                            await AwardBadgeAsync(userId, badge, cancellationToken);
                            newBadges.Add(badge);
                        }
                    }
                    else if (await CheckGenericBadgeCriteriaAsync(userId, badge, achievement, cancellationToken))
                    {
                        await AwardBadgeAsync(userId, badge, cancellationToken);
                        newBadges.Add(badge);
                    }
                }

                _logger.LogInformation("🎉 Awarded {BadgeCount} new badges to user {UserId}: {BadgeNames}", 
                    newBadges.Count, userId, string.Join(", ", newBadges.Select(b => b.Name)));

                return newBadges;
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Failed to award badges to user {UserId} - cosmic badge ceremony interrupted!", userId);
            return new List<Badge>();
        }
    }

    /// <summary>
    /// Calculate and update learning streaks
    /// </summary>
    public async Task<StreakInfo> UpdateStreakAsync(
        string userId, 
        DateTime activityDate, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _retryPolicy.ExecuteAsync(async (context) =>
            {

                // Get or create user streak record
                var existingStreak = await _dbContext.UserStreaks
                    .FirstOrDefaultAsync(us => us.UserId == userId, cancellationToken);
                var streakRecord = existingStreak is null ? 
                    new NetToolkit.Modules.Education.Models.UserStreak { UserId = userId, LastActivityDate = DateTime.UtcNow } : 
                    existingStreak;

                var today = activityDate.Date;
                var yesterday = today.AddDays(-1);
                
                // Calculate streak based on activity pattern
                if (streakRecord.LastActivityDate != DateTime.MinValue)
                {
                    var lastActivity = streakRecord.LastActivityDate.Date;
                    
                    if (lastActivity == today)
                    {
                        // Already counted for today, no change
                    }
                    else if (lastActivity == yesterday)
                    {
                        // Consecutive day, increment streak
                        streakRecord.CurrentStreak++;
                        streakRecord.LastActivityDate = today;
                        
                        if (streakRecord.CurrentStreak > streakRecord.LongestStreak)
                        {
                            streakRecord.LongestStreak = streakRecord.CurrentStreak;
                        }
                        
                        _logger.LogDebug("Streak increased to {Streak} days for user {UserId}", 
                            streakRecord.CurrentStreak, userId);
                    }
                    else if (lastActivity < yesterday)
                    {
                        // Streak broken, reset to 1
                        streakRecord.CurrentStreak = 1;
                        streakRecord.LastActivityDate = today;
                        
                        _logger.LogInformation("💔 Streak reset to 1 day for user {UserId} - but every expert has setbacks!", userId);
                    }
                }
                else
                {
                    // First activity
                    streakRecord.CurrentStreak = 1;
                    streakRecord.LongestStreak = 1;
                    streakRecord.LastActivityDate = today;
                    streakRecord.StreakStartDate = today;
                    
                    _logger.LogInformation("🌟 New learning streak started for user {UserId}!", userId);
                }

                // Save or update streak record
                if (streakRecord.Id == 0)
                {
                    _dbContext.UserStreaks.Add(streakRecord);
                }
                else
                {
                    _dbContext.UserStreaks.Update(streakRecord);
                }
                
                await _dbContext.SaveChangesAsync(cancellationToken);

                var streakInfo = new StreakInfo
                {
                    CurrentStreak = streakRecord.CurrentStreak,
                    LongestStreak = streakRecord.LongestStreak,
                    LastActivity = streakRecord.LastActivityDate,
                    StreakStartDate = streakRecord.StreakStartDate,
                    IsActive = streakRecord.LastActivityDate.Date == today,
                    ActivityDates = await GetRecentActivityDatesAsync(userId, cancellationToken)
                };

                // Publish streak milestone events
                await PublishStreakMilestonesAsync(userId, streakInfo, cancellationToken);

                return streakInfo;
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Failed to update streak for user {UserId} - cosmic streak tracker malfunction!", userId);
            
            return new StreakInfo
            {
                CurrentStreak = 0,
                LongestStreak = 0,
                LastActivity = activityDate,
                IsActive = false
            };
        }
    }

    /// <summary>
    /// Generate motivational engagement messages
    /// </summary>
    public async Task<MotivationMessage> GenerateMotivationAsync(
        ModuleProgress userProgress, 
        LearningActivity recentActivity)
    {
        try
        {

            var completionPercentage = userProgress.CompletionPercentage;
            var streak = await UpdateStreakAsync(userProgress.UserId, DateTime.UtcNow);
            
            var motivationType = DetermineMotivationType(completionPercentage, streak, recentActivity);
            var message = GenerateMotivationMessage(motivationType, completionPercentage, streak, recentActivity);


            return message;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Failed to generate motivation message - but your cosmic journey continues!");
            
            return new MotivationMessage
            {
                PrimaryMessage = "Keep exploring the networking cosmos!",
                SecondaryMessage = "Every step forward is progress.",
                Type = MotivationType.Encouragement,
                Emoji = "🌟"
            };
        }
    }

    /// <summary>
    /// Calculate learning rank based on overall performance
    /// </summary>
    public async Task<LearningRank> CalculateLearningRankAsync(
        string userId, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _retryPolicy.ExecuteAsync(async (context) =>
            {

                // Get user's learning statistics
                var moduleProgresses = await _dbContext.ModuleProgress
                    .Include(mp => mp.LessonProgresses)
                    .Where(mp => mp.UserId == userId)
                    .ToListAsync(cancellationToken);

                var totalLessons = moduleProgresses.SelectMany(mp => mp.LessonProgresses).Count();
                var completedLessons = moduleProgresses.SelectMany(mp => mp.LessonProgresses)
                    .Count(lp => lp.Status == LessonStatus.Completed);
                
                var averageScore = completedLessons > 0 
                    ? moduleProgresses.SelectMany(mp => mp.LessonProgresses)
                        .Where(lp => lp.Status == LessonStatus.Completed)
                        .Average(lp => lp.QuizScore)
                    : 0;

                var badgeCount = await _dbContext.UserBadges
                    .CountAsync(ub => ub.UserId == userId, cancellationToken);

                var streak = await UpdateStreakAsync(userId, DateTime.UtcNow, cancellationToken);

                // Calculate rank points
                var rankPoints = CalculateRankPoints(completedLessons, averageScore, badgeCount, streak.LongestStreak);
                var (rankLevel, rankName, description, icon) = DetermineRank(rankPoints);

                var learningRank = new LearningRank
                {
                    RankName = rankName,
                    RankLevel = rankLevel,
                    Description = description,
                    Icon = icon,
                    PointsToNext = CalculatePointsToNextRank(rankLevel, rankPoints),
                    NextRankName = GetNextRankName(rankLevel)
                };

                _logger.LogInformation("🏆 User {UserId} achieved rank: {Rank} (Level {Level}) with {Points} points", 
                    userId, rankName, rankLevel, rankPoints);

                return learningRank;
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Failed to calculate learning rank for user {UserId} - cosmic ranking system offline!", userId);
            
            return new LearningRank
            {
                RankName = "Cosmic Explorer",
                RankLevel = 0,
                Description = "🌟 On a journey through the digital universe!",
                Icon = "🌟"
            };
        }
    }

    /// <summary>
    /// Get leaderboard data for friendly competition
    /// </summary>
    public async Task<List<LeaderboardEntry>> GetLeaderboardAsync(
        int? moduleId = null,
        LeaderboardTimeframe timeframe = LeaderboardTimeframe.AllTime,
        int limit = 10,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _retryPolicy.ExecuteAsync(async (context) =>
            {

                var query = _dbContext.ModuleProgress
                    .Include(mp => mp.LessonProgresses)
                    .AsQueryable();

                // Apply module filter if specified
                if (moduleId.HasValue)
                {
                    query = query.Where(mp => mp.ModuleId == moduleId.Value);
                }

                // Apply timeframe filter
                var cutoffDate = GetTimeframeCutoffDate(timeframe);
                if (cutoffDate.HasValue)
                {
                    query = query.Where(mp => mp.LastAccessedAt >= cutoffDate.Value);
                }

                // Calculate leaderboard entries
                var leaderboardData = await query
                    .GroupBy(mp => mp.UserId)
                    .Select(g => new
                    {
                        UserId = g.Key,
                        CompletedLessons = g.SelectMany(mp => mp.LessonProgresses).Count(lp => lp.Status == LessonStatus.Completed),
                        AverageScore = g.SelectMany(mp => mp.LessonProgresses)
                            .Where(lp => lp.Status == LessonStatus.Completed)
                            .Average(lp => lp.QuizScore),
                        LastActivity = g.Max(mp => mp.LastAccessedAt)
                    })
                    .OrderByDescending(x => x.CompletedLessons)
                    .ThenByDescending(x => x.AverageScore)
                    .Take(limit)
                    .ToListAsync(cancellationToken);

                var leaderboard = new List<LeaderboardEntry>();
                var position = 1;

                foreach (var entry in leaderboardData)
                {
                    var badgeCount = await _dbContext.UserBadges
                        .CountAsync(ub => ub.UserId == entry.UserId, cancellationToken);
                    
                    var rank = await CalculateLearningRankAsync(entry.UserId, cancellationToken);
                    
                    leaderboard.Add(new LeaderboardEntry
                    {
                        UserId = entry.UserId,
                        UserName = $"User{entry.UserId.GetHashCode():X8}", // Anonymized for privacy
                        Position = position++,
                        Score = (int)(entry.CompletedLessons * 10 + entry.AverageScore),
                        Achievement = $"{entry.CompletedLessons} lessons completed",
                        Rank = rank,
                        BadgeCount = badgeCount,
                        LastActivity = entry.LastActivity
                    });
                }


                return leaderboard;
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Failed to generate leaderboard - cosmic competition tracker offline!");
            return new List<LeaderboardEntry>();
        }
    }

    /// <summary>
    /// Analyze learning patterns and suggest improvements
    /// </summary>
    public async Task<LearningAnalytics> AnalyzeLearningPatternsAsync(
        string userId, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _retryPolicy.ExecuteAsync(async (context) =>
            {

                var lessonProgresses = await _dbContext.LessonProgress
                    .Include(lp => lp.Lesson)
                    .Where(lp => lp.UserId == userId && lp.Status == LessonStatus.Completed)
                    .ToListAsync(cancellationToken);

                var analytics = new LearningAnalytics
                {
                    UserId = userId,
                    StrengthAreas = AnalyzeStrengthAreas(lessonProgresses),
                    ImprovementAreas = AnalyzeImprovementAreas(lessonProgresses),
                    PreferredPattern = DeterminePreferredLearningPattern(lessonProgresses),
                    OptimalLearningDuration = CalculateOptimalLearningDuration(lessonProgresses),
                    Recommendations = GeneratePersonalizedRecommendations(lessonProgresses),
                    PersonalizedTips = GeneratePersonalizedTips(lessonProgresses)
                };

                _logger.LogInformation("📈 Learning analytics completed for user {UserId}: {StrengthCount} strengths, {ImprovementCount} improvement areas", 
                    userId, analytics.StrengthAreas.Count, analytics.ImprovementAreas.Count);

                return analytics;
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Failed to analyze learning patterns for user {UserId} - cosmic analytics offline!", userId);
            
            return new LearningAnalytics
            {
                UserId = userId,
                Recommendations = new List<string> { "Keep exploring the networking cosmos with curiosity and determination!" },
                PersonalizedTips = new List<string> { "Learning is a journey, not a destination. Every step counts!" }
            };
        }
    }

    /// <summary>
    /// Check for milestone achievements
    /// </summary>
    public async Task<MilestoneResult> CheckMilestoneAsync(
        string userId, 
        Milestone milestone, 
        CancellationToken cancellationToken = default)
    {
        try
        {

            var currentValue = await GetMilestoneCurrentValueAsync(userId, milestone, cancellationToken);
            var achieved = currentValue >= milestone.TargetValue;

            var result = new MilestoneResult
            {
                Achieved = achieved,
                Milestone = milestone,
                CelebrationMessage = achieved ? GenerateMilestoneCelebration(milestone) : "",
                RewardBadges = achieved ? await GetMilestoneRewardBadgesAsync(milestone, cancellationToken) : new List<Badge>(),
                BonusPoints = achieved ? CalculateMilestoneBonusPoints(milestone) : 0
            };

            if (achieved)
            {
                _logger.LogInformation("🏆 Milestone achieved: User {UserId} completed '{MilestoneName}' with value {Value}/{Target}!", 
                    userId, milestone.Name, currentValue, milestone.TargetValue);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Failed to check milestone for user {UserId} - cosmic milestone tracker malfunction!", userId);
            
            return new MilestoneResult
            {
                Achieved = false,
                Milestone = milestone,
                CelebrationMessage = "The cosmic milestone checker experienced a temporary glitch, but your progress continues!"
            };
        }
    }

    /// <summary>
    /// Generate daily learning challenges
    /// </summary>
    public async Task<List<DailyChallenge>> GenerateDailyChallengesAsync(
        string userId, 
        DifficultyLevel difficulty = DifficultyLevel.Intermediate,
        CancellationToken cancellationToken = default)
    {
        try
        {

            var challenges = new List<DailyChallenge>();
            var random = new Random();

            // Get user's recent activity to personalize challenges
            var recentProgress = await _dbContext.LessonProgress
                .Where(lp => lp.UserId == userId && lp.LastAccessedAt >= DateTime.UtcNow.AddDays(-7))
                .ToListAsync(cancellationToken);

            // Generate 3 daily challenges
            var challengeTypes = Enum.GetValues<ChallengeType>().OrderBy(x => random.Next()).Take(3);

            foreach (var challengeType in challengeTypes)
            {
                challenges.Add(GenerateDailyChallenge(challengeType, difficulty, recentProgress));
            }

            _logger.LogInformation("🌟 Generated {ChallengeCount} daily challenges for user {UserId}", challenges.Count, userId);

            return challenges;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Failed to generate daily challenges for user {UserId} - cosmic challenge generator offline!", userId);
            
            return new List<DailyChallenge>
            {
                new()
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = "Cosmic Learning Quest",
                    Description = "Continue your networking journey with any lesson!",
                    Type = ChallengeType.Explorer,
                    Difficulty = difficulty,
                    PointReward = 50,
                    ExpiresAt = DateTime.UtcNow.AddDays(1)
                }
            };
        }
    }

    /// <summary>
    /// Track engagement metrics for learning optimization
    /// </summary>
    public async Task<EngagementProfile> TrackEngagementAsync(
        string userId, 
        EngagementData engagementData, 
        CancellationToken cancellationToken = default)
    {
        try
        {

            // Get or create engagement profile
            var existingProfile = await _dbContext.UserEngagementProfiles
                .FirstOrDefaultAsync(uep => uep.UserId == userId, cancellationToken);
            var profile = existingProfile is null ? 
                new NetToolkit.Modules.Education.Models.UserEngagementProfile { UserId = userId } : 
                existingProfile;

            // Update engagement metrics
            UpdateEngagementMetrics(profile, engagementData);

            // Calculate engagement level
            profile.Level = CalculateEngagementLevel(profile);

            // Save profile
            if (profile.Id == 0)
            {
                _dbContext.UserEngagementProfiles.Add(profile);
            }
            else
            {
                _dbContext.UserEngagementProfiles.Update(profile);
            }

            await _dbContext.SaveChangesAsync(cancellationToken);

            var engagementProfile = new EngagementProfile
            {
                UserId = userId,
                Level = profile.Level,
                PreferredFeatures = DeserializeFeaturePreferences(profile.PreferredFeatures),
                AverageSessionDuration = TimeSpan.FromMinutes(profile.AverageSessionMinutes),
                AttentionScore = profile.AttentionScore,
                OptimizationTips = GenerateOptimizationTips(profile)
            };


            return engagementProfile;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Failed to track engagement for user {UserId} - cosmic engagement tracker offline!", userId);
            
            return new EngagementProfile
            {
                UserId = userId,
                Level = EngagementLevel.Moderate,
                OptimizationTips = new List<string> { "Keep exploring and learning at your own pace!" }
            };
        }
    }

    #region Private Helper Methods

    private int CalculateSpeedBonus(int timeSpentSeconds, int questionCount)
    {
        if (questionCount == 0) return 0;

        var averageTimePerQuestion = timeSpentSeconds / (double)questionCount;
        var optimalTime = 60; // 60 seconds per question is optimal

        if (averageTimePerQuestion <= optimalTime * 0.5) // Super fast
            return 20;
        if (averageTimePerQuestion <= optimalTime * 0.75) // Fast
            return 10;
        if (averageTimePerQuestion <= optimalTime) // Optimal
            return 5;
        
        return 0; // No bonus for slow completion
    }

    private string GenerateWittyFeedback(double percentage, int timeSpentSeconds, int totalQuestions, int moduleId = 0, int lessonId = 0)
    {
        var timePerQuestion = totalQuestions > 0 ? timeSpentSeconds / (double)totalQuestions : 0;

        // Module 7 (Wireless Wonders) specific feedback
        if (moduleId == 7)
        {
            return percentage switch
            {
                >= 95 when timePerQuestion <= 30 => "📡 Signal strong! Lightning-fast wireless mastery - you're riding the invisible highways at light speed!",
                >= 95 => "🌟 Wireless wonder achieved! You're a true master of the invisible realm!",
                >= 85 when timePerQuestion <= 45 => "⚡ Excellent signal strength! You're surfing the wireless waves with incredible speed and precision!",
                >= 85 => "📶 Signal excellent! You're broadcasting brilliance across all frequencies!",
                >= 75 => "🎯 Good signal detected! Your wireless knowledge is transmitting beautifully!",
                >= 65 => "📱 Connection established! Keep learning and you'll be a wireless wizard!",
                >= 50 => "💫 Weak but stable signal! Practice more to boost your wireless power!",
                _ => "🔄 Signal lost! Don't worry - even the best networks need reconnection time. Try again, wave wanderer!"
            };
        }

        // Module 8 (Cloud Conquest) specific feedback
        if (moduleId == 8)
        {
            return percentage switch
            {
                >= 95 when timePerQuestion <= 30 => "☁️ Sky cleared! Lightning-fast cloud mastery - you're conquering the digital heavens at warp speed!",
                >= 95 => "👑 Cloud conquest achieved! You reign supreme over all sky-high networks!",
                >= 85 when timePerQuestion <= 45 => "⚡ Excellent altitude! You're soaring through cloud architectures with incredible speed and precision!",
                >= 85 => "🏰 Sky fortress secured! Your cloud knowledge reaches the highest heavens!",
                >= 75 => "🎯 Good elevation gained! Your cloud wisdom is ascending beautifully!",
                >= 65 => "🌤️ Clouds parting! Keep learning and you'll be a sky sovereign!",
                >= 50 => "🌫️ Foggy but ascending! Practice more to clear the cloud confusion!",
                _ => "⛈️ Storm clouds gathering! Don't worry - every cloud has a silver lining. Clear skies await, sky explorer!"
            };
        }

        // Module 9 (Advanced Alchemy - Mixing Protocols) specific feedback
        if (moduleId == 9)
        {
            return percentage switch
            {
                >= 95 when timePerQuestion <= 30 => "🧪 Perfect potion! Lightning-fast alchemy mastery - you're brewing protocols at supernatural speed!",
                >= 95 => "👑 Grand Alchemist achieved! You've mastered the mystical arts of protocol mixing!",
                >= 85 when timePerQuestion <= 45 => "⚗️ Excellent elixir! You're mixing protocols with incredible speed and precision!",
                >= 85 => "🌟 Masterful mixing! Your protocol alchemy creates pure magic!",
                >= 75 => "🔮 Good brewing technique! Your alchemical skills are developing beautifully!",
                >= 65 => "🧙‍♂️ Apprentice progress! Keep practicing and you'll be a protocol wizard!",
                >= 50 => "💫 Potion bubbling! Practice more to perfect your protocol recipes!",
                _ => "💨 Brew fizzled! Don't worry - every master alchemist has had failed experiments. Mix again, future grandmaster!"
            };
        }

        // Module 10 (Mastery Mayhem - Engineer Extraordinaire) specific feedback
        if (moduleId == 10)
        {
            return percentage switch
            {
                >= 98 when timePerQuestion <= 30 => "👑 SUPREME OVERLORD ACHIEVED! Lightning-fast mastery mayhem - you've transcended mortal engineering limitations!",
                >= 98 => "🌟 ENGINEER EXTRAORDINAIRE! You've achieved the ultimate pinnacle of network engineering mastery!",
                >= 95 when timePerQuestion <= 45 => "⚡ LEGENDARY MASTERY! You're orchestrating network empires with godlike speed and precision!",
                >= 95 => "🏆 MAYHEM MASTERED! Your engineering prowess creates order from digital chaos!",
                >= 85 when timePerQuestion <= 60 => "🚀 Excellent empire building! You're commanding network domains with imperial precision!",
                >= 85 => "👨‍💼 Outstanding engineering! Your mastery brings harmony to complex network challenges!",
                >= 75 => "🎯 Good mayhem management! Your engineering skills are reaching extraordinary levels!",
                >= 65 => "📈 Empire expanding! Keep building and you'll rule the network realm supreme!",
                >= 50 => "💪 Foundation solid! Practice more to unleash your full engineering extraordinaire potential!",
                _ => "🔄 Mayhem needs mastery! Even overlords started as apprentices. Conquer the chaos and rise again, future extraordinaire!"
            };
        }

        // Default general feedback
        return percentage switch
        {
            >= 95 when timePerQuestion <= 30 => "🚀 Lightning-fast brilliance! You're a networking speed demon!",
            >= 95 => "🌟 Absolutely stellar! You're a networking virtuoso!",
            >= 85 when timePerQuestion <= 45 => "⚡ Great speed and accuracy! You're mastering the digital realm!",
            >= 85 => "⭐ Excellent work! You're conquering the networking cosmos!",
            >= 75 => "🎯 Great job! Your network knowledge is expanding beautifully!",
            >= 65 => "📚 Good effort! Keep learning and you'll be a network ninja!",
            >= 50 => "💪 Not bad! Practice makes perfect in the networking universe!",
            _ => "🌱 Every expert was once a beginner - keep growing your network wisdom!"
        };
    }

    private async Task<bool> CheckSpecialBadgeCriteriaAsync(string userId, Badge badge, Achievement achievement, CancellationToken cancellationToken)
    {
        return badge.BadgeId switch
        {
            // Module 1 special badges
            "basics_boss" => await CheckBasicsBossEligibilityAsync(userId, cancellationToken),
            "speed_demon" => await CheckSpeedDemonEligibilityAsync(userId, cancellationToken),
            "perfectionist" => await CheckPerfectionistEligibilityAsync(userId, cancellationToken),
            
            // Module 2 special badges
            "hardware_hulk" => await CheckHardwareHulkEligibilityAsync(userId, cancellationToken),
            "troubleshooting_titan" => await CheckTroubleshootingTitanEligibilityAsync(userId, cancellationToken),
            "cosmic_connector" => await CheckCosmicConnectorEligibilityAsync(userId, cancellationToken),
            
            // Module 7 special badges
            "wireless_wonder_wizard" => await CheckWirelessWonderWizardEligibilityAsync(userId, cancellationToken),
            "air_script_sorcerer" => await CheckAirScriptSorcererEligibilityAsync(userId, cancellationToken),
            "protocol_wave_wizard" => await CheckProtocolWaveWizardEligibilityAsync(userId, cancellationToken),
            
            _ => false
        };
    }

    private async Task<bool> CheckGenericBadgeCriteriaAsync(string userId, Badge badge, Achievement achievement, CancellationToken cancellationToken)
    {
        // Generic criteria checking for badges without specific logic
        return false; // Placeholder for generic badge logic
    }

    private async Task<bool> CheckBasicsBossEligibilityAsync(string userId, CancellationToken cancellationToken)
    {
        var moduleProgress = await _dbContext.ModuleProgress
            .Include(mp => mp.LessonProgresses)
            .FirstOrDefaultAsync(mp => mp.UserId == userId && mp.ModuleId == 1, cancellationToken);

        if (moduleProgress == null) return false;

        var completedLessons = moduleProgress.LessonProgresses.Where(lp => lp.Status == LessonStatus.Completed);
        var averageScore = completedLessons.Any() ? completedLessons.Average(lp => lp.QuizScore) : 0;

        return moduleProgress.CompletedLessons >= moduleProgress.TotalLessons && averageScore >= 85;
    }

    private async Task<bool> CheckSpeedDemonEligibilityAsync(string userId, CancellationToken cancellationToken)
    {
        var recentLessons = await _dbContext.LessonProgress
            .Where(lp => lp.UserId == userId && 
                        lp.Status == LessonStatus.Completed && 
                        lp.CompletedAt >= DateTime.UtcNow.AddHours(-1))
            .ToListAsync(cancellationToken);

        return recentLessons.Count >= 3 && recentLessons.Sum(lp => lp.TimeSpentMinutes) <= 45;
    }

    private async Task<bool> CheckPerfectionistEligibilityAsync(string userId, CancellationToken cancellationToken)
    {
        var perfectScores = await _dbContext.LessonProgress
            .CountAsync(lp => lp.UserId == userId && lp.QuizScore == 100, cancellationToken);

        return perfectScores >= 5;
    }

    private async Task<bool> CheckHardwareHulkEligibilityAsync(string userId, CancellationToken cancellationToken)
    {
        var moduleProgress = await _dbContext.ModuleProgress
            .Include(mp => mp.LessonProgresses)
            .FirstOrDefaultAsync(mp => mp.UserId == userId && mp.ModuleId == 2, cancellationToken);

        if (moduleProgress == null) return false;

        var completedLessons = moduleProgress.LessonProgresses.Where(lp => lp.Status == LessonStatus.Completed);
        var averageScore = completedLessons.Any() ? completedLessons.Average(lp => lp.QuizScore) : 0;

        return moduleProgress.CompletedLessons >= moduleProgress.TotalLessons && averageScore >= 85;
    }

    private async Task<bool> CheckTroubleshootingTitanEligibilityAsync(string userId, CancellationToken cancellationToken)
    {
        // Check for high scores on troubleshooting lessons (lessons 15, 17 in Module 2)
        var troubleshootingLessons = await _dbContext.LessonProgress
            .Where(lp => lp.UserId == userId && 
                        lp.Status == LessonStatus.Completed && 
                        (lp.LessonId == 35 || lp.LessonId == 37)) // Lesson 15 and 17 of Module 2
            .ToListAsync(cancellationToken);

        return troubleshootingLessons.Count >= 2 && troubleshootingLessons.All(lp => lp.QuizScore >= 90);
    }

    private async Task<bool> CheckCosmicConnectorEligibilityAsync(string userId, CancellationToken cancellationToken)
    {
        // Check for perfect understanding of all connection types (Module 2 lessons about cables, NICs, switches)
        var connectionLessons = await _dbContext.LessonProgress
            .Where(lp => lp.UserId == userId && 
                        lp.Status == LessonStatus.Completed && 
                        (lp.LessonId >= 22 && lp.LessonId <= 32)) // NIC, Switch, Router, Cable lessons
            .ToListAsync(cancellationToken);

        return connectionLessons.Count >= 8 && connectionLessons.All(lp => lp.QuizScore == 100);
    }

    private async Task AwardBadgeAsync(string userId, Badge badge, CancellationToken cancellationToken)
    {
        var userBadge = new UserBadge
        {
            UserId = userId,
            BadgeId = badge.Id,
            Badge = badge,
            AwardedAt = DateTime.UtcNow
        };

        _dbContext.UserBadges.Add(userBadge);
        await _dbContext.SaveChangesAsync(cancellationToken);

        // Publish badge awarded event
        await _eventBus.PublishAsync(new BadgeUnlockedEvent
        {
            UserId = userId,
            BadgeName = badge.Name,
            BadgeDescription = badge.Description,
            Rarity = badge.Rarity,
            AwardedAt = userBadge.AwardedAt
        }, cancellationToken);

        _logger.LogInformation("🏆 Badge '{BadgeName}' awarded to user {UserId} - {RewardMessage}", 
            badge.Name, userId, badge.RewardMessage);
    }

    private async Task<List<DateTime>> GetRecentActivityDatesAsync(string userId, CancellationToken cancellationToken)
    {
        return await _dbContext.LessonProgress
            .Where(lp => lp.UserId == userId && lp.LastAccessedAt >= DateTime.UtcNow.AddDays(-30))
            .Select(lp => lp.LastAccessedAt.Date)
            .Distinct()
            .OrderByDescending(d => d)
            .ToListAsync(cancellationToken);
    }

    private async Task PublishStreakMilestonesAsync(string userId, StreakInfo streakInfo, CancellationToken cancellationToken)
    {
        var milestoneValues = new[] { 3, 7, 14, 30, 50, 100 };
        
        if (milestoneValues.Contains(streakInfo.CurrentStreak))
        {
            await _eventBus.PublishAsync(new StreakMilestoneEvent
            {
                UserId = userId,
                StreakDays = streakInfo.CurrentStreak,
                MilestoneMessage = streakInfo.GetStreakMessage(),
                AchievedAt = DateTime.UtcNow
            }, cancellationToken);
        }
    }

    // Additional helper methods continue...
    private MotivationType DetermineMotivationType(double completionPercentage, StreakInfo streak, LearningActivity recentActivity)
    {
        if (streak.CurrentStreak >= 7) return MotivationType.Streak;
        if (completionPercentage >= 90) return MotivationType.Achievement;
        if (recentActivity.Performance >= 90) return MotivationType.Achievement;
        if (streak.CurrentStreak == 0) return MotivationType.Comeback;
        
        return MotivationType.Encouragement;
    }

    private MotivationMessage GenerateMotivationMessage(MotivationType type, double completion, StreakInfo streak, LearningActivity activity)
    {
        return type switch
        {
            MotivationType.Streak => new MotivationMessage
            {
                PrimaryMessage = $"🔥 {streak.CurrentStreak} days of cosmic learning!",
                SecondaryMessage = "Your dedication is inspiring the networking universe!",
                Type = type,
                Emoji = "🔥",
                Priority = 3
            },
            MotivationType.Achievement => new MotivationMessage
            {
                PrimaryMessage = "🏆 Outstanding progress achieved!",
                SecondaryMessage = "You're mastering the networking cosmos with excellence!",
                Type = type,
                Emoji = "🏆",
                Priority = 4
            },
            MotivationType.Comeback => new MotivationMessage
            {
                PrimaryMessage = "🌟 Welcome back, cosmic learner!",
                SecondaryMessage = "Every journey has its pauses - let's continue exploring!",
                Type = type,
                Emoji = "🌟",
                Priority = 2
            },
            _ => new MotivationMessage
            {
                PrimaryMessage = "💪 Keep pushing forward!",
                SecondaryMessage = "Every step brings you closer to networking mastery!",
                Type = type,
                Emoji = "💪",
                Priority = 1
            }
        };
    }

    private int CalculateRankPoints(int completedLessons, double averageScore, int badgeCount, int longestStreak)
    {
        var lessonPoints = completedLessons * 10;
        var scorePoints = (int)(averageScore * completedLessons / 10);
        var badgePoints = badgeCount * 50;
        var streakPoints = longestStreak * 5;

        return lessonPoints + scorePoints + badgePoints + streakPoints;
    }

    private (int level, string name, string description, string icon) DetermineRank(int points)
    {
        return points switch
        {
            >= 2000 => (7, "Network Nexus", "🌌 Transcendent master of the networking cosmos!", "🌌"),
            >= 1500 => (6, "Data Deity", "⚡ Omnipotent overseer of digital dimensions!", "⚡"),
            >= 1000 => (5, "Cyber Sage", "🧙‍♂️ Wise wizard of wireless wonders!", "🧙‍♂️"),
            >= 700 => (4, "Network Ninja", "🥷 Stealthy master of network mysteries!", "🥷"),
            >= 400 => (3, "Byte Boss", "👑 Commanding respect in digital realms!", "👑"),
            >= 200 => (2, "Packet Pioneer", "🚀 Blazing trails through network territories!", "🚀"),
            >= 100 => (1, "Digital Disciple", "📚 Knowledge seeker extraordinaire!", "📚"),
            _ => (0, "Network Newbie", "🌱 Everyone starts somewhere - you're growing!", "🌱")
        };
    }

    private int CalculatePointsToNextRank(int currentLevel, int currentPoints)
    {
        var thresholds = new[] { 100, 200, 400, 700, 1000, 1500, 2000 };
        
        if (currentLevel >= thresholds.Length) return 0;
        
        return thresholds[currentLevel] - currentPoints;
    }

    private string GetNextRankName(int currentLevel)
    {
        var ranks = LearningRank.Ranks;
        return currentLevel + 1 < ranks.Count ? ranks[currentLevel + 1].Name : "Maximum Rank Achieved";
    }

    private DateTime? GetTimeframeCutoffDate(LeaderboardTimeframe timeframe)
    {
        return timeframe switch
        {
            LeaderboardTimeframe.Daily => DateTime.UtcNow.AddDays(-1),
            LeaderboardTimeframe.Weekly => DateTime.UtcNow.AddDays(-7),
            LeaderboardTimeframe.Monthly => DateTime.UtcNow.AddDays(-30),
            LeaderboardTimeframe.AllTime => null,
            _ => null
        };
    }

    // Simplified implementations for remaining methods
    private Dictionary<string, double> AnalyzeStrengthAreas(List<LessonProgress> progresses) => new();
    private Dictionary<string, double> AnalyzeImprovementAreas(List<LessonProgress> progresses) => new();
    private LearningPattern DeterminePreferredLearningPattern(List<LessonProgress> progresses) => LearningPattern.Visual;
    private TimeSpan CalculateOptimalLearningDuration(List<LessonProgress> progresses) => TimeSpan.FromMinutes(20);
    private List<string> GeneratePersonalizedRecommendations(List<LessonProgress> progresses) => new();
    private List<string> GeneratePersonalizedTips(List<LessonProgress> progresses) => new();
    private async Task<int> GetMilestoneCurrentValueAsync(string userId, Milestone milestone, CancellationToken ct) => 0;
    private string GenerateMilestoneCelebration(Milestone milestone) => $"🎉 Milestone achieved: {milestone.Name}!";
    private async Task<List<Badge>> GetMilestoneRewardBadgesAsync(Milestone milestone, CancellationToken ct) => new();
    private int CalculateMilestoneBonusPoints(Milestone milestone) => 100;
    private DailyChallenge GenerateDailyChallenge(ChallengeType type, DifficultyLevel difficulty, List<LessonProgress> recent) => 
        new() { Id = Guid.NewGuid().ToString(), Title = $"{type} Challenge", Type = type, Difficulty = difficulty };
    
    private void UpdateEngagementMetrics(NetToolkit.Modules.Education.Models.UserEngagementProfile profile, EngagementData data)
    {
        profile.TotalActivities++;
        profile.LastActivity = DateTime.UtcNow;
        profile.AttentionScore = (profile.AttentionScore + data.FocusScore) / 2;
    }

    private EngagementLevel CalculateEngagementLevel(NetToolkit.Modules.Education.Models.UserEngagementProfile profile) => 
        profile.AttentionScore > 0.8 ? EngagementLevel.Highly : EngagementLevel.Moderate;
    
    private Dictionary<string, double> DeserializeFeaturePreferences(string json) => new();
    private List<string> GenerateOptimizationTips(NetToolkit.Modules.Education.Models.UserEngagementProfile profile) => 
        new() { "Keep up the great learning momentum!" };

    private async Task<bool> CheckWirelessWonderWizardEligibilityAsync(string userId, CancellationToken cancellationToken)
    {
        // Check for mastery of all Module 7 wireless concepts with high average score
        var moduleProgress = await _dbContext.ModuleProgress
            .Include(mp => mp.LessonProgresses)
            .FirstOrDefaultAsync(mp => mp.UserId == userId && mp.ModuleId == 7, cancellationToken);

        if (moduleProgress == null) return false;

        var completedLessons = moduleProgress.LessonProgresses.Where(lp => lp.Status == LessonStatus.Completed);
        var averageScore = completedLessons.Any() ? completedLessons.Average(lp => lp.QuizScore) : 0;

        return moduleProgress.CompletedLessons >= moduleProgress.TotalLessons && averageScore >= 95;
    }

    private async Task<bool> CheckAirScriptSorcererEligibilityAsync(string userId, CancellationToken cancellationToken)
    {
        // Check for high scores on wireless scripting lessons (lesson 13: Scripting Wireless: Air Commands)
        var scriptingLessons = await _dbContext.LessonProgress
            .Where(lp => lp.UserId == userId && 
                        lp.Status == LessonStatus.Completed && 
                        lp.LessonId == 133) // Lesson 13 of Module 7: Scripting Wireless
            .ToListAsync(cancellationToken);

        return scriptingLessons.Any() && scriptingLessons.All(lp => lp.QuizScore >= 90);
    }

    private async Task<bool> CheckProtocolWaveWizardEligibilityAsync(string userId, CancellationToken cancellationToken)
    {
        // Check for mastery of advanced wireless protocols (lesson 19: Cert-Level: WiFi Protocols)
        var protocolLessons = await _dbContext.LessonProgress
            .Where(lp => lp.UserId == userId && 
                        lp.Status == LessonStatus.Completed && 
                        lp.LessonId == 139) // Lesson 19 of Module 7: Cert-Level WiFi Protocols
            .ToListAsync(cancellationToken);

        return protocolLessons.Any() && protocolLessons.All(lp => lp.QuizScore >= 95);
    }

    #endregion
}

// Additional model classes for gamification
public class UserStreak
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int CurrentStreak { get; set; }
    public int LongestStreak { get; set; }
    public DateTime? LastActivityDate { get; set; }
    public DateTime StreakStartDate { get; set; }
}

public class UserEngagementProfile
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public EngagementLevel Level { get; set; }
    public string PreferredFeatures { get; set; } = "{}";
    public double AverageSessionMinutes { get; set; }
    public double AttentionScore { get; set; }
    public int TotalActivities { get; set; }
    public DateTime LastActivity { get; set; }
}

public class StreakMilestoneEvent
{
    public string UserId { get; set; } = string.Empty;
    public int StreakDays { get; set; }
    public string MilestoneMessage { get; set; } = string.Empty;
    public DateTime AchievedAt { get; set; }
}