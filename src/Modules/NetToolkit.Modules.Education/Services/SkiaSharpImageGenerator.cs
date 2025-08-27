using Microsoft.Extensions.Logging;
using NetToolkit.Modules.Education.Interfaces;
using NetToolkit.Modules.Education.Models;
using SkiaSharp;
using System.Text;

namespace NetToolkit.Modules.Education.Services;

/// <summary>
/// SkiaSharp-based image generator - the cosmic artist of educational visuals
/// Where pixels dance with learning concepts to create engaging cartoonish masterpieces
/// </summary>
public class SkiaSharpImageGenerator : ISlideGenerator
{
    private readonly ILogger<SkiaSharpImageGenerator> _logger;
    
    // Standard colors for consistent theming
    private static readonly SKColor BackgroundColor = SKColor.Parse("#F8F9FA");
    private static readonly SKColor PrimaryColor = SKColor.Parse("#007ACC");
    private static readonly SKColor SecondaryColor = SKColor.Parse("#FF6B35");
    private static readonly SKColor AccentColor = SKColor.Parse("#28A745");
    private static readonly SKColor TextColor = SKColor.Parse("#333333");
    private static readonly SKColor LightTextColor = SKColor.Parse("#666666");

    public SkiaSharpImageGenerator(ILogger<SkiaSharpImageGenerator> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Generate image slide with SkiaSharp graphics
    /// </summary>
    public async Task<byte[]> GenerateImageSlideAsync(
        string title, 
        string description, 
        ImageStyle style = ImageStyle.Cartoon,
        int width = 800, 
        int height = 600,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Generating cosmic image slide: '{Title}' in {Style} style ({Width}x{Height})", 
                title, style, width, height);

            using var bitmap = new SKBitmap(width, height);
            using var canvas = new SKCanvas(bitmap);
            
            // Clear canvas with background
            canvas.Clear(BackgroundColor);

            // Generate based on the description content
            await DrawImageContentAsync(canvas, title, description, style, width, height, cancellationToken);

            // Add title overlay
            DrawTitleOverlay(canvas, title, width, height);

            // Encode to PNG
            using var image = SKImage.FromBitmap(bitmap);
            using var data = image.Encode(SKEncodedImageFormat.Png, 95);
            
            var imageBytes = data.ToArray();
            
            _logger.LogDebug("Generated image slide '{Title}' - {Size} bytes of cosmic visual brilliance!", 
                title, imageBytes.Length);

            return imageBytes;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate image slide '{Title}' - cosmic art studio malfunction!", title);
            
            // Return a simple error image
            return await GenerateErrorImageAsync(title, width, height);
        }
    }

    /// <summary>
    /// Generate text slide with hover tips and formatting
    /// </summary>
    public async Task<string> GenerateTextSlideAsync(
        string title,
        string content,
        List<HoverTip> tips,
        TextStyle style = TextStyle.Educational,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Generating cosmic text slide: '{Title}' with {TipCount} hover tips in {Style} style", 
                title, tips.Count, style);

            var htmlBuilder = new StringBuilder();
            
            // Build the HTML structure
            htmlBuilder.AppendLine("<!DOCTYPE html>");
            htmlBuilder.AppendLine("<html lang='en'>");
            htmlBuilder.AppendLine("<head>");
            htmlBuilder.AppendLine("    <meta charset='UTF-8'>");
            htmlBuilder.AppendLine("    <meta name='viewport' content='width=device-width, initial-scale=1.0'>");
            htmlBuilder.AppendLine($"    <title>{title}</title>");
            htmlBuilder.AppendLine(GenerateTextSlideCSS(style));
            htmlBuilder.AppendLine("</head>");
            htmlBuilder.AppendLine("<body>");
            
            // Add slide container
            htmlBuilder.AppendLine($"    <div class='slide-container {GetStyleClass(style)}'>");
            htmlBuilder.AppendLine($"        <div class='slide-header'>");
            htmlBuilder.AppendLine($"            <h1 class='slide-title'>{title}</h1>");
            htmlBuilder.AppendLine("        </div>");
            
            // Add content with hover tips
            htmlBuilder.AppendLine("        <div class='slide-content'>");
            var processedContent = ProcessHoverTips(content, tips);
            htmlBuilder.AppendLine(processedContent);
            htmlBuilder.AppendLine("        </div>");
            
            // Add tip definitions
            if (tips.Any())
            {
                htmlBuilder.AppendLine("        <div class='tip-container'>");
                foreach (var tip in tips)
                {
                    htmlBuilder.AppendLine($"            <div class='tip-definition' data-tip='{tip.Word}'>");
                    htmlBuilder.AppendLine($"                <strong>{tip.Word}:</strong> {tip.Explanation}");
                    htmlBuilder.AppendLine("            </div>");
                }
                htmlBuilder.AppendLine("        </div>");
            }
            
            htmlBuilder.AppendLine("    </div>");
            
            // Add JavaScript for interactivity
            htmlBuilder.AppendLine(GenerateTextSlideJavaScript());
            
            htmlBuilder.AppendLine("</body>");
            htmlBuilder.AppendLine("</html>");

            var html = htmlBuilder.ToString();
            
            _logger.LogDebug("Generated text slide '{Title}' - {Length} characters of cosmic educational content!", 
                title, html.Length);

            return html;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate text slide '{Title}' - cosmic text renderer malfunction!", title);
            return GenerateErrorTextSlide(title, ex.Message);
        }
    }

    /// <summary>
    /// Generate quiz slide with multiple choice options
    /// </summary>
    public async Task<QuizSlideData> GenerateQuizSlideAsync(
        QuizQuestion question,
        QuizStyle style = QuizStyle.Gamified,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Generating cosmic quiz slide for question '{QuestionId}' in {Style} style", 
                question.Id, style);

            var htmlBuilder = new StringBuilder();
            
            htmlBuilder.AppendLine("<!DOCTYPE html>");
            htmlBuilder.AppendLine("<html lang='en'>");
            htmlBuilder.AppendLine("<head>");
            htmlBuilder.AppendLine("    <meta charset='UTF-8'>");
            htmlBuilder.AppendLine("    <meta name='viewport' content='width=device-width, initial-scale=1.0'>");
            htmlBuilder.AppendLine($"    <title>Quiz: {question.Question}</title>");
            htmlBuilder.AppendLine(GenerateQuizSlideCSS(style));
            htmlBuilder.AppendLine("</head>");
            htmlBuilder.AppendLine("<body>");
            
            htmlBuilder.AppendLine($"    <div class='quiz-container {GetQuizStyleClass(style)}'>");
            htmlBuilder.AppendLine("        <div class='quiz-header'>");
            htmlBuilder.AppendLine($"            <h2 class='quiz-question'>{question.Question}</h2>");
            htmlBuilder.AppendLine("        </div>");
            
            htmlBuilder.AppendLine("        <div class='quiz-options'>");
            for (int i = 0; i < question.Options.Count; i++)
            {
                var optionClass = i == question.CorrectAnswerIndex ? "correct" : "incorrect";
                htmlBuilder.AppendLine($"            <button class='quiz-option {optionClass}' data-index='{i}'>");
                htmlBuilder.AppendLine($"                <span class='option-letter'>{(char)('A' + i)}</span>");
                htmlBuilder.AppendLine($"                <span class='option-text'>{question.Options[i]}</span>");
                htmlBuilder.AppendLine("            </button>");
            }
            htmlBuilder.AppendLine("        </div>");
            
            htmlBuilder.AppendLine("        <div class='quiz-feedback' id='quiz-feedback'>");
            htmlBuilder.AppendLine("            <p id='feedback-text'></p>");
            htmlBuilder.AppendLine("        </div>");
            
            if (!string.IsNullOrEmpty(question.Explanation))
            {
                htmlBuilder.AppendLine($"        <div class='quiz-explanation' id='quiz-explanation' style='display:none;'>");
                htmlBuilder.AppendLine($"            <h4>💡 Explanation:</h4>");
                htmlBuilder.AppendLine($"            <p>{question.Explanation}</p>");
                htmlBuilder.AppendLine("        </div>");
            }
            
            htmlBuilder.AppendLine("    </div>");
            
            // Add quiz JavaScript
            htmlBuilder.AppendLine(GenerateQuizJavaScript(question));
            
            htmlBuilder.AppendLine("</body>");
            htmlBuilder.AppendLine("</html>");

            var quizData = new QuizSlideData
            {
                QuestionId = question.Id,
                Html = htmlBuilder.ToString(),
                InteractionData = new Dictionary<string, object>
                {
                    ["questionText"] = question.Question,
                    ["optionCount"] = question.Options.Count,
                    ["correctIndex"] = question.CorrectAnswerIndex,
                    ["style"] = style.ToString()
                },
                AnswerOptions = question.Options,
                CorrectAnswerIndex = question.CorrectAnswerIndex,
                FeedbackHtml = GenerateFeedbackHtml(question)
            };

            _logger.LogDebug("Generated quiz slide for question '{QuestionId}' - cosmic knowledge testing enabled!", 
                question.Id);

            return quizData;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate quiz slide for question '{QuestionId}' - cosmic quiz generator malfunction!", question.Id);
            
            return new QuizSlideData
            {
                QuestionId = question.Id,
                Html = GenerateErrorQuizSlide(question.Question, ex.Message),
                AnswerOptions = question.Options,
                CorrectAnswerIndex = question.CorrectAnswerIndex
            };
        }
    }

    /// <summary>
    /// Generate interactive Three.js slide with 3D models
    /// </summary>
    public async Task<ThreeJsSlideData> GenerateThreeJsSlideAsync(
        string title,
        ModelType modelType,
        Dictionary<string, object> interactionData,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Generating cosmic Three.js slide: '{Title}' with {ModelType} model", title, modelType);

            var sceneConfig = GenerateThreeJsSceneConfig(modelType, interactionData);
            var htmlWrapper = GenerateThreeJsHtmlWrapper(title, modelType);
            var controlsConfig = GenerateThreeJsControls(modelType, interactionData);

            var threeJsData = new ThreeJsSlideData
            {
                SceneId = $"scene_{Guid.NewGuid():N}",
                SceneJson = sceneConfig,
                Controls = controlsConfig,
                Scripts = GetRequiredThreeJsScripts(modelType),
                HtmlWrapper = htmlWrapper
            };

            _logger.LogDebug("Generated Three.js slide '{Title}' - 3D cosmic visualization ready!", title);

            return threeJsData;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate Three.js slide '{Title}' - cosmic 3D renderer malfunction!", title);
            throw;
        }
    }

    /// <summary>
    /// Generate network diagram slide (specific to Module 1)
    /// </summary>
    public async Task<byte[]> GenerateNetworkDiagramAsync(
        NetworkDiagramType diagramType,
        List<NetworkNode> nodes,
        List<NetworkConnection> connections,
        List<DiagramAnnotation> annotations,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Generating cosmic network diagram: {DiagramType} with {NodeCount} nodes and {ConnectionCount} connections", 
                diagramType, nodes.Count, connections.Count);

            const int width = 800;
            const int height = 600;
            
            using var bitmap = new SKBitmap(width, height);
            using var canvas = new SKCanvas(bitmap);
            
            canvas.Clear(BackgroundColor);

            // Draw diagram based on type
            await DrawNetworkDiagramAsync(canvas, diagramType, nodes, connections, annotations, width, height);

            // Add title
            var title = GetDiagramTitle(diagramType);
            DrawTitleOverlay(canvas, title, width, height);

            using var image = SKImage.FromBitmap(bitmap);
            using var data = image.Encode(SKEncodedImageFormat.Png, 95);
            
            var diagramBytes = data.ToArray();
            
            _logger.LogDebug("Generated network diagram '{DiagramType}' - {Size} bytes of cosmic networking wisdom!", 
                diagramType, diagramBytes.Length);

            return diagramBytes;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate network diagram '{DiagramType}' - cosmic network artist malfunction!", diagramType);
            return await GenerateErrorImageAsync($"Network Diagram: {diagramType}", 800, 600);
        }
    }

    // Additional interface methods with simplified implementations for brevity
    public async Task<AnimationSlideData> GenerateAnimatedSlideAsync(string concept, List<AnimationStep> steps, int duration, CancellationToken cancellationToken = default)
    {
        return new AnimationSlideData
        {
            AnimationId = Guid.NewGuid().ToString(),
            Steps = steps,
            HtmlContent = $"<div class='animation-placeholder'>Animation: {concept} (Implementation coming soon!)</div>",
            CssStyles = ".animation-placeholder { text-align: center; padding: 50px; font-size: 24px; }",
            JavaScript = "// Animation logic to be implemented"
        };
    }

    public async Task<Slide> GenerateIntroductionSlideAsync(string lessonTitle, List<string> objectives, int estimatedMinutes, CancellationToken cancellationToken = default)
    {
        var content = new StringBuilder();
        content.AppendLine($"<h1>Welcome to {lessonTitle}!</h1>");
        content.AppendLine($"<p>📚 Estimated time: {estimatedMinutes} minutes of cosmic learning</p>");
        content.AppendLine("<h3>🎯 Learning Objectives:</h3><ul>");
        foreach (var objective in objectives)
        {
            content.AppendLine($"<li>{objective}</li>");
        }
        content.AppendLine("</ul>");

        return new Slide
        {
            Id = Guid.NewGuid().ToString(),
            Title = $"Introduction: {lessonTitle}",
            Type = SlideType.Text,
            Content = content.ToString(),
            Order = 0
        };
    }

    public async Task<Slide> GenerateSummarySlideAsync(string lessonTitle, List<string> keyPoints, List<string> nextSteps, CancellationToken cancellationToken = default)
    {
        var content = new StringBuilder();
        content.AppendLine($"<h1>🎓 {lessonTitle} - Complete!</h1>");
        content.AppendLine("<h3>🌟 Key Takeaways:</h3><ul>");
        foreach (var point in keyPoints)
        {
            content.AppendLine($"<li>{point}</li>");
        }
        content.AppendLine("</ul>");
        
        if (nextSteps.Any())
        {
            content.AppendLine("<h3>🚀 What's Next:</h3><ul>");
            foreach (var step in nextSteps)
            {
                content.AppendLine($"<li>{step}</li>");
            }
            content.AppendLine("</ul>");
        }

        return new Slide
        {
            Id = Guid.NewGuid().ToString(),
            Title = $"Summary: {lessonTitle}",
            Type = SlideType.Text,
            Content = content.ToString(),
            Order = 999
        };
    }

    public async Task<Slide> EmbedHoverTipsAsync(Slide baseSlide, List<HoverTip> tips, CancellationToken cancellationToken = default)
    {
        var enhancedContent = ProcessHoverTips(baseSlide.Content, tips);
        
        return new Slide
        {
            Id = baseSlide.Id,
            Title = baseSlide.Title,
            Type = baseSlide.Type,
            Content = enhancedContent,
            Order = baseSlide.Order,
            Metadata = baseSlide.Metadata
        };
    }

    public async Task<SlideValidationResult> ValidateSlideAsync(Slide slide, DifficultyLevel targetAudience)
    {
        var result = new SlideValidationResult
        {
            IsValid = true,
            ReadabilityScore = 85, // Simplified scoring
            EstimatedDifficulty = DifficultyLevel.Beginner
        };

        // Basic validation checks
        if (string.IsNullOrWhiteSpace(slide.Title))
        {
            result.IsValid = false;
            result.Issues.Add("Slide title is missing");
        }

        if (string.IsNullOrWhiteSpace(slide.Content))
        {
            result.IsValid = false;
            result.Issues.Add("Slide content is empty");
        }

        if (slide.Content?.Length > 2000)
        {
            result.Suggestions.Add("Consider breaking this slide into smaller chunks for better comprehension");
        }

        return result;
    }

    #region Private Helper Methods

    private async Task DrawImageContentAsync(SKCanvas canvas, string title, string description, ImageStyle style, int width, int height, CancellationToken cancellationToken)
    {
        // Determine what to draw based on the description keywords
        var desc = description.ToLowerInvariant();
        
        // Module 1 (Network Basics) drawings
        if (desc.Contains("network") && desc.Contains("hands"))
        {
            DrawKidsLinkingHands(canvas, width, height, style);
        }
        else if (desc.Contains("lego") || desc.Contains("building"))
        {
            DrawLegoBricks(canvas, width, height, style);
        }
        else if (desc.Contains("cable") || desc.Contains("spaghetti"))
        {
            DrawCableSpaghetti(canvas, width, height, style);
        }
        else if (desc.Contains("wifi") && (desc.Contains("waves") || desc.Contains("magic")))
        {
            DrawWifiWaves(canvas, width, height, style);
        }
        else if (desc.Contains("router") && desc.Contains("host") && desc.Contains("party"))
        {
            DrawRouterPartyHost(canvas, width, height, style);
        }
        // Module 2 (Hardware Heroes) drawings
        else if (desc.Contains("superheroes") || desc.Contains("hardware") && desc.Contains("lineup"))
        {
            DrawHardwareSuperheroes(canvas, width, height, style);
        }
        else if (desc.Contains("nic") && desc.Contains("mouth"))
        {
            DrawChattyNIC(canvas, width, height, style);
        }
        else if (desc.Contains("switch") && desc.Contains("dj"))
        {
            DrawSwitchDJ(canvas, width, height, style);
        }
        else if (desc.Contains("router") && desc.Contains("cop"))
        {
            DrawRouterTrafficCop(canvas, width, height, style);
        }
        else if (desc.Contains("firewall") && desc.Contains("shield"))
        {
            DrawFirewallShield(canvas, width, height, style);
        }
        else if (desc.Contains("access") && desc.Contains("point") && desc.Contains("wizard"))
        {
            DrawAccessPointWizard(canvas, width, height, style);
        }
        else if (desc.Contains("server") && desc.Contains("vault"))
        {
            DrawServerVault(canvas, width, height, style);
        }
        // Module 3 (IP Shenanigans) drawings
        else if (desc.Contains("house") && desc.Contains("address"))
        {
            DrawIPHouseAddress(canvas, width, height, style);
        }
        else if (desc.Contains("vintage") && desc.Contains("modern") && desc.Contains("houses"))
        {
            DrawIPv4VsIPv6Houses(canvas, width, height, style);
        }
        else if (desc.Contains("musical") && desc.Contains("chairs"))
        {
            DrawDynamicIPMusicalChairs(canvas, width, height, style);
        }
        else if (desc.Contains("anchored") && desc.Contains("house"))
        {
            DrawStaticIPAnchoredHouse(canvas, width, height, style);
        }
        else if (desc.Contains("butler") && desc.Contains("keys"))
        {
            DrawDHCPButlerKeys(canvas, width, height, style);
        }
        else if (desc.Contains("fenced") && desc.Contains("blocks"))
        {
            DrawSubnetFencedBlocks(canvas, width, height, style);
        }
        else if (desc.Contains("pizza") && desc.Contains("cutter"))
        {
            DrawCIDRPizzaCutter(canvas, width, height, style);
        }
        else if (desc.Contains("home") && desc.Contains("street") && desc.Contains("addresses"))
        {
            DrawPrivateVsPublicAddresses(canvas, width, height, style);
        }
        else if (desc.Contains("interpreter") && desc.Contains("nat"))
        {
            DrawNATInterpreter(canvas, width, height, style);
        }
        else if (desc.Contains("detective") && desc.Contains("arp"))
        {
            DrawARPDetective(canvas, width, height, style);
        }
        else if (desc.Contains("school") && desc.Contains("classes"))
        {
            DrawIPClassesSchool(canvas, width, height, style);
        }
        else if (desc.Contains("mirror") && desc.Contains("loopback"))
        {
            DrawLoopbackMirror(canvas, width, height, style);
        }
        else if (desc.Contains("group") && desc.Contains("call"))
        {
            DrawMulticastGroupCall(canvas, width, height, style);
        }
        else if (desc.Contains("nearest") && desc.Contains("friend"))
        {
            DrawAnycastNearestFriend(canvas, width, height, style);
        }
        else if (desc.Contains("fortress") && desc.Contains("security"))
        {
            DrawIPSecurityFortress(canvas, width, height, style);
        }
        else if (desc.Contains("wand") && desc.Contains("powershell"))
        {
            DrawPowerShellIPWand(canvas, width, height, style);
        }
        else if (desc.Contains("puzzle") && desc.Contains("troubleshooting"))
        {
            DrawIPTroubleshootingPuzzle(canvas, width, height, style);
        }
        else if (desc.Contains("future") && desc.Contains("ipv6"))
        {
            DrawIPv6FutureCity(canvas, width, height, style);
        }
        else if (desc.Contains("binary") && desc.Contains("calculator"))
        {
            DrawSubnetBinaryCalculator(canvas, width, height, style);
        }
        else if (desc.Contains("mastery") && desc.Contains("medal"))
        {
            DrawIPMasteryMedal(canvas, width, height, style);
        }
        
        // Module 4 (Scripting Sorcery) drawings
        else if (desc.Contains("wizard") && desc.Contains("wand") && desc.Contains("computer"))
        {
            DrawScriptWizardComputer(canvas, width, height, style);
        }
        else if (desc.Contains("potion") && desc.Contains("bottles") && desc.Contains("labeled"))
        {
            DrawVariablePotionBottles(canvas, width, height, style);
        }
        else if (desc.Contains("ancient") && desc.Contains("spell") && desc.Contains("book"))
        {
            DrawAncientSpellBook(canvas, width, height, style);
        }
        else if (desc.Contains("labeled") && desc.Contains("building") && desc.Contains("blocks"))
        {
            DrawLabeledBuildingBlocks(canvas, width, height, style);
        }
        else if (desc.Contains("forked") && desc.Contains("enchanted") && desc.Contains("path"))
        {
            DrawForkedEnchantedPath(canvas, width, height, style);
        }
        else if (desc.Contains("magical") && desc.Contains("looping") && desc.Contains("road"))
        {
            DrawMagicalLoopingRoad(canvas, width, height, style);
        }
        else if (desc.Contains("spell") && desc.Contains("scroll") && desc.Contains("collection"))
        {
            DrawSpellScrollCollection(canvas, width, height, style);
        }
        else if (desc.Contains("net") && desc.Contains("catching") && desc.Contains("bugs"))
        {
            DrawNetCatchingBugs(canvas, width, height, style);
        }
        else if (desc.Contains("castle") && desc.Contains("code") && desc.Contains("runes"))
        {
            DrawCastleWithCodeRunes(canvas, width, height, style);
        }
        else if (desc.Contains("enchanted") && desc.Contains("mailbox") && desc.Contains("glowing"))
        {
            DrawEnchantedMailboxGlowing(canvas, width, height, style);
        }
        else if (desc.Contains("protective") && desc.Contains("magical") && desc.Contains("runes"))
        {
            DrawProtectiveMagicalRunes(canvas, width, height, style);
        }
        else if (desc.Contains("robot") && desc.Contains("assistant") && desc.Contains("magical"))
        {
            DrawRobotAssistantMagical(canvas, width, height, style);
        }
        else if (desc.Contains("adjustable") && desc.Contains("magic") && desc.Contains("wand"))
        {
            DrawAdjustableMagicWand(canvas, width, height, style);
        }
        else if (desc.Contains("magical") && desc.Contains("library") && desc.Contains("shelves"))
        {
            DrawMagicalLibraryShelves(canvas, width, height, style);
        }
        else if (desc.Contains("magnifying") && desc.Contains("glass") && desc.Contains("code"))
        {
            DrawMagnifyingGlassCode(canvas, width, height, style);
        }
        else if (desc.Contains("infinite") && desc.Contains("loop") && desc.Contains("symbol"))
        {
            DrawInfiniteLoopSymbol(canvas, width, height, style);
        }
        else if (desc.Contains("glowing") && desc.Contains("magical") && desc.Contains("objects"))
        {
            DrawGlowingMagicalObjects(canvas, width, height, style);
        }
        else if (desc.Contains("magical") && desc.Contains("telescope") && desc.Contains("view"))
        {
            DrawMagicalTelescopeView(canvas, width, height, style);
        }
        else if (desc.Contains("advanced") && desc.Contains("grimoire") && desc.Contains("open"))
        {
            DrawAdvancedGrimoireOpen(canvas, width, height, style);
        }
        else if (desc.Contains("magical") && desc.Contains("crown") && desc.Contains("scripts"))
        {
            DrawMagicalCrownScripts(canvas, width, height, style);
        }
        
        // Module 5 (Routing Riddles) drawings
        else if (desc.Contains("treasure") && desc.Contains("map") && desc.Contains("maze"))
        {
            DrawTreasureMapMazePaths(canvas, width, height, style);
        }
        else if (desc.Contains("straight") && desc.Contains("golden") && desc.Contains("road"))
        {
            DrawStraightGoldenRoad(canvas, width, height, style);
        }
        else if (desc.Contains("adaptive") && desc.Contains("winding") && desc.Contains("trail"))
        {
            DrawAdaptiveWindingTrail(canvas, width, height, style);
        }
        else if (desc.Contains("village") && desc.Contains("gossip") && desc.Contains("whispers"))
        {
            DrawVillageGossipWhispers(canvas, width, height, style);
        }
        else if (desc.Contains("detailed") && desc.Contains("network") && desc.Contains("topology"))
        {
            DrawDetailedNetworkTopology(canvas, width, height, style);
        }
        else if (desc.Contains("world") && desc.Contains("map") && desc.Contains("internet"))
        {
            DrawWorldMapInternetRoutes(canvas, width, height, style);
        }
        else if (desc.Contains("network") && desc.Contains("safety") && desc.Contains("funnel"))
        {
            DrawNetworkSafetyNetFunnel(canvas, width, height, style);
        }
        else if (desc.Contains("digital") && desc.Contains("phone") && desc.Contains("routes"))
        {
            DrawDigitalPhoneBookRoutes(canvas, width, height, style);
        }
        else if (desc.Contains("quality") && desc.Contains("measurement") && desc.Contains("scales"))
        {
            DrawQualityMeasurementScales(canvas, width, height, style);
        }
        else if (desc.Contains("settling") && desc.Contains("dust") && desc.Contains("consensus"))
        {
            DrawSettlingDustConsensus(canvas, width, height, style);
        }
        else if (desc.Contains("infinite") && desc.Contains("loop") && desc.Contains("danger"))
        {
            DrawInfiniteLoopMazeDanger(canvas, width, height, style);
        }
        else if (desc.Contains("security") && desc.Contains("checkpoint") && desc.Contains("guards"))
        {
            DrawSecurityCheckpointGuards(canvas, width, height, style);
        }
        else if (desc.Contains("magical") && desc.Contains("wand") && desc.Contains("paths"))
        {
            DrawMagicalWandNetworkPaths(canvas, width, height, style);
        }
        else if (desc.Contains("parallel") && desc.Contains("universe") && desc.Contains("routing"))
        {
            DrawParallelUniverseRouting(canvas, width, height, style);
        }
        else if (desc.Contains("detective") && desc.Contains("network") && desc.Contains("breadcrumbs"))
        {
            DrawDetectiveNetworkBreadcrumbs(canvas, width, height, style);
        }
        else if (desc.Contains("armored") && desc.Contains("secure") && desc.Contains("pathway"))
        {
            DrawArmoredSecurePathway(canvas, width, height, style);
        }
        else if (desc.Contains("advanced") && desc.Contains("hybrid") && desc.Contains("routing"))
        {
            DrawAdvancedHybridRouting(canvas, width, height, style);
        }
        else if (desc.Contains("express") && desc.Contains("lane") && desc.Contains("labeled"))
        {
            DrawExpressLaneLabeledPackages(canvas, width, height, style);
        }
        else if (desc.Contains("deep") && desc.Contains("ocean") && desc.Contains("bgp"))
        {
            DrawDeepOceanBGPExploration(canvas, width, height, style);
        }
        else if (desc.Contains("trophy") && desc.Contains("routing") && desc.Contains("crown"))
        {
            DrawTrophyRoutingMasteryCrown(canvas, width, height, style);
        }
        
        // Module 6 (Security Shenanigans - Fortress Building 101) drawings
        else if (desc.Contains("castle") && desc.Contains("moat") && desc.Contains("guards"))
        {
            DrawDigitalCastleWithMoatAndGuards(canvas, width, height, style);
        }
        else if (desc.Contains("bouncer") && desc.Contains("club") && desc.Contains("clipboard"))
        {
            DrawBouncerAtClubDoor(canvas, width, height, style);
        }
        else if (desc.Contains("locked") && desc.Contains("chest") && desc.Contains("runes"))
        {
            DrawEncryptedTreasureChest(canvas, width, height, style);
        }
        else if (desc.Contains("underground") && desc.Contains("tunnel") && desc.Contains("glowing"))
        {
            DrawSecretUndergroundTunnel(canvas, width, height, style);
        }
        else if (desc.Contains("medieval") && desc.Contains("watchtower") && desc.Contains("guards"))
        {
            DrawMedievalWatchtowerGuards(canvas, width, height, style);
        }
        else if (desc.Contains("castle") && desc.Contains("wall") && desc.Contains("cracks"))
        {
            DrawCrackedCastleWall(canvas, width, height, style);
        }
        else if (desc.Contains("castle") && desc.Contains("infested") && desc.Contains("bugs"))
        {
            DrawCastleInfestedWithBugs(canvas, width, height, style);
        }
        else if (desc.Contains("fishing") && desc.Contains("rod") && desc.Contains("email"))
        {
            DrawPhishingRodWithEmailBait(canvas, width, height, style);
        }
        else if (desc.Contains("keymaster") && desc.Contains("access") && desc.Contains("control"))
        {
            DrawKeymasterAccessControl(canvas, width, height, style);
        }
        else if (desc.Contains("two") && desc.Contains("keys") && desc.Contains("locks"))
        {
            DrawTwoKeysDoubleLocks(canvas, width, height, style);
        }
        else if (desc.Contains("wand") && desc.Contains("shield") && desc.Contains("security"))
        {
            DrawSecurityWardShield(canvas, width, height, style);
        }
        else if (desc.Contains("walled") && desc.Contains("sections") && desc.Contains("moats"))
        {
            DrawWalledNetworkSections(canvas, width, height, style);
        }
        else if (desc.Contains("scroll") && desc.Contains("chronicle") && desc.Contains("events"))
        {
            DrawSecurityChronicleScroll(canvas, width, height, style);
        }
        else if (desc.Contains("mock") && desc.Contains("battle") && desc.Contains("practice"))
        {
            DrawMockBattlePractice(canvas, width, height, style);
        }
        else if (desc.Contains("suspicious") && desc.Contains("guard") && desc.Contains("verify"))
        {
            DrawSuspiciousGuardVerifying(canvas, width, height, style);
        }
        else if (desc.Contains("cloud") && desc.Contains("castle") && desc.Contains("sky"))
        {
            DrawCloudCastleInSky(canvas, width, height, style);
        }
        else if (desc.Contains("war") && desc.Contains("room") && desc.Contains("battle"))
        {
            DrawIncidentResponseWarRoom(canvas, width, height, style);
        }
        else if (desc.Contains("law") && desc.Contains("book") && desc.Contains("compliance"))
        {
            DrawLawBookCompliance(canvas, width, height, style);
        }
        else if (desc.Contains("armored") && desc.Contains("knight") && desc.Contains("advanced"))
        {
            DrawArmoredKnightAdvanced(canvas, width, height, style);
        }
        else if (desc.Contains("locked") && desc.Contains("trophy") && desc.Contains("fortress"))
        {
            DrawLockedTrophyFortress(canvas, width, height, style);
        }
        
        // Module 7 (Wireless Wonders - Invisible Highways) drawings
        else if (desc.Contains("invisible") && desc.Contains("roads") && desc.Contains("waves"))
        {
            DrawInvisibleWirelessRoads(canvas, width, height, style);
        }
        else if (desc.Contains("home") && desc.Contains("waves") && desc.Contains("wifi"))
        {
            DrawHomeWithWiFiWaves(canvas, width, height, style);
        }
        else if (desc.Contains("family") && desc.Contains("tree") && desc.Contains("802.11"))
        {
            DrawWiFiStandardsFamilyTree(canvas, width, height, style);
        }
        else if (desc.Contains("radio") && desc.Contains("dials") && desc.Contains("frequency"))
        {
            DrawFrequencyRadioDials(canvas, width, height, style);
        }
        else if (desc.Contains("lighthouse") && desc.Contains("beacon") && desc.Contains("access"))
        {
            DrawAccessPointLighthouse(canvas, width, height, style);
        }
        else if (desc.Contains("name") && desc.Contains("tags") && desc.Contains("ssid"))
        {
            DrawSSIDNameTags(canvas, width, height, style);
        }
        else if (desc.Contains("locked") && desc.Contains("waves") && desc.Contains("encryption"))
        {
            DrawEncryptedWirelessWaves(canvas, width, height, style);
        }
        else if (desc.Contains("spider") && desc.Contains("web") && desc.Contains("mesh"))
        {
            DrawMeshNetworkSpiderWeb(canvas, width, height, style);
        }
        else if (desc.Contains("close") && desc.Contains("whispers") && desc.Contains("bluetooth"))
        {
            DrawBluetoothCloseWhispers(canvas, width, height, style);
        }
        else if (desc.Contains("touch") && desc.Contains("spark") && desc.Contains("nfc"))
        {
            DrawNFCTouchSpark(canvas, width, height, style);
        }
        else if (desc.Contains("traffic") && desc.Contains("jam") && desc.Contains("interference"))
        {
            DrawWirelessInterferenceTrafficJam(canvas, width, height, style);
        }
        else if (desc.Contains("handover") && desc.Contains("relay") && desc.Contains("roaming"))
        {
            DrawWirelessRoamingHandover(canvas, width, height, style);
        }
        else if (desc.Contains("wand") && desc.Contains("air") && desc.Contains("wireless") && desc.Contains("script"))
        {
            DrawWirelessScriptingWand(canvas, width, height, style);
        }
        else if (desc.Contains("shielded") && desc.Contains("waves") && desc.Contains("security"))
        {
            DrawWirelessSecurityShieldedWaves(canvas, width, height, style);
        }
        else if (desc.Contains("detective") && desc.Contains("air") && desc.Contains("troubleshoot"))
        {
            DrawWirelessTroubleshootingDetective(canvas, width, height, style);
        }
        else if (desc.Contains("futuristic") && desc.Contains("signals") && desc.Contains("5g"))
        {
            DrawFuturisticFiveGSignals(canvas, width, height, style);
        }
        else if (desc.Contains("connected") && desc.Contains("things") && desc.Contains("iot"))
        {
            DrawIoTConnectedThings(canvas, width, height, style);
        }
        else if (desc.Contains("aerial") && desc.Contains("patterns") && desc.Contains("topology"))
        {
            DrawWirelessTopologyPatterns(canvas, width, height, style);
        }
        else if (desc.Contains("advanced") && desc.Contains("waves") && desc.Contains("protocols"))
        {
            DrawAdvancedWirelessProtocols(canvas, width, height, style);
        }
        else if (desc.Contains("magic") && desc.Contains("trophy") && desc.Contains("wireless") && desc.Contains("mastery"))
        {
            DrawWirelessMasteryTrophy(canvas, width, height, style);
        }
        
        // Module 8 (Cloud Conquest - Sky-High Networks) drawings
        else if (desc.Contains("clouds") && desc.Contains("castles") && desc.Contains("floating"))
        {
            DrawFloatingCloudCastles(canvas, width, height, style);
        }
        else if (desc.Contains("building") && desc.Contains("blocks") && desc.Contains("sky") && desc.Contains("iaas"))
        {
            DrawIaaSBuildingBlocks(canvas, width, height, style);
        }
        else if (desc.Contains("ready") && desc.Contains("platform") && desc.Contains("paas"))
        {
            DrawPaaSPlatform(canvas, width, height, style);
        }
        else if (desc.Contains("apps") && desc.Contains("clouds") && desc.Contains("saas"))
        {
            DrawSaaSAppsInClouds(canvas, width, height, style);
        }
        else if (desc.Contains("azure") && desc.Contains("logo") && desc.Contains("sky"))
        {
            DrawAzureSkyKingdom(canvas, width, height, style);
        }
        else if (desc.Contains("amazon") && desc.Contains("cloud") && desc.Contains("aws"))
        {
            DrawAmazonCloudEmpire(canvas, width, height, style);
        }
        else if (desc.Contains("google") && desc.Contains("clouds") && desc.Contains("gcp"))
        {
            DrawGoogleSmartClouds(canvas, width, height, style);
        }
        else if (desc.Contains("cloned") && desc.Contains("servers") && desc.Contains("virtual"))
        {
            DrawVirtualMachineClones(canvas, width, height, style);
        }
        else if (desc.Contains("shipping") && desc.Contains("containers") && desc.Contains("sky"))
        {
            DrawContainersInSky(canvas, width, height, style);
        }
        else if (desc.Contains("function") && desc.Contains("wands") && desc.Contains("serverless"))
        {
            DrawServerlessFunctionWands(canvas, width, height, style);
        }
        else if (desc.Contains("vault") && desc.Contains("clouds") && desc.Contains("storage"))
        {
            DrawCloudStorageVaults(canvas, width, height, style);
        }
        else if (desc.Contains("cloud") && desc.Contains("roads") && desc.Contains("networking"))
        {
            DrawCloudVirtualHighways(canvas, width, height, style);
        }
        else if (desc.Contains("wand") && desc.Contains("cloud") && desc.Contains("scripting"))
        {
            DrawCloudScriptingWand(canvas, width, height, style);
        }
        else if (desc.Contains("cloud") && desc.Contains("shields") && desc.Contains("security"))
        {
            DrawCloudSecurityShields(canvas, width, height, style);
        }
        else if (desc.Contains("detective") && desc.Contains("fog") && desc.Contains("troubleshoot"))
        {
            DrawCloudFogTroubleshootingDetective(canvas, width, height, style);
        }
        else if (desc.Contains("bridge") && desc.Contains("clouds") && desc.Contains("hybrid"))
        {
            DrawHybridCloudBridge(canvas, width, height, style);
        }
        else if (desc.Contains("cloud") && desc.Contains("wallet") && desc.Contains("cost"))
        {
            DrawCloudCostWallet(canvas, width, height, style);
        }
        else if (desc.Contains("edge") && desc.Contains("clouds") && desc.Contains("computing"))
        {
            DrawEdgeComputingClouds(canvas, width, height, style);
        }
        else if (desc.Contains("blueprints") && desc.Contains("sky") && desc.Contains("architecture"))
        {
            DrawCloudArchitectureBlueprints(canvas, width, height, style);
        }
        else if (desc.Contains("sky") && desc.Contains("crown") && desc.Contains("cloud") && desc.Contains("mastery"))
        {
            DrawCloudMasteryCrown(canvas, width, height, style);
        }
        
        // Module 9 (Advanced Alchemy - Mixing Protocols) drawings
        else if (desc.Contains("cauldron") && desc.Contains("protocol") && desc.Contains("ingredients"))
        {
            DrawProtocolAlchemyCauldron(canvas, width, height, style);
        }
        else if (desc.Contains("web") && desc.Contains("potion") && desc.Contains("lock"))
        {
            DrawWebSecurityPotion(canvas, width, height, style);
        }
        else if (desc.Contains("file") && desc.Contains("bottles") && desc.Contains("security") && desc.Contains("armor"))
        {
            DrawFileTransferPotions(canvas, width, height, style);
        }
        else if (desc.Contains("mail") && desc.Contains("elixir") && desc.Contains("envelope"))
        {
            DrawEmailProtocolElixir(canvas, width, height, style);
        }
        else if (desc.Contains("name") && desc.Contains("tag") && desc.Contains("potion") && desc.Contains("address"))
        {
            DrawDNSNameResolverPotion(canvas, width, height, style);
        }
        else if (desc.Contains("self") && desc.Contains("filling") && desc.Contains("cauldron") && desc.Contains("ip"))
        {
            DrawDHCPAutoAssignCauldron(canvas, width, height, style);
        }
        else if (desc.Contains("tunnel") && desc.Contains("potion") && desc.Contains("encrypted"))
        {
            DrawVPNTunnelElixir(canvas, width, height, style);
        }
        else if (desc.Contains("vip") && desc.Contains("elixir") && desc.Contains("priority"))
        {
            DrawQoSPriorityPotion(canvas, width, height, style);
        }
        else if (desc.Contains("crystal") && desc.Contains("ball") && desc.Contains("monitoring"))
        {
            DrawSNMPMonitoringCrystal(canvas, width, height, style);
        }
        else if (desc.Contains("voice") && desc.Contains("bubbles") && desc.Contains("communication"))
        {
            DrawVoIPCommunicationBubbles(canvas, width, height, style);
        }
        else if (desc.Contains("magical") && desc.Contains("wand") && desc.Contains("code") && desc.Contains("cauldron"))
        {
            DrawScriptingProtocolWand(canvas, width, height, style);
        }
        else if (desc.Contains("multi") && desc.Contains("colored") && desc.Contains("layered") && desc.Contains("potion"))
        {
            DrawMultiprotocolHybridElixir(canvas, width, height, style);
        }
        else if (desc.Contains("shielded") && desc.Contains("cauldron") && desc.Contains("protective"))
        {
            DrawSecurityProtocolShield(canvas, width, height, style);
        }
        else if (desc.Contains("detective") && desc.Contains("magnifying") && desc.Contains("protocol"))
        {
            DrawProtocolTroubleshootingDetective(canvas, width, height, style);
        }
        else if (desc.Contains("futuristic") && desc.Contains("glowing") && desc.Contains("holographic"))
        {
            DrawEmergingProtocolsFuture(canvas, width, height, style);
        }
        else if (desc.Contains("miniature") && desc.Contains("potion") && desc.Contains("smart") && desc.Contains("devices"))
        {
            DrawIoTMiniaturePotions(canvas, width, height, style);
        }
        else if (desc.Contains("floating") && desc.Contains("cloud") && desc.Contains("cauldron") && desc.Contains("api"))
        {
            DrawCloudProtocolCauldron(canvas, width, height, style);
        }
        else if (desc.Contains("transparent") && desc.Contains("stacked") && desc.Contains("layers"))
        {
            DrawProtocolStackLayers(canvas, width, height, style);
        }
        else if (desc.Contains("advanced") && desc.Contains("laboratory") && desc.Contains("apparatus"))
        {
            DrawAdvancedProtocolLaboratory(canvas, width, height, style);
        }
        else if (desc.Contains("golden") && desc.Contains("master") && desc.Contains("alchemist") && desc.Contains("certification"))
        {
            DrawMasterAlchemistPotion(canvas, width, height, style);
        }
        
        // Module 10 (Mastery Mayhem - Engineer Extraordinaire) drawings
        else if (desc.Contains("full") && desc.Contains("network") && desc.Contains("cartoon") && desc.Contains("wonder"))
        {
            DrawNetworkEmpireCartoon(canvas, width, height, style);
        }
        else if (desc.Contains("hero") && desc.Contains("gadgets") && desc.Contains("crowns") && desc.Contains("command"))
        {
            DrawHardwareHeroesCommand(canvas, width, height, style);
        }
        else if (desc.Contains("ip") && desc.Contains("crown") && desc.Contains("royal"))
        {
            DrawIPAddressRoyalCrown(canvas, width, height, style);
        }
        else if (desc.Contains("ultimate") && desc.Contains("magical") && desc.Contains("wand") && desc.Contains("flowing"))
        {
            DrawUltimateMagicalWand(canvas, width, height, style);
        }
        else if (desc.Contains("routing") && desc.Contains("table") && desc.Contains("throne"))
        {
            DrawRoutingTableThrone(canvas, width, height, style);
        }
        else if (desc.Contains("armored") && desc.Contains("castle") && desc.Contains("shields") && desc.Contains("guards"))
        {
            DrawArmoredSecurityCastle(canvas, width, height, style);
        }
        else if (desc.Contains("wireless") && desc.Contains("waves") && desc.Contains("crown") && desc.Contains("power"))
        {
            DrawWirelessWaveCrown(canvas, width, height, style);
        }
        else if (desc.Contains("cloud") && desc.Contains("infrastructure") && desc.Contains("heavenly") && desc.Contains("throne"))
        {
            DrawCloudHeavenlyThrone(canvas, width, height, style);
        }
        else if (desc.Contains("ultimate") && desc.Contains("alchemical") && desc.Contains("potion") && desc.Contains("protocol"))
        {
            DrawUltimateProtocolPotion(canvas, width, height, style);
        }
        else if (desc.Contains("network") && desc.Contains("elements") && desc.Contains("fused") && desc.Contains("powerful"))
        {
            DrawIntegratedNetworkFusion(canvas, width, height, style);
        }
        else if (desc.Contains("code") && desc.Contains("castle") && desc.Contains("automation") && desc.Contains("towers"))
        {
            DrawAutomationCodeCastle(canvas, width, height, style);
        }
        else if (desc.Contains("complex") && desc.Contains("architectural") && desc.Contains("blueprints") && desc.Contains("topology"))
        {
            DrawNetworkArchitecturalBlueprints(canvas, width, height, style);
        }
        else if (desc.Contains("security") && desc.Contains("audit") && desc.Contains("shield") && desc.Contains("testing"))
        {
            DrawSecurityAuditShield(canvas, width, height, style);
        }
        else if (desc.Contains("cloud") && desc.Contains("wireless") && desc.Contains("waves") && desc.Contains("integrated"))
        {
            DrawHybridCloudWireless(canvas, width, height, style);
        }
        else if (desc.Contains("detective") && desc.Contains("magnifying") && desc.Contains("glass") && desc.Contains("crown"))
        {
            DrawTroubleshootingDetectiveCrown(canvas, width, height, style);
        }
        else if (desc.Contains("certification") && desc.Contains("chaos") && desc.Contains("study") && desc.Contains("materials"))
        {
            DrawCertificationChaos(canvas, width, height, style);
        }
        else if (desc.Contains("real") && desc.Contains("network") && desc.Contains("implementation") && desc.Contains("tools"))
        {
            DrawRealWorldImplementation(canvas, width, height, style);
        }
        else if (desc.Contains("balanced") && desc.Contains("scales") && desc.Contains("network") && desc.Contains("ethics"))
        {
            DrawEthicsBalancedScales(canvas, width, height, style);
        }
        else if (desc.Contains("futuristic") && desc.Contains("network") && desc.Contains("ai") && desc.Contains("quantum"))
        {
            DrawFuturisticNetworkAI(canvas, width, height, style);
        }
        else if (desc.Contains("supreme") && desc.Contains("trophy") && desc.Contains("module") && desc.Contains("symbols"))
        {
            DrawSupremeMasteryTrophy(canvas, width, height, style);
        }
        else
        {
            // Default: Draw a generic networking concept
            DrawGenericNetworkConcept(canvas, width, height, style);
        }
    }

    private void DrawKidsLinkingHands(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        // Draw cartoon kids holding hands in a circle
        var centerX = width / 2f;
        var centerY = height / 2f;
        var radius = Math.Min(width, height) * 0.3f;

        using var paint = new SKPaint
        {
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        };

        // Draw 5 kids in a circle
        for (int i = 0; i < 5; i++)
        {
            var angle = (float)(i * 2 * Math.PI / 5);
            var x = centerX + radius * (float)Math.Cos(angle);
            var y = centerY + radius * (float)Math.Sin(angle);

            // Draw kid as simple circle (head) and rectangle (body)
            paint.Color = GetRandomKidColor(i);
            canvas.DrawCircle(x, y - 20, 15, paint); // Head
            canvas.DrawRoundRect(x - 10, y - 5, 20, 30, 5, 5, paint); // Body

            // Draw connection lines
            var nextAngle = (float)((i + 1) * 2 * Math.PI / 5);
            var nextX = centerX + radius * (float)Math.Cos(nextAngle);
            var nextY = centerY + radius * (float)Math.Sin(nextAngle);
            
            paint.Color = PrimaryColor;
            paint.Style = SKPaintStyle.Stroke;
            paint.StrokeWidth = 3;
            canvas.DrawLine(x + 10, y, nextX - 10, nextY, paint);
            paint.Style = SKPaintStyle.Fill;
        }
    }

    private void DrawLegoBricks(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        using var paint = new SKPaint { IsAntialias = true };
        
        var colors = new[] { SKColors.Red, SKColors.Blue, SKColors.Green, SKColors.Yellow, SKColors.Orange };
        var random = new Random(42); // Consistent seed for reproducible results

        // Draw scattered LEGO bricks
        for (int i = 0; i < 8; i++)
        {
            var x = random.Next(50, width - 100);
            var y = random.Next(100, height - 150);
            var color = colors[random.Next(colors.Length)];
            
            DrawLegoBrick(canvas, paint, x, y, color);
        }

        // Draw "DATA" text with brick-like letters
        DrawBrickText(canvas, paint, "DATA", width / 2 - 80, height / 2 + 100);
    }

    private void DrawCableSpaghetti(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        using var paint = new SKPaint 
        { 
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 8,
            StrokeCap = SKStrokeCap.Round
        };

        var colors = new[] { SKColors.Red, SKColors.Blue, SKColors.Green, SKColors.Orange, SKColors.Purple };
        var random = new Random(42);

        // Draw tangled cables
        for (int i = 0; i < 6; i++)
        {
            paint.Color = colors[i % colors.Length];
            
            using var path = new SKPath();
            var startX = random.Next(0, width / 4);
            var startY = random.Next(height / 4, 3 * height / 4);
            path.MoveTo(startX, startY);

            // Create wavy, tangled path
            for (int j = 0; j < 5; j++)
            {
                var controlX1 = random.Next(0, width);
                var controlY1 = random.Next(0, height);
                var controlX2 = random.Next(0, width);
                var controlY2 = random.Next(0, height);
                var endX = random.Next(3 * width / 4, width);
                var endY = random.Next(height / 4, 3 * height / 4);
                
                path.CubicTo(controlX1, controlY1, controlX2, controlY2, endX, endY);
            }
            
            canvas.DrawPath(path, paint);
        }
    }

    private void DrawWifiWaves(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        var centerX = width / 2f;
        var centerY = height / 2f;

        using var paint = new SKPaint
        {
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 4
        };

        // Draw WiFi router in center
        paint.Style = SKPaintStyle.Fill;
        paint.Color = SKColors.DarkGray;
        canvas.DrawRoundRect(centerX - 30, centerY - 15, 60, 30, 5, 5, paint);

        // Draw antenna
        paint.Color = SKColors.Black;
        canvas.DrawRect(centerX - 2, centerY - 35, 4, 20, paint);

        // Draw WiFi waves
        paint.Style = SKPaintStyle.Stroke;
        paint.Color = PrimaryColor;
        
        for (int i = 1; i <= 4; i++)
        {
            var radius = i * 40f;
            paint.StrokeWidth = 6 - i;
            
            // Draw arc for wave
            using var path = new SKPath();
            var rect = new SKRect(centerX - radius, centerY - radius, centerX + radius, centerY + radius);
            path.AddArc(rect, -45, 90);
            canvas.DrawPath(path, paint);
        }

        // Add sparkles for "magic"
        DrawSparkles(canvas, centerX, centerY, 8);
    }

    private void DrawRouterPartyHost(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        var centerX = width / 2f;
        var centerY = height / 2f;

        using var paint = new SKPaint { IsAntialias = true, Style = SKPaintStyle.Fill };

        // Draw router as party host with hat
        paint.Color = SKColors.DarkBlue;
        canvas.DrawRoundRect(centerX - 40, centerY - 20, 80, 40, 8, 8, paint);

        // Draw party hat
        paint.Color = SKColors.Red;
        var hatPath = new SKPath();
        hatPath.MoveTo(centerX, centerY - 50);
        hatPath.LineTo(centerX - 25, centerY - 20);
        hatPath.LineTo(centerX + 25, centerY - 20);
        hatPath.Close();
        canvas.DrawPath(hatPath, paint);

        // Draw devices around router like party guests
        var devicePositions = new[]
        {
            (centerX - 150, centerY - 80),
            (centerX + 150, centerY - 80),
            (centerX - 180, centerY + 80),
            (centerX + 180, centerY + 80),
            (centerX, centerY + 120)
        };

        var deviceColors = new[] { SKColors.Green, SKColors.Orange, SKColors.Purple, SKColors.Pink, SKColors.Cyan };

        for (int i = 0; i < devicePositions.Length; i++)
        {
            var (x, y) = devicePositions[i];
            paint.Color = deviceColors[i];
            
            // Draw device
            canvas.DrawRoundRect(x - 15, y - 10, 30, 20, 3, 3, paint);
            
            // Draw connection line to router
            paint.Color = PrimaryColor;
            paint.Style = SKPaintStyle.Stroke;
            paint.StrokeWidth = 2;
            canvas.DrawLine(x, y, centerX, centerY, paint);
            paint.Style = SKPaintStyle.Fill;
        }
    }

    private void DrawGenericNetworkConcept(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        // Draw a simple network topology
        var centerX = width / 2f;
        var centerY = height / 2f;

        using var paint = new SKPaint { IsAntialias = true };

        // Draw central node
        paint.Style = SKPaintStyle.Fill;
        paint.Color = PrimaryColor;
        canvas.DrawCircle(centerX, centerY, 25, paint);

        // Draw connected nodes
        var positions = new[]
        {
            (centerX - 100, centerY - 100),
            (centerX + 100, centerY - 100),
            (centerX - 100, centerY + 100),
            (centerX + 100, centerY + 100),
            (centerX - 140, centerY),
            (centerX + 140, centerY)
        };

        foreach (var (x, y) in positions)
        {
            // Draw connection
            paint.Style = SKPaintStyle.Stroke;
            paint.Color = LightTextColor;
            paint.StrokeWidth = 2;
            canvas.DrawLine(centerX, centerY, x, y, paint);

            // Draw node
            paint.Style = SKPaintStyle.Fill;
            paint.Color = SecondaryColor;
            canvas.DrawCircle(x, y, 15, paint);
        }
    }

    #region Hardware Heroes Drawing Methods

    private void DrawHardwareSuperheroes(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        using var paint = new SKPaint { IsAntialias = true };
        
        // Draw lineup of hardware superheroes
        var heroPositions = new[]
        {
            (150f, 300f, "Router", SKColors.Red),
            (250f, 300f, "Switch", SKColors.Blue),
            (350f, 300f, "NIC", SKColors.Green),
            (450f, 300f, "Firewall", SKColors.Orange),
            (550f, 300f, "AP", SKColors.Purple)
        };

        foreach (var (x, y, name, color) in heroPositions)
        {
            // Draw superhero body
            paint.Style = SKPaintStyle.Fill;
            paint.Color = color;
            canvas.DrawRoundRect(x - 20, y - 40, 40, 60, 8, 8, paint);
            
            // Draw cape
            paint.Color = color.WithAlpha(150);
            var capePath = new SKPath();
            capePath.MoveTo(x - 25, y - 30);
            capePath.LineTo(x - 35, y + 10);
            capePath.LineTo(x - 15, y + 5);
            capePath.Close();
            canvas.DrawPath(capePath, paint);
            
            // Draw head
            paint.Color = SKColors.PeachPuff;
            canvas.DrawCircle(x, y - 60, 18, paint);
            
            // Draw hero emblem
            paint.Color = SKColors.White;
            paint.TextSize = 12;
            paint.TextAlign = SKTextAlign.Center;
            canvas.DrawText(name[0].ToString(), x, y - 15, paint);
            
            // Draw name label
            paint.Color = TextColor;
            paint.TextSize = 14;
            canvas.DrawText(name, x, y + 40, paint);
        }
        
        // Draw "HARDWARE HEROES" title
        paint.Color = PrimaryColor;
        paint.TextSize = 32;
        paint.TextAlign = SKTextAlign.Center;
        paint.Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyleWeight.Bold, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright);
        canvas.DrawText("HARDWARE HEROES", width / 2f, 100, paint);
    }

    private void DrawChattyNIC(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        var centerX = width / 2f;
        var centerY = height / 2f;
        
        using var paint = new SKPaint { IsAntialias = true };
        
        // Draw NIC card base
        paint.Style = SKPaintStyle.Fill;
        paint.Color = AccentColor;
        canvas.DrawRoundRect(centerX - 80, centerY - 40, 160, 80, 10, 10, paint);
        
        // Draw circuit board pattern
        paint.Color = SKColors.DarkGreen;
        paint.Style = SKPaintStyle.Stroke;
        paint.StrokeWidth = 2;
        for (int i = 0; i < 5; i++)
        {
            canvas.DrawLine(centerX - 70 + i * 30, centerY - 30, centerX - 70 + i * 30, centerY + 30, paint);
            canvas.DrawLine(centerX - 70, centerY - 20 + i * 10, centerX + 70, centerY - 20 + i * 10, paint);
        }
        
        // Draw chatty mouth
        paint.Style = SKPaintStyle.Fill;
        paint.Color = SKColors.Pink;
        var mouthRect = new SKRect(centerX - 30, centerY - 15, centerX + 30, centerY + 15);
        canvas.DrawOval(mouthRect, paint);
        
        // Draw teeth
        paint.Color = SKColors.White;
        for (int i = 0; i < 6; i++)
        {
            canvas.DrawRect(centerX - 25 + i * 8, centerY - 10, 5, 8, paint);
        }
        
        // Draw speech bubbles with data
        DrawSpeechBubble(canvas, paint, centerX - 120, centerY - 80, "Hello!", SKColors.LightBlue);
        DrawSpeechBubble(canvas, paint, centerX + 80, centerY - 100, "01101001", SKColors.LightGreen);
        DrawSpeechBubble(canvas, paint, centerX - 100, centerY + 80, "Packets!", SKColors.LightYellow);
        
        // Draw connector port
        paint.Color = SKColors.Black;
        canvas.DrawRect(centerX + 60, centerY - 8, 25, 16, paint);
        
        // Draw MAC address label
        paint.Color = TextColor;
        paint.TextSize = 10;
        paint.TextAlign = SKTextAlign.Center;
        canvas.DrawText("MAC: 00:1A:2B:3C:4D:5E", centerX, centerY + 60, paint);
    }

    private void DrawSwitchDJ(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        var centerX = width / 2f;
        var centerY = height / 2f;
        
        using var paint = new SKPaint { IsAntialias = true };
        
        // Draw DJ booth/switch base
        paint.Style = SKPaintStyle.Fill;
        paint.Color = SKColors.DarkGray;
        canvas.DrawRoundRect(centerX - 100, centerY, 200, 100, 15, 15, paint);
        
        // Draw switch ports
        paint.Color = SKColors.Black;
        for (int i = 0; i < 8; i++)
        {
            canvas.DrawRoundRect(centerX - 80 + i * 20, centerY + 70, 12, 8, 2, 2, paint);
        }
        
        // Draw DJ turntables
        paint.Color = PrimaryColor;
        canvas.DrawCircle(centerX - 40, centerY + 40, 25, paint);
        canvas.DrawCircle(centerX + 40, centerY + 40, 25, paint);
        
        // Draw records on turntables
        paint.Color = SKColors.Black;
        canvas.DrawCircle(centerX - 40, centerY + 40, 20, paint);
        canvas.DrawCircle(centerX + 40, centerY + 40, 20, paint);
        
        // Draw center dots
        paint.Color = SKColors.Red;
        canvas.DrawCircle(centerX - 40, centerY + 40, 3, paint);
        canvas.DrawCircle(centerX + 40, centerY + 40, 3, paint);
        
        // Draw DJ character (switch personified)
        paint.Color = SecondaryColor;
        canvas.DrawRoundRect(centerX - 15, centerY - 60, 30, 50, 5, 5, paint);
        
        // Draw head
        paint.Color = SKColors.PeachPuff;
        canvas.DrawCircle(centerX, centerY - 80, 20, paint);
        
        // Draw headphones
        paint.Color = SKColors.Black;
        paint.Style = SKPaintStyle.Stroke;
        paint.StrokeWidth = 4;
        canvas.DrawArc(new SKRect(centerX - 25, centerY - 105, centerX + 25, centerY - 55), 0, 180, false, paint);
        
        paint.Style = SKPaintStyle.Fill;
        canvas.DrawCircle(centerX - 20, centerY - 80, 8, paint);
        canvas.DrawCircle(centerX + 20, centerY - 80, 8, paint);
        
        // Draw music notes
        DrawMusicNote(canvas, paint, centerX - 70, centerY - 40);
        DrawMusicNote(canvas, paint, centerX + 70, centerY - 30);
        DrawMusicNote(canvas, paint, centerX, centerY - 120);
        
        // Draw "DJ SWITCH" label
        paint.Color = SKColors.White;
        paint.TextSize = 16;
        paint.TextAlign = SKTextAlign.Center;
        paint.Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyleWeight.Bold, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright);
        canvas.DrawText("DJ SWITCH", centerX, centerY + 130, paint);
    }

    private void DrawRouterTrafficCop(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        var centerX = width / 2f;
        var centerY = height / 2f;
        
        using var paint = new SKPaint { IsAntialias = true };
        
        // Draw router body
        paint.Style = SKPaintStyle.Fill;
        paint.Color = PrimaryColor;
        canvas.DrawRoundRect(centerX - 50, centerY - 10, 100, 40, 8, 8, paint);
        
        // Draw antennas
        paint.Style = SKPaintStyle.Stroke;
        paint.Color = SKColors.Black;
        paint.StrokeWidth = 3;
        canvas.DrawLine(centerX - 30, centerY - 10, centerX - 30, centerY - 40, paint);
        canvas.DrawLine(centerX, centerY - 10, centerX, centerY - 45, paint);
        canvas.DrawLine(centerX + 30, centerY - 10, centerX + 30, centerY - 35, paint);
        
        // Draw traffic cop character on top
        paint.Style = SKPaintStyle.Fill;
        paint.Color = SKColors.Navy;
        canvas.DrawRoundRect(centerX - 12, centerY - 60, 24, 40, 4, 4, paint);
        
        // Draw head
        paint.Color = SKColors.PeachPuff;
        canvas.DrawCircle(centerX, centerY - 80, 15, paint);
        
        // Draw police hat
        paint.Color = SKColors.Navy;
        canvas.DrawRect(centerX - 18, centerY - 88, 36, 8, paint);
        paint.Color = SKColors.Gold;
        canvas.DrawRect(centerX - 15, centerY - 95, 30, 7, paint);
        
        // Draw badge
        paint.Color = SKColors.Gold;
        canvas.DrawCircle(centerX - 8, centerY - 45, 4, paint);
        
        // Draw whistle
        paint.Color = SKColors.Silver;
        canvas.DrawCircle(centerX + 8, centerY - 50, 3, paint);
        
        // Draw stop sign (traffic control)
        paint.Color = SKColors.Red;
        var stopSign = new SKPath();
        stopSign.MoveTo(centerX - 120, centerY - 20);
        stopSign.LineTo(centerX - 100, centerY - 40);
        stopSign.LineTo(centerX - 80, centerY - 40);
        stopSign.LineTo(centerX - 60, centerY - 20);
        stopSign.LineTo(centerX - 60, centerY);
        stopSign.LineTo(centerX - 80, centerY + 20);
        stopSign.LineTo(centerX - 100, centerY + 20);
        stopSign.LineTo(centerX - 120, centerY);
        stopSign.Close();
        canvas.DrawPath(stopSign, paint);
        
        paint.Color = SKColors.White;
        paint.TextSize = 14;
        paint.TextAlign = SKTextAlign.Center;
        paint.Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyleWeight.Bold, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright);
        canvas.DrawText("STOP", centerX - 90, centerY - 5, paint);
        
        // Draw data packet cars
        DrawPacketCar(canvas, paint, centerX + 60, centerY - 30, SKColors.Yellow);
        DrawPacketCar(canvas, paint, centerX + 90, centerY + 10, SKColors.Green);
        DrawPacketCar(canvas, paint, centerX + 120, centerY + 50, SKColors.Blue);
    }

    private void DrawFirewallShield(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        var centerX = width / 2f;
        var centerY = height / 2f;
        
        using var paint = new SKPaint { IsAntialias = true };
        
        // Draw large shield
        paint.Style = SKPaintStyle.Fill;
        paint.Color = PrimaryColor;
        var shieldPath = new SKPath();
        shieldPath.MoveTo(centerX, centerY - 100);
        shieldPath.LineTo(centerX - 80, centerY - 60);
        shieldPath.LineTo(centerX - 80, centerY + 40);
        shieldPath.LineTo(centerX, centerY + 100);
        shieldPath.LineTo(centerX + 80, centerY + 40);
        shieldPath.LineTo(centerX + 80, centerY - 60);
        shieldPath.Close();
        canvas.DrawPath(shieldPath, paint);
        
        // Draw shield border
        paint.Style = SKPaintStyle.Stroke;
        paint.Color = SKColors.DarkBlue;
        paint.StrokeWidth = 5;
        canvas.DrawPath(shieldPath, paint);
        
        // Draw cross pattern (protection symbol)
        paint.Style = SKPaintStyle.Fill;
        paint.Color = SKColors.White;
        canvas.DrawRect(centerX - 5, centerY - 60, 10, 120, paint);
        canvas.DrawRect(centerX - 50, centerY - 5, 100, 10, paint);
        
        // Draw guardian eyes
        paint.Color = SKColors.Red;
        canvas.DrawCircle(centerX - 20, centerY - 30, 8, paint);
        canvas.DrawCircle(centerX + 20, centerY - 30, 8, paint);
        
        // Draw laser beams from eyes
        paint.Style = SKPaintStyle.Stroke;
        paint.Color = SKColors.Red;
        paint.StrokeWidth = 3;
        canvas.DrawLine(centerX - 20, centerY - 30, centerX - 150, centerY - 50, paint);
        canvas.DrawLine(centerX + 20, centerY - 30, centerX + 150, centerY - 50, paint);
        
        // Draw blocked threats
        DrawBlockedThreat(canvas, paint, centerX - 150, centerY - 60, "VIRUS");
        DrawBlockedThreat(canvas, paint, centerX + 150, centerY - 60, "HACK");
        DrawBlockedThreat(canvas, paint, centerX - 100, centerY + 120, "SPAM");
        
        // Draw "FIREWALL" label
        paint.Style = SKPaintStyle.Fill;
        paint.Color = SKColors.White;
        paint.TextSize = 18;
        paint.TextAlign = SKTextAlign.Center;
        paint.Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyleWeight.Bold, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright);
        canvas.DrawText("FIREWALL", centerX, centerY + 20, paint);
    }

    private void DrawAccessPointWizard(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        var centerX = width / 2f;
        var centerY = height / 2f;
        
        using var paint = new SKPaint { IsAntialias = true };
        
        // Draw wizard body (access point)
        paint.Style = SKPaintStyle.Fill;
        paint.Color = AccentColor;
        canvas.DrawRoundRect(centerX - 40, centerY - 20, 80, 60, 10, 10, paint);
        
        // Draw antenna as wizard hat
        paint.Color = SecondaryColor;
        var hatPath = new SKPath();
        hatPath.MoveTo(centerX, centerY - 80);
        hatPath.LineTo(centerX - 25, centerY - 20);
        hatPath.LineTo(centerX + 25, centerY - 20);
        hatPath.Close();
        canvas.DrawPath(hatPath, paint);
        
        // Draw stars on hat
        paint.Color = SKColors.Gold;
        DrawStar(canvas, paint, centerX - 10, centerY - 50, 5, 3);
        DrawStar(canvas, paint, centerX + 8, centerY - 60, 4, 2);
        DrawStar(canvas, paint, centerX, centerY - 35, 3, 2);
        
        // Draw wizard wand (antenna)
        paint.Style = SKPaintStyle.Stroke;
        paint.Color = SKColors.Brown;
        paint.StrokeWidth = 4;
        canvas.DrawLine(centerX + 30, centerY + 10, centerX + 60, centerY - 20, paint);
        
        // Draw magic sparkles at wand tip
        paint.Style = SKPaintStyle.Fill;
        paint.Color = SKColors.Gold;
        DrawStar(canvas, paint, centerX + 60, centerY - 20, 6, 3);
        DrawStar(canvas, paint, centerX + 55, centerY - 25, 4, 2);
        DrawStar(canvas, paint, centerX + 65, centerY - 25, 4, 2);
        
        // Draw magical WiFi waves
        paint.Style = SKPaintStyle.Stroke;
        paint.Color = PrimaryColor;
        paint.StrokeWidth = 3;
        
        for (int i = 1; i <= 4; i++)
        {
            var radius = i * 30f;
            var wavePath = new SKPath();
            var rect = new SKRect(centerX - radius, centerY - radius, centerX + radius, centerY + radius);
            wavePath.AddArc(rect, -30, 60);
            canvas.DrawPath(wavePath, paint);
        }
        
        // Draw connected devices floating around
        DrawFloatingDevice(canvas, paint, centerX - 80, centerY - 80, "📱");
        DrawFloatingDevice(canvas, paint, centerX + 80, centerY - 60, "💻");
        DrawFloatingDevice(canvas, paint, centerX - 60, centerY + 80, "🖥️");
        DrawFloatingDevice(canvas, paint, centerX + 90, centerY + 70, "📟");
        
        // Draw "WiFi Wizard" label
        paint.Style = SKPaintStyle.Fill;
        paint.Color = TextColor;
        paint.TextSize = 16;
        paint.TextAlign = SKTextAlign.Center;
        paint.Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyleWeight.Bold, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright);
        canvas.DrawText("WiFi WIZARD", centerX, centerY + 70, paint);
    }

    private void DrawServerVault(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        var centerX = width / 2f;
        var centerY = height / 2f;
        
        using var paint = new SKPaint { IsAntialias = true };
        
        // Draw vault door (server)
        paint.Style = SKPaintStyle.Fill;
        paint.Color = SKColors.DarkGray;
        canvas.DrawRoundRect(centerX - 80, centerY - 80, 160, 160, 15, 15, paint);
        
        // Draw vault door frame
        paint.Style = SKPaintStyle.Stroke;
        paint.Color = SKColors.Gray;
        paint.StrokeWidth = 8;
        canvas.DrawRoundRect(centerX - 85, centerY - 85, 170, 170, 20, 20, paint);
        
        // Draw combination lock (server ports)
        paint.Style = SKPaintStyle.Fill;
        paint.Color = SKColors.Black;
        canvas.DrawCircle(centerX, centerY, 35, paint);
        
        paint.Color = SKColors.Silver;
        canvas.DrawCircle(centerX, centerY, 30, paint);
        
        // Draw combination numbers
        paint.Color = SKColors.Black;
        paint.TextSize = 10;
        paint.TextAlign = SKTextAlign.Center;
        for (int i = 0; i < 12; i++)
        {
            var angle = i * 30 * Math.PI / 180;
            var x = centerX + 22 * (float)Math.Cos(angle);
            var y = centerY + 22 * (float)Math.Sin(angle);
            canvas.DrawText(i.ToString(), x, y + 4, paint);
        }
        
        // Draw vault handle
        paint.Color = SKColors.Gold;
        canvas.DrawCircle(centerX + 40, centerY, 8, paint);
        canvas.DrawRect(centerX + 35, centerY - 3, 15, 6, paint);
        
        // Draw server rack details
        paint.Color = SKColors.Green;
        for (int i = 0; i < 6; i++)
        {
            canvas.DrawRect(centerX - 70, centerY - 70 + i * 15, 10, 8, paint);
            canvas.DrawRect(centerX + 60, centerY - 70 + i * 15, 10, 8, paint);
        }
        
        // Draw data streams coming out
        paint.Style = SKPaintStyle.Stroke;
        paint.Color = AccentColor;
        paint.StrokeWidth = 2;
        
        var dataPoints = new[]
        {
            (centerX - 120, centerY - 50),
            (centerX - 110, centerY + 20),
            (centerX + 110, centerY - 30),
            (centerX + 120, centerY + 40)
        };
        
        foreach (var (x, y) in dataPoints)
        {
            canvas.DrawLine(centerX, centerY, x, y, paint);
            
            // Draw data packets
            paint.Style = SKPaintStyle.Fill;
            canvas.DrawRect(x - 5, y - 3, 10, 6, paint);
            paint.Style = SKPaintStyle.Stroke;
        }
        
        // Draw "DATA VAULT" label
        paint.Style = SKPaintStyle.Fill;
        paint.Color = SKColors.White;
        paint.TextSize = 14;
        paint.TextAlign = SKTextAlign.Center;
        paint.Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyleWeight.Bold, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright);
        canvas.DrawText("DATA VAULT", centerX, centerY + 110, paint);
    }

    #endregion

    #region Helper Drawing Methods

    private void DrawSpeechBubble(SKCanvas canvas, SKPaint paint, float x, float y, string text, SKColor color)
    {
        paint.Style = SKPaintStyle.Fill;
        paint.Color = color;
        
        // Draw bubble
        canvas.DrawOval(x - 25, y - 15, 50, 30, paint);
        
        // Draw tail
        var tailPath = new SKPath();
        tailPath.MoveTo(x - 10, y + 10);
        tailPath.LineTo(x - 20, y + 20);
        tailPath.LineTo(x + 5, y + 10);
        tailPath.Close();
        canvas.DrawPath(tailPath, paint);
        
        // Draw text
        paint.Color = SKColors.Black;
        paint.TextSize = 10;
        paint.TextAlign = SKTextAlign.Center;
        canvas.DrawText(text, x, y + 3, paint);
    }

    private void DrawMusicNote(SKCanvas canvas, SKPaint paint, float x, float y)
    {
        paint.Style = SKPaintStyle.Fill;
        paint.Color = SKColors.Purple;
        
        // Draw note head
        canvas.DrawOval(x - 4, y + 5, 8, 6, paint);
        
        // Draw stem
        paint.Style = SKPaintStyle.Stroke;
        paint.StrokeWidth = 2;
        canvas.DrawLine(x + 3, y + 5, x + 3, y - 15, paint);
        
        // Draw flag
        paint.Style = SKPaintStyle.Fill;
        var flagPath = new SKPath();
        flagPath.MoveTo(x + 3, y - 15);
        flagPath.LineTo(x + 12, y - 10);
        flagPath.LineTo(x + 3, y - 5);
        flagPath.Close();
        canvas.DrawPath(flagPath, paint);
    }

    private void DrawPacketCar(SKCanvas canvas, SKPaint paint, float x, float y, SKColor color)
    {
        paint.Style = SKPaintStyle.Fill;
        paint.Color = color;
        
        // Draw car body
        canvas.DrawRoundRect(x - 10, y - 5, 20, 10, 3, 3, paint);
        
        // Draw wheels
        paint.Color = SKColors.Black;
        canvas.DrawCircle(x - 6, y + 5, 3, paint);
        canvas.DrawCircle(x + 6, y + 5, 3, paint);
        
        // Draw "DATA" label
        paint.Color = SKColors.White;
        paint.TextSize = 6;
        paint.TextAlign = SKTextAlign.Center;
        canvas.DrawText("DATA", x, y + 1, paint);
    }

    private void DrawBlockedThreat(SKCanvas canvas, SKPaint paint, float x, float y, string threat)
    {
        paint.Style = SKPaintStyle.Fill;
        paint.Color = SKColors.Red;
        
        // Draw X mark
        paint.Style = SKPaintStyle.Stroke;
        paint.StrokeWidth = 4;
        canvas.DrawLine(x - 10, y - 10, x + 10, y + 10, paint);
        canvas.DrawLine(x - 10, y + 10, x + 10, y - 10, paint);
        
        // Draw threat label
        paint.Style = SKPaintStyle.Fill;
        paint.Color = SKColors.Black;
        paint.TextSize = 8;
        paint.TextAlign = SKTextAlign.Center;
        canvas.DrawText(threat, x, y + 20, paint);
    }

    private void DrawFloatingDevice(SKCanvas canvas, SKPaint paint, float x, float y, string emoji)
    {
        paint.Style = SKPaintStyle.Fill;
        paint.Color = SKColors.LightBlue.WithAlpha(100);
        
        // Draw floating glow
        canvas.DrawCircle(x, y, 15, paint);
        
        // Draw device representation (simplified)
        paint.Color = SKColors.DarkBlue;
        canvas.DrawRoundRect(x - 8, y - 6, 16, 12, 2, 2, paint);
        
        // Draw connection line
        paint.Style = SKPaintStyle.Stroke;
        paint.Color = AccentColor;
        paint.StrokeWidth = 1;
        canvas.DrawLine(x, y, 400, 300, paint); // Connect to center (access point)
    }

    #endregion

    private static SKColor GetRandomKidColor(int index)
    {
        var colors = new[] { SKColors.Pink, SKColors.LightBlue, SKColors.LightGreen, SKColors.Yellow, SKColors.Orange };
        return colors[index % colors.Length];
    }

    private void DrawLegoBrick(SKCanvas canvas, SKPaint paint, float x, float y, SKColor color)
    {
        paint.Style = SKPaintStyle.Fill;
        paint.Color = color;
        
        // Draw brick base
        canvas.DrawRoundRect(x, y, 60, 30, 3, 3, paint);
        
        // Draw studs
        paint.Color = color.WithAlpha(200);
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                canvas.DrawCircle(x + 10 + i * 15, y + 8 + j * 14, 4, paint);
            }
        }
    }

    private void DrawBrickText(SKCanvas canvas, SKPaint paint, string text, float x, float y)
    {
        paint.Style = SKPaintStyle.Fill;
        paint.Color = AccentColor;
        paint.TextSize = 32;
        paint.Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyleWeight.Bold, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright);
        
        canvas.DrawText(text, x, y, paint);
    }

    private void DrawSparkles(SKCanvas canvas, float centerX, float centerY, int count)
    {
        using var paint = new SKPaint 
        { 
            IsAntialias = true,
            Color = AccentColor,
            Style = SKPaintStyle.Fill
        };

        var random = new Random(42);
        
        for (int i = 0; i < count; i++)
        {
            var angle = random.NextDouble() * 2 * Math.PI;
            var distance = 60 + random.Next(0, 80);
            var x = centerX + (float)(distance * Math.Cos(angle));
            var y = centerY + (float)(distance * Math.Sin(angle));
            
            // Draw sparkle as small star
            DrawStar(canvas, paint, x, y, 6, 3);
        }
    }

    private void DrawStar(SKCanvas canvas, SKPaint paint, float x, float y, float outerRadius, float innerRadius)
    {
        using var path = new SKPath();
        
        for (int i = 0; i < 10; i++)
        {
            var angle = (float)(i * Math.PI / 5);
            var radius = i % 2 == 0 ? outerRadius : innerRadius;
            var px = x + radius * (float)Math.Cos(angle - Math.PI / 2);
            var py = y + radius * (float)Math.Sin(angle - Math.PI / 2);
            
            if (i == 0)
                path.MoveTo(px, py);
            else
                path.LineTo(px, py);
        }
        
        path.Close();
        canvas.DrawPath(path, paint);
    }

    private void DrawTitleOverlay(SKCanvas canvas, string title, int width, int height)
    {
        using var paint = new SKPaint
        {
            IsAntialias = true,
            Color = SKColors.White.WithAlpha(220),
            Style = SKPaintStyle.Fill
        };
        
        // Draw semi-transparent overlay at bottom
        canvas.DrawRect(0, height - 60, width, 60, paint);
        
        // Draw title text
        paint.Color = TextColor;
        paint.TextSize = 24;
        paint.Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyleWeight.Bold, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright);
        paint.TextAlign = SKTextAlign.Center;
        
        canvas.DrawText(title, width / 2f, height - 20, paint);
    }

    private async Task<byte[]> GenerateErrorImageAsync(string title, int width, int height)
    {
        using var bitmap = new SKBitmap(width, height);
        using var canvas = new SKCanvas(bitmap);
        
        canvas.Clear(SKColors.LightGray);
        
        using var paint = new SKPaint
        {
            IsAntialias = true,
            Color = SKColors.Red,
            TextSize = 32,
            TextAlign = SKTextAlign.Center,
            Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyleWeight.Bold, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright)
        };
        
        canvas.DrawText("⚠️ Image Generation Error", width / 2f, height / 2f - 20, paint);
        
        paint.TextSize = 16;
        paint.Color = SKColors.DarkRed;
        canvas.DrawText(title, width / 2f, height / 2f + 20, paint);
        
        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(SKEncodedImageFormat.Png, 80);
        
        return data.ToArray();
    }

    // Additional helper methods for CSS, JavaScript, and other content generation
    private string GenerateTextSlideCSS(TextStyle style)
    {
        return style switch
        {
            TextStyle.Educational => GetEducationalCSS(),
            TextStyle.Conversational => GetConversationalCSS(),
            TextStyle.Technical => GetTechnicalCSS(),
            TextStyle.Playful => GetPlayfulCSS(),
            TextStyle.Formal => GetFormalCSS(),
            _ => GetEducationalCSS()
        };
    }

    private string GetEducationalCSS() => @"
        <style>
        body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; margin: 0; padding: 20px; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); }
        .slide-container { max-width: 800px; margin: 0 auto; background: white; border-radius: 15px; padding: 30px; box-shadow: 0 10px 30px rgba(0,0,0,0.2); }
        .slide-title { color: #333; font-size: 2.5em; margin-bottom: 20px; text-align: center; }
        .slide-content { font-size: 1.2em; line-height: 1.8; color: #555; }
        .hover-tip { color: #007ACC; cursor: help; border-bottom: 2px dotted #007ACC; }
        .hover-tip:hover { background: #e8f4fd; }
        .fun-fact { background: #f0f8ff; padding: 15px; border-left: 4px solid #007ACC; margin: 20px 0; border-radius: 5px; }
        </style>";

    private string GetConversationalCSS() => @"
        <style>
        body { font-family: 'Comic Sans MS', cursive; margin: 0; padding: 20px; background: #ffeaa7; }
        .slide-container { max-width: 800px; margin: 0 auto; background: white; border-radius: 20px; padding: 25px; border: 3px solid #fdcb6e; }
        .slide-title { color: #e17055; font-size: 2.3em; text-align: center; text-shadow: 2px 2px 0px #fab1a0; }
        .slide-content { font-size: 1.1em; line-height: 1.7; color: #2d3436; }
        .hover-tip { color: #00b894; cursor: help; border-bottom: 2px wavy #00b894; }
        </style>";

    private string GetTechnicalCSS() => @"
        <style>
        body { font-family: 'Consolas', 'Monaco', monospace; margin: 0; padding: 20px; background: #2c3e50; color: #ecf0f1; }
        .slide-container { max-width: 900px; margin: 0 auto; background: #34495e; border-radius: 8px; padding: 25px; border: 1px solid #7f8c8d; }
        .slide-title { color: #3498db; font-size: 2.2em; text-align: center; font-weight: normal; }
        .slide-content { font-size: 1em; line-height: 1.6; color: #bdc3c7; }
        .hover-tip { color: #e74c3c; cursor: help; border-bottom: 1px solid #e74c3c; }
        </style>";

    private string GetPlayfulCSS() => @"
        <style>
        body { font-family: 'Arial', sans-serif; margin: 0; padding: 20px; background: linear-gradient(45deg, #ff9a9e, #fad0c4, #fbc2eb, #a18cd1); animation: bgShift 10s ease infinite; }
        .slide-container { max-width: 800px; margin: 0 auto; background: white; border-radius: 25px; padding: 30px; box-shadow: 0 15px 35px rgba(255,105,180,0.3); }
        .slide-title { color: #ff6b6b; font-size: 2.8em; text-align: center; transform: rotate(-2deg); }
        .slide-content { font-size: 1.3em; line-height: 1.9; color: #4a4a4a; }
        .hover-tip { color: #ff6348; cursor: help; border-bottom: 3px dotted #ff6348; transition: all 0.3s; }
        .hover-tip:hover { transform: scale(1.1); background: #ffe4e1; }
        @keyframes bgShift { 0%, 100% { filter: hue-rotate(0deg); } 50% { filter: hue-rotate(90deg); } }
        </style>";

    private string GetFormalCSS() => @"
        <style>
        body { font-family: 'Times New Roman', serif; margin: 0; padding: 20px; background: #f8f9fa; }
        .slide-container { max-width: 800px; margin: 0 auto; background: white; border: 1px solid #dee2e6; padding: 40px; }
        .slide-title { color: #212529; font-size: 2.5em; text-align: center; font-weight: normal; margin-bottom: 30px; }
        .slide-content { font-size: 1.1em; line-height: 1.8; color: #495057; text-align: justify; }
        .hover-tip { color: #495057; cursor: help; border-bottom: 1px solid #6c757d; }
        </style>";

    // Additional helper methods continue...
    private string GetStyleClass(TextStyle style) => $"style-{style.ToString().ToLowerInvariant()}";
    private string GetQuizStyleClass(QuizStyle style) => $"quiz-{style.ToString().ToLowerInvariant()}";

    private string ProcessHoverTips(string content, List<HoverTip> tips)
    {
        var processedContent = content;
        
        foreach (var tip in tips)
        {
            var pattern = $@"\b{Regex.Escape(tip.Word)}\b";
            var replacement = $"<span class='hover-tip' data-tip='{tip.Explanation}' title='{tip.Explanation}'>{tip.Word}</span>";
            processedContent = Regex.Replace(processedContent, pattern, replacement, RegexOptions.IgnoreCase);
        }
        
        return processedContent;
    }

    private string GenerateTextSlideJavaScript() => @"
        <script>
        document.addEventListener('DOMContentLoaded', function() {
            const hoverTips = document.querySelectorAll('.hover-tip');
            
            hoverTips.forEach(tip => {
                tip.addEventListener('mouseenter', function() {
                    this.style.transform = 'scale(1.05)';
                    this.style.transition = 'all 0.2s ease';
                });
                
                tip.addEventListener('mouseleave', function() {
                    this.style.transform = 'scale(1)';
                });
            });
        });
        </script>";

    private string GenerateQuizSlideCSS(QuizStyle style) => style switch
    {
        QuizStyle.Gamified => GetGamifiedQuizCSS(),
        QuizStyle.Traditional => GetTraditionalQuizCSS(),
        QuizStyle.Interactive => GetInteractiveQuizCSS(),
        QuizStyle.Timed => GetTimedQuizCSS(),
        QuizStyle.Progressive => GetProgressiveQuizCSS(),
        _ => GetGamifiedQuizCSS()
    };

    private string GetGamifiedQuizCSS() => @"
        <style>
        body { font-family: 'Arial', sans-serif; margin: 0; padding: 20px; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); }
        .quiz-container { max-width: 700px; margin: 0 auto; background: white; border-radius: 20px; padding: 30px; box-shadow: 0 15px 35px rgba(0,0,0,0.2); }
        .quiz-question { color: #333; font-size: 1.8em; text-align: center; margin-bottom: 30px; }
        .quiz-options { display: flex; flex-direction: column; gap: 15px; }
        .quiz-option { display: flex; align-items: center; padding: 15px 20px; background: #f8f9fa; border: 2px solid #dee2e6; border-radius: 10px; cursor: pointer; transition: all 0.3s; font-size: 1.1em; }
        .quiz-option:hover { background: #e3f2fd; border-color: #2196f3; transform: translateY(-2px); box-shadow: 0 5px 15px rgba(33,150,243,0.3); }
        .option-letter { background: #007ACC; color: white; border-radius: 50%; width: 30px; height: 30px; display: flex; align-items: center; justify-content: center; font-weight: bold; margin-right: 15px; }
        .quiz-feedback { margin-top: 20px; padding: 15px; border-radius: 10px; display: none; }
        .quiz-explanation { margin-top: 15px; padding: 15px; background: #fff3cd; border-left: 4px solid #ffc107; border-radius: 5px; }
        .correct-feedback { background: #d4edda; color: #155724; border: 1px solid #c3e6cb; }
        .incorrect-feedback { background: #f8d7da; color: #721c24; border: 1px solid #f1b0b7; }
        </style>";

    private string GetTraditionalQuizCSS() => @"
        <style>
        body { font-family: 'Times New Roman', serif; margin: 0; padding: 20px; background: #f5f5f5; }
        .quiz-container { max-width: 600px; margin: 0 auto; background: white; border: 2px solid #ccc; padding: 30px; }
        .quiz-question { color: #333; font-size: 1.5em; margin-bottom: 25px; }
        .quiz-options { list-style: none; padding: 0; }
        .quiz-option { padding: 10px 15px; margin: 10px 0; background: #fafafa; border: 1px solid #ddd; cursor: pointer; }
        .quiz-option:hover { background: #f0f0f0; }
        </style>";

    private string GetInteractiveQuizCSS() => GetGamifiedQuizCSS(); // Similar to gamified for now
    private string GetTimedQuizCSS() => GetGamifiedQuizCSS(); // Similar to gamified for now  
    private string GetProgressiveQuizCSS() => GetGamifiedQuizCSS(); // Similar to gamified for now

    private string GenerateQuizJavaScript(QuizQuestion question) => $@"
        <script>
        document.addEventListener('DOMContentLoaded', function() {{
            const options = document.querySelectorAll('.quiz-option');
            const feedback = document.getElementById('quiz-feedback');
            const feedbackText = document.getElementById('feedback-text');
            const explanation = document.getElementById('quiz-explanation');
            let answered = false;

            options.forEach((option, index) => {{
                option.addEventListener('click', function() {{
                    if (answered) return;
                    answered = true;

                    const isCorrect = index === {question.CorrectAnswerIndex};
                    
                    if (isCorrect) {{
                        this.style.background = '#d4edda';
                        this.style.borderColor = '#28a745';
                        feedbackText.textContent = '{question.Explanation}';
                        feedback.className = 'quiz-feedback correct-feedback';
                    }} else {{
                        this.style.background = '#f8d7da';
                        this.style.borderColor = '#dc3545';
                        
                        // Highlight correct answer
                        options[{question.CorrectAnswerIndex}].style.background = '#d4edda';
                        options[{question.CorrectAnswerIndex}].style.borderColor = '#28a745';
                        
                        const wrongFeedback = {JsonSerializer.Serialize(question.WrongAnswerFeedback)};
                        feedbackText.textContent = wrongFeedback[index] || 'Not quite right, but great effort!';
                        feedback.className = 'quiz-feedback incorrect-feedback';
                    }}

                    feedback.style.display = 'block';
                    if (explanation) {{
                        explanation.style.display = 'block';
                    }}

                    // Disable all options
                    options.forEach(opt => {{
                        opt.style.cursor = 'default';
                        opt.style.pointerEvents = 'none';
                    }});
                }});
            }});
        }});
        </script>";

    private string GenerateFeedbackHtml(QuizQuestion question)
    {
        var feedback = new StringBuilder();
        feedback.AppendLine("<div class='quiz-feedback-container'>");
        feedback.AppendLine($"<h4>✅ Correct Answer: {question.Options[question.CorrectAnswerIndex]}</h4>");
        
        if (!string.IsNullOrEmpty(question.Explanation))
        {
            feedback.AppendLine($"<p><strong>Explanation:</strong> {question.Explanation}</p>");
        }
        
        feedback.AppendLine("</div>");
        return feedback.ToString();
    }

    private string GenerateErrorTextSlide(string title, string error)
    {
        return $@"
            <!DOCTYPE html>
            <html>
            <head><title>Error: {title}</title></head>
            <body style='font-family: Arial; padding: 20px; background: #ffebee;'>
                <div style='max-width: 600px; margin: 0 auto; text-align: center;'>
                    <h1 style='color: #c62828;'>⚠️ Slide Generation Error</h1>
                    <h2>{title}</h2>
                    <p style='color: #666; font-style: italic;'>{error}</p>
                    <p>The cosmic slide generator experienced a temporary disturbance. Please try again!</p>
                </div>
            </body>
            </html>";
    }

    private string GenerateErrorQuizSlide(string question, string error)
    {
        return $@"
            <!DOCTYPE html>
            <html>
            <head><title>Quiz Error</title></head>
            <body style='font-family: Arial; padding: 20px; background: #ffebee;'>
                <div style='max-width: 600px; margin: 0 auto; text-align: center;'>
                    <h1 style='color: #c62828;'>⚠️ Quiz Generation Error</h1>
                    <h3>{question}</h3>
                    <p style='color: #666;'>{error}</p>
                </div>
            </body>
            </html>";
    }

    private async Task DrawNetworkDiagramAsync(SKCanvas canvas, NetworkDiagramType diagramType, List<NetworkNode> nodes, List<NetworkConnection> connections, List<DiagramAnnotation> annotations, int width, int height)
    {
        // Simplified network diagram drawing - full implementation would be more complex
        using var paint = new SKPaint { IsAntialias = true };
        
        // Draw nodes
        foreach (var node in nodes)
        {
            paint.Style = SKPaintStyle.Fill;
            paint.Color = SKColor.Parse(node.Color);
            canvas.DrawCircle(node.X, node.Y, 20, paint);
            
            // Draw label
            paint.Color = TextColor;
            paint.TextSize = 12;
            paint.TextAlign = SKTextAlign.Center;
            canvas.DrawText(node.Label, node.X, node.Y + 35, paint);
        }
        
        // Draw connections
        paint.Style = SKPaintStyle.Stroke;
        foreach (var connection in connections)
        {
            var fromNode = nodes.FirstOrDefault(n => n.Id == connection.FromNodeId);
            var toNode = nodes.FirstOrDefault(n => n.Id == connection.ToNodeId);
            
            if (fromNode != null && toNode != null)
            {
                paint.Color = SKColor.Parse(connection.Color);
                paint.StrokeWidth = connection.Thickness;
                canvas.DrawLine(fromNode.X, fromNode.Y, toNode.X, toNode.Y, paint);
            }
        }
        
        // Draw annotations
        foreach (var annotation in annotations)
        {
            paint.Style = SKPaintStyle.Fill;
            paint.Color = SKColor.Parse(annotation.Color);
            paint.TextSize = 14;
            paint.TextAlign = SKTextAlign.Left;
            canvas.DrawText(annotation.Text, annotation.X, annotation.Y, paint);
        }
    }

    private string GetDiagramTitle(NetworkDiagramType diagramType) => diagramType switch
    {
        NetworkDiagramType.Star => "Star Network Topology",
        NetworkDiagramType.Mesh => "Mesh Network Topology", 
        NetworkDiagramType.Bus => "Bus Network Topology",
        NetworkDiagramType.Ring => "Ring Network Topology",
        NetworkDiagramType.Hybrid => "Hybrid Network Topology",
        NetworkDiagramType.Tree => "Tree Network Topology",
        NetworkDiagramType.OSIModel => "OSI Model Layers",
        NetworkDiagramType.PacketFlow => "Packet Flow Diagram",
        _ => "Network Diagram"
    };

    private string GenerateThreeJsSceneConfig(ModelType modelType, Dictionary<string, object> interactionData)
    {
        // Simplified Three.js scene configuration
        return JsonSerializer.Serialize(new
        {
            modelType = modelType.ToString(),
            camera = new { position = new { x = 0, y = 0, z = 5 } },
            lighting = new { ambient = 0.4f, directional = 0.6f },
            controls = new { enableZoom = true, enableRotate = true },
            background = "#f0f0f0",
            interactionData
        });
    }

    private string GenerateThreeJsHtmlWrapper(string title, ModelType modelType)
    {
        return $@"
            <div id='threejs-container' style='width: 100%; height: 500px; position: relative;'>
                <h2 style='text-align: center; margin-bottom: 20px;'>{title}</h2>
                <div id='threejs-canvas' style='width: 100%; height: 400px; border: 1px solid #ccc; border-radius: 8px;'></div>
                <div style='text-align: center; margin-top: 10px; color: #666; font-size: 0.9em;'>
                    Interactive 3D Model: {modelType} (Click and drag to explore!)
                </div>
            </div>";
    }

    private Dictionary<string, object> GenerateThreeJsControls(ModelType modelType, Dictionary<string, object> interactionData)
    {
        return new Dictionary<string, object>
        {
            ["enableZoom"] = true,
            ["enableRotate"] = true,
            ["enablePan"] = false,
            ["autoRotate"] = modelType == ModelType.NetworkTopology,
            ["autoRotateSpeed"] = 2.0f,
            ["maxDistance"] = 10.0f,
            ["minDistance"] = 2.0f
        };
    }

    private List<string> GetRequiredThreeJsScripts(ModelType modelType)
    {
        var scripts = new List<string>
        {
            "three.min.js",
            "OrbitControls.js"
        };

        switch (modelType)
        {
            case ModelType.NetworkTopology:
                scripts.Add("network-topology.js");
                break;
            case ModelType.OSILayers:
                scripts.Add("osi-layers.js");
                break;
            case ModelType.DataFlow:
                scripts.Add("data-flow.js");
                break;
            default:
                scripts.Add("generic-model.js");
                break;
        }

        return scripts;
    }

    #endregion

    #region Module 3: IP Shenanigans Drawing Methods

    /// <summary>
    /// Draw IP address as digital house with address number
    /// </summary>
    private void DrawIPHouseAddress(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        var centerX = width / 2f;
        var centerY = height / 2f;

        using var paint = new SKPaint();

        // Draw house
        paint.Style = SKPaintStyle.Fill;
        paint.Color = PrimaryColor;
        var houseWidth = 120f;
        var houseHeight = 100f;
        canvas.DrawRect(centerX - houseWidth/2, centerY - houseHeight/2, houseWidth, houseHeight, paint);

        // Draw roof
        paint.Color = SecondaryColor;
        var roofPath = new SKPath();
        roofPath.MoveTo(centerX - houseWidth/2 - 10, centerY - houseHeight/2);
        roofPath.LineTo(centerX, centerY - houseHeight/2 - 30);
        roofPath.LineTo(centerX + houseWidth/2 + 10, centerY - houseHeight/2);
        canvas.DrawPath(roofPath, paint);

        // Draw door
        paint.Color = SKColors.Brown;
        canvas.DrawRect(centerX - 15, centerY + houseHeight/2 - 40, 30, 40, paint);

        // Draw address plaque
        paint.Color = SKColors.White;
        canvas.DrawRoundRect(centerX - 35, centerY - 20, 70, 25, 3, 3, paint);
        
        paint.Color = TextColor;
        paint.TextSize = 14;
        paint.TextAlign = SKTextAlign.Center;
        canvas.DrawText("192.168.1.1", centerX, centerY - 2, paint);

        // Draw mailbox
        paint.Color = AccentColor;
        canvas.DrawRoundRect(centerX + houseWidth/2 + 20, centerY, 25, 40, 5, 5, paint);
        paint.Color = SKColors.White;
        paint.TextSize = 10;
        canvas.DrawText("📬", centerX + houseWidth/2 + 32, centerY + 25, paint);

        DrawTitleOverlay(canvas, "Digital House with IP Address", width, height);
    }

    /// <summary>
    /// Draw IPv4 vs IPv6 houses comparison
    /// </summary>
    private void DrawIPv4VsIPv6Houses(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        var leftX = width * 0.25f;
        var rightX = width * 0.75f;
        var centerY = height / 2f;

        using var paint = new SKPaint();

        // IPv4 House (vintage)
        paint.Style = SKPaintStyle.Fill;
        paint.Color = SKColor.Parse("#8B7355"); // Brown vintage color
        canvas.DrawRect(leftX - 50, centerY - 40, 100, 80, paint);
        
        // IPv4 roof
        paint.Color = SKColor.Parse("#654321");
        var roofPath = new SKPath();
        roofPath.MoveTo(leftX - 60, centerY - 40);
        roofPath.LineTo(leftX, centerY - 70);
        roofPath.LineTo(leftX + 60, centerY - 40);
        canvas.DrawPath(roofPath, paint);

        // IPv4 label
        paint.Color = TextColor;
        paint.TextSize = 16;
        paint.TextAlign = SKTextAlign.Center;
        canvas.DrawText("IPv4", leftX, centerY + 60, paint);
        paint.TextSize = 12;
        canvas.DrawText("~4.3 Billion", leftX, centerY + 80, paint);

        // IPv6 House (modern skyscraper)
        paint.Style = SKPaintStyle.Fill;
        paint.Color = PrimaryColor;
        canvas.DrawRect(rightX - 40, centerY - 80, 80, 120, paint);
        
        // Modern accents
        paint.Color = AccentColor;
        for (int i = 0; i < 5; i++)
        {
            canvas.DrawRect(rightX - 30, centerY - 70 + i * 20, 60, 3, paint);
        }

        // IPv6 label
        paint.Color = TextColor;
        paint.TextSize = 16;
        paint.TextAlign = SKTextAlign.Center;
        canvas.DrawText("IPv6", rightX, centerY + 60, paint);
        paint.TextSize = 12;
        canvas.DrawText("340 Undecillion!", rightX, centerY + 80, paint);

        DrawTitleOverlay(canvas, "IPv4 Vintage vs IPv6 Modern", width, height);
    }

    /// <summary>
    /// Draw dynamic IP as musical chairs
    /// </summary>
    private void DrawDynamicIPMusicalChairs(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        var centerX = width / 2f;
        var centerY = height / 2f;
        var radius = 100f;

        using var paint = new SKPaint();

        // Draw chairs in circle
        var chairColors = new[] { PrimaryColor, SecondaryColor, AccentColor, SKColors.Purple };
        for (int i = 0; i < 4; i++)
        {
            var angle = i * Math.PI / 2;
            var x = centerX + Math.Cos(angle) * radius;
            var y = centerY + Math.Sin(angle) * radius;

            paint.Color = chairColors[i];
            paint.Style = SKPaintStyle.Fill;
            
            // Draw chair
            canvas.DrawRect((float)x - 15, (float)y - 10, 30, 20, paint);
            canvas.DrawRect((float)x - 15, (float)y - 25, 30, 15, paint); // Back
            
            // Draw IP labels
            paint.Color = TextColor;
            paint.TextSize = 10;
            paint.TextAlign = SKTextAlign.Center;
            canvas.DrawText($"192.168.1.{10 + i}", (float)x, (float)y + 40, paint);
        }

        // Draw devices (people) moving around
        paint.Color = LightTextColor;
        for (int i = 0; i < 3; i++)
        {
            var angle = i * Math.PI * 2 / 3;
            var x = centerX + Math.Cos(angle) * (radius - 30);
            var y = centerY + Math.Sin(angle) * (radius - 30);
            
            canvas.DrawCircle((float)x, (float)y, 8, paint);
            paint.TextSize = 12;
            paint.TextAlign = SKTextAlign.Center;
            canvas.DrawText("📱", (float)x, (float)y + 5, paint);
        }

        // Draw movement arrows
        paint.Color = SecondaryColor;
        paint.Style = SKPaintStyle.Stroke;
        paint.StrokeWidth = 3;
        canvas.DrawCircle(centerX, centerY, radius - 30, paint);

        DrawTitleOverlay(canvas, "Dynamic IP Musical Chairs", width, height);
    }

    /// <summary>
    /// Draw static IP as anchored house
    /// </summary>
    private void DrawStaticIPAnchoredHouse(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        var centerX = width / 2f;
        var centerY = height / 2f;

        using var paint = new SKPaint();

        // Draw house
        paint.Style = SKPaintStyle.Fill;
        paint.Color = PrimaryColor;
        canvas.DrawRect(centerX - 60, centerY - 50, 120, 80, paint);

        // Draw roof
        paint.Color = SecondaryColor;
        var roofPath = new SKPath();
        roofPath.MoveTo(centerX - 70, centerY - 50);
        roofPath.LineTo(centerX, centerY - 80);
        roofPath.LineTo(centerX + 70, centerY - 50);
        canvas.DrawPath(roofPath, paint);

        // Draw anchor
        paint.Color = SKColors.Gray;
        paint.StrokeWidth = 6;
        paint.Style = SKPaintStyle.Stroke;
        
        // Anchor chain
        canvas.DrawLine(centerX, centerY + 30, centerX, centerY + 80, paint);
        
        // Anchor body
        paint.Style = SKPaintStyle.Fill;
        canvas.DrawRect(centerX - 15, centerY + 70, 30, 40, paint);
        
        // Anchor arms
        var armPath = new SKPath();
        armPath.MoveTo(centerX - 20, centerY + 100);
        armPath.LineTo(centerX - 10, centerY + 110);
        armPath.LineTo(centerX, centerY + 105);
        armPath.LineTo(centerX + 10, centerY + 110);
        armPath.LineTo(centerX + 20, centerY + 100);
        canvas.DrawPath(armPath, paint);

        // Draw permanent address sign
        paint.Color = AccentColor;
        paint.Style = SKPaintStyle.Fill;
        canvas.DrawRoundRect(centerX - 45, centerY - 10, 90, 20, 5, 5, paint);
        
        paint.Color = SKColors.White;
        paint.TextSize = 12;
        paint.TextAlign = SKTextAlign.Center;
        canvas.DrawText("PERMANENT: 10.0.0.100", centerX, centerY + 5, paint);

        DrawTitleOverlay(canvas, "Static IP Anchored House", width, height);
    }

    /// <summary>
    /// Draw DHCP as butler with keys
    /// </summary>
    private void DrawDHCPButlerKeys(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        var centerX = width / 2f;
        var centerY = height / 2f;

        using var paint = new SKPaint();

        // Draw butler body
        paint.Style = SKPaintStyle.Fill;
        paint.Color = SKColors.Black;
        canvas.DrawOval(centerX - 30, centerY - 40, 60, 80, paint);

        // Draw butler head
        paint.Color = SKColor.Parse("#FDBCB4"); // Skin tone
        canvas.DrawCircle(centerX, centerY - 60, 25, paint);

        // Draw bow tie
        paint.Color = SecondaryColor;
        var bowTie = new SKPath();
        bowTie.MoveTo(centerX - 15, centerY - 45);
        bowTie.LineTo(centerX + 15, centerY - 45);
        bowTie.LineTo(centerX, centerY - 35);
        bowTie.LineTo(centerX - 15, centerY - 45);
        canvas.DrawPath(bowTie, paint);

        // Draw arms
        paint.Color = SKColors.Black;
        canvas.DrawOval(centerX - 50, centerY - 20, 20, 40, paint);
        canvas.DrawOval(centerX + 30, centerY - 20, 20, 40, paint);

        // Draw keys
        paint.Color = SKColors.Gold;
        for (int i = 0; i < 3; i++)
        {
            var keyX = centerX + 35 + i * 8;
            var keyY = centerY - 15 + i * 5;
            
            // Key body
            canvas.DrawCircle(keyX, keyY, 5, paint);
            // Key shaft
            canvas.DrawRect(keyX - 2, keyY, 15, 4, paint);
            // Key teeth
            canvas.DrawRect(keyX + 10, keyY - 2, 3, 2, paint);
            canvas.DrawRect(keyX + 13, keyY + 2, 2, 2, paint);
        }

        // Draw IP addresses on keys
        paint.Color = TextColor;
        paint.TextSize = 8;
        paint.TextAlign = SKTextAlign.Center;
        canvas.DrawText("192.168.1.100", centerX + 45, centerY + 15, paint);
        canvas.DrawText("192.168.1.101", centerX + 45, centerY + 25, paint);
        canvas.DrawText("192.168.1.102", centerX + 45, centerY + 35, paint);

        // Butler face
        paint.Color = TextColor;
        canvas.DrawCircle(centerX - 8, centerY - 65, 2, paint); // Left eye
        canvas.DrawCircle(centerX + 8, centerY - 65, 2, paint); // Right eye
        
        // Smile
        paint.Style = SKPaintStyle.Stroke;
        paint.StrokeWidth = 2;
        var smilePath = new SKPath();
        smilePath.AddArc(SKRect.Create(centerX - 10, centerY - 55, 20, 10), 0, 180);
        canvas.DrawPath(smilePath, paint);

        DrawTitleOverlay(canvas, "DHCP Butler with IP Keys", width, height);
    }

    /// <summary>
    /// Draw subnets as fenced city blocks
    /// </summary>
    private void DrawSubnetFencedBlocks(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        using var paint = new SKPaint();

        var blockColors = new[] { PrimaryColor, SecondaryColor, AccentColor, SKColors.Purple };
        var blockLabels = new[] { "Sales /26", "Engineering /27", "HR /28", "Guest /29" };

        // Draw 2x2 grid of subnet blocks
        for (int row = 0; row < 2; row++)
        {
            for (int col = 0; col < 2; col++)
            {
                var x = width * 0.25f + col * width * 0.4f;
                var y = height * 0.25f + row * height * 0.4f;
                var blockIndex = row * 2 + col;

                // Draw block
                paint.Style = SKPaintStyle.Fill;
                paint.Color = blockColors[blockIndex].WithAlpha(180);
                canvas.DrawRoundRect(x - 80, y - 50, 160, 100, 10, 10, paint);

                // Draw fence around block
                paint.Style = SKPaintStyle.Stroke;
                paint.Color = TextColor;
                paint.StrokeWidth = 3;
                
                // Fence posts
                for (int i = 0; i < 6; i++)
                {
                    var postX = x - 80 + i * 32;
                    canvas.DrawLine(postX, y - 55, postX, y - 45, paint);
                    canvas.DrawLine(postX, y + 45, postX, y + 55, paint);
                }
                
                // Horizontal fence bars
                canvas.DrawLine(x - 80, y - 50, x + 80, y - 50, paint);
                canvas.DrawLine(x - 80, y + 50, x + 80, y + 50, paint);

                // Block label
                paint.Style = SKPaintStyle.Fill;
                paint.Color = TextColor;
                paint.TextSize = 14;
                paint.TextAlign = SKTextAlign.Center;
                canvas.DrawText(blockLabels[blockIndex], x, y, paint);

                // Add small houses in each block
                paint.Color = SKColors.Gray;
                for (int i = 0; i < 3; i++)
                {
                    var houseX = x - 40 + i * 40;
                    canvas.DrawRect(houseX - 5, y + 10, 10, 15, paint);
                }
            }
        }

        DrawTitleOverlay(canvas, "Subnet Fenced City Blocks", width, height);
    }

    /// <summary>
    /// Draw CIDR as pizza cutter
    /// </summary>
    private void DrawCIDRPizzaCutter(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        var centerX = width / 2f;
        var centerY = height / 2f;

        using var paint = new SKPaint();

        // Draw pizza base
        paint.Style = SKPaintStyle.Fill;
        paint.Color = SKColor.Parse("#D2691E"); // Pizza crust color
        canvas.DrawCircle(centerX, centerY, 100, paint);

        // Pizza sauce
        paint.Color = SKColor.Parse("#FF6347"); // Tomato red
        canvas.DrawCircle(centerX, centerY, 85, paint);

        // Pizza cheese
        paint.Color = SKColor.Parse("#FFD700"); // Golden cheese
        canvas.DrawCircle(centerX, centerY, 80, paint);

        // Draw CIDR cuts
        paint.Color = TextColor;
        paint.Style = SKPaintStyle.Stroke;
        paint.StrokeWidth = 3;

        // /24 -> /26 cuts (4 pieces)
        canvas.DrawLine(centerX, centerY - 80, centerX, centerY + 80, paint);
        canvas.DrawLine(centerX - 80, centerY, centerX + 80, centerY, paint);

        // Draw pizza cutter
        paint.Color = SKColors.Silver;
        paint.Style = SKPaintStyle.Fill;
        
        // Handle
        canvas.DrawRect(centerX + 90, centerY - 50, 8, 100, paint);
        
        // Blade
        canvas.DrawCircle(centerX + 70, centerY, 15, paint);
        paint.Style = SKPaintStyle.Stroke;
        paint.StrokeWidth = 2;
        canvas.DrawCircle(centerX + 70, centerY, 15, paint);

        // Label each slice
        paint.Style = SKPaintStyle.Fill;
        paint.Color = TextColor;
        paint.TextSize = 12;
        paint.TextAlign = SKTextAlign.Center;
        
        canvas.DrawText("/26", centerX - 40, centerY - 40, paint);
        canvas.DrawText("64 IPs", centerX - 40, centerY - 25, paint);
        
        canvas.DrawText("/26", centerX + 40, centerY - 40, paint);
        canvas.DrawText("64 IPs", centerX + 40, centerY - 25, paint);
        
        canvas.DrawText("/26", centerX - 40, centerY + 40, paint);
        canvas.DrawText("64 IPs", centerX - 40, centerY + 55, paint);
        
        canvas.DrawText("/26", centerX + 40, centerY + 40, paint);
        canvas.DrawText("64 IPs", centerX + 40, centerY + 55, paint);

        DrawTitleOverlay(canvas, "CIDR Pizza Slicer", width, height);
    }

    /// <summary>
    /// Draw private vs public addresses as home vs street
    /// </summary>
    private void DrawPrivateVsPublicAddresses(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        var leftX = width * 0.3f;
        var rightX = width * 0.7f;
        var centerY = height / 2f;

        using var paint = new SKPaint();

        // Private network (home)
        paint.Style = SKPaintStyle.Fill;
        paint.Color = AccentColor.WithAlpha(100);
        canvas.DrawRoundRect(leftX - 80, centerY - 60, 160, 120, 15, 15, paint);

        // Private house
        paint.Color = PrimaryColor;
        canvas.DrawRect(leftX - 40, centerY - 20, 80, 60, paint);
        
        // Private roof
        var roofPath = new SKPath();
        roofPath.MoveTo(leftX - 50, centerY - 20);
        roofPath.LineTo(leftX, centerY - 45);
        roofPath.LineTo(leftX + 50, centerY - 20);
        canvas.DrawPath(roofPath, paint);

        // Private labels
        paint.Color = TextColor;
        paint.TextSize = 12;
        paint.TextAlign = SKTextAlign.Center;
        canvas.DrawText("PRIVATE", leftX, centerY - 80, paint);
        canvas.DrawText("192.168.1.x", leftX, centerY + 60, paint);
        canvas.DrawText("10.x.x.x", leftX, centerY + 75, paint);

        // Public network (street)
        paint.Style = SKPaintStyle.Fill;
        paint.Color = SecondaryColor.WithAlpha(100);
        canvas.DrawRoundRect(rightX - 80, centerY - 60, 160, 120, 15, 15, paint);

        // Public buildings
        paint.Color = SKColors.Gray;
        for (int i = 0; i < 3; i++)
        {
            var buildingX = rightX - 30 + i * 30;
            var buildingHeight = 40 + i * 10;
            canvas.DrawRect(buildingX - 10, centerY + 40 - buildingHeight, 20, buildingHeight, paint);
        }

        // Street
        paint.Color = SKColors.DarkGray;
        canvas.DrawRect(rightX - 80, centerY + 40, 160, 10, paint);

        // Street lines
        paint.Color = SKColors.White;
        paint.StrokeWidth = 2;
        paint.Style = SKPaintStyle.Stroke;
        for (int i = 0; i < 4; i++)
        {
            var lineX = rightX - 60 + i * 40;
            canvas.DrawLine(lineX, centerY + 42, lineX + 15, centerY + 48, paint);
        }

        // Public labels
        paint.Style = SKPaintStyle.Fill;
        paint.Color = TextColor;
        paint.TextSize = 12;
        paint.TextAlign = SKTextAlign.Center;
        canvas.DrawText("PUBLIC", rightX, centerY - 80, paint);
        canvas.DrawText("8.8.8.8", rightX, centerY + 60, paint);
        canvas.DrawText("208.67.222.222", rightX, centerY + 75, paint);

        DrawTitleOverlay(canvas, "Private Home vs Public Street", width, height);
    }

    /// <summary>
    /// Draw NAT as diplomatic interpreter
    /// </summary>
    private void DrawNATInterpreter(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        var centerX = width / 2f;
        var centerY = height / 2f;

        using var paint = new SKPaint();

        // Draw interpreter person
        paint.Style = SKPaintStyle.Fill;
        paint.Color = SKColor.Parse("#FDBCB4"); // Skin tone
        canvas.DrawCircle(centerX, centerY - 40, 20, paint);

        // Body
        paint.Color = PrimaryColor;
        canvas.DrawOval(centerX - 25, centerY - 20, 50, 60, paint);

        // Arms
        canvas.DrawOval(centerX - 40, centerY - 10, 15, 40, paint);
        canvas.DrawOval(centerX + 25, centerY - 10, 15, 40, paint);

        // Speech bubbles for translation
        paint.Color = SKColors.White;
        paint.Style = SKPaintStyle.Fill;
        
        // Private speech bubble (left)
        canvas.DrawRoundRect(centerX - 120, centerY - 60, 80, 30, 5, 5, paint);
        paint.Color = TextColor;
        paint.TextSize = 10;
        paint.TextAlign = SKTextAlign.Center;
        canvas.DrawText("192.168.1.5:1024", centerX - 80, centerY - 40, paint);

        // Public speech bubble (right)
        paint.Color = SKColors.White;
        canvas.DrawRoundRect(centerX + 40, centerY - 60, 80, 30, 5, 5, paint);
        paint.Color = TextColor;
        canvas.DrawText("203.0.113.1:5000", centerX + 80, centerY - 40, paint);

        // Translation arrows
        paint.Color = SecondaryColor;
        paint.Style = SKPaintStyle.Stroke;
        paint.StrokeWidth = 3;
        
        // Left to right arrow
        canvas.DrawLine(centerX - 40, centerY - 45, centerX + 40, centerY - 45, paint);
        canvas.DrawLine(centerX + 35, centerY - 50, centerX + 40, centerY - 45, paint);
        canvas.DrawLine(centerX + 35, centerY - 40, centerX + 40, centerY - 45, paint);

        // NAT table
        paint.Style = SKPaintStyle.Fill;
        paint.Color = SKColors.LightGray;
        canvas.DrawRoundRect(centerX - 60, centerY + 50, 120, 40, 5, 5, paint);
        
        paint.Color = TextColor;
        paint.TextSize = 10;
        paint.TextAlign = SKTextAlign.Center;
        canvas.DrawText("NAT Translation Table", centerX, centerY + 65, paint);
        canvas.DrawText("Private → Public", centerX, centerY + 80, paint);

        DrawTitleOverlay(canvas, "NAT Diplomatic Interpreter", width, height);
    }

    /// <summary>
    /// Draw ARP as detective
    /// </summary>
    private void DrawARPDetective(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        var centerX = width / 2f;
        var centerY = height / 2f;

        using var paint = new SKPaint();

        // Draw detective body
        paint.Style = SKPaintStyle.Fill;
        paint.Color = SKColors.Brown; // Trench coat
        canvas.DrawOval(centerX - 25, centerY - 20, 50, 80, paint);

        // Detective head
        paint.Color = SKColor.Parse("#FDBCB4"); // Skin tone
        canvas.DrawCircle(centerX, centerY - 50, 18, paint);

        // Detective hat
        paint.Color = SKColors.Brown;
        canvas.DrawOval(centerX - 20, centerY - 65, 40, 15, paint);
        canvas.DrawRect(centerX - 15, centerY - 70, 30, 10, paint);

        // Magnifying glass
        paint.Color = SKColors.Brown; // Handle
        canvas.DrawRect(centerX + 25, centerY - 10, 4, 30, paint);
        
        paint.Color = SKColors.Silver; // Rim
        paint.Style = SKPaintStyle.Stroke;
        paint.StrokeWidth = 3;
        canvas.DrawCircle(centerX + 35, centerY - 15, 12, paint);

        // Glass lens effect
        paint.Color = SKColors.LightBlue.WithAlpha(100);
        paint.Style = SKPaintStyle.Fill;
        canvas.DrawCircle(centerX + 35, centerY - 15, 9, paint);

        // Detective badge
        paint.Color = SKColors.Gold;
        canvas.DrawRect(centerX - 15, centerY, 12, 12, paint);
        paint.Color = TextColor;
        paint.TextSize = 8;
        paint.TextAlign = SKTextAlign.Center;
        canvas.DrawText("ARP", centerX - 9, centerY + 8, paint);

        // Investigation clues
        paint.Color = SKColors.White;
        paint.Style = SKPaintStyle.Fill;
        
        // Clue 1 - IP address
        canvas.DrawRoundRect(centerX - 80, centerY + 30, 60, 20, 3, 3, paint);
        paint.Color = TextColor;
        paint.TextSize = 10;
        paint.TextAlign = SKTextAlign.Center;
        canvas.DrawText("192.168.1.100", centerX - 50, centerY + 43, paint);

        // Arrow pointing to MAC
        paint.Color = SecondaryColor;
        paint.Style = SKPaintStyle.Stroke;
        paint.StrokeWidth = 2;
        canvas.DrawLine(centerX - 20, centerY + 40, centerX + 20, centerY + 40, paint);
        canvas.DrawLine(centerX + 15, centerY + 35, centerX + 20, centerY + 40, paint);
        canvas.DrawLine(centerX + 15, centerY + 45, centerX + 20, centerY + 40, paint);

        // Clue 2 - MAC address
        paint.Color = SKColors.White;
        paint.Style = SKPaintStyle.Fill;
        canvas.DrawRoundRect(centerX + 30, centerY + 30, 80, 20, 3, 3, paint);
        paint.Color = TextColor;
        paint.TextSize = 9;
        paint.TextAlign = SKTextAlign.Center;
        canvas.DrawText("AA:BB:CC:DD:EE:FF", centerX + 70, centerY + 43, paint);

        DrawTitleOverlay(canvas, "ARP Network Detective", width, height);
    }

    /// <summary>
    /// Draw IP classes as school classrooms
    /// </summary>
    private void DrawIPClassesSchool(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        using var paint = new SKPaint();

        var classrooms = new[]
        {
            new { Label = "Class A", Range = "1-126", Color = PrimaryColor, Students = "16M hosts" },
            new { Label = "Class B", Range = "128-191", Color = SecondaryColor, Students = "65K hosts" },
            new { Label = "Class C", Range = "192-223", Color = AccentColor, Students = "254 hosts" }
        };

        // Draw school building
        paint.Style = SKPaintStyle.Fill;
        paint.Color = SKColor.Parse("#DEB887"); // Tan building
        canvas.DrawRect(width * 0.1f, height * 0.3f, width * 0.8f, height * 0.5f, paint);

        // School roof
        paint.Color = SKColor.Parse("#8B4513"); // Brown roof
        var roofPath = new SKPath();
        roofPath.MoveTo(width * 0.05f, height * 0.3f);
        roofPath.LineTo(width * 0.5f, height * 0.15f);
        roofPath.LineTo(width * 0.95f, height * 0.3f);
        canvas.DrawPath(roofPath, paint);

        // Draw classrooms
        for (int i = 0; i < 3; i++)
        {
            var classroom = classrooms[i];
            var x = width * (0.2f + i * 0.25f);
            var y = height * 0.5f;

            // Classroom door
            paint.Color = classroom.Color;
            canvas.DrawRect(x - 30, y, 60, 80, paint);

            // Door handle
            paint.Color = SKColors.Gold;
            canvas.DrawCircle(x + 20, y + 40, 3, paint);

            // Class label above door
            paint.Color = TextColor;
            paint.TextSize = 14;
            paint.TextAlign = SKTextAlign.Center;
            canvas.DrawText(classroom.Label, x, y - 10, paint);

            // IP range
            paint.TextSize = 10;
            canvas.DrawText(classroom.Range, x, y + 95, paint);
            canvas.DrawText(classroom.Students, x, y + 110, paint);

            // Windows
            paint.Color = SKColors.LightBlue;
            canvas.DrawRect(x - 20, y + 15, 15, 15, paint);
            canvas.DrawRect(x + 5, y + 15, 15, 15, paint);
        }

        // School sign
        paint.Color = SKColors.White;
        canvas.DrawRoundRect(width * 0.35f, height * 0.85f, width * 0.3f, 30, 5, 5, paint);
        paint.Color = TextColor;
        paint.TextSize = 12;
        paint.TextAlign = SKTextAlign.Center;
        canvas.DrawText("IP Address School", width * 0.5f, height * 0.87f, paint);

        DrawTitleOverlay(canvas, "IP Classes School", width, height);
    }

    /// <summary>
    /// Draw loopback as mirror
    /// </summary>
    private void DrawLoopbackMirror(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        var centerX = width / 2f;
        var centerY = height / 2f;

        using var paint = new SKPaint();

        // Draw mirror frame
        paint.Style = SKPaintStyle.Fill;
        paint.Color = SKColors.Gold;
        canvas.DrawOval(centerX - 80, centerY - 80, 160, 160, paint);

        // Mirror surface
        paint.Color = SKColors.LightGray;
        canvas.DrawOval(centerX - 70, centerY - 70, 140, 140, paint);

        // Reflection effect
        var gradient = SKShader.CreateLinearGradient(
            new SKPoint(centerX - 70, centerY - 70),
            new SKPoint(centerX + 70, centerY + 70),
            new SKColor[] { SKColors.White.WithAlpha(200), SKColors.LightGray.WithAlpha(100) },
            null,
            SKShaderTileMode.Clamp);
        paint.Shader = gradient;
        canvas.DrawOval(centerX - 70, centerY - 70, 140, 140, paint);
        paint.Shader = null;

        // Computer reflected in mirror
        paint.Color = PrimaryColor;
        canvas.DrawRect(centerX - 25, centerY - 30, 50, 35, paint);
        
        // Screen
        paint.Color = SKColors.Black;
        canvas.DrawRect(centerX - 20, centerY - 25, 40, 25, paint);
        
        // Screen content
        paint.Color = AccentColor;
        paint.TextSize = 10;
        paint.TextAlign = SKTextAlign.Center;
        canvas.DrawText("127.0.0.1", centerX, centerY - 10, paint);

        // Keyboard
        paint.Color = SKColors.DarkGray;
        canvas.DrawRect(centerX - 22, centerY + 10, 44, 12, paint);

        // Mirror stand
        paint.Color = SKColors.Gold;
        canvas.DrawRect(centerX - 5, centerY + 80, 10, 40, paint);
        canvas.DrawOval(centerX - 15, centerY + 115, 30, 10, paint);

        // Loopback arrows
        paint.Color = SecondaryColor;
        paint.Style = SKPaintStyle.Stroke;
        paint.StrokeWidth = 3;
        
        // Curved arrow showing loop
        var loopPath = new SKPath();
        loopPath.AddArc(SKRect.Create(centerX - 40, centerY - 40, 80, 80), -90, 270);
        canvas.DrawPath(loopPath, paint);
        
        // Arrow head
        canvas.DrawLine(centerX - 35, centerY - 45, centerX - 40, centerY - 40, paint);
        canvas.DrawLine(centerX - 35, centerY - 35, centerX - 40, centerY - 40, paint);

        DrawTitleOverlay(canvas, "Loopback Mirror: 127.0.0.1", width, height);
    }

    /// <summary>
    /// Draw multicast as group call
    /// </summary>
    private void DrawMulticastGroupCall(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        var centerX = width / 2f;
        var centerY = height / 2f;

        using var paint = new SKPaint();

        // Draw central broadcaster
        paint.Style = SKPaintStyle.Fill;
        paint.Color = SecondaryColor;
        canvas.DrawCircle(centerX, centerY, 30, paint);
        
        paint.Color = SKColors.White;
        paint.TextSize = 24;
        paint.TextAlign = SKTextAlign.Center;
        canvas.DrawText("📺", centerX, centerY + 8, paint);

        // Draw group members in circle
        var memberColors = new[] { PrimaryColor, AccentColor, SKColors.Purple, SKColors.Orange, SKColors.Pink };
        var memberCount = 5;
        var radius = 100f;

        for (int i = 0; i < memberCount; i++)
        {
            var angle = i * Math.PI * 2 / memberCount;
            var x = centerX + Math.Cos(angle) * radius;
            var y = centerY + Math.Sin(angle) * radius;

            // Draw member device
            paint.Color = memberColors[i];
            canvas.DrawCircle((float)x, (float)y, 20, paint);
            
            paint.Color = SKColors.White;
            paint.TextSize = 16;
            paint.TextAlign = SKTextAlign.Center;
            canvas.DrawText("📱", (float)x, (float)y + 5, paint);

            // Draw signal lines
            paint.Color = memberColors[i];
            paint.Style = SKPaintStyle.Stroke;
            paint.StrokeWidth = 3;
            paint.PathEffect = SKPathEffect.CreateDash(new float[] { 5, 5 }, 0);
            canvas.DrawLine(centerX, centerY, (float)x, (float)y, paint);
            paint.PathEffect = null;
            paint.Style = SKPaintStyle.Fill;

            // IP labels
            paint.Color = TextColor;
            paint.TextSize = 10;
            paint.TextAlign = SKTextAlign.Center;
            canvas.DrawText($"224.0.0.1", (float)x, (float)y + 35, paint);
        }

        // Multicast label
        paint.Color = TextColor;
        paint.TextSize = 12;
        paint.TextAlign = SKTextAlign.Center;
        canvas.DrawText("Multicast Stream", centerX, centerY - 50, paint);
        canvas.DrawText("One-to-Many", centerX, centerY - 35, paint);

        DrawTitleOverlay(canvas, "Multicast Group Communication", width, height);
    }

    /// <summary>
    /// Draw anycast as nearest friend
    /// </summary>
    private void DrawAnycastNearestFriend(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        var centerX = width / 2f;
        var centerY = height / 2f;

        using var paint = new SKPaint();

        // Draw user in center
        paint.Style = SKPaintStyle.Fill;
        paint.Color = PrimaryColor;
        canvas.DrawCircle(centerX, centerY, 25, paint);
        
        paint.Color = SKColors.White;
        paint.TextSize = 20;
        paint.TextAlign = SKTextAlign.Center;
        canvas.DrawText("👤", centerX, centerY + 7, paint);

        // Draw multiple servers with same anycast IP
        var serverPositions = new[]
        {
            new { X = centerX - 120, Y = centerY - 80, Distance = "100ms", Closest = false },
            new { X = centerX + 120, Y = centerY - 80, Distance = "50ms", Closest = true },
            new { X = centerX - 120, Y = centerY + 80, Distance = "200ms", Closest = false },
            new { X = centerX + 120, Y = centerY + 80, Distance = "150ms", Closest = false }
        };

        foreach (var server in serverPositions)
        {
            // Draw server
            paint.Color = server.Closest ? AccentColor : SKColors.Gray;
            canvas.DrawRect(server.X - 25, server.Y - 20, 50, 40, paint);
            
            // Server screen
            paint.Color = server.Closest ? SKColors.LightGreen : SKColors.DarkGray;
            canvas.DrawRect(server.X - 20, server.Y - 15, 40, 20, paint);
            
            paint.Color = TextColor;
            paint.TextSize = 10;
            paint.TextAlign = SKTextAlign.Center;
            canvas.DrawText("DNS", server.X, server.Y - 2, paint);
            
            // Same anycast IP
            canvas.DrawText("198.41.0.4", server.X, server.Y + 35, paint);
            
            // Distance label
            paint.Color = server.Closest ? AccentColor : SKColors.Gray;
            canvas.DrawText(server.Distance, server.X, server.Y + 50, paint);

            // Draw connection line (highlight closest)
            paint.Style = SKPaintStyle.Stroke;
            paint.StrokeWidth = server.Closest ? 4 : 2;
            paint.Color = server.Closest ? AccentColor : SKColors.LightGray;
            paint.PathEffect = server.Closest ? null : SKPathEffect.CreateDash(new float[] { 5, 5 }, 0);
            canvas.DrawLine(centerX, centerY, server.X, server.Y, paint);
            paint.PathEffect = null;
            paint.Style = SKPaintStyle.Fill;
        }

        // Routing decision bubble
        paint.Color = SKColors.White;
        canvas.DrawRoundRect(centerX - 40, centerY - 80, 80, 25, 5, 5, paint);
        paint.Color = TextColor;
        paint.TextSize = 10;
        paint.TextAlign = SKTextAlign.Center;
        canvas.DrawText("Route to nearest!", centerX, centerY - 62, paint);

        DrawTitleOverlay(canvas, "Anycast: Nearest Server", width, height);
    }

    /// <summary>
    /// Draw IP security as digital fortress
    /// </summary>
    private void DrawIPSecurityFortress(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        var centerX = width / 2f;
        var centerY = height / 2f;

        using var paint = new SKPaint();

        // Draw fortress walls
        paint.Style = SKPaintStyle.Fill;
        paint.Color = SKColors.Gray;
        canvas.DrawRect(centerX - 100, centerY - 50, 200, 100, paint);

        // Draw castle towers
        paint.Color = SKColors.DarkGray;
        canvas.DrawRect(centerX - 120, centerY - 70, 40, 120, paint);
        canvas.DrawRect(centerX + 80, centerY - 70, 40, 120, paint);

        // Tower tops
        var leftTower = new SKPath();
        leftTower.MoveTo(centerX - 125, centerY - 70);
        leftTower.LineTo(centerX - 100, centerY - 90);
        leftTower.LineTo(centerX - 75, centerY - 70);
        canvas.DrawPath(leftTower, paint);

        var rightTower = new SKPath();
        rightTower.MoveTo(centerX + 75, centerY - 70);
        rightTower.LineTo(centerX + 100, centerY - 90);
        rightTower.LineTo(centerX + 125, centerY - 70);
        canvas.DrawPath(rightTower, paint);

        // Fortress gate
        paint.Color = SKColors.Brown;
        canvas.DrawRoundRect(centerX - 25, centerY + 25, 50, 25, 0, 0, paint);

        // IPsec shields on walls
        paint.Color = AccentColor;
        for (int i = 0; i < 3; i++)
        {
            var shieldX = centerX - 60 + i * 60;
            var shieldPath = new SKPath();
            shieldPath.MoveTo(shieldX, centerY - 30);
            shieldPath.LineTo(shieldX - 15, centerY - 15);
            shieldPath.LineTo(shieldX - 15, centerY + 5);
            shieldPath.LineTo(shieldX, centerY + 15);
            shieldPath.LineTo(shieldX + 15, centerY + 5);
            shieldPath.LineTo(shieldX + 15, centerY - 15);
            shieldPath.Close();
            canvas.DrawPath(shieldPath, paint);

            // Shield symbols
            paint.Color = SKColors.White;
            paint.TextSize = 12;
            paint.TextAlign = SKTextAlign.Center;
            canvas.DrawText("🔒", shieldX, centerY - 5, paint);
        }

        // Security labels
        paint.Color = TextColor;
        paint.TextSize = 10;
        paint.TextAlign = SKTextAlign.Center;
        canvas.DrawText("IPsec", centerX - 60, centerY + 30, paint);
        canvas.DrawText("ESP", centerX, centerY + 30, paint);
        canvas.DrawText("AH", centerX + 60, centerY + 30, paint);

        // Encrypted data packets floating around
        paint.Color = SecondaryColor;
        var packetPositions = new[]
        {
            new { X = centerX - 140, Y = centerY - 30 },
            new { X = centerX + 140, Y = centerY - 30 },
            new { X = centerX - 140, Y = centerY + 30 },
            new { X = centerX + 140, Y = centerY + 30 }
        };

        foreach (var packet in packetPositions)
        {
            canvas.DrawRoundRect(packet.X - 8, packet.Y - 6, 16, 12, 3, 3, paint);
            paint.Color = SKColors.White;
            paint.TextSize = 8;
            paint.TextAlign = SKTextAlign.Center;
            canvas.DrawText("📦", packet.X, packet.Y + 2, paint);
            paint.Color = SecondaryColor;
        }

        DrawTitleOverlay(canvas, "IP Security Fortress", width, height);
    }

    /// <summary>
    /// Draw PowerShell IP commands as magic wand
    /// </summary>
    private void DrawPowerShellIPWand(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        var centerX = width / 2f;
        var centerY = height / 2f;

        using var paint = new SKPaint();

        // Draw magic wand
        paint.Style = SKPaintStyle.Fill;
        paint.Color = SKColors.Brown;
        canvas.DrawRect(centerX - 5, centerY + 20, 10, 80, paint);

        // Wand tip with star
        paint.Color = SKColors.Gold;
        DrawStar(canvas, paint, centerX, centerY - 10, 15, 7);

        // Magic sparkles
        paint.Color = SecondaryColor;
        var sparklePositions = new[]
        {
            new { X = centerX - 30, Y = centerY - 40 },
            new { X = centerX + 25, Y = centerY - 35 },
            new { X = centerX - 20, Y = centerY - 60 },
            new { X = centerX + 35, Y = centerY - 55 }
        };

        foreach (var sparkle in sparklePositions)
        {
            DrawStar(canvas, paint, sparkle.X, sparkle.Y, 5, 2);
        }

        // PowerShell command scroll
        paint.Color = SKColors.White;
        canvas.DrawRoundRect(centerX - 80, centerY - 90, 160, 60, 5, 5, paint);
        
        // Scroll border
        paint.Style = SKPaintStyle.Stroke;
        paint.Color = PrimaryColor;
        paint.StrokeWidth = 2;
        canvas.DrawRoundRect(centerX - 80, centerY - 90, 160, 60, 5, 5, paint);
        
        // PowerShell commands
        paint.Style = SKPaintStyle.Fill;
        paint.Color = TextColor;
        paint.TextSize = 9;
        paint.TextAlign = SKTextAlign.Left;
        canvas.DrawText("Get-NetIPAddress", centerX - 75, centerY - 70, paint);
        canvas.DrawText("New-NetIPAddress -IP", centerX - 75, centerY - 55, paint);
        canvas.DrawText("Set-DnsClientServerAddress", centerX - 75, centerY - 40, paint);

        // IP address transformation
        paint.Color = AccentColor;
        paint.TextAlign = SKTextAlign.Center;
        paint.TextSize = 12;
        canvas.DrawText("192.168.1.100", centerX - 60, centerY + 40, paint);

        // Arrow
        paint.Style = SKPaintStyle.Stroke;
        paint.StrokeWidth = 3;
        canvas.DrawLine(centerX - 30, centerY + 35, centerX + 30, centerY + 35, paint);
        canvas.DrawLine(centerX + 25, centerY + 30, centerX + 30, centerY + 35, paint);
        canvas.DrawLine(centerX + 25, centerY + 40, centerX + 30, centerY + 35, paint);

        paint.Style = SKPaintStyle.Fill;
        canvas.DrawText("10.0.0.50", centerX + 60, centerY + 40, paint);

        DrawTitleOverlay(canvas, "PowerShell IP Magic Wand", width, height);
    }

    /// <summary>
    /// Draw IP troubleshooting as puzzle
    /// </summary>
    private void DrawIPTroubleshootingPuzzle(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        var centerX = width / 2f;
        var centerY = height / 2f;

        using var paint = new SKPaint();

        // Draw puzzle pieces
        var puzzlePieces = new[]
        {
            new { X = centerX - 60, Y = centerY - 40, Label = "ping 127.0.0.1", Color = PrimaryColor },
            new { X = centerX + 60, Y = centerY - 40, Label = "ipconfig /all", Color = SecondaryColor },
            new { X = centerX - 60, Y = centerY + 40, Label = "nslookup", Color = AccentColor },
            new { X = centerX + 60, Y = centerY + 40, Label = "tracert", Color = SKColors.Purple }
        };

        foreach (var piece in puzzlePieces)
        {
            // Draw puzzle piece shape
            paint.Style = SKPaintStyle.Fill;
            paint.Color = piece.Color;
            
            var piecePath = new SKPath();
            piecePath.AddRoundRect(SKRect.Create(piece.X - 40, piece.Y - 20, 80, 40), 5, 5);
            
            // Add puzzle tabs and blanks
            if (piece.X < centerX) // Left pieces - add right tab
            {
                piecePath.AddCircle(piece.X + 40, piece.Y, 8);
            }
            else // Right pieces - add left blank
            {
                var blank = new SKPath();
                blank.AddCircle(piece.X - 40, piece.Y, 8);
                piecePath.Op(blank, SKPathOp.Difference, piecePath);
            }
            
            canvas.DrawPath(piecePath, paint);

            // Add labels
            paint.Color = SKColors.White;
            paint.TextSize = 10;
            paint.TextAlign = SKTextAlign.Center;
            canvas.DrawText(piece.Label, piece.X, piece.Y + 3, paint);
        }

        // Draw magnifying glass over puzzle
        paint.Color = SKColors.Brown;
        paint.Style = SKPaintStyle.Stroke;
        paint.StrokeWidth = 4;
        canvas.DrawLine(centerX + 20, centerY - 80, centerX + 40, centerY - 100, paint);
        
        paint.Color = SKColors.Silver;
        paint.StrokeWidth = 3;
        canvas.DrawCircle(centerX, centerY - 70, 15, paint);

        // Magnifying glass lens
        paint.Color = SKColors.LightBlue.WithAlpha(100);
        paint.Style = SKPaintStyle.Fill;
        canvas.DrawCircle(centerX, centerY - 70, 12, paint);

        // Problem indicators
        paint.Color = SKColors.Red;
        paint.TextSize = 16;
        paint.TextAlign = SKTextAlign.Center;
        canvas.DrawText("❌", centerX - 100, centerY, paint);
        paint.Color = TextColor;
        paint.TextSize = 8;
        canvas.DrawText("169.254.x.x", centerX - 100, centerY + 20, paint);
        canvas.DrawText("APIPA", centerX - 100, centerY + 30, paint);

        DrawTitleOverlay(canvas, "IP Troubleshooting Puzzle", width, height);
    }

    /// <summary>
    /// Draw IPv6 future as connected city
    /// </summary>
    private void DrawIPv6FutureCity(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        using var paint = new SKPaint();

        // Draw city skyline
        var buildingHeights = new[] { 80, 120, 100, 140, 90, 110, 95 };
        var buildingWidth = width / buildingHeights.Length;

        for (int i = 0; i < buildingHeights.Length; i++)
        {
            var x = i * buildingWidth;
            var buildingHeight = buildingHeights[i];
            var y = height * 0.8f - buildingHeight;

            // Building body
            paint.Style = SKPaintStyle.Fill;
            paint.Color = i % 2 == 0 ? PrimaryColor : SecondaryColor;
            canvas.DrawRect(x, y, buildingWidth - 5, buildingHeight, paint);

            // Building windows with IPv6 glow
            paint.Color = AccentColor;
            for (int row = 0; row < buildingHeight / 20; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    var windowX = x + 5 + col * (buildingWidth - 15) / 3;
                    var windowY = y + 10 + row * 20;
                    canvas.DrawRect(windowX, windowY, 8, 8, paint);
                }
            }

            // IPv6 address labels
            paint.Color = TextColor;
            paint.TextSize = 8;
            paint.TextAlign = SKTextAlign.Center;
            canvas.DrawText($"2001:db8::{i+1}00", x + buildingWidth/2, height * 0.85f, paint);
        }

        // Draw connecting network lines between buildings
        paint.Color = SecondaryColor;
        paint.Style = SKPaintStyle.Stroke;
        paint.StrokeWidth = 2;
        paint.PathEffect = SKPathEffect.CreateDash(new float[] { 3, 3 }, 0);

        for (int i = 0; i < buildingHeights.Length - 1; i++)
        {
            var x1 = i * buildingWidth + buildingWidth / 2;
            var x2 = (i + 1) * buildingWidth + buildingWidth / 2;
            var y1 = height * 0.8f - buildingHeights[i] / 2;
            var y2 = height * 0.8f - buildingHeights[i + 1] / 2;
            canvas.DrawLine(x1, y1, x2, y2, paint);
        }
        paint.PathEffect = null;

        // Flying cars with IPv6 addresses
        paint.Style = SKPaintStyle.Fill;
        var carPositions = new[]
        {
            new { X = width * 0.2f, Y = height * 0.3f },
            new { X = width * 0.6f, Y = height * 0.4f },
            new { X = width * 0.8f, Y = height * 0.25f }
        };

        foreach (var car in carPositions)
        {
            paint.Color = AccentColor;
            canvas.DrawOval(car.X - 12, car.Y - 6, 24, 12, paint);
            
            // Car IPv6 address
            paint.Color = TextColor;
            paint.TextSize = 7;
            paint.TextAlign = SKTextAlign.Center;
            canvas.DrawText("2001:db8::car", car.X, car.Y + 20, paint);
        }

        // IoT devices floating around
        paint.TextSize = 12;
        var iotDevices = new[]
        {
            new { X = width * 0.15f, Y = height * 0.5f, Icon = "🏠" },
            new { X = width * 0.85f, Y = height * 0.45f, Icon = "📱" },
            new { X = width * 0.4f, Y = height * 0.2f, Icon = "🚗" },
            new { X = width * 0.7f, Y = height * 0.6f, Icon = "💡" }
        };

        foreach (var device in iotDevices)
        {
            paint.TextAlign = SKTextAlign.Center;
            canvas.DrawText(device.Icon, device.X, device.Y, paint);
        }

        // Sky gradient for futuristic feel
        paint.Style = SKPaintStyle.Fill;
        var skyGradient = SKShader.CreateLinearGradient(
            new SKPoint(0, 0),
            new SKPoint(0, height * 0.8f),
            new SKColor[] { SKColor.Parse("#001122"), SKColor.Parse("#003366") },
            null,
            SKShaderTileMode.Clamp);
        paint.Shader = skyGradient;
        canvas.DrawRect(0, 0, width, height * 0.8f, paint);
        paint.Shader = null;

        DrawTitleOverlay(canvas, "IPv6 Future Connected City", width, height);
    }

    /// <summary>
    /// Draw subnet binary calculator
    /// </summary>
    private void DrawSubnetBinaryCalculator(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        var centerX = width / 2f;
        var centerY = height / 2f;

        using var paint = new SKPaint();

        // Draw calculator body
        paint.Style = SKPaintStyle.Fill;
        paint.Color = SKColors.Black;
        canvas.DrawRoundRect(centerX - 80, centerY - 80, 160, 160, 10, 10, paint);

        // Calculator screen
        paint.Color = SKColors.Green;
        canvas.DrawRect(centerX - 70, centerY - 70, 140, 40, paint);

        // Screen content - binary calculation
        paint.Color = SKColors.Black;
        paint.TextSize = 10;
        paint.TextAlign = SKTextAlign.Left;
        canvas.DrawText("192.168.1.0/26", centerX - 65, centerY - 55, paint);
        canvas.DrawText("11000000.10101000", centerX - 65, centerY - 42, paint);
        canvas.DrawText("2^6 = 64 addresses", centerX - 65, centerY - 30, paint);

        // Calculator buttons
        var buttonPositions = new[]
        {
            // Row 1
            new { X = centerX - 50, Y = centerY - 10, Label = "7" },
            new { X = centerX - 20, Y = centerY - 10, Label = "8" },
            new { X = centerX + 10, Y = centerY - 10, Label = "9" },
            new { X = centerX + 40, Y = centerY - 10, Label = "/" },
            // Row 2
            new { X = centerX - 50, Y = centerY + 15, Label = "4" },
            new { X = centerX - 20, Y = centerY + 15, Label = "5" },
            new { X = centerX + 10, Y = centerY + 15, Label = "6" },
            new { X = centerX + 40, Y = centerY + 15, Label = "*" },
            // Row 3
            new { X = centerX - 50, Y = centerY + 40, Label = "1" },
            new { X = centerX - 20, Y = centerY + 40, Label = "2" },
            new { X = centerX + 10, Y = centerY + 40, Label = "3" },
            new { X = centerX + 40, Y = centerY + 40, Label = "-" },
            // Row 4
            new { X = centerX - 35, Y = centerY + 65, Label = "0" },
            new { X = centerX + 10, Y = centerY + 65, Label = "=" },
            new { X = centerX + 40, Y = centerY + 65, Label = "+" }
        };

        foreach (var button in buttonPositions)
        {
            paint.Color = SKColors.Gray;
            var buttonWidth = button.Label == "0" ? 35f : 20f;
            canvas.DrawRoundRect(button.X - buttonWidth/2, button.Y - 8, buttonWidth, 16, 3, 3, paint);
            
            paint.Color = SKColors.White;
            paint.TextSize = 12;
            paint.TextAlign = SKTextAlign.Center;
            canvas.DrawText(button.Label, button.X, button.Y + 3, paint);
        }

        // Binary conversion example on sides
        paint.Color = TextColor;
        paint.TextSize = 10;
        paint.TextAlign = SKTextAlign.Left;

        // Left side - binary breakdown
        canvas.DrawText("Binary Math:", centerX - 150, centerY - 40, paint);
        canvas.DrawText("/26 = 26 network bits", centerX - 150, centerY - 25, paint);
        canvas.DrawText("32 - 26 = 6 host bits", centerX - 150, centerY - 10, paint);
        canvas.DrawText("2^6 = 64 total IPs", centerX - 150, centerY + 5, paint);
        canvas.DrawText("64 - 2 = 62 hosts", centerX - 150, centerY + 20, paint);

        // Right side - subnet ranges
        paint.TextAlign = SKTextAlign.Left;
        canvas.DrawText("Subnet Ranges:", centerX + 90, centerY - 40, paint);
        canvas.DrawText("192.168.1.0/26", centerX + 90, centerY - 25, paint);
        canvas.DrawText("192.168.1.64/26", centerX + 90, centerY - 10, paint);
        canvas.DrawText("192.168.1.128/26", centerX + 90, centerY + 5, paint);
        canvas.DrawText("192.168.1.192/26", centerX + 90, centerY + 20, paint);

        DrawTitleOverlay(canvas, "Subnet Binary Calculator", width, height);
    }

    /// <summary>
    /// Draw IP mastery medal
    /// </summary>
    private void DrawIPMasteryMedal(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        var centerX = width / 2f;
        var centerY = height / 2f;

        using var paint = new SKPaint();

        // Draw medal ribbon
        paint.Style = SKPaintStyle.Fill;
        paint.Color = SecondaryColor;
        canvas.DrawRect(centerX - 30, centerY - 120, 60, 80, paint);

        // Ribbon stripes
        paint.Color = PrimaryColor;
        for (int i = 0; i < 4; i++)
        {
            canvas.DrawRect(centerX - 30, centerY - 120 + i * 20, 60, 5, paint);
        }

        // Medal base
        paint.Color = SKColors.Gold;
        canvas.DrawCircle(centerX, centerY, 50, paint);

        // Medal border
        paint.Style = SKPaintStyle.Stroke;
        paint.StrokeWidth = 4;
        paint.Color = SKColors.DarkGoldenrod;
        canvas.DrawCircle(centerX, centerY, 50, paint);
        canvas.DrawCircle(centerX, centerY, 40, paint);

        // Inner medal design
        paint.Style = SKPaintStyle.Fill;
        paint.Color = SKColors.White;
        canvas.DrawCircle(centerX, centerY, 35, paint);

        // IP address pattern in center
        paint.Color = PrimaryColor;
        paint.TextSize = 14;
        paint.TextAlign = SKTextAlign.Center;
        canvas.DrawText("IP", centerX, centerY - 8, paint);
        canvas.DrawText("MASTER", centerX, centerY + 8, paint);

        // Small IP addresses around the border
        paint.TextSize = 8;
        var ipPositions = new[]
        {
            new { Angle = 0, IP = "192.168.1.1" },
            new { Angle = Math.PI / 2, IP = "10.0.0.1" },
            new { Angle = Math.PI, IP = "172.16.0.1" },
            new { Angle = 3 * Math.PI / 2, IP = "127.0.0.1" }
        };

        foreach (var ip in ipPositions)
        {
            var x = centerX + Math.Cos(ip.Angle) * 30;
            var y = centerY + Math.Sin(ip.Angle) * 30;
            canvas.DrawText(ip.IP, (float)x, (float)y, paint);
        }

        // Achievement sparkles around medal
        paint.Color = AccentColor;
        var sparkleCount = 12;
        for (int i = 0; i < sparkleCount; i++)
        {
            var angle = i * Math.PI * 2 / sparkleCount;
            var distance = 70 + (i % 3) * 10;
            var x = centerX + Math.Cos(angle) * distance;
            var y = centerY + Math.Sin(angle) * distance;
            DrawStar(canvas, paint, (float)x, (float)y, 4, 2);
        }

        // Medal inscription at bottom
        paint.Color = TextColor;
        paint.TextSize = 12;
        paint.TextAlign = SKTextAlign.Center;
        canvas.DrawText("IP ADDRESS MASTERY", centerX, centerY + 80, paint);
        canvas.DrawText("Network Engineering Excellence", centerX, centerY + 100, paint);

        DrawTitleOverlay(canvas, "IP Mastery Achievement Medal", width, height);
    }

    #endregion

    #region Module 4: Scripting Sorcery Drawing Methods

    /// <summary>
    /// Draw wizard with wand pointing at computer - representing script magic
    /// </summary>
    private void DrawScriptWizardComputer(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        var centerX = width / 2f;
        var centerY = height / 2f;

        using var paint = new SKPaint();

        // Draw wizard figure
        paint.Style = SKPaintStyle.Fill;
        paint.Color = PrimaryColor;
        
        // Wizard robe
        var robeWidth = 60f;
        var robeHeight = 120f;
        canvas.DrawRoundRect(centerX - 100 - robeWidth/2, centerY - robeHeight/2, robeWidth, robeHeight, 10, 10, paint);
        
        // Wizard hat
        paint.Color = SecondaryColor;
        var hatPath = new SKPath();
        hatPath.MoveTo(centerX - 100 - 20, centerY - robeHeight/2);
        hatPath.LineTo(centerX - 100, centerY - robeHeight/2 - 40);
        hatPath.LineTo(centerX - 100 + 20, centerY - robeHeight/2);
        canvas.DrawPath(hatPath, paint);
        
        // Wizard face
        paint.Color = SKColors.PeachPuff;
        canvas.DrawCircle(centerX - 100, centerY - 30, 15, paint);
        
        // Beard
        paint.Color = SKColors.White;
        canvas.DrawCircle(centerX - 100, centerY - 10, 12, paint);
        
        // Magic wand with sparkles
        paint.Style = SKPaintStyle.Stroke;
        paint.Color = SKColors.Brown;
        paint.StrokeWidth = 3;
        canvas.DrawLine(centerX - 80, centerY, centerX + 20, centerY - 20, paint);
        
        // Wand tip sparkles
        paint.Style = SKPaintStyle.Fill;
        paint.Color = AccentColor;
        for (int i = 0; i < 5; i++)
        {
            var sparkleX = centerX + 20 + (i - 2) * 8;
            var sparkleY = centerY - 20 + (i % 2 == 0 ? -5 : 5);
            DrawStar(canvas, paint, sparkleX, sparkleY, 3, 2);
        }
        
        // Computer monitor
        paint.Style = SKPaintStyle.Fill;
        paint.Color = TextColor;
        canvas.DrawRoundRect(centerX + 40, centerY - 50, 100, 70, 5, 5, paint);
        
        // Computer screen
        paint.Color = SKColors.Black;
        canvas.DrawRect(centerX + 45, centerY - 45, 90, 55, paint);
        
        // Code on screen
        paint.Color = AccentColor;
        paint.TextSize = 8;
        paint.TextAlign = SKTextAlign.Left;
        canvas.DrawText("$ip = '192.168.1.1'", centerX + 50, centerY - 30, paint);
        canvas.DrawText("Test-Connection $ip", centerX + 50, centerY - 20, paint);
        canvas.DrawText("Get-NetAdapter", centerX + 50, centerY - 10, paint);
        
        // Magic energy flowing from wand to computer
        paint.Color = SKColors.Gold;
        paint.StrokeWidth = 2;
        paint.Style = SKPaintStyle.Stroke;
        var magicPath = new SKPath();
        magicPath.MoveTo(centerX + 20, centerY - 20);
        magicPath.CubicTo(centerX + 25, centerY - 30, centerX + 35, centerY - 40, centerX + 40, centerY - 25);
        canvas.DrawPath(magicPath, paint);

        DrawTitleOverlay(canvas, "Script Wizard Casting PowerShell Magic", width, height);
    }

    /// <summary>
    /// Draw colorful potion bottles representing variables
    /// </summary>
    private void DrawVariablePotionBottles(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        var centerX = width / 2f;
        var centerY = height / 2f;

        using var paint = new SKPaint();
        paint.Style = SKPaintStyle.Fill;

        // Draw shelf
        paint.Color = SKColors.Brown;
        canvas.DrawRect(centerX - 150, centerY + 40, 300, 15, paint);

        // Bottle colors and labels
        var bottles = new[]
        {
            new { X = centerX - 100, Color = PrimaryColor, Label = "$ipAddress", Content = "192.168.1.1" },
            new { X = centerX - 30, Color = SecondaryColor, Label = "$computerName", Content = "SERVER01" },
            new { X = centerX + 40, Color = AccentColor, Label = "$userCount", Content = "42" },
            new { X = centerX + 110, Color = SKColors.Purple, Label = "$isOnline", Content = "True" }
        };

        foreach (var bottle in bottles)
        {
            // Bottle body
            paint.Color = bottle.Color;
            canvas.DrawRoundRect(bottle.X - 15, centerY - 30, 30, 60, 5, 5, paint);
            
            // Bottle neck
            paint.Color = bottle.Color;
            canvas.DrawRect(bottle.X - 5, centerY - 45, 10, 15, paint);
            
            // Cork
            paint.Color = SKColors.Brown;
            canvas.DrawRoundRect(bottle.X - 6, centerY - 50, 12, 8, 2, 2, paint);
            
            // Glowing effect inside bottle
            paint.Color = SKColors.White;
            paint.Color = paint.Color.WithAlpha(100);
            canvas.DrawOval(bottle.X - 10, centerY - 20, 20, 40, paint);
            
            // Variable label below bottle
            paint.Color = TextColor;
            paint.TextSize = 10;
            paint.TextAlign = SKTextAlign.Center;
            canvas.DrawText(bottle.Label, bottle.X, centerY + 60, paint);
            
            // Content preview in bottle
            paint.Color = SKColors.White;
            paint.TextSize = 8;
            paint.TextAlign = SKTextAlign.Center;
            canvas.DrawText(bottle.Content, bottle.X, centerY, paint);
        }

        // Magical sparkles around bottles
        paint.Color = SKColors.Gold;
        var random = new Random(42); // Fixed seed for consistent sparkles
        for (int i = 0; i < 15; i++)
        {
            var sparkleX = centerX - 130 + random.Next(260);
            var sparkleY = centerY - 60 + random.Next(80);
            DrawStar(canvas, paint, sparkleX, sparkleY, 2, 1);
        }

        // Title banner
        paint.Color = SKColors.Purple;
        paint.Color = paint.Color.WithAlpha(150);
        canvas.DrawRoundRect(centerX - 80, centerY - 80, 160, 20, 10, 10, paint);
        
        paint.Color = SKColors.White;
        paint.TextSize = 12;
        paint.TextAlign = SKTextAlign.Center;
        canvas.DrawText("Variable Potion Collection", centerX, centerY - 65, paint);

        DrawTitleOverlay(canvas, "Magical Variable Storage", width, height);
    }

    /// <summary>
    /// Draw ancient spell book with glowing PowerShell commands
    /// </summary>
    private void DrawAncientSpellBook(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        var centerX = width / 2f;
        var centerY = height / 2f;

        using var paint = new SKPaint();

        // Book cover
        paint.Style = SKPaintStyle.Fill;
        paint.Color = SKColors.DarkRed;
        canvas.DrawRoundRect(centerX - 80, centerY - 60, 160, 120, 8, 8, paint);
        
        // Book binding
        paint.Color = SKColors.Maroon;
        canvas.DrawRect(centerX - 85, centerY - 60, 10, 120, paint);
        
        // Ornate decorations on cover
        paint.Color = SKColors.Gold;
        paint.Style = SKPaintStyle.Stroke;
        paint.StrokeWidth = 2;
        canvas.DrawRoundRect(centerX - 70, centerY - 50, 140, 100, 5, 5, paint);
        canvas.DrawRoundRect(centerX - 60, centerY - 40, 120, 80, 5, 5, paint);
        
        // Mystical symbol on cover
        paint.Style = SKPaintStyle.Fill;
        paint.Color = SKColors.Gold;
        var symbolSize = 30;
        canvas.DrawCircle(centerX, centerY, symbolSize, paint);
        
        paint.Color = SKColors.DarkRed;
        paint.TextSize = 20;
        paint.TextAlign = SKTextAlign.Center;
        canvas.DrawText("PS", centerX, centerY + 7, paint);
        
        // Open pages on the right
        paint.Style = SKPaintStyle.Fill;
        paint.Color = SKColors.AntiqueWhite;
        canvas.DrawRoundRect(centerX + 10, centerY - 50, 120, 100, 3, 3, paint);
        
        // Page content - PowerShell commands
        paint.Color = PrimaryColor;
        paint.TextSize = 10;
        paint.TextAlign = SKTextAlign.Left;
        var commands = new[]
        {
            "Get-Command",
            "Get-Process",
            "Test-Connection", 
            "Set-Location",
            "Get-ChildItem",
            "Where-Object",
            "ForEach-Object",
            "Write-Output"
        };
        
        for (int i = 0; i < commands.Length; i++)
        {
            var y = centerY - 35 + i * 12;
            canvas.DrawText(commands[i], centerX + 15, y, paint);
            
            // Glowing effect on text
            paint.Color = AccentColor;
            paint.Color = paint.Color.WithAlpha(100);
            canvas.DrawText(commands[i], centerX + 15, y, paint);
            paint.Color = PrimaryColor;
        }
        
        // Magical glow around book
        paint.Style = SKPaintStyle.Stroke;
        paint.Color = SKColors.Cyan;
        paint.Color = paint.Color.WithAlpha(150);
        paint.StrokeWidth = 3;
        for (int i = 0; i < 3; i++)
        {
            canvas.DrawRoundRect(centerX - 85 - i*3, centerY - 65 - i*3, 170 + i*6, 130 + i*6, 10 + i, 10 + i, paint);
        }

        DrawTitleOverlay(canvas, "Ancient PowerShell Grimoire", width, height);
    }

    /// <summary>
    /// Draw building blocks labeled with data types
    /// </summary>
    private void DrawLabeledBuildingBlocks(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        var centerX = width / 2f;
        var centerY = height / 2f;

        using var paint = new SKPaint();
        paint.Style = SKPaintStyle.Fill;

        // Block configurations
        var blocks = new[]
        {
            new { X = centerX - 120, Y = centerY + 20, Color = PrimaryColor, Label = "String", Example = "\"Hello\"" },
            new { X = centerX - 60, Y = centerY + 20, Color = SecondaryColor, Label = "Integer", Example = "42" },
            new { X = centerX, Y = centerY + 20, Color = AccentColor, Label = "Boolean", Example = "True" },
            new { X = centerX + 60, Y = centerY + 20, Color = SKColors.Purple, Label = "Array", Example = "@(1,2,3)" },
            new { X = centerX - 90, Y = centerY - 40, Color = SKColors.Orange, Label = "DateTime", Example = "Now" },
            new { X = centerX - 30, Y = centerY - 40, Color = SKColors.Teal, Label = "Hashtable", Example = "@{}" },
            new { X = centerX + 30, Y = centerY - 40, Color = SKColors.Pink, Label = "Double", Example = "3.14" }
        };

        foreach (var block in blocks)
        {
            // Draw 3D block effect
            paint.Color = block.Color;
            canvas.DrawRoundRect(block.X - 25, block.Y - 25, 50, 50, 5, 5, paint);
            
            // 3D shadow
            paint.Color = block.Color;
            paint.Color = paint.Color.WithAlpha(150);
            canvas.DrawRoundRect(block.X - 22, block.Y - 22, 50, 50, 5, 5, paint);
            
            // Block label
            paint.Color = SKColors.White;
            paint.TextSize = 10;
            paint.TextAlign = SKTextAlign.Center;
            canvas.DrawText(block.Label, block.X, block.Y - 5, paint);
            
            // Example value
            paint.TextSize = 8;
            canvas.DrawText(block.Example, block.X, block.Y + 8, paint);
        }

        // Connection lines showing relationships
        paint.Style = SKPaintStyle.Stroke;
        paint.Color = LightTextColor;
        paint.StrokeWidth = 2;
        
        // Draw some connecting lines between related blocks
        canvas.DrawLine(centerX - 95, centerY + 20, centerX - 35, centerY + 20, paint); // String to Integer
        canvas.DrawLine(centerX - 5, centerY + 20, centerX + 35, centerY + 20, paint); // Boolean to Array
        canvas.DrawLine(centerX - 65, centerY - 15, centerX - 65, centerY - 5, paint); // DateTime down
        
        // Title banner
        paint.Style = SKPaintStyle.Fill;
        paint.Color = TextColor;
        paint.Color = paint.Color.WithAlpha(200);
        canvas.DrawRoundRect(centerX - 100, centerY - 80, 200, 25, 10, 10, paint);
        
        paint.Color = SKColors.White;
        paint.TextSize = 14;
        paint.TextAlign = SKTextAlign.Center;
        canvas.DrawText("PowerShell Data Type Building Blocks", centerX, centerY - 60, paint);

        DrawTitleOverlay(canvas, "Data Type Building Blocks", width, height);
    }

    /// <summary>
    /// Draw forked enchanted forest path representing if-then logic
    /// </summary>
    private void DrawForkedEnchantedPath(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        var centerX = width / 2f;
        var centerY = height / 2f;

        using var paint = new SKPaint();

        // Draw mystical background
        var gradient = SKShader.CreateLinearGradient(
            new SKPoint(0, 0),
            new SKPoint(0, height),
            new[] { SKColors.DarkBlue.WithAlpha(50), SKColors.Purple.WithAlpha(30) },
            SKShaderTileMode.Clamp);
        paint.Shader = gradient;
        paint.Style = SKPaintStyle.Fill;
        canvas.DrawRect(0, 0, width, height, paint);
        paint.Shader = null;

        // Main path (before condition)
        paint.Color = SKColors.SandyBrown;
        paint.Style = SKPaintStyle.Fill;
        canvas.DrawRoundRect(centerX - 15, centerY + 50, 30, 100, 15, 15, paint);

        // Decision diamond (if condition)
        paint.Color = AccentColor;
        var conditionPath = new SKPath();
        conditionPath.MoveTo(centerX, centerY - 20);
        conditionPath.LineTo(centerX + 30, centerY);
        conditionPath.LineTo(centerX, centerY + 20);
        conditionPath.LineTo(centerX - 30, centerY);
        conditionPath.Close();
        canvas.DrawPath(conditionPath, paint);

        // Condition text
        paint.Color = SKColors.White;
        paint.TextSize = 10;
        paint.TextAlign = SKTextAlign.Center;
        canvas.DrawText("If", centerX, centerY - 2, paint);
        canvas.DrawText("$ip", centerX, centerY + 10, paint);

        // True path (left fork)
        paint.Color = AccentColor;
        paint.Style = SKPaintStyle.Fill;
        var truePath = new SKPath();
        truePath.MoveTo(centerX - 30, centerY);
        truePath.CubicTo(centerX - 50, centerY - 20, centerX - 70, centerY - 30, centerX - 90, centerY - 40);
        truePath.LineTo(centerX - 85, centerY - 35);
        truePath.CubicTo(centerX - 65, centerY - 25, centerX - 45, centerY - 15, centerX - 25, centerY + 5);
        truePath.Close();
        canvas.DrawPath(truePath, paint);

        // False path (right fork)
        paint.Color = SecondaryColor;
        var falsePath = new SKPath();
        falsePath.MoveTo(centerX + 30, centerY);
        falsePath.CubicTo(centerX + 50, centerY - 20, centerX + 70, centerY - 30, centerX + 90, centerY - 40);
        falsePath.LineTo(centerX + 85, centerY - 35);
        falsePath.CubicTo(centerX + 65, centerY - 25, centerX + 45, centerY - 15, centerX + 25, centerY + 5);
        falsePath.Close();
        canvas.DrawPath(falsePath, paint);

        // Path labels
        paint.Color = SKColors.White;
        paint.TextSize = 12;
        paint.TextAlign = SKTextAlign.Center;
        canvas.DrawText("True", centerX - 60, centerY - 25, paint);
        canvas.DrawText("Set-Static", centerX - 60, centerY - 15, paint);
        canvas.DrawText("False", centerX + 60, centerY - 25, paint);
        canvas.DrawText("Continue", centerX + 60, centerY - 15, paint);

        // Magical glowing trees along paths
        paint.Color = SKColors.Green;
        paint.Color = paint.Color.WithAlpha(180);
        var treePositions = new[]
        {
            new { X = centerX - 120, Y = centerY - 60 },
            new { X = centerX - 40, Y = centerY - 80 },
            new { X = centerX + 40, Y = centerY - 80 },
            new { X = centerX + 120, Y = centerY - 60 }
        };

        foreach (var tree in treePositions)
        {
            // Tree trunk
            paint.Color = SKColors.Brown;
            canvas.DrawRect(tree.X - 3, tree.Y, 6, 20, paint);
            
            // Tree canopy
            paint.Color = AccentColor;
            paint.Color = paint.Color.WithAlpha(150);
            canvas.DrawCircle(tree.X, tree.Y - 5, 12, paint);
            
            // Magical glow
            paint.Color = SKColors.Cyan;
            paint.Color = paint.Color.WithAlpha(100);
            canvas.DrawCircle(tree.X, tree.Y - 5, 15, paint);
        }

        DrawTitleOverlay(canvas, "Conditional Path - If-Then Magic", width, height);
    }

    /// <summary>
    /// Draw magical looping road with spiral patterns
    /// </summary>
    private void DrawMagicalLoopingRoad(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        var centerX = width / 2f;
        var centerY = height / 2f;

        using var paint = new SKPaint();

        // Draw spiral road path
        paint.Style = SKPaintStyle.Stroke;
        paint.Color = SKColors.Gray;
        paint.StrokeWidth = 20;

        var spiralPath = new SKPath();
        var radius = 80f;
        var turns = 3f;
        var angleStep = 0.1f;
        
        var startAngle = 0f;
        var x = centerX + radius * Math.Cos(startAngle);
        var y = centerY + radius * Math.Sin(startAngle);
        spiralPath.MoveTo((float)x, (float)y);

        for (float angle = angleStep; angle <= turns * Math.PI * 2; angle += angleStep)
        {
            var currentRadius = radius * (1 - angle / (turns * Math.PI * 2) * 0.7f);
            x = centerX + currentRadius * Math.Cos(angle);
            y = centerY + currentRadius * Math.Sin(angle);
            spiralPath.LineTo((float)x, (float)y);
        }

        canvas.DrawPath(spiralPath, paint);

        // Road markings
        paint.Color = SKColors.Yellow;
        paint.StrokeWidth = 2;
        // Draw dashed center line along the spiral (simplified)
        var dashEffect = SKPathEffect.CreateDash(new float[] { 10, 10 }, 0);
        paint.PathEffect = dashEffect;
        canvas.DrawPath(spiralPath, paint);
        paint.PathEffect = null;

        // Loop counter at center
        paint.Style = SKPaintStyle.Fill;
        paint.Color = PrimaryColor;
        canvas.DrawCircle(centerX, centerY, 25, paint);
        
        paint.Color = SKColors.White;
        paint.TextSize = 16;
        paint.TextAlign = SKTextAlign.Center;
        canvas.DrawText("i++", centerX, centerY + 5, paint);

        // Magic sparkles along the road
        paint.Color = AccentColor;
        for (int i = 0; i < 20; i++)
        {
            var angle = i * Math.PI * 2 / 20;
            var sparkleRadius = radius * 0.8f;
            var sparkleX = centerX + sparkleRadius * Math.Cos(angle);
            var sparkleY = centerY + sparkleRadius * Math.Sin(angle);
            DrawStar(canvas, paint, (float)sparkleX, (float)sparkleY, 3, 1);
        }

        // Loop types around the spiral
        paint.Color = TextColor;
        paint.TextSize = 12;
        paint.TextAlign = SKTextAlign.Center;
        canvas.DrawText("ForEach", centerX + 60, centerY - 60, paint);
        canvas.DrawText("While", centerX - 60, centerY - 60, paint);
        canvas.DrawText("For", centerX - 60, centerY + 60, paint);
        canvas.DrawText("Do-While", centerX + 60, centerY + 60, paint);

        // Direction arrows showing flow
        paint.Color = SecondaryColor;
        paint.Style = SKPaintStyle.Fill;
        var arrowPositions = new[]
        {
            new { X = centerX + 50, Y = centerY - 30, Angle = 0.5f },
            new { X = centerX, Y = centerY - 50, Angle = 1.5f },
            new { X = centerX - 50, Y = centerY - 30, Angle = 2.5f },
            new { X = centerX - 50, Y = centerY + 30, Angle = 3.5f }
        };

        foreach (var arrow in arrowPositions)
        {
            var arrowPath = new SKPath();
            var tipX = arrow.X + 8 * Math.Cos(arrow.Angle);
            var tipY = arrow.Y + 8 * Math.Sin(arrow.Angle);
            var baseX1 = arrow.X + 5 * Math.Cos(arrow.Angle + 2.5f);
            var baseY1 = arrow.Y + 5 * Math.Sin(arrow.Angle + 2.5f);
            var baseX2 = arrow.X + 5 * Math.Cos(arrow.Angle - 2.5f);
            var baseY2 = arrow.Y + 5 * Math.Sin(arrow.Angle - 2.5f);
            
            arrowPath.MoveTo((float)tipX, (float)tipY);
            arrowPath.LineTo((float)baseX1, (float)baseY1);
            arrowPath.LineTo((float)baseX2, (float)baseY2);
            arrowPath.Close();
            canvas.DrawPath(arrowPath, paint);
        }

        DrawTitleOverlay(canvas, "Magical Loop Highway", width, height);
    }

    // Additional Module 4 drawing methods would continue here...
    // For brevity, I'll create simplified versions of the remaining methods

    private void DrawSpellScrollCollection(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        DrawGenericScriptingConcept(canvas, width, height, "Function Spell Scrolls", "📜✨");
    }

    private void DrawNetCatchingBugs(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        DrawGenericScriptingConcept(canvas, width, height, "Error Catching Net", "🕸️🐛");
    }

    private void DrawCastleWithCodeRunes(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        DrawGenericScriptingConcept(canvas, width, height, "Network Script Castle", "🏰💻");
    }

    private void DrawEnchantedMailboxGlowing(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        DrawGenericScriptingConcept(canvas, width, height, "Exchange Mailbox Magic", "📮✨");
    }

    private void DrawProtectiveMagicalRunes(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        DrawGenericScriptingConcept(canvas, width, height, "Security Script Runes", "🛡️🔮");
    }

    private void DrawRobotAssistantMagical(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        DrawGenericScriptingConcept(canvas, width, height, "Automation Assistant", "🤖✨");
    }

    private void DrawAdjustableMagicWand(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        DrawGenericScriptingConcept(canvas, width, height, "Parameter Wand", "🪄⚙️");
    }

    private void DrawMagicalLibraryShelves(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        DrawGenericScriptingConcept(canvas, width, height, "PowerShell Module Library", "📚✨");
    }

    private void DrawMagnifyingGlassCode(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        DrawGenericScriptingConcept(canvas, width, height, "Debug Detective Glass", "🔍💻");
    }

    private void DrawInfiniteLoopSymbol(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        DrawGenericScriptingConcept(canvas, width, height, "Advanced Loop Mastery", "♾️🔄");
    }

    private void DrawGlowingMagicalObjects(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        DrawGenericScriptingConcept(canvas, width, height, "PowerShell Objects", "🔮📊");
    }

    private void DrawMagicalTelescopeView(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        DrawGenericScriptingConcept(canvas, width, height, "Remote Script Telescope", "🔭✨");
    }

    private void DrawAdvancedGrimoireOpen(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        DrawGenericScriptingConcept(canvas, width, height, "Advanced Script Grimoire", "📖🌟");
    }

    private void DrawMagicalCrownScripts(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        DrawGenericScriptingConcept(canvas, width, height, "Script Sorcerer Crown", "👑💻");
    }

    /// <summary>
    /// Generic scripting concept drawing helper
    /// </summary>
    private void DrawGenericScriptingConcept(SKCanvas canvas, int width, int height, string conceptName, string emoji)
    {
        var centerX = width / 2f;
        var centerY = height / 2f;

        using var paint = new SKPaint();

        // Background gradient
        var gradient = SKShader.CreateRadialGradient(
            new SKPoint(centerX, centerY),
            Math.Min(width, height) / 3f,
            new[] { PrimaryColor.WithAlpha(30), AccentColor.WithAlpha(10) },
            SKShaderTileMode.Clamp);
        paint.Shader = gradient;
        paint.Style = SKPaintStyle.Fill;
        canvas.DrawRect(0, 0, width, height, paint);
        paint.Shader = null;

        // Central concept circle
        paint.Color = PrimaryColor;
        paint.Style = SKPaintStyle.Fill;
        canvas.DrawCircle(centerX, centerY, 60, paint);

        // Concept emoji/symbol
        paint.Color = SKColors.White;
        paint.TextSize = 48;
        paint.TextAlign = SKTextAlign.Center;
        canvas.DrawText(emoji, centerX, centerY + 15, paint);

        // Concept name
        paint.Color = TextColor;
        paint.TextSize = 14;
        paint.TextAlign = SKTextAlign.Center;
        canvas.DrawText(conceptName, centerX, centerY + 90, paint);

        // Magic sparkles
        paint.Color = AccentColor;
        for (int i = 0; i < 8; i++)
        {
            var angle = i * Math.PI * 2 / 8;
            var sparkleX = centerX + 80 * Math.Cos(angle);
            var sparkleY = centerY + 80 * Math.Sin(angle);
            DrawStar(canvas, paint, (float)sparkleX, (float)sparkleY, 4, 2);
        }

        DrawTitleOverlay(canvas, conceptName, width, height);
    }

    #endregion

    #region Module 5: Routing Riddles Drawing Methods

    /// <summary>
    /// Draw treasure map with maze paths representing routing challenges
    /// </summary>
    private void DrawTreasureMapMazePaths(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        var centerX = width / 2f;
        var centerY = height / 2f;

        using var paint = new SKPaint();

        // Draw parchment background
        var gradient = SKShader.CreateLinearGradient(
            new SKPoint(0, 0),
            new SKPoint(width, height),
            new[] { SKColors.AntiqueWhite, SKColors.Wheat },
            SKShaderTileMode.Clamp);
        paint.Shader = gradient;
        paint.Style = SKPaintStyle.Fill;
        canvas.DrawRect(0, 0, width, height, paint);
        paint.Shader = null;

        // Draw maze-like network paths
        paint.Style = SKPaintStyle.Stroke;
        paint.Color = PrimaryColor;
        paint.StrokeWidth = 4;

        // Create multiple paths through the maze
        var paths = new[]
        {
            new { Start = new SKPoint(50, 100), Mid = new SKPoint(200, 150), End = new SKPoint(350, 120), Label = "Path A" },
            new { Start = new SKPoint(50, 200), Mid = new SKPoint(150, 300), End = new SKPoint(350, 250), Label = "Path B" },
            new { Start = new SKPoint(50, 300), Mid = new SKPoint(250, 200), End = new SKPoint(350, 380), Label = "Path C" }
        };

        foreach (var route in paths)
        {
            // Draw curved path
            var path = new SKPath();
            path.MoveTo(route.Start);
            path.QuadTo(route.Mid, route.End);
            canvas.DrawPath(path, paint);

            // Add route markers
            paint.Style = SKPaintStyle.Fill;
            paint.Color = AccentColor;
            canvas.DrawCircle(route.Start, 8, paint);
            canvas.DrawCircle(route.End, 8, paint);

            // Path labels
            paint.Color = TextColor;
            paint.TextSize = 12;
            paint.TextAlign = SKTextAlign.Center;
            canvas.DrawText(route.Label, route.Mid.X, route.Mid.Y - 10, paint);

            paint.Style = SKPaintStyle.Stroke;
            paint.Color = PrimaryColor;
        }

        // Draw treasure chest at destination
        paint.Style = SKPaintStyle.Fill;
        paint.Color = SKColors.Gold;
        canvas.DrawRoundRect(centerX + 120, centerY - 80, 60, 40, 5, 5, paint);
        
        // Chest lock
        paint.Color = SKColors.Brown;
        canvas.DrawCircle(centerX + 150, centerY - 60, 8, paint);

        // Network nodes (routers)
        paint.Color = SecondaryColor;
        var routers = new[]
        {
            new SKPoint(centerX - 100, centerY - 50),
            new SKPoint(centerX, centerY + 20),
            new SKPoint(centerX + 80, centerY - 30)
        };

        foreach (var router in routers)
        {
            canvas.DrawCircle(router, 15, paint);
            paint.Color = SKColors.White;
            paint.TextSize = 10;
            paint.TextAlign = SKTextAlign.Center;
            canvas.DrawText("R", router.X, router.Y + 3, paint);
            paint.Color = SecondaryColor;
        }

        // Add compass rose
        paint.Color = AccentColor;
        DrawCompassRose(canvas, paint, 100, 100, 30);

        // Title at top
        paint.Color = TextColor;
        paint.TextSize = 16;
        paint.TextAlign = SKTextAlign.Center;
        canvas.DrawText("Network Routing Treasure Map", centerX, 40, paint);

        DrawTitleOverlay(canvas, "The Great Routing Riddle Adventure", width, height);
    }

    /// <summary>
    /// Draw straight golden road representing static routes
    /// </summary>
    private void DrawStraightGoldenRoad(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        var centerX = width / 2f;
        var centerY = height / 2f;

        using var paint = new SKPaint();

        // Sky background gradient
        var skyGradient = SKShader.CreateLinearGradient(
            new SKPoint(0, 0),
            new SKPoint(0, height),
            new[] { SKColors.LightBlue, SKColors.LightGreen },
            SKShaderTileMode.Clamp);
        paint.Shader = skyGradient;
        paint.Style = SKPaintStyle.Fill;
        canvas.DrawRect(0, 0, width, height, paint);
        paint.Shader = null;

        // Golden road
        var roadGradient = SKShader.CreateLinearGradient(
            new SKPoint(0, centerY - 50),
            new SKPoint(0, centerY + 50),
            new[] { SKColors.Gold, SKColors.DarkGoldenrod },
            SKShaderTileMode.Clamp);
        paint.Shader = roadGradient;
        
        // Draw perspective road
        var roadPath = new SKPath();
        roadPath.MoveTo(100, centerY + 50);  // Bottom left
        roadPath.LineTo(width - 100, centerY + 30);  // Bottom right
        roadPath.LineTo(width - 80, centerY - 30);   // Top right
        roadPath.LineTo(120, centerY - 50);  // Top left
        roadPath.Close();
        canvas.DrawPath(roadPath, paint);
        paint.Shader = null;

        // Road center line
        paint.Style = SKPaintStyle.Stroke;
        paint.Color = SKColors.White;
        paint.StrokeWidth = 3;
        var dashEffect = SKPathEffect.CreateDash(new float[] { 15, 10 }, 0);
        paint.PathEffect = dashEffect;
        canvas.DrawLine(110, centerY, width - 90, centerY, paint);
        paint.PathEffect = null;

        // Destination city
        paint.Style = SKPaintStyle.Fill;
        paint.Color = PrimaryColor;
        
        // Buildings in the distance
        var buildings = new[]
        {
            new { X = width - 120, Y = centerY - 60, W = 20, H = 40 },
            new { X = width - 95, Y = centerY - 80, W = 25, H = 60 },
            new { X = width - 65, Y = centerY - 50, W = 18, H = 30 }
        };

        foreach (var building in buildings)
        {
            canvas.DrawRect(building.X, building.Y, building.W, building.H, paint);
            
            // Building windows
            paint.Color = SKColors.Yellow;
            for (int i = 0; i < 3; i++)
            {
                canvas.DrawRect(building.X + 3, building.Y + 5 + i * 8, 3, 3, paint);
                canvas.DrawRect(building.X + building.W - 6, building.Y + 5 + i * 8, 3, 3, paint);
            }
            paint.Color = PrimaryColor;
        }

        // Static route sign
        paint.Color = SKColors.Brown;
        canvas.DrawRect(200, centerY - 100, 120, 60, paint);
        
        paint.Color = SKColors.White;
        paint.TextSize = 12;
        paint.TextAlign = SKTextAlign.Center;
        canvas.DrawText("STATIC ROUTE", 260, centerY - 80, paint);
        canvas.DrawText("192.168.2.0/24", 260, centerY - 65, paint);
        canvas.DrawText("via 192.168.1.1", 260, centerY - 50, paint);

        // Sign post
        paint.Color = SKColors.Brown;
        paint.Style = SKPaintStyle.Stroke;
        paint.StrokeWidth = 4;
        canvas.DrawLine(260, centerY - 40, 260, centerY + 20, paint);

        DrawTitleOverlay(canvas, "The Unchanging Golden Path", width, height);
    }

    /// <summary>
    /// Draw adaptive winding trail representing dynamic routing
    /// </summary>
    private void DrawAdaptiveWindingTrail(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        var centerX = width / 2f;
        var centerY = height / 2f;

        using var paint = new SKPaint();

        // Forest background
        var forestGradient = SKShader.CreateRadialGradient(
            new SKPoint(centerX, centerY),
            Math.Min(width, height) / 2f,
            new[] { SKColors.DarkGreen.WithAlpha(50), SKColors.ForestGreen.WithAlpha(30) },
            SKShaderTileMode.Clamp);
        paint.Shader = forestGradient;
        paint.Style = SKPaintStyle.Fill;
        canvas.DrawRect(0, 0, width, height, paint);
        paint.Shader = null;

        // Multiple adaptive paths
        paint.Style = SKPaintStyle.Stroke;
        paint.StrokeWidth = 8;
        
        var pathColors = new[] { PrimaryColor, SecondaryColor, AccentColor };
        var pathNames = new[] { "OSPF Path", "RIP Path", "BGP Path" };

        for (int i = 0; i < 3; i++)
        {
            paint.Color = pathColors[i];
            
            var adaptivePath = new SKPath();
            var startX = 50f;
            var startY = 100f + i * 120f;
            adaptivePath.MoveTo(startX, startY);

            // Create winding, adaptive path
            var points = new[]
            {
                new SKPoint(startX + 100, startY - 30 + i * 10),
                new SKPoint(startX + 200, startY + 40 - i * 15),
                new SKPoint(startX + 300, startY - 20 + i * 5),
                new SKPoint(width - 50, startY)
            };

            foreach (var point in points)
            {
                adaptivePath.LineTo(point);
            }

            canvas.DrawPath(adaptivePath, paint);

            // Path label
            paint.Style = SKPaintStyle.Fill;
            paint.Color = SKColors.White;
            paint.Color = paint.Color.WithAlpha(200);
            canvas.DrawRoundRect(startX + 50, startY - 15, 80, 20, 5, 5, paint);
            
            paint.Color = TextColor;
            paint.TextSize = 10;
            paint.TextAlign = SKTextAlign.Center;
            canvas.DrawText(pathNames[i], startX + 90, startY - 2, paint);

            paint.Style = SKPaintStyle.Stroke;
        }

        // Adaptive decision points (routers that can switch paths)
        paint.Style = SKPaintStyle.Fill;
        paint.Color = AccentColor;
        
        var decisionPoints = new[]
        {
            new SKPoint(150, 120),
            new SKPoint(250, 220),
            new SKPoint(350, 320)
        };

        foreach (var point in decisionPoints)
        {
            // Router symbol
            canvas.DrawCircle(point, 20, paint);
            
            // Decision indicator
            paint.Color = SKColors.White;
            paint.TextSize = 12;
            paint.TextAlign = SKTextAlign.Center;
            canvas.DrawText("🔄", point.X, point.Y + 4, paint);

            // Glowing effect
            paint.Color = AccentColor.WithAlpha(100);
            for (int r = 25; r <= 35; r += 5)
            {
                paint.Style = SKPaintStyle.Stroke;
                paint.StrokeWidth = 2;
                canvas.DrawCircle(point, r, paint);
            }
            
            paint.Style = SKPaintStyle.Fill;
            paint.Color = AccentColor;
        }

        // Network status indicators
        paint.Color = TextColor;
        paint.TextSize = 10;
        paint.TextAlign = SKTextAlign.Left;
        canvas.DrawText("🟢 Link Up - Fast Path", 20, height - 60, paint);
        canvas.DrawText("🟡 Congestion - Rerouting", 20, height - 45, paint);
        canvas.DrawText("🔴 Link Down - Alternate Path", 20, height - 30, paint);

        DrawTitleOverlay(canvas, "Dynamic Adaptive Network Trails", width, height);
    }

    /// <summary>
    /// Draw village gossip representing RIP protocol communication
    /// </summary>
    private void DrawVillageGossipWhispers(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        var centerX = width / 2f;
        var centerY = height / 2f;

        using var paint = new SKPaint();

        // Village background
        paint.Style = SKPaintStyle.Fill;
        paint.Color = SKColors.LightBlue;
        canvas.DrawRect(0, 0, width, height, paint);

        // Ground
        paint.Color = SKColors.Green;
        canvas.DrawRect(0, height * 0.7f, width, height * 0.3f, paint);

        // Village houses (routers)
        var houses = new[]
        {
            new { X = 100, Y = centerY, Label = "Router A" },
            new { X = 300, Y = centerY - 50, Label = "Router B" },
            new { X = 500, Y = centerY + 20, Label = "Router C" },
            new { X = 700, Y = centerY - 30, Label = "Router D" }
        };

        foreach (var house in houses)
        {
            // House body
            paint.Color = PrimaryColor;
            canvas.DrawRect(house.X - 30, house.Y - 20, 60, 50, paint);
            
            // Roof
            paint.Color = SecondaryColor;
            var roofPath = new SKPath();
            roofPath.MoveTo(house.X - 35, house.Y - 20);
            roofPath.LineTo(house.X, house.Y - 45);
            roofPath.LineTo(house.X + 35, house.Y - 20);
            roofPath.Close();
            canvas.DrawPath(roofPath, paint);
            
            // Door
            paint.Color = SKColors.Brown;
            canvas.DrawRect(house.X - 8, house.Y + 10, 16, 20, paint);
            
            // Window
            paint.Color = SKColors.Yellow;
            canvas.DrawRect(house.X - 20, house.Y - 5, 12, 10, paint);
            
            // Antenna (representing network interface)
            paint.Color = TextColor;
            paint.Style = SKPaintStyle.Stroke;
            paint.StrokeWidth = 2;
            canvas.DrawLine(house.X, house.Y - 45, house.X, house.Y - 60, paint);
            canvas.DrawLine(house.X - 8, house.Y - 55, house.X + 8, house.Y - 55, paint);
            
            // Router label
            paint.Style = SKPaintStyle.Fill;
            paint.Color = TextColor;
            paint.TextSize = 10;
            paint.TextAlign = SKTextAlign.Center;
            canvas.DrawText(house.Label, house.X, house.Y + 50, paint);
        }

        // Speech bubbles with routing information
        var gossipBubbles = new[]
        {
            new { X = 200, Y = centerY - 80, Text = "Net 192.168.1.0\n2 hops away!" },
            new { X = 400, Y = centerY - 100, Text = "Net 10.0.0.0\n1 hop via me!" },
            new { X = 600, Y = centerY - 70, Text = "Update: Net down\nRerouting..." }
        };

        foreach (var bubble in gossipBubbles)
        {
            // Speech bubble
            paint.Color = SKColors.White;
            paint.Style = SKPaintStyle.Fill;
            canvas.DrawRoundRect(bubble.X - 40, bubble.Y - 25, 80, 40, 8, 8, paint);
            
            // Bubble pointer
            var pointer = new SKPath();
            pointer.MoveTo(bubble.X - 10, bubble.Y + 15);
            pointer.LineTo(bubble.X - 20, bubble.Y + 35);
            pointer.LineTo(bubble.X + 5, bubble.Y + 15);
            pointer.Close();
            canvas.DrawPath(pointer, paint);
            
            // Border
            paint.Color = TextColor;
            paint.Style = SKPaintStyle.Stroke;
            paint.StrokeWidth = 1;
            canvas.DrawRoundRect(bubble.X - 40, bubble.Y - 25, 80, 40, 8, 8, paint);
            canvas.DrawPath(pointer, paint);
            
            // Text
            paint.Style = SKPaintStyle.Fill;
            paint.Color = TextColor;
            paint.TextSize = 8;
            paint.TextAlign = SKTextAlign.Center;
            var lines = bubble.Text.Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                canvas.DrawText(lines[i], bubble.X, bubble.Y - 10 + i * 12, paint);
            }
        }

        // RIP update timer
        paint.Color = AccentColor;
        paint.Style = SKPaintStyle.Fill;
        canvas.DrawCircle(centerX, 80, 40, paint);
        
        paint.Color = SKColors.White;
        paint.TextSize = 14;
        paint.TextAlign = SKTextAlign.Center;
        canvas.DrawText("RIP", centerX, 75, paint);
        canvas.DrawText("30s", centerX, 90, paint);

        // Clock hands for 30-second updates
        paint.Color = SKColors.White;
        paint.Style = SKPaintStyle.Stroke;
        paint.StrokeWidth = 3;
        canvas.DrawLine(centerX, 80, centerX, 65, paint); // Minute hand
        canvas.DrawLine(centerX, 80, centerX + 15, 80, paint); // Second hand

        DrawTitleOverlay(canvas, "Village Network Gossip Protocol", width, height);
    }

    // Simplified implementations for remaining routing drawing methods
    private void DrawDetailedNetworkTopology(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        DrawGenericRoutingConcept(canvas, width, height, "OSPF Network Topology Map", "🗺️📡");
    }

    private void DrawWorldMapInternetRoutes(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        DrawGenericRoutingConcept(canvas, width, height, "BGP Global Internet Routes", "🌍🛰️");
    }

    private void DrawNetworkSafetyNetFunnel(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        DrawGenericRoutingConcept(canvas, width, height, "Default Route Safety Net", "🕳️🛡️");
    }

    private void DrawDigitalPhoneBookRoutes(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        DrawGenericRoutingConcept(canvas, width, height, "Routing Table Directory", "📞📋");
    }

    private void DrawQualityMeasurementScales(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        DrawGenericRoutingConcept(canvas, width, height, "Route Metric Quality Scales", "⚖️📊");
    }

    private void DrawSettlingDustConsensus(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        DrawGenericRoutingConcept(canvas, width, height, "Network Convergence Process", "🌪️✅");
    }

    private void DrawInfiniteLoopMazeDanger(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        DrawGenericRoutingConcept(canvas, width, height, "Routing Loop Danger Zone", "♾️⚠️");
    }

    private void DrawSecurityCheckpointGuards(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        DrawGenericRoutingConcept(canvas, width, height, "ACL Route Security Guards", "🛡️🚧");
    }

    private void DrawMagicalWandNetworkPaths(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        DrawGenericRoutingConcept(canvas, width, height, "PowerShell Route Magic", "🪄🛤️");
    }

    private void DrawParallelUniverseRouting(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        DrawGenericRoutingConcept(canvas, width, height, "VRF Virtual Routing Realms", "🌌🔀");
    }

    private void DrawDetectiveNetworkBreadcrumbs(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        DrawGenericRoutingConcept(canvas, width, height, "Traceroute Detective Trail", "🔍🍞");
    }

    private void DrawArmoredSecurePathway(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        DrawGenericRoutingConcept(canvas, width, height, "Secure Routing Protocols", "🛡️🔐");
    }

    private void DrawAdvancedHybridRouting(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        DrawGenericRoutingConcept(canvas, width, height, "EIGRP Hybrid Protocol", "🤖⚡");
    }

    private void DrawExpressLaneLabeledPackages(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        DrawGenericRoutingConcept(canvas, width, height, "MPLS Express Lanes", "🏎️📦");
    }

    private void DrawDeepOceanBGPExploration(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        DrawGenericRoutingConcept(canvas, width, height, "BGP Deep Dive Mastery", "🌊🤿");
    }

    private void DrawTrophyRoutingMasteryCrown(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        DrawGenericRoutingConcept(canvas, width, height, "Routing Riddle Champion", "👑🏆");
    }

    /// <summary>
    /// Generic routing concept drawing helper
    /// </summary>
    private void DrawGenericRoutingConcept(SKCanvas canvas, int width, int height, string conceptName, string emoji)
    {
        var centerX = width / 2f;
        var centerY = height / 2f;

        using var paint = new SKPaint();

        // Background gradient with routing theme
        var gradient = SKShader.CreateRadialGradient(
            new SKPoint(centerX, centerY),
            Math.Min(width, height) / 3f,
            new[] { PrimaryColor.WithAlpha(20), SecondaryColor.WithAlpha(10) },
            SKShaderTileMode.Clamp);
        paint.Shader = gradient;
        paint.Style = SKPaintStyle.Fill;
        canvas.DrawRect(0, 0, width, height, paint);
        paint.Shader = null;

        // Central routing concept circle
        paint.Color = PrimaryColor;
        paint.Style = SKPaintStyle.Fill;
        canvas.DrawCircle(centerX, centerY, 60, paint);

        // Concept emoji/symbol
        paint.Color = SKColors.White;
        paint.TextSize = 48;
        paint.TextAlign = SKTextAlign.Center;
        canvas.DrawText(emoji, centerX, centerY + 15, paint);

        // Routing path indicators around the circle
        paint.Color = AccentColor;
        paint.Style = SKPaintStyle.Stroke;
        paint.StrokeWidth = 3;
        
        for (int i = 0; i < 8; i++)
        {
            var angle = i * Math.PI * 2 / 8;
            var startRadius = 75;
            var endRadius = 95;
            var startX = centerX + startRadius * Math.Cos(angle);
            var startY = centerY + startRadius * Math.Sin(angle);
            var endX = centerX + endRadius * Math.Cos(angle);
            var endY = centerY + endRadius * Math.Sin(angle);
            
            canvas.DrawLine((float)startX, (float)startY, (float)endX, (float)endY, paint);
        }

        // Concept name
        paint.Style = SKPaintStyle.Fill;
        paint.Color = TextColor;
        paint.TextSize = 14;
        paint.TextAlign = SKTextAlign.Center;
        canvas.DrawText(conceptName, centerX, centerY + 90, paint);

        DrawTitleOverlay(canvas, conceptName, width, height);
    }

    /// <summary>
    /// Helper method to draw compass rose
    /// </summary>
    private void DrawCompassRose(SKCanvas canvas, SKPaint paint, float x, float y, float radius)
    {
        paint.Style = SKPaintStyle.Stroke;
        paint.StrokeWidth = 2;
        
        // Main compass circle
        canvas.DrawCircle(x, y, radius, paint);
        
        // Compass points
        var directions = new[] { "N", "E", "S", "W" };
        for (int i = 0; i < 4; i++)
        {
            var angle = i * Math.PI / 2;
            var pointX = x + (radius - 5) * Math.Cos(angle - Math.PI / 2);
            var pointY = y + (radius - 5) * Math.Sin(angle - Math.PI / 2);
            
            paint.Style = SKPaintStyle.Fill;
            paint.TextSize = 10;
            paint.TextAlign = SKTextAlign.Center;
            canvas.DrawText(directions[i], (float)pointX, (float)pointY + 3, paint);
            paint.Style = SKPaintStyle.Stroke;
        }

        // Center point
        paint.Style = SKPaintStyle.Fill;
        canvas.DrawCircle(x, y, 3, paint);
    }

    #endregion

    #region Module 6 Security Shenanigans Drawing Methods

    /// <summary>
    /// Draw digital castle with moat and guards for network security
    /// </summary>
    private void DrawDigitalCastleWithMoatAndGuards(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        var centerX = width / 2f;
        var centerY = height / 2f;
        using var paint = new SKPaint();

        // Draw moat (water around castle)
        paint.Style = SKPaintStyle.Fill;
        paint.Color = SKColor.Parse("#4A90E2");
        canvas.DrawRoundRect(centerX - 150, centerY + 20, 300, 40, 10, 10, paint);

        // Draw castle main structure
        paint.Color = SKColors.Gray;
        canvas.DrawRect(centerX - 80, centerY - 60, 160, 120, paint);

        // Draw castle towers
        paint.Color = SKColors.DarkGray;
        canvas.DrawRect(centerX - 100, centerY - 80, 30, 140, paint);
        canvas.DrawRect(centerX + 70, centerY - 80, 30, 140, paint);
        
        // Security shields on walls
        paint.Color = AccentColor;
        var shieldPath = new SKPath();
        shieldPath.MoveTo(centerX, centerY - 30);
        shieldPath.LineTo(centerX - 15, centerY - 15);
        shieldPath.LineTo(centerX - 15, centerY + 5);
        shieldPath.LineTo(centerX, centerY + 15);
        shieldPath.LineTo(centerX + 15, centerY + 5);
        shieldPath.LineTo(centerX + 15, centerY - 15);
        shieldPath.Close();
        canvas.DrawPath(shieldPath, paint);

        // Security symbol on shield
        paint.Color = SKColors.White;
        paint.TextSize = 16;
        paint.TextAlign = SKTextAlign.Center;
        canvas.DrawText("🛡️", centerX, centerY - 5, paint);
    }

    /// <summary>
    /// Draw bouncer at club door for firewall concept
    /// </summary>
    private void DrawBouncerAtClubDoor(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        var centerX = width / 2f;
        var centerY = height / 2f;
        using var paint = new SKPaint();

        // Draw club door/entrance
        paint.Style = SKPaintStyle.Fill;
        paint.Color = SKColors.Brown;
        canvas.DrawRect(centerX + 40, centerY - 80, 60, 160, paint);

        // Draw bouncer (large figure)
        paint.Color = SKColors.Black;
        canvas.DrawRoundRect(centerX - 40, centerY - 20, 40, 80, 10, 10, paint);
        
        // Bouncer head
        paint.Color = SKColor.Parse("#FFDBAC");
        canvas.DrawCircle(centerX - 20, centerY - 40, 15, paint);
        
        // Clipboard
        paint.Color = SKColors.White;
        canvas.DrawRect(centerX - 15, centerY + 10, 20, 25, paint);
        paint.Color = SKColors.Black;
        paint.TextSize = 8;
        paint.TextAlign = SKTextAlign.Center;
        canvas.DrawText("ACCESS", centerX - 5, centerY + 20, paint);
        canvas.DrawText("LIST", centerX - 5, centerY + 28, paint);
    }

    /// <summary>
    /// Draw encrypted treasure chest with glowing runes
    /// </summary>
    private void DrawEncryptedTreasureChest(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        var centerX = width / 2f;
        var centerY = height / 2f;
        using var paint = new SKPaint();

        // Draw treasure chest
        paint.Style = SKPaintStyle.Fill;
        paint.Color = SKColors.Brown;
        canvas.DrawRoundRect(centerX - 60, centerY - 10, 120, 50, 5, 5, paint);

        // Large lock on chest
        paint.Color = SKColors.Gold;
        canvas.DrawRoundRect(centerX - 15, centerY - 5, 30, 20, 3, 3, paint);
        
        // Glowing encryption runes
        paint.Color = AccentColor;
        paint.TextSize = 20;
        paint.TextAlign = SKTextAlign.Center;
        canvas.DrawText("⚡", centerX - 80, centerY - 30, paint);
        canvas.DrawText("🔮", centerX + 80, centerY - 30, paint);
        canvas.DrawText("🔥", centerX - 80, centerY + 30, paint);
        canvas.DrawText("❄️", centerX + 80, centerY + 30, paint);

        // AES-256 label
        paint.Color = TextColor;
        paint.TextSize = 10;
        paint.TextAlign = SKTextAlign.Center;
        canvas.DrawText("AES-256 ENCRYPTED", centerX, centerY + 60, paint);
    }

    /// <summary>
    /// Draw secret underground tunnel for VPN concept
    /// </summary>
    private void DrawSecretUndergroundTunnel(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        var centerX = width / 2f;
        var centerY = height / 2f;
        using var paint = new SKPaint();

        // Draw ground level
        paint.Style = SKPaintStyle.Fill;
        paint.Color = SKColor.Parse("#8B4513");
        canvas.DrawRect(0, centerY + 40, width, height - (centerY + 40), paint);

        // Draw tunnel entrances
        paint.Color = SKColors.Black;
        canvas.DrawCircle(50, centerY + 20, 25, paint);
        canvas.DrawCircle(width - 50, centerY + 20, 25, paint);

        // Glowing tunnel path
        paint.Color = AccentColor;
        paint.Style = SKPaintStyle.Stroke;
        paint.StrokeWidth = 8;
        canvas.DrawLine(75, centerY + 20, width - 75, centerY + 20, paint);

        // Labels
        paint.Style = SKPaintStyle.Fill;
        paint.Color = TextColor;
        paint.TextSize = 12;
        paint.TextAlign = SKTextAlign.Center;
        canvas.DrawText("Safe VPN Tunnel", centerX, centerY + 90, paint);
    }

    /// <summary>
    /// Draw medieval watchtower with guards for IDS/IPS
    /// </summary>
    private void DrawMedievalWatchtowerGuards(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        var centerX = width / 2f;
        var centerY = height / 2f;
        using var paint = new SKPaint();

        // Draw watchtower
        paint.Style = SKPaintStyle.Fill;
        paint.Color = SKColors.Gray;
        canvas.DrawRect(centerX - 30, centerY - 20, 60, 120, paint);

        // Tower top platform
        paint.Color = SKColors.DarkGray;
        canvas.DrawRect(centerX - 40, centerY - 40, 80, 20, paint);

        // Alert guard
        paint.Color = AccentColor;
        canvas.DrawCircle(centerX, centerY - 60, 8, paint);

        // Detection rays
        paint.Color = SecondaryColor;
        paint.Style = SKPaintStyle.Stroke;
        paint.StrokeWidth = 2;
        for (int i = 0; i < 5; i++)
        {
            var angle = (i - 2) * 0.3;
            var endX = centerX + 80 * Math.Cos(angle);
            var endY = centerY - 40 + 80 * Math.Sin(angle);
            canvas.DrawLine(centerX, centerY - 40, (float)endX, (float)endY, paint);
        }

        // Alert status
        paint.Style = SKPaintStyle.Fill;
        paint.Color = TextColor;
        paint.TextSize = 10;
        paint.TextAlign = SKTextAlign.Center;
        canvas.DrawText("INTRUSION DETECTED!", centerX, centerY + 90, paint);
    }

    /// <summary>
    /// Draw castle wall with cracks for vulnerabilities
    /// </summary>
    private void DrawCrackedCastleWall(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        var centerX = width / 2f;
        var centerY = height / 2f;
        using var paint = new SKPaint();

        // Draw main wall
        paint.Style = SKPaintStyle.Fill;
        paint.Color = SKColors.LightGray;
        canvas.DrawRect(centerX - 120, centerY - 60, 240, 120, paint);

        // Draw cracks (vulnerabilities)
        paint.Color = SKColors.Red;
        paint.Style = SKPaintStyle.Stroke;
        paint.StrokeWidth = 3;
        
        var crack = new SKPath();
        crack.MoveTo(centerX - 80, centerY - 60);
        crack.LineTo(centerX - 75, centerY - 30);
        crack.LineTo(centerX - 85, centerY - 10);
        crack.LineTo(centerX - 70, centerY + 20);
        canvas.DrawPath(crack, paint);

        // Vulnerability markers
        paint.Style = SKPaintStyle.Fill;
        paint.Color = SecondaryColor;
        paint.TextSize = 20;
        paint.TextAlign = SKTextAlign.Center;
        canvas.DrawText("⚠️", centerX - 80, centerY, paint);

        // CVSS severity
        paint.Color = SKColors.Red;
        paint.TextSize = 12;
        canvas.DrawText("CVSS: 9.8", centerX - 80, centerY + 25, paint);
        canvas.DrawText("CRITICAL", centerX - 80, centerY + 35, paint);
    }

    // Add simplified implementations for remaining security drawing methods
    private void DrawCastleInfestedWithBugs(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        DrawDigitalCastleWithMoatAndGuards(canvas, width, height, style);
        using var paint = new SKPaint();
        paint.Color = SKColors.Red;
        paint.TextSize = 16;
        paint.TextAlign = SKTextAlign.Center;
        for (int i = 0; i < 5; i++)
        {
            canvas.DrawText("🐛", width / 2f - 60 + i * 30, height / 2f - 20, paint);
        }
    }

    private void DrawPhishingRodWithEmailBait(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        var centerX = width / 2f;
        var centerY = height / 2f;
        using var paint = new SKPaint();
        
        // Fishing rod
        paint.Style = SKPaintStyle.Stroke;
        paint.Color = SKColors.Brown;
        paint.StrokeWidth = 4;
        canvas.DrawLine(centerX - 80, centerY + 80, centerX + 40, centerY - 40, paint);
        
        // Hook with email bait
        paint.Style = SKPaintStyle.Fill;
        paint.Color = SKColors.White;
        canvas.DrawRect(centerX + 30, centerY - 50, 40, 25, paint);
        paint.Color = TextColor;
        paint.TextSize = 8;
        paint.TextAlign = SKTextAlign.Center;
        canvas.DrawText("You Won!", centerX + 50, centerY - 40, paint);
        canvas.DrawText("Click Here!", centerX + 50, centerY - 32, paint);
    }

    private void DrawKeymasterAccessControl(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        var centerX = width / 2f;
        var centerY = height / 2f;
        using var paint = new SKPaint();
        
        // Keymaster figure
        paint.Style = SKPaintStyle.Fill;
        paint.Color = AccentColor;
        canvas.DrawCircle(centerX, centerY - 40, 15, paint);
        canvas.DrawRect(centerX - 10, centerY - 25, 20, 40, paint);
        
        // Keys
        paint.Color = SKColors.Gold;
        canvas.DrawText("🗝️", centerX - 30, centerY, paint);
        canvas.DrawText("🗝️", centerX + 30, centerY, paint);
    }

    private void DrawTwoKeysDoubleLocks(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        var centerX = width / 2f;
        var centerY = height / 2f;
        using var paint = new SKPaint();
        
        paint.Style = SKPaintStyle.Fill;
        paint.Color = SKColors.Gold;
        paint.TextSize = 40;
        paint.TextAlign = SKTextAlign.Center;
        canvas.DrawText("🔐", centerX - 40, centerY, paint);
        canvas.DrawText("🔐", centerX + 40, centerY, paint);
        
        paint.Color = TextColor;
        paint.TextSize = 12;
        canvas.DrawText("Two-Factor Authentication", centerX, centerY + 60, paint);
    }

    // Placeholder implementations for remaining security methods
    private void DrawSecurityWardShield(SKCanvas canvas, int width, int height, ImageStyle style) => DrawDigitalCastleWithMoatAndGuards(canvas, width, height, style);
    private void DrawWalledNetworkSections(SKCanvas canvas, int width, int height, ImageStyle style) => DrawDigitalCastleWithMoatAndGuards(canvas, width, height, style);
    private void DrawSecurityChronicleScroll(SKCanvas canvas, int width, int height, ImageStyle style) => DrawDigitalCastleWithMoatAndGuards(canvas, width, height, style);
    private void DrawMockBattlePractice(SKCanvas canvas, int width, int height, ImageStyle style) => DrawDigitalCastleWithMoatAndGuards(canvas, width, height, style);
    private void DrawSuspiciousGuardVerifying(SKCanvas canvas, int width, int height, ImageStyle style) => DrawDigitalCastleWithMoatAndGuards(canvas, width, height, style);
    private void DrawCloudCastleInSky(SKCanvas canvas, int width, int height, ImageStyle style) => DrawDigitalCastleWithMoatAndGuards(canvas, width, height, style);
    private void DrawIncidentResponseWarRoom(SKCanvas canvas, int width, int height, ImageStyle style) => DrawDigitalCastleWithMoatAndGuards(canvas, width, height, style);
    private void DrawLawBookCompliance(SKCanvas canvas, int width, int height, ImageStyle style) => DrawDigitalCastleWithMoatAndGuards(canvas, width, height, style);
    private void DrawArmoredKnightAdvanced(SKCanvas canvas, int width, int height, ImageStyle style) => DrawDigitalCastleWithMoatAndGuards(canvas, width, height, style);
    private void DrawLockedTrophyFortress(SKCanvas canvas, int width, int height, ImageStyle style) => DrawDigitalCastleWithMoatAndGuards(canvas, width, height, style);

    #endregion

    #region Module 7: Wireless Wonders Drawing Methods

    /// <summary>
    /// Draw invisible wireless roads with floating data packets traveling through the air
    /// </summary>
    private void DrawInvisibleWirelessRoads(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        var centerX = width / 2f;
        var centerY = height / 2f;

        using var paint = new SKPaint { IsAntialias = true };

        // Draw multiple invisible roads as dotted/dashed lines
        paint.Style = SKPaintStyle.Stroke;
        paint.Color = PrimaryColor;
        paint.StrokeWidth = 4;
        paint.PathEffect = SKPathEffect.CreateDash(new float[] { 10, 5 }, 0);

        // Create curving wireless paths
        for (int i = 0; i < 5; i++)
        {
            using var path = new SKPath();
            var startX = 50 + i * 20;
            var endX = width - 50 - i * 20;
            var amplitude = 30 + i * 10;
            
            path.MoveTo(startX, centerY);
            for (float x = startX; x <= endX; x += 10)
            {
                var y = centerY + (float)(amplitude * Math.Sin((x - startX) * 0.02f + i * 0.5f));
                path.LineTo(x, y);
            }
            canvas.DrawPath(path, paint);
        }

        // Draw floating data packets as small colorful squares
        paint.Style = SKPaintStyle.Fill;
        var dataColors = new[] { SecondaryColor, AccentColor, SKColors.Purple, SKColors.Orange };
        for (int i = 0; i < 8; i++)
        {
            paint.Color = dataColors[i % dataColors.Length];
            var x = 100 + (i * 80) + (float)(Math.Sin(i * 0.7) * 30);
            var y = centerY + (float)(Math.Cos(i * 0.8) * 40);
            canvas.DrawRoundRect(x - 8, y - 8, 16, 16, 4, 4, paint);
        }

        // Add "Invisible Highways" text
        paint.Color = TextColor;
        paint.TextSize = 24;
        paint.TextAlign = SKTextAlign.Center;
        canvas.DrawText("Invisible Highways", centerX, height - 50, paint);
        
        paint.TextSize = 16;
        canvas.DrawText("Data travels through the air on magical wireless paths!", centerX, height - 25, paint);
    }

    /// <summary>
    /// Draw a home surrounded by WiFi waves emanating from a router
    /// </summary>
    private void DrawHomeWithWiFiWaves(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        var centerX = width / 2f;
        var centerY = height / 2f;

        using var paint = new SKPaint { IsAntialias = true };

        // Draw house
        paint.Style = SKPaintStyle.Fill;
        paint.Color = SKColors.Brown;
        canvas.DrawRect(centerX - 50, centerY - 20, 100, 60, paint);

        // House roof
        paint.Color = SKColors.Red;
        using (var path = new SKPath())
        {
            path.MoveTo(centerX - 60, centerY - 20);
            path.LineTo(centerX, centerY - 60);
            path.LineTo(centerX + 60, centerY - 20);
            path.Close();
            canvas.DrawPath(path, paint);
        }

        // Router inside house
        paint.Color = SKColors.Black;
        canvas.DrawRoundRect(centerX - 15, centerY - 5, 30, 10, 2, 2, paint);

        // Router antennas
        paint.Style = SKPaintStyle.Stroke;
        paint.StrokeWidth = 3;
        canvas.DrawLine(centerX - 10, centerY - 5, centerX - 10, centerY - 15, paint);
        canvas.DrawLine(centerX + 10, centerY - 5, centerX + 10, centerY - 15, paint);

        // WiFi waves emanating from router
        paint.Color = PrimaryColor;
        paint.StrokeWidth = 2;
        for (int i = 1; i <= 4; i++)
        {
            var radius = i * 40;
            canvas.DrawCircle(centerX, centerY, radius, paint);
        }

        // Devices connected to WiFi
        var devicePositions = new[] 
        {
            (centerX - 120, centerY + 30),
            (centerX + 120, centerY - 30),
            (centerX - 80, centerY - 80),
            (centerX + 90, centerY + 70)
        };

        paint.Style = SKPaintStyle.Fill;
        foreach (var (x, y) in devicePositions)
        {
            // Draw device
            paint.Color = SecondaryColor;
            canvas.DrawRoundRect(x - 12, y - 8, 24, 16, 4, 4, paint);
            
            // WiFi signal indicator on device
            paint.Color = AccentColor;
            paint.TextSize = 12;
            paint.TextAlign = SKTextAlign.Center;
            canvas.DrawText("📶", x, y + 4, paint);
        }

        // Title
        paint.Color = TextColor;
        paint.TextSize = 22;
        paint.TextAlign = SKTextAlign.Center;
        canvas.DrawText("Home WiFi Network", centerX, height - 40, paint);
        
        paint.TextSize = 14;
        canvas.DrawText("Your router creates invisible waves that connect all your devices!", centerX, height - 20, paint);
    }

    /// <summary>
    /// Draw WiFi standards as a family tree with different generations
    /// </summary>
    private void DrawWiFiStandardsFamilyTree(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        var centerX = width / 2f;
        var startY = 60f;

        using var paint = new SKPaint { IsAntialias = true };

        // Family tree trunk
        paint.Style = SKPaintStyle.Stroke;
        paint.Color = SKColors.Brown;
        paint.StrokeWidth = 6;
        canvas.DrawLine(centerX, startY + 40, centerX, height - 100, paint);

        // WiFi standards as family members with different ages
        var standards = new[]
        {
            ("802.11a\n(1999)", centerX - 140, startY + 80, SKColors.LightGray),
            ("802.11b\n(1999)", centerX - 70, startY + 80, SKColors.LightBlue),
            ("802.11g\n(2003)", centerX + 70, startY + 80, SKColors.Green),
            ("802.11n\n(2009)", centerX - 100, startY + 140, SKColors.Orange),
            ("802.11ac\n(2013)", centerX + 100, startY + 140, SKColors.Purple),
            ("802.11ax\n(WiFi 6)", centerX, startY + 200, SKColors.Red)
        };

        paint.Style = SKPaintStyle.Fill;
        foreach (var (name, x, y, color) in standards)
        {
            // Draw family member circle
            paint.Color = color;
            canvas.DrawCircle(x, y, 35, paint);
            
            // Draw connection to trunk
            paint.Color = SKColors.Brown;
            paint.Style = SKPaintStyle.Stroke;
            paint.StrokeWidth = 2;
            canvas.DrawLine(x, y, centerX, y, paint);
            canvas.DrawLine(centerX, y, centerX, startY + 40, paint);
            
            // Draw name
            paint.Style = SKPaintStyle.Fill;
            paint.Color = TextColor;
            paint.TextSize = 11;
            paint.TextAlign = SKTextAlign.Center;
            canvas.DrawText(name, x, y + 55, paint);
        }

        // Speed indicators
        paint.TextSize = 9;
        paint.Color = LightTextColor;
        canvas.DrawText("11 Mbps", centerX - 140, startY + 130, paint);
        canvas.DrawText("11 Mbps", centerX - 70, startY + 130, paint);
        canvas.DrawText("54 Mbps", centerX + 70, startY + 130, paint);
        canvas.DrawText("600 Mbps", centerX - 100, startY + 190, paint);
        canvas.DrawText("6.9 Gbps", centerX + 100, startY + 190, paint);
        canvas.DrawText("9.6 Gbps", centerX, startY + 250, paint);

        // Title
        paint.Color = TextColor;
        paint.TextSize = 24;
        paint.TextAlign = SKTextAlign.Center;
        canvas.DrawText("WiFi Standards Family Tree", centerX, 30, paint);
        
        paint.TextSize = 14;
        canvas.DrawText("Each generation gets faster - like upgrading from a pony to a jet!", centerX, height - 20, paint);
    }

    /// <summary>
    /// Draw radio dials showing different frequency bands (2.4GHz, 5GHz, 6GHz)
    /// </summary>
    private void DrawFrequencyRadioDials(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        var centerX = width / 2f;
        var centerY = height / 2f;

        using var paint = new SKPaint { IsAntialias = true };

        var frequencies = new[]
        {
            ("2.4 GHz", centerX - 120, centerY, SKColors.Blue, "Crowded but Far"),
            ("5 GHz", centerX, centerY, SKColors.Green, "Fast but Short"),
            ("6 GHz", centerX + 120, centerY, SKColors.Red, "New Speed Lane")
        };

        foreach (var (freq, x, y, color, desc) in frequencies)
        {
            // Draw radio dial background
            paint.Style = SKPaintStyle.Fill;
            paint.Color = SKColors.LightGray;
            canvas.DrawCircle(x, y, 60, paint);
            
            // Draw frequency band as colored arc
            paint.Style = SKPaintStyle.Stroke;
            paint.Color = color;
            paint.StrokeWidth = 8;
            using (var path = new SKPath())
            {
                path.AddArc(SKRect.Create(x - 50, y - 50, 100, 100), -45, 90);
                canvas.DrawPath(path, paint);
            }
            
            // Draw dial needle
            paint.Color = SKColors.Black;
            paint.StrokeWidth = 3;
            var needleAngle = freq == "2.4 GHz" ? -30 : freq == "5 GHz" ? 0 : 30;
            var needleX = x + 40 * (float)Math.Cos(Math.PI * needleAngle / 180);
            var needleY = y + 40 * (float)Math.Sin(Math.PI * needleAngle / 180);
            canvas.DrawLine(x, y, needleX, needleY, paint);
            
            // Draw center dot
            paint.Style = SKPaintStyle.Fill;
            paint.Color = SKColors.Black;
            canvas.DrawCircle(x, y, 5, paint);
            
            // Draw frequency label
            paint.Color = TextColor;
            paint.TextSize = 14;
            paint.TextAlign = SKTextAlign.Center;
            canvas.DrawText(freq, x, y + 80, paint);
            
            // Draw description
            paint.TextSize = 11;
            paint.Color = color;
            canvas.DrawText(desc, x, y + 95, paint);
        }

        // Add interference indicators
        paint.Color = SKColors.Red;
        paint.TextSize = 10;
        paint.TextAlign = SKTextAlign.Center;
        // 2.4GHz interference (microwaves, Bluetooth)
        canvas.DrawText("🍽️📱", centerX - 120, centerY - 80, paint);
        // 5GHz cleaner
        canvas.DrawText("✨", centerX, centerY - 80, paint);
        // 6GHz newest and cleanest
        canvas.DrawText("✨✨", centerX + 120, centerY - 80, paint);

        // Title
        paint.Color = TextColor;
        paint.TextSize = 22;
        paint.TextAlign = SKTextAlign.Center;
        canvas.DrawText("Wireless Frequency Bands", centerX, 40, paint);
        
        paint.TextSize = 14;
        canvas.DrawText("Like radio stations - each frequency has its own wireless highway!", centerX, height - 30, paint);
    }

    /// <summary>
    /// Draw access point as a lighthouse sending out beacon signals
    /// </summary>
    private void DrawAccessPointLighthouse(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        var centerX = width / 2f;
        var centerY = height / 2f + 50;

        using var paint = new SKPaint { IsAntialias = true };

        // Draw lighthouse base
        paint.Style = SKPaintStyle.Fill;
        paint.Color = SKColors.White;
        canvas.DrawRect(centerX - 20, centerY - 100, 40, 150, paint);
        
        // Lighthouse stripes
        paint.Color = SKColors.Red;
        for (int i = 0; i < 3; i++)
        {
            canvas.DrawRect(centerX - 20, centerY - 80 + i * 30, 40, 15, paint);
        }

        // Lighthouse top/beacon housing
        paint.Color = SKColors.Yellow;
        canvas.DrawCircle(centerX, centerY - 110, 25, paint);

        // Beacon light rays
        paint.Style = SKPaintStyle.Stroke;
        paint.Color = SKColors.Yellow;
        paint.StrokeWidth = 3;
        
        // Rotating beacon pattern
        for (int i = 0; i < 8; i++)
        {
            var angle = i * 45f;
            var radians = angle * Math.PI / 180;
            var startX = centerX + 25 * (float)Math.Cos(radians);
            var startY = centerY - 110 + 25 * (float)Math.Sin(radians);
            var endX = centerX + 80 * (float)Math.Cos(radians);
            var endY = centerY - 110 + 80 * (float)Math.Sin(radians);
            
            paint.StrokeWidth = 3 - i % 3; // Varying intensity
            canvas.DrawLine(startX, startY, endX, endY, paint);
        }

        // Ships (devices) receiving signals
        var devicePositions = new[]
        {
            (centerX - 100, centerY + 80),
            (centerX + 120, centerY + 60),
            (centerX - 80, centerY + 120)
        };

        paint.Style = SKPaintStyle.Fill;
        foreach (var (x, y) in devicePositions)
        {
            // Draw simple ship
            paint.Color = SecondaryColor;
            canvas.DrawRect(x - 15, y - 5, 30, 10, paint);
            
            // Mast
            paint.Style = SKPaintStyle.Stroke;
            paint.Color = SKColors.Black;
            paint.StrokeWidth = 2;
            canvas.DrawLine(x, y - 5, x, y - 20, paint);
            
            paint.Style = SKPaintStyle.Fill;
        }

        // Water waves
        paint.Style = SKPaintStyle.Stroke;
        paint.Color = SKColors.Blue;
        paint.StrokeWidth = 2;
        for (int i = 0; i < 5; i++)
        {
            using (var path = new SKPath())
            {
                var waveY = centerY + 100 + i * 10;
                path.MoveTo(50, waveY);
                for (float x = 50; x < width - 50; x += 20)
                {
                    path.QuadTo(x + 10, waveY - 5, x + 20, waveY);
                }
                canvas.DrawPath(path, paint);
            }
        }

        // Title
        paint.Style = SKPaintStyle.Fill;
        paint.Color = TextColor;
        paint.TextSize = 22;
        paint.TextAlign = SKTextAlign.Center;
        canvas.DrawText("Access Point Lighthouse", centerX, 40, paint);
        
        paint.TextSize = 14;
        canvas.DrawText("Access Points broadcast signals like beacons guiding data ships!", centerX, height - 20, paint);
    }

    /// <summary>
    /// Placeholder implementations for remaining wireless drawing methods
    /// </summary>
    private void DrawSSIDNameTags(SKCanvas canvas, int width, int height, ImageStyle style) => DrawHomeWithWiFiWaves(canvas, width, height, style);
    private void DrawEncryptedWirelessWaves(SKCanvas canvas, int width, int height, ImageStyle style) => DrawHomeWithWiFiWaves(canvas, width, height, style);
    private void DrawMeshNetworkSpiderWeb(SKCanvas canvas, int width, int height, ImageStyle style) => DrawHomeWithWiFiWaves(canvas, width, height, style);
    private void DrawBluetoothCloseWhispers(SKCanvas canvas, int width, int height, ImageStyle style) => DrawHomeWithWiFiWaves(canvas, width, height, style);
    private void DrawNFCTouchSpark(SKCanvas canvas, int width, int height, ImageStyle style) => DrawHomeWithWiFiWaves(canvas, width, height, style);
    private void DrawWirelessInterferenceTrafficJam(SKCanvas canvas, int width, int height, ImageStyle style) => DrawFrequencyRadioDials(canvas, width, height, style);
    private void DrawWirelessRoamingHandover(SKCanvas canvas, int width, int height, ImageStyle style) => DrawAccessPointLighthouse(canvas, width, height, style);
    private void DrawWirelessScriptingWand(SKCanvas canvas, int width, int height, ImageStyle style) => DrawInvisibleWirelessRoads(canvas, width, height, style);
    private void DrawWirelessSecurityShieldedWaves(SKCanvas canvas, int width, int height, ImageStyle style) => DrawHomeWithWiFiWaves(canvas, width, height, style);
    private void DrawWirelessTroubleshootingDetective(SKCanvas canvas, int width, int height, ImageStyle style) => DrawAccessPointLighthouse(canvas, width, height, style);
    private void DrawFuturisticFiveGSignals(SKCanvas canvas, int width, int height, ImageStyle style) => DrawInvisibleWirelessRoads(canvas, width, height, style);
    private void DrawIoTConnectedThings(SKCanvas canvas, int width, int height, ImageStyle style) => DrawHomeWithWiFiWaves(canvas, width, height, style);
    private void DrawWirelessTopologyPatterns(SKCanvas canvas, int width, int height, ImageStyle style) => DrawMeshNetworkSpiderWeb(canvas, width, height, style);
    private void DrawAdvancedWirelessProtocols(SKCanvas canvas, int width, int height, ImageStyle style) => DrawWiFiStandardsFamilyTree(canvas, width, height, style);
    private void DrawWirelessMasteryTrophy(SKCanvas canvas, int width, int height, ImageStyle style) => DrawAccessPointLighthouse(canvas, width, height, style);

    #endregion

    #region Module 8: Cloud Conquest Drawing Methods

    /// <summary>
    /// Draw floating cloud castles representing cloud data centers
    /// </summary>
    private void DrawFloatingCloudCastles(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        using var paint = new SKPaint { IsAntialias = true };

        // Draw multiple cloud layers
        var cloudPositions = new[]
        {
            (width * 0.2f, height * 0.3f, 120f),
            (width * 0.6f, height * 0.2f, 100f),
            (width * 0.8f, height * 0.4f, 110f),
            (width * 0.4f, height * 0.5f, 130f)
        };

        foreach (var (x, y, size) in cloudPositions)
        {
            // Draw fluffy cloud
            paint.Style = SKPaintStyle.Fill;
            paint.Color = SKColors.LightCyan;
            
            // Create cloud shape with multiple circles
            for (int i = 0; i < 5; i++)
            {
                var cloudX = x + (i - 2) * size * 0.15f;
                var cloudY = y + (float)(Math.Sin(i * 0.8) * size * 0.1);
                var cloudRadius = size * (0.4f + i * 0.1f);
                canvas.DrawCircle(cloudX, cloudY, cloudRadius, paint);
            }

            // Draw castle on cloud
            paint.Color = SKColors.SandyBrown;
            var castleWidth = size * 0.6f;
            var castleHeight = size * 0.8f;
            canvas.DrawRect(x - castleWidth/2, y - castleHeight/2, castleWidth, castleHeight, paint);

            // Castle towers
            paint.Color = SKColors.Gray;
            canvas.DrawRect(x - castleWidth/3, y - castleHeight/2 - 20, 15, 40, paint);
            canvas.DrawRect(x + castleWidth/3 - 15, y - castleHeight/2 - 20, 15, 40, paint);

            // Data streams between castles
            if (x < width * 0.5f)
            {
                paint.Style = SKPaintStyle.Stroke;
                paint.Color = PrimaryColor;
                paint.StrokeWidth = 3;
                paint.PathEffect = SKPathEffect.CreateDash(new float[] { 8, 4 }, 0);
                
                var nextCastle = cloudPositions.FirstOrDefault(c => c.Item1 > x + 100);
                if (nextCastle != default)
                {
                    canvas.DrawLine(x + castleWidth/2, y, nextCastle.Item1 - nextCastle.Item3 * 0.3f, nextCastle.Item2, paint);
                }
            }
        }

        // Add title
        paint.Style = SKPaintStyle.Fill;
        paint.Color = TextColor;
        paint.TextSize = 24;
        paint.TextAlign = SKTextAlign.Center;
        paint.PathEffect = null;
        canvas.DrawText("Floating Cloud Castles", width / 2f, height - 50, paint);
        
        paint.TextSize = 16;
        canvas.DrawText("Your data lives in magnificent sky fortresses!", width / 2f, height - 25, paint);
    }

    /// <summary>
    /// Draw IaaS building blocks floating in the sky
    /// </summary>
    private void DrawIaaSBuildingBlocks(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        var centerX = width / 2f;
        var centerY = height / 2f;

        using var paint = new SKPaint { IsAntialias = true };

        var iaasComponents = new[]
        {
            ("Compute", centerX - 120, centerY - 60, SKColors.Blue),
            ("Storage", centerX + 120, centerY - 60, SKColors.Green),
            ("Network", centerX - 120, centerY + 60, SKColors.Orange),
            ("Security", centerX + 120, centerY + 60, SKColors.Red)
        };

        foreach (var (label, x, y, color) in iaasComponents)
        {
            // Draw cloud platform
            paint.Style = SKPaintStyle.Fill;
            paint.Color = SKColors.LightBlue.WithAlpha(100);
            canvas.DrawCircle(x, y + 30, 40, paint);

            // Draw LEGO-like building block
            paint.Color = color;
            canvas.DrawRoundRect(x - 35, y - 35, 70, 50, 8, 8, paint);

            // Draw connection pegs on top
            paint.Color = color.WithAlpha(200);
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    var pegX = x - 20 + i * 20;
                    var pegY = y - 35 + j * 15;
                    canvas.DrawCircle(pegX, pegY, 6, paint);
                }
            }

            // Draw label
            paint.Color = SKColors.White;
            paint.TextSize = 12;
            paint.TextAlign = SKTextAlign.Center;
            canvas.DrawText(label, x, y - 5, paint);
        }

        // Draw construction worker in clouds
        paint.Color = SKColors.Yellow;
        canvas.DrawCircle(centerX, centerY - 120, 20, paint); // Hard hat
        
        paint.Color = SKColors.Pink;
        canvas.DrawCircle(centerX, centerY - 100, 15, paint); // Head

        // Title
        paint.Color = TextColor;
        paint.TextSize = 22;
        paint.TextAlign = SKTextAlign.Center;
        canvas.DrawText("IaaS Building Blocks", centerX, height - 40, paint);
        
        paint.TextSize = 14;
        canvas.DrawText("Rent powerful infrastructure components in the cloud!", centerX, height - 20, paint);
    }

    /// <summary>
    /// Draw cloud architecture blueprints floating in the sky
    /// </summary>
    private void DrawCloudArchitectureBlueprints(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        var centerX = width / 2f;
        var centerY = height / 2f;

        using var paint = new SKPaint { IsAntialias = true };

        // Draw blueprint background
        paint.Style = SKPaintStyle.Fill;
        paint.Color = SKColors.DarkBlue;
        canvas.DrawRect(50, 50, width - 100, height - 100, paint);

        // Draw blueprint grid
        paint.Style = SKPaintStyle.Stroke;
        paint.Color = SKColors.LightBlue;
        paint.StrokeWidth = 1;
        
        for (int i = 60; i < width - 50; i += 20)
        {
            canvas.DrawLine(i, 60, i, height - 60, paint);
        }
        for (int i = 60; i < height - 50; i += 20)
        {
            canvas.DrawLine(60, i, width - 60, i, paint);
        }

        // Draw cloud architecture components
        paint.Color = SKColors.White;
        paint.StrokeWidth = 2;
        
        // Load balancer
        canvas.DrawRect(centerX - 60, centerY - 80, 120, 30, paint);
        paint.TextSize = 10;
        paint.TextAlign = SKTextAlign.Center;
        canvas.DrawText("Load Balancer", centerX, centerY - 60, paint);

        // App services
        canvas.DrawRect(centerX - 100, centerY - 20, 60, 40, paint);
        canvas.DrawRect(centerX + 40, centerY - 20, 60, 40, paint);
        canvas.DrawText("App Service", centerX - 70, centerY + 5, paint);
        canvas.DrawText("App Service", centerX + 70, centerY + 5, paint);

        // Database
        canvas.DrawCircle(centerX, centerY + 60, 30, paint);
        canvas.DrawText("Database", centerX, centerY + 85, paint);

        // Connection lines
        paint.StrokeWidth = 2;
        // LB to apps
        canvas.DrawLine(centerX - 30, centerY - 50, centerX - 70, centerY - 20, paint);
        canvas.DrawLine(centerX + 30, centerY - 50, centerX + 70, centerY - 20, paint);
        // Apps to DB
        canvas.DrawLine(centerX - 70, centerY + 20, centerX - 15, centerY + 45, paint);
        canvas.DrawLine(centerX + 70, centerY + 20, centerX + 15, centerY + 45, paint);

        // Title
        paint.Color = SKColors.Yellow;
        paint.TextSize = 18;
        paint.TextAlign = SKTextAlign.Center;
        canvas.DrawText("Cloud Architecture Blueprint", centerX, 35, paint);
    }

    /// <summary>
    /// Placeholder implementations for remaining cloud drawing methods
    /// </summary>
    private void DrawPaaSPlatform(SKCanvas canvas, int width, int height, ImageStyle style) => DrawFloatingCloudCastles(canvas, width, height, style);
    private void DrawSaaSAppsInClouds(SKCanvas canvas, int width, int height, ImageStyle style) => DrawFloatingCloudCastles(canvas, width, height, style);
    private void DrawAzureSkyKingdom(SKCanvas canvas, int width, int height, ImageStyle style) => DrawFloatingCloudCastles(canvas, width, height, style);
    private void DrawAmazonCloudEmpire(SKCanvas canvas, int width, int height, ImageStyle style) => DrawFloatingCloudCastles(canvas, width, height, style);
    private void DrawGoogleSmartClouds(SKCanvas canvas, int width, int height, ImageStyle style) => DrawFloatingCloudCastles(canvas, width, height, style);
    private void DrawVirtualMachineClones(SKCanvas canvas, int width, int height, ImageStyle style) => DrawIaaSBuildingBlocks(canvas, width, height, style);
    private void DrawContainersInSky(SKCanvas canvas, int width, int height, ImageStyle style) => DrawIaaSBuildingBlocks(canvas, width, height, style);
    private void DrawServerlessFunctionWands(SKCanvas canvas, int width, int height, ImageStyle style) => DrawFloatingCloudCastles(canvas, width, height, style);
    private void DrawCloudStorageVaults(SKCanvas canvas, int width, int height, ImageStyle style) => DrawFloatingCloudCastles(canvas, width, height, style);
    private void DrawCloudVirtualHighways(SKCanvas canvas, int width, int height, ImageStyle style) => DrawIaaSBuildingBlocks(canvas, width, height, style);
    private void DrawCloudScriptingWand(SKCanvas canvas, int width, int height, ImageStyle style) => DrawFloatingCloudCastles(canvas, width, height, style);
    private void DrawCloudSecurityShields(SKCanvas canvas, int width, int height, ImageStyle style) => DrawIaaSBuildingBlocks(canvas, width, height, style);
    private void DrawCloudFogTroubleshootingDetective(SKCanvas canvas, int width, int height, ImageStyle style) => DrawFloatingCloudCastles(canvas, width, height, style);
    private void DrawHybridCloudBridge(SKCanvas canvas, int width, int height, ImageStyle style) => DrawIaaSBuildingBlocks(canvas, width, height, style);
    private void DrawCloudCostWallet(SKCanvas canvas, int width, int height, ImageStyle style) => DrawFloatingCloudCastles(canvas, width, height, style);
    private void DrawEdgeComputingClouds(SKCanvas canvas, int width, int height, ImageStyle style) => DrawIaaSBuildingBlocks(canvas, width, height, style);
    private void DrawCloudMasteryCrown(SKCanvas canvas, int width, int height, ImageStyle style) => DrawCloudArchitectureBlueprints(canvas, width, height, style);

    #endregion

    #region Module 9: Advanced Alchemy - Protocol Mixing Drawing Methods

    /// <summary>
    /// Draw a magical protocol alchemy cauldron with bubbling ingredients
    /// </summary>
    private void DrawProtocolAlchemyCauldron(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        using var paint = new SKPaint { IsAntialias = true };

        var centerX = width / 2f;
        var centerY = height / 2f;

        // Draw cauldron
        paint.Color = SKColors.DarkSlateGray;
        paint.Style = SKPaintStyle.Fill;
        canvas.DrawOval(centerX - 80, centerY + 20, 160, 100, paint);

        // Draw cauldron rim
        paint.Color = SKColors.Gray;
        canvas.DrawOval(centerX - 85, centerY + 15, 170, 20, paint);

        // Draw bubbling potion
        paint.Color = SKColors.MediumPurple;
        paint.Style = SKPaintStyle.Fill;
        canvas.DrawOval(centerX - 75, centerY + 20, 150, 90, paint);

        // Draw protocol ingredient bubbles
        var random = new Random(42); // Fixed seed for consistency
        paint.Color = SKColors.Cyan;
        for (int i = 0; i < 8; i++)
        {
            var bubbleX = centerX + (random.Next(-60, 60));
            var bubbleY = centerY + (random.Next(-30, 60));
            canvas.DrawCircle(bubbleX, bubbleY, random.Next(5, 15), paint);
        }

        // Draw magical sparkles
        paint.Color = SKColors.Gold;
        for (int i = 0; i < 12; i++)
        {
            var sparkleX = centerX + (random.Next(-100, 100));
            var sparkleY = centerY + (random.Next(-80, -20));
            canvas.DrawCircle(sparkleX, sparkleY, 2, paint);
        }

        // Draw protocol labels floating around
        paint.Color = SKColors.White;
        paint.Style = SKPaintStyle.Stroke;
        paint.StrokeWidth = 2;
        canvas.DrawOval(centerX - 120, centerY - 60, 40, 20, paint);
        canvas.DrawOval(centerX + 80, centerY - 40, 40, 20, paint);
        canvas.DrawOval(centerX - 100, centerY + 80, 40, 20, paint);
    }

    /// <summary>
    /// Draw a web security potion with HTTPS lock floating above
    /// </summary>
    private void DrawWebSecurityPotion(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        using var paint = new SKPaint { IsAntialias = true };

        var centerX = width / 2f;
        var centerY = height / 2f;

        // Draw potion bottle
        paint.Color = SKColors.DarkBlue;
        paint.Style = SKPaintStyle.Fill;
        var bottlePath = new SKPath();
        bottlePath.MoveTo(centerX - 30, centerY + 60);
        bottlePath.LineTo(centerX - 30, centerY - 20);
        bottlePath.LineTo(centerX - 20, centerY - 30);
        bottlePath.LineTo(centerX + 20, centerY - 30);
        bottlePath.LineTo(centerX + 30, centerY - 20);
        bottlePath.LineTo(centerX + 30, centerY + 60);
        bottlePath.Close();
        canvas.DrawPath(bottlePath, paint);

        // Draw glowing potion liquid
        paint.Color = SKColors.LimeGreen;
        canvas.DrawOval(centerX - 25, centerY - 10, 50, 65, paint);

        // Draw bottle cork
        paint.Color = SKColors.SaddleBrown;
        canvas.DrawOval(centerX - 18, centerY - 35, 36, 15, paint);

        // Draw floating HTTPS lock
        paint.Color = SKColors.Gold;
        paint.Style = SKPaintStyle.Fill;
        canvas.DrawRect(centerX - 15, centerY - 80, 30, 20, paint);
        
        // Lock shackle
        paint.Style = SKPaintStyle.Stroke;
        paint.StrokeWidth = 3;
        canvas.DrawArc(centerX - 10, centerY - 95, 20, 20, 0, 180, false, paint);

        // Draw security glow
        paint.Color = SKColors.Yellow;
        paint.Style = SKPaintStyle.Stroke;
        paint.StrokeWidth = 1;
        for (int i = 1; i <= 3; i++)
        {
            canvas.DrawCircle(centerX, centerY - 70, 20 + (i * 8), paint);
        }
    }

    /// <summary>
    /// Draw file transfer potion bottles with security armor
    /// </summary>
    private void DrawFileTransferPotions(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        using var paint = new SKPaint { IsAntialias = true };

        var centerX = width / 2f;
        var centerY = height / 2f;

        // Draw FTP bottle (plain)
        paint.Color = SKColors.LightBlue;
        paint.Style = SKPaintStyle.Fill;
        canvas.DrawOval(centerX - 80, centerY - 20, 40, 80, paint);
        
        // FTP label
        paint.Color = SKColors.Navy;
        canvas.DrawRect(centerX - 75, centerY - 10, 30, 15, paint);

        // Draw SFTP bottle (armored)
        paint.Color = SKColors.DarkGreen;
        canvas.DrawOval(centerX + 40, centerY - 20, 40, 80, paint);

        // Draw armor plating on SFTP bottle
        paint.Color = SKColors.Silver;
        paint.Style = SKPaintStyle.Stroke;
        paint.StrokeWidth = 3;
        for (int i = 0; i < 4; i++)
        {
            canvas.DrawRect(centerX + 45, centerY - 15 + (i * 15), 30, 10, paint);
        }

        // Draw shield emblem on SFTP bottle
        paint.Color = SKColors.Gold;
        paint.Style = SKPaintStyle.Fill;
        var shieldPath = new SKPath();
        shieldPath.MoveTo(centerX + 60, centerY - 30);
        shieldPath.LineTo(centerX + 70, centerY - 25);
        shieldPath.LineTo(centerX + 70, centerY - 10);
        shieldPath.LineTo(centerX + 60, centerY - 5);
        shieldPath.LineTo(centerX + 50, centerY - 10);
        shieldPath.LineTo(centerX + 50, centerY - 25);
        shieldPath.Close();
        canvas.DrawPath(shieldPath, paint);
    }

    /// <summary>
    /// Placeholder implementations for remaining protocol alchemy drawing methods
    /// </summary>
    private void DrawEmailProtocolElixir(SKCanvas canvas, int width, int height, ImageStyle style) => DrawProtocolAlchemyCauldron(canvas, width, height, style);
    private void DrawDNSNameResolverPotion(SKCanvas canvas, int width, int height, ImageStyle style) => DrawProtocolAlchemyCauldron(canvas, width, height, style);
    private void DrawDHCPAutoAssignCauldron(SKCanvas canvas, int width, int height, ImageStyle style) => DrawProtocolAlchemyCauldron(canvas, width, height, style);
    private void DrawVPNTunnelElixir(SKCanvas canvas, int width, int height, ImageStyle style) => DrawWebSecurityPotion(canvas, width, height, style);
    private void DrawQoSPriorityPotion(SKCanvas canvas, int width, int height, ImageStyle style) => DrawProtocolAlchemyCauldron(canvas, width, height, style);
    private void DrawSNMPMonitoringCrystal(SKCanvas canvas, int width, int height, ImageStyle style) => DrawProtocolAlchemyCauldron(canvas, width, height, style);
    private void DrawVoIPCommunicationBubbles(SKCanvas canvas, int width, int height, ImageStyle style) => DrawProtocolAlchemyCauldron(canvas, width, height, style);
    private void DrawScriptingProtocolWand(SKCanvas canvas, int width, int height, ImageStyle style) => DrawWebSecurityPotion(canvas, width, height, style);
    private void DrawMultiprotocolHybridElixir(SKCanvas canvas, int width, int height, ImageStyle style) => DrawFileTransferPotions(canvas, width, height, style);
    private void DrawSecurityProtocolShield(SKCanvas canvas, int width, int height, ImageStyle style) => DrawWebSecurityPotion(canvas, width, height, style);
    private void DrawProtocolTroubleshootingDetective(SKCanvas canvas, int width, int height, ImageStyle style) => DrawProtocolAlchemyCauldron(canvas, width, height, style);
    private void DrawEmergingProtocolsFuture(SKCanvas canvas, int width, int height, ImageStyle style) => DrawWebSecurityPotion(canvas, width, height, style);
    private void DrawIoTMiniaturePotions(SKCanvas canvas, int width, int height, ImageStyle style) => DrawFileTransferPotions(canvas, width, height, style);
    private void DrawCloudProtocolCauldron(SKCanvas canvas, int width, int height, ImageStyle style) => DrawProtocolAlchemyCauldron(canvas, width, height, style);
    private void DrawProtocolStackLayers(SKCanvas canvas, int width, int height, ImageStyle style) => DrawFileTransferPotions(canvas, width, height, style);
    private void DrawAdvancedProtocolLaboratory(SKCanvas canvas, int width, int height, ImageStyle style) => DrawProtocolAlchemyCauldron(canvas, width, height, style);
    private void DrawMasterAlchemistPotion(SKCanvas canvas, int width, int height, ImageStyle style) => DrawWebSecurityPotion(canvas, width, height, style);

    #endregion

    #region Module 10: Mastery Mayhem - Engineer Extraordinaire Drawing Methods

    /// <summary>
    /// Draw a comprehensive network empire cartoon showing the full journey
    /// </summary>
    private void DrawNetworkEmpireCartoon(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        using var paint = new SKPaint { IsAntialias = true };

        var centerX = width / 2f;
        var centerY = height / 2f;

        // Draw foundation layer (cables and connections)
        paint.Color = SKColors.DarkSlateGray;
        paint.Style = SKPaintStyle.Stroke;
        paint.StrokeWidth = 4;
        
        // Draw interconnected network lines
        for (int i = 0; i < 6; i++)
        {
            var angle = (i * 60) * Math.PI / 180;
            var startX = centerX + (float)(50 * Math.Cos(angle));
            var startY = centerY + (float)(50 * Math.Sin(angle));
            var endX = centerX + (float)(150 * Math.Cos(angle));
            var endY = centerY + (float)(150 * Math.Sin(angle));
            
            canvas.DrawLine(startX, startY, endX, endY, paint);
        }

        // Draw central network core
        paint.Color = SKColors.Gold;
        paint.Style = SKPaintStyle.Fill;
        canvas.DrawCircle(centerX, centerY, 30, paint);

        // Draw network devices around the perimeter
        var devicePositions = new[]
        {
            (centerX - 120, centerY - 60, "Router"),
            (centerX + 120, centerY - 60, "Switch"),
            (centerX - 120, centerY + 60, "Firewall"),
            (centerX + 120, centerY + 60, "Server"),
            (centerX, centerY - 120, "Cloud"),
            (centerX, centerY + 120, "Wireless")
        };

        foreach (var (x, y, type) in devicePositions)
        {
            // Draw device
            paint.Color = SKColors.RoyalBlue;
            canvas.DrawRect(x - 15, y - 10, 30, 20, paint);
            
            // Draw device crown (mastery symbol)
            paint.Color = SKColors.Gold;
            canvas.DrawRect(x - 8, y - 20, 16, 8, paint);
            
            // Add sparkles around devices
            paint.Color = SKColors.Yellow;
            for (int i = 0; i < 4; i++)
            {
                var sparkleX = x + (float)(25 * Math.Cos(i * 90 * Math.PI / 180));
                var sparkleY = y + (float)(25 * Math.Sin(i * 90 * Math.PI / 180));
                canvas.DrawCircle(sparkleX, sparkleY, 2, paint);
            }
        }

        // Draw mastery aura
        paint.Color = SKColors.Purple;
        paint.Style = SKPaintStyle.Stroke;
        paint.StrokeWidth = 2;
        for (int i = 1; i <= 3; i++)
        {
            canvas.DrawCircle(centerX, centerY, 50 + (i * 20), paint);
        }
    }

    /// <summary>
    /// Draw hardware heroes with command staffs and royal insignia
    /// </summary>
    private void DrawHardwareHeroesCommand(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        using var paint = new SKPaint { IsAntialias = true };

        var centerX = width / 2f;
        var centerY = height / 2f;

        // Draw the command center platform
        paint.Color = SKColors.DarkGray;
        paint.Style = SKPaintStyle.Fill;
        canvas.DrawOval(centerX - 140, centerY + 80, 280, 40, paint);

        // Hardware heroes positions
        var heroes = new[]
        {
            (centerX - 100, centerY, "NIC", SKColors.Green),
            (centerX - 50, centerY, "Switch", SKColors.Blue),
            (centerX, centerY, "Router", SKColors.Red),
            (centerX + 50, centerY, "Firewall", SKColors.Purple),
            (centerX + 100, centerY, "Server", SKColors.Orange)
        };

        foreach (var (x, y, name, color) in heroes)
        {
            // Draw hero body
            paint.Color = color;
            paint.Style = SKPaintStyle.Fill;
            canvas.DrawRect(x - 15, y - 10, 30, 40, paint);

            // Draw hero crown
            paint.Color = SKColors.Gold;
            var crownPath = new SKPath();
            crownPath.MoveTo(x - 12, y - 15);
            crownPath.LineTo(x - 8, y - 25);
            crownPath.LineTo(x - 4, y - 20);
            crownPath.LineTo(x, y - 30);
            crownPath.LineTo(x + 4, y - 20);
            crownPath.LineTo(x + 8, y - 25);
            crownPath.LineTo(x + 12, y - 15);
            crownPath.Close();
            canvas.DrawPath(crownPath, paint);

            // Draw command staff
            paint.Color = SKColors.Brown;
            canvas.DrawRect(x - 2, y + 30, 4, 40, paint);
            
            // Draw staff orb
            paint.Color = SKColors.Cyan;
            canvas.DrawCircle(x, y + 25, 8, paint);

            // Draw power aura
            paint.Color = color;
            paint.Style = SKPaintStyle.Stroke;
            paint.StrokeWidth = 2;
            canvas.DrawCircle(x, y + 15, 25, paint);
        }

        // Draw command energy connections
        paint.Color = SKColors.Yellow;
        paint.Style = SKPaintStyle.Stroke;
        paint.StrokeWidth = 3;
        for (int i = 0; i < heroes.Length - 1; i++)
        {
            canvas.DrawLine(heroes[i].Item1, heroes[i].Item2 + 25, 
                          heroes[i + 1].Item1, heroes[i + 1].Item2 + 25, paint);
        }
    }

    /// <summary>
    /// Draw an IP address forming a royal crown
    /// </summary>
    private void DrawIPAddressRoyalCrown(SKCanvas canvas, int width, int height, ImageStyle style)
    {
        using var paint = new SKPaint { IsAntialias = true };

        var centerX = width / 2f;
        var centerY = height / 2f;

        // Draw crown base
        paint.Color = SKColors.Gold;
        paint.Style = SKPaintStyle.Fill;
        canvas.DrawRect(centerX - 60, centerY + 10, 120, 20, paint);

        // Draw crown spikes with IP address segments
        var ipSegments = new[] { "192", "168", "1", "1" };
        var spikePositions = new float[] { centerX - 45, centerX - 15, centerX + 15, centerX + 45 };

        for (int i = 0; i < 4; i++)
        {
            // Draw spike
            paint.Color = SKColors.Gold;
            var spikePath = new SKPath();
            var x = spikePositions[i];
            spikePath.MoveTo(x - 10, centerY + 10);
            spikePath.LineTo(x, centerY - 30 - (i % 2 * 10)); // Varying heights
            spikePath.LineTo(x + 10, centerY + 10);
            spikePath.Close();
            canvas.DrawPath(spikePath, paint);

            // Draw IP segment in the spike
            paint.Color = SKColors.DarkBlue;
            paint.Style = SKPaintStyle.Fill;
            canvas.DrawRect(x - 8, centerY - 5, 16, 10, paint);
            
            // Add glowing effect
            paint.Color = SKColors.Cyan;
            paint.Style = SKPaintStyle.Stroke;
            paint.StrokeWidth = 2;
            canvas.DrawRect(x - 8, centerY - 5, 16, 10, paint);
        }

        // Draw royal jewels (subnet indicators)
        paint.Color = SKColors.Red;
        paint.Style = SKPaintStyle.Fill;
        for (int i = 0; i < 3; i++)
        {
            canvas.DrawCircle(centerX - 30 + (i * 30), centerY, 5, paint);
        }

        // Draw mastery aura around crown
        paint.Color = SKColors.Purple;
        paint.Style = SKPaintStyle.Stroke;
        paint.StrokeWidth = 1;
        for (int i = 1; i <= 3; i++)
        {
            canvas.DrawOval(centerX - 70 - (i * 10), centerY - 40 - (i * 5), 140 + (i * 20), 80 + (i * 10), paint);
        }
    }

    /// <summary>
    /// Placeholder implementations for remaining mastery drawing methods
    /// </summary>
    private void DrawUltimateMagicalWand(SKCanvas canvas, int width, int height, ImageStyle style) => DrawNetworkEmpireCartoon(canvas, width, height, style);
    private void DrawRoutingTableThrone(SKCanvas canvas, int width, int height, ImageStyle style) => DrawHardwareHeroesCommand(canvas, width, height, style);
    private void DrawArmoredSecurityCastle(SKCanvas canvas, int width, int height, ImageStyle style) => DrawNetworkEmpireCartoon(canvas, width, height, style);
    private void DrawWirelessWaveCrown(SKCanvas canvas, int width, int height, ImageStyle style) => DrawIPAddressRoyalCrown(canvas, width, height, style);
    private void DrawCloudHeavenlyThrone(SKCanvas canvas, int width, int height, ImageStyle style) => DrawHardwareHeroesCommand(canvas, width, height, style);
    private void DrawUltimateProtocolPotion(SKCanvas canvas, int width, int height, ImageStyle style) => DrawNetworkEmpireCartoon(canvas, width, height, style);
    private void DrawIntegratedNetworkFusion(SKCanvas canvas, int width, int height, ImageStyle style) => DrawNetworkEmpireCartoon(canvas, width, height, style);
    private void DrawAutomationCodeCastle(SKCanvas canvas, int width, int height, ImageStyle style) => DrawHardwareHeroesCommand(canvas, width, height, style);
    private void DrawNetworkArchitecturalBlueprints(SKCanvas canvas, int width, int height, ImageStyle style) => DrawNetworkEmpireCartoon(canvas, width, height, style);
    private void DrawSecurityAuditShield(SKCanvas canvas, int width, int height, ImageStyle style) => DrawIPAddressRoyalCrown(canvas, width, height, style);
    private void DrawHybridCloudWireless(SKCanvas canvas, int width, int height, ImageStyle style) => DrawHardwareHeroesCommand(canvas, width, height, style);
    private void DrawTroubleshootingDetectiveCrown(SKCanvas canvas, int width, int height, ImageStyle style) => DrawIPAddressRoyalCrown(canvas, width, height, style);
    private void DrawCertificationChaos(SKCanvas canvas, int width, int height, ImageStyle style) => DrawNetworkEmpireCartoon(canvas, width, height, style);
    private void DrawRealWorldImplementation(SKCanvas canvas, int width, int height, ImageStyle style) => DrawHardwareHeroesCommand(canvas, width, height, style);
    private void DrawEthicsBalancedScales(SKCanvas canvas, int width, int height, ImageStyle style) => DrawIPAddressRoyalCrown(canvas, width, height, style);
    private void DrawFuturisticNetworkAI(SKCanvas canvas, int width, int height, ImageStyle style) => DrawNetworkEmpireCartoon(canvas, width, height, style);
    private void DrawSupremeMasteryTrophy(SKCanvas canvas, int width, int height, ImageStyle style) => DrawHardwareHeroesCommand(canvas, width, height, style);

    #endregion
}