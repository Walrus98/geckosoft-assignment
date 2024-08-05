namespace VideoMaker.Api;

public record Page<T>(
    int Offset,
    int Limit,
    int Count,
    IEnumerable<T> Items
);
