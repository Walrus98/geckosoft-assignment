using System.Text;
using Microsoft.AspNetCore.Identity;
using VideoMaker.Database;

namespace VideoMaker.Extensions;

public static class WebServerConfiguration {

    public static void AddIdentity(this WebApplicationBuilder builder) {
        _ = builder.Services.AddIdentity<IdentityUser, IdentityRole>()
        .AddEntityFrameworkStores<ApplicationContext>()
        .AddDefaultTokenProviders();
    }

}
