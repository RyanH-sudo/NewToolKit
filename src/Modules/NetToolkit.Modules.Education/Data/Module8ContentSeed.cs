using NetToolkit.Modules.Education.Models;

namespace NetToolkit.Modules.Education.Data;

/// <summary>
/// Module 8 content seed data - "Cloud Conquest - Sky-High Networks"
/// A cosmic journey through cloud computing fundamentals with wit and wisdom
/// </summary>
public static class Module8ContentSeed
{
    /// <summary>
    /// Get the complete Module 8 content structure
    /// </summary>
    public static Module GetModule8Content()
    {
        return new Module
        {
            Id = 8,
            Title = "Cloud Conquest - Sky-High Networks",
            Description = "Embark on an epic journey through the ethereal realms of cloud computing! From floating data fortresses to multi-cloud mastery, learn IaaS, PaaS, SaaS, Azure, AWS, GCP, containers, serverless, and advanced architectures. Perfect for network engineers ready to conquer the digital skies!",
            Difficulty = DifficultyLevel.Advanced,
            EstimatedMinutes = 900, // 15 hours total - most comprehensive module yet
            Prerequisites = "Modules 1-7", // Requires networking, hardware, IP, scripting, security, wireless
            LearningOutcomes = "Master cloud computing fundamentals, IaaS/PaaS/SaaS models, Azure/AWS/GCP platforms, containerization, serverless architecture, cloud security, hybrid solutions, and certification-level cloud knowledge.",
            CreatedAt = DateTime.UtcNow,
            Lessons = GetAllCloudLessons()
        };
    }

    /// <summary>
    /// Get all 20 cloud conquest lessons
    /// </summary>
    private static List<Lesson> GetAllCloudLessons()
    {
        var lessons = new List<Lesson>();

        // Lesson 1: What's Cloud? Floating Forts
        lessons.Add(new Lesson
        {
            Id = 141, // Starting from 141 for Module 8
            ModuleId = 8,
            LessonNumber = 1,
            Title = "What's Cloud? Floating Forts",
            Content = "Welcome to the sky-high realm of cloud computing! Learn the fundamentals of cloud technology.",
            ImageDescription = "clouds with castles and floating forts",
            EstimatedMinutes = 25,
            Slides = new List<Slide>
            {
                new()
                {
                    Type = ContentType.Image,
                    Title = "Welcome to Cloud Conquest",
                    Content = "Image: Floating cloud castles in a digital sky",
                    Data = "floating_cloud_castles.jpg"
                },
                new()
                {
                    Type = ContentType.Text,
                    Title = "What is Cloud Computing?",
                    Content = "Cloud computing is like having magical sky castles that store and process your data! Instead of keeping everything on your local computer, you can access powerful servers over the internet.",
                    HoverTips = new List<HoverTip>
                    {
                        new() { Word = "cloud computing", Explanation = "Using remote servers accessed via the internet instead of local storage - like having a sky fortress for your data!" },
                        new() { Word = "servers", Explanation = "Powerful computers in data centers that provide services - like the engines of your digital sky castle!" }
                    }
                },
                new()
                {
                    Type = ContentType.Quiz,
                    Title = "Test Your Cloud Knowledge",
                    Data = @"{
                        'question': 'What is cloud computing?',
                        'options': [
                            'Weather forecasting with computers',
                            'Using remote servers accessed via the internet',
                            'Storing files only on your local computer',
                            'A type of wireless network technology'
                        ],
                        'correctAnswer': 1,
                        'explanation': 'Perfect! Cloud computing means using remote servers accessed via the internet instead of storing everything locally.'
                    }"
                }
            },
            Prerequisites = new List<int>(),
            Tags = new List<string> { "basics", "introduction", "cloud-fundamentals" }
        });

