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
}