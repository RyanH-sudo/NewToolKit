using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using NetToolkit.Modules.Education.Interfaces;

namespace NetToolkit.Modules.Education.Models;

/// <summary>
/// Education platform models - the transcendent architecture of digital enlightenment
/// Where knowledge transforms into gamified adventures and learning becomes cosmic exploration
/// </summary>

#region Core Education Models

/// <summary>
/// Education module - a comprehensive learning journey through network cosmos
/// </summary>
public class Module
{
    [Key]
    public int Id { get; set; }
    
    /// <summary>
    /// Module title with cosmic flair
    /// </summary>
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// Module description and learning objectives
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Learning difficulty level
    /// </summary>
    public DifficultyLevel Difficulty { get; set; }
    
    /// <summary>
    /// Estimated completion time in minutes
    /// </summary>
    public int EstimatedMinutes { get; set; }
    
    /// <summary>
    /// Module icon/image data
    /// </summary>
    public byte[]? IconData { get; set; }
    
    /// <summary>
    /// Prerequisites for this module
    /// </summary>
    public string Prerequisites { get; set; } = string.Empty;
    
    /// <summary>
    /// Learning outcomes
    /// </summary>
    public string LearningOutcomes { get; set; } = string.Empty;
    
    /// <summary>
    /// Module category (INFERRED: for grouping modules)
    /// </summary>
    public string Category { get; set; } = string.Empty;
    
    /// <summary>
    /// Module learning objectives (INFERRED: detailed objectives list)
    /// </summary>
    public List<string> LearningObjectives { get; set; } = new List<string>();
    
    /// <summary>
    /// Module tags (INFERRED: for search and filtering)
    /// </summary>
    public List<string> Tags { get; set; } = new List<string>();
    
    /// <summary>
    /// Creation timestamp
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Module lessons
    /// </summary>
    public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
    
    /// <summary>
    /// Module progress records
    /// </summary>
    public virtual ICollection<ModuleProgress> Progress { get; set; } = new List<ModuleProgress>();
    
    /// <summary>
    /// Get witty module summary
    /// </summary>
    public string GetWittySummary()
    {
        var difficultyDesc = Difficulty switch
        {
            DifficultyLevel.Beginner => "Like learning your ABCs, but for networks!",
            DifficultyLevel.Intermediate => "Time to level up your networking ninja skills!",
            DifficultyLevel.Advanced => "For those who dream in binary and speak in packets!",
            DifficultyLevel.Expert => "Prepare for network enlightenment - certification mastery awaits!",
            _ => "A mysterious journey through digital dimensions!"
        };
        
        return $"🎓 {Title}: {difficultyDesc} (Est. {EstimatedMinutes} mins of pure learning magic)";
    }
}

/// <summary>
/// Individual lesson within a module
/// </summary>
public class Lesson
{
    [Key]
    public int Id { get; set; }
    
    /// <summary>
    /// Parent module ID
    /// </summary>
    public int ModuleId { get; set; }
    
    /// <summary>
    /// Lesson number within module (1-20)
    /// </summary>
    public int LessonNumber { get; set; }
    
    /// <summary>
    /// Lesson title
    /// </summary>
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// Brief lesson description
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Lesson content as JSON-serialized slides
    /// </summary>
    public string ContentJson { get; set; } = string.Empty;
    
    /// <summary>
    /// Learning objectives for this lesson
    /// </summary>
    public string Objectives { get; set; } = string.Empty;
    
    /// <summary>
    /// Required score to pass (percentage)
    /// </summary>
    public int PassingScore { get; set; } = 70;
    
    /// <summary>
    /// Estimated time to complete in minutes
    /// </summary>
    public int EstimatedMinutes { get; set; } = 5;
    
    /// <summary>
    /// Lesson order/sequence (INFERRED: same as LessonNumber for compatibility)
    /// </summary>
    public int Order => LessonNumber;
    
    /// <summary>
    /// Lesson learning objectives (INFERRED: detailed objectives list)
    /// </summary>
    public List<string> LearningObjectives { get; set; } = new List<string>();
    
    /// <summary>
    /// Lesson content as list of strings (INFERRED: for content access, renamed for EF compatibility)
    /// </summary>
    public List<string> ContentStrings { get; set; } = new List<string>();
    
    /// <summary>
    /// Quiz questions as list of strings (INFERRED: simplified quiz access, renamed for compatibility)
    /// </summary>
    public List<string> QuizQuestionStrings { get; set; } = new List<string>();
    
