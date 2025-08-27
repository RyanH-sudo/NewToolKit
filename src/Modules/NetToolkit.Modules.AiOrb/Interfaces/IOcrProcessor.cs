using NetToolkit.Modules.AiOrb.Models;

namespace NetToolkit.Modules.AiOrb.Interfaces;

/// <summary>
/// OCR processing interface for screenshot analysis and text extraction
/// Transforms visual information into actionable insights
/// </summary>
public interface IOcrProcessor
{
    /// <summary>
    /// Extract text from image data using OCR technology
    /// </summary>
    /// <param name="imageData">Raw image bytes from screenshot or file</param>
    /// <param name="language">Language code for OCR processing (default: eng)</param>
    /// <returns>Extracted text with confidence metrics</returns>
    Task<OcrTextResult> ExtractTextAsync(byte[] imageData, string language = "eng");

    /// <summary>
    /// Capture screenshot from screen coordinates
    /// </summary>
    /// <param name="region">Screen region to capture, null for full screen</param>
    /// <returns>Screenshot as byte array</returns>
    Task<byte[]> CaptureScreenshotAsync(ScreenRegion? region = null);

    /// <summary>
    /// Process and analyze extracted text for network-related insights
    /// </summary>
    /// <param name="extractedText">Text extracted from OCR operation</param>
    /// <param name="analysisType">Type of analysis to perform</param>
    /// <returns>Analysis result with suggestions and insights</returns>
    Task<TextAnalysisResult> AnalyzeExtractedTextAsync(string extractedText, OcrAnalysisType analysisType);

    /// <summary>
    /// Get supported OCR languages
    /// </summary>
    /// <returns>List of supported language codes</returns>
    Task<IEnumerable<string>> GetSupportedLanguagesAsync();

    /// <summary>
    /// Initialize OCR engine with required data files
    /// </summary>
    /// <returns>Success status of initialization</returns>
    Task<bool> InitializeAsync();

    /// <summary>
    /// Cleanup OCR engine resources
    /// </summary>
    Task DisposeAsync();
}