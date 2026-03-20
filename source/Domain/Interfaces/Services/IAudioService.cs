namespace Project.Domain.Interfaces.Services;

public interface IAudioService
{
    /// <summary>
    /// Re-encodes the audio bytes to 128 kbps CBR MP3 if the source bitrate
    /// exceeds <paramref name="thresholdBitrate"/> (default 160 kbps).
    /// Returns the original bytes unchanged when the source is already lean.
    /// </summary>
    Task<byte[]> CompressAsync(byte[] audioBytes, int thresholdBitrate = 160_000);
}
