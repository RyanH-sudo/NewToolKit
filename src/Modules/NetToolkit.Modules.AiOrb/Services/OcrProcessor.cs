using Microsoft.Extensions.Logging;
using NetToolkit.Modules.AiOrb.Interfaces;
using NetToolkit.Modules.AiOrb.Models;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Tesseract;

namespace NetToolkit.Modules.AiOrb.Services;

/// <summary>
/// OCR processor for screenshot analysis and text extraction
/// Transforms visual information into actionable insights using Tesseract OCR
/// </summary>
public class OcrProcessor : IOcrProcessor
{
    private readonly ILogger<OcrProcessor> _logger;
    private readonly AiClientService _aiClient;
    private TesseractEngine? _tesseractEngine;
    private readonly string _tessDataPath;
    private readonly SemaphoreSlim _processingLock;
    private bool _isInitialized;

    public OcrProcessor(ILogger<OcrProcessor> logger, AiClientService aiClient)
    {
        _logger = logger;
        _aiClient = aiClient;
        _processingLock = new SemaphoreSlim(1, 1);
        
        // Set up Tesseract data path
        _tessDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tessdata");
        
        _logger.LogInformation("OCR processor initialized - Orb optical sensors calibrated! 👁️");
    }

    /// <summary>
    /// Initialize OCR engine with required data files
    /// </summary>
    public async Task<bool> InitializeAsync()
    {
        try
        {
            await _processingLock.WaitAsync();
            
            if (_isInitialized)
            {
                return true;
            }

            // Create tessdata directory if it doesn't exist
            Directory.CreateDirectory(_tessDataPath);
            
            // Check if English language data exists, download if needed
            await EnsureLanguageDataExistsAsync("eng");
            
            // Initialize Tesseract engine
            _tesseractEngine = new TesseractEngine(_tessDataPath, "eng", EngineMode.Default);
            
            // Configure OCR settings for better accuracy
            _tesseractEngine.SetVariable("tessedit_char_whitelist", 
                "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789 .,!?-()[]{}:;\"'@#$%^&*+=<>/\\|`~");
            
            _isInitialized = true;
            
            _logger.LogInformation("OCR engine initialized successfully - Orb vision enhanced! 🔍");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "OCR engine initialization failed - Orb vision clouded! ☁️");
            return false;
        }
        finally
        {
            _processingLock.Release();
        }
    }

    /// <summary>
    /// Extract text from image data using OCR technology
    /// </summary>
    public async Task<OcrTextResult> ExtractTextAsync(byte[] imageData, string language = "eng")
    {
        var startTime = DateTime.UtcNow;
        
        try
        {
            if (!_isInitialized && !await InitializeAsync())
            {
                return new OcrTextResult
                {
                    Text = "OCR engine not initialized - Orb vision offline!",
                    Confidence = 0,
                    ProcessingTime = DateTime.UtcNow - startTime
                };
            }

            await _processingLock.WaitAsync();

            using var memoryStream = new MemoryStream(imageData);
            using var image = Image.FromStream(memoryStream);
            using var bitmap = new Bitmap(image);
            
            // Preprocess image for better OCR accuracy
            var preprocessedBitmap = PreprocessImageForOcr(bitmap);
            
            // Perform OCR
            using var pix = PixConverter.ToPix(preprocessedBitmap);
            using var page = _tesseractEngine!.Process(pix);
            
            var extractedText = page.GetText();
            var confidence = page.GetMeanConfidence();
            
            var result = new OcrTextResult
            {
                Text = extractedText.Trim(),
                Confidence = confidence,
                ProcessingTime = DateTime.UtcNow - startTime,
                Language = language,
                Regions = ExtractTextRegions(page)
            };
            
            _logger.LogInformation("OCR extraction completed - Orb decoded {CharCount} characters with {Confidence}% confidence! 📖", 
                result.Text.Length, Math.Round(confidence * 100, 1));
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "OCR text extraction failed - Orb optical circuits overloaded! ⚡");
            return new OcrTextResult
            {
                Text = $"OCR extraction failed: {ex.Message}",
                Confidence = 0,
                ProcessingTime = DateTime.UtcNow - startTime
            };
        }
        finally
        {
            _processingLock.Release();
        }
    }

    /// <summary>
    /// Capture screenshot from screen coordinates
    /// </summary>
    public async Task<byte[]> CaptureScreenshotAsync(ScreenRegion? region = null)
    {
        try
        {
            Rectangle captureRect;
            
            if (region == null || region.IsFullScreen)
            {
                // Capture full screen
                captureRect = new Rectangle(0, 0, 
                    SystemInformation.VirtualScreen.Width, 
                    SystemInformation.VirtualScreen.Height);
            }
            else
            {
                captureRect = new Rectangle(region.X, region.Y, region.Width, region.Height);
            }

            using var bitmap = new Bitmap(captureRect.Width, captureRect.Height, PixelFormat.Format32bppArgb);
            using var graphics = Graphics.FromImage(bitmap);
            
            graphics.CopyFromScreen(captureRect.X, captureRect.Y, 0, 0, captureRect.Size, CopyPixelOperation.SourceCopy);
            
            using var memoryStream = new MemoryStream();
            bitmap.Save(memoryStream, ImageFormat.Png);
            
            var screenshotData = memoryStream.ToArray();
            
            _logger.LogInformation("Screenshot captured - Orb visual snapshot secured! 📸 Size: {Size} bytes", 
                screenshotData.Length);
            
            return screenshotData;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Screenshot capture failed - Orb camera malfunction! 📷");
            return Array.Empty<byte>();
        }
    }

    /// <summary>
    /// Process and analyze extracted text for network-related insights
    /// </summary>
    public async Task<TextAnalysisResult> AnalyzeExtractedTextAsync(string extractedText, OcrAnalysisType analysisType)
    {
        try
        {
            var insights = await _aiClient.AnalyzeNetworkingContentAsync(extractedText);
            
            var result = new TextAnalysisResult
            {
                Summary = GenerateTextSummary(extractedText, analysisType),
                Insights = insights,
                Suggestions = GenerateActionSuggestions(extractedText, analysisType),
                KeywordFrequency = AnalyzeKeywordFrequency(extractedText),
                SentimentAnalysis = AnalyzeSentiment(extractedText)
            };
            
            _logger.LogInformation("Text analysis completed - Orb wisdom extracted! 🧠 Insights: {InsightCount}", 
                result.Insights.Count);
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Text analysis failed - Orb analysis circuits confused! 🤔");
            return new TextAnalysisResult
            {
                Summary = $"Analysis failed: {ex.Message}",
                Insights = new List<NetworkingInsight>()
            };
        }
    }

    /// <summary>
    /// Get supported OCR languages
    /// </summary>
    public async Task<IEnumerable<string>> GetSupportedLanguagesAsync()
    {
        // For now, return commonly supported languages
        // In a full implementation, this would check available .traineddata files
        var supportedLanguages = new[]
        {
            "eng", // English
            "deu", // German  
            "fra", // French
            "spa", // Spanish
            "ita", // Italian
            "por", // Portuguese
            "rus", // Russian
            "chi_sim", // Chinese Simplified
            "jpn", // Japanese
            "kor"  // Korean
        };
        
        return supportedLanguages;
    }

    /// <summary>
    /// Cleanup OCR engine resources
    /// </summary>
    public async Task DisposeAsync()
    {
        await _processingLock.WaitAsync();
        try
        {
            _tesseractEngine?.Dispose();
            _tesseractEngine = null;
            _isInitialized = false;
            
            _logger.LogInformation("OCR processor disposed - Orb optical systems powered down! 😴");
        }
        finally
        {
            _processingLock.Release();
            _processingLock.Dispose();
        }
    }

    /// <summary>
    /// Ensure language data files exist for Tesseract
    /// </summary>
    private async Task EnsureLanguageDataExistsAsync(string language)
    {
        var languageFile = Path.Combine(_tessDataPath, $"{language}.traineddata");
        
        if (File.Exists(languageFile))
        {
            return;
        }

        _logger.LogInformation("Downloading OCR language data for {Language} - Orb learning new dialects! 🌍", language);
        
        try
        {
            // Download from GitHub Tesseract repository
            var downloadUrl = $"https://github.com/tesseract-ocr/tessdata/raw/main/{language}.traineddata";
            
            using var httpClient = new HttpClient();
            var languageData = await httpClient.GetByteArrayAsync(downloadUrl);
            
            await File.WriteAllBytesAsync(languageFile, languageData);
            
            _logger.LogInformation("OCR language data downloaded successfully - Orb linguistic enhancement complete! 📚");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to download OCR language data for {Language} - Orb will use available dialects! 🗣️", language);
            
            // Create a minimal placeholder file to prevent repeated download attempts
            await File.WriteAllTextAsync(languageFile + ".error", $"Download failed: {ex.Message}");
        }
    }

    /// <summary>
    /// Preprocess image for better OCR accuracy
    /// </summary>
    private static Bitmap PreprocessImageForOcr(Bitmap originalBitmap)
    {
        // Create a copy to avoid modifying the original
        var processedBitmap = new Bitmap(originalBitmap.Width, originalBitmap.Height);
        
        using (var graphics = Graphics.FromImage(processedBitmap))
        {
            // Improve contrast and brightness
            var imageAttributes = new ImageAttributes();
            
            // Adjust contrast and brightness for better OCR
            float[][] colorMatrixElements = {
                new float[] {1.2f,  0,     0,     0,  0},  // red scaling factor
                new float[] {0,     1.2f,  0,     0,  0},  // green scaling factor  
                new float[] {0,     0,     1.2f,  0,  0},  // blue scaling factor
                new float[] {0,     0,     0,     1,  0},  // alpha scaling factor
                new float[] {0.05f, 0.05f, 0.05f, 0,  1}   // brightness adjustment
            };

            var colorMatrix = new ColorMatrix(colorMatrixElements);
            imageAttributes.SetColorMatrix(colorMatrix);
            
            graphics.DrawImage(originalBitmap, new Rectangle(0, 0, originalBitmap.Width, originalBitmap.Height),
                0, 0, originalBitmap.Width, originalBitmap.Height, GraphicsUnit.Pixel, imageAttributes);
        }
        
        return processedBitmap;
    }

    /// <summary>
    /// Extract text regions from OCR results
    /// </summary>
    private static List<TextRegion> ExtractTextRegions(Page page)
    {
        var regions = new List<TextRegion>();
        
        try
        {
            using var iterator = page.GetIterator();
            iterator.Begin();

            do
            {
                if (iterator.TryGetBoundingBox(PageIteratorLevel.Word, out var bounds))
                {
                    var text = iterator.GetText(PageIteratorLevel.Word);
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        regions.Add(new TextRegion
                        {
                            BoundingBox = new Rectangle(bounds.X1, bounds.Y1, 
                                bounds.X2 - bounds.X1, bounds.Y2 - bounds.Y1),
                            Text = text.Trim(),
                            Confidence = iterator.GetConfidence(PageIteratorLevel.Word) / 100.0
                        });
                    }
                }
            }
            while (iterator.Next(PageIteratorLevel.Word));
        }
        catch (Exception)
        {
            // If region extraction fails, return empty list
            // OCR text extraction may still work without region details
        }

        return regions;
    }

    /// <summary>
    /// Generate summary based on analysis type
    /// </summary>
    private static string GenerateTextSummary(string text, OcrAnalysisType analysisType)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return "No text content detected in image.";
        }

        var wordCount = text.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
        var summary = $"Extracted {wordCount} words from {analysisType.ToString().ToLowerInvariant()} content.";

        return analysisType switch
        {
            OcrAnalysisType.ErrorMessage => $"{summary} Contains potential error information for troubleshooting.",
            OcrAnalysisType.NetworkDiagram => $"{summary} Appears to contain network topology or configuration details.",
            OcrAnalysisType.Configuration => $"{summary} Contains configuration or settings information.",
            OcrAnalysisType.LogFile => $"{summary} Contains log file or diagnostic information.",
            OcrAnalysisType.CommandOutput => $"{summary} Contains command-line or terminal output.",
            OcrAnalysisType.Documentation => $"{summary} Contains documentation or instructional content.",
            _ => summary
        };
    }

    /// <summary>
    /// Generate action suggestions based on extracted text
    /// </summary>
    private static List<ActionSuggestion> GenerateActionSuggestions(string text, OcrAnalysisType analysisType)
    {
        var suggestions = new List<ActionSuggestion>();
        
        if (string.IsNullOrWhiteSpace(text))
        {
            return suggestions;
        }

        var lowerText = text.ToLowerInvariant();

        // Common networking keywords and their suggestions
        if (lowerText.Contains("error") || lowerText.Contains("failed") || lowerText.Contains("timeout"))
        {
            suggestions.Add(new ActionSuggestion
            {
                Action = "Investigate Error",
                Rationale = "Error indicators detected in the image text",
                Priority = 3,
                CommandSuggestion = "Check logs and diagnostic information"
            });
        }

        if (lowerText.Contains("ip") || lowerText.Contains("subnet") || lowerText.Contains("network"))
        {
            suggestions.Add(new ActionSuggestion
            {
                Action = "Verify Network Configuration",
                Rationale = "Network configuration elements detected",
                Priority = 2,
                CommandSuggestion = "Validate IP addresses and network settings"
            });
        }

        if (lowerText.Contains("password") || lowerText.Contains("key") || lowerText.Contains("secret"))
        {
            suggestions.Add(new ActionSuggestion
            {
                Action = "Security Review Required",
                Rationale = "Potential sensitive information detected in image",
                Priority = 4,
                CommandSuggestion = "Ensure sensitive data is properly secured"
            });
        }

        return suggestions;
    }

    /// <summary>
    /// Analyze keyword frequency in text
    /// </summary>
    private static Dictionary<string, int> AnalyzeKeywordFrequency(string text)
    {
        var frequency = new Dictionary<string, int>();
        
        if (string.IsNullOrWhiteSpace(text))
        {
            return frequency;
        }

        var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                       .Select(w => w.Trim().ToLowerInvariant())
                       .Where(w => w.Length > 3); // Only count words longer than 3 characters

        foreach (var word in words)
        {
            frequency[word] = frequency.GetValueOrDefault(word, 0) + 1;
        }

        return frequency.Where(kvp => kvp.Value > 1).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }

    /// <summary>
    /// Analyze sentiment of extracted text
    /// </summary>
    private static string AnalyzeSentiment(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return "Neutral";
        }

        var lowerText = text.ToLowerInvariant();
        
        var positiveWords = new[] { "success", "complete", "working", "connected", "active", "healthy" };
        var negativeWords = new[] { "error", "failed", "timeout", "disconnected", "inactive", "broken" };
        
        var positiveCount = positiveWords.Count(word => lowerText.Contains(word));
        var negativeCount = negativeWords.Count(word => lowerText.Contains(word));
        
        if (positiveCount > negativeCount)
            return "Positive";
        if (negativeCount > positiveCount)
            return "Negative";
        
        return "Neutral";
    }
}

/// <summary>
/// System information helper for screen capture
/// </summary>
internal static class SystemInformation
{
    public static Rectangle VirtualScreen
    {
        get
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return new Rectangle(
                    GetSystemMetrics(76), // SM_XVIRTUALSCREEN
                    GetSystemMetrics(77), // SM_YVIRTUALSCREEN  
                    GetSystemMetrics(78), // SM_CXVIRTUALSCREEN
                    GetSystemMetrics(79)  // SM_CYVIRTUALSCREEN
                );
            }
            
            // Default fallback for non-Windows platforms
            return new Rectangle(0, 0, 1920, 1080);
        }
    }

    [DllImport("user32.dll")]
    private static extern int GetSystemMetrics(int nIndex);
}