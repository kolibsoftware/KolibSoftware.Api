using KolibSoftware.Api.Example;
using KolibSoftware.Api.Infra.Events;
using KolibSoftware.Api.Infra.Events.Attributes;
using KolibSoftware.Api.Infra.Repo;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

[assembly: EnableEvents]

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<DbContext, ApiDbContext>(options =>
{
    var connection = builder.Configuration.GetSection("Database:Connection");
    var version = builder.Configuration.GetSection("Database:Version");
    options.UseMySql(connection.Value, ServerVersion.Parse(version.Value, ServerType.MariaDb), sqlOptions =>
    {
        sqlOptions.UseMicrosoftJson();
    });
});
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped(typeof(IQueryableRepository<>), typeof(QueryableRepository<>));
builder.Services.AddControllers();
builder.AddEvents();

var app = builder.Build();
app.MapControllers();

app.Run();
