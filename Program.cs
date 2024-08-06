using Microsoft.EntityFrameworkCore;
using VideoMaker.Api;
using VideoMaker.Api.Endpoint;
using VideoMaker.Database;
using VideoMaker.Extensions;
using VideoMaker.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ApplicationContext>(opt => opt.UseInMemoryDatabase("VideoMaker"));

builder.Services.AddSignalR();
builder.Services.AddSingleton<ThumbnailService>();

builder.Services.AddAntiforgery();

var apis = new Api[] {
    new("api-v0", "v0", "/v0", ApiV0.MapEndpoints),
};

builder.AddIdentity();
builder.AddJwtAuthentication();
builder.AddSwagger(apis);

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();



foreach (var api in apis) {
    var endpoint = app.MapGroup(api.Path).WithGroupName(api.Id);
    api.MapEndpoint(endpoint);
}

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
