using NetToolkit.Modules.Education.Models;

namespace NetToolkit.Modules.Education.Data;

/// <summary>
/// Module 7: Wireless Wonders - "Invisible Highways"
/// Where invisible data highways meet cosmic wireless wisdom!
/// A comprehensive journey through wireless networking from magical waves to certification mastery.
/// </summary>
public static class Module7ContentSeed
{
    /// <summary>
    /// Get the complete Module 7: Wireless Wonders content
    /// </summary>
    public static Module GetModule7Content()
    {
        return new Module
        {
            Id = 7,
            Title = "Wireless Wonders - Invisible Highways",
            Description = "Embark on an epic journey through the invisible realm of wireless networking! Master the art of airwave " +
                         "communication with humor, wonder, and wisdom as you progress from magical wave concepts to advanced wireless " +
                         "engineering mastery. Learn to navigate invisible highways, command wireless signals, and become the ultimate " +
                         "wireless wonder wizard!",
            Difficulty = DifficultyLevel.Intermediate,
            EstimatedMinutes = 720, // 12 hours total - comprehensive wireless education
            Lessons = GetAllWirelessLessons()
        };
    }

    /// <summary>
    /// Generate all 20 wireless-focused lessons with progressive complexity
    /// </summary>
    private static List<Lesson> GetAllWirelessLessons()
    {
        var lessons = new List<Lesson>();

        // Lesson 1: What's Wireless? Magic Waves (Beginner)
        lessons.Add(new Lesson
        {
            Id = 121, // Module 7, Lesson 1
            ModuleId = 7,
            Title = "What's Wireless? Magic Waves",
            Description = "Discover the magical realm of invisible data highways! Learn what wireless networking means using enchanting wave metaphors.",
            EstimatedMinutes = 25,
            Order = 1,
            LearningObjectives = new List<string>
            {
                "Understand basic wireless concepts using wave analogies",
                "Identify what data travels through invisible highways",
                "Recognize different types of wireless communications",
                "Learn the foundation of airwave thinking"
            },
            Content = new List<LessonContent>
            {
                new()
                {
                    Type = ContentType.Image,
                    Title = "Invisible Data Highways",
                    Data = "invisible_roads_with_magical_waves",
                    Description = "Imagine data floating through the air on invisible highways - that's wireless networking in all its magical glory!"
                },
                new()
                {
                    Type = ContentType.Text,
                    Title = "Wireless: Magic Waves in the Air",
                    Data = @"
                        <h3>📡 What is Wireless Networking?</h3>
                        <p>Imagine your favorite superhero can fly through the air without touching the ground. That's exactly what wireless networking does with data! Instead of sending information through cables (like cars on a road), wireless sends data through invisible highways in the air using magical waves.</p>
                        
                        <h4>✨ The Magic Behind Wireless</h4>
                        <ul>
                            <li><strong>Radio Waves:</strong> Invisible energy that carries data through the air</li>
                            <li><strong>No Cables:</strong> Data flies free like birds in the sky</li>
                            <li><strong>Instant Connection:</strong> Devices talk to each other through the air</li>
                            <li><strong>Everywhere Access:</strong> Connect from anywhere within the magic zone</li>
                        </ul>
                        
                        <h4>🌟 What Travels These Invisible Roads?</h4>
                        <ul>
                            <li><strong>Messages:</strong> Your texts and emails flying through the air</li>
                            <li><strong>Pictures:</strong> Photos soaring from your phone to the internet</li>
                            <li><strong>Videos:</strong> Movies streaming through invisible highways</li>
                            <li><strong>Games:</strong> Online adventures floating through airwaves</li>
                        </ul>
                        
                        <h4>🎪 Types of Wireless Magic Shows</h4>
                        <ul>
                            <li><strong>WiFi:</strong> Home and office wireless highways</li>
                            <li><strong>Bluetooth:</strong> Short-distance magical whispers</li>
                            <li><strong>Cellular:</strong> Long-distance superhighways (4G, 5G)</li>
                            <li><strong>Satellite:</strong> Space highways for global connections</li>
                        </ul>
                        
                        <p>Think of it like having magical roads floating in the air that only your devices can see and travel on!</p>
                    ",
                    HoverTips = new List<HoverTip>
                    {
                        new() { Word = "Radio Waves", Explanation = "Invisible energy waves that carry information through the air - like magical messengers flying at the speed of light!" },
                        new() { Word = "WiFi", Explanation = "Wireless Fidelity - your home's invisible internet highway that lets devices connect without cables!" },
                        new() { Word = "Bluetooth", Explanation = "Named after a Viking king! Creates short-range wireless connections between nearby devices - like digital telepathy!" },
                        new() { Word = "Cellular", Explanation = "Mobile phone networks that create invisible highways spanning entire countries!" },
                        new() { Word = "Frequency", Explanation = "How fast the waves wiggle - different frequencies are like different lanes on the invisible highway!" }
                    }
                },
                new()
                {
                    Type = ContentType.Quiz,
                    Title = "Magical Wave Quiz",
                    Data = @"
                        {
                            'question': 'What makes wireless networking different from wired networking?',
                            'options': [
                                'It uses cables and wires',
                                'Data travels through invisible waves in the air',
                                'It only works during the day',
                                'It requires special magical powers'
                            ],
                            'correctAnswer': 1,
                            'explanation': 'Excellent! Wireless networking sends data through invisible radio waves in the air, just like magical highways that only your devices can see and travel on. No cables needed - pure airwave magic!'
                        }
                    "
                }
            ],
            Prerequisites = new List<int>(), // First lesson, no prerequisites
            Tags = new List<string> { "basics", "introduction", "waves", "wireless-fundamentals", "magic" }
        });

        // Lesson 2: WiFi Basics - Home Highways (Beginner)
        lessons.Add(new Lesson
        {
            Id = 122,
            ModuleId = 7,
            Title = "WiFi Basics: Home Highways",
            Description = "Explore your home's invisible internet highways! Learn how WiFi creates magical zones where all your devices can connect.",
            EstimatedMinutes = 30,
            Order = 2,
            LearningObjectives = new List<string>
            {
                "Understand how WiFi works using home analogies",
                "Learn about wireless access points and routers",
                "Identify WiFi coverage areas and signal strength",
                "Recognize how devices connect to WiFi networks"
            },
            Content = new List<LessonContent>
            {
                new()
                {
                    Type = ContentType.Image,
                    Title = "Home with Magical Wave Zones",
                    Data = "house_surrounded_by_wifi_waves",
                    Description = "Your home router creates invisible WiFi zones where all your devices can magically connect to the internet!"
                },
                new()
                {
                    Type = ContentType.Text,
                    Title = "WiFi: Your Home's Invisible Internet Highways",
                    Data = @"
                        <h3>🏠 What is WiFi?</h3>
                        <p>WiFi is like having invisible internet highways floating around your home! Your router (the magical highway generator) creates an invisible bubble of internet connectivity that all your devices can tap into. It's like having a personal internet cloud following you around your house!</p>
                        
                        <h4>📡 The WiFi Highway System</h4>
                        <ul>
                            <li><strong>Router:</strong> The magical gateway that creates invisible internet highways</li>
                            <li><strong>Coverage Area:</strong> The invisible bubble where the magic works</li>
                            <li><strong>Signal Strength:</strong> How strong the magic is in different spots</li>
                            <li><strong>Connected Devices:</strong> All your gadgets riding the invisible highways</li>
                        </ul>
                        
                        <h4>🎯 How WiFi Magic Works</h4>
                        <ol>
                            <li><strong>Router broadcasts signals:</strong> Like a lighthouse sending out invisible beams</li>
                            <li><strong>Devices detect signals:</strong> Your phone, laptop, tablet sense the magic</li>
                            <li><strong>Authentication happens:</strong> Devices prove they belong (password check!)</li>
                            <li><strong>Data flows freely:</strong> Information travels on invisible highways</li>
                        </ol>
                        
                        <h4>📶 Signal Strength Zones</h4>
                        <ul>
                            <li><strong>Strong Signal (Close to Router):</strong> Like standing next to a campfire - warm and bright</li>
                            <li><strong>Medium Signal (Middle Distance):</strong> Like feeling campfire warmth from across the room</li>
                            <li><strong>Weak Signal (Far Away):</strong> Like barely feeling warmth from another room</li>
                            <li><strong>Dead Zones:</strong> Places where the magic can't reach</li>
                        </ul>
                        
                        <h4>🏰 WiFi Network Types</h4>
                        <ul>
                            <li><strong>Home Networks:</strong> Your personal family highway system</li>
                            <li><strong>Public WiFi:</strong> Shared highways at cafes and libraries</li>
                            <li><strong>Enterprise Networks:</strong> Corporate highway systems with fancy security</li>
                            <li><strong>Mesh Networks:</strong> Multiple routers working together like a highway network</li>
                        </ul>
                    ",
                    HoverTips = new List<HoverTip>
                    {
                        new() { Word = "Router", Explanation = "The magical device that creates your home's WiFi bubble - like a wireless internet fountain!" },
                        new() { Word = "Signal Strength", Explanation = "How strong your WiFi connection is - measured in bars like a cell phone signal!" },
                        new() { Word = "Coverage Area", Explanation = "The invisible zone where your WiFi magic works - usually shaped like a sphere around your router!" },
                        new() { Word = "Authentication", Explanation = "Proving you belong on the network - like showing your ID to enter an exclusive club!" },
                        new() { Word = "Dead Zone", Explanation = "Areas where WiFi signals can't reach - like WiFi deserts in your home!" }
                    }
                },
                new()
                {
                    Type = ContentType.Quiz,
                    Title = "Home Highway Quiz",
                    Data = @"
                        {
                            'question': 'What creates the invisible WiFi highways in your home?',
                            'options': [
                                'Your computer',
                                'The wireless router',
                                'Your phone',
                                'The internet provider'
                            ],
                            'correctAnswer': 1,
                            'explanation': 'Perfect! The wireless router is like a magical lighthouse that broadcasts invisible internet highways throughout your home, creating a WiFi bubble where all your devices can connect!'
                        }
                    "
                }
            ],
            Prerequisites = new List<int> { 121 },
            Tags = new List<string> { "wifi", "router", "home-network", "signal-strength", "basics" }
        });

        // Lesson 3: Standards - 802.11 Family (Beginner to Intermediate)
        lessons.Add(new Lesson
        {
            Id = 123,
            ModuleId = 7,
            Title = "Standards: 802.11 Family",
            Description = "Meet the WiFi family tree! Learn how wireless standards evolved from pony-speed to jet-speed connections.",
            EstimatedMinutes = 35,
            Order = 3,
            LearningObjectives = new List<string>
            {
                "Understand WiFi standards using family tree analogies",
                "Learn the evolution from 802.11a to 802.11ax (WiFi 6)",
                "Identify speed and capability differences between standards",
                "Recognize backward compatibility concepts"
            },
            Content = new List<LessonContent>
            {
                new()
                {
                    Type = ContentType.Image,
                    Title = "WiFi Family Tree",
                    Data = "wifi_standards_family_tree",
                    Description = "The WiFi standards family tree - from the ancient 802.11 ancestors to the modern WiFi 6 speedsters!"
                },
                new()
                {
                    Type = ContentType.Text,
                    Title = "The 802.11 Family: From Pony to Jet Speed",
                    Data = @"
                        <h3>👨‍👩‍👧‍👦 The WiFi Family Tree</h3>
                        <p>Think of WiFi standards like a family of transportation methods that keep getting faster and better! From the original 802.11 (like riding a pony) to modern WiFi 6 (like flying a jet), each generation brings amazing improvements while staying friends with their older relatives.</p>
                        
                        <h4>🐎 The Original Ancestors</h4>
                        <ul>
                            <li><strong>802.11 (1997):</strong> The great-grandfather - 2 Mbps (like a slow pony)</li>
                            <li><strong>802.11a (1999):</strong> 54 Mbps on 5GHz (like a fast horse)</li>
                            <li><strong>802.11b (1999):</strong> 11 Mbps on 2.4GHz (like a reliable bicycle)</li>
                            <li><strong>802.11g (2003):</strong> 54 Mbps on 2.4GHz (like a motorcycle)</li>
                        </ul>
                        
                        <h4>🚗 The Modern Generations</h4>
                        <ul>
                            <li><strong>802.11n (WiFi 4, 2009):</strong> Up to 600 Mbps (like a sports car)</li>
                            <li><strong>802.11ac (WiFi 5, 2013):</strong> Up to 3.5 Gbps (like a race car)</li>
                            <li><strong>802.11ax (WiFi 6, 2019):</strong> Up to 9.6 Gbps (like a rocket ship)</li>
                            <li><strong>802.11be (WiFi 7, Coming):</strong> Up to 46 Gbps (like a warp drive!)</li>
                        </ul>
                        
                        <h4>🔧 Key Family Traits</h4>
                        <ul>
                            <li><strong>Speed Evolution:</strong> Each generation gets dramatically faster</li>
                            <li><strong>Range Improvement:</strong> Better signal reach and penetration</li>
                            <li><strong>Multiple Antennas:</strong> MIMO technology (Multiple In, Multiple Out)</li>
                            <li><strong>Better Efficiency:</strong> Handle more devices with less interference</li>
                        </ul>
                        
                        <h4>🤝 Family Compatibility</h4>
                        <p>The best part? This wireless family gets along great! Newer routers can talk to older devices, and older routers can connect newer devices (just at slower speeds). It's like a multilingual family where everyone understands each other!</p>
                        
                        <h4>🚀 Modern WiFi 6 Superpowers</h4>
                        <ul>
                            <li><strong>OFDMA:</strong> Like having express lanes on highways</li>
                            <li><strong>1024-QAM:</strong> Packing more data into each signal</li>
                            <li><strong>Target Wake Time:</strong> Helps devices save battery power</li>
                            <li><strong>BSS Coloring:</strong> Reduces interference between networks</li>
                        </ul>
                    ",
                    HoverTips = new List<HoverTip>
                    {
                        new() { Word = "802.11", Explanation = "The IEEE standard family for WiFi - like the official rule book for how wireless networks should work!" },
                        new() { Word = "MIMO", Explanation = "Multiple Input Multiple Output - using multiple antennas to send more data at once, like having multiple lanes on a highway!" },
                        new() { Word = "Mbps", Explanation = "Megabits per second - how fast data can travel. Higher numbers mean faster internet highways!" },
                        new() { Word = "Backward Compatible", Explanation = "New technology that still works with older devices - like a modern car that can drive on old roads!" },
                        new() { Word = "OFDMA", Explanation = "A smart way to share wireless channels efficiently - like carpooling for data packets!" }
                    }
                },
                new()
                {
                    Type = ContentType.Quiz,
                    Title = "WiFi Family Knowledge Quiz",
                    Data = @"
                        {
                            'question': 'Which WiFi standard is known as WiFi 6 and offers the fastest speeds?',
                            'options': [
                                '802.11n',
                                '802.11ac',
                                '802.11ax',
                                '802.11g'
                            ],
                            'correctAnswer': 2,
                            'explanation': 'Excellent! 802.11ax is WiFi 6, the newest family member that offers rocket-ship speeds up to 9.6 Gbps with amazing efficiency improvements. It\\'s like upgrading from a sports car to a spacecraft!'
                        }
                    "
                }
            ],
            Prerequisites = new List<int> { 122 },
            Tags = new List<string> { "standards", "802.11", "wifi6", "evolution", "family-tree" }
        });

        // Continue with remaining lessons...
        lessons.AddRange(GetWirelessLessons4Through20());
        
        return lessons;
    }

