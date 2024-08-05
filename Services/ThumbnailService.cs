using System.Diagnostics;
using VideoMaker.Database.Entities;

namespace VideoMaker.Services;

public class ThumbnailService {

    private readonly string _uploadsPath;
    private readonly string _thumbnailsPath;

    public ThumbnailService() {
        _uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
        _thumbnailsPath = Path.Combine(Directory.GetCurrentDirectory(), "thumbnails");

        _ = Directory.CreateDirectory(_thumbnailsPath);
    }

    public async Task<Thumbnail> CreateThumbnailAsync(Guid videoId, int width, int height) {

        var thumbnailId = Guid.NewGuid();
        var videoPath = Path.Combine(_uploadsPath, $"{videoId}.mp4");
        var thumbnailPath = Path.Combine(_thumbnailsPath, $"{thumbnailId}.jpg");

        await GenerateThumbnail(videoPath, thumbnailPath, width, height);

        var thumbnail = new Thumbnail {
            Id = thumbnailId,
            VideoId = videoId,
            Width = width,
            Height = height,
            FilePath = thumbnailPath,
        };

        return thumbnail;
    }

    private static async Task GenerateThumbnail(string videoPath, string outputPath, int width, int height) {
        var ffmpegPath = "ffmpeg";
        var arguments = $"-i {videoPath} -vf scale={width}:{height} {outputPath}";

        var processStartInfo = new ProcessStartInfo {
            FileName = ffmpegPath,
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = new Process { StartInfo = processStartInfo };
        _ = process.Start();
        await process.WaitForExitAsync();
    }
}
