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
}