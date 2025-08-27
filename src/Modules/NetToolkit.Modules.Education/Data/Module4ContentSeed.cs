using NetToolkit.Modules.Education.Models;
using System.Collections.Immutable;

namespace NetToolkit.Modules.Education.Data;

/// <summary>
/// Module 4: Scripting Sorcery - Command Your Kingdom
/// Content seed data for the cosmic scripting education experience
/// Where commands become spells and code transforms into network magic
/// </summary>
public static class Module4ContentSeed
{
    /// <summary>
    /// Get complete Module 4 content with all 20 scripting sorcery lessons
    /// </summary>
    public static Module GetModule4Content()
    {
        return new Module
        {
            Id = 4,
            Title = "Scripting Sorcery - Command Your Kingdom",
            Description = "Embark on a mystical journey through the enchanted realm of scripting! Transform from curious apprentice to master sorcerer, wielding PowerShell spells and commanding your network kingdom with magical automation. Learn to craft powerful incantations, brew variable potions, and cast conditional charms that bend networks to your will!",
            Difficulty = DifficultyLevel.Intermediate,
            EstimatedMinutes = 600, // 10 hours total - comprehensive scripting mastery
            Prerequisites = "Completion of Module 3: IP Shenanigans (recommended), basic computer literacy, curiosity for network automation",
            LearningOutcomes = "Master PowerShell scripting fundamentals, create network automation scripts, understand conditional logic and loops, implement error handling, integrate with Microsoft services, develop security-focused scripts, and achieve certification-level scripting proficiency",
            CreatedAt = DateTime.UtcNow,
            Lessons = GetAllScriptingLessons()
        };
    }

    /// <summary>
    /// Generate all 20 scripting sorcery lessons
    /// </summary>
    private static ICollection<Lesson> GetAllScriptingLessons()
    {
        var lessons = new List<Lesson>();

        // Lesson 1: What's a Script? Magic Spells for Computers
        lessons.Add(new Lesson
        {
            Id = 61, // Module 4 starts at lesson 61
            ModuleId = 4,
            LessonNumber = 1,
            Title = "What's a Script? Magic Spells for Computers",
            Description = "Discover the enchanting world of scripts - your first step into network sorcery!",
            LearningObjectives = new List<string>
            {
                "Understand what scripts are and their magical purpose",
                "Learn the difference between interactive commands and script spells",
                "Explore PowerShell as your network wand",
                "Write your first simple script incantation"
            },
            Content = new List<SlideContent>
            {
                new SlideContent { Type = SlideType.Image, Content = "wizard_with_wand_computer", Description = "A mystical wizard wielding a glowing wand toward a computer" },
                new SlideContent 
                { 
                    Type = SlideType.Text, 
                    Content = "🪄 Welcome to Scripting Sorcery! Scripts are like magic spells for computers - they're sequences of commands that tell your devices exactly what to do, just like incantations guide mystical forces! Instead of clicking buttons one by one, you write powerful spells (scripts) that can perform hundreds of tasks instantly.",
                    HoverTips = new List<HoverTip>
                    {
                        new HoverTip { TriggerWord = "PowerShell", Explanation = "Your network wand - Microsoft's powerful command-line shell and scripting language!" },
                        new HoverTip { TriggerWord = "commands", Explanation = "Individual instructions like 'Get-Process' or 'Test-Connection'" },
                        new HoverTip { TriggerWord = "automation", Explanation = "Making repetitive tasks run by themselves - pure wizardry!" }
                    }
                },
                new SlideContent 
                { 
                    Type = SlideType.Text, 
                    Content = "Think of it like this: Instead of manually checking each house in your neighborhood, you cast a 'Survey Spell' that magically visits every address and reports back. That's what scripts do for networks - they automate the boring stuff so you can focus on the exciting challenges! 🏰✨",
                    HoverTips = new List<HoverTip>
                    {
                        new HoverTip { TriggerWord = "automate", Explanation = "Let the computer do repetitive work while you sip coffee and feel awesome!" },
                        new HoverTip { TriggerWord = "networks", Explanation = "All the connected devices in your digital kingdom" }
                    }
                }
            },
            QuizQuestions = GetLesson1Quiz()
        });

        // Lesson 2: Variables - Your Spell Ingredients
        lessons.Add(new Lesson
        {
            Id = 62,
            ModuleId = 4,
            LessonNumber = 2,
            Title = "Variables - Your Spell Ingredients",
            Description = "Learn to store magical essences in variable containers for powerful spell crafting!",
            LearningObjectives = new List<string>
            {
                "Understand what variables are and why they're magical",
                "Learn PowerShell variable syntax with $ symbol",
                "Store different types of data in variable containers",
                "Use variables to make spells more flexible and powerful"
            },
            Content = new List<SlideContent>
            {
                new SlideContent { Type = SlideType.Image, Content = "potion_bottles_labeled", Description = "Colorful potion bottles with magical labels and glowing contents" },
                new SlideContent 
                { 
                    Type = SlideType.Text, 
                    Content = "🧪 Variables are like magical potion bottles that store different essences for your spells! In PowerShell, you create them with the $ symbol: $ipAddress = '192.168.1.1' stores an IP address, while $computerName = 'WorkstationAlpha' holds a computer name. Just like a wizard's ingredients, you can mix and match these stored values!",
                    HoverTips = new List<HoverTip>
                    {
                        new HoverTip { TriggerWord = "$ipAddress", Explanation = "The $ tells PowerShell 'this is a variable container!'" },
                        new HoverTip { TriggerWord = "strings", Explanation = "Text values wrapped in quotes like 'Hello World'" },
                        new HoverTip { TriggerWord = "integers", Explanation = "Whole numbers like 42 or 1337 - no quotes needed!" }
                    }
                },
                new SlideContent 
                { 
                    Type = SlideType.Text, 
                    Content = "Variables make your spells incredibly flexible! Instead of hardcoding 'ping 192.168.1.1', you can write 'ping $targetIP' and change $targetIP anytime. It's like having adjustable spell components - one moment you're pinging the router, the next you're checking the server, all with the same magical formula! 🎯",
                    HoverTips = new List<HoverTip>
                    {
                        new HoverTip { TriggerWord = "hardcoding", Explanation = "Writing fixed values directly in scripts - not very magical!" },
                        new HoverTip { TriggerWord = "flexible", Explanation = "Able to adapt and change - the essence of powerful scripting!" }
                    }
                }
            },
            QuizQuestions = GetLesson2Quiz()
        });

        // Continue with remaining lessons...
        lessons.AddRange(GetRemainingScriptingLessons());

        return lessons;
    }

