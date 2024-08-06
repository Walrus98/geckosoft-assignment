using Microsoft.AspNetCore.SignalR;
using VideoMaker.Database;

namespace VideoMaker.Services;

public class ThumbnailHub : Hub {

    private readonly ApplicationContext _applicatonContext;

    public ThumbnailHub(ApplicationContext context) {
        _applicatonContext = context;
    }

    public async Task JoinGroup(string thumbnailId) {

        var thumbnail = await _applicatonContext.Thumbnails.FindAsync(new Guid(thumbnailId));

        if (thumbnail != null) {
            await Groups.AddToGroupAsync(Context.ConnectionId, thumbnailId);
        }
    }

    public async Task LeaveGroup(string thumbnailId) {

        var thumbnail = await _applicatonContext.Thumbnails.FindAsync(new Guid(thumbnailId));

        if (thumbnail != null) {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, thumbnailId);
        }
    }
}
