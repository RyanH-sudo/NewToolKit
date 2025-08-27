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
}