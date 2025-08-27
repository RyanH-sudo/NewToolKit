using Microsoft.EntityFrameworkCore;
using NetToolkit.Modules.Education.Models;
using System.Text.Json;

namespace NetToolkit.Modules.Education.Data;

/// <summary>
/// Education database context - the cosmic repository of learning wisdom
/// Where knowledge is stored, indexed, and retrieved with stellar efficiency
/// </summary>
public class EducationDbContext : DbContext
{
    public EducationDbContext(DbContextOptions<EducationDbContext> options) : base(options)
    {
    }

    // Core content entities
    public DbSet<Module> Modules { get; set; } = null!;
    public DbSet<Lesson> Lessons { get; set; } = null!;
    public DbSet<LessonContent> LessonContents { get; set; } = null!;
    public DbSet<QuizQuestion> QuizQuestions { get; set; } = null!;
    public DbSet<HoverTip> HoverTips { get; set; } = null!;

    // Progress tracking entities
    public DbSet<ModuleProgress> ModuleProgress { get; set; } = null!;
    public DbSet<LessonProgress> LessonProgress { get; set; } = null!;

    // Gamification entities
    public DbSet<Badge> Badges { get; set; } = null!;
    public DbSet<UserBadge> UserBadges { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureModuleEntity(modelBuilder);
        ConfigureLessonEntity(modelBuilder);
        ConfigureLessonContentEntity(modelBuilder);
        ConfigureQuizQuestionEntity(modelBuilder);
        ConfigureHoverTipEntity(modelBuilder);
        ConfigureModuleProgressEntity(modelBuilder);
        ConfigureLessonProgressEntity(modelBuilder);
        ConfigureBadgeEntity(modelBuilder);
        ConfigureUserBadgeEntity(modelBuilder);
    }