    /// <summary>
    /// Get lessons 4-20 for Module 7 with detailed wireless content
    /// </summary>
    private static List<Lesson> GetWirelessLessons4Through20()
    {
        var lessons = new List<Lesson>();

        // Lesson 4: Frequencies - Wave Bands
        lessons.Add(new Lesson
        {
            Id = 124,
            ModuleId = 7,
            Title = "Frequencies: Wave Bands",
            Description = "Explore the invisible radio spectrum! Learn about different frequency bands and how they affect your wireless experience.",
            EstimatedMinutes = 35,
            Order = 4,
            LearningObjectives = new List<string>
            {
                "Understand frequency bands using highway analogies",
                "Learn about 2.4GHz, 5GHz, and 6GHz characteristics",
                "Identify trade-offs between range and speed",
                "Recognize interference and congestion issues"
            },
            Content = new List<LessonContent>
            {
                new()
                {
                    Type = ContentType.Image,
                    Title = "Radio Frequency Dial",
                    Data = "radio_dial_with_frequency_bands",
                    Description = "The invisible radio spectrum dial showing different frequency highways where wireless signals travel!"
                },
                new()
                {
                    Type = ContentType.Text,
                    Title = "Frequency Bands: Choosing Your Invisible Highway Lane",
                    Data = @"
                        <h3>📻 What are Frequency Bands?</h3>
                        <p>Think of frequency bands like different types of highways in the sky! Just as you might choose a busy city street (slower but reaches everywhere) versus a fast expressway (faster but more limited), wireless devices can choose different frequency 'lanes' based on what they need.</p>
                        
                        <h4>🛣️ The Main Wireless Highway Types</h4>
                        
                        <h5>2.4GHz - The Crowded City Street</h5>
                        <ul>
                            <li><strong>Range:</strong> Goes far and penetrates walls well (like sound travels through buildings)</li>
                            <li><strong>Speed:</strong> Slower speeds but reliable coverage</li>
                            <li><strong>Congestion:</strong> Very crowded! Shares space with microwaves, baby monitors</li>
                            <li><strong>Channels:</strong> Only 3 non-overlapping channels (1, 6, 11)</li>
                            <li><strong>Best for:</strong> IoT devices, long-range connections, basic internet</li>
                        </ul>
                        
                        <h5>5GHz - The Fast Expressway</h5>
                        <ul>
                            <li><strong>Range:</strong> Shorter range, doesn't penetrate walls as well</li>
                            <li><strong>Speed:</strong> Much faster speeds for data-hungry applications</li>
                            <li><strong>Congestion:</strong> Less crowded, cleaner signals</li>
                            <li><strong>Channels:</strong> Many more channels available (up to 25!)</li>
                            <li><strong>Best for:</strong> Streaming, gaming, high-speed internet</li>
                        </ul>
                        
                        <h5>6GHz - The New Super Highway (WiFi 6E/7)</h5>
                        <ul>
                            <li><strong>Range:</strong> Even shorter range than 5GHz</li>
                            <li><strong>Speed:</strong> Ultra-fast with massive bandwidth</li>
                            <li><strong>Congestion:</strong> Brand new, virtually empty lanes!</li>
                            <li><strong>Channels:</strong> Enormous number of channels available</li>
                            <li><strong>Best for:</strong> AR/VR, ultra-high-speed applications</li>
                        </ul>
                        
                        <h4>🚦 The Great Trade-Off</h4>
                        <p>It's like choosing between different transportation options:</p>
                        <ul>
                            <li><strong>Walking (2.4GHz):</strong> Slow but can go anywhere, through any terrain</li>
                            <li><strong>Highway (5GHz):</strong> Fast but only on smooth roads</li>
                            <li><strong>Airplane (6GHz):</strong> Super fast but needs perfect conditions</li>
                        </ul>
                        
                        <h4>🌊 Wave Behavior Fun Facts</h4>
                        <ul>
                            <li>Lower frequencies (longer waves) travel farther but carry less data</li>
                            <li>Higher frequencies (shorter waves) carry more data but don\\'t travel as far</li>
                            <li>Walls and obstacles affect higher frequencies more</li>
                            <li>Weather can impact higher frequency signals</li>
                        </ul>
                    ",
                    HoverTips = new List<HoverTip>
                    {
                        new() { Word = "2.4GHz", Explanation = "The older, more crowded frequency band that goes far but slower - like taking city streets everywhere!" },
                        new() { Word = "5GHz", Explanation = "The faster frequency band that doesn't go as far - like taking the highway when available!" },
                        new() { Word = "6GHz", Explanation = "The newest, fastest frequency band with tons of room - like having a private jet lane!" },
                        new() { Word = "Channel", Explanation = "Specific frequency lanes within a band - like choosing which radio station to tune to!" },
                        new() { Word = "Interference", Explanation = "When different signals interfere with each other - like multiple people talking at once!" }
                    }
                }
            ],
            Prerequisites = new List<int> { 123 },
            Tags = new List<string> { "frequency", "2.4GHz", "5GHz", "6GHz", "channels", "spectrum" }
        });

        // Add simplified implementations for remaining lessons (5-20)
        lessons.AddRange(CreateRemainingWirelessLessons());
        
        return lessons;
    }

