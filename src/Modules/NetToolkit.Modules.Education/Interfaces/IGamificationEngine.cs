using NetToolkit.Modules.Education.Models;

namespace NetToolkit.Modules.Education.Interfaces;

/// <summary>
/// Gamification engine interface - the cosmic conductor of achievement symphonies
/// Where learning transforms into legendary quests with badges, streaks, and digital glory
/// </summary>
public interface IGamificationEngine
{
    /// <summary>
    /// Calculate quiz score with witty feedback
    /// </summary>
    /// <param name="answers">User's quiz answers</param>
    /// <param name="correctAnswers">Correct answers</param>
    /// <param name="timeSpentSeconds">Time spent on quiz</param>
    /// <returns>Score result with feedback</returns>
    Task<QuizScoreResult> CalculateQuizScoreAsync(
        Dictionary<string, int> answers, 
        Dictionary<string, int> correctAnswers,
        int timeSpentSeconds = 0);
    
    /// <summary>
    /// Award badges based on achievements
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="achievement">Achievement data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Newly awarded badges</returns>
    Task<List<Badge>> AwardBadgesAsync(
        string userId, 
        Achievement achievement, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Calculate and update learning streaks
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="activityDate">Date of learning activity</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated streak information</returns>
    Task<StreakInfo> UpdateStreakAsync(
        string userId, 
        DateTime activityDate, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Generate motivational engagement messages
    /// </summary>
    /// <param name="userProgress">Current user progress</param>
    /// <param name="recentActivity">Recent learning activity</param>
    /// <returns>Witty motivation message</returns>
    Task<MotivationMessage> GenerateMotivationAsync(
        ModuleProgress userProgress, 
        LearningActivity recentActivity);
    
    /// <summary>
    /// Calculate learning rank based on overall performance
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Current learning rank with description</returns>
    Task<LearningRank> CalculateLearningRankAsync(
        string userId, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get leaderboard data for friendly competition
    /// </summary>
    /// <param name="moduleId">Optional module filter</param>
    /// <param name="timeframe">Leaderboard timeframe</param>
    /// <param name="limit">Number of entries to return</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Leaderboard entries</returns>
    Task<List<LeaderboardEntry>> GetLeaderboardAsync(
        int? moduleId = null,
        LeaderboardTimeframe timeframe = LeaderboardTimeframe.AllTime,
        int limit = 10,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Analyze learning patterns and suggest improvements
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Learning analytics with recommendations</returns>
    Task<LearningAnalytics> AnalyzeLearningPatternsAsync(
        string userId, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Check for milestone achievements
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="milestone">Milestone to check</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Achievement result</returns>
    Task<MilestoneResult> CheckMilestoneAsync(
        string userId, 
        Milestone milestone, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Generate daily learning challenges
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="difficulty">Challenge difficulty preference</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Personalized daily challenges</returns>
    Task<List<DailyChallenge>> GenerateDailyChallengesAsync(
        string userId, 
        DifficultyLevel difficulty = DifficultyLevel.Intermediate,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Track engagement metrics for learning optimization
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="engagementData">Engagement activity data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated engagement profile</returns>
    Task<EngagementProfile> TrackEngagementAsync(
        string userId, 
        EngagementData engagementData, 
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Quiz score calculation result
/// </summary>
public class QuizScoreResult
{
    public int Score { get; set; }
    public int MaxScore { get; set; }
    public double Percentage { get; set; }
    public string WittyFeedback { get; set; } = string.Empty;
    public List<string> IncorrectAnswers { get; set; } = new();
    public bool PassedThreshold { get; set; }
    public TimeSpan TimeSpent { get; set; }
    public int BonusPoints { get; set; }
    
    public string GetEncouragingMessage()
    {
        return Percentage switch
        {
            >= 95 => "🏆 Absolutely stellar! You're a networking virtuoso!",
            >= 85 => "⭐ Excellent work! You're mastering the digital realm!",
            >= 75 => "🎯 Great job! Your network knowledge is expanding beautifully!",
            >= 65 => "📚 Good effort! Keep learning and you'll be a network ninja!",
            >= 50 => "💪 Not bad! Practice makes perfect in the networking cosmos!",
            _ => "🌱 Every expert was once a beginner - keep growing your network wisdom!"
        };
    }
}

/// <summary>
/// Achievement data for badge awarding
/// </summary>
public class Achievement
{
    public string Type { get; set; } = string.Empty;
    public int Value { get; set; }
    public Dictionary<string, object> Context { get; set; } = new();
    public DateTime AchievedAt { get; set; } = DateTime.UtcNow;
    public int ModuleId { get; set; }
    public int? LessonId { get; set; }
}

/// <summary>
/// Streak information tracking
/// </summary>
public class StreakInfo
{
    public int CurrentStreak { get; set; }
    public int LongestStreak { get; set; }
    public DateTime LastActivity { get; set; }
    public DateTime StreakStartDate { get; set; }
    public bool IsActive { get; set; }
    public List<DateTime> ActivityDates { get; set; } = new();
    
    public string GetStreakMessage()
    {
        return CurrentStreak switch
        {
            0 => "🌟 Ready to start a new learning adventure?",
            1 => "🔥 Day 1 of your learning journey - keep it burning!",
            < 7 => $"⚡ {CurrentStreak} days strong! You're building momentum!",
            < 30 => $"🚀 {CurrentStreak} days of dedication! You're on fire!",
            < 100 => $"👑 {CurrentStreak} days of mastery! Network royalty status!",
            _ => $"🌌 {CurrentStreak} days of cosmic learning! You're a legend!"
        };
    }
}

/// <summary>
/// Motivational message generation
/// </summary>
public class MotivationMessage
{
    public string PrimaryMessage { get; set; } = string.Empty;
    public string SecondaryMessage { get; set; } = string.Empty;
    public string Emoji { get; set; } = "🎓";
    public MotivationType Type { get; set; }
    public int Priority { get; set; }
    
    public string GetFullMessage() => $"{Emoji} {PrimaryMessage} {SecondaryMessage}";
}

/// <summary>
/// Learning activity data
/// </summary>
public class LearningActivity
{
    public string ActivityType { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public int Duration { get; set; }
    public Dictionary<string, object> Data { get; set; } = new();
    public double Performance { get; set; }
}

/// <summary>
/// Learning rank calculation
/// </summary>
public class LearningRank
{
    public string RankName { get; set; } = string.Empty;
    public int RankLevel { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public int PointsToNext { get; set; }
    public string NextRankName { get; set; } = string.Empty;
    
    public static readonly List<(string Name, string Description, string Icon)> Ranks = new()
    {
        ("Network Newbie", "🌱 Everyone starts somewhere - you're growing!", "🌱"),
        ("Digital Disciple", "📚 Knowledge seeker extraordinaire!", "📚"),
        ("Packet Pioneer", "🚀 Blazing trails through network territories!", "🚀"),
        ("Byte Boss", "👑 Commanding respect in digital realms!", "👑"),
        ("Network Ninja", "🥷 Stealthy master of network mysteries!", "🥷"),
        ("Cyber Sage", "🧙‍♂️ Wise wizard of wireless wonders!", "🧙‍♂️"),
        ("Data Deity", "⚡ Omnipotent overseer of digital dimensions!", "⚡"),
        ("Network Nexus", "🌌 Transcendent master of the networking cosmos!", "🌌")
    };
}

/// <summary>
/// Leaderboard entry
/// </summary>
public class LeaderboardEntry
{
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public int Position { get; set; }
    public int Score { get; set; }
    public string Achievement { get; set; } = string.Empty;
    public LearningRank Rank { get; set; } = new();
    public int BadgeCount { get; set; }
    public DateTime LastActivity { get; set; }
}

/// <summary>
/// Learning analytics and recommendations
/// </summary>
public class LearningAnalytics
{
    public string UserId { get; set; } = string.Empty;
    public Dictionary<string, double> StrengthAreas { get; set; } = new();
    public Dictionary<string, double> ImprovementAreas { get; set; } = new();
    public List<string> Recommendations { get; set; } = new();
    public LearningPattern PreferredPattern { get; set; }
    public TimeSpan OptimalLearningDuration { get; set; }
    public List<string> PersonalizedTips { get; set; } = new();
    
    public string GetInsightfulSummary()
    {
        var strongestArea = StrengthAreas.OrderByDescending(x => x.Value).FirstOrDefault();
        var weakestArea = ImprovementAreas.OrderByDescending(x => x.Value).FirstOrDefault();
        
        return $"🎯 Your networking superpower: {strongestArea.Key}! " +
               $"Growth opportunity: {weakestArea.Key}. " +
               $"Optimal learning sessions: {OptimalLearningDuration.TotalMinutes:F0} minutes of pure focus!";
    }
}

/// <summary>
/// Milestone achievement result
/// </summary>
public class MilestoneResult
{
    public bool Achieved { get; set; }
    public Milestone Milestone { get; set; } = new();
    public string CelebrationMessage { get; set; } = string.Empty;
    public List<Badge> RewardBadges { get; set; } = new();
    public int BonusPoints { get; set; }
}

/// <summary>
/// Milestone definition
/// </summary>
public class Milestone
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public MilestoneType Type { get; set; }
    public int TargetValue { get; set; }
    public Dictionary<string, object> Requirements { get; set; } = new();
}

/// <summary>
/// Daily learning challenge
/// </summary>
public class DailyChallenge
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DifficultyLevel Difficulty { get; set; }
    public ChallengeType Type { get; set; }
    public int PointReward { get; set; }
    public DateTime ExpiresAt { get; set; }
    public Dictionary<string, object> Parameters { get; set; } = new();
    
    public string GetMotivationalDescription()
    {
        return Type switch
        {
            ChallengeType.QuizMastery => "🧠 Prove your quiz prowess - ace 3 quizzes today!",
            ChallengeType.StreakMaintainer => "🔥 Keep that learning fire burning - maintain your streak!",
            ChallengeType.SpeedLearner => "⚡ Lightning round - complete a lesson in record time!",
            ChallengeType.Perfectionist => "🎯 Aim for perfection - score 100% on your next quiz!",
            ChallengeType.Explorer => "🗺️ Venture into uncharted territories - start a new module!",
            _ => "🌟 Special challenge awaits - rise to the occasion!"
        };
    }
}

/// <summary>
/// Engagement tracking data
/// </summary>
public class EngagementData
{
    public string ActivityType { get; set; } = string.Empty;
    public TimeSpan Duration { get; set; }
    public int InteractionCount { get; set; }
    public Dictionary<string, int> ClickPatterns { get; set; } = new();
    public List<string> FeaturesUsed { get; set; } = new();
    public double FocusScore { get; set; }
}

/// <summary>
/// User engagement profile
/// </summary>
public class EngagementProfile
{
    public string UserId { get; set; } = string.Empty;
    public EngagementLevel Level { get; set; }
    public Dictionary<string, double> PreferredFeatures { get; set; } = new();
    public TimeSpan AverageSessionDuration { get; set; }
    public double AttentionScore { get; set; }
    public List<string> OptimizationTips { get; set; } = new();
    
    public string GetEngagementInsight()
    {
        return Level switch
        {
            EngagementLevel.Highly => "🚀 You're a learning machine! Your focus and dedication are inspiring!",
            EngagementLevel.Moderate => "📈 Great engagement! You're making steady progress in your network journey!",
            EngagementLevel.Low => "💪 Let's boost that learning energy! Small steps lead to big achievements!",
            _ => "🎯 Finding your learning rhythm - every expert started somewhere!"
        };
    }
}

/// <summary>
/// Motivation message types
/// </summary>
public enum MotivationType
{
    Encouragement,
    Achievement,
    Challenge,
    Streak,
    Comeback,
    Milestone
}

/// <summary>
/// Leaderboard timeframe options
/// </summary>
public enum LeaderboardTimeframe
{
    Daily,
    Weekly,
    Monthly,
    AllTime
}

/// <summary>
/// Learning pattern preferences
/// </summary>
public enum LearningPattern
{
    Visual,
    Interactive,
    Sequential,
    Explorative,
    Competitive,
    Collaborative
}

/// <summary>
/// Milestone achievement types
/// </summary>
public enum MilestoneType
{
    LessonsCompleted,
    QuizScore,
    StreakDays,
    ModulesFinished,
    BadgesEarned,
    TimeSpent,
    PerfectScores,
    ConsistentLearning
}

/// <summary>
/// Daily challenge types
/// </summary>
public enum ChallengeType
{
    QuizMastery,
    StreakMaintainer,
    SpeedLearner,
    Perfectionist,
    Explorer,
    Reviewer,
    SocialLearner,
    Innovation
}

/// <summary>
/// Engagement level classification
/// </summary>
public enum EngagementLevel
{
    Low,
    Moderate,
    Highly,
    Expert
}