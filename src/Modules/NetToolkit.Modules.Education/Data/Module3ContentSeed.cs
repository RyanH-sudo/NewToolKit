using NetToolkit.Modules.Education.Models;
using Newtonsoft.Json;

namespace NetToolkit.Modules.Education.Data;

/// <summary>
/// Simplified Module 3 content seed matching actual model structure
/// </summary>
public static class Module3ContentSeed
{
    public static List<Lesson> GetModule3Lessons()
    {
        var lessons = new List<Lesson>();
        
        lessons.Add(new Lesson
        {
            Id = 41,
            ModuleId = 3,
            LessonNumber = 1,
            Title = "IP Shenanigans - Address Basics",
            Description = "Learn about IP addresses - the digital doorplates of the internet.",
            EstimatedMinutes = 40,
            Objectives = "Understand IP addressing concepts and basic subnet principles",
            ContentJson = JsonConvert.SerializeObject(new LessonContent
            {
                Slides = new List<Slide>
                {
                    new()
                    {
                        Title = "IP Addresses Explained",
                        Content = "IP addresses are unique identifiers that help data find its destination.",
                        Type = SlideType.Text
                    }
                }
            })
        });

        lessons.Add(new Lesson
        {
            Id = 42,
            ModuleId = 3,
            LessonNumber = 2,
            Title = "Subnets and Network Division",
            Description = "Explore how networks are divided into subnets for organization.",
            EstimatedMinutes = 35,
            Objectives = "Learn subnet concepts and basic network organization",
            ContentJson = JsonConvert.SerializeObject(new LessonContent
            {
                Slides = new List<Slide>
                {
                    new()
                    {
                        Title = "Subnet Fundamentals",
                        Content = "Subnets divide large networks into smaller, manageable segments.",
                        Type = SlideType.Text
                    }
                }
            })
        });

        return lessons;
    }

    /// <summary>
    /// Get complete Module 3 content with lessons (INFERRED: combines module metadata with lessons)
    /// </summary>
    public static Module GetModule3Content()
    {
        return new Module
        {
            Id = 3,
            Title = "Protocol Fundamentals",
            Description = "Master the essential networking protocols and communication standards",
            Category = "Networking",
            Difficulty = DifficultyLevel.Intermediate,
            EstimatedMinutes = 480, // 8 hours in minutes
            Prerequisites = "Complete Module 2: Network Hardware & Devices",
            LearningOutcomes = "Students will understand core networking protocols, implement protocol configurations, and analyze network communication patterns",
            LearningObjectives = new List<string>
            {
                "Understand TCP/IP protocol suite architecture",
                "Configure and troubleshoot common protocols",
                "Analyze network packet flows and communication patterns"
            },
            Tags = new List<string> { "networking", "protocols", "tcp-ip", "intermediate" },
            Lessons = GetModule3Lessons(),
            CreatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Get Module 3 badges for gamification (INFERRED: achievement badges for module completion)
    /// </summary>
    public static List<Badge> GetModule3Badges()
    {
        return new List<Badge>
        {
            new Badge
            {
                BadgeId = "protocol_pioneer",
                Name = "Protocol Pioneer",
                Description = "Successfully completed Protocol Fundamentals module",
                Category = BadgeCategory.Progress,
                Rarity = BadgeRarity.Common,
                Requirements = "Complete all lessons in Module 3",
                RewardMessage = "🌐 You've mastered the language of networks!",
                CreatedAt = DateTime.UtcNow
            },
            new Badge
            {
                BadgeId = "packet_analyst",
                Name = "Packet Analyst",
                Description = "Demonstrated advanced protocol analysis skills",
                Category = BadgeCategory.Mastery,
                Rarity = BadgeRarity.Rare,
                Requirements = "Score 90%+ on protocol analysis scenarios",
                RewardMessage = "📡 Your protocol expertise is extraordinary!",
                CreatedAt = DateTime.UtcNow
            }
        };
    }
}