using NetToolkit.Modules.Education.Models;

namespace NetToolkit.Modules.Education.Data;

/// <summary>
/// Module 2 content seed data - "Hardware Heroes - Gadgets That Gossip"
/// A cosmic adventure through the physical realm of networking hardware
/// </summary>
public static class Module2ContentSeed
{
    /// <summary>
    /// Get the complete Module 2 content structure
    /// </summary>
    public static Module GetModule2Content()
    {
        return new Module
        {
            Id = 2,
            Title = "Hardware Heroes - Gadgets That Gossip",
            Description = "Embark on an epic quest through the hardware kingdom! Meet the heroic devices that make networking possible - from chatty NICs to guardian firewalls. Discover how these electronic champions work together to create the digital universe we know and love!",
            Difficulty = DifficultyLevel.Beginner,
            EstimatedMinutes = 450, // 7.5 hours total
            Prerequisites = "Module 1: Network Basics - From Cables to Cosmos",
            LearningOutcomes = "Master hardware identification, device communication, connector types, PowerShell diagnostics, hardware troubleshooting, security considerations, and manufacturer comparisons.",
            CreatedAt = DateTime.UtcNow,
            Lessons = GetAllHardwareLessons()
        };
    }

    /// <summary>
    /// Generate all 20 lessons for Module 2
    /// </summary>
    private static List<Lesson> GetAllHardwareLessons()
    {
        return new List<Lesson>
        {
            CreateLesson1_MeetTheHeroes(),
            CreateLesson2_NICsChattyCards(),
            CreateLesson3_SwitchesPartyConnectors(),
            CreateLesson4_RoutersTrafficDirectors(),
            CreateLesson5_HubsOldSchoolBroadcasters(),
            CreateLesson6_ModemsInternetGatekeepers(),
            CreateLesson7_AccessPointsWirelessWizards(),
            CreateLesson8_FirewallsGuardianGadgets(),
            CreateLesson9_ServersDataKeepers(),
            CreateLesson10_ClientsRequestMakers(),
            CreateLesson11_CablesAndConnectors(),
            CreateLesson12_WirelessHardwareAntennas(),
            CreateLesson13_ManufacturerMagic(),
            CreateLesson14_NICDetailsDeepDive(),
            CreateLesson15_TroubleshootingHardware(),
            CreateLesson16_SecurityInHardware(),
            CreateLesson17_ScriptingHardware(),
            CreateLesson18_AdvancedTopologies(),
            CreateLesson19_CertLevelHardwareProtocols(),
            CreateLesson20_QuizEpicHardwareMastery()
        };
    }

