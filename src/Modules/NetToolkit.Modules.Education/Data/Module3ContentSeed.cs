using NetToolkit.Modules.Education.Models;

namespace NetToolkit.Modules.Education.Data;

/// <summary>
/// Module 3 content seed data - "IP Shenanigans - Addresses: Not Just for Mail"
/// A witty adventure through the mystical world of IP addresses and networking protocols
/// </summary>
public static class Module3ContentSeed
{
    /// <summary>
    /// Get the complete Module 3 content structure
    /// </summary>
    public static Module GetModule3Content()
    {
        return new Module
        {
            Id = 3,
            Title = "IP Shenanigans - Addresses: Not Just for Mail",
            Description = "Embark on a hilarious journey through the mystical realm of IP addresses! From digital doorplates to subnet shenanigans, discover how addresses power the internet universe. Perfect for aspiring address architects and subnet sorcerers!",
            Difficulty = DifficultyLevel.Intermediate,
            EstimatedMinutes = 480, // 8 hours total
            Prerequisites = "Module 1: Network Basics, Module 2: Hardware Heroes",
            LearningOutcomes = "Master IP addressing, subnet calculations, DHCP operations, NAT functionality, IPv6 migration, security considerations, PowerShell IP commands, and advanced troubleshooting techniques.",
            CreatedAt = DateTime.UtcNow,
            Lessons = GetAllIPLessons()
        };
    }

    /// <summary>
    /// Generate all 20 IP-focused lessons with escalating complexity
    /// </summary>
    private static List<Lesson> GetAllIPLessons()
    {
        var lessons = new List<Lesson>();

        // Lesson 1: What's an IP? The Digital Doorplate
        lessons.Add(new Lesson
        {
            Id = 41, // Continuing from Module 2's 40 lessons
            ModuleId = 3,
            LessonNumber = 1,
            Title = "What's an IP? The Digital Doorplate",
            Description = "Meet IP addresses - the magical house numbers of the internet that help data find its way home!",
            Objectives = "Understand what IP addresses are and why they're essential for network communication",
            EstimatedMinutes = 15,
            PassingScore = 70,
            ContentJson = CreateLesson1Content()
        });

        // Lesson 2: IPv4 vs IPv6: Old School vs New Wave
        lessons.Add(new Lesson
        {
            Id = 42,
            ModuleId = 3,
            LessonNumber = 2,
            Title = "IPv4 vs IPv6: Old School vs New Wave",
            Description = "Explore the epic battle between the classic IPv4 addresses and the futuristic IPv6 powerhouses!",
            Objectives = "Compare and contrast IPv4 and IPv6 addressing schemes and their practical applications",
            EstimatedMinutes = 20,
            PassingScore = 70,
            ContentJson = CreateLesson2Content()
        });

        // Lesson 3: Dynamic IPs: The Changing Addresses
        lessons.Add(new Lesson
        {
            Id = 43,
            ModuleId = 3,
            LessonNumber = 3,
            Title = "Dynamic IPs: The Changing Addresses",
            Description = "Discover the magical world of dynamic IP addresses that dance and change like digital nomads!",
            Objectives = "Understand dynamic IP allocation, its benefits and challenges in network management",
            EstimatedMinutes = 18,
            PassingScore = 70,
            ContentJson = CreateLesson3Content()
        });

        // Lesson 4: Static IPs: The Fixed Abodes
        lessons.Add(new Lesson
        {
            Id = 44,
            ModuleId = 3,
            LessonNumber = 4,
            Title = "Static IPs: The Fixed Abodes",
            Description = "Meet the steady and reliable static IP addresses - the permanent residents of the network neighborhood!",
            Objectives = "Learn when and why to use static IP addresses and their configuration requirements",
            EstimatedMinutes = 16,
            PassingScore = 70,
            ContentJson = CreateLesson4Content()
        });

        // Lesson 5: DHCP: The Address Butler
        lessons.Add(new Lesson
        {
            Id = 45,
            ModuleId = 3,
            LessonNumber = 5,
            Title = "DHCP: The Address Butler",
            Description = "Meet DHCP, the courteous butler that hands out IP addresses like a well-trained digital concierge!",
            Objectives = "Understand DHCP operation, lease management, and common configuration scenarios",
            EstimatedMinutes = 22,
            PassingScore = 75,
            ContentJson = CreateLesson5Content()
        });

        // Lesson 6: Subnets: Dividing the Neighborhood
        lessons.Add(new Lesson
        {
            Id = 46,
            ModuleId = 3,
            LessonNumber = 6,
            Title = "Subnets: Dividing the Neighborhood",
            Description = "Explore the art of subnet wizardry - dividing IP ranges like a digital city planner!",
            Objectives = "Master basic subnetting concepts, subnet masks, and network segmentation principles",
            EstimatedMinutes = 25,
            PassingScore = 75,
            ContentJson = CreateLesson6Content()
        });

        // Lesson 7: CIDR: The Smart Slicer
        lessons.Add(new Lesson
        {
            Id = 47,
            ModuleId = 3,
            LessonNumber = 7,
            Title = "CIDR: The Smart Slicer",
            Description = "Master CIDR notation - the elegant pizza slicer of network addressing that makes subnetting a breeze!",
            Objectives = "Understand CIDR notation, calculate network ranges, and apply flexible subnetting",
            EstimatedMinutes = 28,
            PassingScore = 80,
            ContentJson = CreateLesson7Content()
        });

        // Lesson 8: Private vs Public IPs: Inside/Outside Worlds
        lessons.Add(new Lesson
        {
            Id = 48,
            ModuleId = 3,
            LessonNumber = 8,
            Title = "Private vs Public IPs: Inside/Outside Worlds",
            Description = "Navigate the dual universe of private and public IP addresses - like having both home and street addresses!",
            Objectives = "Distinguish between private and public IP ranges and understand their security implications",
            EstimatedMinutes = 20,
            PassingScore = 75,
            ContentJson = CreateLesson8Content()
        });

        // Lesson 9: NAT: The Address Translator
        lessons.Add(new Lesson
        {
            Id = 49,
            ModuleId = 3,
            LessonNumber = 9,
            Title = "NAT: The Address Translator",
            Description = "Meet NAT, the diplomatic translator that helps private networks speak to the public internet!",
            Objectives = "Understand NAT operation, types of NAT, and troubleshooting NAT-related issues",
            EstimatedMinutes = 24,
            PassingScore = 80,
            ContentJson = CreateLesson9Content()
        });

        // Lesson 10: ARP: The Address Resolver
        lessons.Add(new Lesson
        {
            Id = 50,
            ModuleId = 3,
            LessonNumber = 10,
            Title = "ARP: The Address Resolver",
            Description = "Discover ARP, the detective protocol that maps IP addresses to MAC addresses like a digital sleuth!",
            Objectives = "Master ARP operation, ARP tables, and common ARP troubleshooting techniques",
            EstimatedMinutes = 20,
            PassingScore = 75,
            ContentJson = CreateLesson10Content()
        });

        // Lesson 11: IP Classes: The Old Hierarchy
        lessons.Add(new Lesson
        {
            Id = 51,
            ModuleId = 3,
            LessonNumber = 11,
            Title = "IP Classes: The Old Hierarchy",
            Description = "Journey back to the legacy world of IP classes - the traditional approach to network organization!",
            Objectives = "Understand classful networking, its limitations, and why we moved to classless systems",
            EstimatedMinutes = 18,
            PassingScore = 70,
            ContentJson = CreateLesson11Content()
        });

        // Lesson 12: Loopback: Talking to Yourself
        lessons.Add(new Lesson
        {
            Id = 52,
            ModuleId = 3,
            LessonNumber = 12,
            Title = "Loopback: Talking to Yourself",
            Description = "Meet the localhost address - where your computer practices its networking skills in front of the mirror!",
            Objectives = "Understand loopback addresses, their uses in testing and development",
            EstimatedMinutes = 15,
            PassingScore = 70,
            ContentJson = CreateLesson12Content()
        });

        // Lesson 13: Multicast: Group Chats
        lessons.Add(new Lesson
        {
            Id = 53,
            ModuleId = 3,
            LessonNumber = 13,
            Title = "Multicast: Group Chats",
            Description = "Explore multicast addressing - the group messaging system of the networking world!",
            Objectives = "Understand multicast concepts, IGMP, and multicast routing principles",
            EstimatedMinutes = 22,
            PassingScore = 75,
            ContentJson = CreateLesson13Content()
        });

        // Lesson 14: Anycast: Closest Buddy
        lessons.Add(new Lesson
        {
            Id = 54,
            ModuleId = 3,
            LessonNumber = 14,
            Title = "Anycast: Closest Buddy",
            Description = "Discover anycast routing - the smart addressing that always finds your nearest network friend!",
            Objectives = "Learn anycast principles, applications in CDNs, and routing considerations",
            EstimatedMinutes = 20,
            PassingScore = 80,
            ContentJson = CreateLesson14Content()
        });

        // Lesson 15: IP Security: Guarding Addresses
        lessons.Add(new Lesson
        {
            Id = 55,
            ModuleId = 3,
            LessonNumber = 15,
            Title = "IP Security: Guarding Addresses",
            Description = "Master IP security - protecting your addresses like digital fortresses with encrypted envelopes!",
            Objectives = "Understand IPsec, VPNs, and IP-based security measures",
            EstimatedMinutes = 26,
            PassingScore = 80,
            ContentJson = CreateLesson15Content()
        });

        // Lesson 16: Scripting IPs: Command Shenanigans
        lessons.Add(new Lesson
        {
            Id = 56,
            ModuleId = 3,
            LessonNumber = 16,
            Title = "Scripting IPs: Command Shenanigans",
            Description = "Wield the magical powers of PowerShell to configure and manage IP addresses like a network wizard!",
            Objectives = "Master PowerShell IP commands and automation techniques",
            EstimatedMinutes = 30,
            PassingScore = 85,
            ContentJson = CreateLesson16Content()
        });

        // Lesson 17: Troubleshooting IPs: Fix the Mix-Ups
        lessons.Add(new Lesson
        {
            Id = 57,
            ModuleId = 3,
            LessonNumber = 17,
            Title = "Troubleshooting IPs: Fix the Mix-Ups",
            Description = "Become an IP detective - solving address mysteries and fixing network puzzles with style!",
            Objectives = "Master common IP troubleshooting techniques and diagnostic tools",
            EstimatedMinutes = 28,
            PassingScore = 80,
            ContentJson = CreateLesson17Content()
        });

        // Lesson 18: IPv6 Adoption: Future-Proofing
        lessons.Add(new Lesson
        {
            Id = 58,
            ModuleId = 3,
            LessonNumber = 18,
            Title = "IPv6 Adoption: Future-Proofing",
            Description = "Prepare for the IPv6 future - unlimited addresses for a connected cosmos of endless possibilities!",
            Objectives = "Understand IPv6 migration strategies, dual-stack configurations, and future planning",
            EstimatedMinutes = 25,
            PassingScore = 85,
            ContentJson = CreateLesson18Content()
        });

        // Lesson 19: Cert-Level: Subnet Calculations
        lessons.Add(new Lesson
        {
            Id = 59,
            ModuleId = 3,
            LessonNumber = 19,
            Title = "Cert-Level: Subnet Calculations",
            Description = "Master advanced subnet mathematics - the binary wizardry that separates networking pros from rookies!",
            Objectives = "Master VLSM, complex subnet calculations, and optimization techniques",
            EstimatedMinutes = 35,
            PassingScore = 90,
            ContentJson = CreateLesson19Content()
        });

        // Lesson 20: Quiz Shenanigans: IP Mastery
        lessons.Add(new Lesson
        {
            Id = 60,
            ModuleId = 3,
            LessonNumber = 20,
            Title = "Quiz Shenanigans: IP Mastery",
            Description = "The ultimate IP challenge - prove your addressing mastery in this epic finale of network knowledge!",
            Objectives = "Demonstrate comprehensive understanding of IP addressing concepts",
            EstimatedMinutes = 40,
            PassingScore = 90,
            ContentJson = CreateLesson20Content()
        });

        return lessons;
    }

