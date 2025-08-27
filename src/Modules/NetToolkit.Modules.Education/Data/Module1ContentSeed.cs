using NetToolkit.Modules.Education.Models;
using Newtonsoft.Json;

namespace NetToolkit.Modules.Education.Data;

/// <summary>
/// Simplified Module 1 content seed matching actual model structure
/// </summary>
public static class Module1ContentSeed
{
    public static List<Lesson> GetModule1Lessons()
    {
        var lessons = new List<Lesson>();
        
        lessons.Add(new Lesson
        {
            Id = 1,
            ModuleId = 1,
            LessonNumber = 1,
            Title = "Network Basics - From Cables to Cosmos",
            Description = "Discover the fundamentals of networking and how devices communicate.",
            EstimatedMinutes = 30,
            Objectives = "Understand basic networking concepts and device communication",
            ContentJson = JsonConvert.SerializeObject(new LessonContent
            {
                Slides = new List<Slide>
                {
                    new()
                    {
                        Title = "Welcome to Networking",
                        Content = "Learn how computers connect and communicate across networks.",
                        Type = SlideType.Text
                    }
                }
            })
        });

        lessons.Add(new Lesson
        {
            Id = 2,
            ModuleId = 1,
            LessonNumber = 2,
            Title = "Cables and Physical Connections",
            Description = "Explore the physical layer of networking with cables and connectors.",
            EstimatedMinutes = 25,
            Objectives = "Identify common network cables and understand physical connectivity",
            ContentJson = JsonConvert.SerializeObject(new LessonContent
            {
                Slides = new List<Slide>
                {
                    new()
                    {
                        Title = "Network Cables",
                        Content = "Ethernet cables carry data between devices using copper wires.",
                        Type = SlideType.Text
                    }
                }
            })
        });

        return lessons;
    }
}