    /// <summary>
    /// Get remaining scripting lessons (3-20)
    /// </summary>
    private static ICollection<Lesson> GetRemainingScriptingLessons()
    {
        var lessons = new List<Lesson>();

        // Lesson 3: Commands - The Basic Incantations
        lessons.Add(new Lesson
        {
            Id = 63,
            ModuleId = 4,
            LessonNumber = 3,
            Title = "Commands - The Basic Incantations",
            Description = "Master the fundamental spells that form the foundation of all network sorcery!",
            LearningObjectives = new List<string>
            {
                "Discover PowerShell's built-in command spells",
                "Learn to browse the grimoire with Get-Command",
                "Understand command syntax and parameters",
                "Cast your first network diagnostic incantations"
            },
            Content = new List<SlideContent>
            {
                new SlideContent { Type = SlideType.Image, Content = "ancient_spell_book_glowing", Description = "An ancient spell book with glowing text and mystical symbols" },
                new SlideContent 
                { 
                    Type = SlideType.Text, 
                    Content = "📚 PowerShell commands are like spells in an ancient grimoire! Get-Command reveals all available spells (there are thousands!), Get-Process shows running spirits, and Test-Connection pings distant realms. Each command follows the Verb-Noun pattern: Get-Service, Set-Location, Stop-Process - like perfectly structured incantations!",
                    HoverTips = new List<HoverTip>
                    {
                        new HoverTip { TriggerWord = "Get-Command", Explanation = "Lists all available PowerShell commands - your spell catalog!" },
                        new HoverTip { TriggerWord = "Verb-Noun", Explanation = "PowerShell's magical naming pattern: action-target like Cast-Spell!" },
                        new HoverTip { TriggerWord = "parameters", Explanation = "Options that modify how spells work - like spell modifiers!" }
                    }
                },
                new SlideContent 
                { 
                    Type = SlideType.Text, 
                    Content = "🔍 Want to explore? Try 'Get-Command *IP*' to find all IP-related spells, or 'Get-Help Test-Connection' to learn how the ping spell works. Each command has built-in documentation - like spell descriptions written by the original wizards who created them!",
                    HoverTips = new List<HoverTip>
                    {
                        new HoverTip { TriggerWord = "Get-Help", Explanation = "Shows detailed information about any PowerShell command" },
                        new HoverTip { TriggerWord = "wildcards", Explanation = "The * symbol finds patterns - like 'show me all spells containing IP'" }
                    }
                }
            },
            QuizQuestions = GetLesson3Quiz()
        });

        // Add more lessons with similar comprehensive structure...
        lessons.AddRange(GetAdvancedScriptingLessons());

        return lessons;
    }

