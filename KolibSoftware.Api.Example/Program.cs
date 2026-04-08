using KolibSoftware.Api.Example;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ApiDbContext>(options =>
{
    var connection = builder.Configuration.GetSection("Database:Connection");
    var version = builder.Configuration.GetSection("Database:Version");
    options.UseMySql(connection.Value, ServerVersion.Parse(version.Value, ServerType.MariaDb));
});

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