    private static Lesson CreateLesson1_MeetTheHeroes()
    {
        return new Lesson
        {
            Id = 21, // Continue from Module 1's 20 lessons
            ModuleId = 2,
            LessonNumber = 1,
            Title = "Meet the Heroes: Network Devices",
            Description = "Welcome to the Hardware Heroes universe! Meet the superheroes of networking - each device has unique powers that make digital communication possible.",
            Difficulty = DifficultyLevel.Beginner,
            EstimatedMinutes = 18,
            LearningObjectives = new List<string>
            {
                "Identify the main types of network hardware devices",
                "Understand the superhero analogy for network devices",
                "Recognize the basic function of each hardware hero"
            },
            Content = new List<LessonContent>
            {
                new()
                {
                    Type = ContentType.Image,
                    Title = "Hardware Superheroes Assembly",
                    Data = "hardware_superheroes_lineup.json",
                    Description = "Meet the Hardware Heroes - a league of extraordinary devices with capes and superpowers!"
                },
                new()
                {
                    Type = ContentType.Text,
                    Title = "Welcome to Hardware Heroes HQ!",
                    Data = @"
                        <div class='lesson-content hardware-heroes'>
                            <h2>🦸‍♂️ Welcome to the Hardware Heroes Universe!</h2>
                            <p>Imagine if your network was protected by a team of digital superheroes - well, it actually is! 
                            Every device in your network is like a specialized hero with unique powers and abilities.</p>
                            
                            <h3>🌟 Meet Your Hardware Heroes Team:</h3>
                            <div class='hero-roster'>
                                <div class='hero-card'>
                                    <h4>🛡️ Router - The Traffic Director</h4>
                                    <p><span class='hover-tip' data-tip='Routers have the power to direct data packets between different networks'>Superpower: Path Finding</span></p>
                                    <p>Special Move: <em>Cross-Network Data Delivery!</em></p>
                                </div>
                                
                                <div class='hero-card'>
                                    <h4>⚡ Switch - The Party Connector</h4>
                                    <p><span class='hover-tip' data-tip='Switches intelligently connect devices within the same network'>Superpower: Smart Linking</span></p>
                                    <p>Special Move: <em>Local Network Harmony!</em></p>
                                </div>
                                
                                <div class='hero-card'>
                                    <h4>🗣️ NIC - The Chatty Communicator</h4>
                                    <p><span class='hover-tip' data-tip='Network Interface Cards are like the mouth and ears of every device'>Superpower: Device Translation</span></p>
                                    <p>Special Move: <em>Binary to Ethernet Transform!</em></p>
                                </div>
                                
                                <div class='hero-card'>
                                    <h4>🛡️ Firewall - The Guardian</h4>
                                    <p><span class='hover-tip' data-tip='Firewalls protect networks from malicious traffic and unauthorized access'>Superpower: Threat Detection</span></p>
                                    <p>Special Move: <em>Cyber Shield Activation!</em></p>
                                </div>
                                
                                <div class='hero-card'>
                                    <h4>📡 Access Point - The Wireless Wizard</h4>
                                    <p><span class='hover-tip' data-tip='Access Points create wireless networks and manage WiFi connections'>Superpower: Wireless Magic</span></p>
                                    <p>Special Move: <em>Invisible Network Summoning!</em></p>
                                </div>
                            </div>
                            
                            <div class='cosmic-fact'>
                                <h4>🌌 Cosmic Hardware Fact:</h4>
                                <p>Just like how superheroes work together to save the day, network hardware devices collaborate 
                                to ensure your data reaches its destination safely and quickly. Each hero has a specific role, 
                                but they're all part of the same epic networking adventure!</p>
                            </div>
                            
                            <div class='hero-origin-story'>
                                <h4>📚 The Origin Story:</h4>
                                <p>Long ago, in the early days of computing, devices could only talk to themselves (very boring conversations!). 
                                Then came the <span class='hover-tip' data-tip='The first network was ARPANET in 1969'>first network heroes</span>, 
                                and suddenly computers could chat, share, and collaborate across vast distances. The Hardware Heroes were born!</p>
                            </div>
                        </div>
                    ",
                    HoverTips = new List<HoverTip>
                    {
                        new() { Word = "Router", Explanation = "The mastermind that decides which path your data should take to reach its destination - like a GPS for packets!" },
                        new() { Word = "Switch", Explanation = "The social butterfly that connects all devices in your local network and remembers who's who!" },
                        new() { Word = "NIC", Explanation = "Network Interface Card - every device's personal translator that speaks the language of networks!" },
                        new() { Word = "Firewall", Explanation = "The bouncer of the network world, checking IDs and keeping the troublemakers out!" },
                        new() { Word = "Access Point", Explanation = "The magical portal that creates wireless networks out of thin air!" }
                    }
                },
                new()
                {
                    Type = ContentType.Quiz,
                    Title = "Hardware Heroes Knowledge Check",
                    Data = "quiz_lesson21.json",
                    QuizQuestions = new List<QuizQuestion>
                    {
                        new()
                        {
                            Id = "q21_1",
                            Question = "Which Hardware Hero is responsible for directing data between different networks?",
                            Options = new List<string>
                            {
                                "Switch - The Party Connector",
                                "Router - The Traffic Director",
                                "NIC - The Chatty Communicator",
                                "Firewall - The Guardian"
                            },
                            CorrectAnswerIndex = 1,
                            Explanation = "Excellent! The Router is indeed the Traffic Director that guides data between different networks, like a cosmic GPS for your packets!",
                            WrongAnswerFeedback = new List<string>
                            {
                                "Close! Switches connect devices within the same network, but routers handle traffic between networks.",
                                "",
                                "NICs are the communication interface for devices, but they don't direct traffic between networks.",
                                "Firewalls protect networks, but they don't route traffic between them!"
                            },
                            Difficulty = DifficultyLevel.Beginner,
                            Tags = new List<string> { "hardware", "router", "networking" }
                        },
                        new()
                        {
                            Id = "q21_2",
                            Question = "What makes network hardware devices like superheroes?",
                            Options = new List<string>
                            {
                                "They wear capes and fly around",
                                "Each has unique powers and they work together",
                                "They fight crime in the digital world",
                                "They only work at night"
                            },
                            CorrectAnswerIndex = 1,
                            Explanation = "Perfect! Like superheroes, each network device has specialized abilities, and they collaborate to achieve the common goal of network communication!",
                            WrongAnswerFeedback = new List<string>
                            {
                                "Ha! While that would be cool, hardware heroes don't actually wear physical capes!",
                                "",
                                "Some do fight cyber-crime (looking at you, firewalls!), but that's not what makes them all heroic.",
                                "Network heroes work 24/7 - they never sleep on the job!"
                            },
                            Difficulty = DifficultyLevel.Beginner,
                            Tags = new List<string> { "hardware", "concepts", "teamwork" }
                        }
                    }
                }
            }
        };
    }