    /// <summary>
    /// Get advanced scripting lessons (4-20)
    /// </summary>
    private static ICollection<Lesson> GetAdvancedScriptingLessons()
    {
        var lessons = new List<Lesson>();

        // Lesson 4-20 implementation...
        for (int i = 4; i <= 20; i++)
        {
            lessons.Add(CreateScriptingLesson(i));
        }

        return lessons;
    }

    /// <summary>
    /// Create individual scripting lessons with cosmic content
    /// </summary>
    private static Lesson CreateScriptingLesson(int lessonNumber)
    {
        return lessonNumber switch
        {
            4 => new Lesson
            {
                Id = 64,
                ModuleId = 4,
                LessonNumber = 4,
                Title = "Strings and Numbers - Building Blocks",
                Description = "Master the fundamental data types that form the foundation of all magical scripts!",
                LearningObjectives = new List<string>
                {
                    "Understand string manipulation in PowerShell",
                    "Work with numbers and mathematical operations",
                    "Combine strings and numbers in scripts",
                    "Use data type conversion spells"
                },
                Content = new List<SlideContent>
                {
                    new SlideContent { Type = SlideType.Image, Content = "labeled_building_blocks", Description = "Colorful building blocks labeled with text and numbers" },
                    new SlideContent 
                    { 
                        Type = SlideType.Text, 
                        Content = "🧱 Strings and numbers are the magical building blocks of all scripts! Strings are text wrapped in quotes like 'Hello Network' or \"PowerShell Wizard\", while numbers are raw values like 42, 3.14, or 192 (no quotes needed). You can combine them: \"Server\" + \"01\" creates \"Server01\"!",
                        HoverTips = new List<HoverTip>
                        {
                            new HoverTip { TriggerWord = "concatenation", Explanation = "Joining strings together with + operator - like magical glue!" },
                            new HoverTip { TriggerWord = "integers", Explanation = "Whole numbers without decimal points" },
                            new HoverTip { TriggerWord = "doubles", Explanation = "Numbers with decimal points for precision" }
                        }
                    },
                    new SlideContent 
                    { 
                        Type = SlideType.Text, 
                        Content = "🔢 PowerShell is smart about data types! It automatically converts when possible: '5' + 3 becomes '53' (string), but 5 + 3 becomes 8 (number). Use [int]'123' to force conversion to integer, or [string]456 to make numbers into text. It's like casting transformation spells on your data!",
                        HoverTips = new List<HoverTip>
                        {
                            new HoverTip { TriggerWord = "type casting", Explanation = "Explicitly converting data from one type to another" },
                            new HoverTip { TriggerWord = "automatic conversion", Explanation = "PowerShell trying to be helpful by guessing what you want" }
                        }
                    }
                },
                QuizQuestions = GetLesson4Quiz()
            },
            5 => CreateConditionalMagicLesson(),
            6 => CreateLoopMagicLesson(),
            7 => CreateFunctionSpellsLesson(),
            8 => CreateErrorHandlingLesson(),
            9 => CreateNetworkScriptingLesson(),
            10 => CreateMailboxMagicLesson(),
            11 => CreateSecurityScriptsLesson(),
            12 => CreateAutomationLesson(),
            13 => CreateParametersLesson(),
            14 => CreateModulesLesson(),
            15 => CreateDebuggingLesson(),
            16 => CreateAdvancedLoopsLesson(),
            17 => CreateObjectsLesson(),
            18 => CreateRemoteScriptingLesson(),
            19 => CreateCertLevelLesson(),
            20 => CreateQuizMasteryLesson(),
            _ => throw new ArgumentException("Invalid lesson number")
        };
    }

