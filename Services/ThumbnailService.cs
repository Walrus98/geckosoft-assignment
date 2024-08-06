using System.Diagnostics;
using Microsoft.AspNetCore.SignalR;
using VideoMaker.Database;
using static VideoMaker.Database.Entities.Thumbnail;

namespace VideoMaker.Services;

public class ThumbnailService {

    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ThumbnailService> _logger;

    private readonly IHubContext<ThumbnailHub> _hubContext;

    public ThumbnailService(IServiceProvider serviceProvider, ILogger<ThumbnailService> logger, IHubContext<ThumbnailHub> hubContext) {
        _serviceProvider = serviceProvider;
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task SubmitThumbnail(Guid id, CancellationToken stoppingToken) {

        // submits the thumbnail creation process to a threadpool
        await Task.Run(async () => {

            // takes the scope of the service to get the the application database context
            using var scope = _serviceProvider.CreateScope();
            var applicationContext = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

            // takes the thumbnail from the database
            var thumbnail = await applicationContext.Thumbnails.FindAsync(id);

            // notifies all clients connected to the group the status of the job
            _logger.LogInformation("Current status: {Status}", thumbnail!.Status);
            await _hubContext.Clients.Group(id.ToString()).SendAsync("ReceiveStatusChange", thumbnail.Status.ToString());
            // await _hubContext.Clients.All.SendAsync("ReceiveStatusChange", $"{thumbnail.Id} {thumbnail.Status}");

            // changes the state to analyzing and notifies to all client connected
            thumbnail.Status = ThumbnailStatus.ANALYZING;
            _ = await applicationContext.SaveChangesAsync();

            // Is for debugging so you can better see the state change
            // await Task.Delay(5000, stoppingToken);

            _logger.LogInformation("Current status: {Status}", thumbnail!.Status);
            await _hubContext.Clients.Group(id.ToString()).SendAsync("ReceiveStatusChange", thumbnail.Status.ToString());
            // await _hubContext.Clients.All.SendAsync("ReceiveStatusChange", thumbnail.Status.ToString());

            var video = await applicationContext.Videos.FindAsync(thumbnail.VideoId);

            try {

                // Generates the thumbnail launching a ffmpeg process
                await GenerateThumbnail(video!.FilePath, thumbnail.FilePath, thumbnail.Width, thumbnail.Height);

                // Is for debugging so you can better see the state change
                // await Task.Delay(5000, stoppingToken);

                thumbnail.Status = ThumbnailStatus.COMPLETED;

            } catch (Exception exception) {

                // if ffmpeg gives an error, I remove the thumbnail from the database
                _logger.LogError("Exception: {exception}", exception);

                thumbnail.Status = ThumbnailStatus.FAILED;

                _ = applicationContext.Thumbnails.Remove(thumbnail);

            } finally {

                // finally I notify the status of the change and update the database
                _logger.LogInformation("Current status: {Status}", thumbnail!.Status);
                await _hubContext.Clients.Group(id.ToString()).SendAsync("ReceiveStatusChange", thumbnail.Status.ToString());
                // await _hubContext.Clients.All.SendAsync("ReceiveStatusChange", thumbnail.Status.ToString());

                _ = await applicationContext.SaveChangesAsync();
            }

        }, stoppingToken);
    }

    private static async Task GenerateThumbnail(string videoPath, string outputPath, int width, int height) {
        var ffmpegPath = "ffmpeg";

        // in order to make ffmpeg works correctly it is necessary provide the absolute path file
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
