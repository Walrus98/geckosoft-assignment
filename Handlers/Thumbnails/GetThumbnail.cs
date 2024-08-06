using Microsoft.EntityFrameworkCore;
using VideoMaker.Database;
using static VideoMaker.Database.Entities.Thumbnail;

namespace VideoMaker.Handlers.Thumbnails;

public static class GetThumbnail {

    public static async Task<IResult> Handle(Guid? id, int w, int h, ApplicationContext applicationContext) {

        if (id == null) {
            return TypedResults.BadRequest("Invalid video ID.");
        }

        var thumbnail = await applicationContext.Thumbnails.FirstOrDefaultAsync(t => t.VideoId == id && t.Width == w && t.Height == h);

        if (thumbnail == null) {
            return TypedResults.NotFound();
        }

        if (thumbnail.Status != ThumbnailStatus.COMPLETED) {
            return TypedResults.Ok($"Thumbnail is processing, current status: {thumbnail.Status}");
        }

        var absoluteFilePath = Path.Combine(Directory.GetCurrentDirectory(), thumbnail.FilePath);

        return Results.File(absoluteFilePath, "image/jpeg");
    }
}