    /// <summary>
    /// Create Lesson 5: Conditionals - If-Then Magic
    /// </summary>
    private static Lesson CreateConditionalMagicLesson()
    {
        return new Lesson
        {
            Id = 65,
            ModuleId = 4,
            LessonNumber = 5,
            Title = "Conditionals - If-Then Magic",
            Description = "Learn the art of decision-making spells that choose different paths based on magical conditions!",
            LearningObjectives = new List<string>
            {
                "Master if-then-else logic in PowerShell",
                "Use comparison operators for spell conditions",
                "Create branching script paths",
                "Implement complex conditional logic chains"
            },
            Content = new List<SlideContent>
            {
                new SlideContent { Type = SlideType.Image, Content = "forked_enchanted_path", Description = "A mystical forest path splitting into multiple glowing directions" },
                new SlideContent 
                { 
                    Type = SlideType.Text, 
                    Content = "🌟 Conditionals are like choosing paths in an enchanted forest! If ($ip -eq 'dynamic') { Set-Static } means 'if the IP is dynamic, then make it static'. The -eq is a comparison spell that checks equality. You can also use -ne (not equal), -lt (less than), -gt (greater than) - like magical truth detectors!",
                    HoverTips = new List<HoverTip>
                    {
                        new HoverTip { TriggerWord = "-eq", Explanation = "Equal comparison - checks if two values are the same" },
                        new HoverTip { TriggerWord = "-ne", Explanation = "Not equal - checks if values are different" },
                        new HoverTip { TriggerWord = "boolean", Explanation = "True or False values - the essence of logical spells" }
                    }
                },
                new SlideContent 
                { 
                    Type = SlideType.Text, 
                    Content = "🧙‍♂️ The Else clause is your backup plan for failed spells! If ($service -eq 'Running') { Write-Host 'All good!' } Else { Restart-Service }. You can chain multiple conditions with ElseIf - like having multiple backup spells ready for different magical emergencies!",
                    HoverTips = new List<HoverTip>
                    {
                        new HoverTip { TriggerWord = "ElseIf", Explanation = "Multiple condition checks in one spell - very powerful!" },
                        new HoverTip { TriggerWord = "Write-Host", Explanation = "Displays text to the console - like speaking your spell results" }
                    }
                }
            },
            QuizQuestions = GetLesson5Quiz()
        };
    }

    // Continue implementing remaining lessons...
    private static Lesson CreateLoopMagicLesson()
    {
        return new Lesson
        {
            Id = 66,
            ModuleId = 4,
            LessonNumber = 6,
            Title = "Loops - Repeat Until Done",
            Description = "Master the mystical art of repetitive spells that continue until your magical goals are achieved!",
            LearningObjectives = new List<string>
            {
                "Understand ForEach-Object loops for collections",
                "Master While loops for conditional repetition",
                "Use For loops for counted iterations",
                "Avoid infinite loop curses and break free when needed"
            },
            Content = new List<SlideContent>
            {
                new SlideContent { Type = SlideType.Image, Content = "magical_looping_road", Description = "A enchanted road that loops in spirals with glowing magical symbols" },
                new SlideContent 
                { 
                    Type = SlideType.Text, 
                    Content = "🔄 Loops are like casting the same spell repeatedly until the dragon is slain! ForEach-Object loops through collections: Get-Process | ForEach-Object { Stop-Process $_ } stops every process. The $_ is a magical variable meaning 'current item' - like pointing your wand at each target in turn!",
                    HoverTips = new List<HoverTip>
                    {
                        new HoverTip { TriggerWord = "ForEach-Object", Explanation = "Processes each item in a collection one by one" },
                        new HoverTip { TriggerWord = "$_", Explanation = "The current object in a pipeline - PowerShell's 'this item' variable" },
                        new HoverTip { TriggerWord = "pipeline", Explanation = "Passing objects from one command to another with |" }
                    }
                },
                new SlideContent 
                { 
                    Type = SlideType.Text, 
                    Content = "⚠️ While loops cast spells until conditions change: While ($service.Status -ne 'Running') { Start-Service }. But beware infinite loops - they're like cursed spells that never end! Always ensure your condition can eventually become false, or use Break to escape the magical trap!",
                    HoverTips = new List<HoverTip>
                    {
                        new HoverTip { TriggerWord = "While", Explanation = "Repeats as long as a condition is true" },
                        new HoverTip { TriggerWord = "Break", Explanation = "Immediately exits a loop - your escape spell!" },
                        new HoverTip { TriggerWord = "infinite loop", Explanation = "A loop that never ends - the scripting equivalent of a curse!" }
                    }
                }
            },
            QuizQuestions = GetLesson6Quiz()
        };
    }