    #region Lesson Content Generators

    private static string CreateLesson1Content()
    {
        var content = new LessonContent
        {
            Slides = new List<Slide>
            {
                new Slide
                {
                    Type = SlideType.Introduction,
                    Title = "What's an IP? The Digital Doorplate",
                    Content = "🏠 Welcome to the world of IP addresses! Just like your house has a street address for mail delivery, every device on the internet needs an IP address for data delivery. Let's discover these magical digital doorplates!",
                    HoverTips = { "IP stands for Internet Protocol!", "Every connected device gets a unique address" }
                },
                new Slide
                {
                    Type = SlideType.Image,
                    Title = "Digital House with Address",
                    Content = "Imagine the internet as a massive neighborhood where every house (device) needs a unique address to receive visitors (data packets)!",
                    ImageData = "ip_house_address"
                },
                new Slide
                {
                    Type = SlideType.Text,
                    Title = "IP Address Basics",
                    Content = "🔢 An IP address is like a postal address for the digital world:\n\n• IPv4 addresses look like: 192.168.1.1\n• Made up of four numbers (0-255) separated by dots\n• Each number is called an 'octet'\n• Total possible combinations: Over 4 billion addresses!",
                    HoverTips = { "IPv4 = Internet Protocol version 4", "Octet = 8 bits of data", "192.168.x.x are private addresses" }
                }
            },
            Quiz = new List<QuizQuestion>
            {
                new QuizQuestion
                {
                    Question = "What does IP stand for in IP address?",
                    Options = { "Internet Protocol", "Internal Process", "Information Package", "Internet Package" },
                    CorrectAnswerIndex = 0,
                    Explanation = "IP stands for Internet Protocol - the fundamental communication protocol of the internet!",
                    IncorrectFeedback = { "Not quite! Think 'Protocol' not 'Process'", "Close, but it's about protocols, not packages", "Almost there, but it's 'Protocol' not 'Package'" }
                }
            }
        };

        return Newtonsoft.Json.JsonConvert.SerializeObject(content, Newtonsoft.Json.Formatting.Indented);
    }