    /// <summary>
    /// Lesson difficulty level (INFERRED: individual lesson difficulty)
    /// </summary>
    public string Difficulty { get; set; } = "Intermediate";
    
    /// <summary>
    /// Parent module
    /// </summary>
    public virtual Module Module { get; set; } = null!;
    
    /// <summary>
    /// Lesson progress records
    /// </summary>
    public virtual ICollection<LessonProgress> Progress { get; set; } = new List<LessonProgress>();
    
    /// <summary>
    /// Lesson content collection (INFERRED: EF Core navigation for HasMany relationship)
    /// </summary>
    public virtual ICollection<LessonContent> Content { get; set; } = new List<LessonContent>();
    
    /// <summary>
    /// Quiz questions collection (INFERRED: EF Core navigation for HasMany relationship)
    /// </summary>
    public virtual ICollection<QuizQuestion> QuizQuestions { get; set; } = new List<QuizQuestion>();
    
    /// <summary>
    /// Deserialize lesson content
    /// </summary>
    public LessonContent GetContent()
    {
        try
        {
            return JsonConvert.DeserializeObject<LessonContent>(ContentJson) ?? new LessonContent();
        }
        catch
        {
            return new LessonContent();
        }
    }
    
    /// <summary>
    /// Serialize lesson content
    /// </summary>
    public void SetContent(LessonContent content)
    {
        ContentJson = JsonConvert.SerializeObject(content, Formatting.Indented);
    }
    
    /// <summary>
    /// Get witty lesson preview
    /// </summary>
    public string GetWittyPreview()
    {
        return $"📚 Lesson {LessonNumber}: {Title} - {Description} (⏱️ ~{EstimatedMinutes} mins of enlightenment!)";
    }
}

/// <summary>
/// Lesson content structure with slides
/// </summary>
public class LessonContent
{
    /// <summary>
    /// Unique content identifier (INFERRED: for content management)
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    /// <summary>
    /// Content title (INFERRED: for content organization)
    /// </summary>
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// Content data/body (INFERRED: main content)
    /// </summary>
    public string Data { get; set; } = string.Empty;
    
    /// <summary>
    /// Content description (INFERRED: content summary)
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Content type (INFERRED: for content categorization)
    /// </summary>
    public string Type { get; set; } = "Lesson";
    
    /// <summary>
    /// Content order (INFERRED: for sequencing)
    /// </summary>
    public int Order { get; set; } = 0;
    
    /// <summary>
    /// Lesson slides in order
    /// </summary>
    public List<Slide> Slides { get; set; } = new();
    
    /// <summary>
    /// Hover tips scattered throughout lesson
    /// </summary>
    public List<HoverTip> Tips { get; set; } = new();
    
    /// <summary>
    /// Lesson ID for foreign key relationship (INFERRED: EF Core navigation)
    /// </summary>
    public int LessonId { get; set; }
    
    /// <summary>
    /// Parent lesson navigation property (INFERRED: EF Core relationship)
    /// </summary>
    public virtual Lesson Lesson { get; set; } = null!;
    
    /// <summary>
    /// Hover tips property alias (INFERRED: for compatibility)
    /// </summary>
    public List<HoverTip> HoverTips => Tips;
    
    /// <summary>
    /// Quiz questions for this lesson
    /// </summary>
    public List<QuizQuestion> Quiz { get; set; } = new();
    
    /// <summary>
    /// Quiz questions property alias (INFERRED: for compatibility)
    /// </summary>
    public List<QuizQuestion> QuizQuestions => Quiz;
    
    /// <summary>
    /// Additional resources
    /// </summary>
    public List<Resource> Resources { get; set; } = new();
}

/// <summary>
/// Individual slide within a lesson
/// </summary>
public class Slide
{
    /// <summary>
    /// Unique slide identifier
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    /// <summary>
    /// Type of slide content
    /// </summary>
    public SlideType Type { get; set; }
    
    /// <summary>
    /// Slide title
    /// </summary>
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// Main slide content
    /// </summary>
    public string Content { get; set; } = string.Empty;
    
    /// <summary>
    /// Image data for image slides (base64 or file path)
    /// </summary>
    public string? ImageData { get; set; }
    
    /// <summary>
    /// Animation instructions for Three.js
    /// </summary>
    public string? AnimationData { get; set; }
    
