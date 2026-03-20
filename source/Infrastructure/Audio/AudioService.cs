using FFMpegCore;
using FFMpegCore.Enums;
using Microsoft.Extensions.Logging;
using Project.Domain.Interfaces.Services;

namespace Project.Infrastructure.Audio;

public class AudioService(ILogger<AudioService> logger) : IAudioService
{
    // Target bitrate for re-encoded output (bits/s)
    private const int TargetBitrate = 128;

    public async Task<byte[]> CompressAsync(byte[] audioBytes, int thresholdBitrate = 160_000)
    {
        if (audioBytes == null || audioBytes.Length == 0)
            return audioBytes ?? [];

        var inputPath  = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.mp3");
        var outputPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.mp3");

        try
        {
            await File.WriteAllBytesAsync(inputPath, audioBytes);

            var info = await FFProbe.AnalyseAsync(inputPath);
            var sourceBitrate = info.PrimaryAudioStream?.BitRate ?? 0; // bits/s

            if (sourceBitrate <= thresholdBitrate)
            {
                logger.LogInformation(
                    "Audio compression skipped: source bitrate {Bitrate} kbps is at or below threshold {Threshold} kbps.",
                    sourceBitrate / 1000, thresholdBitrate / 1000);

                return audioBytes;
            }

            logger.LogInformation(
                "Compressing audio from {Source} kbps → {Target} kbps CBR.",
                sourceBitrate / 1000, TargetBitrate);

            var success = await FFMpegArguments
                .FromFileInput(inputPath)
                .OutputToFile(outputPath, overwrite: true, opts => opts
                    .WithAudioCodec(AudioCodec.LibMp3Lame)
                    .WithAudioBitrate(TargetBitrate)
                    .ForceFormat("mp3")
                    .DisableChannel(Channel.Video)) // strip any cover-art stream
                .ProcessAsynchronously();

            if (!success || !File.Exists(outputPath))
            {
                logger.LogWarning("FFMpeg compression failed; uploading original file.");
                return audioBytes;
            }

            var compressed = await File.ReadAllBytesAsync(outputPath);

            logger.LogInformation(
                "Audio compressed: {Before} KB → {After} KB ({Pct:F1}% reduction).",
                audioBytes.Length / 1024,
                compressed.Length / 1024,
                (1.0 - (double)compressed.Length / audioBytes.Length) * 100);

            return compressed;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Audio compression threw an exception; uploading original file.");
            return audioBytes;
        }
        finally
        {
            if (File.Exists(inputPath))  File.Delete(inputPath);
            if (File.Exists(outputPath)) File.Delete(outputPath);
        }
    }
}