    private static string CreateLesson2Content()
    {
        var content = new LessonContent
        {
            Slides = new List<Slide>
            {
                new Slide
                {
                    Type = SlideType.Introduction,
                    Title = "IPv4 vs IPv6: Old School vs New Wave",
                    Content = "🏚️ vs 🏢 Time for the ultimate address showdown! IPv4, the veteran with 4 billion addresses, faces IPv6, the newcomer with virtually unlimited space. Who will win your network's heart?",
                    HoverTips = { "IPv4 was created in the 1970s!", "IPv6 has 340 undecillion addresses!" }
                },
                new Slide
                {
                    Type = SlideType.Image,
                    Title = "Vintage vs Modern Houses",
                    Content = "IPv4 is like a cozy vintage house, while IPv6 is a massive modern skyscraper with rooms for everyone!",
                    ImageData = "ipv4_vs_ipv6_houses"
                },
                new Slide
                {
                    Type = SlideType.Text,
                    Title = "The Great Comparison",
                    Content = "📊 IPv4 vs IPv6 Showdown:\n\n**IPv4 (The Classic):**\n• Format: 192.168.1.1 (32-bit)\n• Addresses: ~4.3 billion\n• Age: Since 1981\n• Status: Running out of room!\n\n**IPv6 (The Future):**\n• Format: 2001:db8::1 (128-bit)\n• Addresses: 340 undecillion (that's 36 zeros!)\n• Age: Since 1998\n• Status: Ready for infinite devices!",
                    HoverTips = { "Undecillion = 1 followed by 36 zeros", "IPv6 uses hexadecimal notation", "Both can coexist on same network" }
                }
            },
            Quiz = new List<QuizQuestion>
            {
                new QuizQuestion
                {
                    Question = "How many bits does an IPv6 address contain?",
                    Options = { "32 bits", "64 bits", "96 bits", "128 bits" },
                    CorrectAnswerIndex = 3,
                    Explanation = "IPv6 addresses are 128 bits long, giving us virtually unlimited address space!",
                    IncorrectFeedback = { "That's IPv4! IPv6 is much larger", "Bigger! IPv6 needs more space", "Close, but IPv6 is even larger!" }
                }
            }
        };

        return Newtonsoft.Json.JsonConvert.SerializeObject(content, Newtonsoft.Json.Formatting.Indented);
    }

    private static string CreateLesson3Content()
    {
        var content = new LessonContent
        {
            Slides = new List<Slide>
            {
                new Slide
                {
                    Type = SlideType.Introduction,
                    Title = "Dynamic IPs: The Changing Addresses",
                    Content = "🎪 Welcome to the magical world of dynamic IP addresses! Like musical chairs at a digital party, these addresses dance and change, keeping your network flexible and fun!",
                    HoverTips = { "DHCP assigns dynamic IPs automatically", "Perfect for most home networks" }
                },
                new Slide
                {
                    Type = SlideType.Image,
                    Title = "Musical Chairs Network",
                    Content = "Dynamic IPs are like a cosmic game of musical chairs - devices get different seats (addresses) each time they join the network party!",
                    ImageData = "dynamic_ip_musical_chairs"
                },
                new Slide
                {
                    Type = SlideType.Text,
                    Title = "Dynamic IP Magic",
                    Content = "✨ How Dynamic IPs Work:\n\n• **Auto-Assignment:** DHCP server hands out available addresses\n• **Temporary Leases:** Addresses are 'rented' for a specific time\n• **Automatic Renewal:** Devices request lease extensions\n• **Pool Management:** Server maintains a pool of available IPs\n\n**Pros:** Easy setup, efficient use of addresses\n**Cons:** Addresses can change unexpectedly!",
                    HoverTips = { "Lease time usually 24-168 hours", "Great for laptops and phones", "Conflicts are automatically avoided" }
                }
            },
            Quiz = new List<QuizQuestion>
            {
                new QuizQuestion
                {
                    Question = "What protocol typically assigns dynamic IP addresses?",
                    Options = { "DNS", "DHCP", "ARP", "ICMP" },
                    CorrectAnswerIndex = 1,
                    Explanation = "DHCP (Dynamic Host Configuration Protocol) is the address butler that assigns dynamic IPs!",
                    IncorrectFeedback = { "DNS resolves names, not addresses!", "ARP maps IPs to MACs, different job!", "ICMP is for ping, not addressing!" }
                }
            }
        };

        return Newtonsoft.Json.JsonConvert.SerializeObject(content, Newtonsoft.Json.Formatting.Indented);
    }

    private static string CreateLesson4Content()
    {
        var content = new LessonContent
        {
            Slides = new List<Slide>
            {
                new Slide
                {
                    Type = SlideType.Introduction,
                    Title = "Static IPs: The Fixed Abodes",
                    Content = "🏠 Meet the steady and reliable static IP addresses - the permanent residents who never move! These addresses are like owning your digital home forever, perfect for servers and important devices.",
                    HoverTips = { "Servers usually need static IPs", "No DHCP required for static addresses" }
                },
                new Slide
                {
                    Type = SlideType.Image,
                    Title = "Anchored House",
                    Content = "Static IP addresses are like houses anchored firmly to their foundations - they never move and everyone always knows where to find them!",
                    ImageData = "static_ip_anchored_house"
                },
                new Slide
                {
                    Type = SlideType.Text,
                    Title = "When to Go Static",
                    Content = "⚓ Static IP Perfect For:\n\n• **Web Servers:** Always at the same address\n• **Email Servers:** Reliable mail delivery\n• **Printers:** Everyone can find them easily\n• **Remote Access:** VPN endpoints need stability\n• **Gaming Servers:** Consistent connection point\n\n**Remember:** Manual configuration required!\n**Scanner Tip:** Check your NIC properties to see current IP type!",
                    HoverTips = { "Static = manually configured", "Great for critical infrastructure", "Requires careful IP planning" }
                }
            },
            Quiz = new List<QuizQuestion>
            {
                new QuizQuestion
                {
                    Question = "Which type of server typically needs a static IP address?",
                    Options = { "Web server", "DHCP client", "Mobile device", "Temporary workstation" },
                    CorrectAnswerIndex = 0,
                    Explanation = "Web servers need static IPs so visitors can always find them at the same address!",
                    IncorrectFeedback = { "DHCP clients get dynamic addresses!", "Mobile devices work great with dynamic IPs", "Temporary workstations don't need fixed addresses" }
                }
            }
        };

        return Newtonsoft.Json.JsonConvert.SerializeObject(content, Newtonsoft.Json.Formatting.Indented);
    }

    private static string CreateLesson5Content()
    {
        var content = new LessonContent
        {
            Slides = new List<Slide>
            {
                new Slide
                {
                    Type = SlideType.Introduction,
                    Title = "DHCP: The Address Butler",
                    Content = "🤵 Meet DHCP, your network's courteous butler! This distinguished service hands out IP addresses like a well-trained digital concierge, ensuring every device gets exactly what it needs to join the network party!",
                    HoverTips = { "DHCP = Dynamic Host Configuration Protocol", "Invented to automate IP assignment" }
                },
                new Slide
                {
                    Type = SlideType.Image,
                    Title = "Butler Handing Keys",
                    Content = "Like an elegant butler presenting keys to guests, DHCP gracefully assigns IP addresses, subnet masks, gateways, and DNS servers to network devices!",
                    ImageData = "dhcp_butler_keys"
                },
                new Slide
                {
                    Type = SlideType.Text,
                    Title = "The DHCP Dance",
                    Content = "💃 The Four-Step DHCP Tango:\n\n1. **DISCOVER:** \"Hello, I need an IP!\" (Client broadcasts)\n2. **OFFER:** \"Here's what I can give you!\" (Server responds)\n3. **REQUEST:** \"Yes, I'll take it!\" (Client accepts)\n4. **ACKNOWLEDGE:** \"It's yours! Enjoy!\" (Server confirms)\n\n**Lease Details:**\n• Duration: Usually 24-168 hours\n• Renewal: Automatic at 50% lease time\n• Don't worry about lease expiration - DHCP renews automatically!",
                    HoverTips = { "Also called DORA process", "Broadcast means everyone hears it", "Lease renewal is automatic" }
                }
            },
            Quiz = new List<QuizQuestion>
            {
                new QuizQuestion
                {
                    Question = "What is the correct order of the DHCP process?",
                    Options = { "Offer, Discover, Acknowledge, Request", "Discover, Offer, Request, Acknowledge", "Request, Discover, Offer, Acknowledge", "Acknowledge, Request, Discover, Offer" },
                    CorrectAnswerIndex = 1,
                    Explanation = "The DHCP process follows DORA: Discover, Offer, Request, Acknowledge - like a polite conversation!",
                    IncorrectFeedback = { "Start with Discover - the client asks first!", "Close! Remember DORA sequence", "The client must Discover before anything else!" }
                }
            }
        };

        return Newtonsoft.Json.JsonConvert.SerializeObject(content, Newtonsoft.Json.Formatting.Indented);
    }