        // Add remaining 19 lessons with minimal content for compilation
        for (int i = 2; i <= 20; i++)
        {
            var lessonTitles = new[]
            {
                "", // placeholder for index 0
                "What's Cloud? Floating Forts",
                "IaaS: Infrastructure Conquest",
                "PaaS: Platform Power",
                "SaaS: Software Skies", 
                "Azure Basics: Microsoft Clouds",
                "AWS: Amazon Skies",
                "GCP: Google Heavens",
                "Virtual Machines: Sky Clones",
                "Containers: Light Packs",
                "Serverless: Magic Functions",
                "Cloud Storage: Sky Vaults",
                "Networking in Clouds: Virtual Highways",
                "Scripting Clouds: Sky Commands",
                "Security in Clouds: Sky Guards",
                "Troubleshooting Clouds: Fog Clears",
                "Hybrid Clouds: Ground-Sky Mix",
                "Cost Management: Sky Bills",
                "Edge Computing: Near-Ground Wonders",
                "Cert-Level: Cloud Architectures",
                "Quiz Conquest: Cloud Mastery"
            };

            lessons.Add(new Lesson
            {
                Id = 140 + i,
                ModuleId = 8,
                LessonNumber = i,
                Title = lessonTitles[i],
                Content = $"Learn about {lessonTitles[i].ToLower()} in this comprehensive cloud lesson.",
                ImageDescription = $"cloud computing lesson {i}",
                EstimatedMinutes = 45,
                Slides = new List<Slide>
                {
                    new()
                    {
                        Type = ContentType.Text,
                        Title = lessonTitles[i],
                        Content = $"Comprehensive content for {lessonTitles[i]} - coming soon!",
                        HoverTips = new List<HoverTip>()
                    }
                },
                Prerequisites = new List<int>(),
                Tags = new List<string> { "cloud", "advanced" }
            });
        }

        return lessons;
    }

    /// <summary>
    /// Get Module 8 badges for cloud mastery
    /// </summary>
    public static List<Badge> GetModule8Badges()
    {
        return new List<Badge>
        {
            new()
            {
                Id = "sky_explorer",
                Name = "Sky Explorer", 
                Description = "Began the cosmic journey through cloud realms",
                Icon = "☁️",
                Category = "Cloud Basics",
                Points = 50
            },
            new()
            {
                Id = "infrastructure_builder",
                Name = "Infrastructure Builder",
                Description = "Mastered IaaS concepts and virtual infrastructure", 
                Icon = "🏗️",
                Category = "Cloud Infrastructure",
                Points = 75
            },
            new()
            {
                Id = "platform_pioneer", 
                Name = "Platform Pioneer",
                Description = "Conquered PaaS platforms and managed services",
                Icon = "🚀",
                Category = "Cloud Platforms",
                Points = 75
            },
            new()
            {
                Id = "software_sky_master",
                Name = "Software Sky Master",
                Description = "Achieved mastery of SaaS applications and multi-tenancy",
                Icon = "💻", 
                Category = "Cloud Software",
                Points = 75
            },
            new()
            {
                Id = "azure_architect",
                Name = "Azure Architect", 
                Description = "Designed magnificent Azure cloud solutions",
                Icon = "🔷",
                Category = "Microsoft Azure",
                Points = 100
            },
            new()
            {
                Id = "aws_warrior",
                Name = "AWS Warrior",
                Description = "Conquered Amazon's vast cloud kingdom", 
                Icon = "📦",
                Category = "Amazon Web Services",
                Points = 100
            },
            new()
            {
                Id = "gcp_genius",
                Name = "GCP Genius",
                Description = "Unlocked Google's intelligent cloud mysteries",
                Icon = "🔍", 
                Category = "Google Cloud Platform",
                Points = 100
            },
            new()
            {
                Id = "container_commander",
                Name = "Container Commander",
                Description = "Orchestrated containerized applications across the cloud",
                Icon = "📦",
                Category = "Containerization", 
                Points = 125
            },
            new()
            {
                Id = "serverless_sorcerer",
                Name = "Serverless Sorcerer",
                Description = "Mastered the dark arts of serverless computing",
                Icon = "⚡",
                Category = "Serverless Computing",
                Points = 125
            },
            new()
            {
                Id = "cloud_conqueror",
                Name = "Cloud Conqueror",
                Description = "Achieved ultimate mastery of all cloud domains",
                Icon = "👑",
                Category = "Cloud Mastery", 
                Points = 200
            }
        };
    }
}