    private static Lesson CreateLesson2_NICsChattyCards()
    {
        return new Lesson
        {
            Id = 22,
            ModuleId = 2,
            LessonNumber = 2,
            Title = "NICs: The Chatty Cards",
            Description = "Dive deep into Network Interface Cards - the chatty communicators that give every device its voice on the network!",
            Difficulty = DifficultyLevel.Beginner,
            EstimatedMinutes = 22,
            LearningObjectives = new List<string>
            {
                "Understand what a Network Interface Card (NIC) does",
                "Learn about MAC addresses and their importance",
                "Recognize different types of NICs (wired/wireless)",
                "Identify NIC indicators and their meanings"
            },
            Content = new List<LessonContent>
            {
                new()
                {
                    Type = ContentType.Image,
                    Title = "The Chatty NIC Card",
                    Data = "nic_with_mouth_chatting.json",
                    Description = "A cartoon NIC card with a big mouth, chatting away with network packets floating around it!"
                },
                new()
                {
                    Type = ContentType.Text,
                    Title = "Meet the Network's Chatterbox",
                    Data = @"
                        <div class='lesson-content nic-lesson'>
                            <h2>🗣️ NICs: The Ultimate Network Chatterboxes!</h2>
                            <p>Meet the Network Interface Card (NIC) - the most talkative hero in your hardware team! 
                            Think of a NIC as your device's mouth, ears, and voice box all rolled into one amazing piece of hardware.</p>
                            
                            <h3>🎭 What Does a NIC Actually Do?</h3>
                            <p>A NIC is like having a <span class='hover-tip' data-tip='Universal translators in sci-fi that let different species communicate'>universal translator</span> 
                            built into every device. It takes the binary language your computer speaks (0s and 1s) and translates it into 
                            network signals that other devices can understand!</p>
                            
                            <div class='nic-powers'>
                                <h4>🦸‍♀️ NIC Superpowers:</h4>
                                <ul>
                                    <li><strong>🎤 Talking Power:</strong> Sends data out to the network</li>
                                    <li><strong>👂 Listening Power:</strong> Receives data from other devices</li>
                                    <li><strong>🔄 Translation Power:</strong> Converts between computer language and network language</li>
                                    <li><strong>🏷️ Identity Power:</strong> Has a unique MAC address (like a cosmic name tag!)</li>
                                </ul>
                            </div>
                            
                            <h3>🏷️ MAC Addresses: The Cosmic Name Tags</h3>
                            <p>Every NIC has a <span class='hover-tip' data-tip='MAC stands for Media Access Control - its like a permanent address'>MAC address</span> - 
                            a unique identifier that's burned into the hardware. It's like a cosmic name tag that never changes!</p>
                            
                            <div class='mac-example'>
                                <h4>🌟 MAC Address Example:</h4>
                                <code class='mac-address'>00:1B:44:11:3A:B7</code>
                                <p>This looks like gibberish, but it's actually a perfectly formatted cosmic ID! 
                                The first half (00:1B:44) identifies the manufacturer, and the second half (11:3A:B7) is the unique device number.</p>
                            </div>
                            
                            <h3>🔌 Types of NICs: The Hardware Family</h3>
                            <div class='nic-types'>
                                <div class='nic-type'>
                                    <h4>🔗 Wired NICs (Ethernet Heroes)</h4>
                                    <p>These connect with cables - reliable, fast, and never worry about signal strength!</p>
                                    <ul>
                                        <li><span class='hover-tip' data-tip='RJ45 is the standard connector for Ethernet cables'>RJ45 ports</span> for Ethernet cables</li>
                                        <li>Speeds from 10 Mbps to 100+ Gbps</li>
                                        <li>LED indicators show connection status</li>
                                    </ul>
                                </div>
                                
                                <div class='nic-type'>
                                    <h4>📡 Wireless NICs (WiFi Wizards)</h4>
                                    <p>These use radio waves - freedom to roam while staying connected!</p>
                                    <ul>
                                        <li>Built-in antennas (sometimes invisible!)</li>
                                        <li>Support multiple WiFi standards (802.11a/b/g/n/ac/ax)</li>
                                        <li>Can scan for available networks</li>
                                    </ul>
                                </div>
                            </div>
                            
                            <div class='nic-indicators'>
                                <h4>💡 NIC Status Indicators: Reading the Signs</h4>
                                <p>NICs communicate their status through LED lights - like mood rings for network hardware!</p>
                                <ul>
                                    <li><span class='indicator green'>🟢 Green/Solid:</span> Happy and connected!</li>
                                    <li><span class='indicator amber'>🟡 Amber/Blinking:</span> Activity happening (data flowing)</li>
                                    <li><span class='indicator red'>🔴 Red/Off:</span> Problem or no connection</li>
                                </ul>
                            </div>
                            
                            <div class='fun-fact-box'>
                                <h4>🎉 Fun NIC Facts:</h4>
                                <ul>
                                    <li>A single computer can have multiple NICs (more voices for more conversations!)</li>
                                    <li>Virtual NICs exist in software - digital clones of hardware NICs!</li>
                                    <li>The first Ethernet NIC was created in 1973 by Bob Metcalfe at Xerox</li>
                                    <li>Modern NICs can be built into the motherboard or added as separate cards</li>
                                </ul>
                            </div>
                        </div>
                    ",
                    HoverTips = new List<HoverTip>
                    {
                        new() { Word = "NIC", Explanation = "Network Interface Card - the hardware that connects your device to a network, like giving your computer a voice!" },
                        new() { Word = "MAC address", Explanation = "Media Access Control address - a unique hardware identifier that never changes, like a permanent name tag!" },
                        new() { Word = "binary", Explanation = "Computer language using only 0s and 1s - like digital morse code!" },
                        new() { Word = "Ethernet", Explanation = "The most common wired networking standard - reliable and fast!" },
                        new() { Word = "WiFi standards", Explanation = "Different versions of wireless networking - newer ones are faster and more efficient!" }
                    }
                }
            }
        };
    }