    private static string CreateLesson6Content()
    {
        var content = new LessonContent
        {
            Slides = new List<Slide>
            {
                new Slide
                {
                    Type = SlideType.Introduction,
                    Title = "Subnets: Dividing the Neighborhood",
                    Content = "🏘️ Welcome to subnet city planning! Just like dividing a city into neighborhoods, subnets split IP networks into organized, manageable sections. Time to become a digital urban planner!",
                    HoverTips = { "Subnet = sub-network", "Helps organize and secure networks" }
                },
                new Slide
                {
                    Type = SlideType.Image,
                    Title = "Fenced City Blocks",
                    Content = "Subnets are like fenced city blocks - each section has its own identity while being part of the larger city (network)!",
                    ImageData = "subnet_fenced_blocks"
                },
                new Slide
                {
                    Type = SlideType.Text,
                    Title = "Subnet Mask Magic",
                    Content = "🎭 Understanding Subnet Masks:\n\n• **Purpose:** Defines network vs host portions\n• **Common Masks:**\n  - /24 or 255.255.255.0 = 256 addresses (254 hosts)\n  - /16 or 255.255.0.0 = 65,536 addresses\n  - /8 or 255.0.0.0 = 16,777,216 addresses\n\n**Benefits:**\n• Improved security (segment traffic)\n• Better performance (reduce broadcast domains)\n• Organized network management\n• Efficient IP address usage",
                    HoverTips = { "/24 means 24 bits for network", "254 hosts because 2 addresses reserved", "Broadcast domain = who hears broadcasts" }
                }
            },
            Quiz = new List<QuizQuestion>
            {
                new QuizQuestion
                {
                    Question = "How many host addresses are available in a /24 subnet?",
                    Options = { "256", "255", "254", "253" },
                    CorrectAnswerIndex = 2,
                    Explanation = "A /24 subnet has 256 total addresses, but 2 are reserved (network and broadcast), leaving 254 for hosts!",
                    IncorrectFeedback = { "Don't forget to subtract reserved addresses!", "Close! Remember network and broadcast are reserved", "We need one more - think about reserved addresses" }
                }
            }
        };

        return Newtonsoft.Json.JsonConvert.SerializeObject(content, Newtonsoft.Json.Formatting.Indented);
    }

    private static string CreateLesson7Content()
    {
        var content = new LessonContent
        {
            Slides = new List<Slide>
            {
                new Slide
                {
                    Type = SlideType.Introduction,
                    Title = "CIDR: The Smart Slicer",
                    Content = "🍕 CIDR notation is like having the perfect pizza slicer for network addresses! Instead of fixed slices, you can create custom portions that fit exactly what you need. No more waste!",
                    HoverTips = { "CIDR = Classless Inter-Domain Routing", "Replaced the old class-based system" }
                },
                new Slide
                {
                    Type = SlideType.Image,
                    Title = "Pizza Cutter Subnets",
                    Content = "Just like a pizza cutter creates perfect slices, CIDR notation lets you slice IP address space into exactly the sizes you need!",
                    ImageData = "cidr_pizza_cutter"
                },
                new Slide
                {
                    Type = SlideType.Text,
                    Title = "CIDR Calculation Wizardry",
                    Content = "🧮 CIDR Magic Formula:\n\n**Notation Examples:**\n• 192.168.1.0/24 = 256 addresses (1 subnet)\n• 192.168.1.0/25 = 128 addresses (2 subnets)\n• 192.168.1.0/26 = 64 addresses (4 subnets)\n• 192.168.1.0/27 = 32 addresses (8 subnets)\n\n**Quick Math:**\n• /24 → 2^(32-24) = 2^8 = 256 addresses\n• /27 → 2^(32-27) = 2^5 = 32 addresses\n\n**Pro Tip:** Higher CIDR number = smaller subnet!",
                    HoverTips = { "32 is total IPv4 bits", "Each increase halves the addresses", "Perfect for efficient IP planning" }
                }
            },
            Quiz = new List<QuizQuestion>
            {
                new QuizQuestion
                {
                    Question = "How many addresses are in a /26 subnet?",
                    Options = { "32", "64", "128", "256" },
                    CorrectAnswerIndex = 1,
                    Explanation = "A /26 subnet has 2^(32-26) = 2^6 = 64 addresses. Perfect for small departments!",
                    IncorrectFeedback = { "That's /27! /26 is larger", "That's /25! /26 is smaller", "That's /24! /26 is much smaller" }
                }
            }
        };

        return Newtonsoft.Json.JsonConvert.SerializeObject(content, Newtonsoft.Json.Formatting.Indented);
    }

    private static string CreateLesson8Content()
    {
        var content = new LessonContent
        {
            Slides = new List<Slide>
            {
                new Slide
                {
                    Type = SlideType.Introduction,
                    Title = "Private vs Public IPs: Inside/Outside Worlds",
                    Content = "🏠🌍 Every device lives in two worlds: the cozy private network at home and the vast public internet outside! Understanding this dual existence is key to network mastery.",
                    HoverTips = { "RFC 1918 defines private ranges", "NAT bridges private and public worlds" }
                },
                new Slide
                {
                    Type = SlideType.Image,
                    Title = "Home vs Street Addresses",
                    Content = "Private IPs are like apartment numbers (internal use only), while public IPs are like street addresses (visible to the whole world)!",
                    ImageData = "private_vs_public_addresses"
                },
                new Slide
                {
                    Type = SlideType.Text,
                    Title = "The Address Divide",
                    Content = "🔒 Private IP Ranges (RFC 1918):\n• **Class A:** 10.0.0.0 to 10.255.255.255 (/8)\n• **Class B:** 172.16.0.0 to 172.31.255.255 (/12)\n• **Class C:** 192.168.0.0 to 192.168.255.255 (/16)\n\n🌐 Public IPs:\n• Everything else!\n• Globally unique\n• Routable on internet\n• Assigned by ISPs/registrars\n\n**Security Bonus:** Private addresses are hidden from direct internet access - like having tinted windows!",
                    HoverTips = { "10.x.x.x is most common for large networks", "192.168.x.x perfect for home use", "Can't route private addresses on internet" }
                }
            },
            Quiz = new List<QuizQuestion>
            {
                new QuizQuestion
                {
                    Question = "Which of these is a private IP address?",
                    Options = { "8.8.8.8", "172.20.1.100", "208.67.222.222", "74.125.224.72" },
                    CorrectAnswerIndex = 1,
                    Explanation = "172.20.1.100 falls in the private range 172.16.0.0 to 172.31.255.255 - perfect for internal networks!",
                    IncorrectFeedback = { "That's Google's public DNS!", "That's OpenDNS - definitely public", "That's a Google public IP!" }
                }
            }
        };

        return Newtonsoft.Json.JsonConvert.SerializeObject(content, Newtonsoft.Json.Formatting.Indented);
    }

