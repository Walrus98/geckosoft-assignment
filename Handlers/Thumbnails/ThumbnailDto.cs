using VideoMaker.Database.Entities;

namespace VideoMaker.Handlers.Thumbnails;

public class ThumbnailDto {

    public Guid Id { get; set; }
    public Guid VideoId { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public string FilePath { get; set; }
    public string Status { get; set; }

    // Is used to convert the Enum used for the status of thumbnail jobs into a string
    public ThumbnailDto(Thumbnail thumbnail) =>
        (Id, VideoId, Width, Height, FilePath, Status) =
        (thumbnail.Id, thumbnail.VideoId, thumbnail.Width, thumbnail.Height, thumbnail.FilePath, thumbnail.Status.ToString());

}


