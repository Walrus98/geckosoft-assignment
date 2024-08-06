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

        // create new GUID for video entity
        var videoId = Guid.NewGuid();

        // takes the path to the uploads folder, if it doesn't exist I create it
        var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
        _ = Directory.CreateDirectory(uploadsPath);

        // creates the file path for the video
        var filePath = Path.Combine("uploads", videoId + Path.GetExtension(file.FileName));

        // opens a byte stream to copy the file inside the folder
        using (var stream = new FileStream(filePath, FileMode.Create)) {
            await file.CopyToAsync(stream);
        }

        // creates a video entity for the database and adds it
        var video = new Video {
            Id = videoId,
            FilePath = filePath,
        };

        _ = await applicationContext.Videos.AddAsync(video);
        _ = await applicationContext.SaveChangesAsync();

        return TypedResults.Created($"/videos/{videoId}", video);
    }
}