    /// <summary>
    /// Create the remaining wireless lessons (5-20) with proper structure
    /// </summary>
    private static List<Lesson> CreateRemainingWirelessLessons()
    {
        var lessons = new List<Lesson>();

        // Lesson 5: Access Points - Wave Stations
        lessons.Add(new Lesson
        {
            Id = 125,
            ModuleId = 7,
            Title = "Access Points: Wave Stations",
            Description = "Discover wireless lighthouses! Learn how access points broadcast signals to guide data ships safely to shore.",
            EstimatedMinutes = 30,
            Order = 5,
            LearningObjectives = new List<string> { "Learn access point concepts", "Understand signal broadcasting" },
            Content = new List<LessonContent>
            {
                new() { Type = ContentType.Image, Title = "Wireless Lighthouse", Data = "lighthouse_broadcasting_wifi_waves", Description = "Access points as digital lighthouses guiding data ships" },
                new() { Type = ContentType.Text, Title = "Access Points", Data = "<h3>🗼 Wave Stations</h3><p>Access points broadcast signals like beacons!</p>" }
            },
            Prerequisites = new List<int> { 124 },
            Tags = new List<string> { "access-points", "lighthouse", "broadcasting", "infrastructure" }
        });

        // Lesson 6: SSID - Network Names  
        lessons.Add(new Lesson
        {
            Id = 126,
            ModuleId = 7,
            Title = "SSID: Network Names",
            Description = "Learn about network name tags! Discover how SSIDs help devices find and identify wireless networks.",
            EstimatedMinutes = 25,
            Order = 6,
            LearningObjectives = new List<string> { "Understand SSID concepts", "Learn network identification" },
            Content = new List<LessonContent>
            {
                new() { Type = ContentType.Image, Title = "Network Name Tags", Data = "network_name_tags_floating", Description = "SSIDs as digital name tags for wireless networks" },
                new() { Type = ContentType.Text, Title = "Network Names", Data = "<h3>🏷️ SSID Name Tags</h3><p>Every network needs a name tag!</p>" }
            },
            Prerequisites = new List<int> { 125 },
            Tags = new List<string> { "SSID", "network-names", "identification", "broadcasting" }
        });

        // Lesson 7: Encryption - Secure Waves
        lessons.Add(new Lesson
        {
            Id = 127,
            ModuleId = 7,
            Title = "Encryption: Secure Waves",
            Description = "Protect your airwaves! Learn how wireless encryption keeps your data safe while traveling invisible highways.",
            EstimatedMinutes = 40,
            Order = 7,
            LearningObjectives = new List<string> { "Learn wireless encryption", "Understand WPA3 security" },
            Content = new List<LessonContent>
            {
                new() { Type = ContentType.Image, Title = "Encrypted Waves", Data = "locked_wireless_waves", Description = "Wireless signals protected with encryption locks" },
                new() { Type = ContentType.Text, Title = "Wireless Security", Data = "<h3>🔐 Secure Waves</h3><p>Encrypt air traffic with secret codes!</p>" }
            },
            Prerequisites = new List<int> { 126 },
            Tags = new List<string> { "encryption", "WPA3", "security", "wireless-security" }
        });

        // Lesson 8: Mesh Networks - Wave Webs
        lessons.Add(new Lesson
        {
            Id = 128,
            ModuleId = 7,
            Title = "Mesh Networks: Wave Webs",
            Description = "Explore interconnected wireless webs! Learn how mesh networks create seamless coverage with teamwork.",
            EstimatedMinutes = 35,
            Order = 8,
            LearningObjectives = new List<string> { "Understand mesh networking", "Learn coverage extension" },
            Content = new List<LessonContent>
            {
                new() { Type = ContentType.Image, Title = "Wireless Spider Web", Data = "mesh_network_spider_web", Description = "Mesh networks as interconnected web of wireless nodes" },
                new() { Type = ContentType.Text, Title = "Mesh Networks", Data = "<h3>🕸️ Wave Webs</h3><p>Access points team up for coverage!</p>" }
            },
            Prerequisites = new List<int> { 127 },
            Tags = new List<string> { "mesh", "coverage", "teamwork", "seamless" }
        });

        // Lesson 9: Bluetooth - Short-Range Magic
        lessons.Add(new Lesson
        {
            Id = 129,
            ModuleId = 7,
            Title = "Bluetooth: Short-Range Magic",
            Description = "Master close-range wireless whispers! Learn how Bluetooth creates intimate device connections.",
            EstimatedMinutes = 30,
            Order = 9,
            LearningObjectives = new List<string> { "Learn Bluetooth concepts", "Understand pairing" },
            Content = new List<LessonContent>
            {
                new() { Type = ContentType.Image, Title = "Device Whispers", Data = "bluetooth_devices_whispering", Description = "Bluetooth devices communicating through close whispers" },
                new() { Type = ContentType.Text, Title = "Bluetooth Magic", Data = "<h3>🔮 Short-Range Magic</h3><p>Device chats nearby like whispers!</p>" }
            },
            Prerequisites = new List<int> { 128 },
            Tags = new List<string> { "bluetooth", "short-range", "pairing", "personal-area" }
        });

        // Lesson 10: NFC - Touch Wonders
        lessons.Add(new Lesson
        {
            Id = 130,
            ModuleId = 7,
            Title = "NFC: Touch Wonders",
            Description = "Experience magical touch connections! Learn how Near Field Communication creates instant device bonds.",
            EstimatedMinutes = 25,
            Order = 10,
            LearningObjectives = new List<string> { "Understand NFC technology", "Learn touch-based communication" },
            Content = new List<LessonContent>
            {
                new() { Type = ContentType.Image, Title = "Touch Magic Spark", Data = "nfc_touch_spark", Description = "NFC creating magical sparks when devices touch" },
                new() { Type = ContentType.Text, Title = "Touch Communication", Data = "<h3>✨ Touch Wonders</h3><p>Tap to connect like magic handshakes!</p>" }
            },
            Prerequisites = new List<int> { 129 },
            Tags = new List<string> { "NFC", "touch", "near-field", "instant-connection" }
        });

        // Lesson 11: Interference - Wave Jams
        lessons.Add(new Lesson
        {
            Id = 131,
            ModuleId = 7,
            Title = "Interference: Wave Jams",
            Description = "Navigate wireless traffic jams! Learn about interference sources and how to avoid signal collisions.",
            EstimatedMinutes = 35,
            Order = 11,
            LearningObjectives = new List<string> { "Identify interference sources", "Learn mitigation strategies" },
            Content = new List<LessonContent>
            {
                new() { Type = ContentType.Image, Title = "Wireless Traffic Jam", Data = "wireless_traffic_jam", Description = "Wireless signals stuck in interference traffic jams" },
                new() { Type = ContentType.Text, Title = "Signal Interference", Data = "<h3>🚦 Wave Jams</h3><p>Microwaves disrupt like roadblocks!</p>" }
            },
            Prerequisites = new List<int> { 130 },
            Tags = new List<string> { "interference", "congestion", "microwave", "channel-hopping" }
        });

        // Lesson 12: Roaming - Seamless Switches
        lessons.Add(new Lesson
        {
            Id = 132,
            ModuleId = 7,
            Title = "Roaming: Seamless Switches",
            Description = "Master wireless handoffs! Learn how devices seamlessly switch between access points without dropping connections.",
            EstimatedMinutes = 35,
            Order = 12,
            LearningObjectives = new List<string> { "Understand roaming concepts", "Learn handoff mechanisms" },
            Content = new List<LessonContent>
            {
                new() { Type = ContentType.Image, Title = "Wireless Relay Race", Data = "wireless_relay_handover", Description = "Wireless roaming like a relay race baton pass" },
                new() { Type = ContentType.Text, Title = "Seamless Roaming", Data = "<h3>🏃 Seamless Switches</h3><p>Jump APs without dropping calls!</p>" }
            },
            Prerequisites = new List<int> { 131 },
            Tags = new List<string> { "roaming", "handoff", "seamless", "mobility" }
        });

        // Lesson 13: Scripting Wireless - Air Commands
        lessons.Add(new Lesson
        {
            Id = 133,
            ModuleId = 7,
            Title = "Scripting Wireless: Air Commands",
            Description = "Cast wireless spells with PowerShell! Learn to command airwaves with magical scripting powers.",
            EstimatedMinutes = 45,
            Order = 13,
            LearningObjectives = new List<string> { "Learn wireless PowerShell commands", "Automate wireless tasks" },
            Content = new List<LessonContent>
            {
                new() { Type = ContentType.Image, Title = "Magic Wand in Air", Data = "powershell_wand_wireless", Description = "PowerShell wand casting wireless command spells" },
                new() { Type = ContentType.Text, Title = "Wireless Scripting", Data = "<h3>🪄 Air Commands</h3><p>Get-NetAdapter -Wireless: View wave wonders!</p>" }
            },
            Prerequisites = new List<int> { 132 },
            Tags = new List<string> { "powershell", "scripting", "automation", "commands" }
        });

        // Lesson 14: Security in Wireless - Air Guards
        lessons.Add(new Lesson
        {
            Id = 134,
            ModuleId = 7,
            Title = "Security in Wireless: Air Guards",
            Description = "Deploy wireless guardians! Learn advanced security measures to protect your invisible highways.",
            EstimatedMinutes = 40,
            Order = 14,
            LearningObjectives = new List<string> { "Learn wireless security measures", "Understand MAC filtering" },
            Content = new List<LessonContent>
            {
                new() { Type = ContentType.Image, Title = "Shielded Wireless Waves", Data = "wireless_waves_with_shields", Description = "Wireless signals protected by security shields" },
                new() { Type = ContentType.Text, Title = "Wireless Security", Data = "<h3>🛡️ Air Guards</h3><p>MAC filtering: VIP air access only!</p>" }
            },
            Prerequisites = new List<int> { 133 },
            Tags = new List<string> { "wireless-security", "MAC-filtering", "guards", "protection" }
        });

        // Lesson 15: Troubleshooting Wireless - Wave Fixes
        lessons.Add(new Lesson
        {
            Id = 135,
            ModuleId = 7,
            Title = "Troubleshooting Wireless: Wave Fixes",
            Description = "Become a wireless detective! Learn to diagnose and fix wireless connectivity problems like a pro.",
            EstimatedMinutes = 40,
            Order = 15,
            LearningObjectives = new List<string> { "Learn wireless troubleshooting", "Master diagnostic tools" },
            Content = new List<LessonContent>
            {
                new() { Type = ContentType.Image, Title = "Wireless Detective", Data = "detective_investigating_wireless", Description = "Wireless detective investigating signal problems in the air" },
                new() { Type = ContentType.Text, Title = "Wireless Troubleshooting", Data = "<h3>🔍 Wave Fixes</h3><p>Test signal strength like highway conditions!</p>" }
            },
            Prerequisites = new List<int> { 134 },
            Tags = new List<string> { "troubleshooting", "diagnostics", "fixes", "detective" }
        });

        // Lesson 16: 5G and Beyond - Future Waves
        lessons.Add(new Lesson
        {
            Id = 136,
            ModuleId = 7,
            Title = "5G and Beyond: Future Waves",
            Description = "Explore the future of wireless! Learn about 5G technology and next-generation wireless innovations.",
            EstimatedMinutes = 45,
            Order = 16,
            LearningObjectives = new List<string> { "Understand 5G technology", "Learn about future wireless" },
            Content = new List<LessonContent>
            {
                new() { Type = ContentType.Image, Title = "Futuristic 5G Signals", Data = "futuristic_5g_waves", Description = "Futuristic 5G signals creating ultra-fast highways" },
                new() { Type = ContentType.Text, Title = "5G Technology", Data = "<h3>🚀 Future Waves</h3><p>5G: Ultra-fast mobile superhighways!</p>" }
            },
            Prerequisites = new List<int> { 135 },
            Tags = new List<string> { "5G", "future", "mobile", "ultra-fast" }
        });

        // Lesson 17: IoT Wireless - Gadget Waves
        lessons.Add(new Lesson
        {
            Id = 137,
            ModuleId = 7,
            Title = "IoT Wireless: Gadget Waves",
            Description = "Connect the Internet of Things! Learn about specialized wireless protocols for smart home gadgets.",
            EstimatedMinutes = 40,
            Order = 17,
            LearningObjectives = new List<string> { "Learn IoT wireless protocols", "Understand Zigbee and Z-Wave" },
            Content = new List<LessonContent>
            {
                new() { Type = ContentType.Image, Title = "Connected Smart Devices", Data = "iot_devices_connected", Description = "Smart home devices connected through specialized wireless waves" },
                new() { Type = ContentType.Text, Title = "IoT Wireless", Data = "<h3>🏠 Gadget Waves</h3><p>Zigbee, Z-Wave: Low-power smart roads!</p>" }
            },
            Prerequisites = new List<int> { 136 },
            Tags = new List<string> { "IoT", "zigbee", "z-wave", "smart-home" }
        });

        // Lesson 18: Wireless Topologies - Air Formations
        lessons.Add(new Lesson
        {
            Id = 138,
            ModuleId = 7,
            Title = "Wireless Topologies: Air Formations",
            Description = "Master wireless network patterns! Learn different ways to organize wireless connections in the air.",
            EstimatedMinutes = 40,
            Order = 18,
            LearningObjectives = new List<string> { "Understand wireless topologies", "Learn ad-hoc networks" },
            Content = new List<LessonContent>
            {
                new() { Type = ContentType.Image, Title = "Aerial Network Patterns", Data = "wireless_topology_patterns", Description = "Different wireless network formation patterns in the air" },
                new() { Type = ContentType.Text, Title = "Wireless Patterns", Data = "<h3>✈️ Air Formations</h3><p>Ad-hoc: Peer-to-peer sky meetings!</p>" }
            },
            Prerequisites = new List<int> { 137 },
            Tags = new List<string> { "topology", "ad-hoc", "formations", "patterns" }
        });

        // Lesson 19: Cert-Level - WiFi Protocols
        lessons.Add(new Lesson
        {
            Id = 139,
            ModuleId = 7,
            Title = "Cert-Level: WiFi Protocols",
            Description = "Master advanced wireless protocols! Tackle certification-level concepts and become a wireless wizard.",
            EstimatedMinutes = 50,
            Order = 19,
            LearningObjectives = new List<string> { "Master advanced protocols", "Learn EAP authentication" },
            Content = new List<LessonContent>
            {
                new() { Type = ContentType.Image, Title = "Advanced Wireless Waves", Data = "advanced_wifi_protocols", Description = "Complex wireless protocol waves with advanced patterns" },
                new() { Type = ContentType.Text, Title = "Advanced Protocols", Data = "<h3>🎓 Advanced Wireless</h3><p>EAP methods: Authentication flows!</p>" }
            },
            Prerequisites = new List<int> { 138 },
            Tags = new List<string> { "certification", "advanced", "EAP", "protocols" }
        });

        // Lesson 20: Quiz Wonders - Wireless Mastery
        lessons.Add(new Lesson
        {
            Id = 140,
            ModuleId = 7,
            Title = "Quiz Wonders: Wireless Mastery",
            Description = "The ultimate wireless challenge! Prove your mastery of invisible highways with this comprehensive wonder quiz.",
            EstimatedMinutes = 45,
            Order = 20,
            LearningObjectives = new List<string> { "Test wireless mastery", "Complete comprehensive assessment" },
            Content = new List<LessonContent>
            {
                new() { Type = ContentType.Image, Title = "Wireless Mastery Trophy", Data = "wireless_trophy_with_waves", Description = "Magical trophy surrounded by mastered wireless waves" },
                new() { Type = ContentType.Quiz, Title = "Wireless Mastery Quiz", Data = "{\"question\": \"Wireless mastery quiz coming soon!\", \"options\": [\"Ready!\"], \"correctAnswer\": 0}" }
            },
            Prerequisites = new List<int> { 139 },
            Tags = new List<string> { "final-quiz", "mastery-test", "comprehensive", "wireless-wizard" }
        });

        return lessons;
    }

