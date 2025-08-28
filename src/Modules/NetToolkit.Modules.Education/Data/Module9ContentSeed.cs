using NetToolkit.Modules.Education.Models;
using Newtonsoft.Json;

namespace NetToolkit.Modules.Education.Data;

/// <summary>
/// Simplified Module 9 content seed matching actual model structure
/// </summary>
public static class Module9ContentSeed
{
    public static List<Lesson> GetModule9Lessons()
    {
        var lessons = new List<Lesson>();
        
        lessons.Add(new Lesson
        {
            Id = 161,
            ModuleId = 9,
            LessonNumber = 1,
            Title = "Protocol Alchemy Basics",
            Description = "Learn about advanced protocol mixing and configuration.",
            EstimatedMinutes = 45,
            Objectives = "Understand protocol alchemy and advanced networking",
            ContentJson = JsonConvert.SerializeObject(new LessonContent
            {
                Slides = new List<Slide>
                {
                    new()
                    {
                        Title = "Protocol Alchemy",
                        Content = "Advanced networking requires mastery of protocol interactions.",
                        Type = SlideType.Text
                    }
                }
            })
        });

        return lessons;
    }

    public static Module GetModule9Content()
    {
        return new Module
        {
            Id = 9,
            Title = "Protocol Alchemy & Integration",
            Description = "Master advanced protocol integration and network alchemy",
            Category = "Advanced",
            Difficulty = DifficultyLevel.Expert,
            EstimatedMinutes = 720,
            Prerequisites = "Complete Module 8: Cloud Computing & Virtualization", 
            LearningOutcomes = "Students will integrate complex protocols, design multi-protocol architectures, and optimize network interoperability",
            LearningObjectives = new List<string> { "Integrate multiple networking protocols", "Design complex network architectures", "Optimize protocol interactions and performance" },
            Tags = new List<string> { "protocols", "integration", "alchemy", "expert" },
            Lessons = GetModule9Lessons(),
            CreatedAt = DateTime.UtcNow
        };
    }

    public static List<Badge> GetModule9Badges()
    {
        return new List<Badge>
        {
            new Badge { BadgeId = "protocol_alchemist", Name = "Protocol Alchemist", Description = "Successfully completed Protocol Alchemy & Integration module", Category = BadgeCategory.Special, Rarity = BadgeRarity.Epic, Requirements = "Complete all lessons in Module 9", RewardMessage = "🧪 Your protocol alchemy is pure genius!", CreatedAt = DateTime.UtcNow }
        };
    }
}