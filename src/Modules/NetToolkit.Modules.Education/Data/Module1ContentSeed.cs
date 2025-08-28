using NetToolkit.Modules.Education.Models;
using Newtonsoft.Json;

namespace NetToolkit.Modules.Education.Data;

/// <summary>
/// Simplified Module 1 content seed matching actual model structure
/// </summary>
public static class Module1ContentSeed
{
    public static List<Lesson> GetModule1Lessons()
    {
        var lessons = new List<Lesson>();
        
        lessons.Add(new Lesson
        {
            Id = 1,
            ModuleId = 1,
            LessonNumber = 1,
            Title = "Network Basics - From Cables to Cosmos",
            Description = "Discover the fundamentals of networking and how devices communicate.",
            EstimatedMinutes = 30,
            Objectives = "Understand basic networking concepts and device communication",
            ContentJson = JsonConvert.SerializeObject(new LessonContent
            {
                Slides = new List<Slide>
                {
                    new()
                    {
                        Title = "Welcome to Networking",
                        Content = "Learn how computers connect and communicate across networks.",
                        Type = SlideType.Text
                    }
                }
            })
        });

        lessons.Add(new Lesson
        {
            Id = 2,
            ModuleId = 1,
            LessonNumber = 2,
            Title = "Cables and Physical Connections",
            Description = "Explore the physical layer of networking with cables and connectors.",
            EstimatedMinutes = 25,
            Objectives = "Identify common network cables and understand physical connectivity",
            ContentJson = JsonConvert.SerializeObject(new LessonContent
            {
                Slides = new List<Slide>
                {
                    new()
                    {
                        Title = "Network Cables",
                        Content = "Ethernet cables carry data between devices using copper wires.",
                        Type = SlideType.Text
                    }
                }
            })
        });

        return lessons;
    }

    /// <summary>
    /// Get complete Module 1 content with lessons (INFERRED: combines module metadata with lessons)
    /// </summary>
    public static Module GetModule1Content()
    {
        return new Module
        {
            Id = 1,
            Title = "Network Fundamentals",
            Description = "Master the basics of computer networking and communication protocols",
            Category = "Networking",
            Difficulty = DifficultyLevel.Beginner,
            EstimatedMinutes = 480, // 8 hours in minutes
            Prerequisites = "None - This is a beginner-friendly module",
            LearningOutcomes = "Students will understand network topology concepts, learn about cables and physical connections, and grasp basic communication protocols",
            LearningObjectives = new List<string>
            {
                "Understand network topology concepts",
                "Learn about cables and physical connections",
                "Grasp basic communication protocols"
            },
            Tags = new List<string> { "networking", "fundamentals", "beginner" },
            Lessons = GetModule1Lessons(),
            CreatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Get Module 1 badges for gamification (INFERRED: achievement badges for module completion)
    /// </summary>
    public static List<Badge> GetModule1Badges()
    {
        return new List<Badge>
        {
            new Badge
            {
                BadgeId = "network_novice",
                Name = "Network Novice",
                Description = "Successfully completed Network Fundamentals module",
                Category = BadgeCategory.Progress,
                Rarity = BadgeRarity.Common,
                Requirements = "Complete all lessons in Module 1",
                RewardMessage = "🎉 You've mastered the networking basics!",
                CreatedAt = DateTime.UtcNow
            },
            new Badge
            {
                BadgeId = "cable_expert",
                Name = "Cable Expert",
                Description = "Demonstrated mastery of physical network connections",
                Category = BadgeCategory.Mastery,
                Rarity = BadgeRarity.Uncommon,
                Requirements = "Score 90%+ on cable identification quiz",
                RewardMessage = "⚡ Your cable knowledge is electrifying!",
                CreatedAt = DateTime.UtcNow
            }
        };
    }
}