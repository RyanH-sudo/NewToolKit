using NetToolkit.Modules.Education.Models;
using Newtonsoft.Json;

namespace NetToolkit.Modules.Education.Data;

/// <summary>
/// Simplified Module 7 content seed matching actual model structure
/// </summary>
public static class Module7ContentSeed
{
    public static List<Lesson> GetModule7Lessons()
    {
        var lessons = new List<Lesson>();
        
        lessons.Add(new Lesson
        {
            Id = 121,
            ModuleId = 7,
            LessonNumber = 1,
            Title = "Wireless Networking Basics",
            Description = "Learn about wireless networking fundamentals.",
            EstimatedMinutes = 30,
            Objectives = "Understand wireless concepts and WiFi technology",
            ContentJson = JsonConvert.SerializeObject(new LessonContent
            {
                Slides = new List<Slide>
                {
                    new()
                    {
                        Title = "Wireless Fundamentals",
                        Content = "Wireless networks use radio waves to transmit data.",
                        Type = SlideType.Text
                    }
                }
            })
        });

        return lessons;
    }

    /// <summary>
    /// Get complete Module 7 content with lessons (INFERRED: combines module metadata with lessons)
    /// </summary>
    public static Module GetModule7Content()
    {
        return new Module
        {
            Id = 7,
            Title = "Wireless Networking & Mobility",
            Description = "Master wireless technologies and mobile network solutions",
            Category = "Wireless",
            Difficulty = DifficultyLevel.Advanced,
            EstimatedMinutes = 540, // 9 hours in minutes
            Prerequisites = "Complete Module 6: Advanced Routing & Switching",
            LearningOutcomes = "Students will design wireless networks, implement mobility solutions, and optimize wireless performance",
            LearningObjectives = new List<string>
            {
                "Design and deploy wireless network infrastructures",
                "Configure wireless security and authentication",
                "Optimize wireless performance and coverage"
            },
            Tags = new List<string> { "wireless", "wifi", "mobility", "advanced" },
            Lessons = GetModule7Lessons(),
            CreatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Get Module 7 badges for gamification (INFERRED: achievement badges for module completion)
    /// </summary>
    public static List<Badge> GetModule7Badges()
    {
        return new List<Badge>
        {
            new Badge
            {
                BadgeId = "wireless_wizard",
                Name = "Wireless Wizard",
                Description = "Successfully completed Wireless Networking & Mobility module",
                Category = BadgeCategory.Progress,
                Rarity = BadgeRarity.Rare,
                Requirements = "Complete all lessons in Module 7",
                RewardMessage = "📡 Your wireless wizardry is magnificent!",
                CreatedAt = DateTime.UtcNow
            },
            new Badge
            {
                BadgeId = "mobility_master",
                Name = "Mobility Master",
                Description = "Mastered advanced wireless and mobility solutions",
                Category = BadgeCategory.Mastery,
                Rarity = BadgeRarity.Epic,
                Requirements = "Design a complex multi-site wireless infrastructure",
                RewardMessage = "🌊 Your mobility mastery flows seamlessly!",
                CreatedAt = DateTime.UtcNow
            }
        };
    }
}