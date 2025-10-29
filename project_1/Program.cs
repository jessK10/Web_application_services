using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using project_1.Data;
using project_1.Services;
using project_1.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// -------------------- DB / Identity --------------------
var connectionString = builder.Configuration.GetConnectionString("project_1ContextConnection")
    ?? throw new InvalidOperationException("Connection string 'project_1ContextConnection' not found.");

builder.Services.AddDbContext<project_1Context>(opt =>
    opt.UseSqlServer(connectionString, sql =>
    {
        sql.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(10), errorNumbersToAdd: null);
        sql.CommandTimeout(60);
    }));

builder.Services
    .AddDefaultIdentity<IdentityUser>(o => o.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<project_1Context>();

// -------------------- Controllers / Swagger --------------------
builder.Services.AddControllers()
    .AddJsonOptions(o =>
        o.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    // Safety net: if there is any accidental duplicate route, prefer the first one
    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
});

builder.Services.AddProblemDetails();

// -------------------- DI --------------------
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IAuthorService, AuthorService>();
builder.Services.AddScoped<IPublisherService, PublisherService>();
builder.Services.AddScoped<IGenreService, GenreService>();
builder.Services.AddScoped<IMemberService, MemberService>();

var app = builder.Build();

// -------------------- Pipeline --------------------
app.UseSwagger();
app.UseSwaggerUI();

app.UseRequestLogging();
app.UseGlobalExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// Liveness only (controller handles /healthz/db)
app.MapGet("/healthz", () => Results.Ok(new { status = "ok" }));

// IMPORTANT: Do NOT also MapGet("/healthz/db", ...) here.
// The controller route would collide.

// Controllers (includes HealthDbController -> /healthz/db)
app.MapControllers();

app.Run();
