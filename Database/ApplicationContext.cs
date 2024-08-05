using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VideoMaker.Database.Entities;

namespace VideoMaker.Database;

public class ApplicationContext : IdentityDbContext<IdentityUser> {

    public DbSet<Video> Videos => Set<Video>();
    public DbSet<Thumbnail> Thumbnails => Set<Thumbnail>();

    public ApplicationContext() { }

    public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options) {
    }
}
