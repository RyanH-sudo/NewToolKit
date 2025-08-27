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
}