using VideoMaker.Database;

namespace VideoMaker.Handlers.Thumbnails;

public static class GetThumbnailStatus {

    public static async Task<IResult> Handle(Guid? id, ApplicationContext applicationContext) {

        if (id == null) {
            return TypedResults.BadRequest("Invalid video ID.");
        }

        // search if the required thumbnail exists
        var thumbnail = await applicationContext.Thumbnails.FindAsync(id);

        if (thumbnail == null) {
            return TypedResults.NotFound();
        }

        // if it exists it return the thumbnail job state as a string instead of an Enum
        return TypedResults.Ok(thumbnail.Status.ToString());
    }
}

