using NetToolkit.Modules.Education.Models;
using Newtonsoft.Json;

namespace NetToolkit.Modules.Education.Data;

/// <summary>
/// Simplified Module 4 content seed matching actual model structure
/// </summary>
public static class Module4ContentSeed
{
    public static List<Lesson> GetModule4Lessons()
    {
        var lessons = new List<Lesson>();
        
        lessons.Add(new Lesson
        {
            Id = 61,
            ModuleId = 4,
            LessonNumber = 1,
            Title = "Scripting Sorcery - PowerShell Basics",
            Description = "Learn the magic of PowerShell scripting for network management.",
            EstimatedMinutes = 45,
            Objectives = "Understand PowerShell fundamentals and basic network commands",
            ContentJson = JsonConvert.SerializeObject(new LessonContent
            {
                Slides = new List<Slide>
                {
                    new()
                    {
                        Title = "PowerShell Magic",
                        Content = "PowerShell lets you automate network tasks with powerful commands.",
                        Type = SlideType.Text
                    }
                }
            })
        });

        lessons.Add(new Lesson
        {
            Id = 62,
            ModuleId = 4,
            LessonNumber = 2,
            Title = "Variables and Network Data",
            Description = "Store and manipulate network information using PowerShell variables.",
            EstimatedMinutes = 40,
            Objectives = "Learn to work with variables and network data in scripts",
            ContentJson = JsonConvert.SerializeObject(new LessonContent
            {
                Slides = new List<Slide>
                {
                    new()
                    {
                        Title = "Working with Variables",
                        Content = "Variables store network information for use throughout your scripts.",
                        Type = SlideType.Text
                    }
                }
            })
        });

        return lessons;
    }

    /// <summary>
    /// Get complete Module 4 content with lessons (INFERRED: combines module metadata with lessons)
    /// </summary>
    public static Module GetModule4Content()
    {
        return new Module
        {
            Id = 4,
            Title = "Network Scripting & Automation",
            Description = "Learn to automate network operations using scripting technologies",
            Category = "Automation",
            Difficulty = DifficultyLevel.Intermediate,
            EstimatedMinutes = 540, // 9 hours in minutes
            Prerequisites = "Complete Module 3: Protocol Fundamentals",
            LearningOutcomes = "Students will create network automation scripts, implement configuration management, and develop monitoring solutions",
            LearningObjectives = new List<string>
            {
                "Write PowerShell scripts for network automation",
                "Implement configuration management workflows",
                "Create automated monitoring and alerting systems"
            },
            Tags = new List<string> { "automation", "scripting", "powershell", "intermediate" },
            Lessons = GetModule4Lessons(),
            CreatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Get Module 4 badges for gamification (INFERRED: achievement badges for module completion)
    /// </summary>
    public static List<Badge> GetModule4Badges()
    {
        return new List<Badge>
        {
            new Badge
            {
                BadgeId = "script_sorcerer",
                Name = "Script Sorcerer",
                Description = "Successfully completed Network Scripting & Automation module",
                Category = BadgeCategory.Progress,
                Rarity = BadgeRarity.Uncommon,
                Requirements = "Complete all lessons in Module 4",
                RewardMessage = "⚡ Your automation magic is powerful!",
                CreatedAt = DateTime.UtcNow
            },
            new Badge
            {
                BadgeId = "automation_architect",
                Name = "Automation Architect",
                Description = "Mastered complex network automation workflows",
                Category = BadgeCategory.Mastery,
                Rarity = BadgeRarity.Rare,
                Requirements = "Create and deploy a complete automation solution",
                RewardMessage = "🏗️ Your architectural vision is inspiring!",
                CreatedAt = DateTime.UtcNow
            }
        };
    }
}