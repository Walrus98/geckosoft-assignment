using Microsoft.AspNetCore.StaticFiles;
using VideoMaker.Database;

namespace VideoMaker.Handlers.Videos;

public static class GetVideo {

    public static async Task<IResult> Handle(Guid? id, ApplicationContext applicationContext) {

        if (id == null) {
            return TypedResults.BadRequest("Invalid video ID.");
        }

        var video = await applicationContext.Videos.FindAsync(id.Value);

        if (video == null) {
            return TypedResults.NotFound();
        }

        if (!File.Exists(video.FilePath)) {
            return TypedResults.NotFound("File not found.");
        }

        var provider = new FileExtensionContentTypeProvider();
        if (!provider.TryGetContentType(video.FilePath, out var contentType)) {
            contentType = "application/octet-stream";
        }

        var fileStream = new FileStream(video.FilePath, FileMode.Open, FileAccess.Read);

        return Results.Stream(fileStream, contentType, Path.GetFileName(video.FilePath));
    }
}

