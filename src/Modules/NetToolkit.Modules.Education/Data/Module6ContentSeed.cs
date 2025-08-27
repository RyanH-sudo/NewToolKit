using NetToolkit.Modules.Education.Models;
using Newtonsoft.Json;

namespace NetToolkit.Modules.Education.Data;

/// <summary>
/// Simplified Module 6 content seed matching actual model structure
/// </summary>
public static class Module6ContentSeed
{
    public static List<Lesson> GetModule6Lessons()
    {
        var lessons = new List<Lesson>();
        
        lessons.Add(new Lesson
        {
            Id = 104,
            ModuleId = 6,
            LessonNumber = 1,
            Title = "Firewall Fundamentals",
            Description = "Learn about digital firewalls and network security.",
            EstimatedMinutes = 25,
            Objectives = "Understand firewall concepts and network protection",
            ContentJson = JsonConvert.SerializeObject(new LessonContent
            {
                Slides = new List<Slide>
                {
                    new()
                    {
                        Title = "Firewall Basics",
                        Content = "Firewalls protect your network by filtering traffic.",
                        Type = SlideType.Text
                    }
                }
            })
        });

        return lessons;
    }
}