    /// <summary>
    /// Slide-specific hover tips
    /// </summary>
    public List<string> HoverTips { get; set; } = new();
    
    /// <summary>
    /// Speaker notes for educators
    /// </summary>
    public string? SpeakerNotes { get; set; }
    
    /// <summary>
    /// Slide order/sequence (INFERRED: for slide ordering)
    /// </summary>
    public int Order { get; set; } = 0;
    
    /// <summary>
    /// Slide metadata (INFERRED: additional slide data)
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();
}

/// <summary>
/// Hover tip for interactive learning
/// </summary>
public class HoverTip
{
    /// <summary>
    /// Unique hover tip identifier (INFERRED: EF Core primary key)
    /// </summary>
    [Key]
    public int Id { get; set; }
    
    /// <summary>
    /// Element identifier to attach tip to
    /// </summary>
    public string ElementId { get; set; } = string.Empty;
    
    /// <summary>
    /// Witty tip text
    /// </summary>
    public string TipText { get; set; } = string.Empty;
    
    /// <summary>
    /// Tip category for theming
    /// </summary>
    public TipCategory Category { get; set; }
    
    
    /// <summary>
    /// Position relative to element
    /// </summary>
    public TipPosition Position { get; set; } = TipPosition.Top;
    
    /// <summary>
    /// Word/term being explained (INFERRED: key term)
    /// </summary>
    public string Word { get; set; } = string.Empty;
    
    /// <summary>
    /// Detailed explanation (INFERRED: expanded explanation)
    /// </summary>
    public string Explanation { get; set; } = string.Empty;
    
    /// <summary>
    /// Lesson content ID for foreign key relationship (INFERRED: EF Core navigation)
    /// </summary>
    public string? LessonContentId { get; set; }
    
    /// <summary>
    /// Parent lesson content navigation property (INFERRED: EF Core relationship)
    /// </summary>
    public virtual LessonContent? LessonContent { get; set; }
}

/// <summary>
/// Quiz question for lesson assessment
/// </summary>
public class QuizQuestion
{
    /// <summary>
    /// Unique question identifier
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    /// <summary>
    /// Question text
    /// </summary>
    public string Question { get; set; } = string.Empty;
    
    /// <summary>
    /// Multiple choice options
    /// </summary>
    public List<string> Options { get; set; } = new();
    
    /// <summary>
    /// Index of correct answer (0-based)
    /// </summary>
    public int CorrectAnswerIndex { get; set; }
    
    /// <summary>
    /// Explanation for correct answer
    /// </summary>
    public string Explanation { get; set; } = string.Empty;
    
    /// <summary>
    /// Witty feedback for incorrect answers
    /// </summary>
    public List<string> IncorrectFeedback { get; set; } = new();
    
    /// <summary>
    /// Question difficulty level
    /// </summary>
    public QuestionDifficulty Difficulty { get; set; } = QuestionDifficulty.Easy;
    
    /// <summary>
    /// Points awarded for correct answer
    /// </summary>
    public int Points { get; set; } = 10;
    
    /// <summary>
    /// Get witty feedback for answer
    /// </summary>
    public string GetFeedback(int selectedIndex)
    {
        if (selectedIndex == CorrectAnswerIndex)
        {
            return $"🎉 Correct! {Explanation}";
        }
        
        if (selectedIndex >= 0 && selectedIndex < IncorrectFeedback.Count)
        {
            return $"🤔 {IncorrectFeedback[selectedIndex]}";
        }
        
        return "🤯 Oops! That's like confusing WiFi with witchcraft - close, but no spell!";
    }
    
    /// <summary>
    /// Wrong answer feedback (INFERRED: alias for IncorrectFeedback compatibility)
    /// </summary>
    public List<string> WrongAnswerFeedback => IncorrectFeedback;
    
    /// <summary>
    /// Question tags for categorization (INFERRED: question taxonomy)
    /// </summary>
    public List<string> Tags { get; set; } = new List<string>();
    
    /// <summary>
    /// Lesson ID for foreign key relationship (INFERRED: EF Core navigation)
    /// </summary>
    public int LessonId { get; set; }
    
    /// <summary>
    /// Parent lesson navigation property (INFERRED: EF Core relationship)
    /// </summary>
    public virtual Lesson Lesson { get; set; } = null!;
    
    /// <summary>
    /// Lesson content ID for foreign key relationship (INFERRED: EF Core navigation)
    /// </summary>
    public string? LessonContentId { get; set; }
    