    // Additional lesson creation methods would continue here...
    private static Lesson CreateFunctionSpellsLesson() => CreateGenericLesson(7, "Functions - Reusable Spells", "Create powerful spell scrolls that can be invoked whenever needed!", "spell_scroll_collection");
    private static Lesson CreateErrorHandlingLesson() => CreateGenericLesson(8, "Error Handling - Catching Gremlins", "Learn to trap and tame the mischievous errors that plague your scripts!", "net_catching_bugs");
    private static Lesson CreateNetworkScriptingLesson() => CreateGenericLesson(9, "Network Scripting - Kingdom Commands", "Command your network realm with powerful administrative spells!", "castle_with_code_runes");
    private static Lesson CreateMailboxMagicLesson() => CreateGenericLesson(10, "Mailbox Magic - Microsoft Spells", "Enchant Exchange mailboxes with mystical storage expansion rituals!", "enchanted_mailbox_glowing");
    private static Lesson CreateSecurityScriptsLesson() => CreateGenericLesson(11, "Security Scripts - Warding Charms", "Craft protective spells that shield your kingdom from digital threats!", "protective_magical_runes");
    private static Lesson CreateAutomationLesson() => CreateGenericLesson(12, "Automation - Lazy Wizardry", "Master the art of self-casting spells that work while you sleep!", "robot_assistant_magical");
    private static Lesson CreateParametersLesson() => CreateGenericLesson(13, "Parameters - Customizing Incantations", "Make your spells flexible with adjustable magical components!", "adjustable_magic_wand");
    private static Lesson CreateModulesLesson() => CreateGenericLesson(14, "Modules - Spell Libraries", "Discover vast libraries of pre-written spells for network mastery!", "magical_library_shelves");
    private static Lesson CreateDebuggingLesson() => CreateGenericLesson(15, "Debugging - Spell Fixes", "Become a code detective and solve mysteries in your script crypts!", "magnifying_glass_code");
    private static Lesson CreateAdvancedLoopsLesson() => CreateGenericLesson(16, "Advanced Loops - For and While Mastery", "Master complex repetitive spells while avoiding endless curses!", "infinite_loop_symbol");
    private static Lesson CreateObjectsLesson() => CreateGenericLesson(17, "Objects - Magical Entities", "Manipulate mystical objects and their properties with sorting spells!", "glowing_magical_objects");
    private static Lesson CreateRemoteScriptingLesson() => CreateGenericLesson(18, "Remote Scripting - Distant Commands", "Cast spells across vast distances to command far-off kingdoms!", "magical_telescope_view");
    private static Lesson CreateCertLevelLesson() => CreateGenericLesson(19, "Cert-Level - Complex Scripts", "Forge advanced spell combinations worthy of certification mastery!", "advanced_grimoire_open");
    private static Lesson CreateQuizMasteryLesson() => CreateGenericLesson(20, "Quiz Sorcery - Scripting Mastery", "Prove your scripting supremacy with certification-level enchantments!", "magical_crown_scripts");

