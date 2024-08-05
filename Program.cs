using Microsoft.EntityFrameworkCore;
using VideoMaker.Api;
using VideoMaker.Database;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ApplicationContext>(opt => opt.UseInMemoryDatabase("VideoMaker"));

var app = builder.Build();

await app.RunAsync();
