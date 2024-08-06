using Microsoft.EntityFrameworkCore;
using VideoMaker.Api;
using VideoMaker.Database;

namespace VideoMaker.Handlers.Thumbnails;

public static class ListThumbnails {

    public static async Task<IResult> Handle(int? offset, int? limit, ApplicationContext applicationContext) {

        if (offset == null || limit == null || offset < 0 || limit <= 0 || limit < offset) {
            return TypedResults.BadRequest();
        }

        // takes the total number of thumbnail entries
        var totalCount = await applicationContext.Thumbnails.CountAsync();

        // take the required entries and convert them to an array
        var thumbnails = await applicationContext.Thumbnails
            .Skip(offset.Value)
            .Take(limit.Value)
            .Select(x => new ThumbnailDto(x))
            .ToArrayAsync();

        // return the response message as a page
        var response = new Page<ThumbnailDto>(offset.Value, limit.Value, totalCount, thumbnails);

        return TypedResults.Ok(response);
    }
}

