using Microsoft.EntityFrameworkCore;
using VideoMaker.Api;
using VideoMaker.Database;
using VideoMaker.Database.Entities;

namespace VideoMaker.Handlers.Thumbnails;

public static class ListThumbnails {

    public static async Task<IResult> Handle(int? offset, int? limit, ApplicationContext applicationContext) {

        if (offset == null || limit == null || offset < 0 || limit <= 0 || limit < offset) {
            return TypedResults.BadRequest();
        }

        var totalCount = await applicationContext.Thumbnails.CountAsync();

        Console.WriteLine(totalCount);

        var thumbnails = await applicationContext.Thumbnails
            .Skip(offset.Value)
            .Take(limit.Value)
            .ToArrayAsync();

        var response = new Page<Thumbnail>(offset.Value, limit.Value, totalCount, thumbnails);

        return TypedResults.Ok(response);
    }
}

