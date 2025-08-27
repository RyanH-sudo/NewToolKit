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
}