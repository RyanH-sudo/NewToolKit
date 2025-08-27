using NetToolkit.Modules.Education.Models;

namespace NetToolkit.Modules.Education.Data;

/// <summary>
/// Module 10 content seed data - "Mastery Mayhem - Engineer Extraordinaire"
/// The ultimate capstone module that synthesizes all previous learning into mastery projects
/// </summary>
public static class Module10ContentSeed
{
    /// <summary>
    /// Get the complete Module 10 content structure
    /// </summary>
    public static Module GetModule10Content()
    {
        return new Module
        {
            Id = 10,
            Title = "Mastery Mayhem - Engineer Extraordinaire",
            Description = "The ultimate capstone journey that transforms you into a network engineering overlord! Synthesize all previous modules into comprehensive mastery projects, advanced troubleshooting scenarios, real-world implementations, and certification-level challenges. From basic recap reviews to complex integrated solutions, become the supreme Engineer Extraordinaire!",
            Difficulty = DifficultyLevel.Expert,
            EstimatedMinutes = 1200, // 20 hours total - the ultimate capstone experience
            Prerequisites = "Modules 1-9", // Requires ALL previous knowledge
            LearningOutcomes = "Master comprehensive network engineering synthesis, advanced project implementation, real-world troubleshooting, certification-level knowledge integration, ethical engineering practices, emerging technology awareness, and ultimate Engineer Extraordinaire capabilities.",
            CreatedAt = DateTime.UtcNow,
            Lessons = GetAllMasteryMayhemLessons()
        };
    }

    /// <summary>
    /// Get all 20 mastery mayhem capstone lessons
    /// </summary>
    private static List<Lesson> GetAllMasteryMayhemLessons()
    {
        var lessons = new List<Lesson>();

        // Lesson 1: Recap Basics: From Wires to Wonders
        lessons.Add(new Lesson
        {
            Id = 181, // Starting from 181 for Module 10
            ModuleId = 10,
            LessonNumber = 1,
            Title = "Recap Basics: From Wires to Wonders",
            Content = "Welcome to the ultimate mastery journey! Let's revisit the foundational networking concepts that form the bedrock of your engineering empire.",
            ImageDescription = "full network cartoon with connections and wonder",
            EstimatedMinutes = 35,
            Slides = new List<Slide>
            {
                new()
                {
                    Type = ContentType.Image,
                    Title = "The Network Engineering Empire",
                    Content = "Image: A comprehensive cartoon network showing the full journey from cables to clouds",
                    Data = "full_network_cartoon_empire.jpg"
                },
                new()
                {
                    Type = ContentType.Text,
                    Title = "From Humble Beginnings to Network Mastery",
                    Content = @"
                        <div class='mastery-recap'>
                            <h3>🏰 Welcome to Your Network Engineering Empire!</h3>
                            <p>You've journeyed from networking neophyte to approaching network nobility! Let's revisit the foundational building blocks that support your growing empire of knowledge.</p>
                            
                            <div class='knowledge-foundation'>
                                <h4>🧱 The Foundation Stones of Your Empire</h4>
                                <ul>
                                    <li><span class='hover-tip' data-tip='The physical layer - where electrons become information and cables carry digital dreams!'>Physical Connections: Cables, switches, and the tangible backbone of networks</span></li>
                                    <li><span class='hover-tip' data-tip='The logical layer - where addresses become destinations and data finds its way home!'>Logical Addressing: IP addresses, subnets, and the addressing hierarchy</span></li>
                                    <li><span class='hover-tip' data-tip='The communication layer - where protocols become conversations and data becomes meaningful!'>Protocol Communication: How devices speak the same digital language</span></li>
                                    <li><span class='hover-tip' data-tip='The security layer - where trust becomes verified and safety becomes systematic!'>Security Foundations: Authentication, authorization, and protection</span></li>
                                </ul>
                            </div>
                            
                            <div class='mastery-perspective'>
                                <h4>🎯 The Mastery Perspective</h4>
                                <p><strong>Remember:</strong> Every network engineering master began exactly where you started - with curiosity about how devices connect and communicate. Now you're ready to orchestrate complex network symphonies!</p>
                            </div>
                        </div>
                    ",
                    HoverTips = new List<HoverTip>
                    {
                        new() { Word = "network engineering", Explanation = "The art and science of designing, implementing, and maintaining computer networks - like being the architect of digital highways!" },
                        new() { Word = "empire", Explanation = "Your growing domain of network knowledge and skills - every lesson adds another territory to your expertise!" },
                        new() { Word = "foundation stones", Explanation = "The core concepts that support all advanced networking - like the bedrock upon which your expertise castle is built!" }
                    }
                },
                new()
                {
                    Type = ContentType.Quiz,
                    Title = "Foundation Mastery Check",
                    Data = @"{
                        'question': 'What are the fundamental layers that form the foundation of network engineering mastery?',
                        'options': [
                            'Only physical cables and wireless connections',
                            'Physical connections, logical addressing, protocol communication, and security foundations',
                            'Just IP addresses and routing protocols',
                            'Only software applications and user interfaces'
                        ],
                        'correctAnswer': 1,
                        'explanation': 'Perfect mastery recall! Network engineering foundation includes physical connections (cables/wireless), logical addressing (IP/subnets), protocol communication (how devices talk), and security foundations (protection/authentication) - all working together to create your network empire!'
                    }"
                }
            },
            Prerequisites = new List<int>(),
            Tags = new List<string> { "recap", "mastery", "foundations", "synthesis", "engineering" }
        });