    private static void ConfigureModuleEntity(ModelBuilder modelBuilder)
    {
        var moduleEntity = modelBuilder.Entity<Module>();
        
        moduleEntity.HasKey(m => m.Id);
        
        moduleEntity.Property(m => m.Title)
            .IsRequired()
            .HasMaxLength(200);
            
        moduleEntity.Property(m => m.Description)
            .IsRequired()
            .HasMaxLength(1000);
            
        moduleEntity.Property(m => m.Category)
            .IsRequired()
            .HasMaxLength(100);

        // Configure JSON columns for lists
        moduleEntity.Property(m => m.Prerequisites)
            .HasConversion(
                v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                v => JsonSerializer.Deserialize<List<string>>(v, JsonSerializerOptions.Default) ?? new List<string>()
            );

        moduleEntity.Property(m => m.LearningObjectives)
            .HasConversion(
                v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                v => JsonSerializer.Deserialize<List<string>>(v, JsonSerializerOptions.Default) ?? new List<string>()
            );

        moduleEntity.Property(m => m.Tags)
            .HasConversion(
                v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                v => JsonSerializer.Deserialize<List<string>>(v, JsonSerializerOptions.Default) ?? new List<string>()
            );

        // Relationships
        moduleEntity.HasMany(m => m.Lessons)
            .WithOne(l => l.Module)
            .HasForeignKey(l => l.ModuleId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes for performance
        moduleEntity.HasIndex(m => m.Category);
        moduleEntity.HasIndex(m => m.Difficulty);
        moduleEntity.HasIndex(m => m.CreatedAt);
    }

    private static void ConfigureLessonEntity(ModelBuilder modelBuilder)
    {
        var lessonEntity = modelBuilder.Entity<Lesson>();
        
        lessonEntity.HasKey(l => l.Id);
        
        lessonEntity.Property(l => l.Title)
            .IsRequired()
            .HasMaxLength(200);
            
        lessonEntity.Property(l => l.Description)
            .IsRequired()
            .HasMaxLength(500);

        // Configure JSON column for learning objectives
        lessonEntity.Property(l => l.LearningObjectives)
            .HasConversion(
                v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                v => JsonSerializer.Deserialize<List<string>>(v, JsonSerializerOptions.Default) ?? new List<string>()
            );

        // Relationships
        lessonEntity.HasMany(l => l.Content)
            .WithOne(lc => lc.Lesson)
            .HasForeignKey(lc => lc.LessonId)
            .OnDelete(DeleteBehavior.Cascade);

        lessonEntity.HasMany(l => l.QuizQuestions)
            .WithOne(qq => qq.Lesson)
            .HasForeignKey(qq => qq.LessonId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        lessonEntity.HasIndex(l => new { l.ModuleId, l.LessonNumber }).IsUnique();
        lessonEntity.HasIndex(l => l.Difficulty);
    }

    private static void ConfigureLessonContentEntity(ModelBuilder modelBuilder)
    {
        var contentEntity = modelBuilder.Entity<LessonContent>();
        
        contentEntity.HasKey(lc => lc.Id);
        
        contentEntity.Property(lc => lc.Title)
            .IsRequired()
            .HasMaxLength(200);
            
        contentEntity.Property(lc => lc.Data)
            .IsRequired();

        contentEntity.Property(lc => lc.Description)
            .HasMaxLength(500);

        // Relationships
        contentEntity.HasMany(lc => lc.HoverTips)
            .WithOne(ht => ht.LessonContent)
            .HasForeignKey(ht => ht.LessonContentId)
            .OnDelete(DeleteBehavior.Cascade);

        contentEntity.HasMany(lc => lc.QuizQuestions)
            .WithOne(qq => qq.LessonContent)
            .HasForeignKey(qq => qq.LessonContentId)
            .OnDelete(DeleteBehavior.SetNull);

        // Indexes
        contentEntity.HasIndex(lc => lc.Type);
        contentEntity.HasIndex(lc => lc.Order);
    }

    private static void ConfigureQuizQuestionEntity(ModelBuilder modelBuilder)
    {
        var quizEntity = modelBuilder.Entity<QuizQuestion>();
        
        quizEntity.HasKey(qq => qq.Id);
        
        quizEntity.Property(qq => qq.Question)
            .IsRequired()
            .HasMaxLength(500);

        quizEntity.Property(qq => qq.Explanation)
            .HasMaxLength(1000);

        // Configure JSON columns for lists
        quizEntity.Property(qq => qq.Options)
            .HasConversion(
                v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                v => JsonSerializer.Deserialize<List<string>>(v, JsonSerializerOptions.Default) ?? new List<string>()
            );

        quizEntity.Property(qq => qq.WrongAnswerFeedback)
            .HasConversion(
                v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                v => JsonSerializer.Deserialize<List<string>>(v, JsonSerializerOptions.Default) ?? new List<string>()
            );

        quizEntity.Property(qq => qq.Tags)
            .HasConversion(
                v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                v => JsonSerializer.Deserialize<List<string>>(v, JsonSerializerOptions.Default) ?? new List<string>()
            );

        // Indexes
        quizEntity.HasIndex(qq => qq.Difficulty);
        quizEntity.HasIndex(qq => qq.LessonId);
    }

    private static void ConfigureHoverTipEntity(ModelBuilder modelBuilder)
    {
        var tipEntity = modelBuilder.Entity<HoverTip>();
        
        tipEntity.HasKey(ht => ht.Id);
        
        tipEntity.Property(ht => ht.Word)
            .IsRequired()
            .HasMaxLength(100);
            
        tipEntity.Property(ht => ht.Explanation)
            .IsRequired()
            .HasMaxLength(500);

        // Indexes
        tipEntity.HasIndex(ht => ht.Word);
    }

    private static void ConfigureModuleProgressEntity(ModelBuilder modelBuilder)
    {
        var progressEntity = modelBuilder.Entity<ModuleProgress>();
        
        progressEntity.HasKey(mp => mp.Id);
        
        progressEntity.Property(mp => mp.UserId)
            .IsRequired()
            .HasMaxLength(100);

        // Relationships
        progressEntity.HasOne(mp => mp.Module)
            .WithMany()
            .HasForeignKey(mp => mp.ModuleId);

        progressEntity.HasMany(mp => mp.LessonProgresses)
            .WithOne(lp => lp.ModuleProgress)
            .HasForeignKey(lp => lp.ModuleProgressId)
            .OnDelete(DeleteBehavior.Cascade);

        // Unique constraint and indexes
        progressEntity.HasIndex(mp => new { mp.UserId, mp.ModuleId }).IsUnique();
        progressEntity.HasIndex(mp => mp.UserId);
        progressEntity.HasIndex(mp => mp.StartedAt);
        progressEntity.HasIndex(mp => mp.CompletedAt);
    }

    private static void ConfigureLessonProgressEntity(ModelBuilder modelBuilder)
    {
        var lessonProgressEntity = modelBuilder.Entity<LessonProgress>();
        
        lessonProgressEntity.HasKey(lp => lp.Id);
        
        lessonProgressEntity.Property(lp => lp.UserId)
            .IsRequired()
            .HasMaxLength(100);

        lessonProgressEntity.Property(lp => lp.QuizFeedback)
            .HasMaxLength(500);

        // Relationships
        lessonProgressEntity.HasOne(lp => lp.Lesson)
            .WithMany()
            .HasForeignKey(lp => lp.LessonId);

        // Unique constraint and indexes
        lessonProgressEntity.HasIndex(lp => new { lp.UserId, lp.LessonId }).IsUnique();
        lessonProgressEntity.HasIndex(lp => lp.Status);
        lessonProgressEntity.HasIndex(lp => lp.CompletedAt);
    }

    private static void ConfigureBadgeEntity(ModelBuilder modelBuilder)
    {
        var badgeEntity = modelBuilder.Entity<Badge>();
        
        badgeEntity.HasKey(b => b.Id);
        
        badgeEntity.Property(b => b.Name)
            .IsRequired()
            .HasMaxLength(100);
            
        badgeEntity.Property(b => b.Description)
            .IsRequired()
            .HasMaxLength(500);

        badgeEntity.Property(b => b.IconPath)
            .HasMaxLength(200);

        badgeEntity.Property(b => b.Requirements)
            .HasMaxLength(300);

        badgeEntity.Property(b => b.RewardMessage)
            .HasMaxLength(200);

        // Relationships
        badgeEntity.HasMany(b => b.UserBadges)
            .WithOne(ub => ub.Badge)
            .HasForeignKey(ub => ub.BadgeId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        badgeEntity.HasIndex(b => b.Rarity);
        badgeEntity.HasIndex(b => b.ModuleId);
    }

    private static void ConfigureUserBadgeEntity(ModelBuilder modelBuilder)
    {
        var userBadgeEntity = modelBuilder.Entity<UserBadge>();
        
        userBadgeEntity.HasKey(ub => ub.Id);
        
        userBadgeEntity.Property(ub => ub.UserId)
            .IsRequired()
            .HasMaxLength(100);

        // Unique constraint - user can't earn the same badge twice
        userBadgeEntity.HasIndex(ub => new { ub.UserId, ub.BadgeId }).IsUnique();
        userBadgeEntity.HasIndex(ub => ub.AwardedAt);
    }

    /// <summary>
    /// Configure database for SQLite with optimizations
    /// </summary>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // Default SQLite configuration for development
            optionsBuilder.UseSqlite("Data Source=NetToolkitEducation.db", options =>
            {
                options.MigrationsAssembly("NetToolkit.Modules.Education");
            });
        }

        // Enable sensitive data logging in development
        optionsBuilder.EnableSensitiveDataLogging(true);
        optionsBuilder.EnableDetailedErrors(true);
    }

    /// <summary>
    /// Apply database optimizations
    /// </summary>
    public async Task OptimizeDatabaseAsync()
    {
        // SQLite-specific optimizations
        await Database.ExecuteSqlRawAsync("PRAGMA journal_mode = WAL;");
        await Database.ExecuteSqlRawAsync("PRAGMA synchronous = NORMAL;");
        await Database.ExecuteSqlRawAsync("PRAGMA cache_size = 10000;");
        await Database.ExecuteSqlRawAsync("PRAGMA temp_store = MEMORY;");
        await Database.ExecuteSqlRawAsync("PRAGMA mmap_size = 268435456;"); // 256MB
    }
}