    private static string CreateLesson9Content()
    {
        var content = new LessonContent
        {
            Slides = new List<Slide>
            {
                new Slide
                {
                    Type = SlideType.Introduction,
                    Title = "NAT: The Address Translator",
                    Content = "🗣️ Meet NAT, the diplomatic interpreter of the networking world! This clever translator helps your private devices communicate with the public internet, like having a universal translator at a global conference.",
                    HoverTips = { "NAT = Network Address Translation", "Solves IPv4 address shortage" }
                },
                new Slide
                {
                    Type = SlideType.Image,
                    Title = "Diplomatic Interpreter",
                    Content = "NAT acts like a skilled interpreter, translating between private network language and public internet speech, ensuring perfect communication!",
                    ImageData = "nat_interpreter"
                },
                new Slide
                {
                    Type = SlideType.Text,
                    Title = "NAT Translation Magic",
                    Content = "✨ How NAT Works:\n\n**Outbound:** Private → Public\n• Device (192.168.1.5:1024) → Router\n• Router changes to (203.0.113.1:5000) → Internet\n\n**Inbound:** Public → Private  \n• Internet → Router (203.0.113.1:5000)\n• Router translates back to (192.168.1.5:1024)\n\n**Types of NAT:**\n• **Static NAT:** 1:1 mapping\n• **Dynamic NAT:** Pool of public IPs\n• **PAT (NAT Overload):** Many private → 1 public\n\n**Security Bonus:** Acts like a firewall - external devices can't directly reach internal ones!",
                    HoverTips = { "Port numbers help track connections", "Most home routers use PAT", "NAT table stores translations" }
                }
            },
            Quiz = new List<QuizQuestion>
            {
                new QuizQuestion
                {
                    Question = "What is the most common type of NAT used in home routers?",
                    Options = { "Static NAT", "Dynamic NAT", "PAT (NAT Overload)", "Inverse NAT" },
                    CorrectAnswerIndex = 2,
                    Explanation = "PAT (Port Address Translation) or NAT Overload allows many devices to share one public IP - perfect for home networks!",
                    IncorrectFeedback = { "Static NAT is 1:1 - too expensive for homes", "Dynamic NAT needs multiple public IPs", "That's not a real NAT type!" }
                }
            }
        };

        return Newtonsoft.Json.JsonConvert.SerializeObject(content, Newtonsoft.Json.Formatting.Indented);
    }

    private static string CreateLesson10Content()
    {
        var content = new LessonContent
        {
            Slides = new List<Slide>
            {
                new Slide
                {
                    Type = SlideType.Introduction,
                    Title = "ARP: The Address Resolver",
                    Content = "🕵️ Meet ARP, the brilliant detective of the networking world! This protocol solves the mystery of 'Who has this IP address?' by mapping IP addresses to MAC addresses like a digital sherlock!",
                    HoverTips = { "ARP = Address Resolution Protocol", "Works at Layer 2 and 3 boundary" }
                },
                new Slide
                {
                    Type = SlideType.Image,
                    Title = "Network Detective",
                    Content = "ARP is like a detective with a magnifying glass, investigating IP addresses and revealing their MAC address secrets on the local network!",
                    ImageData = "arp_detective"
                },
                new Slide
                {
                    Type = SlideType.Text,
                    Title = "The ARP Investigation Process",
                    Content = "🔍 How ARP Solves the Mystery:\n\n**Step 1: The Question**\n• \"Who has 192.168.1.100? Tell 192.168.1.50!\"\n• Broadcasts to entire local network\n\n**Step 2: The Answer**\n• Device with 192.168.1.100 responds\n• \"192.168.1.100 is at AA:BB:CC:DD:EE:FF\"\n\n**Step 3: The Memory**\n• Requesting device stores in ARP cache\n• No need to ask again for a while!\n\n**ARP Cache:** Temporary memory of IP→MAC mappings\n**Gratuitous ARP:** Announcing your own IP/MAC (like introducing yourself)",
                    HoverTips = { "Cache timeout usually 4 hours", "Only works on local network segment", "Gratuitous = voluntary announcement" }
                }
            },
            Quiz = new List<QuizQuestion>
            {
                new QuizQuestion
                {
                    Question = "What does ARP resolve IP addresses to?",
                    Options = { "Domain names", "Port numbers", "MAC addresses", "Subnet masks" },
                    CorrectAnswerIndex = 2,
                    Explanation = "ARP resolves IP addresses to MAC (hardware) addresses, enabling communication on the local network segment!",
                    IncorrectFeedback = { "That's DNS, not ARP!", "ARP doesn't deal with ports", "ARP doesn't handle subnet masks" }
                }
            }
        };

        return Newtonsoft.Json.JsonConvert.SerializeObject(content, Newtonsoft.Json.Formatting.Indented);
    }

    private static string CreateLesson11Content()
    {
        var content = new LessonContent
        {
            Slides = new List<Slide>
            {
                new Slide
                {
                    Type = SlideType.Introduction,
                    Title = "IP Classes: The Old Hierarchy",
                    Content = "🏫 Journey back to networking's school days! Before CIDR came along, IP addresses were organized into classes like students in different grades. Let's explore this vintage addressing system!",
                    HoverTips = { "Classes were used from 1981-1993", "Replaced by CIDR for efficiency" }
                },
                new Slide
                {
                    Type = SlideType.Image,
                    Title = "Network School Classrooms",
                    Content = "IP classes were like different classrooms in a school - Class A for the big kids, Class B for medium groups, and Class C for small classes!",
                    ImageData = "ip_classes_school"
                },
                new Slide
                {
                    Type = SlideType.Text,
                    Title = "The Classic Class System",
                    Content = "📚 The Three Main Classes:\n\n**Class A (1.0.0.0 - 126.255.255.255)**\n• First bit: 0\n• Networks: 128 (126 usable)\n• Hosts per network: 16,777,214\n• For huge organizations\n\n**Class B (128.0.0.0 - 191.255.255.255)**\n• First bits: 10\n• Networks: 16,384\n• Hosts per network: 65,534\n• For medium organizations\n\n**Class C (192.0.0.0 - 223.255.255.255)**\n• First bits: 110\n• Networks: 2,097,152\n• Hosts per network: 254\n• For small organizations\n\n**Problem:** Wasteful! Classes were too rigid for real needs.",
                    HoverTips = { "127.x.x.x reserved for loopback", "Class D for multicast", "Class E for experimental use" }
                }
            },
            Quiz = new List<QuizQuestion>
            {
                new QuizQuestion
                {
                    Question = "What class does the IP address 150.200.100.50 belong to?",
                    Options = { "Class A", "Class B", "Class C", "Class D" },
                    CorrectAnswerIndex = 1,
                    Explanation = "150.x.x.x falls in the range 128-191, making it a Class B address - perfect for medium-sized networks!",
                    IncorrectFeedback = { "Class A is 1-126", "Class C starts at 192", "Class D is for multicast (224-239)" }
                }
            }
        };

        return Newtonsoft.Json.JsonConvert.SerializeObject(content, Newtonsoft.Json.Formatting.Indented);
    }

