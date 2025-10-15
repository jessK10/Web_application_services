using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using project_1.Data;
using project_1.Services;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Connection string (User Secrets / appsettings.*)
var connectionString = builder.Configuration.GetConnectionString("project_1ContextConnection")
    ?? throw new InvalidOperationException("Connection string 'project_1ContextConnection' not found.");

builder.Services.AddDbContext<project_1Context>(opt =>
    opt.UseSqlServer(connectionString, sql =>
    {
        // Resiliency for transient Azure SQL hiccups
        sql.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null);

        // Give the DB more time on first requests / cold starts
        sql.CommandTimeout(60);
    }));

builder.Services.AddDefaultIdentity<IdentityUser>(o => o.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<project_1Context>();

// Controllers + Swagger
builder.Services.AddControllers()
    // (optional) if you ever serialize entities directly, prevent cycles
    .AddJsonOptions(o => o.JsonSerializerOptions.ReferenceHandler =
        System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Your services
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IAuthorService, AuthorService>();
builder.Services.AddScoped<IPublisherService, PublisherService>();
builder.Services.AddScoped<IGenreService, GenreService>();
builder.Services.AddScoped<IMemberService, MemberService>();

var app = builder.Build();

// Swagger
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
