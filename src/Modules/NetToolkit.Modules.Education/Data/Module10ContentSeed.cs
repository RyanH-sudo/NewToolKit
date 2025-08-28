using NetToolkit.Modules.Education.Models;
using Newtonsoft.Json;

namespace NetToolkit.Modules.Education.Data;

/// <summary>
/// Simplified Module 10 content seed matching actual model structure
/// </summary>
public static class Module10ContentSeed
{
    public static List<Lesson> GetModule10Lessons()
    {
        var lessons = new List<Lesson>();
        
        lessons.Add(new Lesson
        {
            Id = 181,
            ModuleId = 10,
            LessonNumber = 1,
            Title = "Mastery Mayhem - Engineer Review",
            Description = "Review and synthesize all networking concepts learned in previous modules.",
            EstimatedMinutes = 60,
            Objectives = "Integrate knowledge from all modules and prepare for advanced scenarios",
            ContentJson = JsonConvert.SerializeObject(new LessonContent
            {
                Slides = new List<Slide>
                {
                    new()
                    {
                        Title = "Network Engineering Mastery",
                        Content = "Combine all your networking knowledge into real-world solutions.",
                        Type = SlideType.Text
                    }
                }
            })
        });

        lessons.Add(new Lesson
        {
            Id = 182,
            ModuleId = 10,
            LessonNumber = 2,
            Title = "Advanced Troubleshooting Projects",
            Description = "Apply comprehensive troubleshooting to complex network scenarios.",
            EstimatedMinutes = 55,
            Objectives = "Master advanced troubleshooting techniques and network problem solving",
            ContentJson = JsonConvert.SerializeObject(new LessonContent
            {
                Slides = new List<Slide>
                {
                    new()
                    {
                        Title = "Expert Troubleshooting",
                        Content = "Use systematic approaches to solve complex network problems.",
                        Type = SlideType.Text
                    }
                }
            })
        });

        return lessons;
    }

    public static Module GetModule10Content()
    {
        return new Module
        {
            Id = 10,
            Title = "Network Mastery & Future Technologies",
            Description = "Achieve network mastery and explore emerging technologies",
            Category = "Mastery",
            Difficulty = DifficultyLevel.Expert,
            EstimatedMinutes = 800,
            Prerequisites = "Complete Module 9: Protocol Alchemy & Integration",
            LearningOutcomes = "Students will demonstrate complete network mastery, explore emerging technologies, and architect next-generation network solutions",
            LearningObjectives = new List<string> { "Demonstrate comprehensive network mastery", "Explore emerging networking technologies", "Design next-generation network architectures" },
            Tags = new List<string> { "mastery", "future-tech", "innovation", "expert" },
            Lessons = GetModule10Lessons(),
            CreatedAt = DateTime.UtcNow
        };
    }

    public static List<Badge> GetModule10Badges()
    {
        return new List<Badge>
        {
            new Badge { BadgeId = "network_grand_master", Name = "Network Grand Master", Description = "Successfully completed Network Mastery & Future Technologies module", Category = BadgeCategory.Special, Rarity = BadgeRarity.Legendary, Requirements = "Complete all lessons in Module 10", RewardMessage = "👑 You are the ultimate Network Grand Master!", CreatedAt = DateTime.UtcNow }
        };
    }
}