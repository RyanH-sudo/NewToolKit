using NetToolkit.Modules.Education.Models;
using Newtonsoft.Json;

namespace NetToolkit.Modules.Education.Data;

/// <summary>
/// Simplified Module 6 content seed matching actual model structure
/// </summary>
public static class Module6ContentSeed
{
    public static List<Lesson> GetModule6Lessons()
    {
        var lessons = new List<Lesson>();
        
        lessons.Add(new Lesson
        {
            Id = 104,
            ModuleId = 6,
            LessonNumber = 1,
            Title = "Firewall Fundamentals",
            Description = "Learn about digital firewalls and network security.",
            EstimatedMinutes = 25,
            Objectives = "Understand firewall concepts and network protection",
            ContentJson = JsonConvert.SerializeObject(new LessonContent
            {
                Slides = new List<Slide>
                {
                    new()
                    {
                        Title = "Firewall Basics",
                        Content = "Firewalls protect your network by filtering traffic.",
                        Type = SlideType.Text
                    }
                }
            })
        });

        return lessons;
    }

    /// <summary>
    /// Get complete Module 6 content with lessons (INFERRED: combines module metadata with lessons)
    /// </summary>
    public static Module GetModule6Content()
    {
        return new Module
        {
            Id = 6,
            Title = "Advanced Routing & Switching",
            Description = "Master advanced routing protocols and switching technologies",
            Category = "Networking",
            Difficulty = DifficultyLevel.Advanced,
            EstimatedMinutes = 720, // 12 hours in minutes
            Prerequisites = "Complete Module 5: Network Security Foundations",
            LearningOutcomes = "Students will configure complex routing protocols, implement advanced switching features, and optimize network performance",
            LearningObjectives = new List<string>
            {
                "Configure dynamic routing protocols (OSPF, BGP)",
                "Implement advanced switching technologies (VLANs, STP)",
                "Optimize network performance and redundancy"
            },
            Tags = new List<string> { "routing", "switching", "ospf", "bgp", "advanced" },
            Lessons = GetModule6Lessons(),
            CreatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Get Module 6 badges for gamification (INFERRED: achievement badges for module completion)
    /// </summary>
    public static List<Badge> GetModule6Badges()
    {
        return new List<Badge>
        {
            new Badge
            {
                BadgeId = "routing_master",
                Name = "Routing Master",
                Description = "Successfully completed Advanced Routing & Switching module",
                Category = BadgeCategory.Progress,
                Rarity = BadgeRarity.Rare,
                Requirements = "Complete all lessons in Module 6",
                RewardMessage = "🌐 Your routing mastery knows no bounds!",
                CreatedAt = DateTime.UtcNow
            },
            new Badge
            {
                BadgeId = "network_architect",
                Name = "Network Architect",
                Description = "Designed and implemented complex network topologies",
                Category = BadgeCategory.Special,
                Rarity = BadgeRarity.Legendary,
                Requirements = "Score 95%+ on advanced network design scenarios",
                RewardMessage = "🏛️ Your architectural genius is legendary!",
                CreatedAt = DateTime.UtcNow
            }
        };
    }
}