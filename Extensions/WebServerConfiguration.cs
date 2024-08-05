using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using VideoMaker.Database;

namespace VideoMaker.Extensions;

public static class WebServerConfiguration {

    public static void AddIdentity(this WebApplicationBuilder builder) {
        _ = builder.Services.AddIdentity<IdentityUser, IdentityRole>()
        .AddEntityFrameworkStores<ApplicationContext>()
        .AddDefaultTokenProviders();
    }

    public static void AddJwtAuthentication(this WebApplicationBuilder builder) {

        _ = builder.Services.AddAuthentication(options => {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options => {
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = "https://localhost:5281",
                ValidAudience = "https://localhost:5281",
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("YourSuperSecretKeyThatIsAtLeast32BytesLong!"))
            };
        });

        _ = builder.Services.AddAuthorization();
    }

}
