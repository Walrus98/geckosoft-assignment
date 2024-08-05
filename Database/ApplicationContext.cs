using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VideoMaker.Database.Entities;

namespace VideoMaker.Database;

public class ApplicationContext : IdentityDbContext<IdentityUser> {
    public ApplicationContext() { }

    public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options) {
}
