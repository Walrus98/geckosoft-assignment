using VideoMaker.Database;

namespace VideoMaker.Handlers.Thumbnails;

public static class GetThumbnailStatus {

    public static async Task<IResult> Handle(Guid? id, ApplicationContext applicationContext) {

        if (id == null) {
            return TypedResults.BadRequest("Invalid video ID.");
        }

        var thumbnail = await applicationContext.Thumbnails.FindAsync(id);

        if (thumbnail == null) {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(thumbnail.Status.ToString());
    }
}

