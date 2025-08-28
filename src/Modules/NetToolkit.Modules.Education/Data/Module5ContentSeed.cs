using NetToolkit.Modules.Education.Models;
using Newtonsoft.Json;

namespace NetToolkit.Modules.Education.Data;

/// <summary>
/// Simplified Module 5 content seed matching actual model structure
/// </summary>
public static class Module5ContentSeed
{
    public static List<Lesson> GetModule5Lessons()
    {
        var lessons = new List<Lesson>();
        
        lessons.Add(new Lesson
        {
            Id = 81,
            ModuleId = 5,
            LessonNumber = 1,
            Title = "Routing Riddles - Path Fundamentals",
            Description = "Discover how data packets find their way through network paths.",
            EstimatedMinutes = 50,
            Objectives = "Understand routing concepts and path selection algorithms",
            ContentJson = JsonConvert.SerializeObject(new LessonContent
            {
                Slides = new List<Slide>
                {
                    new()
                    {
                        Title = "Network Routing Basics",
                        Content = "Routers determine the best path for data to travel across networks.",
                        Type = SlideType.Text
                    }
                }
            })
        });

        lessons.Add(new Lesson
        {
            Id = 82,
            ModuleId = 5,
            LessonNumber = 2,
            Title = "Static vs Dynamic Routing",
            Description = "Learn the difference between static and dynamic routing protocols.",
            EstimatedMinutes = 45,
            Objectives = "Compare static routing with dynamic routing protocols",
            ContentJson = JsonConvert.SerializeObject(new LessonContent
            {
                Slides = new List<Slide>
                {
                    new()
                    {
                        Title = "Routing Types",
                        Content = "Static routes are manually configured, dynamic routes adapt automatically.",
                        Type = SlideType.Text
                    }
                }
            })
        });

        return lessons;
    }

    /// <summary>
    /// Get complete Module 5 content with lessons (INFERRED: combines module metadata with lessons)
    /// </summary>
    public static Module GetModule5Content()
    {
        return new Module
        {
            Id = 5,
            Title = "Network Security Foundations",
            Description = "Master fundamental network security principles and defense strategies",
            Category = "Security",
            Difficulty = DifficultyLevel.Intermediate,
            EstimatedMinutes = 600, // 10 hours in minutes
            Prerequisites = "Complete Module 4: Network Scripting & Automation",
            LearningOutcomes = "Students will implement security controls, analyze threats, and design defensive network architectures",
            LearningObjectives = new List<string>
            {
                "Understand network security threat landscape",
                "Implement firewall and access control systems",
                "Design secure network architectures"
            },
            Tags = new List<string> { "security", "firewall", "defense", "intermediate" },
            Lessons = GetModule5Lessons(),
            CreatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Get Module 5 badges for gamification (INFERRED: achievement badges for module completion)
    /// </summary>
    public static List<Badge> GetModule5Badges()
    {
        return new List<Badge>
        {
            new Badge
            {
                BadgeId = "security_sentinel",
                Name = "Security Sentinel",
                Description = "Successfully completed Network Security Foundations module",
                Category = BadgeCategory.Progress,
                Rarity = BadgeRarity.Uncommon,
                Requirements = "Complete all lessons in Module 5",
                RewardMessage = "🛡️ Your defensive skills are formidable!",
                CreatedAt = DateTime.UtcNow
            },
            new Badge
            {
                BadgeId = "cyber_guardian",
                Name = "Cyber Guardian",
                Description = "Demonstrated exceptional network security expertise",
                Category = BadgeCategory.Special,
                Rarity = BadgeRarity.Epic,
                Requirements = "Design and implement a comprehensive security architecture",
                RewardMessage = "🏰 Your security fortress is impenetrable!",
                CreatedAt = DateTime.UtcNow
            }
        };
    }
}