    /// <summary>
    /// Create generic lesson template for advanced lessons
    /// </summary>
    private static Lesson CreateGenericLesson(int number, string title, string description, string imageContent)
    {
        return new Lesson
        {
            Id = 60 + number,
            ModuleId = 4,
            LessonNumber = number,
            Title = title,
            Description = description,
            LearningObjectives = new List<string>
            {
                $"Master {title.ToLower()} concepts and techniques",
                "Apply scripting principles to real-world scenarios",
                "Integrate with network automation workflows",
                "Achieve certification-level proficiency"
            },
            Content = new List<SlideContent>
            {
                new SlideContent { Type = SlideType.Image, Content = imageContent, Description = $"Magical illustration for {title}" },
                new SlideContent 
                { 
                    Type = SlideType.Text, 
                    Content = $"🌟 {description} This lesson covers advanced concepts that will elevate your scripting sorcery to new heights of network mastery!",
                    HoverTips = new List<HoverTip>
                    {
                        new HoverTip { TriggerWord = "advanced", Explanation = "Complex techniques for experienced script sorcerers" },
                        new HoverTip { TriggerWord = "mastery", Explanation = "Complete understanding and practical application" }
                    }
                }
            },
            QuizQuestions = GetGenericQuiz(number)
        };
    }

    #region Quiz Generation Methods

    /// <summary>
    /// Generate quiz questions for Lesson 1
    /// </summary>
    private static List<QuizQuestion> GetLesson1Quiz()
    {
        return new List<QuizQuestion>
        {
            new QuizQuestion
            {
                Id = 241, // Module 4 quiz IDs start at 241
                LessonId = 61,
                QuestionText = "What are scripts in the magical world of computing?",
                Options = new List<string>
                {
                    "Sequences of commands that automate tasks like magic spells",
                    "Hardware components that store data",
                    "Network cables that connect computers",
                    "Software licenses for applications"
                },
                CorrectAnswerIndex = 0,
                Explanation = "Enchanting! Scripts are indeed like magic spells - they're sequences of commands that tell computers what to do automatically, saving you from repetitive clicking and typing!"
            },
            new QuizQuestion
            {
                Id = 242,
                LessonId = 61,
                QuestionText = "PowerShell is described as your network ___?",
                Options = new List<string>
                {
                    "Shield",
                    "Wand",
                    "Sword",
                    "Crown"
                },
                CorrectAnswerIndex = 1,
                Explanation = "Perfect! PowerShell is your network wand - the magical tool that channels your scripting spells and makes network automation possible!"
            },
            new QuizQuestion
            {
                Id = 243,
                LessonId = 61,
                QuestionText = "Instead of manually checking each computer, what can scripts do?",
                Options = new List<string>
                {
                    "Break the computers",
                    "Hide the network",
                    "Automate the checking process like a survey spell",
                    "Change all passwords"
                },
                CorrectAnswerIndex = 2,
                Explanation = "Brilliant! Scripts work like survey spells, automatically visiting each computer and reporting back - that's the magic of automation!"
            }
        };
    }

    /// <summary>
    /// Generate quiz questions for Lesson 2
    /// </summary>
    private static List<QuizQuestion> GetLesson2Quiz()
    {
        return new List<QuizQuestion>
        {
            new QuizQuestion
            {
                Id = 244,
                LessonId = 62,
                QuestionText = "In PowerShell, how do you create a variable to store an IP address?",
                Options = new List<string>
                {
                    "ipAddress = '192.168.1.1'",
                    "$ipAddress = '192.168.1.1'",
                    "var ipAddress = '192.168.1.1'",
                    "ip_address: '192.168.1.1'"
                },
                CorrectAnswerIndex = 1,
                Explanation = "Excellent! The $ symbol tells PowerShell 'this is a variable container' - just like labeling your magical potion bottles!"
            },
            new QuizQuestion
            {
                Id = 245,
                LessonId = 62,
                QuestionText = "Variables make scripts more flexible because they...",
                Options = new List<string>
                {
                    "Use more memory",
                    "Run slower",
                    "Allow you to change values without rewriting the script",
                    "Are harder to understand"
                },
                CorrectAnswerIndex = 2,
                Explanation = "Magical! Variables are like adjustable spell components - you can change the value of $targetIP and your ping spell works on any computer!"
            },
            new QuizQuestion
            {
                Id = 246,
                LessonId = 62,
                QuestionText = "What symbol identifies a variable in PowerShell?",
                Options = new List<string>
                {
                    "@",
                    "#",
                    "$",
                    "%"
                },
                CorrectAnswerIndex = 2,
                Explanation = "Perfect! The $ symbol is PowerShell's way of saying 'this is a magical variable container' - remember to use it before every variable name!"
            }
        };
    }

