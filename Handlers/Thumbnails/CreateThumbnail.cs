using Microsoft.EntityFrameworkCore;
using VideoMaker.Database;
using VideoMaker.Database.Entities;
using VideoMaker.Services;
using static VideoMaker.Database.Entities.Thumbnail;

namespace VideoMaker.Handlers.Thumbnails;

public static class CreateThumbnail {

    public class CreateThumbnailRequest {
        public Guid VideoId { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public static async Task<IResult> Handle(CreateThumbnailRequest request, ApplicationContext applicationContext, ThumbnailService thumbnailService, CancellationToken cancellationToken) {

        if (request == null) {
            return Results.BadRequest();
        }

        var video = await applicationContext.Videos.FindAsync([request.VideoId], cancellationToken: cancellationToken);

        if (video == null) {
            return Results.NotFound();
        }

        // Check if the thumbnail that was requested is already existing
        var existingThumbnail = await applicationContext.Thumbnails.FirstOrDefaultAsync(
            t => t.VideoId == request.VideoId &&
            t.Width == request.Width &&
            t.Height == request.Height, cancellationToken: cancellationToken);

        // If it has already been created, then I return an error message
        if (existingThumbnail != null) {
            return Results.BadRequest("This thumbnail is already sumbitted.");
        }

        if (request.Width <= 0 || request.Height <= 0) {
            return Results.BadRequest("Width and height must be positive.");
        }

        // create new GUID for thumbnail entity
        var thumbnailId = Guid.NewGuid();

        // takes the path to the thumbnails folder, if it doesn't exist I create it
        var thumbnailsPath = Path.Combine(Directory.GetCurrentDirectory(), "thumbnails");
        _ = Directory.CreateDirectory(thumbnailsPath);

        // creates the file path for the thumbnail in .jpg format
        var thumbnailPath = Path.Combine("thumbnails", thumbnailId + ".jpg");

        // creates a thumbnail entity for the database and adds it
        var thumbnail = new Thumbnail {
            Id = thumbnailId,
            VideoId = request.VideoId,
            Width = request.Width,
            Height = request.Height,
            FilePath = thumbnailPath,
            Status = ThumbnailStatus.QUEUED
        };

        _ = await applicationContext.Thumbnails.AddAsync(thumbnail, cancellationToken);
        _ = await applicationContext.SaveChangesAsync(cancellationToken);

        // submits the task of creating a new thumbnail to the thumbnail service
        _ = thumbnailService.SubmitThumbnail(thumbnailId, cancellationToken);

        var thumbnailDto = new ThumbnailDto(thumbnail);

        return Results.Accepted($"/thumbnails/status/{thumbnail.Id}", thumbnailDto);
    }
}

