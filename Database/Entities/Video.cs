namespace VideoMaker.Database.Entities;

public class Video {
    public Guid Id { get; set; }
    public required string FilePath { get; set; }
}