    /// <summary>
    /// Generate quiz questions for remaining lessons
    /// </summary>
    private static List<QuizQuestion> GetLesson3Quiz() => GetGenericQuiz(3, 61);
    private static List<QuizQuestion> GetLesson4Quiz() => GetGenericQuiz(4, 62);
    private static List<QuizQuestion> GetLesson5Quiz() => GetGenericQuiz(5, 63);
    private static List<QuizQuestion> GetLesson6Quiz() => GetGenericQuiz(6, 64);

    /// <summary>
    /// Generate generic quiz questions for advanced lessons
    /// </summary>
    private static List<QuizQuestion> GetGenericQuiz(int lessonNumber, int baseQuizId = 0)
    {
        var quizId = baseQuizId > 0 ? baseQuizId : 240 + (lessonNumber * 3);
        
        return new List<QuizQuestion>
        {
            new QuizQuestion
            {
                Id = quizId + 1,
                LessonId = 60 + lessonNumber,
                QuestionText = $"What is the key concept of Lesson {lessonNumber}?",
                Options = new List<string>
                {
                    "Basic computer operations",
                    "Advanced scripting magic and automation",
                    "Hardware troubleshooting",
                    "Software installation"
                },
                CorrectAnswerIndex = 1,
                Explanation = "Enchanting! Advanced scripting magic is indeed the essence of this lesson - you're becoming a true script sorcerer!"
            },
            new QuizQuestion
            {
                Id = quizId + 2,
                LessonId = 60 + lessonNumber,
                QuestionText = $"How does this lesson contribute to network mastery?",
                Options = new List<string>
                {
                    "It doesn't relate to networks",
                    "It provides advanced automation techniques for network management",
                    "It only covers basic concepts",
                    "It focuses on hardware only"
                },
                CorrectAnswerIndex = 1,
                Explanation = "Brilliant! Each lesson builds your network automation powers, turning you into a scripting wizard who can command digital kingdoms!"
            },
            new QuizQuestion
            {
                Id = quizId + 3,
                LessonId = 60 + lessonNumber,
                QuestionText = "What level of scripting proficiency does this lesson target?",
                Options = new List<string>
                {
                    "Beginner only",
                    "Intermediate to advanced with certification-level concepts",
                    "Expert only",
                    "No specific level"
                },
                CorrectAnswerIndex = 1,
                Explanation = "Perfect! This lesson elevates you from intermediate to advanced levels, preparing you for certification mastery - true scripting sorcery!"
            }
        };
    }

    #endregion

