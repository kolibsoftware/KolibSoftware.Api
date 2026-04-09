using System.Runtime.InteropServices;
using Docnet.Core;
using Docnet.Core.Models;
using SkiaSharp;
using Tesseract;

namespace KolibSoftware.Api.Example;

public static class ExtractionExtensions
{

    public static async IAsyncEnumerable<byte[]> ExtractBitmapsAsync(this IDocLib docLib, byte[] pdfBytes)
    {
        using var docReader = docLib.GetDocReader(pdfBytes, new PageDimensions(1080, 1920));
        var pageCount = docReader.GetPageCount();

        for (var i = 0; i < pageCount; i++)
        {
            using var pageReader = docReader.GetPageReader(i);
            var bytes = pageReader.GetImage();

            var width = pageReader.GetPageWidth();
            var height = pageReader.GetPageHeight();

            GCHandle gcHandle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            SKImageInfo info = new(width, height, SKColorType.Rgba8888, SKAlphaType.Premul);

            using var bitmap = new SKBitmap();
            bitmap.InstallPixels(info, gcHandle.AddrOfPinnedObject(), info.RowBytes, delegate { gcHandle.Free(); });
            using var scaled = bitmap.Resize(new SKImageInfo(width, height), SKSamplingOptions.Default);

            using var image = SKImage.FromBitmap(scaled);
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);
            var span = data.AsSpan();
            yield return span.ToArray();
        }
    }

    public static async IAsyncEnumerable<string> ExtractTextsAsync(this IDocLib docLib, byte[] pdfBytes)
    {
        using var docReader = docLib.GetDocReader(pdfBytes, new PageDimensions(1080, 1920));
        var pageCount = docReader.GetPageCount();

        for (var i = 0; i < pageCount; i++)
        {
            using var pageReader = docReader.GetPageReader(i);
            var text = pageReader.GetText();
            yield return text;
        }
    }

    public static async IAsyncEnumerable<(string Text, float Confidence)> ExtractVisualTextsAsync(this TesseractEngine engine, byte[] pdfBytes)
    {
        await foreach (var bitmapBytes in DocLib.Instance.ExtractBitmapsAsync(pdfBytes))
        {
            using var pix = Pix.LoadFromMemory(bitmapBytes);
            using var page = engine.Process(pix);
            var text = page.GetText();
            var confidence = page.GetMeanConfidence();
            yield return (text, confidence);
        }
    }

}