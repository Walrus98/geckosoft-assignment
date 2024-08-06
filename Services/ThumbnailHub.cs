using Microsoft.AspNetCore.SignalR;
using VideoMaker.Database;

namespace VideoMaker.Services;

public class ThumbnailHub : Hub {

    private readonly ApplicationContext _applicatonContext;

    public ThumbnailHub(ApplicationContext context) {
        _applicatonContext = context;
    }

    // In order to receive notifications about the change of the job status of a thumbnail,
    // it is necessary for the client to register to the group which has as for name the id of the thumbnail
    public async Task JoinGroup(string thumbnailId) {

        // If a request comes in, it is necessary to check that the thumbnail actually exists
        var thumbnail = await _applicatonContext.Thumbnails.FindAsync(new Guid(thumbnailId));

        if (thumbnail != null) {
            await Groups.AddToGroupAsync(Context.ConnectionId, thumbnailId);
        }
    }

    // Used to unregister a client from a group  which it has previously registered
    public async Task LeaveGroup(string thumbnailId) {

        // In order to stop receiving notifications about a thumbnail job change, it is necessary ensure that the thumbnail exists
        var thumbnail = await _applicatonContext.Thumbnails.FindAsync(new Guid(thumbnailId));

        if (thumbnail != null) {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, thumbnailId);
        }
    }
}