    private static string CreateLesson12Content()
    {
        var content = new LessonContent
        {
            Slides = new List<Slide>
            {
                new Slide
                {
                    Type = SlideType.Introduction,
                    Title = "Loopback: Talking to Yourself",
                    Content = "🪞 Meet the localhost address - where your computer practices its networking skills in front of the mirror! Every device can talk to itself at the magical address 127.0.0.1.",
                    HoverTips = { "127.0.0.1 is 'home sweet home'", "Perfect for testing and development" }
                },
                new Slide
                {
                    Type = SlideType.Image,
                    Title = "Computer Mirror Practice",
                    Content = "The loopback address is like having a mirror for your computer - it can practice networking by talking to itself!",
                    ImageData = "loopback_mirror"
                },
                new Slide
                {
                    Type = SlideType.Text,
                    Title = "Loopback Magic",
                    Content = "🏠 The Loopback Range: 127.0.0.0/8\n\n**Common Uses:**\n• **127.0.0.1 (localhost):** Most popular\n• **::1:** IPv6 loopback equivalent\n\n**Perfect For:**\n• Testing web servers locally\n• Database connections in development\n• Network troubleshooting\n• Learning networking commands\n\n**Fun Facts:**\n• Never leaves your computer\n• Always responds (if networking works)\n• Fastest possible network connection\n• No physical hardware involved\n\n**PowerShell Test:** `ping 127.0.0.1` - if this fails, your network stack has serious problems!",
                    HoverTips = { "Also called 'lo' interface on Linux", "Loopback = loop back to yourself", "Ping localhost to test TCP/IP stack" }
                }
            },
            Quiz = new List<QuizQuestion>
            {
                new QuizQuestion
                {
                    Question = "What is the most common loopback IP address?",
                    Options = { "192.168.1.1", "127.0.0.1", "10.0.0.1", "255.255.255.255" },
                    CorrectAnswerIndex = 1,
                    Explanation = "127.0.0.1 is the classic localhost address - your computer's way of talking to itself!",
                    IncorrectFeedback = { "That's usually a router address", "That's usually a private network gateway", "That's the broadcast address" }
                }
            }
        };

        return Newtonsoft.Json.JsonConvert.SerializeObject(content, Newtonsoft.Json.Formatting.Indented);
    }

    private static string CreateLesson13Content()
    {
        var content = new LessonContent
        {
            Slides = new List<Slide>
            {
                new Slide
                {
                    Type = SlideType.Introduction,
                    Title = "Multicast: Group Chats",
                    Content = "👥 Welcome to multicast - the group messaging system of the networking world! Instead of sending individual messages, multicast lets you broadcast to specific groups, like having themed chat rooms!",
                    HoverTips = { "Multicast = one-to-many communication", "More efficient than multiple unicast" }
                },
                new Slide
                {
                    Type = SlideType.Image,
                    Title = "Network Group Call",
                    Content = "Multicast is like hosting a group video call where the host sends one stream, but multiple people can join and listen!",
                    ImageData = "multicast_group_call"
                },
                new Slide
                {
                    Type = SlideType.Text,
                    Title = "Multicast Address Magic",
                    Content = "🎯 Multicast Range: 224.0.0.0 to 239.255.255.255\n\n**Special Addresses:**\n• **224.0.0.1:** All Systems (everyone listening)\n• **224.0.0.2:** All Routers\n• **224.0.0.22:** IGMP (group membership)\n• **239.x.x.x:** Private multicast (like private networks)\n\n**How It Works:**\n1. Devices join multicast groups (IGMP)\n2. Sender transmits to group address\n3. Network delivers to all group members\n4. Efficient: One packet → Many recipients\n\n**Perfect For:** Video streaming, software updates, gaming, real-time data",
                    HoverTips = { "IGMP = Internet Group Management Protocol", "Routers track group memberships", "Saves bandwidth compared to many unicasts" }
                }
            },
            Quiz = new List<QuizQuestion>
            {
                new QuizQuestion
                {
                    Question = "What protocol manages multicast group membership?",
                    Options = { "ARP", "DHCP", "IGMP", "ICMP" },
                    CorrectAnswerIndex = 2,
                    Explanation = "IGMP (Internet Group Management Protocol) manages which devices belong to multicast groups - like a club membership manager!",
                    IncorrectFeedback = { "ARP resolves IP to MAC addresses", "DHCP assigns IP addresses", "ICMP is for ping and error messages" }
                }
            }
        };

        return Newtonsoft.Json.JsonConvert.SerializeObject(content, Newtonsoft.Json.Formatting.Indented);
    }

    private static string CreateLesson14Content()
    {
        var content = new LessonContent
        {
            Slides = new List<Slide>
            {
                new Slide
                {
                    Type = SlideType.Introduction,
                    Title = "Anycast: Closest Buddy",
                    Content = "🎯 Discover anycast - the intelligent addressing system that always connects you to your nearest network friend! Like GPS finding the closest pizza place, anycast routes you to the best available destination.",
                    HoverTips = { "Anycast = one-to-nearest communication", "Critical for CDNs and DNS" }
                },
                new Slide
                {
                    Type = SlideType.Image,
                    Title = "Nearest Friend Network",
                    Content = "Anycast is like having the same phone number for multiple pizza shops - when you call, you automatically get connected to the closest one!",
                    ImageData = "anycast_nearest_friend"
                },
                new Slide
                {
                    Type = SlideType.Text,
                    Title = "Anycast Routing Intelligence",
                    Content = "🧠 How Anycast Works:\n\n**The Magic:**\n• Multiple servers share same IP address\n• Routing protocols find \"closest\" server\n• Client connects automatically to best option\n• Transparent to end users\n\n**Real-World Examples:**\n• **DNS Root Servers:** 13 logical, 1000+ physical\n• **CDNs:** Netflix, YouTube content servers\n• **Cloud Services:** AWS, Google Cloud endpoints\n\n**Benefits:**\n• Faster response times\n• Automatic failover\n• Load distribution\n• DDoS attack mitigation\n\n**Routing Magic:** BGP (Border Gateway Protocol) makes the distance decisions",
                    HoverTips = { "Closest = lowest routing cost", "BGP considers many factors for best path", "Invisible to applications" }
                }
            },
            Quiz = new List<QuizQuestion>
            {
                new QuizQuestion
                {
                    Question = "What's the main benefit of anycast addressing?",
                    Options = { "Saves IP addresses", "Connects to nearest server", "Provides encryption", "Manages group communication" },
                    CorrectAnswerIndex = 1,
                    Explanation = "Anycast's superpower is connecting clients to the nearest available server, providing faster service and better reliability!",
                    IncorrectFeedback = { "That's more of a NAT benefit", "That's what encryption protocols do", "That's multicast, not anycast" }
                }
            }
        };

        return Newtonsoft.Json.JsonConvert.SerializeObject(content, Newtonsoft.Json.Formatting.Indented);
    }

    private static string CreateLesson15Content()
    {
        var content = new LessonContent
        {
            Slides = new List<Slide>
            {
                new Slide
                {
                    Type = SlideType.Introduction,
                    Title = "IP Security: Guarding Addresses",
                    Content = "🛡️ Time to fortify your IP addresses! Just like protecting your home address from unwanted visitors, IP security ensures your network communications stay private and secure from digital intruders.",
                    HoverTips = { "IPsec provides end-to-end security", "Links to Module 6: Security Deep-Dive" }
                },
                new Slide
                {
                    Type = SlideType.Image,
                    Title = "Digital Fortress",
                    Content = "IP security is like surrounding your network communications with an impenetrable digital fortress, complete with encrypted envelopes for all your data!",
                    ImageData = "ip_security_fortress"
                },
                new Slide
                {
                    Type = SlideType.Text,
                    Title = "IP Security Arsenal",
                    Content = "⚔️ IP Security Weapons:\n\n**IPsec (IP Security Protocol):**\n• Authentication Header (AH): Verify sender identity\n• Encapsulating Security Payload (ESP): Encrypt data\n• Transport Mode: Protect payload only\n• Tunnel Mode: Protect entire packet\n\n**VPNs (Virtual Private Networks):**\n• Create secure tunnels over public networks\n• Site-to-site connections\n• Remote access for workers\n\n**Security Considerations:**\n• IP spoofing attacks\n• Man-in-the-middle threats\n• Packet sniffing prevention\n\n**Coming Up:** Module 6 will dive deeper into security scanning and protection!",
                    HoverTips = { "AH provides integrity, not encryption", "ESP provides both integrity and encryption", "Tunnel mode often used for VPNs" }
                }
            },
            Quiz = new List<QuizQuestion>
            {
                new QuizQuestion
                {
                    Question = "Which IPsec component provides data encryption?",
                    Options = { "Authentication Header (AH)", "Encapsulating Security Payload (ESP)", "Internet Key Exchange (IKE)", "Security Association (SA)" },
                    CorrectAnswerIndex = 1,
                    Explanation = "ESP (Encapsulating Security Payload) provides encryption for your data, keeping it secret from prying eyes!",
                    IncorrectFeedback = { "AH only authenticates, doesn't encrypt", "IKE manages keys, not encryption", "SA defines security parameters" }
                }
            }
        };

        return Newtonsoft.Json.JsonConvert.SerializeObject(content, Newtonsoft.Json.Formatting.Indented);
    }

