using NetToolkit.Modules.Education.Models;
using Newtonsoft.Json;

namespace NetToolkit.Modules.Education.Data;

/// <summary>
/// Simplified Module 8 content seed matching actual model structure
/// </summary>
public static class Module8ContentSeed
{
    public static List<Lesson> GetModule8Lessons()
    {
        var lessons = new List<Lesson>();
        
        lessons.Add(new Lesson
        {
            Id = 141,
            ModuleId = 8,
            LessonNumber = 1,
            Title = "Cloud Conquest - Sky-High Basics",
            Description = "Learn about cloud computing fundamentals and service models.",
            EstimatedMinutes = 55,
            Objectives = "Understand cloud computing concepts and IaaS, PaaS, SaaS models",
            ContentJson = JsonConvert.SerializeObject(new LessonContent
            {
                Slides = new List<Slide>
                {
                    new()
                    {
                        Title = "Cloud Computing Fundamentals",
                        Content = "Cloud services provide computing resources over the internet.",
                        Type = SlideType.Text
                    }
                }
            })
        });

        lessons.Add(new Lesson
        {
            Id = 142,
            ModuleId = 8,
            LessonNumber = 2,
            Title = "Azure and AWS Platforms",
            Description = "Explore major cloud platforms and their networking capabilities.",
            EstimatedMinutes = 50,
            Objectives = "Compare cloud platforms and understand their network services",
            ContentJson = JsonConvert.SerializeObject(new LessonContent
            {
                Slides = new List<Slide>
                {
                    new()
                    {
                        Title = "Major Cloud Providers",
                        Content = "Azure, AWS, and GCP offer comprehensive cloud networking solutions.",
                        Type = SlideType.Text
                    }
                }
            })
        });

        return lessons;
    }

    public static Module GetModule8Content()
    {
        return new Module
        {
            Id = 8,
            Title = "Cloud Computing & Virtualization",
            Description = "Master cloud technologies and network virtualization",
            Category = "Cloud",
            Difficulty = DifficultyLevel.Advanced,
            EstimatedMinutes = 600,
            Prerequisites = "Complete Module 7: Wireless Networking & Mobility",
            LearningOutcomes = "Students will architect cloud networks, implement virtualization solutions, and manage hybrid cloud environments",
            LearningObjectives = new List<string> { "Design cloud network architectures", "Implement network virtualization technologies", "Manage hybrid and multi-cloud environments" },
            Tags = new List<string> { "cloud", "virtualization", "aws", "azure", "advanced" },
            Lessons = GetModule8Lessons(),
            CreatedAt = DateTime.UtcNow
        };
    }

    public static List<Badge> GetModule8Badges()
    {
        return new List<Badge>
        {
            new Badge { BadgeId = "cloud_commander", Name = "Cloud Commander", Description = "Successfully completed Cloud Computing & Virtualization module", Category = BadgeCategory.Progress, Rarity = BadgeRarity.Rare, Requirements = "Complete all lessons in Module 8", RewardMessage = "☁️ Your cloud command is supreme!", CreatedAt = DateTime.UtcNow }
        };
    }
}