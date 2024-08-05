namespace VideoMaker.Api;

public sealed record Api(
    string Id,
    string Version,
    string Path,
    Action<RouteGroupBuilder> MapEndpoint
);