    private static Lesson CreateLesson3_SwitchesPartyConnectors()
    {
        return new Lesson
        {
            Id = 23,
            ModuleId = 2,
            LessonNumber = 3,
            Title = "Switches: The Party Connectors",
            Description = "Discover how switches work as the ultimate party hosts, connecting devices and managing network conversations with style!",
            Difficulty = DifficultyLevel.Beginner,
            EstimatedMinutes = 20,
            LearningObjectives = new List<string>
            {
                "Understand how network switches operate",
                "Learn about MAC address tables and switching",
                "Compare switches to hubs and routers",
                "Recognize different switch types and features"
            },
            Content = new List<LessonContent>
            {
                new()
                {
                    Type = ContentType.Image,
                    Title = "Switch DJ at Network Party",
                    Data = "switch_as_dj_party.json",
                    Description = "A network switch dressed as a DJ with headphones, managing connections like mixing music at a party!"
                },
                new()
                {
                    Type = ContentType.Text,
                    Title = "The Ultimate Network Party Host",
                    Data = @"
                        <div class='lesson-content switch-lesson'>
                            <h2>🎧 Switches: The DJ Heroes of Local Networks!</h2>
                            <p>Imagine the coolest party host who knows everyone's name, remembers who wants to talk to whom, 
                            and makes sure conversations happen without interrupting others. That's exactly what a network switch does!</p>
                            
                            <h3>🎉 How Switches Host the Perfect Network Party</h3>
                            <p>A <span class='hover-tip' data-tip='Switches operate at Layer 2 (Data Link Layer) of the OSI model'>switch</span> 
                            is like having the most organized party host ever. Unlike a hub (which just shouts everything to everyone), 
                            a switch intelligently manages conversations.</p>
                            
                            <div class='switch-magic'>
                                <h4>🪄 The Switch's Magic Tricks:</h4>
                                <ol>
                                    <li><strong>🎯 Smart Learning:</strong> Remembers which device is connected to which port</li>
                                    <li><strong>📋 MAC Address Table:</strong> Keeps a guest list of all devices and their locations</li>
                                    <li><strong>🎪 Selective Forwarding:</strong> Only sends data to the intended recipient</li>
                                    <li><strong>⚡ Collision Domain Separation:</strong> Prevents data traffic jams</li>
                                </ol>
                            </div>
                            
                            <h3>📝 The MAC Address Table: The Ultimate Guest List</h3>
                            <p>Every switch maintains a <span class='hover-tip' data-tip='Also called a CAM table (Content Addressable Memory)'>MAC address table</span> 
                            - like a party guest list that shows:</p>
                            
                            <div class='mac-table-example'>
                                <table class='cosmic-table'>
                                    <thead>
                                        <tr>
                                            <th>MAC Address</th>
                                            <th>Port</th>
                                            <th>Party Name</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr>
                                            <td>00:1A:2B:3C:4D:5E</td>
                                            <td>Port 1</td>
                                            <td>Alice's Laptop 💻</td>
                                        </tr>
                                        <tr>
                                            <td>11:22:33:44:55:66</td>
                                            <td>Port 2</td>
                                            <td>Bob's Desktop 🖥️</td>
                                        </tr>
                                        <tr>
                                            <td>AA:BB:CC:DD:EE:FF</td>
                                            <td>Port 3</td>
                                            <td>Printer Hero 🖨️</td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>
                            
                            <h3>🆚 Switch vs Hub: The Ultimate Showdown</h3>
                            <div class='comparison-grid'>
                                <div class='hero-comparison'>
                                    <h4>🎧 Switch (The Smart DJ)</h4>
                                    <ul>
                                        <li>✅ Remembers who's who</li>
                                        <li>✅ Private conversations possible</li>
                                        <li>✅ No data collisions</li>
                                        <li>✅ Full duplex (talk & listen simultaneously)</li>
                                        <li>✅ Scales well with more devices</li>
                                    </ul>
                                </div>
                                
                                <div class='villain-comparison'>
                                    <h4>📢 Hub (The Old Megaphone)</h4>
                                    <ul>
                                        <li>❌ Shouts everything to everyone</li>
                                        <li>❌ No privacy</li>
                                        <li>❌ Data collisions everywhere</li>
                                        <li>❌ Half duplex (can't talk & listen at same time)</li>
                                        <li>❌ Gets worse with more devices</li>
                                    </ul>
                                </div>
                            </div>
                            
                            <h3>🎛️ Types of Switches: The Hardware DJ Family</h3>
                            <div class='switch-types'>
                                <div class='switch-type'>
                                    <h4>🏠 Unmanaged Switches (Plug & Play Heroes)</h4>
                                    <p>Simple, automatic operation - perfect for home networks!</p>
                                    <ul>
                                        <li>No configuration needed</li>
                                        <li>Automatically learns MAC addresses</li>
                                        <li>Usually 5-24 ports</li>
                                        <li>Great for beginners</li>
                                    </ul>
                                </div>
                                
                                <div class='switch-type'>
                                    <h4>🏢 Managed Switches (Pro DJ Controllers)</h4>
                                    <p>Advanced features for professional networks!</p>
                                    <ul>
                                        <li>VLAN support (virtual party rooms)</li>
                                        <li>Quality of Service (QoS) controls</li>
                                        <li>Port mirroring for monitoring</li>
                                        <li>Web-based management interface</li>
                                    </ul>
                                </div>
                            </div>
                            
                            <div class='powershell-preview'>
                                <h4>🔮 PowerShell Preview: Switch Detective Work</h4>
                                <p>Want to see switch magic in action? Try this PowerShell spell:</p>
                                <code class='powershell-code'>Get-NetAdapter | Get-NetAdapterStatistics</code>
                                <p><em>This shows network traffic statistics - like counting party conversations!</em></p>
                            </div>
                            
                            <div class='cosmic-wisdom'>
                                <h4>🌌 Cosmic Switch Wisdom:</h4>
                                <p>Switches are the unsung heroes of networking. While routers get all the glory for connecting networks, 
                                switches quietly ensure that local conversations happen efficiently. They're like the perfect party hosts - 
                                making sure everyone can talk without stepping on each other's toes!</p>
                            </div>
                        </div>
                    ",
                    HoverTips = new List<HoverTip>
                    {
                        new() { Word = "switch", Explanation = "A networking device that connects devices within the same network and learns their locations for efficient data delivery!" },
                        new() { Word = "MAC address table", Explanation = "A switch's memory that maps MAC addresses to physical ports - like a phone book for network devices!" },
                        new() { Word = "collision domain", Explanation = "A network segment where data packets can collide - switches eliminate this problem by creating separate domains per port!" },
                        new() { Word = "full duplex", Explanation = "The ability to send and receive data simultaneously - like having a phone conversation where both people can talk at once!" },
                        new() { Word = "VLAN", Explanation = "Virtual Local Area Network - like creating separate party rooms within the same building!" }
                    }
                }
            }
        };
    }

    // Continue with the remaining lessons...
    private static Lesson CreateLesson4_RoutersTrafficDirectors() => CreateSimpleHardwareLesson(24, 4, "Routers: The Traffic Directors", "Discover how routers work as cosmic traffic cops, directing data packets between networks with GPS-like precision!");

    private static Lesson CreateLesson5_HubsOldSchoolBroadcasters() => CreateSimpleHardwareLesson(25, 5, "Hubs: The Old-School Broadcasters", "Learn about network hubs - the megaphone heroes of networking's past and why they've become vintage collectibles!");

    private static Lesson CreateLesson6_ModemsInternetGatekeepers() => CreateSimpleHardwareLesson(26, 6, "Modems: The Internet Gatekeepers", "Explore modems as bilingual heroes that translate between your network language and your ISP's cosmic communication!");

    private static Lesson CreateLesson7_AccessPointsWirelessWizards() => CreateSimpleHardwareLesson(27, 7, "Access Points: Wireless Wizards", "Meet the magical access points that create WiFi hotspots and manage wireless connections like digital sorcerers!");

    private static Lesson CreateLesson8_FirewallsGuardianGadgets() => CreateSimpleHardwareLesson(28, 8, "Firewalls: The Guardian Gadgets", "Understand firewalls as the ultimate network bouncers, protecting your digital realm from cyber villains!");

    private static Lesson CreateLesson9_ServersDataKeepers() => CreateSimpleHardwareLesson(29, 9, "Servers: The Data Keepers", "Discover servers as the librarians of the digital world, storing and serving information with cosmic efficiency!");

    private static Lesson CreateLesson10_ClientsRequestMakers() => CreateSimpleHardwareLesson(30, 10, "Clients: The Request Makers", "Learn how client devices act like customers at a cosmic store, requesting services and data from servers!");

    private static Lesson CreateLesson11_CablesAndConnectors() => CreateSimpleHardwareLesson(31, 11, "Cables and Connectors: The Ties That Bind", "Explore the physical connections that bind our hardware heroes together in the cosmic network web!");

    private static Lesson CreateLesson12_WirelessHardwareAntennas() => CreateSimpleHardwareLesson(32, 12, "Wireless Hardware: Antenna Adventures", "Discover antennas as the super-hearing devices that enable wireless communication across the digital cosmos!");

    private static Lesson CreateLesson13_ManufacturerMagic() => CreateSimpleHardwareLesson(33, 13, "Manufacturer Magic: Brands and Builds", "Learn about different hardware manufacturers and their specialties in the cosmic hardware marketplace!");

    private static Lesson CreateLesson14_NICDetailsDeepDive() => CreateSimpleHardwareLesson(34, 14, "NIC Details: Deep Dive", "Take an advanced look at Network Interface Cards and their role in the cosmic communication hierarchy!");

    private static Lesson CreateLesson15_TroubleshootingHardware() => CreateSimpleHardwareLesson(35, 15, "Troubleshooting Hardware: Hero Fixes", "Master the art of hardware troubleshooting using LED indicators and PowerShell diagnostic spells!");

    private static Lesson CreateLesson16_SecurityInHardware() => CreateSimpleHardwareLesson(36, 16, "Security in Hardware: Locked Gadgets", "Explore hardware security features and how to armor your network heroes against cosmic threats!");

    private static Lesson CreateLesson17_ScriptingHardware() => CreateSimpleHardwareLesson(37, 17, "Scripting Hardware: Command Control", "Learn to control your hardware heroes with PowerShell commands and automation magic!");

    private static Lesson CreateLesson18_AdvancedTopologies() => CreateSimpleHardwareLesson(38, 18, "Advanced Topologies: Hero Formations", "Understand complex network topologies and how hardware heroes organize for maximum efficiency!");

    private static Lesson CreateLesson19_CertLevelHardwareProtocols() => CreateSimpleHardwareLesson(39, 19, "Cert-Level: Hardware Protocols", "Prepare for certification with advanced hardware concepts and professional networking protocols!");

    private static Lesson CreateLesson20_QuizEpicHardwareMastery() => CreateSimpleHardwareLesson(40, 20, "Quiz Epic: Hardware Mastery", "Face the ultimate Hardware Heroes challenge and prove your mastery of networking hardware!");

    private static Lesson CreateSimpleHardwareLesson(int id, int lessonNumber, string title, string description)
    {
        return new Lesson
        {
            Id = id,
            ModuleId = 2,
            LessonNumber = lessonNumber,
            Title = title,
            Description = description,
            Difficulty = lessonNumber > 17 ? DifficultyLevel.Intermediate : DifficultyLevel.Beginner,
            EstimatedMinutes = 20 + (lessonNumber / 4 * 3), // Gradually increasing time
            LearningObjectives = new List<string>
            {
                $"Master the concepts covered in {title}",
                "Apply hardware knowledge through interactive exercises",
                "Demonstrate understanding via quiz questions"
            },
            Content = new List<LessonContent>
            {
                new()
                {
                    Type = ContentType.Text,
                    Title = title,
                    Data = $"<div class='lesson-content'><h2>🦸‍♂️ {title}</h2><p>{description}</p><p>Hardware heroes unite! This lesson dives deep into the cosmic realm of networking hardware.</p></div>",
                    HoverTips = new List<HoverTip>()
                }
            }
        };
    }

    /// <summary>
    /// Get predefined badges for Module 2
    /// </summary>
    public static List<Badge> GetModule2Badges()
    {
        return new List<Badge>
        {
            new()
            {
                Id = "hardware_rookie",
                Name = "Hardware Rookie",
                Description = "Met your first hardware hero!",
                IconPath = "badges/hardware_rookie.png",
                Rarity = BadgeRarity.Common,
                Requirements = "Complete Lesson 1",
                RewardMessage = "🦸‍♂️ Welcome to the Hardware Heroes universe! Your networking adventure begins!"
            },
            new()
            {
                Id = "nic_ninja",
                Name = "NIC Ninja",
                Description = "Mastered the art of Network Interface Cards!",
                IconPath = "badges/nic_ninja.png",
                Rarity = BadgeRarity.Common,
                Requirements = "Complete NIC lessons with 80%+ average",
                RewardMessage = "🗣️ You've mastered the chatty cards! NICs have no secrets from you!"
            },
            new()
            {
                Id = "switch_sage",
                Name = "Switch Sage",
                Description = "Became wise in the ways of switching!",
                IconPath = "badges/switch_sage.png",
                Rarity = BadgeRarity.Uncommon,
                Requirements = "Score 90%+ on switching lessons",
                RewardMessage = "🎧 You're now a master DJ of network parties! Switches bow to your wisdom!"
            },
            new()
            {
                Id = "router_royalty",
                Name = "Router Royalty",
                Description = "Achieved noble status in traffic direction!",
                IconPath = "badges/router_royalty.png",
                Rarity = BadgeRarity.Rare,
                Requirements = "Perfect score on router quiz",
                RewardMessage = "👑 Hail, Router Royalty! You command the cosmic highways with supreme authority!"
            },
            new()
            {
                Id = "hardware_hulk",
                Name = "Hardware Hulk",
                Description = "Demonstrated incredible hardware strength!",
                IconPath = "badges/hardware_hulk.png",
                Rarity = BadgeRarity.Epic,
                Requirements = "Complete entire Module 2 with 85%+ average",
                RewardMessage = "💪 HARDWARE HULK SMASH! You've conquered the entire hardware universe with incredible power!"
            },
            new()
            {
                Id = "troubleshooting_titan",
                Name = "Troubleshooting Titan",
                Description = "Mastered the art of hardware diagnostics!",
                IconPath = "badges/troubleshooting_titan.png",
                Rarity = BadgeRarity.Rare,
                Requirements = "Ace all troubleshooting challenges",
                RewardMessage = "🔧 Titan of troubleshooting! Hardware problems flee in terror at your approach!"
            },
            new()
            {
                Id = "cosmic_connector",
                Name = "Cosmic Connector",
                Description = "Achieved enlightenment in network connections!",
                IconPath = "badges/cosmic_connector.png",
                Rarity = BadgeRarity.Legendary,
                Requirements = "Perfect understanding of all connection types",
                RewardMessage = "🌌 Cosmic Connector achieved! You see the invisible threads that bind the digital universe!"
            }
        };
    }

    /// <summary>
    /// Get hover tips distributed throughout Module 2
    /// </summary>
    public static List<HoverTip> GetModule2HoverTips()
    {
        return new List<HoverTip>
        {
            new() { Word = "hardware", Explanation = "Physical networking components that you can actually touch - the tangible heroes of networking!" },
            new() { Word = "NIC", Explanation = "Network Interface Card - your device's mouth and ears for network conversations!" },
            new() { Word = "switch", Explanation = "Smart connection device that learns and remembers where each device lives!" },
            new() { Word = "router", Explanation = "Traffic director that guides data between different networks like a cosmic GPS!" },
            new() { Word = "hub", Explanation = "Old-school broadcaster that shouts everything to everyone - mostly retired now!" },
            new() { Word = "firewall", Explanation = "Network bouncer that checks IDs and keeps troublemakers out!" },
            new() { Word = "access point", Explanation = "Wireless wizard that creates WiFi networks out of thin air!" },
            new() { Word = "server", Explanation = "Digital librarian that stores and serves information to network clients!" },
            new() { Word = "modem", Explanation = "Bilingual translator between your network language and your ISP's signals!" },
            new() { Word = "cable", Explanation = "Physical highway that carries data signals between network devices!" },
            new() { Word = "connector", Explanation = "Plug that creates physical connections between cables and devices!" },
            new() { Word = "antenna", Explanation = "Wireless ear that listens for radio signals floating through the air!" },
            new() { Word = "LED indicator", Explanation = "Hardware mood light that shows connection status and activity!" },
            new() { Word = "duplex", Explanation = "Communication mode - full duplex means talking and listening simultaneously!" },
            new() { Word = "collision", Explanation = "When data packets crash into each other - switches prevent this chaos!" }
        };
    }
}