    private static string CreateLesson16Content()
    {
        var content = new LessonContent
        {
            Slides = new List<Slide>
            {
                new Slide
                {
                    Type = SlideType.Introduction,
                    Title = "Scripting IPs: Command Shenanigans",
                    Content = "🪄 Ready to become an IP wizard? PowerShell gives you magical powers to configure, manage, and troubleshoot IP addresses with elegant commands. Time to wave your scripting wand!",
                    HoverTips = { "PowerShell cmdlets are verb-noun format", "Connect to Module 2: PowerShell Terminal" }
                },
                new Slide
                {
                    Type = SlideType.Image,
                    Title = "Wizard Wand on Address",
                    Content = "PowerShell commands work like a magic wand - with the right incantations, you can transform IP configurations instantly!",
                    ImageData = "powershell_ip_wand"
                },
                new Slide
                {
                    Type = SlideType.Text,
                    Title = "PowerShell IP Spellbook",
                    Content = "✨ Essential IP Magic Commands:\n\n**View Current Configuration:**\n```powershell\nGet-NetIPAddress\nGet-NetIPConfiguration\nGet-NetAdapter\n```\n\n**Set Static IP:**\n```powershell\nNew-NetIPAddress -InterfaceAlias \"Ethernet\" `\n  -IPAddress 192.168.1.100 `\n  -PrefixLength 24 `\n  -DefaultGateway 192.168.1.1\n```\n\n**Set DNS Servers:**\n```powershell\nSet-DnsClientServerAddress -InterfaceAlias \"Ethernet\" `\n  -ServerAddresses 8.8.8.8,8.8.4.4\n```\n\n**Quick DHCP Reset:**\n```powershell\nRestart-NetAdapter -Name \"Ethernet\"\n```",
                    HoverTips = { "Backtick (`) continues lines in PowerShell", "InterfaceAlias is the adapter name", "Always test commands safely first!" }
                }
            },
            Quiz = new List<QuizQuestion>
            {
                new QuizQuestion
                {
                    Question = "Which PowerShell command sets a static IP address?",
                    Options = { "Set-NetIPAddress", "New-NetIPAddress", "Add-NetIPAddress", "Configure-NetIPAddress" },
                    CorrectAnswerIndex = 1,
                    Explanation = "New-NetIPAddress creates a new static IP configuration - like conjuring a new address with your PowerShell wand!",
                    IncorrectFeedback = { "Set-NetIPAddress modifies existing ones", "That's not a real PowerShell cmdlet", "That's not a real PowerShell cmdlet" }
                }
            }
        };

        return Newtonsoft.Json.JsonConvert.SerializeObject(content, Newtonsoft.Json.Formatting.Indented);
    }

    private static string CreateLesson17Content()
    {
        var content = new LessonContent
        {
            Slides = new List<Slide>
            {
                new Slide
                {
                    Type = SlideType.Introduction,
                    Title = "Troubleshooting IPs: Fix the Mix-Ups",
                    Content = "🔧 Welcome to IP detective school! When addresses misbehave and connections go wonky, you need the right tools and techniques to solve network mysteries. Let's become IP troubleshooting heroes!",
                    HoverTips = { "Most IP issues are configuration problems", "Always start with basics - ping localhost!" }
                },
                new Slide
                {
                    Type = SlideType.Image,
                    Title = "Network Detective Puzzle",
                    Content = "IP troubleshooting is like solving a complex puzzle - each clue leads you closer to the solution!",
                    ImageData = "ip_troubleshooting_puzzle"
                },
                new Slide
                {
                    Type = SlideType.Text,
                    Title = "IP Detective Toolkit",
                    Content = "🕵️ Essential Troubleshooting Commands:\n\n**Basic Connectivity:**\n```cmd\nping 127.0.0.1          # Test local TCP/IP\nping 192.168.1.1        # Test gateway\nping 8.8.8.8            # Test internet\nipconfig /all           # View all settings\n```\n\n**Advanced Diagnostics:**\n```cmd\narp -a                  # View ARP cache\nnslookup google.com     # Test DNS\ntracert 8.8.8.8        # Trace route path\nnetstat -an             # Show connections\n```\n\n**Common Issues:**\n• **Duplicate IPs:** APIPA addresses (169.254.x.x)\n• **Wrong Gateway:** Can't reach internet\n• **DNS Problems:** Can ping IPs but not names\n• **DHCP Failures:** Static conflicts or server down",
                    HoverTips = { "APIPA = Automatic Private IP Addressing", "169.254.x.x means DHCP failed", "Always test in logical order" }
                }
            },
            Quiz = new List<QuizQuestion>
            {
                new QuizQuestion
                {
                    Question = "What does an IP address starting with 169.254 indicate?",
                    Options = { "DHCP is working properly", "Static IP configuration", "APIPA - DHCP failed", "Loopback address" },
                    CorrectAnswerIndex = 2,
                    Explanation = "169.254.x.x addresses indicate APIPA (Automatic Private IP Addressing) - your device couldn't get DHCP and assigned itself!",
                    IncorrectFeedback = { "If DHCP worked, you'd have a normal address", "Static IPs are manually configured", "Loopback is 127.x.x.x" }
                }
            }
        };

        return Newtonsoft.Json.JsonConvert.SerializeObject(content, Newtonsoft.Json.Formatting.Indented);
    }

    private static string CreateLesson18Content()
    {
        var content = new LessonContent
        {
            Slides = new List<Slide>
            {
                new Slide
                {
                    Type = SlideType.Introduction,
                    Title = "IPv6 Adoption: Future-Proofing",
                    Content = "🚀 Welcome to the future of addressing! IPv6 isn't just bigger than IPv4 - it's a complete reimagining of how devices connect. With enough addresses for every grain of sand on Earth, IPv6 is ready for our connected cosmos!",
                    HoverTips = { "IPv6 eliminates NAT necessity", "Built-in security features" }
                },
                new Slide
                {
                    Type = SlideType.Image,
                    Title = "Futuristic Connected City",
                    Content = "IPv6 enables a truly connected future where every device, from smart refrigerators to space satellites, can have its own unique address!",
                    ImageData = "ipv6_future_city"
                },
                new Slide
                {
                    Type = SlideType.Text,
                    Title = "IPv6 Migration Strategies",
                    Content = "🌉 Bridging IPv4 to IPv6:\n\n**Dual Stack:**\n• Run IPv4 and IPv6 simultaneously\n• Gradual transition approach\n• Applications choose best protocol\n\n**Tunneling:**\n• IPv6 packets through IPv4 networks\n• 6to4, Teredo, ISATAP tunnels\n• Temporary solution\n\n**Translation:**\n• NAT64/DNS64 for IPv4↔IPv6 communication\n• Stateless and stateful options\n\n**IPv6 Benefits:**\n• No NAT required (end-to-end connectivity)\n• Built-in IPsec support\n• Better mobile device support\n• Simplified network administration\n• Ready for IoT explosion",
                    HoverTips = { "Most modern devices support dual stack", "6to4 uses IPv4 infrastructure", "IoT = Internet of Things" }
                }
            },
            Quiz = new List<QuizQuestion>
            {
                new QuizQuestion
                {
                    Question = "What is dual stack in IPv6 migration?",
                    Options = { "Two IPv6 addresses per device", "Running IPv4 and IPv6 together", "Two different subnet masks", "Backup IPv6 configuration" },
                    CorrectAnswerIndex = 1,
                    Explanation = "Dual stack means running both IPv4 and IPv6 protocols simultaneously, allowing gradual migration to the future!",
                    IncorrectFeedback = { "That would just be multiple IPv6 addresses", "That's not related to IPv6 migration", "That's not what dual stack means" }
                }
            }
        };

        return Newtonsoft.Json.JsonConvert.SerializeObject(content, Newtonsoft.Json.Formatting.Indented);
    }