    /// <summary>
    /// Parent lesson content navigation property (INFERRED: EF Core relationship)
    /// </summary>
    public virtual LessonContent? LessonContent { get; set; }
}

/// <summary>
/// Educational resource reference
/// </summary>
public class Resource
{
    /// <summary>
    /// Resource title
    /// </summary>
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// Resource URL or path
    /// </summary>
    public string Url { get; set; } = string.Empty;
    
    /// <summary>
    /// Resource type
    /// </summary>
    public ResourceType Type { get; set; }
    
    /// <summary>
    /// Brief description
    /// </summary>
    public string Description { get; set; } = string.Empty;
}

#endregion

#region Progress and Gamification Models

/// <summary>
/// User module progress tracking alias for compatibility
/// </summary>
public class UserModuleProgress : ModuleProgress
{
}

/// <summary>
/// User progress through a module
/// </summary>
public class ModuleProgress
{
    [Key]
    public int Id { get; set; }
    
    /// <summary>
    /// User identifier
    /// </summary>
    public string UserId { get; set; } = string.Empty;
    
    /// <summary>
    /// Module ID
    /// </summary>
    public int ModuleId { get; set; }
    
    /// <summary>
    /// Module start timestamp
    /// </summary>
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Module completion timestamp
    /// </summary>
    public DateTime? CompletedAt { get; set; }
    
    /// <summary>
    /// Overall module score (percentage)
    /// </summary>
    public double OverallScore { get; set; }
    
    /// <summary>
    /// Last accessed timestamp (INFERRED: progress tracking)
    /// </summary>
    public DateTime LastAccessedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Current lesson being studied
    /// </summary>
    public int CurrentLesson { get; set; } = 1;
    
    /// <summary>
    /// Total time spent in minutes
    /// </summary>
    public int TotalTimeMinutes { get; set; }
    
    /// <summary>
    /// Consecutive days studying (streak)
    /// </summary>
    public int Streak { get; set; }
    
    /// <summary>
    /// Last activity date for streak calculation
    /// </summary>
    public DateTime LastActivityDate { get; set; } = DateTime.UtcNow.Date;
    
    /// <summary>
    /// Module completion status
    /// </summary>
    public CompletionStatus Status { get; set; } = CompletionStatus.NotStarted;
    
    /// <summary>
    /// Average score across all lessons (INFERRED: calculated from lesson progress)
    /// </summary>
    public double AverageScore => LessonProgress.Any() ? LessonProgress.Average(lp => lp.Score) : 0.0;
    
    /// <summary>
    /// Total lessons in module (INFERRED: calculated from module, converted to settable)
    /// </summary>
    public int TotalLessons { get; set; }
    
    /// <summary>
    /// Current lesson ID (INFERRED: derived from CurrentLesson, converted to settable)
    /// </summary>
    public int CurrentLessonId { get; set; }
    
    /// <summary>
    /// Completed lessons count (INFERRED: count of completed lesson progress, converted to settable)
    /// </summary>
    public int CompletedLessons { get; set; }
    
    /// <summary>
    /// Lesson progress collection (INFERRED: alias for LessonProgress, converted to settable)
    /// </summary>
    public ICollection<LessonProgress> LessonProgresses { get; set; } = new List<LessonProgress>();
    
    /// <summary>
    /// Parent module
    /// </summary>
    public virtual Module Module { get; set; } = null!;
    
    /// <summary>
    /// Lesson progress records
    /// </summary>
    public virtual ICollection<LessonProgress> LessonProgress { get; set; } = new List<LessonProgress>();
    
    /// <summary>
    /// Earned badges
    /// </summary>
    public virtual ICollection<Badge> Badges { get; set; } = new List<Badge>();
    
    /// <summary>
    /// Completion percentage property (INFERRED: computed property for easy access)
    /// </summary>
    public double CompletionPercentage => GetCompletionPercentage();
    
    /// <summary>
    /// Calculate completion percentage
    /// </summary>
    public double GetCompletionPercentage()
    {
        if (!LessonProgress.Any()) return 0;
        
        var completedLessons = LessonProgress.Count(lp => lp.IsCompleted);
        var totalLessons = Module?.Lessons.Count ?? 20;
        
        return Math.Round((double)completedLessons / totalLessons * 100, 2);
    }
    
