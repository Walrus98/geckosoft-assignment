using VideoMaker.Database;
using VideoMaker.Database.Entities;

namespace VideoMaker.Handlers.Videos;

public static class UploadVideo {

    private static readonly List<string> _allowedExtensions = [
        ".mp4", ".avi", ".mov", ".mkv"
    ];

    public static async Task<IResult> Handle(IFormFile file, ApplicationContext applicationContext) {

        if (file == null || file.Length == 0) {
            return TypedResults.BadRequest("No file uploaded.");
        }

        var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!_allowedExtensions.Contains(fileExtension)) {
            return TypedResults.BadRequest("Invalid file format. Allowed formats are: .mp4, .avi, .mov, .mkv.");
        }


        var videoId = Guid.NewGuid();

        var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
        _ = Directory.CreateDirectory(uploadsPath);

        var filePath = Path.Combine("uploads", videoId + Path.GetExtension(file.FileName));

        using (var stream = new FileStream(filePath, FileMode.Create)) {
            await file.CopyToAsync(stream);
        }

        var video = new Video {
            Id = videoId,
            FilePath = filePath,
        };

        _ = await applicationContext.Videos.AddAsync(video);
        _ = await applicationContext.SaveChangesAsync();

        return TypedResults.Created($"/videos/{videoId}", video);
    }
}

