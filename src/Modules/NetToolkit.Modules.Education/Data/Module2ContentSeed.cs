using NetToolkit.Modules.Education.Models;
using Newtonsoft.Json;

namespace NetToolkit.Modules.Education.Data;

/// <summary>
/// Simplified Module 2 content seed matching actual model structure
/// </summary>
public static class Module2ContentSeed
{
    public static List<Lesson> GetModule2Lessons()
    {
        var lessons = new List<Lesson>();
        
        lessons.Add(new Lesson
        {
            Id = 21,
            ModuleId = 2,
            LessonNumber = 1,
            Title = "Hardware Heroes - Network Devices",
            Description = "Meet the heroic devices that make networking possible.",
            EstimatedMinutes = 35,
            Objectives = "Identify network hardware devices and understand their functions",
            ContentJson = JsonConvert.SerializeObject(new LessonContent
            {
                Slides = new List<Slide>
                {
                    new()
                    {
                        Title = "Network Hardware Heroes",
                        Content = "Routers, switches, and NICs work together to create networks.",
                        Type = SlideType.Text
                    }
                }
            })
        });

        lessons.Add(new Lesson
        {
            Id = 22,
            ModuleId = 2,
            LessonNumber = 2,
            Title = "Switches and Hubs",
            Description = "Learn about switches and hubs for network connectivity.",
            EstimatedMinutes = 30,
            Objectives = "Understand the difference between switches and hubs",
            ContentJson = JsonConvert.SerializeObject(new LessonContent
            {
                Slides = new List<Slide>
                {
                    new()
                    {
                        Title = "Switches vs Hubs",
                        Content = "Switches are intelligent, hubs are simple broadcast devices.",
                        Type = SlideType.Text
                    }
                }
            })
        });

        return lessons;
    }

    /// <summary>
    /// Get complete Module 2 content with lessons (INFERRED: combines module metadata with lessons)
    /// </summary>
    public static Module GetModule2Content()
    {
        return new Module
        {
            Id = 2,
            Title = "Network Hardware & Devices",
            Description = "Explore network hardware components and device configurations",
            Category = "Networking",
            Difficulty = DifficultyLevel.Beginner,
            EstimatedMinutes = 420, // 7 hours in minutes
            Prerequisites = "Complete Module 1: Network Fundamentals",
            LearningOutcomes = "Students will identify network devices, understand hardware configurations, and master device troubleshooting techniques",
            LearningObjectives = new List<string>
            {
                "Identify common network devices and their functions",
                "Configure basic network hardware settings",
                "Troubleshoot hardware connectivity issues"
            },
            Tags = new List<string> { "networking", "hardware", "devices", "beginner" },
            Lessons = GetModule2Lessons(),
            CreatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Get Module 2 badges for gamification (INFERRED: achievement badges for module completion)
    /// </summary>
    public static List<Badge> GetModule2Badges()
    {
        return new List<Badge>
        {
            new Badge
            {
                BadgeId = "hardware_hero",
                Name = "Hardware Hero",
                Description = "Successfully completed Network Hardware & Devices module",
                Category = BadgeCategory.Progress,
                Rarity = BadgeRarity.Common,
                Requirements = "Complete all lessons in Module 2",
                RewardMessage = "🔧 You've conquered the world of network hardware!",
                CreatedAt = DateTime.UtcNow
            },
            new Badge
            {
                BadgeId = "device_detective",
                Name = "Device Detective",
                Description = "Demonstrated expertise in network device troubleshooting",
                Category = BadgeCategory.Mastery,
                Rarity = BadgeRarity.Uncommon,
                Requirements = "Score 85%+ on hardware troubleshooting scenarios",
                RewardMessage = "🕵️ Your diagnostic skills are legendary!",
                CreatedAt = DateTime.UtcNow
            }
        };
    }
}