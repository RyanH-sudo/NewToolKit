using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NetToolkit.Core.Events;
using NetToolkit.Core.Interfaces;
using NetToolkit.Modules.Education.Data;
using NetToolkit.Modules.Education.Events;
using NetToolkit.Modules.Education.Interfaces;
using NetToolkit.Modules.Education.Models;
using Polly;
using Polly.Retry;
using System.Collections.Immutable;

namespace NetToolkit.Modules.Education.Services;

/// <summary>
/// Education content service - the cosmic librarian of learning wisdom
/// Where knowledge is curated, delivered, and transformed into enlightenment experiences
/// </summary>
public class EducationContentService : IEducationService
{
    private readonly EducationDbContext _dbContext;
    private readonly IGamificationEngine _gamificationEngine;
    private readonly IEventBus _eventBus;
    private readonly ILogger<EducationContentService> _logger;
    private readonly ResiliencePipeline _retryPolicy;
    
    // Cache for frequently accessed content
    private readonly Dictionary<int, Module> _moduleCache = new();
    private readonly Dictionary<int, Lesson> _lessonCache = new();
    
    public EducationContentService(
        EducationDbContext dbContext,
        IGamificationEngine gamificationEngine,
        IEventBus eventBus,
        ILogger<EducationContentService> logger)
    {
        _dbContext = dbContext;
        _gamificationEngine = gamificationEngine;
        _eventBus = eventBus;
        _logger = logger;
        
        // Configure resilience policy for database operations (INFERRED: Polly v8 ResiliencePipeline)
        _retryPolicy = new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions
            {
                MaxRetryAttempts = 3,
                BackoffType = DelayBackoffType.Exponential,
                Delay = TimeSpan.FromSeconds(1),
                MaxDelay = TimeSpan.FromSeconds(30)
            })
            .Build();
    }

    /// <summary>
    /// Load module content with all lessons and slides
    /// </summary>
    public async Task<Module?> LoadModuleAsync(int moduleId, CancellationToken cancellationToken = default)
    {
        try
        {
            // Check cache first for cosmic efficiency
            if (_moduleCache.TryGetValue(moduleId, out var cachedModule))
            {
                return cachedModule;
            }

            return await _retryPolicy.ExecuteAsync(async (context) =>
            {
                _logger.LogInformation("Loading module {ModuleId} from the cosmic knowledge database...", moduleId);

                var module = await _dbContext.Modules
                    .Include(m => m.Lessons.OrderBy(l => l.LessonNumber))
                        .ThenInclude(l => l.Content)
                    .Include(m => m.Lessons)
                        .ThenInclude(l => l.QuizQuestions)
                    .FirstOrDefaultAsync(m => m.Id == moduleId, cancellationToken);

                if (module == null)
                {
                    _logger.LogWarning("Module {ModuleId} not found in the knowledge cosmos - perhaps it's in another dimension?", moduleId);
                    return null;
                }

                // Cache the module for future cosmic retrievals
                _moduleCache[moduleId] = module;
                
                // Pre-cache lessons for stellar performance
                foreach (var lesson in module.Lessons)
                {
                    _lessonCache[lesson.Id] = lesson;
                }

                _logger.LogInformation("Module '{ModuleName}' loaded successfully with {LessonCount} lessons - knowledge delivery complete!", 
                    module.Title, module.Lessons.Count);

                // Publish module loading event for cosmic coordination
                await _eventBus.PublishAsync<ModuleLoadedEvent>(new ModuleLoadedEvent
                {
                    ModuleId = moduleId,
                    ModuleName = module.Title,
                    LessonCount = module.Lessons.Count,
                    LoadedAt = DateTime.UtcNow
                }, cancellationToken);

                return module;
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Cosmic disturbance while loading module {ModuleId} - knowledge retrieval failed!", moduleId);
            return null;
        }
    }

    /// <summary>
    /// Get all available modules
    /// </summary>
    public async Task<List<Module>> GetAllModulesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _retryPolicy.ExecuteAsync(async (context) =>
            {

                var modules = await _dbContext.Modules
                    .Include(m => m.Lessons)
                    .OrderBy(m => m.Id)
                    .ToListAsync(cancellationToken);

                _logger.LogInformation("Retrieved {ModuleCount} modules from the learning cosmos!", modules.Count);

                return modules;
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve modules from the cosmic catalog - returning empty constellation!");
            return new List<Module>();
        }
    }

    /// <summary>
    /// Get user's progress across all modules
    /// </summary>
    public async Task<List<ModuleProgress>> GetProgressAsync(string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _retryPolicy.ExecuteAsync(async (context) =>
            {

                var progressList = await _dbContext.ModuleProgress
                    .Include(mp => mp.LessonProgresses)
                    .Include(mp => mp.Module)
                    .Where(mp => mp.UserId == userId)
                    .OrderBy(mp => mp.ModuleId)
                    .ToListAsync(cancellationToken);

                _logger.LogInformation("Retrieved progress for {ProgressCount} modules for user {UserId} - journey tracking complete!", 
                    progressList.Count, userId);

                return progressList;
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve progress for user {UserId} - cosmic navigation system offline!", userId);
            return new List<ModuleProgress>();
        }
    }

    /// <summary>
    /// Get user's progress for specific module
    /// </summary>
    public async Task<ModuleProgress?> GetModuleProgressAsync(string userId, int moduleId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _retryPolicy.ExecuteAsync(async (context) =>
            {

                var progress = await _dbContext.ModuleProgress
                    .Include(mp => mp.LessonProgresses.OrderBy(lp => lp.LessonId))
                    .Include(mp => mp.Module)
                    .FirstOrDefaultAsync(mp => mp.UserId == userId && mp.ModuleId == moduleId, cancellationToken);

                if (progress != null)
                {
                }

                return progress;
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve module progress for user {UserId} in module {ModuleId}!", userId, moduleId);
            return null;
        }
    }

    /// <summary>
    /// Start a new module for user
    /// </summary>
    public async Task<ModuleProgress> StartModuleAsync(string userId, int moduleId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _retryPolicy.ExecuteAsync(async (context) =>
            {
                _logger.LogInformation("Starting cosmic learning journey for user {UserId} in module {ModuleId}!", userId, moduleId);

                // Check if progress already exists
                var existingProgress = await GetModuleProgressAsync(userId, moduleId, cancellationToken);
                if (existingProgress != null)
                {
                    _logger.LogInformation("Module {ModuleId} already started for user {UserId} - returning existing journey!", moduleId, userId);
                    return existingProgress;
                }

                // Load the module to get lesson information
                var module = await LoadModuleAsync(moduleId, cancellationToken);
                if (module == null)
                {
                    throw new InvalidOperationException($"Cannot start module {moduleId} - module not found in the cosmic library!");
                }

                // Create new module progress
                var moduleProgress = new ModuleProgress
                {
                    UserId = userId,
                    ModuleId = moduleId,
                    Module = module,
                    StartedAt = DateTime.UtcNow,
                    CurrentLessonId = module.Lessons.OrderBy(l => l.LessonNumber).First().Id,
                    CompletedLessons = 0,
                    TotalLessons = module.Lessons.Count,
                    LessonProgresses = new List<LessonProgress>()
                };

                // Create lesson progress entries for all lessons
                foreach (var lesson in module.Lessons.OrderBy(l => l.LessonNumber))
                {
                    var lessonProgress = new LessonProgress
                    {
                        UserId = userId,
                        LessonId = lesson.Id,
                        Lesson = lesson,
                        ModuleProgressId = moduleProgress.Id,
                        Status = lesson.LessonNumber == 1 ? LessonStatus.InProgress : LessonStatus.NotStarted,
                        CurrentSlideIndex = lesson.LessonNumber == 1 ? 0 : -1
                    };

                    moduleProgress.LessonProgresses.Add(lessonProgress);
                }

                _dbContext.ModuleProgress.Add(moduleProgress);
                await _dbContext.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Module {ModuleId} started successfully for user {UserId} - cosmic adventure begins!", moduleId, userId);

                // Publish module started event
                await _eventBus.PublishAsync<ModuleStartedEvent>(new ModuleStartedEvent
                {
                    UserId = userId,
                    ModuleId = moduleId,
                    ModuleName = module.Title,
                    StartedAt = moduleProgress.StartedAt,
                    TotalLessons = moduleProgress.TotalLessons
                }, cancellationToken);

                return moduleProgress;
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start module {ModuleId} for user {UserId} - cosmic initialization error!", moduleId, userId);
            throw;
        }
    }

    /// <summary>
    /// Complete lesson and handle quiz scoring
    /// </summary>
    public async Task<(LessonProgress Progress, List<Badge> NewBadges)> CompleteLessonAsync(
        string userId, 
        int lessonId, 
        Dictionary<string, int>? quizAnswers = null,
        int timeSpentMinutes = 0,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _retryPolicy.ExecuteAsync(async (context) =>
            {
                _logger.LogInformation("Completing lesson {LessonId} for cosmic scholar {UserId}...", lessonId, userId);

                // Get the lesson and its progress
                var lesson = await GetLessonAsync(lessonId, cancellationToken);
                if (lesson == null)
                {
                    throw new InvalidOperationException($"Lesson {lessonId} not found in the cosmic curriculum!");
                }

                var lessonProgress = await _dbContext.LessonProgress
                    .Include(lp => lp.ModuleProgress)
                    .FirstOrDefaultAsync(lp => lp.UserId == userId && lp.LessonId == lessonId, cancellationToken);

                if (lessonProgress == null)
                {
                    throw new InvalidOperationException($"No progress found for user {userId} in lesson {lessonId} - perhaps they need to start the module first?");
                }

                // Handle quiz scoring if quiz answers provided
                var quizScore = 0.0;
                if (quizAnswers != null && lesson.QuizQuestions.Any())
                {
                    var correctAnswers = lesson.QuizQuestions.ToDictionary(q => q.Id, q => q.CorrectAnswerIndex);
                    var scoreResult = await _gamificationEngine.CalculateQuizScoreAsync(quizAnswers, correctAnswers, timeSpentMinutes * 60);
                    
                    quizScore = scoreResult.Percentage;
                    lessonProgress.QuizScore = quizScore;
                    lessonProgress.QuizFeedback = scoreResult.WittyFeedback;
                    
                    _logger.LogInformation("Quiz completed for lesson {LessonId} with score {Score}% - {Feedback}", 
                        lessonId, quizScore, scoreResult.WittyFeedback);
                }

                // Update lesson progress
                lessonProgress.Status = LessonStatus.Completed;
                lessonProgress.CompletedAt = DateTime.UtcNow;
                lessonProgress.TimeSpentMinutes = timeSpentMinutes;

                // Update module progress
                var moduleProgress = lessonProgress.ModuleProgress;
                moduleProgress.CompletedLessons++;
                moduleProgress.LastAccessedAt = DateTime.UtcNow;

                // Check if module is complete
                if (moduleProgress.CompletedLessons >= moduleProgress.TotalLessons)
                {
                    moduleProgress.CompletedAt = DateTime.UtcNow;
                    _logger.LogInformation("Module {ModuleId} completed for user {UserId}", 
                        moduleProgress.ModuleId, userId);
                }

                await _dbContext.SaveChangesAsync(cancellationToken);

                // Award badges and update achievements
                var achievement = new Achievement
                {
                    Type = "LessonCompleted",
                    Value = (int)quizScore,
                    Context = new Dictionary<string, object>
                    {
                        ["LessonId"] = lessonId,
                        ["ModuleId"] = lesson.ModuleId,
                        ["TimeSpent"] = timeSpentMinutes,
                        ["QuizScore"] = quizScore
                    },
                    ModuleId = lesson.ModuleId,
                    LessonId = lessonId
                };

                var newBadges = await _gamificationEngine.AwardBadgesAsync(userId, achievement, cancellationToken);

                // Update streak
                await _gamificationEngine.UpdateStreakAsync(userId, DateTime.UtcNow, cancellationToken);

                // Publish lesson completion event
                await _eventBus.PublishAsync<LessonCompletedEvent>(new LessonCompletedEvent
                {
                    UserId = userId,
                    LessonId = lessonId,
                    ModuleId = lesson.ModuleId,
                    QuizScore = quizScore,
                    TimeSpentMinutes = timeSpentMinutes,
                    CompletedAt = DateTime.UtcNow,
                    BadgesAwarded = newBadges.Select(b => b.Name).ToList()
                }, cancellationToken);

                // Publish IP-specific integration event for Module 3 lessons
                if (lesson.ModuleId == 3) // Module 3: IP Shenanigans
                {
                    await PublishIPLessonCompletionEvent(userId, lesson, quizScore, cancellationToken);
                }

                // Publish scripting-specific integration event for Module 4 lessons
                if (lesson.ModuleId == 4) // Module 4: Scripting Sorcery
                {
                    await PublishScriptingLessonCompletionEvent(userId, lesson, quizScore, cancellationToken);
                }

                // Publish routing-specific integration event for Module 5 lessons
                if (lesson.ModuleId == 5) // Module 5: Routing Riddles
                {
                    await PublishRoutingLessonCompletionEvent(userId, lesson, quizScore, cancellationToken);
                }

                // Publish security-specific integration event for Module 6 lessons
                if (lesson.ModuleId == 6) // Module 6: Security Shenanigans
                {
                    await PublishSecurityLessonCompletionEvent(userId, lesson, quizScore, cancellationToken);
                }

                // Publish wireless-specific integration event for Module 7 lessons
                if (lesson.ModuleId == 7) // Module 7: Wireless Wonders
                {
                    await PublishWirelessLessonCompletionEvent(userId, lesson, quizScore, cancellationToken);
                }

                // Publish cloud-specific integration event for Module 8 lessons
                if (lesson.ModuleId == 8) // Module 8: Cloud Conquest
                {
                    await PublishCloudLessonCompletionEvent(userId, lesson, quizScore, cancellationToken);
                }

                // Publish mastery-specific integration event for Module 10 lessons
                if (lesson.ModuleId == 10) // Module 10: Mastery Mayhem - Engineer Extraordinaire
                {
                    await PublishMasteryMayhemLessonCompletionEvent(userId, lesson, quizScore, cancellationToken);
                }

                // Check for IP mastery achievement
                if (lesson.ModuleId == 3 && moduleProgress.CompletedLessons >= moduleProgress.TotalLessons)
                {
                    await CheckIPMasteryAchievement(userId, moduleProgress, cancellationToken);
                }

                // Check for scripting mastery achievement
                if (lesson.ModuleId == 4 && moduleProgress.CompletedLessons >= moduleProgress.TotalLessons)
                {
                    await CheckScriptingMasteryAchievement(userId, moduleProgress, cancellationToken);
                }

                // Check for routing mastery achievement
                if (lesson.ModuleId == 5 && moduleProgress.CompletedLessons >= moduleProgress.TotalLessons)
                {
                    await CheckRoutingMasteryAchievement(userId, moduleProgress, cancellationToken);
                }

                // Check for security mastery achievement
                if (lesson.ModuleId == 6 && moduleProgress.CompletedLessons >= moduleProgress.TotalLessons)
                {
                    await CheckSecurityMasteryAchievement(userId, moduleProgress, cancellationToken);
                }

                // Check for cloud mastery achievement
                if (lesson.ModuleId == 8 && moduleProgress.CompletedLessons >= moduleProgress.TotalLessons)
                {
                    await CheckCloudMasteryAchievement(userId, (UserModuleProgress)moduleProgress, cancellationToken);
                }

                // Publish protocol alchemy-specific integration event for Module 9 lessons
                if (lesson.ModuleId == 9) // Module 9: Advanced Alchemy - Mixing Protocols
                {
                    await PublishProtocolAlchemyLessonCompletionEvent(userId, lesson, quizScore, cancellationToken);
                }

                // Check for protocol alchemy mastery achievement
                if (lesson.ModuleId == 9 && moduleProgress.CompletedLessons >= moduleProgress.TotalLessons)
                {
                    await CheckProtocolAlchemyMasteryAchievement(userId, (UserModuleProgress)moduleProgress, cancellationToken);
                }

                // Check for Engineer Extraordinaire achievement
                if (lesson.ModuleId == 10 && moduleProgress.CompletedLessons >= moduleProgress.TotalLessons)
                {
                    await CheckEngineerExtraordinaireAchievement(userId, moduleProgress, cancellationToken);
                }

                _logger.LogInformation("Lesson {LessonId} completed successfully for user {UserId} with {BadgeCount} new badges",
                    lessonId, userId, newBadges.Count);

                return (lessonProgress, newBadges);
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to complete lesson {LessonId} for user {UserId} - cosmic completion ceremony interrupted!", lessonId, userId);
            throw;
        }
    }

    /// <summary>
    /// Get user's earned badges
    /// </summary>
    public async Task<List<Badge>> GetUserBadgesAsync(string userId, int? moduleId = null, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _retryPolicy.ExecuteAsync(async (context) =>
            {

                var badgesQuery = _dbContext.UserBadges
                    .Include(ub => ub.Badge)
                    .Where(ub => ub.UserId == userId);

                if (moduleId.HasValue)
                {
                    badgesQuery = badgesQuery.Where(ub => ub.Badge.ModuleId == moduleId);
                }

                var userBadges = await badgesQuery
                    .OrderByDescending(ub => ub.AwardedAt)
                    .Select(ub => ub.Badge)
                    .ToListAsync(cancellationToken);


                return userBadges;
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve badges for user {UserId} - trophy case malfunction!", userId);
            return new List<Badge>();
        }
    }

    /// <summary>
    /// Get detailed lesson content with slides
    /// </summary>
    public async Task<Lesson?> GetLessonAsync(int lessonId, CancellationToken cancellationToken = default)
    {
        try
        {
            // Check cache first
            if (_lessonCache.TryGetValue(lessonId, out var cachedLesson))
            {
                return cachedLesson;
            }

            return await _retryPolicy.ExecuteAsync(async (context) =>
            {

                var lesson = await _dbContext.Lessons
                    .Include(l => l.Content)
                    .Include(l => l.QuizQuestions)
                    .Include(l => l.Module)
                    .FirstOrDefaultAsync(l => l.Id == lessonId, cancellationToken);

                if (lesson != null)
                {
                    // Cache for future retrievals
                    _lessonCache[lessonId] = lesson;
                }

                return lesson;
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load lesson {LessonId} - cosmic curriculum access error!", lessonId);
            return null;
        }
    }

    /// <summary>
    /// Update user's current position in lesson
    /// </summary>
    public async Task<LessonProgress> UpdateLessonPositionAsync(
        string userId, 
        int lessonId, 
        int slideIndex, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _retryPolicy.ExecuteAsync(async (context) =>
            {
                var lessonProgress = await _dbContext.LessonProgress
                    .FirstOrDefaultAsync(lp => lp.UserId == userId && lp.LessonId == lessonId, cancellationToken);

                if (lessonProgress == null)
                {
                    throw new InvalidOperationException($"No progress found for user {userId} in lesson {lessonId}");
                }

                lessonProgress.CurrentSlideIndex = slideIndex;
                lessonProgress.LastAccessedAt = DateTime.UtcNow;

                if (lessonProgress.Status == LessonStatus.NotStarted)
                {
                    lessonProgress.Status = LessonStatus.InProgress;
                    lessonProgress.StartedAt = DateTime.UtcNow;
                }

                await _dbContext.SaveChangesAsync(cancellationToken);


                return lessonProgress;
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update lesson position for user {UserId} in lesson {LessonId}", userId, lessonId);
            throw;
        }
    }

    /// <summary>
    /// Get learning statistics for user
    /// </summary>
    public async Task<LearningStatistics> GetLearningStatisticsAsync(string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _retryPolicy.ExecuteAsync(async (context) =>
            {

                var moduleProgresses = await _dbContext.ModuleProgress
                    .Include(mp => mp.LessonProgresses)
                    .Where(mp => mp.UserId == userId)
                    .ToListAsync(cancellationToken);

                var lessonProgresses = moduleProgresses.SelectMany(mp => mp.LessonProgresses).ToList();
                var completedLessons = lessonProgresses.Where(lp => lp.Status == LessonStatus.Completed).ToList();

                var statistics = new LearningStatistics
                {
                    ModulesStarted = moduleProgresses.Count,
                    ModulesCompleted = moduleProgresses.Count(mp => mp.CompletedAt.HasValue),
                    LessonsCompleted = completedLessons.Count,
                    TotalTimeMinutes = completedLessons.Sum(lp => lp.TimeSpentMinutes),
                    AverageScore = completedLessons.Any() ? completedLessons.Average(lp => lp.QuizScore) : 0,
                    BadgesEarned = await _dbContext.UserBadges.CountAsync(ub => ub.UserId == userId, cancellationToken)
                };

                // Calculate streak information
                var streak = await _gamificationEngine.UpdateStreakAsync(userId, DateTime.UtcNow, cancellationToken);
                statistics.CurrentStreak = streak.CurrentStreak;
                statistics.LongestStreak = streak.LongestStreak;

                // Determine learning rank
                var rank = await _gamificationEngine.CalculateLearningRankAsync(userId, cancellationToken);
                statistics.LearningRank = rank.RankName;

                _logger.LogInformation("Statistics calculated for user {UserId}: {ModulesCompleted} modules, {LessonsCompleted} lessons, {Rank} rank",
                    userId, statistics.ModulesCompleted, statistics.LessonsCompleted, statistics.LearningRank);

                return statistics;
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to calculate learning statistics for user {UserId} - cosmic analytics offline!", userId);
            return new LearningStatistics { LearningRank = "Cosmic Explorer" };
        }
    }

    /// <summary>
    /// Reset user progress for module (for retaking)
    /// </summary>
    public async Task<bool> ResetModuleProgressAsync(string userId, int moduleId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _retryPolicy.ExecuteAsync(async (context) =>
            {
                _logger.LogInformation("Resetting module {ModuleId} progress for user {UserId} - cosmic do-over initiated!", moduleId, userId);

                var moduleProgress = await _dbContext.ModuleProgress
                    .Include(mp => mp.LessonProgresses)
                    .FirstOrDefaultAsync(mp => mp.UserId == userId && mp.ModuleId == moduleId, cancellationToken);

                if (moduleProgress == null)
                {
                    _logger.LogWarning("No progress found to reset for user {UserId} in module {ModuleId}", userId, moduleId);
                    return false;
                }

                // Reset all lesson progresses
                foreach (var lessonProgress in moduleProgress.LessonProgresses)
                {
                    lessonProgress.Status = LessonStatus.NotStarted;
                    lessonProgress.CurrentSlideIndex = -1;
                    lessonProgress.QuizScore = 0;
                    lessonProgress.StartedAt = DateTime.MinValue;
                    lessonProgress.CompletedAt = null;
                    lessonProgress.TimeSpentMinutes = 0;
                    lessonProgress.QuizFeedback = string.Empty;
                }

                // Reset module progress
                moduleProgress.CompletedLessons = 0;
                moduleProgress.CompletedAt = null;
                moduleProgress.StartedAt = DateTime.UtcNow;
                moduleProgress.LastAccessedAt = DateTime.UtcNow;

                // Set first lesson as in progress
                var firstLesson = moduleProgress.LessonProgresses.OrderBy(lp => lp.LessonId).First();
                firstLesson.Status = LessonStatus.InProgress;
                firstLesson.CurrentSlideIndex = 0;
                moduleProgress.CurrentLessonId = firstLesson.LessonId;

                await _dbContext.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Module {ModuleId} progress reset successfully for user {UserId} - fresh cosmic journey begins!", moduleId, userId);
                return true;
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to reset module {ModuleId} progress for user {UserId} - cosmic reset malfunction!", moduleId, userId);
            return false;
        }
    }

    /// <summary>
    /// Search lessons by content or title
    /// </summary>
    public async Task<List<Lesson>> SearchLessonsAsync(string searchTerm, int? moduleId = null, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _retryPolicy.ExecuteAsync(async (context) =>
            {

                var lessonsQuery = _dbContext.Lessons
                    .Include(l => l.Module)
                    .Where(l => l.Title.Contains(searchTerm) || 
                               l.Description.Contains(searchTerm) ||
                               l.LearningObjectives.Any(obj => obj.Contains(searchTerm)));

                if (moduleId.HasValue)
                {
                    lessonsQuery = lessonsQuery.Where(l => l.ModuleId == moduleId);
                }

                var lessons = await lessonsQuery
                    .OrderBy(l => l.ModuleId)
                    .ThenBy(l => l.LessonNumber)
                    .Take(50) // Limit results for performance
                    .ToListAsync(cancellationToken);


                return lessons;
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to search lessons for '{SearchTerm}' - cosmic search engine offline!", searchTerm);
            return new List<Lesson>();
        }
    }

    /// <summary>
    /// Initialize and seed the database with content
    /// </summary>
    public async Task InitializeContentAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Initializing cosmic education content database...");

            // Ensure database is created
            await _dbContext.Database.EnsureCreatedAsync(cancellationToken);

            // Check if modules already exist
            var existingModules = await _dbContext.Modules.CountAsync(cancellationToken);
            if (existingModules >= 5)
            {
                _logger.LogInformation("Modules already exist in cosmic database - skipping seed operation");
                return;
            }

            var totalLessons = 0;
            var totalBadges = 0;

            // Seed Module 1 content if it doesn't exist
            var existingModule1 = await _dbContext.Modules.FirstOrDefaultAsync(m => m.Id == 1, cancellationToken);
            if (existingModule1 == null)
            {
                var module1 = Module1ContentSeed.GetModule1Content();
                _dbContext.Modules.Add(module1);
                totalLessons += module1.Lessons.Count;

                // Seed Module 1 badges
                var module1Badges = Module1ContentSeed.GetModule1Badges();
                foreach (var badge in module1Badges)
                {
                    badge.ModuleId = 1;
                    _dbContext.Badges.Add(badge);
                }
                totalBadges += module1Badges.Count;

                _logger.LogInformation("Module 1 seeded - {LessonCount} lessons added to cosmic curriculum!", module1.Lessons.Count);
            }

            // Seed Module 2 content if it doesn't exist
            var existingModule2 = await _dbContext.Modules.FirstOrDefaultAsync(m => m.Id == 2, cancellationToken);
            if (existingModule2 == null)
            {
                var module2 = Module2ContentSeed.GetModule2Content();
                _dbContext.Modules.Add(module2);
                totalLessons += module2.Lessons.Count;

                // Seed Module 2 badges
                var module2Badges = Module2ContentSeed.GetModule2Badges();
                foreach (var badge in module2Badges)
                {
                    badge.ModuleId = 2;
                    _dbContext.Badges.Add(badge);
                }
                totalBadges += module2Badges.Count;

                _logger.LogInformation("Module 2 seeded - {LessonCount} Hardware Heroes lessons added to cosmic curriculum!", module2.Lessons.Count);
            }

            // Seed Module 3 content if it doesn't exist
            var existingModule3 = await _dbContext.Modules.FirstOrDefaultAsync(m => m.Id == 3, cancellationToken);
            if (existingModule3 == null)
            {
                var module3 = Module3ContentSeed.GetModule3Content();
                _dbContext.Modules.Add(module3);
                totalLessons += module3.Lessons.Count;

                // Seed Module 3 badges
                var module3Badges = Module3ContentSeed.GetModule3Badges();
                foreach (var badge in module3Badges)
                {
                    badge.ModuleId = 3;
                    _dbContext.Badges.Add(badge);
                }
                totalBadges += module3Badges.Count;

                _logger.LogInformation("Module 3 seeded - {LessonCount} IP Shenanigans lessons added to cosmic curriculum!", module3.Lessons.Count);
            }

            // Seed Module 4 content if it doesn't exist
            var existingModule4 = await _dbContext.Modules.FirstOrDefaultAsync(m => m.Id == 4, cancellationToken);
            if (existingModule4 == null)
            {
                var module4 = Module4ContentSeed.GetModule4Content();
                _dbContext.Modules.Add(module4);
                totalLessons += module4.Lessons.Count;

                // Seed Module 4 badges
                var module4Badges = Module4ContentSeed.GetModule4Badges();
                foreach (var badge in module4Badges)
                {
                    badge.ModuleId = 4;
                    _dbContext.Badges.Add(badge);
                }
                totalBadges += module4Badges.Count;

                _logger.LogInformation("Module 4 seeded - {LessonCount} Scripting Sorcery lessons added to cosmic curriculum!", module4.Lessons.Count);
            }

            // Seed Module 5 content if it doesn't exist
            var existingModule5 = await _dbContext.Modules.FirstOrDefaultAsync(m => m.Id == 5, cancellationToken);
            if (existingModule5 == null)
            {
                var module5 = Module5ContentSeed.GetModule5Content();
                _dbContext.Modules.Add(module5);
                totalLessons += module5.Lessons.Count;

                // Seed Module 5 badges
                var module5Badges = Module5ContentSeed.GetModule5Badges();
                foreach (var badge in module5Badges)
                {
                    badge.ModuleId = 5;
                    _dbContext.Badges.Add(badge);
                }
                totalBadges += module5Badges.Count;

                _logger.LogInformation("Module 5 seeded - {LessonCount} Routing Riddles lessons added to cosmic curriculum!", module5.Lessons.Count);
            }

            // Seed Module 6 content if it doesn't exist
            var existingModule6 = await _dbContext.Modules.FirstOrDefaultAsync(m => m.Id == 6, cancellationToken);
            if (existingModule6 == null)
            {
                var module6 = Module6ContentSeed.GetModule6Content();
                _dbContext.Modules.Add(module6);
                totalLessons += module6.Lessons.Count;

                // Seed Module 6 badges
                var module6Badges = Module6ContentSeed.GetModule6Badges();
                foreach (var badge in module6Badges)
                {
                    badge.ModuleId = 6;
                    _dbContext.Badges.Add(badge);
                }
                totalBadges += module6Badges.Count;

                _logger.LogInformation("Module 6 seeded - {LessonCount} Security Shenanigans lessons added to cosmic curriculum!", module6.Lessons.Count);
            }

            // Seed Module 7 content if it doesn't exist
            var existingModule7 = await _dbContext.Modules.FirstOrDefaultAsync(m => m.Id == 7, cancellationToken);
            if (existingModule7 == null)
            {
                var module7 = Module7ContentSeed.GetModule7Content();
                _dbContext.Modules.Add(module7);
                totalLessons += module7.Lessons.Count;

                // Seed Module 7 badges
                var module7Badges = Module7ContentSeed.GetModule7Badges();
                foreach (var badge in module7Badges)
                {
                    badge.ModuleId = 7;
                    _dbContext.Badges.Add(badge);
                }
                totalBadges += module7Badges.Count;

                _logger.LogInformation("Module 7 seeded - {LessonCount} Wireless Wonders lessons added to cosmic curriculum!", module7.Lessons.Count);
            }

            // Seed Module 8 content if it doesn't exist
            var existingModule8 = await _dbContext.Modules.FirstOrDefaultAsync(m => m.Id == 8, cancellationToken);
            if (existingModule8 == null)
            {
                var module8 = Module8ContentSeed.GetModule8Content();
                _dbContext.Modules.Add(module8);
                totalLessons += module8.Lessons.Count;

                // Seed Module 8 badges
                var module8Badges = Module8ContentSeed.GetModule8Badges();
                foreach (var badge in module8Badges)
                {
                    badge.ModuleId = 8;
                    _dbContext.Badges.Add(badge);
                }

                totalBadges += module8Badges.Count;

                _logger.LogInformation("Module 8 seeded - {LessonCount} Cloud Conquest lessons added to cosmic curriculum!", module8.Lessons.Count);
            }

            // Seed Module 9 content if it doesn't exist
            var existingModule9 = await _dbContext.Modules.FirstOrDefaultAsync(m => m.Id == 9, cancellationToken);
            if (existingModule9 == null)
            {
                var module9 = Module9ContentSeed.GetModule9Content();
                _dbContext.Modules.Add(module9);
                totalLessons += module9.Lessons.Count;

                // Seed Module 9 badges
                var module9Badges = Module9ContentSeed.GetModule9Badges();
                foreach (var badge in module9Badges)
                {
                    badge.ModuleId = 9;
                    _dbContext.Badges.Add(badge);
                }

                totalBadges += module9Badges.Count;

                _logger.LogInformation("Module 9 seeded - {LessonCount} Advanced Alchemy lessons added to cosmic curriculum!", module9.Lessons.Count);
            }

            // Seed Module 10 content if it doesn't exist
            var existingModule10 = await _dbContext.Modules.FirstOrDefaultAsync(m => m.Id == 10, cancellationToken);
            if (existingModule10 == null)
            {
                var module10 = Module10ContentSeed.GetModule10Content();
                _dbContext.Modules.Add(module10);
                totalLessons += module10.Lessons.Count;

                // Seed Module 10 badges
                var module10Badges = Module10ContentSeed.GetModule10Badges();
                foreach (var badge in module10Badges)
                {
                    badge.ModuleId = 10;
                    _dbContext.Badges.Add(badge);
                }

                totalBadges += module10Badges.Count;

                _logger.LogInformation("Module 10 seeded - {LessonCount} Mastery Mayhem capstone lessons added to cosmic curriculum!", module10.Lessons.Count);
            }

            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Cosmic education content initialized successfully - {TotalLessons} lessons and {TotalBadges} badges seeded across all modules!",
                totalLessons, totalBadges);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize cosmic education content - database seeding failed!");
            throw;
        }
    }

    /// <summary>
    /// Publish IP lesson completion event with cross-module integration data
    /// </summary>
    /// <summary>
    /// Publish IP lesson completion event with CS8852-compliant object initializer pattern
    /// </summary>
    private async Task PublishIPLessonCompletionEvent(string userId, Lesson lesson, double quizScore, CancellationToken cancellationToken)
    {
        try
        {
            // Pre-calculate all lesson-specific properties before object construction
            var enablesIntegration = GetIPLessonEnablesIntegration(lesson.LessonNumber, quizScore);
            var ipConcepts = GetIPConceptsForLesson(lesson.LessonNumber);
            var suggestedScripts = GetIPScriptsForLesson(lesson.LessonNumber);
            var addressesToScan = GetIPAddressesToScanForLesson(lesson.LessonNumber);
            var integrationData = GetIPIntegrationDataForLesson(lesson, enablesIntegration);

            // Create event with ALL properties in object initializer (CS8852-compliant)
            var ipEvent = new IPLessonCompletedEvent
            {
                UserId = userId,
                LessonId = lesson.Id,
                ModuleId = lesson.ModuleId,
                LessonNumber = lesson.LessonNumber,
                QuizScore = quizScore,
                CompletedAt = DateTime.UtcNow,
                EnablesIntegration = enablesIntegration,        // ✅ Pre-calculated
                IPConcepts = ipConcepts,                        // ✅ Pre-calculated list
                SuggestedScripts = suggestedScripts,            // ✅ Pre-calculated list
                IPAddressesToScan = addressesToScan,            // ✅ Pre-calculated list
                IntegrationData = integrationData               // ✅ Pre-calculated dictionary
            };

            // NO POST-CONSTRUCTION ASSIGNMENTS (eliminates CS8852 errors)

            await _eventBus.PublishAsync<IPLessonCompletedEvent>(ipEvent, cancellationToken);

            _logger.LogDebug("IP lesson completion event published for user {UserId} lesson {LessonNumber} with {ConceptCount} concepts",
                userId, lesson.LessonNumber, ipEvent.IPConcepts.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish IP lesson completion event for user {UserId} lesson {LessonId}",
                userId, lesson.Id);
        }
    }

    /// <summary>
    /// Helper method: Determine if integration is enabled for IP lesson
    /// </summary>
    private bool GetIPLessonEnablesIntegration(int lessonNumber, double quizScore)
    {
        var enablesIntegration = quizScore >= 70; // Default threshold
        
        switch (lessonNumber)
        {
            case 16: // Scripting IPs - always enable
                enablesIntegration = true;
                break;
            case 19: // Cert-Level - higher threshold
                enablesIntegration = quizScore >= 80;
                break;
        }
        
        return enablesIntegration;
    }

    /// <summary>
    /// Helper method: Get IP concepts for specific lesson
    /// </summary>
    private List<string> GetIPConceptsForLesson(int lessonNumber) => lessonNumber switch
    {
        1 => new List<string> { "IPv4 Basics", "IP Address Structure", "Digital Addressing" },
        2 => new List<string> { "IPv4 vs IPv6", "Address Space", "Transition Technologies" },
        4 => new List<string> { "Static IP Configuration", "Manual Assignment", "Network Interface Cards" },
        5 => new List<string> { "DHCP", "Dynamic Assignment", "IP Leasing" },
        6 => new List<string> { "Subnetting", "Network Segmentation", "Subnet Masks" },
        8 => new List<string> { "Private IP Ranges", "Public IP Space", "RFC 1918" },
        9 => new List<string> { "Network Address Translation", "Port Forwarding", "NAT Tables" },
        12 => new List<string> { "Loopback Interface", "Localhost", "127.0.0.1" },
        16 => new List<string> { "IP Scripting", "PowerShell Network", "Automation" },
        17 => new List<string> { "IP Conflicts", "Network Troubleshooting", "Diagnostic Commands" },
        19 => new List<string> { "Subnet Calculations", "VLSM", "Binary Math" },
        _ => new List<string>()
    };

    /// <summary>
    /// Helper method: Get suggested scripts for specific IP lesson
    /// </summary>
    private List<string> GetIPScriptsForLesson(int lessonNumber) => lessonNumber switch
    {
        1 => new List<string> { "Get-NetIPAddress" },
        2 => new List<string> { "Get-NetIPAddress -AddressFamily IPv4", "Get-NetIPAddress -AddressFamily IPv6" },
        4 => new List<string> { "Get-NetIPConfiguration", "New-NetIPAddress" },
        5 => new List<string> { "Get-DhcpServerv4Lease", "ipconfig /renew" },
        6 => new List<string> { "Get-NetRoute" },
        9 => new List<string> { "Get-NetNatStaticMapping" },
        12 => new List<string> { "ping 127.0.0.1" },
        16 => new List<string> { "Set-NetIPAddress", "Remove-NetIPAddress", "Get-NetAdapter" },
        17 => new List<string> { "ipconfig /release", "ipconfig /renew", "Test-NetConnection" },
        _ => new List<string>()
    };

    /// <summary>
    /// Helper method: Get IP addresses to scan for specific lesson
    /// </summary>
    private List<string> GetIPAddressesToScanForLesson(int lessonNumber) => lessonNumber switch
    {
        4 => new List<string> { "127.0.0.1" },
        6 => new List<string> { "192.168.1.0/24", "10.0.0.0/24" },
        8 => new List<string> { "192.168.1.1", "10.0.0.1", "172.16.0.1" },
        12 => new List<string> { "127.0.0.1" },
        _ => new List<string>()
    };

    /// <summary>
    /// Helper method: Get integration data for IP lesson
    /// </summary>
    private Dictionary<string, object> GetIPIntegrationDataForLesson(Lesson lesson, bool enablesIntegration)
    {
        var integrationData = new Dictionary<string, object>
        {
            ["LessonTitle"] = lesson.Title,
            ["Description"] = lesson.Description
        };
        
        if (enablesIntegration)
        {
            integrationData["PowerShellModule"] = "Suggest IP-related scripts";
            integrationData["ScannerModule"] = "Enable IP range scanning";
            integrationData["SecurityModule"] = "Suggest IP security scans";
            integrationData["SSHModule"] = "Enable IP-based connections";
        }
        
        return integrationData;
    }

    /// <summary>
    /// Check if user has achieved IP mastery and publish mastery event
    /// </summary>
    private async Task CheckIPMasteryAchievement(string userId, ModuleProgress moduleProgress, CancellationToken cancellationToken)
    {
        try
        {
            // Calculate overall score for Module 3
            var completedLessons = moduleProgress.LessonProgresses
                .Where(lp => lp.Status == LessonStatus.Completed)
                .ToList();

            if (completedLessons.Count < moduleProgress.TotalLessons)
                return;

            var overallScore = completedLessons.Average(lp => lp.QuizScore);
            var masteryLevel = DetermineMasteryLevel(overallScore);
            
            if (masteryLevel == "No Mastery")
                return;

            // Get user's IP-related badges to determine unlocked features
            var ipBadges = await GetUserBadgesAsync(userId, 3, cancellationToken);
            var unlockedSkills = DetermineUnlockedSkills(ipBadges, overallScore);

            var masteryEvent = new IPMasteryUnlockedEvent
            {
                UserId = userId,
                OverallScore = overallScore,
                MasteryLevel = masteryLevel,
                AchievedAt = DateTime.UtcNow,
                BadgesAwarded = ipBadges.Select(b => b.Name).ToList(),
                CosmicMessage = GenerateCosmicMasteryMessage(masteryLevel, overallScore),
                UnlockedSkills = unlockedSkills
            };

            // Add cross-module integrations based on mastery level
            switch (masteryLevel)
            {
                case "IP Initiate":
                    masteryEvent.PowerShellIntegrations.AddRange(new[] { "Basic IP Commands", "Network Adapter Info" });
                    masteryEvent.ScannerFeatures.AddRange(new[] { "Local Network Scan", "Ping Sweep" });
                    break;

                case "Address Architect":
                    masteryEvent.PowerShellIntegrations.AddRange(new[] { 
                        "IP Configuration", "DHCP Management", "Network Diagnostics" 
                    });
                    masteryEvent.ScannerFeatures.AddRange(new[] { 
                        "Subnet Discovery", "Port Scanning", "Network Mapping" 
                    });
                    masteryEvent.SecurityAssessments.Add("Basic IP Security Scan");
                    break;

                case "Subnet Sorcerer":
                    masteryEvent.PowerShellIntegrations.AddRange(new[] { 
                        "Advanced Subnetting", "VLSM Calculations", "Routing Tables" 
                    });
                    masteryEvent.ScannerFeatures.AddRange(new[] { 
                        "Advanced Network Discovery", "Topology Mapping", "VLAN Detection" 
                    });
                    masteryEvent.SecurityAssessments.AddRange(new[] { 
                        "Network Vulnerability Assessment", "IP-based Attack Detection" 
                    });
                    masteryEvent.SSHConfigurations.AddRange(new[] { 
                        "Multi-host SSH", "IP-based Connection Profiles" 
                    });
                    break;

                case "Protocol Paladin":
                    // Unlock all advanced features
                    masteryEvent.PowerShellIntegrations.AddRange(new[] { 
                        "Network Automation Scripts", "Enterprise IP Management", "Custom Network Tools" 
                    });
                    masteryEvent.ScannerFeatures.AddRange(new[] { 
                        "Enterprise Network Discovery", "Custom Scan Profiles", "Network Compliance Checks" 
                    });
                    masteryEvent.SecurityAssessments.AddRange(new[] { 
                        "Advanced Threat Detection", "Network Forensics", "Security Automation" 
                    });
                    masteryEvent.SSHConfigurations.AddRange(new[] { 
                        "Advanced SSH Tunneling", "Network Device Management", "Automated Deployments" 
                    });
                    break;
            }

            // Add integration data
            masteryEvent.IntegrationData["CompletionDate"] = DateTime.UtcNow;
            masteryEvent.IntegrationData["ModuleId"] = 3;
            masteryEvent.IntegrationData["TotalLessons"] = moduleProgress.TotalLessons;
            masteryEvent.IntegrationData["AverageScore"] = overallScore;

            await _eventBus.PublishAsync<IPMasteryUnlockedEvent>(masteryEvent, cancellationToken);

            _logger.LogInformation("IP mastery achieved by user {UserId} - {MasteryLevel} with {Score}% overall score",
                userId, masteryLevel, overallScore);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check IP mastery achievement for user {UserId}", userId);
        }
    }

    /// <summary>
    /// Determine mastery level based on overall score
    /// </summary>
    private static string DetermineMasteryLevel(double overallScore)
    {
        return overallScore switch
        {
            >= 95 => "Protocol Paladin",
            >= 85 => "Subnet Sorcerer", 
            >= 75 => "Address Architect",
            >= 65 => "IP Initiate",
            _ => "No Mastery"
        };
    }

    /// <summary>
    /// Determine unlocked skills based on badges and score
    /// </summary>
    private static List<string> DetermineUnlockedSkills(List<Badge> badges, double overallScore)
    {
        var skills = new List<string>();

        if (badges.Any(b => b.Name == "ip_initiate"))
            skills.Add("Basic IP Operations");

        if (badges.Any(b => b.Name == "address_architect"))
            skills.Add("IP Configuration Management");

        if (badges.Any(b => b.Name == "subnet_sorcerer"))
            skills.Add("Advanced Subnetting");

        if (badges.Any(b => b.Name == "dhcp_dynamo"))
            skills.Add("DHCP Management");

        if (badges.Any(b => b.Name == "nat_navigator"))
            skills.Add("NAT Configuration");

        if (badges.Any(b => b.Name == "loopback_legend"))
            skills.Add("Network Troubleshooting");

        if (badges.Any(b => b.Name == "script_sage"))
            skills.Add("Network Automation");

        if (badges.Any(b => b.Name == "troubleshoot_titan"))
            skills.Add("Advanced Diagnostics");

        if (badges.Any(b => b.Name == "ipv6_innovator"))
            skills.Add("IPv6 Management");

        if (badges.Any(b => b.Name == "cert_conqueror"))
            skills.Add("Certification-Level Knowledge");

        if (overallScore >= 90)
            skills.Add("Expert Network Analysis");

        return skills;
    }

    /// <summary>
    /// Generate cosmic mastery message based on achievement level
    /// </summary>
    private static string GenerateCosmicMasteryMessage(string masteryLevel, double score)
    {
        return masteryLevel switch
        {
            "Protocol Paladin" => $"🌟 COSMIC MASTERY ACHIEVED! With {score:F1}% mastery, you've transcended mortal IP limitations and become a Protocol Paladin - the universe's networks bow to your supreme addressing wisdom!",
            "Subnet Sorcerer" => $"🧙‍♂️ Excellent! {score:F1}% mastery makes you a Subnet Sorcerer - your binary spells can divide any network with mystical precision!",
            "Address Architect" => $"🏗️ Impressive! {score:F1}% mastery grants you the title Address Architect - design network neighborhoods with cosmic blueprint precision!",
            "IP Initiate" => $"🌟 Well done! {score:F1}% mastery makes you an IP Initiate - the first step on your journey to network enlightenment!",
            _ => $"Keep exploring the cosmic networks - every address tells a story of connection and discovery!"
        };
    }

    /// <summary>
    /// Publish scripting lesson completion event with pre-calculated properties to avoid CS8852 errors
    /// </summary>
    private async Task PublishScriptingLessonCompletionEvent(string userId, Lesson lesson, double quizScore, CancellationToken cancellationToken)
    {
        try
        {
            // ✅ PRE-CALCULATE ALL CONDITIONAL VALUES (eliminates CS8852 errors)
            var (enablesPowerShellIntegration, unlocksNetworkAutomation) = GetScriptingLessonBooleanProperties(lesson.LessonNumber, quizScore);
            var scriptingConcepts = GetScriptingConceptsForLesson(lesson.LessonNumber);
            var demonstrationScripts = GetDemonstrationScriptsForLesson(lesson.LessonNumber);
            var wittyFeedback = GetScriptingWittyFeedbackForLesson(lesson.LessonNumber, quizScore);
            var networkAutomationOpportunities = GetScriptingNetworkAutomationOpportunities(lesson.LessonNumber);
            var securityEnhancements = GetScriptingSecurityEnhancements(lesson.LessonNumber);
            var automationOpportunities = GetScriptingAutomationOpportunities(lesson.LessonNumber);
            var integrationData = GetScriptingIntegrationDataForLesson(lesson.LessonNumber, quizScore, enablesPowerShellIntegration);

            // ✅ OBJECT INITIALIZER ONLY (no post-construction assignments)
            var scriptingEvent = new ScriptingLessonCompletedEvent
            {
                UserId = userId,
                LessonId = lesson.Id,
                ModuleId = lesson.ModuleId,
                LessonNumber = lesson.LessonNumber,
                QuizScore = quizScore,
                CompletedAt = DateTime.UtcNow,
                EnablesPowerShellIntegration = enablesPowerShellIntegration,    // ✅ Pre-calculated
                UnlocksNetworkAutomation = unlocksNetworkAutomation,            // ✅ Pre-calculated
                ScriptingConcepts = scriptingConcepts,                          // ✅ Pre-calculated list
                DemonstrationScripts = demonstrationScripts,                    // ✅ Pre-calculated list
                WittyFeedback = wittyFeedback,                                  // ✅ Pre-calculated
                NetworkAutomationOpportunities = networkAutomationOpportunities, // ✅ Pre-calculated list
                SecurityEnhancements = securityEnhancements,                   // ✅ Pre-calculated list
                AutomationOpportunities = automationOpportunities,             // ✅ Pre-calculated list
                IntegrationData = integrationData                               // ✅ Pre-calculated dictionary
            };

            // NO POST-CONSTRUCTION ASSIGNMENTS (eliminates CS8852 errors)

            await _eventBus.PublishAsync<ScriptingLessonCompletedEvent>(scriptingEvent, cancellationToken);

            _logger.LogDebug("Scripting lesson completion event published for user {UserId} lesson {LessonNumber} with {ConceptCount} concepts",
                userId, lesson.LessonNumber, scriptingEvent.ScriptingConcepts.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish scripting lesson completion event for user {UserId} lesson {LessonId}",
                userId, lesson.Id);
        }
    }

    /// <summary>
    /// Helper method: Get boolean properties for scripting lesson
    /// </summary>
    private (bool enablesPowerShellIntegration, bool unlocksNetworkAutomation) GetScriptingLessonBooleanProperties(int lessonNumber, double quizScore)
    {
        return lessonNumber switch
        {
            1 => (quizScore >= 60, false),                    // INFERRED: Lower threshold for intro
            2 => (quizScore >= 65, false),                    // INFERRED: Variables unlock basics
            3 => (true, false),                               // INFERRED: Commands always enable PS
            4 => (quizScore >= 70, quizScore >= 75),          // INFERRED: Loops enable automation
            5 => (quizScore >= 70, quizScore >= 80),          // INFERRED: Functions unlock advanced
            6 => (true, quizScore >= 80),                     // INFERRED: Error handling always enables PS
            7 => (true, quizScore >= 75),                     // INFERRED: Functions unlock networking
            8 => (true, quizScore >= 80),                     // INFERRED: Error handling unlock automation
            9 => (true, true),                                // INFERRED: Network scripting unlocks both
            10 => (true, true),                               // INFERRED: Advanced topics unlock all
            11 => (true, true),                               // INFERRED: Security scripting advanced
            12 => (true, quizScore >= 75),                    // INFERRED: Automation conditional
            13 => (true, true),                               // INFERRED: Network automation mastery
            14 => (true, true),                               // INFERRED: Integration mastery
            15 => (true, true),                               // INFERRED: Real-world scenarios
            16 => (quizScore >= 75, true),                    // INFERRED: Advanced loops conditional PS
            17 => (true, true),                               // INFERRED: Enterprise scripting
            18 => (true, true),                               // INFERRED: Remote scripting advanced
            19 => (quizScore >= 90, quizScore >= 90),         // INFERRED: Certification level high bar
            20 => (quizScore >= 90, quizScore >= 90),         // INFERRED: Master level high bar
            _ => (quizScore >= 70, quizScore >= 80)
        };
    }

    /// <summary>
    /// Helper method: Get scripting concepts for lesson (full 20 cases)
    /// </summary>
    private List<string> GetScriptingConceptsForLesson(int lessonNumber)
    {
        return lessonNumber switch
        {
            1 => new List<string> { "Script Basics", "PowerShell Introduction", "Command Automation" },
            2 => new List<string> { "Variable Storage", "$Variable Syntax", "Data Containers" },
            3 => new List<string> { "PowerShell Commands", "Get-Command", "Verb-Noun Pattern" },
            4 => new List<string> { "Loop Structures", "ForEach-Object", "Repetitive Tasks" },
            5 => new List<string> { "If-Then Logic", "Comparison Operators", "Branching Logic" },
            6 => new List<string> { "ForEach-Object", "While Loops", "For Loops", "Iteration" },
            7 => new List<string> { "Function Creation", "Reusable Code", "Parameters", "Return Values" },
            8 => new List<string> { "Try-Catch", "Error Management", "Exception Handling" },
            9 => new List<string> { "Network Commands", "Get-NetIPAddress", "Network Automation" },
            10 => new List<string> { "Advanced Scripting", "Complex Logic", "Optimization" },
            11 => new List<string> { "Security Automation", "Firewall Scripts", "Security Hardening" },
            12 => new List<string> { "Task Scheduling", "Automated Scripts", "Lazy Wizardry" },
            13 => new List<string> { "Network Automation", "Switch Configuration", "VLAN Management" },
            14 => new List<string> { "System Integration", "API Calls", "External Systems" },
            15 => new List<string> { "Real-World Scenarios", "Production Scripts", "Enterprise Solutions" },
            16 => new List<string> { "Advanced Iteration", "Nested Loops", "Break/Continue" },
            17 => new List<string> { "Enterprise Scripting", "Large-Scale Automation", "Infrastructure Management" },
            18 => new List<string> { "Remote Commands", "Invoke-Command", "Distant Automation" },
            19 => new List<string> { "Advanced Scripting", "Certification Level", "Complex Automation" },
            20 => new List<string> { "Scripting Mastery", "Expert Techniques", "Innovation Patterns" },
            _ => new List<string> { "General Scripting", "PowerShell Basics", "Automation Concepts" }
        };
    }

    /// <summary>
    /// Helper method: Get demonstration scripts for lesson (full 20 cases)
    /// </summary>
    private List<string> GetDemonstrationScriptsForLesson(int lessonNumber)
    {
        return lessonNumber switch
        {
            1 => new List<string> { "Get-Command | Select-Object Name" },
            2 => new List<string> { "$computerName = $env:COMPUTERNAME", "$ipAddress = (Get-NetIPAddress -AddressFamily IPv4)[0].IPAddress" },
            3 => new List<string> { "Get-Command *Network*", "Get-Help Get-Process", "Get-Process | Where-Object {$_.CPU -gt 100}" },
            4 => new List<string> { "1..10 | ForEach-Object { Write-Host \"Count: $_\" }", "Get-Service | Where-Object {$_.Status -eq 'Running'}" },
            5 => new List<string> { "if ($service.Status -eq 'Running') { Write-Host 'Service is active' }", "if (Test-Connection 8.8.8.8 -Count 1 -Quiet) { 'Internet connected' } else { 'No internet' }" },
            6 => new List<string> { "1..10 | ForEach-Object { Test-Connection \"192.168.1.$_\" -Count 1 -Quiet }", "Get-Process | ForEach-Object { \"$($_.Name): $($_.CPU)\" }" },
            7 => new List<string> { "function Test-NetworkDevice { param($IP) Test-Connection $IP -Count 1 -Quiet }" },
            8 => new List<string> { "try { Get-Service NonExistent } catch { Write-Warning 'Service not found!' }" },
            9 => new List<string> { "Get-NetIPAddress -AddressFamily IPv4", "Get-NetAdapter | Where-Object {$_.Status -eq 'Up'}" },
            10 => new List<string> { "Register-ScheduledTask -TaskName 'NetworkCheck' -Action (New-ScheduledTaskAction -Execute 'PowerShell')" },
            11 => new List<string> { "Enable-NetFirewallRule -DisplayGroup 'Remote Desktop'" },
            12 => new List<string> { "Register-ScheduledJob -Name 'NetworkScan' -ScriptBlock { Test-Connection 8.8.8.8 }" },
            13 => new List<string> { "New-NetFirewallRule -DisplayName 'Allow-HTTP' -Direction Inbound -Protocol TCP -LocalPort 80" },
            14 => new List<string> { "Invoke-RestMethod -Uri 'https://api.example.com/network' -Method GET", "$response | ConvertTo-Json" },
            15 => new List<string> { "Deploy-NetworkConfiguration -Environment Production", "Backup-NetworkSettings -Path C:\\Backup" },
            16 => new List<string> { "for ($i=0; $i -lt 10; $i++) { if ($i -eq 5) { break } }" },
            17 => new List<string> { "Start-Workflow NetworkDeployment", "Parallel { InlineScript { Configure-Switches } }" },
            18 => new List<string> { "Invoke-Command -ComputerName SERVER01 -ScriptBlock { Get-Process }" },
            19 => new List<string> { "Optimize-NetworkPerformance", "Measure-NetworkLatency -Detailed" },
            20 => new List<string> { "Start-InnovativeNetworkSolution", "Deploy-AI-NetworkOptimization" },
            _ => new List<string> { "Get-Help about_PowerShell", "Get-Command -Module Microsoft.PowerShell.Core" }
        };
    }

    /// <summary>
    /// Helper method: Get witty feedback for scripting lesson (full 20 cases)
    /// </summary>
    private string GetScriptingWittyFeedbackForLesson(int lessonNumber, double quizScore)
    {
        var baseMessages = lessonNumber switch
        {
            1 => "🪄 Welcome to the magical realm of scripting! You've taken your first step into network sorcery!",
            2 => "🧪 Your variable potions are brewing perfectly! Time to mix some magical essences!",
            3 => "📚 Your spell book knowledge expands! The grimoire reveals its secrets to you!",
            4 => "🔄 Loop mastery achieved! You cast spells repeatedly until victory is yours!",
            5 => "🌟 Your decision-making spells guide you through the enchanted forest paths!",
            6 => "🔄 Loop mastery achieved! You cast spells repeatedly until victory is yours!",
            7 => "📜 Your reusable spell scrolls rise like phoenixes - summon them whenever needed!",
            8 => "🕸️ Your error-catching nets trap every gremlin! No bug escapes your magical defenses!",
            9 => "🏰 Your network kingdom bows to your scripted commands! Rule your digital realm with code!",
            10 => "⚙️ Advanced automation architect! Building self-running digital machinery!",
            11 => "🛡️ Your protective ward spells shield the kingdom! Security through scripting sorcery!",
            12 => "🤖 Lazy wizardry perfected! Your self-casting spells work while you sleep!",
            13 => "🔌 Network automation wizard! Orchestrating switches like a digital conductor!",
            14 => "🌉 Integration innovator! Bridging systems with scriptual excellence!",
            15 => "🏢 Real-world warrior! Deploying production-ready automation solutions!",
            16 => "♾️ Loop mastery transcends mortal limits! Avoid the infinite curse while wielding repetitive power!",
            17 => "🏭 Enterprise engineer! Scaling automation across digital empires!",
            18 => "🔭 Your scripting telescope reaches across vast network distances! Command far kingdoms!",
            19 => "👑 Script Sorcerer Supreme status achieved! Your automation magic knows no bounds!",
            20 => "👑 Scripting sovereignty achieved! You are the ultimate PowerShell potentate!",
            _ => "✨ Scripting magic in progress! Keep weaving those digital spells!"
        };

        return quizScore switch
        {
            >= 95 => $"{baseMessages} LEGENDARY PERFORMANCE! 🌟",
            >= 85 => $"{baseMessages} EXCELLENT MASTERY! 💎",
            >= 75 => $"{baseMessages} GREAT PROGRESS! 🔥",
            >= 65 => $"{baseMessages} SOLID FOUNDATION! 💪",
            _ => $"{baseMessages} GOOD START! 📈"
        };
    }

    /// <summary>
    /// Helper method: Get network automation opportunities for lesson
    /// </summary>
    private List<string> GetScriptingNetworkAutomationOpportunities(int lessonNumber)
    {
        return lessonNumber switch
        {
            9 => new List<string> { "IP Configuration", "Network Monitoring", "Adapter Management" },
            13 => new List<string> { "Switch Configuration", "VLAN Management", "Network Discovery" },
            18 => new List<string> { "Remote Administration", "Distributed Management", "Cross-Network Operations" },
            _ => new List<string>()
        };
    }

    /// <summary>
    /// Helper method: Get security enhancements for lesson
    /// </summary>
    private List<string> GetScriptingSecurityEnhancements(int lessonNumber)
    {
        return lessonNumber switch
        {
            11 => new List<string> { "Firewall Management", "Security Baseline", "Compliance Checks" },
            _ => new List<string>()
        };
    }

    /// <summary>
    /// Helper method: Get automation opportunities for lesson
    /// </summary>
    private List<string> GetScriptingAutomationOpportunities(int lessonNumber)
    {
        return lessonNumber switch
        {
            12 => new List<string> { "Scheduled Tasks", "Background Jobs", "Automated Monitoring" },
            _ => new List<string>()
        };
    }

    /// <summary>
    /// Helper method: Get integration data for scripting lesson
    /// </summary>
    private Dictionary<string, object> GetScriptingIntegrationDataForLesson(int lessonNumber, double quizScore, bool enablesPowerShellIntegration)
    {
        var baseData = new Dictionary<string, object>
        {
            ["lesson_title"] = $"Scripting Lesson {lessonNumber}",
            ["quiz_score"] = quizScore,
            ["scripting_level"] = GetScriptingLevelForLesson(lessonNumber),
            ["completion_timestamp"] = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
        };

        if (enablesPowerShellIntegration)
        {
            baseData["PowerShellModule"] = "Enable advanced scripting features";
            baseData["ScannerModule"] = "Unlock script-based network scanning";
            baseData["SecurityModule"] = "Enable security automation scripts";
            baseData["SSHModule"] = "Unlock remote script execution";
        }

        return baseData;
    }

    private string GetScriptingLevelForLesson(int lessonNumber) => lessonNumber switch
    {
        <= 3 => "PowerShell Padawan",
        <= 6 => "Script Squire",
        <= 10 => "Automation Apprentice", 
        <= 15 => "PowerShell Paladin",
        <= 19 => "Scripting Sage",
        20 => "PowerShell Potentate",
        _ => "Digital Dabbler"
    };

    /// <summary>
    /// Check if user has achieved scripting mastery and publish mastery event
    /// </summary>
    private async Task CheckScriptingMasteryAchievement(string userId, ModuleProgress moduleProgress, CancellationToken cancellationToken)
    {
        try
        {
            // Calculate overall score for Module 4
            var completedLessons = moduleProgress.LessonProgresses
                .Where(lp => lp.Status == LessonStatus.Completed)
                .ToList();

            if (completedLessons.Count < moduleProgress.TotalLessons)
                return;

            var overallScore = completedLessons.Average(lp => lp.QuizScore);
            var masteryLevel = DetermineScriptingMasteryLevel(overallScore);

            if (masteryLevel == "No Mastery")
                return;

            // Get user's scripting-related badges to determine unlocked features
            var scriptingBadges = await GetUserBadgesAsync(userId, 4, cancellationToken);
            var unlockedSkills = DetermineScriptingUnlockedSkills(scriptingBadges, overallScore);

            var masteryEvent = new ScriptingSorceryMasteredEvent
            {
                UserId = userId,
                OverallScore = overallScore,
                MasteryLevel = masteryLevel,
                AchievedAt = DateTime.UtcNow,
                BadgesAwarded = scriptingBadges.Select(b => b.Name).ToList(),
                CosmicMessage = GenerateCosmicScriptingMessage(masteryLevel, overallScore),
                UnlockedSkills = unlockedSkills,
                QualifiesForMentorRole = overallScore >= 95
            };

            // Add cross-module integrations based on mastery level
            switch (masteryLevel)
            {
                case "Script Apprentice":
                    masteryEvent.PowerShellFeatures.AddRange(new[] { "Basic Script Execution", "Command History" });
                    masteryEvent.NetworkAutomationScripts.Add("Simple Network Checks");
                    break;

                case "Automation Archmage":
                    masteryEvent.PowerShellFeatures.AddRange(new[] { 
                        "Advanced Script Debugging", "Custom Functions", "Module Creation" 
                    });
                    masteryEvent.NetworkAutomationScripts.AddRange(new[] { 
                        "Network Discovery Scripts", "Automated Monitoring", "Configuration Management" 
                    });
                    masteryEvent.SecurityAutomationFeatures.Add("Security Baseline Scripts");
                    break;

                case "Script Sorcerer Supreme":
                    // Unlock all advanced features
                    masteryEvent.PowerShellFeatures.AddRange(new[] { 
                        "Enterprise Script Management", "Advanced Debugging", "Performance Optimization", "Custom Cmdlets" 
                    });
                    masteryEvent.NetworkAutomationScripts.AddRange(new[] { 
                        "Enterprise Network Automation", "Advanced Monitoring Scripts", "Self-Healing Networks" 
                    });
                    masteryEvent.SecurityAutomationFeatures.AddRange(new[] { 
                        "Advanced Security Scripts", "Compliance Automation", "Threat Response Scripts" 
                    });
                    masteryEvent.RemoteScriptingFeatures.AddRange(new[] { 
                        "Multi-Server Script Execution", "Orchestrated Deployments", "Remote Management Scripts" 
                    });
                    masteryEvent.MicrosoftIntegrations.AddRange(new[] { 
                        "Exchange Script Automation", "Active Directory Scripts", "Azure PowerShell Integration" 
                    });
                    break;
            }

            // Add integration data
            masteryEvent.IntegrationData["CompletionDate"] = DateTime.UtcNow;
            masteryEvent.IntegrationData["ModuleId"] = 4;
            masteryEvent.IntegrationData["TotalLessons"] = moduleProgress.TotalLessons;
            masteryEvent.IntegrationData["AverageScore"] = overallScore;

            await _eventBus.PublishAsync<ScriptingSorceryMasteredEvent>(masteryEvent, cancellationToken);

            _logger.LogInformation("Scripting mastery achieved by user {UserId} - {MasteryLevel} with {Score}% overall score",
                userId, masteryLevel, overallScore);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check scripting mastery achievement for user {UserId}", userId);
        }
    }

    /// <summary>
    /// Determine scripting mastery level based on overall score
    /// </summary>
    private static string DetermineScriptingMasteryLevel(double overallScore)
    {
        return overallScore switch
        {
            >= 95 => "Script Sorcerer Supreme",
            >= 85 => "Automation Archmage",
            >= 75 => "Code Conjurer",
            >= 65 => "Script Apprentice",
            _ => "No Mastery"
        };
    }

    /// <summary>
    /// Determine unlocked scripting skills based on badges and score
    /// </summary>
    private static List<string> DetermineScriptingUnlockedSkills(List<Badge> badges, double overallScore)
    {
        var skills = new List<string>();

        if (badges.Any(b => b.Name == "script_apprentice"))
            skills.Add("Basic PowerShell Operations");

        if (badges.Any(b => b.Name == "variable_virtuoso"))
            skills.Add("Variable Management Mastery");

        if (badges.Any(b => b.Name == "command_conjurer"))
            skills.Add("Advanced Command Usage");

        if (badges.Any(b => b.Name == "logic_luminary"))
            skills.Add("Conditional Logic Mastery");

        if (badges.Any(b => b.Name == "loop_legend"))
            skills.Add("Loop and Iteration Expertise");

        if (badges.Any(b => b.Name == "function_phoenix"))
            skills.Add("Function Creation and Reuse");

        if (badges.Any(b => b.Name == "error_exorcist"))
            skills.Add("Advanced Error Handling");

        if (badges.Any(b => b.Name == "network_necromancer"))
            skills.Add("Network Script Automation");

        if (badges.Any(b => b.Name == "automation_archmage"))
            skills.Add("Enterprise Automation");

        if (badges.Any(b => b.Name == "debug_deity"))
            skills.Add("Script Debugging Mastery");

        if (badges.Any(b => b.Name == "remote_ruler"))
            skills.Add("Remote Script Execution");

        if (badges.Any(b => b.Name == "script_sorcerer_supreme"))
            skills.Add("Complete PowerShell Mastery");

        if (overallScore >= 90)
            skills.Add("Certification-Level Scripting");

        return skills;
    }

    /// <summary>
    /// Generate cosmic scripting mastery message
    /// </summary>
    private static string GenerateCosmicScriptingMessage(string masteryLevel, double score)
    {
        return masteryLevel switch
        {
            "Script Sorcerer Supreme" => $"🌟 ULTIMATE SCRIPTING SUPREMACY! With {score:F1}% mastery, you've become the Script Sorcerer Supreme - your PowerShell magic commands the entire digital universe!",
            "Automation Archmage" => $"🧙‍♂️ Magnificent! {score:F1}% mastery elevates you to Automation Archmage - your lazy wizardry scripts work while lesser mortals toil!",
            "Code Conjurer" => $"⚡ Impressive! {score:F1}% mastery makes you a Code Conjurer - your logical spells bend networks to your will!",
            "Script Apprentice" => $"🌟 Well done! {score:F1}% mastery begins your journey as a Script Apprentice - the first steps into automation sorcery!",
            _ => $"Keep practicing your scripting spells - every command brings you closer to automation mastery!"
        };
    }

    /// <summary>
    /// Publish routing lesson completion event with cross-module integration data
    /// </summary>
    private async Task PublishRoutingLessonCompletionEvent(string userId, Lesson lesson, double quizScore, CancellationToken cancellationToken)
    {
        try
        {
            // ✅ PRE-CALCULATE ALL CONDITIONAL VALUES (eliminates CS8852 errors)
            var (enables3DTopology, unlocksAdvancedRouting) = GetRoutingLessonBooleanProperties(lesson.LessonNumber, quizScore);
            var routingConcepts = GetRoutingConceptsForLesson(lesson.LessonNumber);
            var routingScripts = GetRoutingScriptsForLesson(lesson.LessonNumber);
            var protocolsCovered = GetProtocolsCoveredForLesson(lesson.LessonNumber);
            var topologyEnhancements = GetTopologyEnhancementsForRoutingLesson(lesson.LessonNumber);
            var securityConsiderations = GetSecurityConsiderationsForRoutingLesson(lesson.LessonNumber);
            var troubleshootingTools = GetTroubleshootingToolsForLesson(lesson.LessonNumber);
            var riddleFeedback = GetRiddleFeedbackForLesson(lesson.LessonNumber, quizScore);

            // ✅ OBJECT INITIALIZER ONLY (no post-construction assignments)
            var routingEvent = new RoutingLessonCompletedEvent
            {
                UserId = userId,
                LessonId = lesson.Id,
                ModuleId = lesson.ModuleId,
                LessonNumber = lesson.LessonNumber,
                QuizScore = quizScore,
                CompletedAt = DateTime.UtcNow,
                Enables3DTopology = enables3DTopology,
                UnlocksAdvancedRouting = unlocksAdvancedRouting,
                RoutingConcepts = routingConcepts,
                RoutingScripts = routingScripts,
                ProtocolsCovered = protocolsCovered,
                TopologyEnhancements = topologyEnhancements,
                SecurityConsiderations = securityConsiderations,
                TroubleshootingTools = troubleshootingTools,
                RiddleFeedback = riddleFeedback,
                IntegrationData = GetRoutingIntegrationData(lesson, enables3DTopology, unlocksAdvancedRouting)
            };


            await _eventBus.PublishAsync<RoutingLessonCompletedEvent>(routingEvent, cancellationToken);

            _logger.LogDebug("Routing lesson completion event published for user {UserId} lesson {LessonNumber} with {ConceptCount} concepts",
                userId, lesson.LessonNumber, routingEvent.RoutingConcepts.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish routing lesson completion event for user {UserId} lesson {LessonId}",
                userId, lesson.Id);
        }
    }

    /// <summary>
    /// Check if user has achieved routing mastery and publish mastery event
    /// </summary>
    private async Task CheckRoutingMasteryAchievement(string userId, ModuleProgress moduleProgress, CancellationToken cancellationToken)
    {
        try
        {
            // Calculate overall score for Module 5
            var completedLessons = moduleProgress.LessonProgresses
                .Where(lp => lp.Status == LessonStatus.Completed)
                .ToList();

            if (completedLessons.Count < moduleProgress.TotalLessons)
                return;

            var overallScore = completedLessons.Average(lp => lp.QuizScore);
            var masteryLevel = DetermineRoutingMasteryLevel(overallScore);

            if (masteryLevel == "No Mastery")
                return;

            // Get user's routing-related badges to determine unlocked features
            var routingBadges = await GetUserBadgesAsync(userId, 5, cancellationToken);
            var masteredProtocols = DetermineRoutingMasteredProtocols(routingBadges, overallScore);

            var masteryEvent = new RoutingRiddleMasteredEvent
            {
                UserId = userId,
                OverallScore = overallScore,
                MasteryLevel = masteryLevel,
                AchievedAt = DateTime.UtcNow,
                BadgesAwarded = routingBadges.Select(b => b.Name).ToList(),
                CosmicRiddleMessage = GenerateCosmicRoutingMessage(masteryLevel, overallScore),
                MasteredProtocols = masteredProtocols,
                QualifiesAsPathfindingMentor = overallScore >= 95
            };

            // Add cross-module integrations based on mastery level
            switch (masteryLevel)
            {
                case "Path Seeker":
                    masteryEvent.TopologyFeatures.AddRange(new[] { "Basic Route Visualization", "Static Route Display" });
                    masteryEvent.ScanningEnhancements.Add("Route Discovery Scanning");
                    break;

                case "OSPF Oracle":
                    masteryEvent.TopologyFeatures.AddRange(new[] { 
                        "Link-State Database Visualization", "OSPF Area Mapping", "Shortest Path Highlighting" 
                    });
                    masteryEvent.ScanningEnhancements.AddRange(new[] { 
                        "OSPF Neighbor Discovery", "Link State Analysis", "Area Boundary Detection" 
                    });
                    masteryEvent.AutomationScripts.Add("OSPF Configuration Scripts");
                    break;

                case "BGP Border Guardian":
                    masteryEvent.TopologyFeatures.AddRange(new[] { 
                        "AS Path Visualization", "BGP Policy Mapping", "Internet Route Analysis" 
                    });
                    masteryEvent.SecurityFeatures.AddRange(new[] { 
                        "BGP Route Hijacking Detection", "AS Path Validation", "Route Origin Authentication" 
                    });
                    masteryEvent.AutomationScripts.AddRange(new[] { 
                        "BGP Configuration Scripts", "Route Policy Automation", "Internet Routing Analysis" 
                    });
                    break;

                case "Routing Riddle Master":
                    // Unlock all advanced features
                    masteryEvent.TopologyFeatures.AddRange(new[] { 
                        "Advanced 3D Network Topology", "Multi-Protocol Route Visualization", "Dynamic Path Animation",
                        "VRF Route Separation", "MPLS Label Path Tracking", "Real-time Convergence Display"
                    });
                    masteryEvent.ScanningEnhancements.AddRange(new[] { 
                        "Multi-Protocol Route Discovery", "Advanced Topology Mapping", "Routing Loop Detection"
                    });
                    masteryEvent.AutomationScripts.AddRange(new[] { 
                        "Enterprise Routing Automation", "Multi-Vendor Route Configuration", "Advanced Troubleshooting Scripts"
                    });
                    masteryEvent.SecurityFeatures.AddRange(new[] { 
                        "Comprehensive Route Security", "Advanced BGP Protection", "Routing Attack Prevention"
                    });
                    masteryEvent.SSHRoutingCapabilities.AddRange(new[] { 
                        "Multi-Device Route Configuration", "Automated Route Deployment", "Remote Routing Management"
                    });
                    masteryEvent.DiagnosticTools.AddRange(new[] { 
                        "Advanced Route Analysis", "Path Optimization Tools", "Network Performance Diagnostics"
                    });
                    masteryEvent.CertificationFeatures.AddRange(new[] { 
                        "CCNP-Level Routing Knowledge", "Enterprise Route Design", "Advanced Troubleshooting Mastery"
                    });
                    break;
            }

            // Add integration data
            masteryEvent.IntegrationData["CompletionDate"] = DateTime.UtcNow;
            masteryEvent.IntegrationData["ModuleId"] = 5;
            masteryEvent.IntegrationData["TotalLessons"] = moduleProgress.TotalLessons;
            masteryEvent.IntegrationData["AverageScore"] = overallScore;

            await _eventBus.PublishAsync<RoutingRiddleMasteredEvent>(masteryEvent, cancellationToken);

            _logger.LogInformation("Routing mastery achieved by user {UserId} - {MasteryLevel} with {Score}% overall score",
                userId, masteryLevel, overallScore);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check routing mastery achievement for user {UserId}", userId);
        }
    }

    /// <summary>
    /// Determine routing mastery level based on overall score
    /// </summary>
    private static string DetermineRoutingMasteryLevel(double overallScore)
    {
        return overallScore switch
        {
            >= 95 => "Routing Riddle Master",
            >= 90 => "BGP Border Guardian",
            >= 85 => "OSPF Oracle", 
            >= 75 => "Dynamic Discoverer",
            >= 65 => "Path Seeker",
            _ => "No Mastery"
        };
    }

    /// <summary>
    /// Determine mastered routing protocols based on badges and score
    /// </summary>
    private static List<string> DetermineRoutingMasteredProtocols(List<Badge> badges, double overallScore)
    {
        var protocols = new List<string>();

        if (badges.Any(b => b.Name == "static_navigator"))
            protocols.Add("Static Routing");

        if (badges.Any(b => b.Name == "gossip_guru"))
            protocols.Add("RIP (Routing Information Protocol)");

        if (badges.Any(b => b.Name == "ospf_oracle"))
            protocols.Add("OSPF (Open Shortest Path First)");

        if (badges.Any(b => b.Name == "bgp_border_guardian"))
            protocols.Add("BGP (Border Gateway Protocol)");

        if (badges.Any(b => b.Name == "table_tactician"))
            protocols.Add("Advanced Route Table Management");

        if (badges.Any(b => b.Name == "metric_mastermind"))
            protocols.Add("Route Metric Optimization");

        if (badges.Any(b => b.Name == "convergence_conductor"))
            protocols.Add("Network Convergence Control");

        if (badges.Any(b => b.Name == "loop_liberator"))
            protocols.Add("Routing Loop Prevention");

        if (badges.Any(b => b.Name == "script_pathweaver"))
            protocols.Add("Automated Route Configuration");

        if (badges.Any(b => b.Name == "troubleshoot_tracker"))
            protocols.Add("Advanced Route Troubleshooting");

        if (overallScore >= 90)
            protocols.Add("Enterprise-Level Routing Mastery");

        return protocols;
    }

    /// <summary>
    /// Generate cosmic routing riddle mastery message
    /// </summary>
    private static string GenerateCosmicRoutingMessage(string masteryLevel, double score)
    {
        return masteryLevel switch
        {
            "Routing Riddle Master" => $"🌟 ULTIMATE ROUTING RIDDLE MASTERY! With {score:F1}% mastery, you've become the supreme pathfinder - all network routes bow to your navigation wisdom!",
            "BGP Border Guardian" => $"🌍 Magnificent! {score:F1}% mastery elevates you to BGP Border Guardian - your global routing decisions connect worldwide digital realms!",
            "OSPF Oracle" => $"🗺️ Excellent! {score:F1}% mastery grants you OSPF Oracle status - your link-state wisdom maps networks with cartographer precision!",
            "Dynamic Discoverer" => $"🌟 Impressive! {score:F1}% mastery makes you a Dynamic Discoverer - your adaptive paths evolve like intelligent explorers!",
            "Path Seeker" => $"🗺️ Well done! {score:F1}% mastery begins your journey as a Path Seeker - the first steps into network navigation!",
            _ => $"Keep solving routing riddles - every path puzzle brings you closer to navigation mastery!"
        };
    }

    /// <summary>
    /// Publish security lesson completion event with cross-module integration data
    /// </summary>
    private async Task PublishSecurityLessonCompletionEvent(string userId, Lesson lesson, double quizScore, CancellationToken cancellationToken)
    {
        try
        {
            // ✅ PRE-CALCULATE ALL CONDITIONAL VALUES (eliminates CS8852 errors)
            var (enablesAdvancedScanning, unlocksPentestingFeatures) = GetSecurityLessonBooleanProperties(lesson.LessonNumber, quizScore);
            var securityFeedback = GenerateSecurityFeedback(lesson.LessonNumber, quizScore);
            var securityConcepts = GetSecurityConceptsForLesson(lesson.LessonNumber);
            var securityScripts = GetSecurityScriptsForLesson(lesson.LessonNumber);
            var securityScanTypes = GetSecurityScanTypesForLesson(lesson.LessonNumber);
            var vulnerabilityTechniques = GetVulnerabilityTechniquesForLesson(lesson.LessonNumber);
            var networkSecurityEnhancements = GetNetworkSecurityEnhancementsForLesson(lesson.LessonNumber);
            var sshSecurityConfigs = GetSSHSecurityConfigsForLesson(lesson.LessonNumber);
            var complianceFrameworks = GetComplianceFrameworksForLesson(lesson.LessonNumber);
            var threatIntelInsights = GetThreatIntelInsightsForLesson(lesson.LessonNumber);
            var automationCapabilities = GetSecurityAutomationCapabilitiesForLesson(lesson.LessonNumber);
            var securityLevel = GetSecurityLevelForLesson(lesson.LessonNumber);

            // ✅ OBJECT INITIALIZER ONLY (no post-construction assignments)
            var securityEvent = new SecurityLessonCompletedEvent
            {
                UserId = userId,
                LessonId = lesson.Id,
                ModuleId = lesson.ModuleId,
                LessonNumber = lesson.LessonNumber,
                QuizScore = quizScore,
                CompletedAt = DateTime.UtcNow,
                SecurityFeedback = securityFeedback,
                SecurityConcepts = securityConcepts,
                SecurityScripts = securityScripts,
                SecurityScanTypes = securityScanTypes,
                VulnerabilityTechniques = vulnerabilityTechniques,
                NetworkSecurityEnhancements = networkSecurityEnhancements,
                SSHSecurityConfigs = sshSecurityConfigs,
                ComplianceFrameworks = complianceFrameworks,
                ThreatIntelInsights = threatIntelInsights,
                AutomationCapabilities = automationCapabilities,
                EnablesAdvancedScanning = enablesAdvancedScanning,
                UnlocksPentestingFeatures = unlocksPentestingFeatures,
                IntegrationData = GetSecurityIntegrationData(lesson, quizScore, securityLevel)
            };

            await _eventBus.PublishAsync<SecurityLessonCompletedEvent>(securityEvent, cancellationToken);
            
            _logger.LogDebug("Security lesson completion event published for user {UserId}, lesson {LessonNumber}",
                userId, lesson.LessonNumber);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish security lesson completion event for user {UserId}, lesson {LessonId}",
                userId, lesson.Id);
        }
    }

    /// <summary>
    /// Check for security mastery achievement when Module 6 is completed
    /// </summary>
    private async Task CheckSecurityMasteryAchievement(string userId, ModuleProgress moduleProgress, CancellationToken cancellationToken)
    {
        try
        {
            var averageScore = moduleProgress.AverageScore;
            var masteryLevel = GetSecurityMasteryLevel(averageScore);
            
            if (averageScore >= 70.0) // Minimum security mastery threshold
            {
                var masteryEvent = new SecurityMasteryAchievedEvent
                {
                    UserId = userId,
                    OverallScore = averageScore,
                    MasteryLevel = masteryLevel,
                    AchievedAt = DateTime.UtcNow,
                    SecurityMasteryMessage = GenerateCosmicSecurityMessage(masteryLevel, averageScore),
                    MasteredSecurityDomains = GetMasteredSecurityDomains(averageScore),
                    AdvancedScanningCapabilities = GetAdvancedScanningCapabilities(masteryLevel),
                    PentestingCapabilities = GetPentestingCapabilities(masteryLevel),
                    SecurityAutomationScripts = GetSecurityAutomationScripts(masteryLevel),
                    TopologySecurityFeatures = GetTopologySecurityFeatures(masteryLevel),
                    SSHSecurityCapabilities = GetSSHSecurityCapabilities(masteryLevel),
                    IncidentResponseTools = GetIncidentResponseTools(masteryLevel),
                    ComplianceCapabilities = GetComplianceCapabilities(masteryLevel),
                    ThreatHuntingCapabilities = GetThreatHuntingCapabilities(masteryLevel),
                    ZeroTrustFeatures = GetZeroTrustFeatures(masteryLevel),
                    CloudSecurityCapabilities = GetCloudSecurityCapabilities(masteryLevel),
                    QualifiesAsSecurityMentor = averageScore >= 95.0,
                    BadgesAwarded = new List<string> { "security_shenanigan_master" },
                    CustomSecurityConfigs = GetCustomSecurityConfigs(masteryLevel),
                    CertificationSecurityFeatures = GetCertificationSecurityFeatures(masteryLevel),
                    IntegrationData = new Dictionary<string, object>
                    {
                        ["overall_score"] = averageScore,
                        ["mastery_level"] = masteryLevel,
                        ["completion_date"] = DateTime.UtcNow
                    }
                };

                await _eventBus.PublishAsync<SecurityMasteryAchievedEvent>(masteryEvent, cancellationToken);
                
                _logger.LogInformation("Security mastery achieved for user {UserId} with {MasteryLevel} level",
                    userId, masteryLevel);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check security mastery achievement for user {UserId}", userId);
        }
    }

    // Security-specific helper methods for lesson content

    private List<string> GetSecurityConceptsForLesson(int lessonNumber)
    {
        return lessonNumber switch
        {
            1 => new() { "Network Security Basics", "CIA Triad", "Threat Landscape", "Defense in Depth" },
            2 => new() { "Firewall Rules", "Stateful Inspection", "Network Segmentation", "Packet Filtering" },
            3 => new() { "Symmetric Encryption", "Asymmetric Encryption", "AES", "RSA", "Key Management" },
            4 => new() { "VPN Protocols", "Tunneling", "IPSec", "OpenVPN", "WireGuard" },
            5 => new() { "Intrusion Detection", "Intrusion Prevention", "Signature-based Detection", "Anomaly Detection" },
            6 => new() { "Vulnerability Assessment", "CVSS Scoring", "Patch Management", "Zero-day Threats" },
            7 => new() { "Malware Types", "Virus Detection", "Behavioral Analysis", "Endpoint Protection" },
            8 => new() { "Phishing Detection", "Social Engineering", "Email Security", "Awareness Training" },
            9 => new() { "RBAC", "Least Privilege", "Authentication", "Authorization" },
            10 => new() { "Multi-factor Authentication", "TOTP", "Biometrics", "Hardware Tokens" },
            11 => new() { "PowerShell Security", "Script Signing", "Execution Policies", "Security Automation" },
            12 => new() { "Network Segmentation", "VLANs", "Micro-segmentation", "Zero Trust" },
            13 => new() { "Security Logging", "SIEM", "Log Analysis", "Forensics" },
            14 => new() { "Penetration Testing", "Ethical Hacking", "Red Team", "Vulnerability Assessment" },
            15 => new() { "Zero Trust Architecture", "Never Trust Always Verify", "Continuous Verification" },
            16 => new() { "Cloud Security", "Shared Responsibility", "Cloud Controls", "Container Security" },
            17 => new() { "Incident Response", "Digital Forensics", "Crisis Management", "Recovery Planning" },
            18 => new() { "GDPR", "HIPAA", "SOX", "Compliance Frameworks", "Risk Management" },
            19 => new() { "Advanced Persistent Threats", "Threat Hunting", "Security Architecture" },
            20 => new() { "Security Mastery", "Comprehensive Defense", "Strategic Security Planning" },
            _ => new() { "Security Fundamentals" }
        };
    }

    private List<string> GetSecurityScriptsForLesson(int lessonNumber)
    {
        return lessonNumber switch
        {
            2 => new() { "Enable-WindowsFirewall", "Test-NetConnection", "Get-NetFirewallRule" },
            11 => new() { "Set-ExecutionPolicy", "Get-AuthenticodeSignature", "Test-ScriptSigning" },
            13 => new() { "Get-WinEvent", "Export-SecurityLog", "Search-EventLog" },
            14 => new() { "Invoke-SecurityScan", "Test-NetworkSecurity", "Get-VulnerabilityReport" },
            16 => new() { "Get-AzSecurityAlert", "Test-CloudSecurity", "Enable-CloudCompliance" },
            17 => new() { "Start-IncidentResponse", "Collect-ForensicEvidence", "Generate-IRReport" },
            _ => new() { "Get-SecurityStatus", "Test-SecurityConfiguration" }
        };
    }

    private List<string> GetSecurityScanTypesForLesson(int lessonNumber)
    {
        return lessonNumber switch
        {
            5 => new() { "Network Intrusion Detection", "Behavioral Analysis" },
            6 => new() { "Vulnerability Scanning", "Port Scanning", "Service Enumeration" },
            7 => new() { "Malware Scanning", "Signature Detection", "Heuristic Analysis" },
            14 => new() { "Penetration Testing", "Web Application Scanning", "Network Assessment" },
            19 => new() { "Advanced Threat Hunting", "IOC Detection", "Behavioral Analytics" },
            _ => new() { "Basic Security Scanning" }
        };
    }

    private string GetSecurityMasteryLevel(double averageScore)
    {
        return averageScore switch
        {
            >= 95.0 => "Security Shenanigan Master",
            >= 90.0 => "Fortress Architect", 
            >= 85.0 => "Incident Commander",
            >= 80.0 => "Pentest Paladin",
            >= 75.0 => "Vulnerability Vanquisher",
            >= 70.0 => "Security Sentinel",
            _ => "Fortress Apprentice"
        };
    }

    private string GenerateSecurityFeedback(int lessonNumber, double score)
    {
        var feedback = lessonNumber switch
        {
            1 => score >= 80 ? "Excellent! You're now a Security Sentinel, guardian of the digital realm!" : "Keep learning - the fortress needs strong foundations!",
            2 => score >= 80 ? "Outstanding! Your firewall knowledge makes you a true digital bouncer!" : "Practice those access rules - the club needs better security!",
            3 => score >= 80 ? "Fantastic! Your encryption skills are now unbreakable cosmic codes!" : "Keep practicing those secret codes - cryptography requires patience!",
            20 => score >= 95 ? "LEGENDARY! You are the ultimate Security Shenanigan Master!" : "Good work, but mastery requires perfection!",
            _ => score >= 80 ? "Great work, digital fortress defender!" : "Keep building those security walls!"
        };
        return feedback;
    }

    /// <summary>
    /// Publish wireless lesson completion event with cross-module integration data
    /// </summary>
    private async Task PublishWirelessLessonCompletionEvent(string userId, Lesson lesson, double quizScore, CancellationToken cancellationToken)
    {
        try
        {
            // ✅ PRE-CALCULATE ALL CONDITIONAL VALUES (eliminates CS8852 errors)
            var enablesIntegration = quizScore >= 70;
            var wirelessMasteryLevel = GetWirelessMasteryLevelForLesson(lesson.LessonNumber);
            var wirelessAchievementMessage = GetWirelessAchievementMessage(lesson.LessonNumber, quizScore);
            var wirelessConcepts = GetWirelessConceptsForLesson(lesson.LessonNumber);
            var suggestedWirelessScripts = GetSuggestedWirelessScriptsForLesson(lesson.LessonNumber);
            var recommendedWirelessCmdlets = GetRecommendedWirelessCmdletsForLesson(lesson.LessonNumber);
            var wirelessSecurityScans = GetWirelessSecurityScansForLesson(lesson.LessonNumber);
            var advancedWirelessConcepts = GetAdvancedWirelessConceptsForLesson(lesson.LessonNumber);
            var wirelessIntegrationData = GetWirelessIntegrationDataForLesson(enablesIntegration, quizScore);

            // ✅ OBJECT INITIALIZER ONLY (no post-construction assignments)
            var wirelessEvent = new WirelessLessonCompletedEvent
            {
                UserId = userId,
                LessonId = lesson.Id,
                ModuleId = lesson.ModuleId,
                LessonNumber = lesson.LessonNumber,
                QuizScore = quizScore,
                CompletedAt = DateTime.UtcNow,
                EnablesIntegration = enablesIntegration,
                WirelessMasteryLevel = wirelessMasteryLevel,
                WirelessAchievementMessage = wirelessAchievementMessage,
                WirelessConcepts = wirelessConcepts,
                SuggestedWirelessScripts = suggestedWirelessScripts,
                RecommendedWirelessCmdlets = recommendedWirelessCmdlets,
                WirelessSecurityScans = wirelessSecurityScans,
                AdvancedWirelessConcepts = advancedWirelessConcepts,
                WirelessIntegrationData = wirelessIntegrationData
            };

            await _eventBus.PublishAsync<WirelessLessonCompletedEvent>(wirelessEvent, cancellationToken);

            _logger.LogDebug("Wireless lesson {LessonNumber} completion event published for user {UserId} with {Score}% mastery",
                lesson.LessonNumber, userId, quizScore, wirelessEvent.WirelessAchievementMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish wireless lesson completion event for user {UserId}, lesson {LessonId}",
                userId, lesson.Id);
        }
    }

    /// <summary>
    /// Get wireless mastery level for lesson number
    /// </summary>
    private string GetWirelessMasteryLevelForLesson(int lessonNumber) =>
        lessonNumber switch
        {
            <= 5 => "Wave Wanderer",
            <= 10 => "Frequency Fighter",
            <= 15 => "Wireless Warrior",
            <= 19 => "Signal Sorcerer",
            20 => "Wireless Wonder Wizard",
            _ => "Air Apprentice"
        };

    /// <summary>
    /// Get wireless achievement message based on lesson and score
    /// </summary>
    private string GetWirelessAchievementMessage(int lessonNumber, double score) =>
        lessonNumber switch
        {
            1 => score >= 80 ? "📡 Excellent! You've discovered the invisible highways - welcome to the wireless wonder realm!" : "Keep exploring - the airwaves have many secrets!",
            2 => score >= 80 ? "🏠 Outstanding! Your home WiFi knowledge makes you a true residential wireless hero!" : "Practice those home network concepts - every house needs good wireless!",
            13 => score >= 90 ? "🪄 Magical! Your wireless scripting makes you an Air Script Sorcerer!" : "Practice those wireless commands - automation awaits!",
            20 => score >= 95 ? "🏆 LEGENDARY! You are the ultimate Wireless Wonder Wizard - master of all invisible highways!" : "Good work, but wireless mastery requires wave perfection!",
            _ => score >= 80 ? "📶 Great work, wireless wave rider!" : "Keep learning - the airwaves are calling your name!"
        };

    /// <summary>
    /// Get wireless completion level for scoring
    /// </summary>
    private string GetWirelessCompletionLevel(double score) =>
        score switch
        {
            >= 95 => "Expert",
            >= 85 => "Advanced", 
            >= 75 => "Proficient",
            >= 65 => "Competent",
            _ => "Basic"
        };

    // Placeholder methods for advanced security capabilities
    private List<string> GetVulnerabilityTechniquesForLesson(int lessonNumber) => new();
    private List<string> GetNetworkSecurityEnhancementsForLesson(int lessonNumber) => new();
    private List<string> GetSSHSecurityConfigsForLesson(int lessonNumber) => new();
    private List<string> GetComplianceFrameworksForLesson(int lessonNumber) => new();
    private List<string> GetThreatIntelInsightsForLesson(int lessonNumber) => new();
    private List<string> GetSecurityAutomationCapabilitiesForLesson(int lessonNumber) => new();
    private string GetSecurityLevelForLesson(int lessonNumber) => lessonNumber <= 5 ? "Basic" : lessonNumber <= 15 ? "Intermediate" : "Advanced";
    
    private List<string> GetMasteredSecurityDomains(double score) => new() { "Network Security", "Encryption", "Vulnerability Management" };
    private List<string> GetAdvancedScanningCapabilities(string masteryLevel) => new() { "Advanced Threat Detection", "Behavioral Analysis" };
    private List<string> GetPentestingCapabilities(string masteryLevel) => new() { "Ethical Hacking", "Red Team Operations" };
    private List<string> GetSecurityAutomationScripts(string masteryLevel) => new() { "Automated Security Monitoring", "Incident Response Scripts" };
    private List<string> GetTopologySecurityFeatures(string masteryLevel) => new() { "Security Visualization", "Threat Mapping" };
    private List<string> GetSSHSecurityCapabilities(string masteryLevel) => new() { "Secure Tunneling", "Advanced SSH Configuration" };
    private List<string> GetIncidentResponseTools(string masteryLevel) => new() { "Forensic Analysis", "Crisis Management" };
    private List<string> GetComplianceCapabilities(string masteryLevel) => new() { "Regulatory Compliance", "Risk Assessment" };
    private List<string> GetThreatHuntingCapabilities(string masteryLevel) => new() { "Proactive Threat Detection", "IOC Analysis" };
    private List<string> GetZeroTrustFeatures(string masteryLevel) => new() { "Continuous Verification", "Least Privilege Access" };
    private List<string> GetCloudSecurityCapabilities(string masteryLevel) => new() { "Cloud Security Posture", "Container Security" };
    private List<string> GetCustomSecurityConfigs(string masteryLevel) => new() { "Custom Security Policies", "Tailored Defense Strategies" };
    private List<string> GetCertificationSecurityFeatures(string masteryLevel) => new() { "CISSP-level Knowledge", "Advanced Security Architecture" };

    /// <summary>
    /// Generate cosmic security mastery message
    /// </summary>
    private static string GenerateCosmicSecurityMessage(string masteryLevel, double score)
    {
        return masteryLevel switch
        {
            "Security Shenanigan Master" => $"🏆 ULTIMATE SECURITY MASTERY! With {score:F1}% fortress-building excellence, you've become the supreme digital guardian - all cyber threats tremble before your defenses!",
            "Fortress Architect" => $"🏗️ Magnificent! {score:F1}% mastery elevates you to Fortress Architect - your security designs create impenetrable digital strongholds!",
            "Incident Commander" => $"🎖️ Excellent! {score:F1}% mastery grants you Incident Commander status - your crisis leadership protects kingdoms during digital sieges!",
            "Pentest Paladin" => $"⚔️ Impressive! {score:F1}% mastery makes you a Pentest Paladin - your ethical hacking strengthens fortress walls!",
            "Vulnerability Vanquisher" => $"🛡️ Well done! {score:F1}% mastery crowns you Vulnerability Vanquisher - your weakness-hunting skills protect the realm!",
            "Security Sentinel" => $"🛡️ Good start! {score:F1}% mastery begins your journey as a Security Sentinel - the first guardian of digital safety!",
            _ => $"Keep building security knowledge - every defense lesson makes your digital fortress stronger!"
        };
    }

    /// <summary>
    /// Publish cloud lesson completion event with cross-module integration data
    /// </summary>
    private async Task PublishCloudLessonCompletionEvent(string userId, Lesson lesson, double quizScore, CancellationToken cancellationToken)
    {
        try
        {
            // ✅ PRE-CALCULATE ALL CONDITIONAL VALUES (eliminates CS8852 errors)
            var (enablesCloudAutomation, enablesCloudSecurity, unlocksMultiCloudManagement, unlocksHybridCloud) = GetCloudLessonBooleanProperties(lesson.LessonNumber, quizScore);
            var cloudConcepts = GetCloudConceptsForLesson(lesson.LessonNumber);
            var cloudScripts = GetCloudScriptsForLesson(lesson.LessonNumber);
            var azureAutomationOpportunities = GetAzureAutomationOpportunitiesForLesson(lesson.LessonNumber);
            var awsAutomationOpportunities = GetAWSAutomationOpportunitiesForLesson(lesson.LessonNumber);
            var gcpAutomationOpportunities = GetGCPAutomationOpportunitiesForLesson(lesson.LessonNumber);
            var networkTopologyPatterns = GetNetworkTopologyPatternsForCloudLesson(lesson.LessonNumber);
            var securityEnhancements = GetSecurityEnhancementsForCloudLesson(lesson.LessonNumber);
            var iacTemplates = GetIaCTemplatesForLesson(lesson.LessonNumber);

            // ✅ OBJECT INITIALIZER ONLY (no post-construction assignments)
            var cloudEvent = new CloudLessonCompletedEvent
            {
                UserId = userId,
                LessonId = lesson.Id,
                ModuleId = lesson.ModuleId,
                LessonNumber = lesson.LessonNumber,
                QuizScore = quizScore,
                CompletedAt = DateTime.UtcNow,
                EnablesCloudAutomation = enablesCloudAutomation,
                EnablesCloudSecurity = enablesCloudSecurity,
                UnlocksMultiCloudManagement = unlocksMultiCloudManagement,
                UnlocksHybridCloud = unlocksHybridCloud,
                WittyFeedback = GenerateCloudWittyFeedback(lesson.LessonNumber, quizScore),
                CloudConcepts = cloudConcepts,
                CloudScripts = cloudScripts,
                AzureAutomationOpportunities = azureAutomationOpportunities,
                AWSAutomationOpportunities = awsAutomationOpportunities,
                GCPAutomationOpportunities = gcpAutomationOpportunities,
                NetworkTopologyPatterns = networkTopologyPatterns,
                SecurityEnhancements = securityEnhancements,
                IaCTemplates = iacTemplates,
                IntegrationData = GetCloudIntegrationData(quizScore)
            };


            await _eventBus.PublishAsync<CloudLessonCompletedEvent>(cloudEvent, cancellationToken);

            _logger.LogDebug("Cloud lesson completion event published for user {UserId}, lesson {LessonId}",
                userId, lesson.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish cloud lesson completion event for user {UserId}, lesson {LessonId}",
                userId, lesson.Id);
        }
    }

    /// <summary>
    /// Check for cloud mastery achievement (Module 8)
    /// </summary>
    private async Task CheckCloudMasteryAchievement(string userId, UserModuleProgress moduleProgress, CancellationToken cancellationToken)
    {
        try
        {
            // Calculate overall cloud mastery score
            var cloudLessons = await _dbContext.UserLessonProgress
                .Where(ulp => ulp.UserId == userId && ulp.Lesson.ModuleId == 8 && ulp.IsCompleted)
                .Include(ulp => ulp.Lesson)
                .ToListAsync(cancellationToken);

            if (cloudLessons.Count < 20) // Ensure all 20 cloud lessons are completed
                return;

            var overallScore = cloudLessons.Average(l => l.QuizScore ?? 0);
            var excellentLessons = cloudLessons.Count(l => (l.QuizScore ?? 0) >= 80);

            // Determine cloud mastery level
            var masteryLevel = GetCloudMasteryLevel(overallScore, excellentLessons);

            var cloudMasteryEvent = new CloudMasteryAchievedEvent
            {
                UserId = userId,
                ModuleId = 8,
                OverallScore = overallScore,
                ExcellentLessons = excellentLessons,
                TotalTimeSpent = TimeSpan.FromMinutes(cloudLessons.Sum(l => l.TimeSpentMinutes)),
                MasteryLevel = masteryLevel,
                MasteredPlatforms = GetMasteredCloudPlatforms(overallScore),
                UnlockedSkills = GetUnlockedCloudSkills(masteryLevel),
                ArchitecturePatterns = GetMasteredArchitecturePatterns(masteryLevel),
                CertificationPathways = GetCloudCertificationPathways(masteryLevel),
                AchievedAt = DateTime.UtcNow,
                EnablesExpertFeatures = overallScore >= 90,
                UnlocksConsultingMode = overallScore >= 95,
                EnablesAdvancedOrchestration = excellentLessons >= 18,
                CosmicCongratulations = GenerateCosmicCloudMessage(masteryLevel, overallScore)
            };

            // Set mastery integrations
            cloudMasteryEvent.MasteryIntegrations.Add("MultiCloudManagement", overallScore >= 85);
            cloudMasteryEvent.MasteryIntegrations.Add("HybridCloudOrchestration", overallScore >= 90);
            cloudMasteryEvent.MasteryIntegrations.Add("CloudSecurityExpert", overallScore >= 88);
            cloudMasteryEvent.MasteryIntegrations.Add("InfrastructureAutomation", excellentLessons >= 15);

            await _eventBus.PublishAsync<CloudMasteryAchievedEvent>(cloudMasteryEvent, cancellationToken);

            _logger.LogInformation("Cloud mastery achieved by user {UserId} with {OverallScore}% mastery",
                userId, overallScore);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check cloud mastery achievement for user {UserId}", userId);
        }
    }

    /// <summary>
    /// Generate witty cloud feedback based on lesson and score
    /// </summary>
    private string GenerateCloudWittyFeedback(int lessonNumber, double score)
    {
        var (cloudTheme, concept) = lessonNumber switch
        {
            1 => ("floating forts", "cloud basics"),
            2 => ("infrastructure conquest", "IaaS mastery"),
            3 => ("platform power", "PaaS prowess"),
            4 => ("software skies", "SaaS savvy"),
            5 => ("Azure skies", "Microsoft mastery"),
            6 => ("Amazon heavens", "AWS expertise"),
            7 => ("Google clouds", "GCP genius"),
            8 => ("sky clones", "VM virtuosity"),
            9 => ("light containers", "containerization"),
            10 => ("magic functions", "serverless sorcery"),
            11 => ("sky vaults", "storage supremacy"),
            12 => ("virtual highways", "cloud networking"),
            13 => ("sky commands", "automation artistry"),
            14 => ("sky guards", "cloud security"),
            15 => ("fog clearing", "troubleshooting"),
            16 => ("hybrid harmony", "ground-sky mastery"),
            17 => ("sky economics", "cost optimization"),
            18 => ("edge wonders", "distributed dominance"),
            19 => ("architecture artistry", "design mastery"),
            20 => ("cloud conquest", "ultimate mastery"),
            _ => ("cloud mastery", "sky knowledge")
        };

        return score switch
        {
            >= 95 => $"☁️ Celestial conquest! {cloudTheme} mastered with {score:F0}% sky-high excellence - you reign supreme over the digital heavens!",
            >= 85 => $"🌤️ Excellent sky mastery! {concept} conquered with {score:F0}% - your cloud kingdom grows stronger!",
            >= 75 => $"⛅ Good cloud progress! {score:F0}% shows your {cloudTheme} skills are ascending - keep climbing!",
            >= 65 => $"🌥️ Cloudy but clearing! {score:F0}% - your {concept} understanding is breaking through the fog!",
            _ => $"⛈️ Storm clouds gathering! Don't worry - every cloud conqueror faces tempests. Rally your sky forces and try again!"
        };
    }

    /// <summary>
    /// Get cloud mastery level based on performance
    /// </summary>
    private string GetCloudMasteryLevel(double overallScore, int excellentLessons)
    {
        return (overallScore, excellentLessons) switch
        {
            (>= 95, >= 18) => "Cloud Conqueror Supreme",
            (>= 90, >= 16) => "Sky Architect Master",
            (>= 85, >= 14) => "Multi-Cloud Commander",
            (>= 80, >= 12) => "Infrastructure Virtuoso",
            (>= 75, >= 10) => "Platform Pioneer",
            (>= 70, >= 8) => "Cloud Navigator",
            _ => "Sky Explorer"
        };
    }

    /// <summary>
    /// Get mastered cloud platforms based on score
    /// </summary>
    private List<string> GetMasteredCloudPlatforms(double score)
    {
        var platforms = new List<string>();
        
        if (score >= 70) platforms.Add("Microsoft Azure");
        if (score >= 75) platforms.Add("Amazon Web Services");
        if (score >= 80) platforms.Add("Google Cloud Platform");
        if (score >= 85) platforms.Add("Multi-Cloud Management");
        if (score >= 90) platforms.Add("Hybrid Cloud Solutions");
        
        return platforms;
    }

    /// <summary>
    /// Get cloud lesson boolean properties based on lesson number and score
    /// </summary>
    private (bool enablesCloudAutomation, bool enablesCloudSecurity, bool unlocksMultiCloudManagement, bool unlocksHybridCloud) GetCloudLessonBooleanProperties(int lessonNumber, double quizScore)
    {
        return lessonNumber switch
        {
            1 => (quizScore >= 70, quizScore >= 75, quizScore >= 80, quizScore >= 85),
            2 => (quizScore >= 70, quizScore >= 75, quizScore >= 80, quizScore >= 85),
            3 => (quizScore >= 70, quizScore >= 75, quizScore >= 80, quizScore >= 85),
            4 => (quizScore >= 70, quizScore >= 75, quizScore >= 80, quizScore >= 85),
            5 => (quizScore >= 70, quizScore >= 75, quizScore >= 80, quizScore >= 85),
            6 => (quizScore >= 70, quizScore >= 75, quizScore >= 80, quizScore >= 85),
            7 => (quizScore >= 70, quizScore >= 75, quizScore >= 80, quizScore >= 85),
            8 => (quizScore >= 70, quizScore >= 75, quizScore >= 80, quizScore >= 85),
            9 => (quizScore >= 70, quizScore >= 75, quizScore >= 80, quizScore >= 85),
            10 => (quizScore >= 70, quizScore >= 75, quizScore >= 80, quizScore >= 85),
            11 => (quizScore >= 70, quizScore >= 75, quizScore >= 80, quizScore >= 85),
            12 => (quizScore >= 70, quizScore >= 75, quizScore >= 80, quizScore >= 85),
            13 => (quizScore >= 70, quizScore >= 75, quizScore >= 80, quizScore >= 85),
            14 => (quizScore >= 70, quizScore >= 75, quizScore >= 80, quizScore >= 85),
            15 => (quizScore >= 70, quizScore >= 75, quizScore >= 80, quizScore >= 85),
            16 => (quizScore >= 70, quizScore >= 75, quizScore >= 80, true), // Always unlocks hybrid for lesson 16
            17 => (quizScore >= 70, quizScore >= 75, quizScore >= 80, quizScore >= 85),
            18 => (quizScore >= 70, quizScore >= 75, quizScore >= 80, quizScore >= 85),
            19 => (quizScore >= 70, quizScore >= 75, true, quizScore >= 85), // Always unlocks multi-cloud for lesson 19
            20 => (true, true, true, true), // Mastery lesson unlocks everything
            _ => (quizScore >= 70, quizScore >= 75, quizScore >= 80, quizScore >= 85)
        };
    }

    /// <summary>
    /// Get cloud concepts for specific lesson number
    /// </summary>
    private List<string> GetCloudConceptsForLesson(int lessonNumber)
    {
        return lessonNumber switch
        {
            1 => new List<string> { "Cloud Computing Basics", "Public/Private/Hybrid Clouds", "Cloud Service Models" },
            2 => new List<string> { "Infrastructure as a Service", "Virtual Machines", "Cloud Resources" },
            3 => new List<string> { "Platform as a Service", "App Services", "Managed Platforms" },
            4 => new List<string> { "Software as a Service", "Cloud Applications", "Multi-Tenancy" },
            5 => new List<string> { "Microsoft Azure", "Azure Resource Manager", "Azure Services" },
            6 => new List<string> { "Amazon Web Services", "EC2", "S3", "IAM" },
            7 => new List<string> { "Google Cloud Platform", "Compute Engine", "Cloud Storage" },
            8 => new List<string> { "Virtual Machines", "Hypervisors", "VM Management" },
            9 => new List<string> { "Containers", "Docker", "Kubernetes" },
            10 => new List<string> { "Serverless Computing", "Functions as a Service", "Event-Driven Architecture" },
            11 => new List<string> { "Cloud Storage", "Object Storage", "Blob Storage" },
            12 => new List<string> { "Virtual Private Cloud", "Cloud Networking", "Software-Defined Networking" },
            13 => new List<string> { "Cloud Automation", "Infrastructure as Code", "Cloud CLI" },
            14 => new List<string> { "Cloud Security", "Identity and Access Management", "Cloud Compliance" },
            15 => new List<string> { "Cloud Monitoring", "Troubleshooting", "Performance Optimization" },
            16 => new List<string> { "Hybrid Cloud", "On-Premises Integration", "Cloud Connectivity" },
            17 => new List<string> { "Cloud Cost Management", "Resource Optimization", "Billing" },
            18 => new List<string> { "Edge Computing", "CDN", "Distributed Computing" },
            19 => new List<string> { "Cloud Architecture", "Enterprise Design", "Scalability Patterns" },
            20 => new List<string> { "Cloud Mastery", "Certification Readiness", "Expert Knowledge" },
            _ => new List<string>()
        };
    }

    /// <summary>
    /// Get cloud scripts for specific lesson number
    /// </summary>
    private List<string> GetCloudScriptsForLesson(int lessonNumber)
    {
        return lessonNumber switch
        {
            1 => new List<string> { "Get-AzContext" },
            2 => new List<string> { "New-AzVM", "Get-AzResourceGroup" },
            3 => new List<string>(),
            4 => new List<string>(),
            5 => new List<string> { "Connect-AzAccount", "Get-AzSubscription", "Get-AzResource" },
            6 => new List<string> { "aws ec2 describe-instances", "aws s3 ls", "aws iam list-users" },
            7 => new List<string> { "gcloud compute instances list", "gcloud projects list" },
            8 => new List<string>(),
            9 => new List<string>(),
            10 => new List<string>(),
            11 => new List<string>(),
            12 => new List<string>(),
            13 => new List<string> { "az", "aws", "gcloud", "terraform", "pulumi" },
            14 => new List<string>(),
            15 => new List<string>(),
            16 => new List<string>(),
            17 => new List<string> { "Get-AzConsumptionUsageDetail", "aws ce get-cost-and-usage" },
            18 => new List<string>(),
            19 => new List<string>(),
            20 => new List<string>(),
            _ => new List<string>()
        };
    }

    /// <summary>
    /// Get Azure automation opportunities for specific lesson number
    /// </summary>
    private List<string> GetAzureAutomationOpportunitiesForLesson(int lessonNumber)
    {
        return lessonNumber switch
        {
            1 => new List<string>(),
            2 => new List<string> { "VM Creation", "Resource Group Management" },
            3 => new List<string> { "App Service Deployment" },
            4 => new List<string>(),
            5 => new List<string> { "Resource Management", "Policy Deployment", "Cost Management" },
            6 => new List<string>(),
            7 => new List<string>(),
            8 => new List<string>(),
            9 => new List<string>(),
            10 => new List<string> { "Azure Functions" },
            11 => new List<string>(),
            12 => new List<string>(),
            13 => new List<string>(),
            14 => new List<string>(),
            15 => new List<string> { "Azure Monitor" },
            16 => new List<string>(),
            17 => new List<string>(),
            18 => new List<string>(),
            19 => new List<string>(),
            20 => new List<string>(),
            _ => new List<string>()
        };
    }

    /// <summary>
    /// Get AWS automation opportunities for specific lesson number
    /// </summary>
    private List<string> GetAWSAutomationOpportunitiesForLesson(int lessonNumber)
    {
        return lessonNumber switch
        {
            1 => new List<string>(),
            2 => new List<string>(),
            3 => new List<string> { "Elastic Beanstalk Management" },
            4 => new List<string>(),
            5 => new List<string>(),
            6 => new List<string> { "Instance Management", "S3 Operations", "IAM Configuration" },
            7 => new List<string>(),
            8 => new List<string>(),
            9 => new List<string>(),
            10 => new List<string> { "AWS Lambda" },
            11 => new List<string>(),
            12 => new List<string>(),
            13 => new List<string>(),
            14 => new List<string>(),
            15 => new List<string> { "CloudWatch" },
            16 => new List<string>(),
            17 => new List<string>(),
            18 => new List<string>(),
            19 => new List<string>(),
            20 => new List<string>(),
            _ => new List<string>()
        };
    }

    /// <summary>
    /// Get GCP automation opportunities for specific lesson number
    /// </summary>
    private List<string> GetGCPAutomationOpportunitiesForLesson(int lessonNumber)
    {
        return lessonNumber switch
        {
            1 => new List<string>(),
            2 => new List<string>(),
            3 => new List<string>(),
            4 => new List<string>(),
            5 => new List<string>(),
            6 => new List<string>(),
            7 => new List<string> { "VM Management", "Storage Operations" },
            8 => new List<string>(),
            9 => new List<string>(),
            10 => new List<string> { "Google Cloud Functions" },
            11 => new List<string>(),
            12 => new List<string>(),
            13 => new List<string>(),
            14 => new List<string>(),
            15 => new List<string> { "Cloud Logging" },
            16 => new List<string>(),
            17 => new List<string>(),
            18 => new List<string>(),
            19 => new List<string>(),
            20 => new List<string>(),
            _ => new List<string>()
        };
    }

    /// <summary>
    /// Get network topology patterns for cloud lessons
    /// </summary>
    private List<string> GetNetworkTopologyPatternsForCloudLesson(int lessonNumber)
    {
        return lessonNumber switch
        {
            1 => new List<string> { "Basic Cloud Architecture" },
            2 => new List<string>(),
            3 => new List<string>(),
            4 => new List<string>(),
            5 => new List<string>(),
            6 => new List<string>(),
            7 => new List<string>(),
            8 => new List<string> { "VM Network Design", "Virtual Subnets" },
            9 => new List<string>(),
            10 => new List<string>(),
            11 => new List<string>(),
            12 => new List<string> { "VPC Design", "Subnet Architecture", "Network Security Groups" },
            13 => new List<string>(),
            14 => new List<string> { "Zero Trust Architecture" },
            15 => new List<string>(),
            16 => new List<string> { "Hybrid Network Design", "Site-to-Site VPN" },
            17 => new List<string>(),
            18 => new List<string> { "Edge Network Architecture" },
            19 => new List<string> { "Multi-Region Architecture", "Load Balancer Design" },
            20 => new List<string>(),
            _ => new List<string>()
        };
    }

    /// <summary>
    /// Get security enhancements for cloud lessons
    /// </summary>
    private List<string> GetSecurityEnhancementsForCloudLesson(int lessonNumber)
    {
        return lessonNumber switch
        {
            1 => new List<string>(),
            2 => new List<string>(),
            3 => new List<string>(),
            4 => new List<string> { "SaaS Security Assessment" },
            5 => new List<string>(),
            6 => new List<string>(),
            7 => new List<string>(),
            8 => new List<string> { "VM Security Configuration" },
            9 => new List<string> { "Container Security Scanning" },
            10 => new List<string>(),
            11 => new List<string> { "Storage Encryption", "Access Control" },
            12 => new List<string> { "Cloud Network Security" },
            13 => new List<string>(),
            14 => new List<string> { "IAM Policies", "Security Baseline", "Compliance Monitoring" },
            15 => new List<string>(),
            16 => new List<string>(),
            17 => new List<string>(),
            18 => new List<string>(),
            19 => new List<string>(),
            20 => new List<string>(),
            _ => new List<string>()
        };
    }

    /// <summary>
    /// Get Infrastructure as Code templates for lesson
    /// </summary>
    private List<string> GetIaCTemplatesForLesson(int lessonNumber)
    {
        return lessonNumber switch
        {
            1 => new List<string>(),
            2 => new List<string>(),
            3 => new List<string> { "PaaS ARM Template" },
            4 => new List<string>(),
            5 => new List<string> { "Azure ARM Template" },
            6 => new List<string> { "AWS CloudFormation Template" },
            7 => new List<string> { "GCP Deployment Manager Template" },
            8 => new List<string>(),
            9 => new List<string> { "Docker Compose", "Kubernetes Manifest" },
            10 => new List<string>(),
            11 => new List<string> { "Storage Account Template" },
            12 => new List<string>(),
            13 => new List<string> { "Terraform Configuration", "Pulumi Script" },
            14 => new List<string>(),
            15 => new List<string>(),
            16 => new List<string>(),
            17 => new List<string>(),
            18 => new List<string>(),
            19 => new List<string>(),
            20 => new List<string>(),
            _ => new List<string>()
        };
    }

    /// <summary>
    /// Get cloud integration data
    /// </summary>
    private Dictionary<string, object> GetCloudIntegrationData(double quizScore)
    {
        return new Dictionary<string, object>
        {
            { "PowerShellModule", "Available" },
            { "SecurityModule", "Available" },
            { "NetworkModule", "Available" },
            { "CloudPlatforms", new[] { "Azure", "AWS", "GCP" } },
            { "EnableAdvancedFeatures", quizScore >= 80 }
        };
    }

    /// <summary>
    /// Get routing lesson boolean properties based on lesson number and score
    /// </summary>
    private (bool enables3DTopology, bool unlocksAdvancedRouting) GetRoutingLessonBooleanProperties(int lessonNumber, double quizScore)
    {
        return lessonNumber switch
        {
            1 => (quizScore >= 75, quizScore >= 85),
            2 => (quizScore >= 75, quizScore >= 85),
            3 => (true, quizScore >= 85), // Always enables 3D for lesson 3
            4 => (quizScore >= 75, quizScore >= 85),
            5 => (true, quizScore >= 85), // Always enables 3D for lesson 5
            6 => (quizScore >= 75, true), // Always unlocks advanced for lesson 6
            7 => (quizScore >= 75, quizScore >= 85),
            8 => (quizScore >= 75, quizScore >= 85),
            9 => (quizScore >= 75, quizScore >= 85),
            10 => (quizScore >= 75, quizScore >= 85),
            11 => (quizScore >= 75, quizScore >= 85),
            12 => (quizScore >= 75, quizScore >= 85),
            13 => (quizScore >= 75, true), // Always unlocks advanced for lesson 13
            14 => (quizScore >= 75, quizScore >= 85),
            15 => (quizScore >= 75, quizScore >= 85),
            16 => (quizScore >= 75, quizScore >= 85),
            17 => (quizScore >= 75, quizScore >= 80), // Different threshold for EIGRP
            18 => (quizScore >= 75, quizScore >= 85),
            19 => (quizScore >= 90, quizScore >= 90), // Higher threshold for advanced lessons
            20 => (quizScore >= 90, quizScore >= 90), // Quiz mastery
            _ => (quizScore >= 75, quizScore >= 85)
        };
    }

    /// <summary>
    /// Get routing concepts for specific lesson number
    /// </summary>
    private List<string> GetRoutingConceptsForLesson(int lessonNumber)
    {
        return lessonNumber switch
        {
            1 => new List<string> { "Routing Basics", "Path Selection", "Network Interconnection" },
            2 => new List<string> { "Static Routing", "Manual Configuration", "Fixed Paths" },
            3 => new List<string> { "Dynamic Routing", "Protocol Communication", "Adaptive Paths" },
            4 => new List<string> { "RIP Protocol", "Distance Vector", "Hop Count Metric" },
            5 => new List<string> { "OSPF Protocol", "Link State", "Dijkstra Algorithm", "Areas" },
            6 => new List<string> { "BGP Protocol", "Internet Routing", "AS Paths", "Policy Control" },
            7 => new List<string> { "Default Gateway", "Catch-All Route", "0.0.0.0/0" },
            8 => new List<string> { "Routing Tables", "Route Entries", "Destination Prefixes" },
            9 => new List<string> { "Route Metrics", "Path Cost", "Best Path Selection" },
            10 => new List<string> { "Network Convergence", "Protocol Agreement", "Topology Consistency" },
            11 => new List<string> { "Routing Loops", "TTL", "Loop Prevention", "Split Horizon" },
            12 => new List<string> { "Access Control Lists", "Route Filtering", "Security Policies" },
            13 => new List<string> { "Route Automation", "PowerShell Routing", "Script-Based Configuration" },
            14 => new List<string> { "Virtual Routing", "VRF", "Route Isolation", "Multi-Tenancy" },
            15 => new List<string> { "Network Troubleshooting", "Traceroute", "Path Analysis" },
            16 => new List<string> { "Routing Security", "Authentication", "Route Validation" },
            17 => new List<string> { "EIGRP", "Hybrid Protocol", "DUAL Algorithm", "Cisco Proprietary" },
            18 => new List<string> { "MPLS", "Label Switching", "Traffic Engineering", "Service Provider Networks" },
            19 => new List<string> { "Advanced Routing", "Certification Level", "Complex Topologies" },
            20 => new List<string> { "Advanced Routing", "Certification Level", "Complex Topologies" },
            _ => new List<string>()
        };
    }

    /// <summary>
    /// Get routing scripts for specific lesson number
    /// </summary>
    private List<string> GetRoutingScriptsForLesson(int lessonNumber)
    {
        return lessonNumber switch
        {
            1 => new List<string> { "Get-NetRoute | Format-Table" },
            2 => new List<string> { "New-NetRoute -DestinationPrefix '192.168.2.0/24' -NextHop '192.168.1.1'", "Get-NetRoute -DestinationPrefix '0.0.0.0/0'" },
            3 => new List<string>(),
            4 => new List<string> { "netsh interface ip show config" },
            5 => new List<string>(),
            6 => new List<string>(),
            7 => new List<string> { "route add 0.0.0.0 mask 0.0.0.0 192.168.1.1" },
            8 => new List<string> { "Get-NetRoute", "route print", "netstat -r" },
            9 => new List<string> { "Get-NetRoute | Select-Object DestinationPrefix, RouteMetric" },
            10 => new List<string>(),
            11 => new List<string>(),
            12 => new List<string>(),
            13 => new List<string> { 
                "New-NetRoute -DestinationPrefix $subnet -NextHop $gateway",
                "Remove-NetRoute -DestinationPrefix $subnet -Confirm:$false",
                "Test-NetConnection -ComputerName $destination -TraceRoute"
            },
            14 => new List<string>(),
            15 => new List<string> { 
                "Test-NetConnection -TraceRoute", "tracert", "pathping" 
            },
            16 => new List<string>(),
            17 => new List<string>(),
            18 => new List<string>(),
            19 => new List<string>(),
            20 => new List<string>(),
            _ => new List<string>()
        };
    }

    /// <summary>
    /// Get protocols covered for specific lesson number
    /// </summary>
    private List<string> GetProtocolsCoveredForLesson(int lessonNumber)
    {
        return lessonNumber switch
        {
            1 => new List<string>(),
            2 => new List<string>(),
            3 => new List<string> { "RIP", "OSPF", "BGP" },
            4 => new List<string> { "RIP" },
            5 => new List<string> { "OSPF" },
            6 => new List<string> { "BGP" },
            7 => new List<string>(),
            8 => new List<string>(),
            9 => new List<string>(),
            10 => new List<string>(),
            11 => new List<string>(),
            12 => new List<string>(),
            13 => new List<string>(),
            14 => new List<string>(),
            15 => new List<string>(),
            16 => new List<string>(),
            17 => new List<string> { "EIGRP" },
            18 => new List<string>(),
            19 => new List<string>(),
            20 => new List<string>(),
            _ => new List<string>()
        };
    }

    /// <summary>
    /// Get topology enhancements for routing lessons
    /// </summary>
    private List<string> GetTopologyEnhancementsForRoutingLesson(int lessonNumber)
    {
        return lessonNumber switch
        {
            1 => new List<string>(),
            2 => new List<string>(),
            3 => new List<string>(),
            4 => new List<string>(),
            5 => new List<string> { "Link State Database Visualization", "Area Hierarchy Display", "Shortest Path Highlighting" },
            6 => new List<string>(),
            7 => new List<string>(),
            8 => new List<string> { "Interactive Route Table Visualization" },
            9 => new List<string>(),
            10 => new List<string> { "Convergence Animation" },
            11 => new List<string>(),
            12 => new List<string>(),
            13 => new List<string>(),
            14 => new List<string> { "VRF Visualization with Color Coding" },
            15 => new List<string>(),
            16 => new List<string>(),
            17 => new List<string>(),
            18 => new List<string> { "MPLS Label Path Visualization" },
            19 => new List<string>(),
            20 => new List<string>(),
            _ => new List<string>()
        };
    }

    /// <summary>
    /// Get security considerations for routing lessons
    /// </summary>
    private List<string> GetSecurityConsiderationsForRoutingLesson(int lessonNumber)
    {
        return lessonNumber switch
        {
            1 => new List<string>(),
            2 => new List<string>(),
            3 => new List<string>(),
            4 => new List<string>(),
            5 => new List<string>(),
            6 => new List<string> { "BGP Route Hijacking Protection" },
            7 => new List<string>(),
            8 => new List<string>(),
            9 => new List<string>(),
            10 => new List<string>(),
            11 => new List<string>(),
            12 => new List<string> { "Route Advertisement Filtering", "Network Segmentation" },
            13 => new List<string>(),
            14 => new List<string>(),
            15 => new List<string>(),
            16 => new List<string> { "Route Authentication", "BGP Security", "RPKI Validation" },
            17 => new List<string>(),
            18 => new List<string>(),
            19 => new List<string>(),
            20 => new List<string>(),
            _ => new List<string>()
        };
    }

    /// <summary>
    /// Get troubleshooting tools for specific lesson number
    /// </summary>
    private List<string> GetTroubleshootingToolsForLesson(int lessonNumber)
    {
        return lessonNumber switch
        {
            1 => new List<string>(),
            2 => new List<string>(),
            3 => new List<string>(),
            4 => new List<string>(),
            5 => new List<string>(),
            6 => new List<string>(),
            7 => new List<string>(),
            8 => new List<string>(),
            9 => new List<string>(),
            10 => new List<string>(),
            11 => new List<string> { "Loop Detection", "TTL Analysis", "Path Tracing" },
            12 => new List<string>(),
            13 => new List<string>(),
            14 => new List<string>(),
            15 => new List<string> { 
                "Traceroute Analysis", "MTR Continuous Monitoring", "Route Path Visualization" 
            },
            16 => new List<string>(),
            17 => new List<string>(),
            18 => new List<string>(),
            19 => new List<string>(),
            20 => new List<string>(),
            _ => new List<string>()
        };
    }

    /// <summary>
    /// Get riddle feedback for lesson based on lesson number and score
    /// </summary>
    private string GetRiddleFeedbackForLesson(int lessonNumber, double quizScore)
    {
        return lessonNumber switch
        {
            1 => "🗺️ Path cleared! You've unlocked the mystery of network routing - the adventure begins!",
            2 => "🛤️ Golden path established! Your static route shortcut never changes direction!",
            3 => "🌟 Adaptive magic unlocked! Your network paths evolve like intelligent explorers!",
            4 => "🗣️ Village gossip mastered! RIP spreads route rumors every 30 seconds!",
            5 => "🗺️ Cartographer wisdom gained! OSPF maps reveal all network secrets!",
            6 => "🌍 Global networking mastery! BGP connects the world's digital realms!",
            7 => "🕳️ Safety net deployed! Unknown destinations funnel through your default route!",
            8 => "📋 Directory mastery achieved! Your routing tables organize all network destinations!",
            9 => "⚖️ Quality measurement perfected! Your metrics weigh paths like a cosmic scale!",
            10 => "🌪️ Convergence conducted! All routing protocols sing in perfect harmony!",
            11 => "♾️ Loop liberation achieved! Your TTL wisdom breaks infinite routing mazes!",
            12 => "🛡️ Security gates established! Your ACL guards protect network pathways!",
            13 => "🪄 Path spells mastered! Your PowerShell wand weaves perfect network routes!",
            14 => "🌌 Parallel universe navigation! VRF creates separate routing realms!",
            15 => "🔍 Detective skills activated! Your breadcrumb trails solve any routing mystery!",
            16 => "🛡️ Secure pathways fortified! Your routing protocols wear digital armor!",
            17 => "🤖 Hybrid intelligence unlocked! EIGRP combines the best of both worlds!",
            18 => "🏎️ Express lane mastery! MPLS labels speed packets through network highways!",
            19 => quizScore >= 90 
                ? "👑 Routing Riddle Master supreme! All network paths bow to your navigation wisdom!"
                : "🏆 Advanced routing knowledge grows! Continue solving riddles for mastery!",
            20 => quizScore >= 90 
                ? "👑 Routing Riddle Master supreme! All network paths bow to your navigation wisdom!"
                : "🏆 Advanced routing knowledge grows! Continue solving riddles for mastery!",
            _ => "🗺️ Routing wisdom grows with every solved riddle!"
        };
    }

    /// <summary>
    /// Get routing integration data
    /// </summary>
    private Dictionary<string, object> GetRoutingIntegrationData(Lesson lesson, bool enables3DTopology, bool unlocksAdvancedRouting)
    {
        var integrationData = new Dictionary<string, object>
        {
            { "LessonTitle", lesson.Title },
            { "Description", lesson.Description }
        };

        if (enables3DTopology)
        {
            integrationData.Add("TopologyModule", "Enable 3D routing visualization");
            integrationData.Add("ScannerModule", "Unlock routing-aware network discovery");
        }

        if (unlocksAdvancedRouting)
        {
            integrationData.Add("PowerShellModule", "Enable advanced routing scripts");
            integrationData.Add("SecurityModule", "Unlock routing security assessments");
            integrationData.Add("SSHModule", "Enable routing configuration via SSH");
        }

        return integrationData;
    }

    /// <summary>
    /// Get security lesson boolean properties based on lesson number and score
    /// </summary>
    private (bool enablesAdvancedScanning, bool unlocksPentestingFeatures) GetSecurityLessonBooleanProperties(int lessonNumber, double quizScore)
    {
        return lessonNumber switch
        {
            1 => (quizScore >= 85, quizScore >= 90),
            2 => (quizScore >= 85, quizScore >= 90),
            3 => (quizScore >= 85, quizScore >= 90),
            4 => (quizScore >= 85, quizScore >= 90),
            5 => (quizScore >= 85, quizScore >= 90),
            6 => (quizScore >= 85, quizScore >= 90),
            7 => (quizScore >= 85, quizScore >= 90),
            8 => (quizScore >= 85, quizScore >= 90),
            9 => (quizScore >= 85, quizScore >= 90),
            10 => (quizScore >= 85, quizScore >= 90),
            11 => (quizScore >= 85, quizScore >= 90),
            12 => (quizScore >= 85, quizScore >= 90),
            13 => (quizScore >= 85, quizScore >= 90),
            14 => (true, true), // Pen testing and beyond always enables
            15 => (true, true),
            16 => (true, true),
            17 => (true, true),
            18 => (true, true),
            19 => (true, true),
            20 => (true, true),
            _ => (lessonNumber >= 14, lessonNumber >= 14)
        };
    }

    /// <summary>
    /// Get security integration data
    /// </summary>
    private Dictionary<string, object> GetSecurityIntegrationData(Lesson lesson, double quizScore, string securityLevel)
    {
        return new Dictionary<string, object>
        {
            { "lesson_title", lesson.Title },
            { "quiz_score", quizScore },
            { "security_level", securityLevel }
        };
    }

    /// <summary>
    /// Get wireless concepts for specific lesson number
    /// </summary>
    private List<string> GetWirelessConceptsForLesson(int lessonNumber)
    {
        return lessonNumber switch
        {
            1 => new List<string> { "Wireless Fundamentals", "Electromagnetic Waves", "Data Transmission" },
            2 => new List<string> { "WiFi Basics", "Wireless Networking", "Home Networks" },
            13 => new List<string> { "Wireless Automation", "PowerShell Wireless", "Network Scripting" },
            14 => new List<string> { "Wireless Security", "MAC Filtering", "Access Control" },
            20 => new List<string> { "Complete Wireless Mastery", "Certification Readiness", "Expert Knowledge" },
            _ => new List<string>()
        };
    }

    /// <summary>
    /// Get suggested wireless scripts for specific lesson number
    /// </summary>
    private List<string> GetSuggestedWirelessScriptsForLesson(int lessonNumber)
    {
        return lessonNumber switch
        {
            1 => new List<string> { "Get-NetAdapter -Wireless" },
            2 => new List<string> { "netsh wlan show profiles", "Get-NetAdapter -Name WiFi" },
            13 => new List<string> { "netsh wlan export", "Get-NetConnectionProfile" },
            14 => new List<string>(),
            20 => new List<string>(),
            _ => new List<string>()
        };
    }

    /// <summary>
    /// Get recommended wireless cmdlets for specific lesson number
    /// </summary>
    private List<string> GetRecommendedWirelessCmdletsForLesson(int lessonNumber)
    {
        return lessonNumber switch
        {
            1 => new List<string>(),
            2 => new List<string>(),
            13 => new List<string> { "Get-NetAdapter", "Set-NetAdapter", "Get-WifiProfile" },
            14 => new List<string>(),
            20 => new List<string>(),
            _ => new List<string>()
        };
    }

    /// <summary>
    /// Get wireless security scans for specific lesson number
    /// </summary>
    private List<string> GetWirelessSecurityScansForLesson(int lessonNumber)
    {
        return lessonNumber switch
        {
            1 => new List<string>(),
            2 => new List<string>(),
            13 => new List<string>(),
            14 => new List<string> { "Rogue AP Detection", "Wireless Security Assessment", "MAC Address Analysis" },
            20 => new List<string>(),
            _ => new List<string>()
        };
    }

    /// <summary>
    /// Get advanced wireless concepts for specific lesson number
    /// </summary>
    private List<string> GetAdvancedWirelessConceptsForLesson(int lessonNumber)
    {
        return lessonNumber switch
        {
            1 => new List<string>(),
            2 => new List<string>(),
            13 => new List<string>(),
            14 => new List<string>(),
            20 => new List<string> { "Professional Wireless Design", "Enterprise Planning", "Certification Preparation" },
            _ => new List<string>()
        };
    }

    /// <summary>
    /// Get wireless integration data for lesson based on integration status and score
    /// </summary>
    private Dictionary<string, object> GetWirelessIntegrationDataForLesson(bool enablesIntegration, double quizScore)
    {
        var integrationData = new Dictionary<string, object>();

        if (enablesIntegration)
        {
            integrationData.Add("PowerShellAutomation", true);
            integrationData.Add("SecurityScanning", true);
            integrationData.Add("TopologyVisualization", true);
            integrationData.Add("CompletionLevel", GetWirelessCompletionLevel(quizScore));
        }

        return integrationData;
    }

    /// <summary>
    /// Get protocol alchemy boolean properties based on lesson number and score
    /// </summary>
    private (bool enablesAdvancedAutomation, bool enablesProtocolSecurity, bool unlocksMultiprotocolArchitecture, bool unlocksPerformanceOptimization) GetProtocolAlchemyBooleanProperties(int lessonNumber, double quizScore)
    {
        return lessonNumber switch
        {
            1 => (quizScore >= 75, quizScore >= 80, quizScore >= 85, quizScore >= 90),
            2 => (quizScore >= 75, quizScore >= 80, quizScore >= 85, quizScore >= 90),
            3 => (quizScore >= 75, quizScore >= 80, quizScore >= 85, quizScore >= 90),
            4 => (quizScore >= 75, quizScore >= 80, quizScore >= 85, quizScore >= 90),
            5 => (quizScore >= 75, quizScore >= 80, quizScore >= 85, quizScore >= 90),
            6 => (quizScore >= 75, quizScore >= 80, quizScore >= 85, quizScore >= 90),
            7 => (quizScore >= 75, quizScore >= 80, quizScore >= 85, quizScore >= 90),
            8 => (quizScore >= 75, quizScore >= 80, quizScore >= 85, quizScore >= 90),
            9 => (quizScore >= 75, quizScore >= 80, quizScore >= 85, quizScore >= 90),
            10 => (quizScore >= 75, quizScore >= 80, quizScore >= 85, quizScore >= 90),
            11 => (quizScore >= 75, quizScore >= 80, quizScore >= 85, quizScore >= 90),
            12 => (quizScore >= 75, quizScore >= 80, quizScore >= 85, quizScore >= 90),
            13 => (quizScore >= 75, quizScore >= 80, quizScore >= 85, quizScore >= 90),
            14 => (quizScore >= 75, quizScore >= 80, quizScore >= 85, quizScore >= 90),
            15 => (quizScore >= 75, quizScore >= 80, quizScore >= 85, quizScore >= 90),
            16 => (quizScore >= 75, quizScore >= 80, quizScore >= 85, quizScore >= 90),
            17 => (quizScore >= 75, quizScore >= 80, quizScore >= 85, quizScore >= 90),
            18 => (quizScore >= 75, quizScore >= 80, quizScore >= 85, quizScore >= 90),
            19 => (quizScore >= 75, quizScore >= 80, true, true), // Cert level unlocks architecture and optimization
            20 => (true, true, true, true), // Quiz mastery unlocks everything
            _ => (quizScore >= 75, quizScore >= 80, quizScore >= 85, quizScore >= 90)
        };
    }

    /// <summary>
    /// Get protocol concepts for alchemy lesson
    /// </summary>
    private List<string> GetProtocolConceptsForLesson(int lessonNumber)
    {
        return lessonNumber switch
        {
            1 => new List<string> { "Protocol Fundamentals", "Network Stack Basics", "Communication Layers" },
            2 => new List<string> { "Web Protocols", "SSL/TLS Security", "HTTP Methods" },
            3 => new List<string> { "File Transfer Protocols", "SSH Tunneling", "Secure File Operations" },
            4 => new List<string> { "SMTP", "POP3", "IMAP", "Email Security" },
            5 => new List<string> { "Domain Name System", "DNS Resolution", "Name Servers" },
            6 => new List<string> { "Dynamic Host Configuration", "IP Address Management", "Lease Management" },
            7 => new List<string> { "OpenVPN", "IPSec", "WireGuard", "Secure Tunneling" },
            8 => new List<string> { "Quality of Service", "Traffic Prioritization", "Bandwidth Management" },
            9 => new List<string> { "Load Balancing", "Traffic Distribution", "High Availability" },
            10 => new List<string> { "REST APIs", "GraphQL", "API Design", "Microservices" },
            11 => new List<string> { "WebSocket Protocol", "Real-Time Communication", "Bidirectional Data Flow" },
            12 => new List<string> { "Protocol Translation", "Bridge Design", "Multi-Protocol Networks" },
            13 => new List<string> { "Protocol Security", "TLS Implementation", "Secure Communications" },
            14 => new List<string> { "Protocol Analysis", "Network Troubleshooting", "Packet Inspection" },
            15 => new List<string> { "QUIC Protocol", "HTTP/3", "Next-Generation Protocols" },
            16 => new List<string> { "MQTT", "CoAP", "IoT Communications" },
            17 => new List<string> { "Cloud APIs", "Microservices", "Container Networking" },
            18 => new List<string> { "OSI Model", "TCP/IP Stack", "Protocol Layering" },
            19 => new List<string> { "Enterprise Protocol Design", "Advanced Architecture", "Performance Engineering" },
            20 => new List<string> { "Protocol Mastery", "Advanced Integration", "Expert Knowledge" },
            _ => new List<string>()
        };
    }

    /// <summary>
    /// Get protocol recipes for alchemy lesson 
    /// </summary>
    private List<string> GetProtocolRecipesForLesson(int lessonNumber)
    {
        return lessonNumber switch
        {
            1 => new List<string> { "Basic TCP/IP Blend" },
            2 => new List<string> { "Secure Web Elixir", "RESTful API Potion" },
            3 => new List<string> { "Secure File Transfer Brew", "Authenticated Transfer Elixir" },
            4 => new List<string> { "Secure Email Elixir", "Message Authentication Potion" },
            5 => new List<string> { "DNS Resolution Brew" },
            6 => new List<string> { "Address Assignment Elixir" },
            7 => new List<string> { "Secure Tunnel Brew", "Remote Access Elixir" },
            8 => new List<string> { "Traffic Priority Elixir" },
            9 => new List<string> { "Load Distribution Potion" },
            10 => new List<string> { "API Integration Elixir" },
            11 => new List<string> { "Real-Time Communication Elixir" },
            12 => new List<string> { "Protocol Bridge Brew" },
            13 => new List<string> { "Security Enhancement Elixir" },
            14 => new List<string> { "Diagnostic Analysis Potion" },
            15 => new List<string> { "Future Protocol Elixir" },
            16 => new List<string> { "IoT Communication Potion" },
            17 => new List<string> { "Cloud Integration Elixir" },
            18 => new List<string> { "Layered Stack Brew" },
            19 => new List<string> { "Master Alchemist Formula" },
            20 => new List<string> { "Grandmaster Elixir" },
            _ => new List<string>()
        };
    }

    /// <summary>
    /// Get integration patterns for alchemy lesson
    /// </summary>
    private List<string> GetIntegrationPatternsForLesson(int lessonNumber)
    {
        return lessonNumber switch
        {
            1 => new List<string> { "Layered Architecture" },
            2 => new List<string>(),
            3 => new List<string>(),
            4 => new List<string>(),
            5 => new List<string>(),
            6 => new List<string>(),
            7 => new List<string>(),
            8 => new List<string>(),
            9 => new List<string>(),
            10 => new List<string>(),
            11 => new List<string>(),
            12 => new List<string> { "Protocol Bridging", "Unified Communications" },
            13 => new List<string>(),
            14 => new List<string>(),
            15 => new List<string> { "Next-Gen Architecture" },
            16 => new List<string>(),
            17 => new List<string> { "Cloud-Native Architecture" },
            18 => new List<string> { "Stack Optimization" },
            19 => new List<string>(),
            20 => new List<string>(),
            _ => new List<string>()
        };
    }

    /// <summary>
    /// Get security enhancements for protocol alchemy lesson
    /// </summary>
    private List<string> GetSecurityEnhancementsForProtocolLesson(int lessonNumber)
    {
        return lessonNumber switch
        {
            1 => new List<string>(),
            2 => new List<string> { "Certificate Validation", "Encrypted Communications" },
            3 => new List<string> { "SSH Key Authentication" },
            4 => new List<string> { "Email Encryption" },
            5 => new List<string>(),
            6 => new List<string>(),
            7 => new List<string> { "VPN Security", "Tunnel Encryption" },
            8 => new List<string>(),
            9 => new List<string>(),
            10 => new List<string>(),
            11 => new List<string>(),
            12 => new List<string>(),
            13 => new List<string> { "Protocol Hardening", "Security Layers" },
            14 => new List<string>(),
            15 => new List<string>(),
            16 => new List<string>(),
            17 => new List<string>(),
            18 => new List<string>(),
            19 => new List<string>(),
            20 => new List<string>(),
            _ => new List<string>()
        };
    }

    /// <summary>
    /// Get automation scripts for alchemy lesson
    /// </summary>
    private List<string> GetAutomationScriptsForLesson(int lessonNumber)
    {
        return lessonNumber switch
        {
            1 => new List<string>(),
            2 => new List<string> { "Invoke-WebRequest" },
            3 => new List<string>(),
            4 => new List<string>(),
            5 => new List<string> { "Resolve-DnsName", "nslookup" },
            6 => new List<string> { "Get-DhcpServerv4Lease" },
            7 => new List<string>(),
            8 => new List<string>(),
            9 => new List<string>(),
            10 => new List<string> { "Invoke-RestMethod", "ConvertFrom-Json" },
            11 => new List<string>(),
            12 => new List<string>(),
            13 => new List<string>(),
            14 => new List<string>(),
            15 => new List<string>(),
            16 => new List<string>(),
            17 => new List<string>(),
            18 => new List<string>(),
            19 => new List<string>(),
            20 => new List<string>(),
            _ => new List<string>()
        };
    }

    /// <summary>
    /// Get optimization strategies for alchemy lesson
    /// </summary>
    private List<string> GetOptimizationStrategiesForLesson(int lessonNumber)
    {
        return lessonNumber switch
        {
            1 => new List<string>(),
            2 => new List<string>(),
            3 => new List<string>(),
            4 => new List<string>(),
            5 => new List<string>(),
            6 => new List<string>(),
            7 => new List<string>(),
            8 => new List<string> { "Bandwidth Optimization", "Latency Reduction" },
            9 => new List<string>(),
            10 => new List<string>(),
            11 => new List<string>(),
            12 => new List<string>(),
            13 => new List<string>(),
            14 => new List<string>(),
            15 => new List<string>(),
            16 => new List<string> { "Low-Power Networking" },
            17 => new List<string>(),
            18 => new List<string>(),
            19 => new List<string>(),
            20 => new List<string>(),
            _ => new List<string>()
        };
    }

    /// <summary>
    /// Get performance metrics for alchemy lesson
    /// </summary>
    private Dictionary<string, double> GetPerformanceMetricsForLesson(int lessonNumber)
    {
        return lessonNumber switch
        {
            1 => new Dictionary<string, double>(),
            2 => new Dictionary<string, double>(),
            3 => new Dictionary<string, double>(),
            4 => new Dictionary<string, double>(),
            5 => new Dictionary<string, double>(),
            6 => new Dictionary<string, double>(),
            7 => new Dictionary<string, double>(),
            8 => new Dictionary<string, double>(),
            9 => new Dictionary<string, double> { { "Throughput Monitoring", 95.5 }, { "Response Time Analysis", 89.2 } },
            10 => new Dictionary<string, double>(),
            11 => new Dictionary<string, double>(),
            12 => new Dictionary<string, double>(),
            13 => new Dictionary<string, double>(),
            14 => new Dictionary<string, double>(),
            15 => new Dictionary<string, double>(),
            16 => new Dictionary<string, double>(),
            17 => new Dictionary<string, double>(),
            18 => new Dictionary<string, double>(),
            19 => new Dictionary<string, double>(),
            20 => new Dictionary<string, double>(),
            _ => new Dictionary<string, double>()
        };
    }

    /// <summary>
    /// Get interoperability features for alchemy lesson
    /// </summary>
    private List<string> GetInteroperabilityFeaturesForLesson(int lessonNumber)
    {
        return lessonNumber switch
        {
            1 => new List<string>(),
            2 => new List<string>(),
            3 => new List<string>(),
            4 => new List<string>(),
            5 => new List<string>(),
            6 => new List<string>(),
            7 => new List<string>(),
            8 => new List<string>(),
            9 => new List<string>(),
            10 => new List<string>(),
            11 => new List<string> { "Cross-Platform Messaging" },
            12 => new List<string>(),
            13 => new List<string>(),
            14 => new List<string>(),
            15 => new List<string>(),
            16 => new List<string>(),
            17 => new List<string>(),
            18 => new List<string>(),
            19 => new List<string>(),
            20 => new List<string>(),
            _ => new List<string>()
        };
    }

    /// <summary>
    /// Get unlocked cloud skills based on mastery level
    /// </summary>
    private List<string> GetUnlockedCloudSkills(string masteryLevel)
    {
        return masteryLevel switch
        {
            "Cloud Conqueror Supreme" => new() { "Advanced Cloud Architecture", "Multi-Cloud Orchestration", "Enterprise Cloud Strategy", "Cloud Consulting Expertise", "Hybrid Cloud Mastery" },
            "Sky Architect Master" => new() { "Cloud Architecture Design", "Infrastructure as Code", "Cloud Security Expertise", "Cost Optimization", "Performance Tuning" },
            "Multi-Cloud Commander" => new() { "Multi-Platform Management", "Cloud Migration", "DevOps Integration", "Container Orchestration", "Serverless Architecture" },
            "Infrastructure Virtuoso" => new() { "IaaS Mastery", "VM Management", "Network Design", "Storage Solutions", "Backup Strategies" },
            "Platform Pioneer" => new() { "PaaS Deployment", "Application Services", "Database Management", "Integration Services", "Monitoring" },
            "Cloud Navigator" => new() { "Basic Cloud Management", "Resource Provisioning", "Service Configuration", "Basic Security", "Cost Awareness" },
            _ => new() { "Cloud Fundamentals", "Basic Service Usage" }
        };
    }

    /// <summary>
    /// Get mastered architecture patterns based on mastery level
    /// </summary>
    private List<string> GetMasteredArchitecturePatterns(string masteryLevel)
    {
        return masteryLevel switch
        {
            "Cloud Conqueror Supreme" => new() { "Multi-Cloud Architecture", "Hybrid Cloud Design", "Microservices Architecture", "Event-Driven Design", "Zero Trust Security" },
            "Sky Architect Master" => new() { "Scalable Cloud Architecture", "High Availability Design", "Disaster Recovery", "Load Balancing", "Auto-Scaling" },
            "Multi-Cloud Commander" => new() { "Cross-Platform Integration", "Container Architecture", "Serverless Patterns", "API Gateway Design", "Cloud-Native Design" },
            "Infrastructure Virtuoso" => new() { "Network Architecture", "Storage Architecture", "Compute Optimization", "Security Architecture", "Monitoring Design" },
            "Platform Pioneer" => new() { "Application Architecture", "Database Design", "Service Integration", "Basic Security Patterns", "Performance Patterns" },
            "Cloud Navigator" => new() { "Basic Cloud Patterns", "Simple Architectures", "Resource Organization", "Basic Networking", "Fundamental Security" },
            _ => new() { "Cloud Basics", "Simple Deployments" }
        };
    }

    /// <summary>
    /// Get cloud certification pathways based on mastery level
    /// </summary>
    private List<string> GetCloudCertificationPathways(string masteryLevel)
    {
        return masteryLevel switch
        {
            "Cloud Conqueror Supreme" => new() { "Azure Solutions Architect Expert", "AWS Solutions Architect Professional", "GCP Professional Cloud Architect", "Multi-Cloud Architect", "Cloud Consultant" },
            "Sky Architect Master" => new() { "Azure Solutions Architect Associate", "AWS Solutions Architect Associate", "GCP Professional Cloud Architect", "Cloud Security Specialist" },
            "Multi-Cloud Commander" => new() { "Azure Administrator", "AWS SysOps Administrator", "GCP Cloud Engineer", "DevOps Engineer", "Cloud Developer" },
            "Infrastructure Virtuoso" => new() { "Azure Fundamentals", "AWS Cloud Practitioner", "GCP Associate Cloud Engineer", "Cloud+ CompTIA" },
            "Platform Pioneer" => new() { "Azure Fundamentals", "AWS Cloud Practitioner", "GCP Cloud Digital Leader", "Cloud Essentials" },
            "Cloud Navigator" => new() { "Cloud Fundamentals", "Basic Cloud Certifications", "Vendor-Specific Basics" },
            _ => new() { "Cloud Introduction Courses", "Foundational Learning" }
        };
    }

    /// <summary>
    /// Generate cosmic cloud mastery message
    /// </summary>
    private static string GenerateCosmicCloudMessage(string masteryLevel, double score)
    {
        return masteryLevel switch
        {
            "Cloud Conqueror Supreme" => $"👑 ULTIMATE CLOUD SUPREMACY! With {score:F1}% sky-high mastery, you've ascended beyond mortal cloud limitations to become the supreme ruler of all digital heavens - every cloud bows to your infinite wisdom!",
            "Sky Architect Master" => $"🏗️ Magnificent! {score:F1}% mastery elevates you to Sky Architect Master - your cloud blueprints create ethereal kingdoms of infinite scalability!",
            "Multi-Cloud Commander" => $"🌐 Excellent! {score:F1}% mastery grants you Multi-Cloud Commander status - you orchestrate multiple cloud realms with masterful precision!",
            "Infrastructure Virtuoso" => $"🎼 Impressive! {score:F1}% mastery makes you an Infrastructure Virtuoso - your cloud symphonies harmonize servers across the digital cosmos!",
            "Platform Pioneer" => $"🚀 Great work! {score:F1}% mastery crowns you a Platform Pioneer - you blaze new trails through uncharted cloud territories!",
            "Cloud Navigator" => $"🧭 Well done! {score:F1}% mastery makes you a Cloud Navigator - your compass guides others through the misty cloud realms!",
            "Sky Explorer" => $"🌤️ Good start! {score:F1}% mastery begins your journey as a Sky Explorer - the first step toward cloud conquest greatness!",
            _ => $"Keep ascending through the cloud layers - every lesson lifts you higher toward sky-high mastery!"
        };
    }

    /// <summary>
    /// Publish protocol alchemy lesson completion event with cross-module integration data
    /// </summary>
    private async Task PublishProtocolAlchemyLessonCompletionEvent(string userId, Lesson lesson, double quizScore, CancellationToken cancellationToken)
    {
        try
        {
            // ✅ PRE-CALCULATE ALL CONDITIONAL VALUES (eliminates CS8852 errors)
            var (enablesAdvancedAutomation, enablesProtocolSecurity, unlocksMultiprotocolArchitecture, unlocksPerformanceOptimization) = GetProtocolAlchemyBooleanProperties(lesson.LessonNumber, quizScore);
            var alchemicalFeedback = GenerateProtocolAlchemyFeedback(lesson.LessonNumber, quizScore);
            var protocolConcepts = GetProtocolConceptsForLesson(lesson.LessonNumber);
            var protocolRecipes = GetProtocolRecipesForLesson(lesson.LessonNumber);
            var integrationPatterns = GetIntegrationPatternsForLesson(lesson.LessonNumber);
            var securityEnhancements = GetSecurityEnhancementsForProtocolLesson(lesson.LessonNumber);
            var automationScripts = GetAutomationScriptsForLesson(lesson.LessonNumber);
            var optimizationStrategies = GetOptimizationStrategiesForLesson(lesson.LessonNumber);
            var performanceMetrics = GetPerformanceMetricsForLesson(lesson.LessonNumber);
            var interoperabilityFeatures = GetInteroperabilityFeaturesForLesson(lesson.LessonNumber);

            // ✅ OBJECT INITIALIZER ONLY (no post-construction assignments)
            var alchemyEvent = new ProtocolAlchemyLessonCompletedEvent
            {
                UserId = userId,
                LessonId = lesson.Id,
                ModuleId = lesson.ModuleId,
                LessonNumber = lesson.LessonNumber,
                QuizScore = quizScore,
                CompletedAt = DateTime.UtcNow,
                EnablesAdvancedAutomation = enablesAdvancedAutomation,
                EnablesProtocolSecurity = enablesProtocolSecurity,
                UnlocksMultiprotocolArchitecture = unlocksMultiprotocolArchitecture,
                UnlocksPerformanceOptimization = unlocksPerformanceOptimization,
                AlchemicalFeedback = alchemicalFeedback,
                ProtocolConcepts = protocolConcepts,
                ProtocolRecipes = protocolRecipes,
                IntegrationPatterns = integrationPatterns,
                SecurityEnhancements = securityEnhancements,
                AutomationScripts = automationScripts,
                OptimizationStrategies = optimizationStrategies,
                PerformanceMetrics = performanceMetrics,
                InteroperabilityFeatures = interoperabilityFeatures
            };


            await _eventBus.PublishAsync<ProtocolAlchemyLessonCompletedEvent>(alchemyEvent, cancellationToken);

            _logger.LogDebug("Protocol alchemy lesson completion event published for user {UserId}, lesson {LessonId}",
                userId, lesson.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish protocol alchemy lesson completion event for user {UserId}, lesson {LessonId}",
                userId, lesson.Id);
        }
    }

    /// <summary>
    /// Check for protocol alchemy mastery achievement (Module 9)
    /// </summary>
    private async Task CheckProtocolAlchemyMasteryAchievement(string userId, UserModuleProgress moduleProgress, CancellationToken cancellationToken)
    {
        try
        {
            // Calculate overall protocol alchemy mastery score
            var alchemyLessons = await _dbContext.UserLessonProgress
                .Where(ulp => ulp.UserId == userId && ulp.Lesson.ModuleId == 9 && ulp.IsCompleted)
                .Include(ulp => ulp.Lesson)
                .ToListAsync(cancellationToken);

            if (alchemyLessons.Count < 20) // Ensure all 20 alchemy lessons are completed
                return;

            var overallScore = alchemyLessons.Average(l => l.QuizScore ?? 0);
            var excellentLessons = alchemyLessons.Count(l => (l.QuizScore ?? 0) >= 85);

            // Determine protocol alchemy mastery level
            var masteryLevel = GetProtocolAlchemyMasteryLevel(overallScore, excellentLessons);

            var alchemyMasteryEvent = new ProtocolAlchemyMasteredEvent
            {
                UserId = userId,
                ModuleId = 9,
                OverallScore = overallScore,
                ExcellentLessons = excellentLessons,
                TotalTimeSpent = TimeSpan.FromMinutes(alchemyLessons.Sum(l => l.TimeSpentMinutes)),
                MasteryLevel = masteryLevel,
                MasteredProtocolFamilies = GetMasteredProtocolFamilies(overallScore),
                UnlockedAlchemicalSkills = GetUnlockedAlchemicalSkills(masteryLevel),
                ArchitecturePatterns = GetMasteredProtocolArchitecturePatterns(masteryLevel),
                CertificationPathways = GetProtocolCertificationPathways(masteryLevel),
                TroubleshootingCapabilities = GetProtocolTroubleshootingCapabilities(masteryLevel),
                OptimizationTechniques = GetProtocolOptimizationTechniques(masteryLevel),
                AchievedAt = DateTime.UtcNow,
                EnablesExpertProtocolFeatures = overallScore >= 90,
                UnlocksArchitectureConsulting = overallScore >= 95,
                EnablesAdvancedOrchestration = excellentLessons >= 18,
                UnlocksProtocolResearch = overallScore >= 98,
                CosmicAlchemicalCongratulations = GenerateCosmicAlchemyMessage(masteryLevel, overallScore)
            };

            // Set mastery integrations
            alchemyMasteryEvent.MasteryIntegrations.Add("AdvancedProtocolDesign", overallScore >= 85);
            alchemyMasteryEvent.MasteryIntegrations.Add("MultiprotocolArchitecture", overallScore >= 90);
            alchemyMasteryEvent.MasteryIntegrations.Add("ProtocolSecurityExpert", overallScore >= 88);
            alchemyMasteryEvent.MasteryIntegrations.Add("NetworkAutomationMaster", excellentLessons >= 15);
            alchemyMasteryEvent.MasteryIntegrations.Add("ProtocolResearchCapable", overallScore >= 95);

            await _eventBus.PublishAsync<ProtocolAlchemyMasteredEvent>(alchemyMasteryEvent, cancellationToken);

            _logger.LogInformation("Protocol alchemy mastery achieved by user {UserId} with {OverallScore}% mastery",
                userId, overallScore);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check protocol alchemy mastery achievement for user {UserId}", userId);
        }
    }

    /// <summary>
    /// Generate witty protocol alchemy feedback based on lesson and score
    /// </summary>
    private string GenerateProtocolAlchemyFeedback(int lessonNumber, double score)
    {
        var (alchemyTheme, concept) = lessonNumber switch
        {
            1 => ("protocol blending", "mixing fundamentals"),
            2 => ("web elixirs", "HTTP/HTTPS mastery"),
            3 => ("file potions", "transfer protocols"),
            4 => ("email alchemy", "communication magic"),
            5 => ("name resolution", "DNS sorcery"),
            6 => ("auto-configuration", "DHCP wizardry"),
            7 => ("tunnel brewing", "VPN mastery"),
            8 => ("priority potions", "QoS excellence"),
            9 => ("monitoring magic", "SNMP mastery"),
            10 => ("voice blends", "VoIP expertise"),
            11 => ("protocol spells", "scripting alchemy"),
            12 => ("hybrid elixirs", "multiprotocol mastery"),
            13 => ("secure blends", "security alchemy"),
            14 => ("diagnostic brewing", "troubleshooting mastery"),
            15 => ("future potions", "emerging protocols"),
            16 => ("miniature elixirs", "IoT protocol mastery"),
            17 => ("sky-high mixes", "cloud protocol expertise"),
            18 => ("layered alchemy", "protocol stack mastery"),
            19 => ("master formulas", "advanced architecture"),
            20 => ("grandmaster elixir", "ultimate mastery"),
            _ => ("protocol mastery", "alchemical knowledge")
        };

        return score switch
        {
            >= 95 => $"🧪 Perfect transmutation! {alchemyTheme} perfected with {score:F0}% - you've achieved true alchemical mastery!",
            >= 85 => $"⚗️ Excellent brewing! {concept} mastered with {score:F0}% - your protocol potions are legendary!",
            >= 75 => $"🔮 Good alchemy progress! {score:F0}% shows your {alchemyTheme} skills are developing - keep brewing!",
            >= 65 => $"💫 Potion bubbling nicely! {score:F0}% - your {concept} understanding is taking shape!",
            _ => $"💨 Brew needs refinement! Every master alchemist has experiments that fizzle. Perfect your {alchemyTheme} and try again!"
        };
    }

    /// <summary>
    /// Get protocol alchemy mastery level based on performance
    /// </summary>
    private string GetProtocolAlchemyMasteryLevel(double overallScore, int excellentLessons)
    {
        return (overallScore, excellentLessons) switch
        {
            (>= 98, >= 19) => "Alchemy Grandmaster Supreme",
            (>= 95, >= 18) => "Grand Protocol Alchemist",
            (>= 90, >= 16) => "Master Protocol Architect",
            (>= 85, >= 14) => "Advanced Protocol Mixer",
            (>= 80, >= 12) => "Protocol Virtuoso",
            (>= 75, >= 10) => "Skilled Alchemist",
            (>= 70, >= 8) => "Protocol Apprentice",
            _ => "Novice Mixer"
        };
    }

    /// <summary>
    /// Get mastered protocol families based on score
    /// </summary>
    private List<string> GetMasteredProtocolFamilies(double score)
    {
        var families = new List<string>();
        
        if (score >= 70) families.Add("Web Protocols (HTTP/HTTPS)");
        if (score >= 75) families.Add("File Transfer Protocols (FTP/SFTP)");
        if (score >= 80) families.Add("Network Services (DNS/DHCP)");
        if (score >= 85) families.Add("Security Protocols (VPN/TLS)");
        if (score >= 90) families.Add("Advanced Protocols (QoS/SNMP)");
        if (score >= 95) families.Add("Emerging Technologies (QUIC/IoT)");
        
        return families;
    }

    /// <summary>
    /// Get unlocked alchemical skills based on mastery level
    /// </summary>
    private List<string> GetUnlockedAlchemicalSkills(string masteryLevel)
    {
        return masteryLevel switch
        {
            "Alchemy Grandmaster Supreme" => new() { "Ultimate Protocol Design", "Research-Level Expertise", "Industry Leadership", "Innovation Mastery", "Teaching Excellence" },
            "Grand Protocol Alchemist" => new() { "Advanced Protocol Architecture", "Expert Troubleshooting", "Performance Engineering", "Security Mastery", "Automation Excellence" },
            "Master Protocol Architect" => new() { "Complex Protocol Design", "Multi-layer Integration", "Advanced Security", "Performance Optimization", "Enterprise Architecture" },
            "Advanced Protocol Mixer" => new() { "Protocol Optimization", "Security Implementation", "Automation Scripting", "Advanced Troubleshooting", "Integration Patterns" },
            "Protocol Virtuoso" => new() { "Protocol Analysis", "Performance Tuning", "Basic Automation", "Security Awareness", "Integration Skills" },
            "Skilled Alchemist" => new() { "Protocol Configuration", "Basic Troubleshooting", "Security Implementation", "Monitoring Setup", "Documentation" },
            "Protocol Apprentice" => new() { "Basic Protocol Knowledge", "Simple Configuration", "Basic Troubleshooting", "Security Awareness", "Learning Foundations" },
            _ => new() { "Protocol Fundamentals", "Basic Understanding" }
        };
    }

    /// <summary>
    /// Get mastered protocol architecture patterns based on mastery level
    /// </summary>
    private List<string> GetMasteredProtocolArchitecturePatterns(string masteryLevel)
    {
        return masteryLevel switch
        {
            "Alchemy Grandmaster Supreme" => new() { "Advanced Microservices", "Zero-Trust Architecture", "Edge Computing Protocols", "AI-Driven Protocol Optimization", "Quantum-Safe Protocols" },
            "Grand Protocol Alchemist" => new() { "Microservices Architecture", "Service Mesh", "API Gateway Patterns", "Event-Driven Architecture", "Cloud-Native Protocols" },
            "Master Protocol Architect" => new() { "Layered Architecture", "Protocol Bridging", "Load Balancing", "High Availability", "Disaster Recovery" },
            "Advanced Protocol Mixer" => new() { "Multi-Protocol Integration", "Security Layers", "Performance Optimization", "Monitoring Architecture", "Automation Patterns" },
            "Protocol Virtuoso" => new() { "Basic Architecture Patterns", "Protocol Stacks", "Simple Integration", "Basic Security", "Monitoring Setup" },
            "Skilled Alchemist" => new() { "Standard Protocols", "Basic Integration", "Security Implementation", "Simple Monitoring", "Documentation Patterns" },
            "Protocol Apprentice" => new() { "Protocol Basics", "Simple Configurations", "Basic Security", "Learning Patterns", "Foundation Knowledge" },
            _ => new() { "Basic Understanding", "Fundamental Concepts" }
        };
    }

    /// <summary>
    /// Get protocol certification pathways based on mastery level
    /// </summary>
    private List<string> GetProtocolCertificationPathways(string masteryLevel)
    {
        return masteryLevel switch
        {
            "Alchemy Grandmaster Supreme" => new() { "CCIE Enterprise", "CISSP", "SABSA", "Custom Certification Development", "Industry Expert Recognition" },
            "Grand Protocol Alchemist" => new() { "CCNP Enterprise", "CISSP Associate", "CISM", "Certified Network Architect", "Protocol Specialist" },
            "Master Protocol Architect" => new() { "CCNA Enterprise", "Security+", "Network+", "Certified Security Analyst", "Cloud Architect" },
            "Advanced Protocol Mixer" => new() { "CCNA", "Network+", "Security Fundamentals", "Cloud Practitioner", "Protocol Analyst" },
            "Protocol Virtuoso" => new() { "CompTIA Network+", "Security Fundamentals", "Cloud Essentials", "Protocol Technician", "IT Specialist" },
            "Skilled Alchemist" => new() { "Network Fundamentals", "IT Fundamentals", "Security Awareness", "Basic Certifications", "Foundation Learning" },
            "Protocol Apprentice" => new() { "Entry-Level IT", "Network Basics", "Foundation Courses", "Learning Pathways", "Skill Development" },
            _ => new() { "Beginner IT Courses", "Foundation Learning" }
        };
    }

    /// <summary>
    /// Get protocol troubleshooting capabilities based on mastery level
    /// </summary>
    private List<string> GetProtocolTroubleshootingCapabilities(string masteryLevel)
    {
        return masteryLevel switch
        {
            "Alchemy Grandmaster Supreme" => new() { "Advanced Protocol Analysis", "Performance Forensics", "Security Incident Response", "Root Cause Analysis", "Predictive Troubleshooting" },
            "Grand Protocol Alchemist" => new() { "Expert Packet Analysis", "Complex Problem Solving", "Advanced Diagnostics", "Performance Analysis", "Security Investigations" },
            "Master Protocol Architect" => new() { "Protocol Debugging", "Network Analysis", "Performance Monitoring", "Issue Resolution", "Advanced Tools Usage" },
            "Advanced Protocol Mixer" => new() { "Packet Inspection", "Basic Analysis", "Monitoring Setup", "Issue Identification", "Tool Utilization" },
            "Protocol Virtuoso" => new() { "Basic Troubleshooting", "Simple Analysis", "Monitoring Awareness", "Problem Recognition", "Tool Familiarity" },
            "Skilled Alchemist" => new() { "Basic Diagnostics", "Simple Tools", "Problem Recognition", "Documentation Review", "Learning Resources" },
            _ => new() { "Basic Understanding", "Simple Problem Recognition" }
        };
    }

    /// <summary>
    /// Get protocol optimization techniques based on mastery level
    /// </summary>
    private List<string> GetProtocolOptimizationTechniques(string masteryLevel)
    {
        return masteryLevel switch
        {
            "Alchemy Grandmaster Supreme" => new() { "AI-Driven Optimization", "Predictive Performance", "Advanced Caching", "Protocol Innovation", "Custom Solutions" },
            "Grand Protocol Alchemist" => new() { "Advanced Performance Tuning", "Complex Optimization", "Multi-Protocol Optimization", "Advanced Caching", "Load Optimization" },
            "Master Protocol Architect" => new() { "Performance Analysis", "Protocol Tuning", "Bandwidth Optimization", "Latency Reduction", "Throughput Enhancement" },
            "Advanced Protocol Mixer" => new() { "Basic Performance Tuning", "Simple Optimization", "Monitoring Setup", "Configuration Optimization", "Basic Analysis" },
            "Protocol Virtuoso" => new() { "Performance Awareness", "Basic Monitoring", "Simple Tuning", "Configuration Basics", "Performance Metrics" },
            _ => new() { "Basic Understanding", "Fundamental Concepts" }
        };
    }

    /// <summary>
    /// Generate cosmic protocol alchemy mastery message
    /// </summary>
    private static string GenerateCosmicAlchemyMessage(string masteryLevel, double score)
    {
        return masteryLevel switch
        {
            "Alchemy Grandmaster Supreme" => $"👑 SUPREME ALCHEMICAL MASTERY! With {score:F1}% legendary brewing excellence, you've transcended all protocol limitations to become the ultimate network alchemist - every protocol bows to your infinite wisdom!",
            "Grand Protocol Alchemist" => $"🧪 Magnificent! {score:F1}% mastery elevates you to Grand Protocol Alchemist - your protocol potions create network magic beyond mortal comprehension!",
            "Master Protocol Architect" => $"⚗️ Excellent! {score:F1}% mastery grants you Master Protocol Architect status - you design protocol architectures with masterful precision!",
            "Advanced Protocol Mixer" => $"🔮 Impressive! {score:F1}% mastery makes you an Advanced Protocol Mixer - your alchemical blends solve complex network challenges!",
            "Protocol Virtuoso" => $"🌟 Great work! {score:F1}% mastery crowns you a Protocol Virtuoso - your mixing skills create elegant network solutions!",
            "Skilled Alchemist" => $"💫 Well done! {score:F1}% mastery makes you a Skilled Alchemist - your protocol knowledge grows ever stronger!",
            "Protocol Apprentice" => $"🌱 Good start! {score:F1}% mastery begins your journey as a Protocol Apprentice - the first step toward alchemical greatness!",
            _ => $"Keep brewing and learning - every protocol lesson brings you closer to alchemical mastery!"
        };
    }

    /// <summary>
    /// Publish mastery mayhem lesson completion event for comprehensive cross-module integration
    /// </summary>
    private async Task PublishMasteryMayhemLessonCompletionEvent(string userId, Lesson lesson, double quizScore, CancellationToken cancellationToken)
    {
        try
        {
            var masteryEvent = new MasteryMayhemLessonCompletedEvent
            {
                UserId = userId,
                LessonId = lesson.Id,
                ModuleId = lesson.ModuleId,
                LessonNumber = lesson.LessonNumber,
                QuizScore = quizScore,
                MasteryConcepts = GetMasteryConcepts(lesson.LessonNumber),
                IntegratedKnowledgeAreas = GetIntegratedKnowledgeAreas(lesson.LessonNumber),
                ProjectTemplates = GetProjectTemplates(lesson.LessonNumber),
                AutomationCapabilities = GetAutomationCapabilities(lesson.LessonNumber),
                TroubleshootingMastery = GetTroubleshootingMastery(lesson.LessonNumber),
                SecurityImplementations = GetSecurityImplementations(lesson.LessonNumber),
                ArchitecturePatterns = GetArchitecturePatterns(lesson.LessonNumber),
                CertificationPathways = GetCertificationPathways(lesson.LessonNumber),
                CompletedAt = DateTime.UtcNow,
                EnablesComprehensiveAutomation = lesson.LessonNumber >= 10,
                UnlocksEnterpriseConsulting = lesson.LessonNumber >= 15,
                EnablesAdvancedSecurity = lesson.LessonNumber >= 12,
                UnlocksProfessionalLevel = lesson.LessonNumber >= 18,
                EnablesRealWorldProjects = lesson.LessonNumber >= 8,
                UltimateIntegrationData = GetUltimateIntegrationData(lesson.LessonNumber, quizScore),
                MasteryMayhemFeedback = GenerateMasteryMayhemFeedback(lesson.LessonNumber, quizScore)
            };

            await _eventBus.PublishAsync<MasteryMayhemLessonCompletedEvent>(masteryEvent, cancellationToken);

            _logger.LogDebug("Published mastery mayhem lesson completion event for user {UserId}, lesson {LessonId} with {Score}% mastery score",
                userId, lesson.Id, quizScore);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish mastery mayhem lesson completion event for user {UserId}, lesson {LessonId}",
                userId, lesson.Id);
        }
    }

    /// <summary>
    /// Check for Engineer Extraordinaire achievement - the ultimate network engineering mastery
    /// </summary>
    private async Task CheckEngineerExtraordinaireAchievement(string userId, ModuleProgress moduleProgress, CancellationToken cancellationToken)
    {
        try
        {
            // Get all user's module progress to calculate overall mastery
            var allModuleProgress = await _dbContext.ModuleProgress
                .Where(mp => mp.UserId == userId && mp.CompletedAt.HasValue)
                .ToListAsync(cancellationToken);

            if (allModuleProgress.Count < 10) // Must complete all 10 modules
            {
                _logger.LogInformation("User {UserId} hasn't completed all modules yet - {CompletedCount}/10 modules completed",
                    userId, allModuleProgress.Count);
                return;
            }

            // Calculate overall mastery score
            var allLessonProgress = await _dbContext.LessonProgress
                .Where(lp => lp.UserId == userId && lp.Status == LessonStatus.Completed)
                .ToListAsync(cancellationToken);

            var overallMasteryScore = allLessonProgress.Any() ? allLessonProgress.Average(lp => lp.QuizScore) : 0;
            var excellentLessons = allLessonProgress.Count(lp => lp.QuizScore >= 90);
            var totalMasteryTime = TimeSpan.FromMinutes(allLessonProgress.Sum(lp => lp.TimeSpentMinutes));

            var extraordinaireLevel = GetExtraordinaireLevel(overallMasteryScore);
            
            var masteryAchievementEvent = new EngineerExtraordinaireAchievedEvent
            {
                UserId = userId,
                ModuleId = 10,
                OverallMasteryScore = overallMasteryScore,
                ExcellentLessons = excellentLessons,
                TotalMasteryTime = totalMasteryTime,
                ExtraordinaireLevel = extraordinaireLevel,
                MasteredDomains = GetMasteredDomains(),
                UltimateEngineeringSkills = GetUltimateEngineeringSkills(extraordinaireLevel),
                ProjectCapabilities = GetProjectCapabilities(extraordinaireLevel),
                DiagnosticExpertise = GetDiagnosticExpertise(extraordinaireLevel),
                ArchitectureMastery = GetArchitectureMastery(extraordinaireLevel),
                CertificationReadiness = GetCertificationReadiness(extraordinaireLevel),
                LeadershipCapabilities = GetLeadershipCapabilities(extraordinaireLevel),
                AchievedAt = DateTime.UtcNow,
                EnablesIndustryConsulting = overallMasteryScore >= 95,
                UnlocksAdvancedRnD = overallMasteryScore >= 98,
                EnablesArchitectureDesign = overallMasteryScore >= 90,
                UnlocksTeachingMentoring = overallMasteryScore >= 92,
                EnablesIndustryLeadership = overallMasteryScore >= 96,
                BadgeCollection = await GetBadgeCollection(userId, cancellationToken),
                UltimateCosmicCongratulations = GenerateUltimateCosmicCongratulations(extraordinaireLevel, overallMasteryScore),
                UltimateMasteryIntegrations = GetUltimateMasteryIntegrations(extraordinaireLevel, overallMasteryScore)
            };

            await _eventBus.PublishAsync<EngineerExtraordinaireAchievedEvent>(masteryAchievementEvent, cancellationToken);

            _logger.LogInformation("ENGINEER EXTRAORDINAIRE ACHIEVED! User {UserId} has achieved ultimate mastery with {Score}% overall score and {ExcellentCount} excellent lessons",
                userId, overallMasteryScore, excellentLessons);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check Engineer Extraordinaire achievement for user {UserId}",
                userId);
        }
    }

    /// <summary>
    /// Get mastery concepts for a Module 10 lesson
    /// </summary>
    private List<string> GetMasteryConcepts(int lessonNumber)
    {
        return lessonNumber switch
        {
            1 => new() { "Network Fundamentals Synthesis", "Physical Layer Mastery", "Protocol Stack Integration" },
            2 => new() { "Switch Mastery Synthesis", "VLAN Advanced Integration", "STP Optimization Techniques" },
            3 => new() { "IP Address Space Mastery", "Subnet Design Perfection", "Routing Table Optimization" },
            4 => new() { "Automation Script Excellence", "PowerShell Network Integration", "Infrastructure as Code" },
            5 => new() { "Advanced Routing Synthesis", "Protocol Convergence Mastery", "Dynamic Routing Optimization" },
            6 => new() { "Security Framework Integration", "Advanced Threat Mitigation", "Zero Trust Architecture" },
            7 => new() { "Wireless Technology Synthesis", "RF Optimization Mastery", "Enterprise Wireless Design" },
            8 => new() { "Cloud Integration Excellence", "Hybrid Architecture Mastery", "Multi-Cloud Optimization" },
            9 => new() { "Protocol Synthesis Mastery", "Advanced Integration Techniques", "Performance Optimization" },
            _ when lessonNumber <= 20 => new() { "Comprehensive Integration", "Advanced Synthesis", "Professional Mastery" },
            _ => new() { "Mastery Concepts" }
        };
    }

    /// <summary>
    /// Get integrated knowledge areas for cross-module synthesis
    /// </summary>
    private List<string> GetIntegratedKnowledgeAreas(int lessonNumber)
    {
        var baseAreas = new List<string> { "Network Fundamentals", "Switching Technologies", "IP Addressing", "Routing Protocols", "Network Security" };
        
        if (lessonNumber >= 8) baseAreas.AddRange(new[] { "Automation & Scripting", "Wireless Networks", "Cloud Technologies", "Advanced Protocols" });
        if (lessonNumber >= 15) baseAreas.AddRange(new[] { "Enterprise Architecture", "Performance Optimization", "Professional Certification" });
        
        return baseAreas;
    }

    /// <summary>
    /// Get project templates unlocked by mastery lessons
    /// </summary>
    private List<string> GetProjectTemplates(int lessonNumber)
    {
        return lessonNumber switch
        {
            <= 5 => new() { "Basic Network Design Template", "Fundamental Configuration Scripts" },
            <= 10 => new() { "Enterprise Network Template", "Advanced Configuration Automation", "Security Implementation Template" },
            <= 15 => new() { "Multi-Site Network Design", "Cloud Integration Template", "Comprehensive Security Framework" },
            _ => new() { "Ultimate Network Architecture Template", "Complete Automation Framework", "Professional Consulting Template" }
        };
    }

    /// <summary>
    /// Get automation capabilities enabled by mastery progression
    /// </summary>
    private List<string> GetAutomationCapabilities(int lessonNumber)
    {
        var capabilities = new List<string> { "Basic Configuration Automation", "Network Discovery Scripts" };
        
        if (lessonNumber >= 5) capabilities.AddRange(new[] { "Advanced PowerShell Automation", "Multi-Vendor Configuration" });
        if (lessonNumber >= 10) capabilities.AddRange(new[] { "Infrastructure as Code", "Automated Testing", "Performance Monitoring" });
        if (lessonNumber >= 15) capabilities.AddRange(new[] { "AI-Driven Optimization", "Predictive Analytics", "Self-Healing Networks" });
        
        return capabilities;
    }

    /// <summary>
    /// Get troubleshooting mastery techniques
    /// </summary>
    private List<string> GetTroubleshootingMastery(int lessonNumber)
    {
        return lessonNumber switch
        {
            <= 5 => new() { "Systematic Problem Analysis", "Layer-by-Layer Diagnosis", "Root Cause Identification" },
            <= 10 => new() { "Advanced Diagnostic Tools", "Performance Analysis", "Automated Problem Detection" },
            <= 15 => new() { "Predictive Problem Prevention", "AI-Assisted Diagnosis", "Complex System Analysis" },
            _ => new() { "Ultimate Diagnostic Mastery", "Preemptive Problem Resolution", "Enterprise-Level Troubleshooting" }
        };
    }

    /// <summary>
    /// Get security implementations unlocked
    /// </summary>
    private List<string> GetSecurityImplementations(int lessonNumber)
    {
        var implementations = new List<string> { "Basic Access Control", "Fundamental Security Policies" };
        
        if (lessonNumber >= 6) implementations.AddRange(new[] { "Advanced Threat Detection", "Zero Trust Principles", "Security Automation" });
        if (lessonNumber >= 12) implementations.AddRange(new[] { "AI-Powered Security", "Advanced Threat Hunting", "Compliance Frameworks" });
        if (lessonNumber >= 18) implementations.AddRange(new[] { "Enterprise Security Architecture", "Security Consulting", "Incident Response Leadership" });
        
        return implementations;
    }

    /// <summary>
    /// Get architecture patterns mastered
    /// </summary>
    private List<string> GetArchitecturePatterns(int lessonNumber)
    {
        return lessonNumber switch
        {
            <= 8 => new() { "Hierarchical Design", "Redundancy Patterns", "Scalability Principles" },
            <= 15 => new() { "Hybrid Cloud Architecture", "Software-Defined Networking", "Microservices Integration" },
            _ => new() { "Enterprise Architecture Mastery", "Innovation Leadership Patterns", "Future-Proof Design" }
        };
    }

    /// <summary>
    /// Get certification pathways opened
    /// </summary>
    private List<string> GetCertificationPathways(int lessonNumber)
    {
        var pathways = new List<string> { "CCNA Preparation", "Network+ Readiness" };
        
        if (lessonNumber >= 10) pathways.AddRange(new[] { "CCNP Pathway", "Advanced Specialization Tracks" });
        if (lessonNumber >= 15) pathways.AddRange(new[] { "CCIE Track", "Professional Architect Certifications" });
        if (lessonNumber >= 18) pathways.AddRange(new[] { "Expert-Level Certifications", "Industry Leadership Programs" });
        
        return pathways;
    }

    /// <summary>
    /// Get ultimate integration data for cross-module functionality
    /// </summary>
    private Dictionary<string, object> GetUltimateIntegrationData(int lessonNumber, double quizScore)
    {
        return new Dictionary<string, object>
        {
            ["CrossModuleIntegration"] = true,
            ["MasteryLevel"] = GetMasteryLevel(quizScore),
            ["UnlockedCapabilities"] = GetUnlockedCapabilities(lessonNumber, quizScore),
            ["IntegrationScore"] = CalculateIntegrationScore(lessonNumber, quizScore),
            ["AdvancedFeatures"] = lessonNumber >= 10 && quizScore >= 85,
            ["ProfessionalReadiness"] = lessonNumber >= 15 && quizScore >= 90,
            ["ConsultingCapability"] = lessonNumber >= 18 && quizScore >= 95,
            ["TeachingReadiness"] = lessonNumber >= 12 && quizScore >= 92
        };
    }

    /// <summary>
    /// Generate mastery mayhem feedback message
    /// </summary>
    private string GenerateMasteryMayhemFeedback(int lessonNumber, double quizScore)
    {
        return quizScore switch
        {
            >= 98 => $"🌟 ULTIMATE MASTERY MAYHEM! Lesson {lessonNumber} conquered with {quizScore:F1}% supreme excellence - you're transcending engineering limitations!",
            >= 95 => $"👑 Extraordinary! {quizScore:F1}% mastery in lesson {lessonNumber} showcases your engineer extraordinaire potential!",
            >= 90 => $"🔥 Excellent mastery! {quizScore:F1}% in lesson {lessonNumber} demonstrates advanced engineering prowess!",
            >= 85 => $"⚡ Great integration! {quizScore:F1}% mastery in lesson {lessonNumber} shows solid synthesis skills!",
            >= 80 => $"🌟 Good synthesis! {quizScore:F1}% in lesson {lessonNumber} indicates growing mastery capabilities!",
            _ => $"Keep pushing forward! Lesson {lessonNumber} mastery grows with each attempt - engineering greatness awaits!"
        };
    }

    /// <summary>
    /// Get Engineer Extraordinaire level based on overall mastery score
    /// </summary>
    private string GetExtraordinaireLevel(double overallScore)
    {
        return overallScore switch
        {
            >= 98 => "Network Overlord Supreme",
            >= 95 => "Grand Master Engineer",
            >= 92 => "Master Engineer Extraordinaire",
            >= 88 => "Senior Engineer Extraordinaire",
            >= 85 => "Engineer Extraordinaire",
            _ => "Advanced Network Professional"
        };
    }

    /// <summary>
    /// Get all mastered domains from Modules 1-10
    /// </summary>
    private List<string> GetMasteredDomains()
    {
        return new()
        {
            "Network Fundamentals & Physical Layer",
            "Switching Technologies & VLANs",
            "IP Addressing & Subnetting",
            "Network Automation & Scripting",
            "Routing Protocols & Algorithms",
            "Network Security & Defense",
            "Wireless Technologies & RF",
            "Cloud Networking & Integration",
            "Advanced Protocol Synthesis",
            "Mastery Integration & Leadership"
        };
    }

    /// <summary>
    /// Get ultimate engineering skills unlocked
    /// </summary>
    private List<string> GetUltimateEngineeringSkills(string extraordinaireLevel)
    {
        return extraordinaireLevel switch
        {
            "Network Overlord Supreme" => new() { "AI-Driven Network Design", "Quantum Network Preparation", "Industry Innovation Leadership", "Global Architecture Consulting", "Advanced R&D Capabilities" },
            "Grand Master Engineer" => new() { "Enterprise Architecture Mastery", "Advanced Automation Leadership", "Strategic Technology Planning", "Professional Mentoring", "Industry Consulting" },
            "Master Engineer Extraordinaire" => new() { "Complex System Design", "Advanced Troubleshooting Leadership", "Technology Innovation", "Team Leadership", "Consulting Readiness" },
            "Senior Engineer Extraordinaire" => new() { "Senior Technical Leadership", "Advanced Problem Solving", "Mentor Capabilities", "Strategic Planning", "Professional Excellence" },
            _ => new() { "Professional Engineering Skills", "Advanced Technical Capabilities", "Leadership Potential", "Consulting Awareness", "Innovation Thinking" }
        };
    }

    /// <summary>
    /// Get project capabilities gained
    /// </summary>
    private List<string> GetProjectCapabilities(string extraordinaireLevel)
    {
        return extraordinaireLevel switch
        {
            "Network Overlord Supreme" => new() { "Global Enterprise Projects", "Multi-Billion Dollar Implementations", "Industry-Changing Initiatives", "Quantum Network Research", "AI Integration Projects" },
            "Grand Master Engineer" => new() { "Large-Scale Enterprise Projects", "Multi-Million Dollar Implementations", "Strategic Technology Initiatives", "Industry Leadership Projects", "Innovation Research" },
            _ => new() { "Complex Enterprise Projects", "Advanced Implementation Projects", "Strategic Technology Projects", "Professional Consulting Projects", "Innovation Development" }
        };
    }

    /// <summary>
    /// Get diagnostic expertise levels
    /// </summary>
    private List<string> GetDiagnosticExpertise(string extraordinaireLevel)
    {
        return extraordinaireLevel switch
        {
            "Network Overlord Supreme" => new() { "AI-Powered Predictive Diagnostics", "Quantum-Level Analysis", "Industry-Leading Methodologies", "Global Problem Resolution", "Research-Grade Analysis" },
            "Grand Master Engineer" => new() { "Advanced Predictive Analysis", "Enterprise-Level Diagnostics", "Strategic Problem Resolution", "Industry-Standard Excellence", "Professional Leadership" },
            _ => new() { "Advanced Diagnostic Techniques", "Professional Problem Resolution", "Strategic Analysis", "Leadership Capabilities", "Excellence Standards" }
        };
    }

    /// <summary>
    /// Get architecture mastery capabilities
    /// </summary>
    private List<string> GetArchitectureMastery(string extraordinaireLevel)
    {
        return extraordinaireLevel switch
        {
            "Network Overlord Supreme" => new() { "Quantum Network Architecture", "AI-Driven Design", "Global Enterprise Architecture", "Industry Innovation Leadership", "Research-Level Design" },
            "Grand Master Engineer" => new() { "Enterprise Architecture Leadership", "Advanced Cloud Integration", "Strategic Design Excellence", "Professional Architecture", "Innovation Integration" },
            _ => new() { "Professional Architecture Design", "Advanced Integration Capabilities", "Strategic Planning", "Excellence Standards", "Professional Leadership" }
        };
    }

    /// <summary>
    /// Get certification readiness achieved
    /// </summary>
    private List<string> GetCertificationReadiness(string extraordinaireLevel)
    {
        return extraordinaireLevel switch
        {
            "Network Overlord Supreme" => new() { "CCIE Expert Level", "Industry Leadership Certifications", "Research Credentials", "Global Recognition Programs", "Innovation Certificates" },
            "Grand Master Engineer" => new() { "CCIE Readiness", "Professional Architecture Certifications", "Advanced Specializations", "Industry Recognition", "Leadership Programs" },
            _ => new() { "CCNP Advanced Readiness", "Professional Certifications", "Specialization Tracks", "Industry Recognition", "Professional Development" }
        };
    }

    /// <summary>
    /// Get leadership capabilities unlocked
    /// </summary>
    private List<string> GetLeadershipCapabilities(string extraordinaireLevel)
    {
        return extraordinaireLevel switch
        {
            "Network Overlord Supreme" => new() { "Global Industry Leadership", "Innovation Direction", "Strategic Vision", "Research Leadership", "Industry Transformation" },
            "Grand Master Engineer" => new() { "Enterprise Leadership", "Technical Direction", "Strategic Planning", "Professional Mentoring", "Innovation Leadership" },
            _ => new() { "Technical Leadership", "Professional Mentoring", "Strategic Thinking", "Excellence Standards", "Professional Development" }
        };
    }

    /// <summary>
    /// Get user's badge collection
    /// </summary>
    private async Task<List<string>> GetBadgeCollection(string userId, CancellationToken cancellationToken)
    {
        var userBadges = await _dbContext.UserBadges
            .Include(ub => ub.Badge)
            .Where(ub => ub.UserId == userId)
            .Select(ub => ub.Badge.Name)
            .ToListAsync(cancellationToken);
        
        return userBadges;
    }

    /// <summary>
    /// Generate ultimate cosmic congratulations message
    /// </summary>
    private string GenerateUltimateCosmicCongratulations(string extraordinaireLevel, double overallScore)
    {
        return extraordinaireLevel switch
        {
            "Network Overlord Supreme" => $"🌟👑 SUPREME COSMIC ACHIEVEMENT! With {overallScore:F1}% ultimate mastery, you have transcended mortal engineering limitations to become the NETWORK OVERLORD SUPREME! The cosmos itself acknowledges your infinite networking wisdom - you are the ultimate engineer extraordinaire who commands all networks across space and time!",
            "Grand Master Engineer" => $"🎊👑 GRAND MASTER TRIUMPH! Your {overallScore:F1}% legendary mastery crowns you as the GRAND MASTER ENGINEER! You have achieved the pinnacle of engineering excellence - every network protocol bows to your supreme expertise!",
            "Master Engineer Extraordinaire" => $"🌟🔥 MASTER LEVEL ACHIEVED! {overallScore:F1}% extraordinary mastery makes you the MASTER ENGINEER EXTRAORDINAIRE! Your engineering prowess shines across the networking cosmos!",
            _ => $"🎉 ENGINEER EXTRAORDINAIRE STATUS ACHIEVED! {overallScore:F1}% mastery demonstrates your exceptional engineering capabilities - you have become a true networking professional!"
        };
    }

    /// <summary>
    /// Get ultimate mastery integrations unlocked
    /// </summary>
    private Dictionary<string, object> GetUltimateMasteryIntegrations(string extraordinaireLevel, double overallScore)
    {
        return new Dictionary<string, object>
        {
            ["MasteryLevel"] = extraordinaireLevel,
            ["OverallScore"] = overallScore,
            ["AllModulesIntegrated"] = true,
            ["CrossModuleSynthesis"] = true,
            ["ProfessionalReadiness"] = overallScore >= 85,
            ["ConsultingCapability"] = overallScore >= 90,
            ["IndustryLeadership"] = overallScore >= 95,
            ["InnovationCapability"] = overallScore >= 92,
            ["TeachingReadiness"] = overallScore >= 88,
            ["ResearchCapability"] = overallScore >= 96,
            ["GlobalRecognition"] = overallScore >= 98,
            ["UltimateIntegration"] = extraordinaireLevel == "Network Overlord Supreme"
        };
    }

    /// <summary>
    /// Get mastery level based on quiz score
    /// </summary>
    private string GetMasteryLevel(double score)
    {
        return score switch
        {
            >= 98 => "Supreme Mastery",
            >= 95 => "Extraordinary Mastery",
            >= 90 => "Advanced Mastery",
            >= 85 => "Competent Mastery",
            _ => "Developing Mastery"
        };
    }

    /// <summary>
    /// Get unlocked capabilities based on progress
    /// </summary>
    private List<string> GetUnlockedCapabilities(int lessonNumber, double quizScore)
    {
        var capabilities = new List<string>();
        
        if (quizScore >= 90) capabilities.Add("Advanced Integration");
        if (lessonNumber >= 10) capabilities.Add("Comprehensive Automation");
        if (lessonNumber >= 15 && quizScore >= 85) capabilities.Add("Professional Consulting");
        if (lessonNumber >= 18 && quizScore >= 90) capabilities.Add("Industry Leadership");
        if (quizScore >= 95) capabilities.Add("Innovation Excellence");
        
        return capabilities;
    }

    /// <summary>
    /// Calculate integration score for cross-module functionality
    /// </summary>
    private double CalculateIntegrationScore(int lessonNumber, double quizScore)
    {
        var baseScore = quizScore;
        var progressBonus = (lessonNumber / 20.0) * 10; // Up to 10 point bonus for progress
        var integrationScore = Math.Min(100, baseScore + progressBonus);
        
        return Math.Round(integrationScore, 2);
    }
}