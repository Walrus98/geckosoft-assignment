using Microsoft.EntityFrameworkCore;
using VideoMaker.Database;

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

        return Results.File(thumbnail.FilePath, "image/jpeg");
    }
}

