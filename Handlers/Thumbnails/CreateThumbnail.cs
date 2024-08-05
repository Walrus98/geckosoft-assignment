using Microsoft.EntityFrameworkCore;
using VideoMaker.Database;
using VideoMaker.Services;

namespace VideoMaker.Handlers.Thumbnails;

public static class CreateThumbnail {

    public class CreateThumbnailRequest {
        public Guid VideoId { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public static async Task<IResult> Handle(CreateThumbnailRequest request, ApplicationContext applicationContext, ThumbnailService thumbnailService) {

        var video = await applicationContext.Videos.FindAsync(request.VideoId);

        if (video is null) {
            return Results.NotFound();
        }

        var existingThumbnail = await applicationContext.Thumbnails.FirstOrDefaultAsync(
            t => t.VideoId == request.VideoId &&
            t.Width == request.Width &&
            t.Height == request.Height
        );

        if (existingThumbnail is not null) {
            return Results.BadRequest("This thumbnail already exists!");
        }

        if (request.Width <= 0 || request.Height <= 0) {
            return Results.BadRequest("Width and height must be positive.");
        }

        var thumbnail = await thumbnailService.CreateThumbnailAsync(request.VideoId, request.Width, request.Height);

        _ = await applicationContext.Thumbnails.AddAsync(thumbnail);
        _ = await applicationContext.SaveChangesAsync();

        return Results.Created($"/api/thumbnails/{thumbnail.Id}", new {
            thumbnail.Id,
            thumbnail.VideoId,
            Status = "QUEUED"
        });
    }
}