        // Lesson 2: Hardware Mastery: Gadget Command
        lessons.Add(new Lesson
        {
            Id = 182,
            ModuleId = 10,
            LessonNumber = 2,
            Title = "Hardware Mastery: Gadget Command",
            Content = "Command your network hardware army like a true engineering overlord! Master the art of hardware orchestration and device dominion.",
            ImageDescription = "hero gadgets with crowns and command staffs",
            EstimatedMinutes = 40,
            Slides = new List<Slide>
            {
                new()
                {
                    Type = ContentType.Image,
                    Title = "Hardware Heroes Assembly",
                    Content = "Image: Network devices as superhero characters with command staffs and royal insignia",
                    Data = "hardware_heroes_command.jpg"
                },
                new()
                {
                    Type = ContentType.Text,
                    Title = "Your Hardware Command Center",
                    Content = @"
                        <div class='hardware-mastery'>
                            <h3>👑 Master of the Hardware Realm</h3>
                            <p>You now possess the knowledge to command an entire army of network devices! Each piece of hardware serves your greater networking vision.</p>
                            
                            <div class='device-command'>
                                <h4>🎮 Your Device Command Structure</h4>
                                <ul>
                                    <li><span class='hover-tip' data-tip='Your loyal foot soldiers - they connect every device to your network empire!'>Network Interface Cards (NICs): Your communication ambassadors</span></li>
                                    <li><span class='hover-tip' data-tip='Your traffic directors - they know where every data packet should go in your network kingdom!'>Switches: Your intelligent traffic conductors</span></li>
                                    <li><span class='hover-tip' data-tip='Your border guards - they protect your network realm and control what enters and leaves!'>Routers: Your pathway commanders and border guardians</span></li>
                                    <li><span class='hover-tip' data-tip='Your fortress walls - they inspect and protect against digital invaders!'>Firewalls: Your network fortress defenders</span></li>
                                    <li><span class='hover-tip' data-tip='Your aerial command - they extend your empire through invisible airways!'>Access Points: Your wireless realm extenders</span></li>
                                    <li><span class='hover-tip' data-tip='Your treasure vaults - they store and serve the digital wealth of your empire!'>Servers: Your data kingdom vaults</span></li>
                                </ul>
                            </div>
                            
                            <div class='mastery-integration'>
                                <h4>⚡ Integration Mastery</h4>
                                <p><strong>Advanced Skill:</strong> You can now design hardware architectures that seamlessly blend physical and virtual components, creating hybrid empires that span from data centers to cloud kingdoms!</p>
                            </div>
                        </div>
                    ",
                    HoverTips = new List<HoverTip>
                    {
                        new() { Word = "hardware command", Explanation = "The ability to orchestrate and manage all network devices as an integrated system - like conducting a symphony of silicon and circuits!" },
                        new() { Word = "device army", Explanation = "Your collection of network hardware working together toward common networking goals - each device plays its specialized role!" },
                        new() { Word = "hybrid empires", Explanation = "Modern networks that blend physical hardware with virtualized components and cloud services - the future of network architecture!" }
                    }
                },
                new()
                {
                    Type = ContentType.Quiz,
                    Title = "Hardware Command Assessment",
                    Data = @"{
                        'question': 'As a hardware master, what is the most important principle for commanding your device army?',
                        'options': [
                            'Use only the most expensive equipment available',
                            'Each device serves a specific role in your integrated network architecture',
                            'Replace all hardware with cloud services immediately',
                            'Focus only on wireless devices and ignore wired infrastructure'
                        ],
                        'correctAnswer': 1,
                        'explanation': 'Excellent hardware mastery! Each device serves a specific role in your integrated architecture - NICs connect, switches direct, routers route, firewalls protect, access points extend, and servers serve. True mastery comes from orchestrating them as a unified system!'
                    }"
                }
            },
            Prerequisites = new List<int> { 181 },
            Tags = new List<string> { "hardware", "mastery", "devices", "integration", "command" }
        });

        // Add remaining 18 lessons with basic structure
        var remainingLessons = new[]
        {
            (3, "IP Mayhem: Address Overlord", "IP addresses forming a royal crown"),
            (4, "Scripting Supreme: Spell Sovereign", "ultimate magical wand with code flowing"),
            (5, "Routing Reign: Path Emperor", "routing table forming a throne"),
            (6, "Security Supremacy: Fortress Lord", "armored castle with shields and guards"),
            (7, "Wireless Wizardry: Air Overlord", "wireless waves forming a crown of power"),
            (8, "Cloud Command: Sky Emperor", "cloud infrastructure forming a heavenly throne"),
            (9, "Protocol Potency: Mix Master", "ultimate alchemical potion with protocol symbols"),
            (10, "Integrated Mayhem: Full Synthesis", "all network elements fused into one powerful diagram"),
            (11, "Scripting Projects: Mayhem Builds", "code castle with automation towers"),
            (12, "Network Designs: Blueprint Mayhem", "complex architectural blueprints with network topology"),
            (13, "Security Audits: Defense Mayhem", "security audit shield with testing symbols"),
            (14, "Wireless/Cloud Hybrids: Sky-Air Mayhem", "cloud and wireless waves integrated"),
            (15, "Troubleshooting Mastery: Fix Mayhem", "detective with magnifying glass and crown"),
            (16, "Certification Prep: Exam Mayhem", "certification chaos with study materials"),
            (17, "Real-World Projects: Applied Mayhem", "real network implementation with tools"),
            (18, "Ethical Engineering: Responsible Mayhem", "balanced scales with network ethics symbols"),
            (19, "Future Trends: Emerging Mayhem", "futuristic network with AI and quantum elements"),
            (20, "Final Quiz Mayhem: Overlord Certification", "supreme trophy with all module symbols")
        };

        foreach (var (lessonNum, title, imageDesc) in remainingLessons)
        {
            lessons.Add(new Lesson
            {
                Id = 180 + lessonNum,
                ModuleId = 10,
                LessonNumber = lessonNum,
                Title = title,
                Content = $"Master the ultimate synthesis of {title.ToLower()} in this capstone engineering lesson.",
                ImageDescription = imageDesc,
                EstimatedMinutes = lessonNum < 19 ? 50 : 60, // Final lessons are longest
                Slides = new List<Slide>
                {
                    new()
                    {
                        Type = ContentType.Text,
                        Title = title,
                        Content = $"Comprehensive mastery synthesis for {title} - ultimate engineering extraordinaire knowledge coming soon!",
                        HoverTips = new List<HoverTip>
                        {
                            new() { Word = "mastery", Explanation = "The highest level of skill and understanding - where knowledge becomes wisdom and practice becomes art!" }
                        }
                    }
                },
                Prerequisites = new List<int> { 180 + lessonNum - 1 },
                Tags = new List<string> { "mastery", "synthesis", "capstone", "engineering", "extraordinaire" }
            });
        }

        return lessons;
    }

    /// <summary>
    /// Get Module 10 badges for ultimate mastery achievements
    /// </summary>
    public static List<Badge> GetModule10Badges()
    {
        return new List<Badge>
        {
            new()
            {
                Id = "foundation_overlord",
                Name = "Foundation Overlord",
                Description = "Mastered the fundamental building blocks of network engineering",
                Icon = "🏰",
                Category = "Foundation Mastery",
                Points = 75
            },
            new()
            {
                Id = "hardware_commander",
                Name = "Hardware Commander",
                Description = "Achieved supreme command over all network hardware devices",
                Icon = "👑",
                Category = "Hardware Mastery",
                Points = 85
            },
            new()
            {
                Id = "ip_emperor",
                Name = "IP Emperor",
                Description = "Conquered the vast empire of IP addressing and network management",
                Icon = "🌐",
                Category = "Addressing Mastery",
                Points = 90
            },
            new()
            {
                Id = "script_sovereign",
                Name = "Script Sovereign",
                Description = "Achieved sovereign mastery of network automation and scripting",
                Icon = "🪄",
                Category = "Automation Mastery",
                Points = 95
            },
            new()
            {
                Id = "routing_regent",
                Name = "Routing Regent",
                Description = "Reigned supreme over all routing protocols and path optimization",
                Icon = "🛣️",
                Category = "Routing Mastery",
                Points = 100
            },
            new()
            {
                Id = "security_supreme",
                Name = "Security Supreme",
                Description = "Achieved supreme mastery of network security and defense",
                Icon = "🛡️",
                Category = "Security Mastery",
                Points = 105
            },
            new()
            {
                Id = "wireless_wizard_supreme",
                Name = "Wireless Wizard Supreme",
                Description = "Mastered the mystical arts of wireless networking and invisible communications",
                Icon = "📶",
                Category = "Wireless Mastery",
                Points = 110
            },
            new()
            {
                Id = "cloud_conqueror_supreme",
                Name = "Cloud Conqueror Supreme",
                Description = "Conquered the highest peaks of cloud computing and sky-high architectures",
                Icon = "☁️",
                Category = "Cloud Mastery",
                Points = 115
            },
            new()
            {
                Id = "protocol_potentate",
                Name = "Protocol Potentate",
                Description = "Achieved potentate status in the realm of protocol mixing and alchemy",
                Icon = "⚗️",
                Category = "Protocol Mastery",
                Points = 120
            },
            new()
            {
                Id = "integration_emperor",
                Name = "Integration Emperor",
                Description = "Mastered the art of integrating all network technologies into unified solutions",
                Icon = "🔗",
                Category = "Integration Mastery",
                Points = 125
            },
            new()
            {
                Id = "project_overlord",
                Name = "Project Overlord",
                Description = "Commanded real-world network projects with masterful execution",
                Icon = "🏗️",
                Category = "Project Mastery",
                Points = 130
            },
            new()
            {
                Id = "design_deity",
                Name = "Design Deity",
                Description = "Achieved divine status in network architecture and blueprint creation",
                Icon = "📐",
                Category = "Design Mastery",
                Points = 135
            },
            new()
            {
                Id = "security_sentinel",
                Name = "Security Sentinel",
                Description = "Became the ultimate guardian through advanced security auditing mastery",
                Icon = "🔒",
                Category = "Audit Mastery",
                Points = 140
            },
            new()
            {
                Id = "hybrid_hegemon",
                Name = "Hybrid Hegemon",
                Description = "Achieved hegemony over hybrid cloud-wireless network architectures",
                Icon = "🌤️",
                Category = "Hybrid Mastery",
                Points = 145
            },
            new()
            {
                Id = "troubleshooting_titan",
                Name = "Troubleshooting Titan",
                Description = "Became a titan of network troubleshooting and problem resolution",
                Icon = "🔍",
                Category = "Troubleshooting Mastery",
                Points = 150
            },
            new()
            {
                Id = "certification_champion",
                Name = "Certification Champion",
                Description = "Conquered certification-level challenges with champion expertise",
                Icon = "🏆",
                Category = "Certification Readiness",
                Points = 155
            },
            new()
            {
                Id = "implementation_emperor",
                Name = "Implementation Emperor",
                Description = "Mastered real-world network implementation with imperial precision",
                Icon = "⚙️",
                Category = "Implementation Mastery",
                Points = 160
            },
            new()
            {
                Id = "ethics_exemplar",
                Name = "Ethics Exemplar",
                Description = "Exemplified the highest standards of ethical network engineering",
                Icon = "⚖️",
                Category = "Professional Ethics",
                Points = 165
            },
            new()
            {
                Id = "future_prophet",
                Name = "Future Prophet",
                Description = "Prophesied the future of networking with visionary mastery",
                Icon = "🔮",
                Category = "Future Vision",
                Points = 170
            },
            new()
            {
                Id = "engineer_extraordinaire",
                Name = "Engineer Extraordinaire",
                Description = "Achieved the ultimate title of Network Engineering Extraordinaire - master of all domains",
                Icon = "👨‍💼",
                Category = "Ultimate Mastery",
                Points = 500
            },
            new()
            {
                Id = "network_overlord_supreme",
                Name = "Network Overlord Supreme",
                Description = "Transcended all limitations to become the supreme overlord of network engineering",
                Icon = "🌟",
                Category = "Legendary Achievement",
                Points = 1000
            }
        };
    }
}