    /// <summary>
    /// Get Module 7 wireless mastery badges
    /// </summary>
    public static List<Badge> GetModule7Badges()
    {
        return new List<Badge>
        {
            new()
            {
                Id = "wave_wanderer",
                Name = "Wave Wanderer",
                Description = "Begin your journey through invisible highways! Completed your first wireless lesson.",
                Icon = "📡",
                Category = "Wireless Basics",
                Points = 50,
                Rarity = BadgeRarity.Common,
                UnlockCriteria = "Complete Lesson 1: What's Wireless? Magic Waves"
            },
            new()
            {
                Id = "wifi_warrior",
                Name = "WiFi Warrior",
                Description = "Conqueror of home wireless highways! You understand how WiFi creates magical connection zones.",
                Icon = "🏠",
                Category = "WiFi Fundamentals",
                Points = 75,
                Rarity = BadgeRarity.Common,
                UnlockCriteria = "Score 75%+ on Lesson 2: WiFi Basics - Home Highways"
            },
            new()
            {
                Id = "standards_sage",
                Name = "Standards Sage",
                Description = "Master of the WiFi family tree! You know the evolution from pony speed to jet speed connections.",
                Icon = "👨‍👩‍👧‍👦",
                Category = "WiFi Standards",
                Points = 100,
                Rarity = BadgeRarity.Uncommon,
                UnlockCriteria = "Score 80%+ on Lesson 3: Standards - 802.11 Family"
            },
            new()
            {
                Id = "frequency_phantom",
                Name = "Frequency Phantom",
                Description = "Navigator of invisible spectrum lanes! You understand the trade-offs between different frequency highways.",
                Icon = "📻",
                Category = "Frequency Management",
                Points = 100,
                Rarity = BadgeRarity.Uncommon,
                UnlockCriteria = "Score 80%+ on Lesson 4: Frequencies - Wave Bands"
            },
            new()
            {
                Id = "beacon_keeper",
                Name = "Beacon Keeper",
                Description = "Guardian of wireless lighthouses! You know how access points guide data ships to safety.",
                Icon = "🗼",
                Category = "Infrastructure",
                Points = 125,
                Rarity = BadgeRarity.Uncommon,
                UnlockCriteria = "Score 85%+ on Lesson 5: Access Points - Wave Stations"
            },
            new()
            {
                Id = "network_namer",
                Name = "Network Namer",
                Description = "Master of digital name tags! You understand how SSIDs help devices find their wireless homes.",
                Icon = "🏷️",
                Category = "Network Identification",
                Points = 75,
                Rarity = BadgeRarity.Common,
                UnlockCriteria = "Score 75%+ on Lesson 6: SSID - Network Names"
            },
            new()
            {
                Id = "wave_protector",
                Name = "Wave Protector",
                Description = "Defender of airwave security! You know how to encrypt wireless traffic with invisible armor.",
                Icon = "🔐",
                Category = "Wireless Security",
                Points = 125,
                Rarity = BadgeRarity.Uncommon,
                UnlockCriteria = "Score 85%+ on Lesson 7: Encryption - Secure Waves"
            },
            new()
            {
                Id = "mesh_master",
                Name = "Mesh Master",
                Description = "Weaver of wireless webs! You understand how mesh networks create seamless coverage through teamwork.",
                Icon = "🕸️",
                Category = "Mesh Networking",
                Points = 150,
                Rarity = BadgeRarity.Rare,
                UnlockCriteria = "Score 90%+ on Lesson 8: Mesh Networks - Wave Webs"
            },
            new()
            {
                Id = "bluetooth_whisperer",
                Name = "Bluetooth Whisperer",
                Description = "Master of intimate device conversations! You know how to make devices whisper secrets wirelessly.",
                Icon = "🔮",
                Category = "Personal Area Networks",
                Points = 100,
                Rarity = BadgeRarity.Common,
                UnlockCriteria = "Score 80%+ on Lesson 9: Bluetooth - Short-Range Magic"
            },
            new()
            {
                Id = "touch_wizard",
                Name = "Touch Wizard",
                Description = "Conjurer of magical handshakes! You understand the instant magic of NFC touch connections.",
                Icon = "✨",
                Category = "Near Field Communication",
                Points = 100,
                Rarity = BadgeRarity.Common,
                UnlockCriteria = "Score 80%+ on Lesson 10: NFC - Touch Wonders"
            },
            new()
            {
                Id = "interference_investigator",
                Name = "Interference Investigator",
                Description = "Detective of wireless traffic jams! You can identify and solve signal interference problems.",
                Icon = "🚦",
                Category = "Troubleshooting",
                Points = 125,
                Rarity = BadgeRarity.Uncommon,
                UnlockCriteria = "Score 85%+ on Lesson 11: Interference - Wave Jams"
            },
            new()
            {
                Id = "roaming_runner",
                Name = "Roaming Runner",
                Description = "Champion of seamless switches! You know how devices run between access points without dropping connections.",
                Icon = "🏃",
                Category = "Mobility",
                Points = 125,
                Rarity = BadgeRarity.Uncommon,
                UnlockCriteria = "Score 85%+ on Lesson 12: Roaming - Seamless Switches"
            },
            new()
            {
                Id = "air_commander",
                Name = "Air Commander",
                Description = "Wizard of wireless spells! You can cast PowerShell commands to control airwaves.",
                Icon = "🪄",
                Category = "Scripting",
                Points = 150,
                Rarity = BadgeRarity.Rare,
                UnlockCriteria = "Score 90%+ on Lesson 13: Scripting Wireless - Air Commands"
            },
            new()
            {
                Id = "wireless_guardian",
                Name = "Wireless Guardian",
                Description = "Protector of invisible highways! You know how to deploy advanced security measures for wireless networks.",
                Icon = "🛡️",
                Category = "Advanced Security",
                Points = 150,
                Rarity = BadgeRarity.Rare,
                UnlockCriteria = "Score 90%+ on Lesson 14: Security in Wireless - Air Guards"
            },
            new()
            {
                Id = "signal_sleuth",
                Name = "Signal Sleuth",
                Description = "Master detective of wireless mysteries! You can diagnose and fix any wireless connection problem.",
                Icon = "🔍",
                Category = "Diagnostics",
                Points = 150,
                Rarity = BadgeRarity.Rare,
                UnlockCriteria = "Score 90%+ on Lesson 15: Troubleshooting Wireless - Wave Fixes"
            },
            new()
            {
                Id = "future_forecaster",
                Name = "Future Forecaster",
                Description = "Prophet of next-generation wireless! You understand 5G and the future of wireless technology.",
                Icon = "🚀",
                Category = "Future Technology",
                Points = 175,
                Rarity = BadgeRarity.Epic,
                UnlockCriteria = "Score 90%+ on Lesson 16: 5G and Beyond - Future Waves"
            },
            new()
            {
                Id = "iot_orchestrator",
                Name = "IoT Orchestrator",
                Description = "Conductor of smart device symphonies! You know how to connect and manage Internet of Things gadgets.",
                Icon = "🏠",
                Category = "IoT Networks",
                Points = 150,
                Rarity = BadgeRarity.Rare,
                UnlockCriteria = "Score 90%+ on Lesson 17: IoT Wireless - Gadget Waves"
            },
            new()
            {
                Id = "formation_flyer",
                Name = "Formation Flyer",
                Description = "Architect of aerial patterns! You understand different wireless topology formations and their uses.",
                Icon = "✈️",
                Category = "Network Topology",
                Points = 175,
                Rarity = BadgeRarity.Epic,
                UnlockCriteria = "Score 90%+ on Lesson 18: Wireless Topologies - Air Formations"
            },
            new()
            {
                Id = "protocol_professor",
                Name = "Protocol Professor",
                Description = "Scholar of advanced wireless protocols! You've mastered certification-level WiFi concepts.",
                Icon = "🎓",
                Category = "Advanced Protocols",
                Points = 200,
                Rarity = BadgeRarity.Epic,
                UnlockCriteria = "Score 95%+ on Lesson 19: Cert-Level - WiFi Protocols"
            },
            new()
            {
                Id = "wireless_wonder_wizard",
                Name = "Wireless Wonder Wizard",
                Description = "Ultimate master of invisible highways! You have conquered all wireless challenges and earned the right to be called a true wireless wonder wizard!",
                Icon = "🧙‍♂️📡",
                Category = "Master Achievement",
                Points = 500,
                Rarity = BadgeRarity.Legendary,
                UnlockCriteria = "Complete Module 7 with 95%+ average score"
            }
        };
    }
}