namespace VideoMaker.Database.Entities;

public class Thumbnail {

    public enum ThumbnailStatus {
        QUEUED,
        ANALYZING,
        COMPLETED,
        FAILED,
    }

    public Guid Id { get; set; }
    public Guid VideoId { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public required string FilePath { get; set; }
    public ThumbnailStatus Status { get; set; }
}
