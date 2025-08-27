using NetToolkit.Modules.Education.Models;

namespace NetToolkit.Modules.Education.Data;

/// <summary>
/// Module 9 content seed data - "Advanced Alchemy - Mixing Protocols"
/// A cosmic journey through protocol combinations with alchemical wit and wisdom
/// </summary>
public static class Module9ContentSeed
{
    /// <summary>
    /// Get the complete Module 9 content structure
    /// </summary>
    public static Module GetModule9Content()
    {
        return new Module
        {
            Id = 9,
            Title = "Advanced Alchemy - Mixing Protocols",
            Description = "Master the mystical art of protocol mixing! Learn to brew powerful network elixirs by combining HTTP/HTTPS, FTP/SFTP, DNS/DHCP, VPN tunnels, QoS potions, VoIP blends, and advanced protocol stacks. From simple web elixirs to complex multiprotocol hybrid brews, become the supreme alchemist of network communications!",
            Difficulty = DifficultyLevel.Expert,
            EstimatedMinutes = 1000, // 16.7 hours total - the most advanced module
            Prerequisites = "Modules 1-8", // Requires all previous knowledge
            LearningOutcomes = "Master protocol combinations, HTTP/HTTPS security, FTP/SFTP transfers, DNS/DHCP services, VPN tunneling, QoS prioritization, VoIP communications, SNMP monitoring, multiprotocol design, troubleshooting techniques, and certification-level protocol architecture.",
            CreatedAt = DateTime.UtcNow,
            Lessons = GetAllAlchemyLessons()
        };
    }

    /// <summary>
    /// Get all 20 advanced alchemy lessons
    /// </summary>
    private static List<Lesson> GetAllAlchemyLessons()
    {
        var lessons = new List<Lesson>();

        // Lesson 1: What's Protocol Mixing? Potion Blends
        lessons.Add(new Lesson
        {
            Id = 161, // Starting from 161 for Module 9
            ModuleId = 9,
            LessonNumber = 1,
            Title = "What's Protocol Mixing? Potion Blends",
            Content = "Welcome to the mystical laboratory of protocol alchemy! Learn the ancient art of combining network protocols to create powerful communication elixirs.",
            ImageDescription = "cauldron with protocol ingredients and potion bottles",
            EstimatedMinutes = 30,
            Slides = new List<Slide>
            {
                new()
                {
                    Type = ContentType.Image,
                    Title = "The Alchemical Network Laboratory",
                    Content = "Image: A magical cauldron bubbling with protocol ingredients",
                    Data = "protocol_alchemy_cauldron.jpg"
                },
                new()
                {
                    Type = ContentType.Text,
                    Title = "The Art of Protocol Mixing",
                    Content = @"
                        <div class='alchemy-intro'>
                            <h3>🧪 Welcome to Protocol Alchemy!</h3>
                            <p>Protocol mixing is like being a master alchemist - you combine different communication methods to create powerful network elixirs! Just as an alchemist mixes ingredients to brew magical potions, network engineers mix protocols to create robust, secure, and efficient communications.</p>
                            
                            <div class='protocol-examples'>
                                <h4>🔮 Classic Protocol Blends</h4>
                                <ul>
                                    <li><span class='hover-tip' data-tip='The foundation of all internet communication - reliable delivery guaranteed!'>TCP/IP: The Universal Communication Elixir</span></li>
                                    <li><span class='hover-tip' data-tip='Combines web protocols with security magic for safe browsing'>HTTP + SSL/TLS: The Secure Web Potion</span></li>
                                    <li><span class='hover-tip' data-tip='Mixes file transfer with SSH encryption for protected delivery'>FTP + SSH: The Armored File Carrier</span></li>
                                    <li><span class='hover-tip' data-tip='Blends voice data with network protocols for digital conversations'>VoIP: The Speaking Elixir</span></li>
                                </ul>
                            </div>
                            
                            <div class='alchemical-wisdom'>
                                <h4>💡 Alchemical Wisdom</h4>
                                <p><strong>Remember:</strong> The best protocol mixes balance reliability, security, and performance - like the perfect potion that works every time!</p>
                            </div>
                        </div>
                    ",
                    HoverTips = new List<HoverTip>
                    {
                        new() { Word = "protocol mixing", Explanation = "The art of combining different communication protocols to create more powerful, secure, or efficient network solutions - like mixing ingredients for the perfect potion!" },
                        new() { Word = "TCP/IP", Explanation = "The fundamental protocol blend that makes the internet work - Transmission Control Protocol over Internet Protocol, ensuring reliable delivery of data packets!" },
                        new() { Word = "elixirs", Explanation = "In our alchemical world, these are the powerful communication solutions created by mixing protocols - each one crafted for specific network magic!" }
                    }
                },
                new()
                {
                    Type = ContentType.Quiz,
                    Title = "Test Your Alchemical Knowledge",
                    Data = @"{
                        'question': 'What is protocol mixing in network alchemy?',
                        'options': [
                            'Physically combining network cables together',
                            'Combining different communication protocols to create better solutions',
                            'Mixing different types of network hardware',
                            'Creating chemical solutions for cleaning network equipment'
                        ],
                        'correctAnswer': 1,
                        'explanation': 'Perfect! Protocol mixing is the art of combining different communication protocols to create more powerful, secure, or efficient network solutions - just like an alchemist mixing ingredients for the perfect potion!'
                    }"
                }
            },
            Prerequisites = new List<int>(),
            Tags = new List<string> { "protocols", "alchemy", "mixing", "fundamentals", "introduction" }
        });

