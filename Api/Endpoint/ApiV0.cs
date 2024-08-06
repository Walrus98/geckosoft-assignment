using Microsoft.AspNetCore.Mvc;
using VideoMaker.Handlers.Thumbnails;
using VideoMaker.Handlers.Videos;
using VideoMaker.Services;

namespace VideoMaker.Api.Endpoint;

public static class ApiV0 {

    public static void MapEndpoints(RouteGroupBuilder api) {

        api
        .MapGroup("videos")
        .WithTags("Videos")
        .MapVideoEndpoints();

        api
        .MapGroup("thumbnails")
        .WithTags("Thumbnails")
        .MapThumbnailEndpoints();
    }

    static void MapVideoEndpoints(this RouteGroupBuilder group) {
        _ = group.MapPost("/upload", UploadVideo.Handle).WithMetadata(new RequestSizeLimitAttribute(1L * 1024 * 1024 * 1024)).DisableAntiforgery();
        _ = group.MapGet("/", ListVideos.Handle);
        _ = group.MapGet("/{id}", GetVideo.Handle);
    }

    static void MapThumbnailEndpoints(this RouteGroupBuilder group) {
        _ = group.MapPost("/generate", CreateThumbnail.Handle);
        _ = group.MapGet("/", ListThumbnails.Handle);
        _ = group.MapGet("/{id}", GetThumbnail.Handle);
        _ = group.MapGet("status/{id}", GetThumbnailStatus.Handle);
        _ = group.MapHub<ThumbnailHub>("/thumbnailHub");
    }
}


