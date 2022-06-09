using System.Reflection;
using Dashboard_Backend.Database;
using Dashboard_Backend.Helpers;
using LinqToDB.AspNet;
using LinqToDB.Configuration;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "PickIt Dashboard API",
        Version = "v1",
        Description = "Backend REST API for the PickIt-Dashboard"
    });
    
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

builder.Services.AddLinqToDBContext<DatabaseMain>((provider, options) =>
{
    options.UseMySql(builder.Configuration["Database:ConnectionString"]);
});

builder.Services.AddSingleton<Generator>();
builder.Services.AddSingleton<Validator>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();