        // Lesson 2: HTTP/HTTPS: Web Elixirs
        lessons.Add(new Lesson
        {
            Id = 162,
            ModuleId = 9,
            LessonNumber = 2,
            Title = "HTTP/HTTPS: Web Elixirs",
            Content = "Brew the fundamental web elixirs! Learn how HTTP and HTTPS combine to create secure web communication potions.",
            ImageDescription = "web potion with lock symbol and browser elements",
            EstimatedMinutes = 35,
            Slides = new List<Slide>
            {
                new()
                {
                    Type = ContentType.Image,
                    Title = "The Web Elixir Laboratory",
                    Content = "Image: A glowing web potion with padlock floating above it",
                    Data = "web_security_potion.jpg"
                },
                new()
                {
                    Type = ContentType.Text,
                    Title = "HTTP vs HTTPS: The Security Enhancement",
                    Content = @"
                        <div class='web-alchemy'>
                            <h3>🌐 The Web Protocol Elixirs</h3>
                            <p>HTTP and HTTPS are like two versions of the same magical potion - one basic, one with powerful security enchantments!</p>
                            
                            <div class='protocol-comparison'>
                                <div class='basic-potion'>
                                    <h4>🧪 HTTP: The Basic Web Elixir</h4>
                                    <ul>
                                        <li>Plain text communication - like talking in the open</li>
                                        <li>Fast and simple - no encryption overhead</li>
                                        <li>Perfect for non-sensitive content</li>
                                        <li>Port 80 - the traditional web gateway</li>
                                    </ul>
                                </div>
                                
                                <div class='secure-potion'>
                                    <h4>🔐 HTTPS: The Armored Web Elixir</h4>
                                    <ul>
                                        <li><span class='hover-tip' data-tip='SSL/TLS encryption creates a secure tunnel for data'>Encrypted communication - like whispering secrets in a magical bubble</span></li>
                                        <li><span class='hover-tip' data-tip='Certificates verify the server identity like a magical seal'>Authentication - proves the server is who it claims to be</span></li>
                                        <li><span class='hover-tip' data-tip='Data integrity ensures information cannot be tampered with'>Data integrity - prevents tampering during transport</span></li>
                                        <li>Port 443 - the secure web gateway</li>
                                    </ul>
                                </div>
                            </div>
                            
                            <div class='alchemical-formula'>
                                <h4>🔮 The Alchemical Formula</h4>
                                <p><strong>HTTPS = HTTP + SSL/TLS + Certificates</strong></p>
                                <p>Like adding protective spells to your basic communication potion!</p>
                            </div>
                        </div>
                    ",
                    HoverTips = new List<HoverTip>
                    {
                        new() { Word = "HTTP", Explanation = "HyperText Transfer Protocol - the basic language that web browsers and servers use to communicate, like the fundamental spell for web magic!" },
                        new() { Word = "HTTPS", Explanation = "HTTP Secure - the armored version of HTTP with SSL/TLS encryption, authentication, and data integrity protection!" },
                        new() { Word = "SSL/TLS", Explanation = "Secure Socket Layer/Transport Layer Security - the encryption magic that creates secure tunnels for web communication!" }
                    }
                },
                new()
                {
                    Type = ContentType.Quiz,
                    Title = "Web Elixir Mastery Test",
                    Data = @"{
                        'question': 'What makes HTTPS more secure than HTTP?',
                        'options': [
                            'It uses a different port number',
                            'It adds SSL/TLS encryption, authentication, and data integrity',
                            'It transmits data faster than HTTP',
                            'It only works with specific web browsers'
                        ],
                        'correctAnswer': 1,
                        'explanation': 'Excellent alchemy! HTTPS adds SSL/TLS encryption (secure tunnels), authentication (server verification), and data integrity (tamper protection) to basic HTTP - like adding powerful protective enchantments to your web potion!'
                    }"
                }
            ],
            Prerequisites = new List<int> { 161 },
            Tags = new List<string> { "HTTP", "HTTPS", "web", "security", "encryption", "SSL", "TLS" }
        });

        // Add remaining 18 lessons with titles and basic structure
        var remainingLessons = new[]
        {
            (3, "FTP/SFTP: File Transfer Elixirs", "file bottles with security armor"),
            (4, "SMTP/IMAP: Email Potions", "mail elixir with envelope bubbles"),
            (5, "DNS: Name Resolver Magic", "name tag potion with address book"),
            (6, "DHCP: Auto-Assign Brew", "self-filling cauldron with IP addresses"),
            (7, "VPN Protocols: Tunnel Elixirs", "tunnel potion with encrypted pathways"),
            (8, "QoS: Priority Potions", "VIP elixir with priority lanes"),
            (9, "SNMP: Monitoring Magic", "crystal ball in monitoring potion"),
            (10, "VoIP: Voice Blends", "voice bubbles floating from communication potion"),
            (11, "Scripting Mixes: Protocol Spells", "magical wand stirring code cauldron"),
            (12, "Multiprotocol: Hybrid Elixirs", "multi-colored layered potion with labels"),
            (13, "Security in Mixes: Safe Blends", "shielded cauldron with protective barriers"),
            (14, "Troubleshooting Mixes: Brew Diagnostics", "detective magnifying glass over protocol potion"),
            (15, "Emerging Protocols: Future Potions", "futuristic glowing elixir with holographic elements"),
            (16, "IoT Protocols: Tiny Device Blends", "miniature potion bottles for smart devices"),
            (17, "Cloud Protocols: Sky-High Mixes", "floating cloud cauldron with API streams"),
            (18, "Protocol Stacks: Layered Alchemy", "transparent stacked potion bottles showing layers"),
            (19, "Cert-Level: Master Alchemist Design", "advanced laboratory with complex protocol apparatus"),
            (20, "Quiz Alchemy: Protocol Mastery", "golden master alchemist potion with certification seal")
        };

        foreach (var (lessonNum, title, imageDesc) in remainingLessons)
        {
            lessons.Add(new Lesson
            {
                Id = 160 + lessonNum,
                ModuleId = 9,
                LessonNumber = lessonNum,
                Title = title,
                Content = $"Master the advanced alchemical arts of {title.ToLower()} in this comprehensive protocol mixing lesson.",
                ImageDescription = imageDesc,
                EstimatedMinutes = lessonNum < 19 ? 40 : 50, // Final lessons are longer
                Slides = new List<Slide>
                {
                    new()
                    {
                        Type = ContentType.Text,
                        Title = title,
                        Content = $"Comprehensive alchemical knowledge for {title} - advanced protocol mixing mastery coming soon!",
                        HoverTips = new List<HoverTip>
                        {
                            new() { Word = "protocol", Explanation = "Communication rules and standards that enable different network devices to understand each other - like the universal language of network magic!" }
                        }
                    }
                },
                Prerequisites = lessonNum == 3 ? new List<int> { 162 } : new List<int> { 160 + lessonNum - 1 },
                Tags = new List<string> { "protocols", "alchemy", "advanced", "mixing" }
            });
        }

        return lessons;
    }

    /// <summary>
    /// Get Module 9 badges for protocol alchemy mastery
    /// </summary>
    public static List<Badge> GetModule9Badges()
    {
        return new List<Badge>
        {
            new()
            {
                Id = "apprentice_alchemist",
                Name = "Apprentice Alchemist",
                Description = "Began the mystical journey into protocol mixing arts",
                Icon = "🧪",
                Category = "Alchemy Basics",
                Points = 60
            },
            new()
            {
                Id = "web_elixir_brewer",
                Name = "Web Elixir Brewer",
                Description = "Mastered the sacred arts of HTTP/HTTPS mixing",
                Icon = "🌐",
                Category = "Web Protocols",
                Points = 80
            },
            new()
            {
                Id = "file_potion_master",
                Name = "File Potion Master",
                Description = "Perfected FTP/SFTP transfer elixirs and secure file alchemy",
                Icon = "📁",
                Category = "File Transfer",
                Points = 80
            },
            new()
            {
                Id = "communication_sage",
                Name = "Communication Sage",
                Description = "Achieved mastery of email and messaging protocol potions",
                Icon = "✉️",
                Category = "Communication",
                Points = 85
            },
            new()
            {
                Id = "network_services_wizard",
                Name = "Network Services Wizard",
                Description = "Conjured powerful DNS/DHCP service enchantments",
                Icon = "🔮",
                Category = "Network Services",
                Points = 90
            },
            new()
            {
                Id = "tunnel_architect",
                Name = "Tunnel Architect",
                Description = "Built magnificent VPN tunnel elixirs for secure passage",
                Icon = "🚇",
                Category = "VPN Security",
                Points = 95
            },
            new()
            {
                Id = "qos_priority_mage",
                Name = "QoS Priority Mage",
                Description = "Mastered the mystical arts of traffic prioritization potions",
                Icon = "⚡",
                Category = "Quality of Service",
                Points = 90
            },
            new()
            {
                Id = "voice_blend_virtuoso",
                Name = "Voice Blend Virtuoso",
                Description = "Perfected the complex alchemy of VoIP communication elixirs",
                Icon = "🎙️",
                Category = "Voice Protocols",
                Points = 95
            },
            new()
            {
                Id = "monitoring_oracle",
                Name = "Monitoring Oracle",
                Description = "Achieved omniscient network monitoring through SNMP divination",
                Icon = "👁️",
                Category = "Network Monitoring",
                Points = 85
            },
            new()
            {
                Id = "script_enchanter",
                Name = "Script Enchanter",
                Description = "Combined protocol knowledge with automation spells",
                Icon = "🪄",
                Category = "Protocol Automation",
                Points = 100
            },
            new()
            {
                Id = "multiprotocol_architect",
                Name = "Multiprotocol Architect",
                Description = "Designed complex hybrid protocol solutions with masterful precision",
                Icon = "🏗️",
                Category = "Advanced Architecture",
                Points = 105
            },
            new()
            {
                Id = "security_guardian",
                Name = "Security Guardian",
                Description = "Forged impenetrable security enchantments for all protocol mixes",
                Icon = "🛡️",
                Category = "Protocol Security",
                Points = 110
            },
            new()
            {
                Id = "troubleshooting_detective",
                Name = "Troubleshooting Detective",
                Description = "Solved the deepest mysteries of protocol mixing malfunctions",
                Icon = "🔍",
                Category = "Protocol Analysis",
                Points = 95
            },
            new()
            {
                Id = "future_protocol_visionary",
                Name = "Future Protocol Visionary",
                Description = "Mastered emerging protocols and next-generation communication elixirs",
                Icon = "🚀",
                Category = "Emerging Technologies",
                Points = 115
            },
            new()
            {
                Id = "iot_miniaturist",
                Name = "IoT Miniaturist",
                Description = "Perfected the delicate art of lightweight device protocol potions",
                Icon = "📱",
                Category = "IoT Protocols",
                Points = 100
            },
            new()
            {
                Id = "cloud_protocol_master",
                Name = "Cloud Protocol Master",
                Description = "Achieved mastery of cloud-native protocol architectures and API alchemy",
                Icon = "☁️",
                Category = "Cloud Protocols",
                Points = 105
            },
            new()
            {
                Id = "stack_architect_supreme",
                Name = "Stack Architect Supreme",
                Description = "Designed magnificent layered protocol stacks with architectural mastery",
                Icon = "🏛️",
                Category = "Protocol Stacks",
                Points = 120
            },
            new()
            {
                Id = "certification_alchemist",
                Name = "Certification Alchemist",
                Description = "Achieved professional-level protocol design and troubleshooting expertise",
                Icon = "🎓",
                Category = "Professional Mastery",
                Points = 125
            },
            new()
            {
                Id = "grand_protocol_alchemist",
                Name = "Grand Protocol Alchemist",
                Description = "Attained supreme mastery of all protocol mixing arts and alchemical wisdom",
                Icon = "👑",
                Category = "Ultimate Mastery",
                Points = 250
            },
            new()
            {
                Id = "alchemy_grandmaster",
                Name = "Alchemy Grandmaster",
                Description = "Transcended mortal protocol limitations to become the supreme master of network alchemy",
                Icon = "🌟",
                Category = "Legendary Achievement",
                Points = 300
            }
        };
    }
}