    /// <summary>
    /// Get witty progress summary
    /// </summary>
    public string GetProgressSummary()
    {
        var completion = GetCompletionPercentage();
        var statusEmoji = completion switch
        {
            >= 100 => "🏆",
            >= 80 => "🎯",
            >= 60 => "📈",
            >= 40 => "🌟",
            >= 20 => "🚀",
            _ => "🌱"
        };
        
        var motivation = completion switch
        {
            >= 100 => "Module mastered! You're a networking wizard! 🧙‍♀️",
            >= 80 => "Almost there! The finish line is calling your name!",
            >= 60 => "Great momentum! Keep up the excellent work!",
            >= 40 => "You're getting the hang of it! Networks, beware!",
            >= 20 => "Nice start! Your learning journey has begun!",
            _ => "Ready to dive into the network universe? Let's go!"
        };
        
        return $"{statusEmoji} {completion:F0}% Complete - {motivation}";
    }
}

/// <summary>
/// User progress through individual lesson
/// </summary>
public class LessonProgress
{
    [Key]
    public int Id { get; set; }
    
    /// <summary>
    /// User identifier
    /// </summary>
    public string UserId { get; set; } = string.Empty;
    
    /// <summary>
    /// Lesson ID
    /// </summary>
    public int LessonId { get; set; }
    
    /// <summary>
    /// Module progress ID
    /// </summary>
    public int ModuleProgressId { get; set; }
    
    /// <summary>
    /// Lesson start timestamp
    /// </summary>
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Lesson completion timestamp
    /// </summary>
    public DateTime? CompletedAt { get; set; }
    
    /// <summary>
    /// Quiz score (percentage)
    /// </summary>
    public double QuizScore { get; set; }
    
    /// <summary>
    /// Quiz feedback text (INFERRED: EF Core configuration expectation)
    /// </summary>
    public string QuizFeedback { get; set; } = string.Empty;
    
    /// <summary>
    /// Score alias for QuizScore (INFERRED: compatibility property for progress calculations)
    /// </summary>
    public double Score => QuizScore;
    
    /// <summary>
    /// Number of quiz attempts
    /// </summary>
    public int Attempts { get; set; } = 1;
    
    /// <summary>
    /// Time spent on lesson (minutes)
    /// </summary>
    public int TimeSpentMinutes { get; set; }
    
    /// <summary>
    /// Current slide index
    /// </summary>
    public int CurrentSlide { get; set; }
    
    /// <summary>
    /// Last accessed timestamp (INFERRED: progress tracking)
    /// </summary>
    public DateTime LastAccessedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Current slide index (INFERRED: alias for CurrentSlide, converted to settable)
    /// </summary>
    public int CurrentSlideIndex { get; set; }
    
    /// <summary>
    /// Current lesson status
    /// </summary>
    public LessonStatus Status { get; set; } = LessonStatus.NotStarted;
    
    /// <summary>
    /// Whether lesson is completed
    /// </summary>
    public bool IsCompleted { get; set; }
    
    /// <summary>
    /// Quiz answers as JSON
    /// </summary>
    public string? QuizAnswersJson { get; set; }
    
    /// <summary>
    /// Parent lesson
    /// </summary>
    public virtual Lesson Lesson { get; set; } = null!;
    
    /// <summary>
    /// Parent module progress
    /// </summary>
    public virtual ModuleProgress ModuleProgress { get; set; } = null!;
    
    /// <summary>
    /// Get quiz answers
    /// </summary>
    public Dictionary<string, int> GetQuizAnswers()
    {
        if (string.IsNullOrEmpty(QuizAnswersJson))
            return new Dictionary<string, int>();
        
        try
        {
            return JsonConvert.DeserializeObject<Dictionary<string, int>>(QuizAnswersJson) ?? new Dictionary<string, int>();
        }
        catch
        {
            return new Dictionary<string, int>();
        }
    }
    
    /// <summary>
    /// Set quiz answers
    /// </summary>
    public void SetQuizAnswers(Dictionary<string, int> answers)
    {
        QuizAnswersJson = JsonConvert.SerializeObject(answers);
    }
    
    /// <summary>
    /// Get witty performance summary
    /// </summary>
    public string GetPerformanceSummary()
    {
        if (!IsCompleted)
            return "🌟 In progress - knowledge is brewing!";
        
        return QuizScore switch
        {
            >= 95 => "🏆 Perfect! You're a networking genius!",
            >= 85 => "🎉 Excellent work! Networks bow to your wisdom!",
            >= 75 => "👍 Great job! You're mastering the digital realm!",
            >= 65 => "📚 Good progress! Keep building that knowledge!",
            >= 50 => "🔄 Getting there! Practice makes perfect!",
            _ => "💪 Don't give up! Every expert was once a beginner!"
        };
    }
}

