using System.Diagnostics;
using VideoMaker.Database;
using static VideoMaker.Database.Entities.Thumbnail;

namespace VideoMaker.Services;

public class ThumbnailService {

    private readonly IServiceProvider _serviceProvider;

    public ThumbnailService(IServiceProvider serviceProvider) {
        _serviceProvider = serviceProvider;
    }

    public async Task SubmitThumbnail(Guid id, CancellationToken stoppingToken) {

        await Task.Run(async () => {
            using var scope = _serviceProvider.CreateScope();
            var applicationContext = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

            var thumbnail = await applicationContext.Thumbnails.FindAsync(id);

            Console.WriteLine("STATUS: " + thumbnail!.Status);

            thumbnail.Status = ThumbnailStatus.ANALYZING;
            _ = await applicationContext.SaveChangesAsync();

            // await Task.Delay(5000, stoppingToken);

            Console.WriteLine("STATUS: " + thumbnail.Status);

            var video = await applicationContext.Videos.FindAsync(thumbnail.VideoId);

            try {
                await GenerateThumbnail(video!.FilePath, thumbnail.FilePath, thumbnail.Width, thumbnail.Height);

                // await Task.Delay(5000, stoppingToken);

                thumbnail.Status = ThumbnailStatus.COMPLETED;

            } catch (Exception exception) {

                Console.Error.WriteLine(exception);
                thumbnail.Status = ThumbnailStatus.FAILED;

                _ = applicationContext.Thumbnails.Remove(thumbnail);

            } finally {

                Console.WriteLine("STATUS: " + thumbnail.Status);
                _ = await applicationContext.SaveChangesAsync();
            }

        }, stoppingToken);
    }

    private static async Task GenerateThumbnail(string videoPath, string outputPath, int width, int height) {
        var ffmpegPath = "ffmpeg";

        var absoluteVideoFilePath = Path.Combine(Directory.GetCurrentDirectory(), videoPath);
        var absoluteThumbnailFilePath = Path.Combine(Directory.GetCurrentDirectory(), outputPath);

        var arguments = $"-i {absoluteVideoFilePath} -vf scale={width}:{height} -frames:v 1 {absoluteThumbnailFilePath}";

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

        // var output = await process.StandardOutput.ReadToEndAsync();
        var error = await process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync();

        if (process.ExitCode != 0) {
            throw new Exception($"ffmpeg exited with code {process.ExitCode}: {error}");
        }
    }
}
