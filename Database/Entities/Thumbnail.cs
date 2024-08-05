namespace VideoMaker.Database.Entities;

public class Thumbnail {
    public Guid Id { get; set; }
    public Guid VideoId { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public required string FilePath { get; set; }
}