/// <summary>
/// Achievement badge for gamification
/// </summary>
public class Badge
{
    [Key]
    public int Id { get; set; }
    
    /// <summary>
    /// Badge identifier
    /// </summary>
    public string BadgeId { get; set; } = string.Empty;
    
    /// <summary>
    /// Badge name for display
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Badge title
    /// </summary>
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// Badge description
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Badge icon file path
    /// </summary>
    public string? IconPath { get; set; }
    
    /// <summary>
    /// Badge icon data
    /// </summary>
    public byte[]? IconData { get; set; }
    
    /// <summary>
    /// Achievement requirements description
    /// </summary>
    public string? Requirements { get; set; }
    
    /// <summary>
    /// Reward message when badge is earned
    /// </summary>
    public string? RewardMessage { get; set; }
    
    /// <summary>
    /// Associated module ID
    /// </summary>
    public int? ModuleId { get; set; }
    
    /// <summary>
    /// Badge category
    /// </summary>
    public BadgeCategory Category { get; set; }
    
    /// <summary>
    /// Badge rarity level
    /// </summary>
    public BadgeRarity Rarity { get; set; }
    
    /// <summary>
    /// Creation timestamp
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Collection of user badges (users who earned this badge)
    /// </summary>
    public virtual ICollection<UserBadge> UserBadges { get; set; } = new List<UserBadge>();
    
    /// <summary>
    /// Parent module (optional)
    /// </summary>
    public virtual Module? Module { get; set; }
    
    /// <summary>
    /// Get witty badge announcement
    /// </summary>
    public string GetAchievementMessage()
    {
        var rarityEmoji = Rarity switch
        {
            BadgeRarity.Common => "⭐",
            BadgeRarity.Uncommon => "🌟",
            BadgeRarity.Rare => "💫",
            BadgeRarity.Epic => "🏆",
            BadgeRarity.Legendary => "👑",
            _ => "🎖️"
        };
        
        return $"{rarityEmoji} Badge Unlocked: {Title}! {Description}";
    }
}

/// <summary>
/// User badge relationship - tracks which badges users have earned
/// </summary>
public class UserBadge
{
    [Key]
    public int Id { get; set; }
    
    /// <summary>
    /// User identifier who earned the badge
    /// </summary>
    public string UserId { get; set; } = string.Empty;
    
    /// <summary>
    /// Badge ID that was earned
    /// </summary>
    public int BadgeId { get; set; }
    
    /// <summary>
    /// When the badge was awarded
    /// </summary>
    public DateTime AwardedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Navigation property to the badge
    /// </summary>
    public virtual Badge? Badge { get; set; }
}

/// <summary>
/// User streak tracking for gamification (INFERRED: learning consistency tracking)
/// </summary>
public class UserStreak
{
    [Key]
    public int Id { get; set; }
    
    /// <summary>
    /// User identifier
    /// </summary>
    public string UserId { get; set; } = string.Empty;
    
    /// <summary>
    /// Current streak count (days)
    /// </summary>
    public int CurrentStreak { get; set; }
    
    /// <summary>
    /// Longest streak achieved
    /// </summary>
    public int LongestStreak { get; set; }
    
    /// <summary>
    /// Last activity date
    /// </summary>
    public DateTime LastActivityDate { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Streak start date
    /// </summary>
    public DateTime StreakStartDate { get; set; } = DateTime.UtcNow;
}

#endregion

#region Enumerations

/// <summary>
/// Module difficulty levels
/// </summary>
public enum DifficultyLevel
{
    Beginner,
    Intermediate,
    Advanced,
    Expert
}

/// <summary>
/// Slide content types
/// </summary>
public enum SlideType
{
    Introduction,
    Image,
    Text,
    Interactive,
    Quiz,
    Summary,
    ThreeJS
}

/// <summary>
/// Hover tip categories
/// </summary>
public enum TipCategory
{
    Information,
    Warning,
    Fun,
    Technical,
    Historical
}

/// <summary>
/// Tip positioning
/// </summary>
public enum TipPosition
{
    Top,
    Bottom,
    Left,
    Right,
    Center
}

/// <summary>
/// Quiz question difficulty
/// </summary>
public enum QuestionDifficulty
{
    Easy,
    Medium,
    Hard,
    Expert
}

/// <summary>
/// Resource types
/// </summary>
public enum ResourceType
{
    Documentation,
    Video,
    Article,
    Tool,
    Reference,
    Practice
}

/// <summary>
/// Progress completion status
/// </summary>
public enum CompletionStatus
{
    NotStarted,
    InProgress,
    Completed,
    Mastered
}

/// <summary>
/// Lesson status enumeration for tracking individual lesson progress
/// </summary>
public enum LessonStatus
{
    NotStarted,
    InProgress,
    Completed,
    Mastered
}

/// <summary>
/// Badge categories
/// </summary>
public enum BadgeCategory
{
    Progress,
    Performance,
    Streak,
    Special,
    Mastery
}

/// <summary>
/// Badge rarity levels
/// </summary>
public enum BadgeRarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}

