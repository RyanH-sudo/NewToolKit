using NetToolkit.Modules.Education.Models;
using Newtonsoft.Json;

namespace NetToolkit.Modules.Education.Data;

/// <summary>
/// Simplified Module 3 content seed matching actual model structure
/// </summary>
public static class Module3ContentSeed
{
    public static List<Lesson> GetModule3Lessons()
    {
        var lessons = new List<Lesson>();
        
        lessons.Add(new Lesson
        {
            Id = 41,
            ModuleId = 3,
            LessonNumber = 1,
            Title = "IP Shenanigans - Address Basics",
            Description = "Learn about IP addresses - the digital doorplates of the internet.",
            EstimatedMinutes = 40,
            Objectives = "Understand IP addressing concepts and basic subnet principles",
            ContentJson = JsonConvert.SerializeObject(new LessonContent
            {
                Slides = new List<Slide>
                {
                    new()
                    {
                        Title = "IP Addresses Explained",
                        Content = "IP addresses are unique identifiers that help data find its destination.",
                        Type = SlideType.Text
                    }
                }
            })
        });

        lessons.Add(new Lesson
        {
            Id = 42,
            ModuleId = 3,
            LessonNumber = 2,
            Title = "Subnets and Network Division",
            Description = "Explore how networks are divided into subnets for organization.",
            EstimatedMinutes = 35,
            Objectives = "Learn subnet concepts and basic network organization",
            ContentJson = JsonConvert.SerializeObject(new LessonContent
            {
                Slides = new List<Slide>
                {
                    new()
                    {
                        Title = "Subnet Fundamentals",
                        Content = "Subnets divide large networks into smaller, manageable segments.",
                        Type = SlideType.Text
                    }
                }
            })
        });

        return lessons;
    }
}