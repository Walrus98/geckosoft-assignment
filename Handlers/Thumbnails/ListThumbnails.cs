using Microsoft.EntityFrameworkCore;
using VideoMaker.Api;
using VideoMaker.Database;

namespace VideoMaker.Handlers.Thumbnails;

public static class ListThumbnails {

    public static async Task<IResult> Handle(int? offset, int? limit, ApplicationContext applicationContext) {

        if (offset == null || limit == null || offset < 0 || limit <= 0 || limit < offset) {
            return TypedResults.BadRequest();
        }

        var totalCount = await applicationContext.Thumbnails.CountAsync();

        var thumbnails = await applicationContext.Thumbnails
            .Skip(offset.Value)
            .Take(limit.Value)
            .Select(x => new ThumbnailDto(x))
            .ToArrayAsync();

        var response = new Page<ThumbnailDto>(offset.Value, limit.Value, totalCount, thumbnails);

        return TypedResults.Ok(response);
    }
}