#endregion

#region Event Models

/// <summary>
/// Education event base class
/// </summary>
public abstract class EducationEventBase
{
    public string UserId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public Dictionary<string, object> Properties { get; set; } = new();
}

/// <summary>
/// Module started event
/// </summary>
public class ModuleStartedEvent : EducationEventBase
{
    public int ModuleId { get; set; }
    public string ModuleTitle { get; set; } = string.Empty;
    public DifficultyLevel Difficulty { get; set; }
    
    // INFERRED: Missing properties for module started event
    public string ModuleName { get; set; } = string.Empty; // Alias for consistency, converted to settable
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public int TotalLessons { get; set; } = 0; // To be set when creating the event
}

/// <summary>
/// User-specific lesson progress tracking with detailed analytics
/// Bridges user activity with lesson completion metrics for cosmic learning insights
/// </summary>
public class UserLessonProgress
{
    [Key]
    public int Id { get; init; }
    
    /// <summary>
    /// User identifier - the cosmic learner's unique signature
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string UserId { get; init; } = string.Empty;
    
    /// <summary>
    /// Associated lesson ID
    /// </summary>
    public int LessonId { get; init; }
    
    /// <summary>
    /// Module ID for efficient querying
    /// </summary>
    public int ModuleId { get; init; }
    
    /// <summary>
    /// Current progress status
    /// </summary>
    public LessonStatus Status { get; set; } = LessonStatus.NotStarted;
    
    /// <summary>
    /// Quiz score achieved (0-100)
    /// </summary>
    public double? QuizScore { get; set; }
    
    /// <summary>
    /// Number of attempts made
    /// </summary>
    public int Attempts { get; set; } = 0;
    
    /// <summary>
    /// Total time spent in minutes
    /// </summary>
    public int TimeSpentMinutes { get; set; } = 0;
    
    /// <summary>
    /// When the user first started this lesson
    /// </summary>
    public DateTime StartedAt { get; init; } = DateTime.UtcNow;
    
    /// <summary>
    /// When the lesson was completed (if applicable)
    /// </summary>
    public DateTime? CompletedAt { get; set; }
    
    /// <summary>
    /// Last activity timestamp for session tracking
    /// </summary>
    public DateTime LastActivityAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Navigation property to lesson
    /// </summary>
    public virtual Lesson? Lesson { get; set; }
    
    /// <summary>
    /// Navigation property to module
    /// </summary>
    public virtual Module? Module { get; set; }
    
    /// <summary>
    /// Get witty progress summary with cosmic flair
    /// </summary>
    public string GetProgressSummary()
    {
        return Status switch
        {
            LessonStatus.NotStarted => "🌟 Ready to embark on this cosmic journey!",
            LessonStatus.InProgress => $"🚀 {TimeSpentMinutes} minutes into the adventure (Attempt #{Attempts + 1})",
            LessonStatus.Completed => $"🎓 Mastered with {QuizScore:F1}% stellar performance in {Attempts} attempts!",
            _ => "🌌 Floating in the digital cosmos..."
        };
    }
    
    /// <summary>
    /// Check if the lesson is completed
    /// </summary>
    public bool IsCompleted => Status == LessonStatus.Completed;
}

/// <summary>
/// Lesson completed event
/// </summary>
public class LessonCompletedEvent : EducationEventBase
{
    public int LessonId { get; set; }
    public int ModuleId { get; set; }
    public string LessonTitle { get; set; } = string.Empty;
    public double Score { get; set; }
    public int Attempts { get; set; }
    public int TimeSpentMinutes { get; set; }
    
