using Microsoft.EntityFrameworkCore;
using VideoMaker.Api;
using VideoMaker.Api.Endpoint;
using VideoMaker.Database;
using VideoMaker.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ApplicationContext>(opt => opt.UseInMemoryDatabase("VideoMaker"));

var apis = new Api[] {
    new("api-v0", "v0", "/v0", ApiV0.MapEndpoints),
};

builder.AddIdentity();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();


foreach (var api in apis) {
    var endpoint = app.MapGroup(api.Path).WithGroupName(api.Id);
    api.MapEndpoint(endpoint);
}


await app.RunAsync();