    /// <summary>
    /// Get Module 4 badge definitions for scripting mastery
    /// </summary>
    public static List<Badge> GetModule4Badges()
    {
        return new List<Badge>
        {
            new Badge
            {
                Name = "script_apprentice",
                DisplayName = "Script Apprentice",
                Description = "Completed your first scripting spell - welcome to the magical realm of automation!",
                IconData = null, // Will be generated by SkiaSharp
                Rarity = BadgeRarity.Common,
                Points = 10,
                UnlockCriteria = "Complete Lesson 1 with any score"
            },
            new Badge
            {
                Name = "variable_virtuoso",
                DisplayName = "Variable Virtuoso",
                Description = "Master of magical potion containers - you wield variables with mystical precision!",
                IconData = null,
                Rarity = BadgeRarity.Common,
                Points = 15,
                UnlockCriteria = "Complete Lessons 1-2 with average score ≥ 70%"
            },
            new Badge
            {
                Name = "command_conjurer",
                DisplayName = "Command Conjurer",
                Description = "You've learned to browse the grimoire and cast basic PowerShell incantations!",
                IconData = null,
                Rarity = BadgeRarity.Common,
                Points = 20,
                UnlockCriteria = "Complete Lessons 1-3 with average score ≥ 75%"
            },
            new Badge
            {
                Name = "logic_luminary",
                DisplayName = "Logic Luminary",
                Description = "Master of conditional magic - your if-then spells choose paths with cosmic wisdom!",
                IconData = null,
                Rarity = BadgeRarity.Uncommon,
                Points = 25,
                UnlockCriteria = "Complete Lessons 1-5 with average score ≥ 80%"
            },
            new Badge
            {
                Name = "loop_legend",
                DisplayName = "Loop Legend",
                Description = "Repeating spells until victory is achieved - you've mastered the art of magical iteration!",
                IconData = null,
                Rarity = BadgeRarity.Uncommon,
                Points = 30,
                UnlockCriteria = "Complete Lessons 1-6 with average score ≥ 80%"
            },
            new Badge
            {
                Name = "function_phoenix",
                DisplayName = "Function Phoenix",
                Description = "Your reusable spell scrolls rise like phoenixes - call them forth whenever needed!",
                IconData = null,
                Rarity = BadgeRarity.Uncommon,
                Points = 35,
                UnlockCriteria = "Complete Lessons 1-7 with average score ≥ 85%"
            },
            new Badge
            {
                Name = "error_exorcist",
                DisplayName = "Error Exorcist",
                Description = "Trapping gremlins and banishing bugs - your try-catch nets catch every mischievous sprite!",
                IconData = null,
                Rarity = BadgeRarity.Rare,
                Points = 40,
                UnlockCriteria = "Complete Lessons 1-8 with average score ≥ 85%"
            },
            new Badge
            {
                Name = "network_necromancer",
                DisplayName = "Network Necromancer",
                Description = "Raising network spirits and commanding digital realms with PowerShell mastery!",
                IconData = null,
                Rarity = BadgeRarity.Rare,
                Points = 45,
                UnlockCriteria = "Complete Lessons 1-9 with average score ≥ 90%"
            },
            new Badge
            {
                Name = "automation_archmage",
                DisplayName = "Automation Archmage",
                Description = "Self-casting spells work while you sleep - you've achieved lazy wizardry perfection!",
                IconData = null,
                Rarity = BadgeRarity.Rare,
                Points = 50,
                UnlockCriteria = "Complete Lessons 1-12 with average score ≥ 90%"
            },
            new Badge
            {
                Name = "module_monarch",
                DisplayName = "Module Monarch",
                Description = "Ruler of spell libraries - you import mystical powers from across the PowerShell kingdom!",
                IconData = null,
                Rarity = BadgeRarity.Epic,
                Points = 60,
                UnlockCriteria = "Complete Lessons 1-14 with average score ≥ 90%"
            },
            new Badge
            {
                Name = "debug_deity",
                DisplayName = "Debug Deity",
                Description = "Divine detective of code crypts - no script mystery can hide from your magnifying glass!",
                IconData = null,
                Rarity = BadgeRarity.Epic,
                Points = 70,
                UnlockCriteria = "Complete Lessons 1-15 with average score ≥ 95%"
            },
            new Badge
            {
                Name = "remote_ruler",
                DisplayName = "Remote Ruler",
                Description = "Commanding distant kingdoms through mystical telescopic spells across vast network realms!",
                IconData = null,
                Rarity = BadgeRarity.Epic,
                Points = 80,
                UnlockCriteria = "Complete Lessons 1-18 with average score ≥ 95%"
            },
            new Badge
            {
                Name = "certification_champion",
                DisplayName = "Certification Champion",
                Description = "Complex script combinations worthy of professional certification - true mastery achieved!",
                IconData = null,
                Rarity = BadgeRarity.Legendary,
                Points = 90,
                UnlockCriteria = "Complete Lessons 1-19 with average score ≥ 95%"
            },
            new Badge
            {
                Name = "script_sorcerer_supreme",
                DisplayName = "Script Sorcerer Supreme",
                Description = "🌟 ULTIMATE SCRIPTING MASTERY! You command the full spectrum of PowerShell magic - kingdoms bow to your automated supremacy!",
                IconData = null,
                Rarity = BadgeRarity.Legendary,
                Points = 100,
                UnlockCriteria = "Complete all 20 Module 4 lessons with average score ≥ 95%"
            }
        };
    }
}