    /// <summary>
    /// Quiz score (INFERRED: alias for Score for compatibility, converted to settable)
    /// </summary>
    public double QuizScore { get; set; }
    
    /// <summary>
    /// Badges awarded for completion (INFERRED: achievement tracking)
    /// </summary>
    public List<string> BadgesAwarded { get; set; } = new List<string>();
    
    /// <summary>
    /// Completion timestamp (INFERRED: lesson completion tracking)
    /// </summary>
    public DateTime CompletedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Quiz graded event
/// </summary>
public class QuizGradedEvent : EducationEventBase
{
    public int LessonId { get; set; }
    public double Score { get; set; }
    public int CorrectAnswers { get; set; }
    public int TotalQuestions { get; set; }
    public bool Passed { get; set; }
    public int Attempts { get; set; }
}

/// <summary>
/// Badge unlocked event
/// </summary>
public class BadgeUnlockedEvent : EducationEventBase
{
    public string BadgeId { get; set; } = string.Empty;
    public string BadgeTitle { get; set; } = string.Empty;
    public BadgeCategory Category { get; set; }
    public BadgeRarity Rarity { get; set; }
    public string Criteria { get; set; } = string.Empty;
    
    /// <summary>
    /// Badge name (INFERRED: alias for BadgeTitle for compatibility, converted to settable)
    /// </summary>
    public string BadgeName { get; set; } = string.Empty;
    
    /// <summary>
    /// Badge description (INFERRED: detailed description of badge achievement)
    /// </summary>
    public string BadgeDescription { get; set; } = string.Empty;
    
    /// <summary>
    /// When badge was awarded (INFERRED: timestamp for badge unlock event)
    /// </summary>
    public DateTime AwardedAt { get; set; }
}

/// <summary>
/// Tip hovered event
/// </summary>
public class TipHoveredEvent : EducationEventBase
{
    public string ElementId { get; set; } = string.Empty;
    public string TipText { get; set; } = string.Empty;
    public TipCategory Category { get; set; }
    public int LessonId { get; set; }
}

/// <summary>
/// Quiz passed event
/// </summary>
public class QuizPassedEvent : EducationEventBase
{
    public int LessonId { get; set; }
    public int ModuleId { get; set; }
    public double Score { get; set; }
    public bool Passed { get; set; }
    public string LessonTitle { get; set; } = string.Empty;
}

/// <summary>
/// User engagement profile for analytics and personalization (INFERRED: user analytics entity)
/// </summary>
public class UserEngagementProfile
{
    [Key]
    public int Id { get; set; }
    
    /// <summary>
    /// User identifier
    /// </summary>
    public string UserId { get; set; } = string.Empty;
    
    /// <summary>
    /// Total time spent learning (in minutes)
    /// </summary>
    public int TotalLearningTimeMinutes { get; set; }
    
    /// <summary>
    /// Preferred learning times of day
    /// </summary>
    public List<int> PreferredLearningHours { get; set; } = new();
    
    /// <summary>
    /// Engagement score (0-100)
    /// </summary>
    public double EngagementScore { get; set; }
    
    /// <summary>
    /// Learning style preferences
    /// </summary>
    public List<string> LearningStylePreferences { get; set; } = new();
    
    /// <summary>
    /// Last activity timestamp
    /// </summary>
    public DateTime LastActivityAt { get; set; }
    
    /// <summary>
    /// User engagement level (INFERRED: for gamification)
    /// </summary>
    public EngagementLevel Level { get; set; }
    
    /// <summary>
    /// Preferred features as JSON (INFERRED: feature preferences)
    /// </summary>
    public string PreferredFeatures { get; set; } = "{}";
    
    /// <summary>
    /// Average session duration in minutes (INFERRED: analytics metric)
    /// </summary>
    public double AverageSessionMinutes { get; set; }
    
    /// <summary>
    /// Attention score for gamification (INFERRED: duplicate of EngagementScore for compatibility)
    /// </summary>
    public double AttentionScore { get; set; }
    
    /// <summary>
    /// Total activities count (INFERRED: activity counter)
    /// </summary>
    public int TotalActivities { get; set; }
    
    /// <summary>
    /// Last activity date for compatibility (INFERRED: alias for LastActivityAt)
    /// </summary>
    public DateTime LastActivity { get; set; }
}

#endregion