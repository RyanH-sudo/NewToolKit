using NetToolkit.Modules.Education.Models;

namespace NetToolkit.Modules.Education.Interfaces;

/// <summary>
/// Education service interface - the omniscient orchestrator of digital enlightenment
/// Where learning journeys unfold with cosmic precision and knowledge transforms minds
/// </summary>
public interface IEducationService
{
    /// <summary>
    /// Load module content with all lessons and slides
    /// </summary>
    /// <param name="moduleId">Module identifier to load</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Complete module content structure</returns>
    Task<Module?> LoadModuleAsync(int moduleId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get all available modules
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of available learning modules</returns>
    Task<List<Module>> GetAllModulesAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get user's progress across all modules
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Comprehensive progress data</returns>
    Task<List<ModuleProgress>> GetProgressAsync(string userId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get user's progress for specific module
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="moduleId">Module identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Module-specific progress data</returns>
    Task<ModuleProgress?> GetModuleProgressAsync(string userId, int moduleId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Start a new module for user
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="moduleId">Module to start</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created module progress</returns>
    Task<ModuleProgress> StartModuleAsync(string userId, int moduleId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Advance to next lesson and handle quiz scoring
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="lessonId">Lesson identifier</param>
    /// <param name="quizAnswers">Quiz answers if applicable</param>
    /// <param name="timeSpentMinutes">Time spent on lesson</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated lesson progress and any awarded badges</returns>
    Task<(LessonProgress Progress, List<Badge> NewBadges)> CompleteLessonAsync(
        string userId, 
        int lessonId, 
        Dictionary<string, int>? quizAnswers = null,
        int timeSpentMinutes = 0,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get user's earned badges
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="moduleId">Optional module filter</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of earned badges</returns>
    Task<List<Badge>> GetUserBadgesAsync(string userId, int? moduleId = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get detailed lesson content with slides
    /// </summary>
    /// <param name="lessonId">Lesson identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Complete lesson content</returns>
    Task<Lesson?> GetLessonAsync(int lessonId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Update user's current position in lesson
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="lessonId">Lesson identifier</param>
    /// <param name="slideIndex">Current slide index</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated lesson progress</returns>
    Task<LessonProgress> UpdateLessonPositionAsync(
        string userId, 
        int lessonId, 
        int slideIndex, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get learning statistics for user
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Comprehensive learning statistics</returns>
    Task<LearningStatistics> GetLearningStatisticsAsync(string userId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Reset user progress for module (for retaking)
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="moduleId">Module to reset</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status</returns>
    Task<bool> ResetModuleProgressAsync(string userId, int moduleId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Search lessons by content or title
    /// </summary>
    /// <param name="searchTerm">Search term</param>
    /// <param name="moduleId">Optional module filter</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Matching lessons</returns>
    Task<List<Lesson>> SearchLessonsAsync(string searchTerm, int? moduleId = null, CancellationToken cancellationToken = default);
}

/// <summary>
/// Learning statistics summary
/// </summary>
public class LearningStatistics
{
    /// <summary>
    /// Total modules started
    /// </summary>
    public int ModulesStarted { get; set; }
    
    /// <summary>
    /// Total modules completed
    /// </summary>
    public int ModulesCompleted { get; set; }
    
    /// <summary>
    /// Total lessons completed
    /// </summary>
    public int LessonsCompleted { get; set; }
    
    /// <summary>
    /// Total time spent learning (minutes)
    /// </summary>
    public int TotalTimeMinutes { get; set; }
    
    /// <summary>
    /// Average quiz score across all lessons
    /// </summary>
    public double AverageScore { get; set; }
    
    /// <summary>
    /// Current learning streak (consecutive days)
    /// </summary>
    public int CurrentStreak { get; set; }
    
    /// <summary>
    /// Longest learning streak achieved
    /// </summary>
    public int LongestStreak { get; set; }
    
    /// <summary>
    /// Total badges earned
    /// </summary>
    public int BadgesEarned { get; set; }
    
    /// <summary>
    /// Learning rank based on performance
    /// </summary>
    public string LearningRank { get; set; } = string.Empty;
    
    /// <summary>
    /// Favorite learning topics
    /// </summary>
    public List<string> FavoriteTopics { get; set; } = new();
    
    /// <summary>
    /// Areas needing improvement
    /// </summary>
    public List<string> ImprovementAreas { get; set; } = new();
    
    /// <summary>
    /// Recommended next modules
    /// </summary>
    public List<int> RecommendedModules { get; set; } = new();
    
    /// <summary>
    /// Get witty statistics summary
    /// </summary>
    public string GetWittySummary()
    {
        var completionRate = ModulesStarted > 0 ? (double)ModulesCompleted / ModulesStarted * 100 : 0;
        
        var rankDescription = LearningRank switch
        {
            "Network Newbie" => "🌱 Everyone starts somewhere - you're growing!",
            "Digital Disciple" => "📚 Knowledge seeker extraordinaire!",
            "Packet Pioneer" => "🚀 Blazing trails through network territories!",
            "Byte Boss" => "👑 Commanding respect in digital realms!",
            "Network Ninja" => "🥷 Stealthy master of network mysteries!",
            _ => "🎯 Unique learning journey in progress!"
        };
        
        return $"🎓 {LearningRank}: {completionRate:F0}% module completion, {AverageScore:F0}% avg score, {CurrentStreak} day streak! {rankDescription}";
    }
}