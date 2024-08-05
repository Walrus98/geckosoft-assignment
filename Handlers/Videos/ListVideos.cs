using Microsoft.EntityFrameworkCore;
using VideoMaker.Api;
using VideoMaker.Database;
using VideoMaker.Database.Entities;

namespace VideoMaker.Handlers.Videos;

public static class ListVideos {

    public static async Task<IResult> Handle(int? offset, int? limit, ApplicationContext applicationContext) {

        if (offset == null || limit == null || offset < 0 || limit <= 0 || limit < offset) {
            return TypedResults.BadRequest();
        }

        var totalCount = await applicationContext.Videos.CountAsync();

        var videos = await applicationContext.Videos
            .Skip(offset.Value)
            .Take(limit.Value)
            .ToArrayAsync();

        var response = new Page<Video>(offset.Value, limit.Value, totalCount, videos);

        return TypedResults.Ok(response);
    }
}

