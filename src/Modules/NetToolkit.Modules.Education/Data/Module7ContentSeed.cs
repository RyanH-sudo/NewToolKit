using NetToolkit.Modules.Education.Models;
using Newtonsoft.Json;

namespace NetToolkit.Modules.Education.Data;

/// <summary>
/// Simplified Module 7 content seed matching actual model structure
/// </summary>
public static class Module7ContentSeed
{
    public static List<Lesson> GetModule7Lessons()
    {
        var lessons = new List<Lesson>();
        
        lessons.Add(new Lesson
        {
            Id = 121,
            ModuleId = 7,
            LessonNumber = 1,
            Title = "Wireless Networking Basics",
            Description = "Learn about wireless networking fundamentals.",
            EstimatedMinutes = 30,
            Objectives = "Understand wireless concepts and WiFi technology",
            ContentJson = JsonConvert.SerializeObject(new LessonContent
            {
                Slides = new List<Slide>
                {
                    new()
                    {
                        Title = "Wireless Fundamentals",
                        Content = "Wireless networks use radio waves to transmit data.",
                        Type = SlideType.Text
                    }
                }
            })
        });

        return lessons;
    }
}