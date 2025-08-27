using NetToolkit.Modules.Education.Models;
using Newtonsoft.Json;

namespace NetToolkit.Modules.Education.Data;

/// <summary>
/// Simplified Module 4 content seed matching actual model structure
/// </summary>
public static class Module4ContentSeed
{
    public static List<Lesson> GetModule4Lessons()
    {
        var lessons = new List<Lesson>();
        
        lessons.Add(new Lesson
        {
            Id = 61,
            ModuleId = 4,
            LessonNumber = 1,
            Title = "Scripting Sorcery - PowerShell Basics",
            Description = "Learn the magic of PowerShell scripting for network management.",
            EstimatedMinutes = 45,
            Objectives = "Understand PowerShell fundamentals and basic network commands",
            ContentJson = JsonConvert.SerializeObject(new LessonContent
            {
                Slides = new List<Slide>
                {
                    new()
                    {
                        Title = "PowerShell Magic",
                        Content = "PowerShell lets you automate network tasks with powerful commands.",
                        Type = SlideType.Text
                    }
                }
            })
        });

        lessons.Add(new Lesson
        {
            Id = 62,
            ModuleId = 4,
            LessonNumber = 2,
            Title = "Variables and Network Data",
            Description = "Store and manipulate network information using PowerShell variables.",
            EstimatedMinutes = 40,
            Objectives = "Learn to work with variables and network data in scripts",
            ContentJson = JsonConvert.SerializeObject(new LessonContent
            {
                Slides = new List<Slide>
                {
                    new()
                    {
                        Title = "Working with Variables",
                        Content = "Variables store network information for use throughout your scripts.",
                        Type = SlideType.Text
                    }
                }
            })
        });

        return lessons;
    }
}