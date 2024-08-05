using VideoMaker.Handlers.Videos;

namespace VideoMaker.Api.Endpoint;

public static class ApiV0 {

    public static void MapEndpoints(RouteGroupBuilder api) {

        api
        .MapGroup("videos")
        .WithTags("Videos")
        .MapVideoEndpoints();
    }

    static void MapVideoEndpoints(this RouteGroupBuilder group) {
        _ = group.MapPost("/upload", UploadVideo.Handle).DisableAntiforgery();
        _ = group.MapGet("/", ListVideos.Handle);
        _ = group.MapGet("/{id}", GetVideo.Handle);
    }
}