    private static string CreateLesson19Content()
    {
        var content = new LessonContent
        {
            Slides = new List<Slide>
            {
                new Slide
                {
                    Type = SlideType.Introduction,
                    Title = "Cert-Level: Subnet Calculations",
                    Content = "🧮 Ready for the ultimate subnet challenge? This is where networking professionals separate from beginners! Master the binary wizardry and mathematical magic of advanced subnetting. Your certification awaits!",
                    HoverTips = { "Binary math is key to subnetting", "VLSM saves address space" }
                },
                new Slide
                {
                    Type = SlideType.Image,
                    Title = "Binary Calculator Magic",
                    Content = "Advanced subnet calculations require thinking in binary - where every bit counts and mathematical precision creates perfect network designs!",
                    ImageData = "subnet_binary_calculator"
                },
                new Slide
                {
                    Type = SlideType.Text,
                    Title = "VLSM Mastery Challenge",
                    Content = "🎯 **VLSM Scenario:** Design subnets for company with:\n• Sales: 50 hosts\n• Engineering: 25 hosts  \n• HR: 10 hosts\n• Management: 5 hosts\n• Point-to-point links: 2 hosts each (3 links)\n\n**Solution Strategy:**\n1. **Order by size:** Sales(50) > Engineering(25) > HR(10) > Management(5) > Links(2)\n2. **Calculate requirements:** Add 2 for network/broadcast\n3. **Binary magic:** Next power of 2\n   - Sales: 52 hosts → 64 addresses (/26)\n   - Engineering: 27 hosts → 32 addresses (/27)\n   - HR: 12 hosts → 16 addresses (/28)\n   - Management: 7 hosts → 8 addresses (/29)\n   - Links: 4 hosts → 4 addresses each (/30)\n\n**Memory Trick:** /30 = 4 addresses = perfect for point-to-point!",
                    HoverTips = { "Always start with largest subnets first", "Don't forget network and broadcast", "VLSM = Variable Length Subnet Masking" }
                }
            },
            Quiz = new List<QuizQuestion>
            {
                new QuizQuestion
                {
                    Question = "For a subnet needing 30 hosts, what's the minimum subnet mask?",
                    Options = { "/26 (64 addresses)", "/27 (32 addresses)", "/28 (16 addresses)", "/29 (8 addresses)" },
                    CorrectAnswerIndex = 0,
                    Explanation = "30 hosts + 2 reserved = 32 needed. Next power of 2 is 64, requiring /26. Remember: always round UP to next power of 2!",
                    IncorrectFeedback = { "Too small! 32-2 = 30 hosts max", "Too small! Only 16-2 = 14 hosts", "Too small! Only 8-2 = 6 hosts" }
                }
            }
        };

        return Newtonsoft.Json.JsonConvert.SerializeObject(content, Newtonsoft.Json.Formatting.Indented);
    }

    private static string CreateLesson20Content()
    {
        var content = new LessonContent
        {
            Slides = new List<Slide>
            {
                new Slide
                {
                    Type = SlideType.Introduction,
                    Title = "Quiz Shenanigans: IP Mastery",
                    Content = "🏆 The ultimate IP address championship! You've journeyed through digital doorplates, subnet wizardry, and protocol magic. Now prove your mastery in this epic finale of networking knowledge!",
                    HoverTips = { "This quiz covers everything from Module 3", "Certification-level questions ahead!" }
                },
                new Slide
                {
                    Type = SlideType.Image,
                    Title = "IP Mastery Medal",
                    Content = "The IP Mastery Medal awaits those brave enough to conquer the ultimate addressing challenges! Will you claim your place among the subnet sorcerers?",
                    ImageData = "ip_mastery_medal"
                },
                new Slide
                {
                    Type = SlideType.Text,
                    Title = "Your IP Journey Recap",
                    Content = "🌟 **Congratulations! You've Mastered:**\n\n✅ **IP Fundamentals:** IPv4, IPv6, addressing basics\n✅ **Network Segmentation:** Subnets, CIDR, VLSM\n✅ **Protocol Magic:** DHCP, NAT, ARP operations\n✅ **Address Types:** Static, dynamic, private, public\n✅ **Advanced Concepts:** Multicast, anycast, security\n✅ **Practical Skills:** PowerShell commands, troubleshooting\n✅ **Future Planning:** IPv6 migration strategies\n\n**Certification Readiness:** You're now prepared for professional networking certifications!\n\n**Next Adventures:** Modules 4-10 await with even more networking wonders!\n\n**Remember:** Every IP address tells a story - you're now fluent in the language!",
                    HoverTips = { "Your knowledge spans from beginner to expert", "Ready for CCNA, Network+, and more!", "The networking cosmos awaits your expertise" }
                }
            },
            Quiz = new List<QuizQuestion>
            {
                new QuizQuestion
                {
                    Question = "A company needs to divide 192.168.1.0/24 into 4 equal subnets. What subnet mask should be used?",
                    Options = { "/24 (255.255.255.0)", "/25 (255.255.255.128)", "/26 (255.255.255.192)", "/27 (255.255.255.224)" },
                    CorrectAnswerIndex = 2,
                    Explanation = "To divide /24 into 4 subnets, borrow 2 bits: 24+2 = /26. Each subnet gets 64 addresses (62 hosts). Perfect subnet sorcery!",
                    IncorrectFeedback = { "That's the original - no division!", "That creates 2 subnets, not 4", "That creates 8 subnets - too many!" }
                },
                new QuizQuestion
                {
                    Question = "Which command shows all current IP configurations in PowerShell?",
                    Options = { "Show-NetConfiguration", "Get-NetIPConfiguration", "Display-IPConfig", "List-NetworkSettings" },
                    CorrectAnswerIndex = 1,
                    Explanation = "Get-NetIPConfiguration is the PowerShell wizard's spell for viewing complete IP settings. Pure command magic!",
                    IncorrectFeedback = { "Not a real PowerShell cmdlet", "Not a real PowerShell cmdlet", "Not a real PowerShell cmdlet" }
                },
                new QuizQuestion
                {
                    Question = "What does APIPA address 169.254.100.50 indicate?",
                    Options = { "DHCP is working normally", "Static IP was configured", "DHCP failed, auto-assigned IP", "IPv6 is being used" },
                    CorrectAnswerIndex = 2,
                    Explanation = "169.254.x.x is APIPA - your device's 'help, I can't find DHCP!' automatic address. Time for network detective work!",
                    IncorrectFeedback = { "DHCP failure is exactly what this indicates", "Static IPs are manually configured", "This is IPv4, and indicates a problem" }
                }
            }
        };

        return Newtonsoft.Json.JsonConvert.SerializeObject(content, Newtonsoft.Json.Formatting.Indented);
    }

    #endregion
}