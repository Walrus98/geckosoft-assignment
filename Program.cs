using Microsoft.EntityFrameworkCore;
using VideoMaker.Api;
using VideoMaker.Api.Endpoint;
using VideoMaker.Database;
using VideoMaker.Extensions;
using VideoMaker.Services;

var builder = WebApplication.CreateBuilder(args);

// Memory DB Context
builder.Services.AddDbContext<ApplicationContext>(opt => opt.UseInMemoryDatabase("VideoMaker"));

// SignlarR service for handle real time update for thumbnail
builder.Services.AddSignalR();
// Service for handling the Thumbnail creation
builder.Services.AddSingleton<ThumbnailService>();

// Used to be able to upload videos
builder.Services.AddAntiforgery();


// Api Endpoint
var apis = new Api[] {
    new("api-v0", "v0", "/v0", ApiV0.MapEndpoints),
};

// Extensions Method
builder.AddIdentity();
builder.AddJwtAuthentication();
builder.AddCorsPolicy();
builder.AddSwagger(apis);

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// Enable Cors Policy to handle real time thumbnail creation with SignalR
app.UseCors("AllowSpecificOrigins");

// Used to be able to upload videos
app.UseAntiforgery();

// Endpoint creation
foreach (var api in apis) {
    var endpoint = app.MapGroup(api.Path).WithGroupName(api.Id);
    api.MapEndpoint(endpoint);
}

// Enable SwaggerUI in development mode
if (builder.Environment.IsDevelopment()) {
    foreach (var api in apis) {
        _ = app.UseSwagger();
        _ = app.UseSwaggerUI(options => {
            options.SwaggerEndpoint($"{api.Id}/swagger.yaml", api.Id);
            options.EnablePersistAuthorization();
        });
    }
}

await app.RunAsync();
