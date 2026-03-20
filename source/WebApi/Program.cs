
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.HttpOverrides;
using Project.Infrastructure.Data;
using Project.Infrastructure.Data.Seeds;
using Project.WebApi.Configurations;

static async Task InitialiseDatabaseAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    await context.Database.MigrateAsync();
    await SeedData.InsertSeedsAsync(context);
}

var builder = WebApplication.CreateBuilder(args);

// Trust the X-Forwarded-For / X-Forwarded-Proto headers sent by Render's reverse proxy.
// Without this, RemoteIpAddress is always the proxy IP and rate-limiting per user IP breaks.
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    // Clear known proxy/network lists so ALL forwarded headers are trusted
    // (Render's proxy IPs are not public/static; restrict further if you know the range)
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

// Remove the "Server: Kestrel" response header to avoid tech fingerprinting.
builder.WebHost.ConfigureKestrel(opts => opts.AddServerHeader = false);

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddWebServices(builder.Configuration);
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

var app = builder.Build();

app.UseForwardedHeaders();

if (app.Environment.IsDevelopment())
{
    await InitialiseDatabaseAsync(app);

}
else
{
    app.UseHsts();
}

app.UseRequestLocalization(app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);

app.UseHealthChecks("/health");

// Security headers
app.Use(async (context, next) =>
{
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["X-Frame-Options"] = "DENY";
    context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
    context.Response.Headers["Permissions-Policy"] = "camera=(), microphone=(), geolocation=()";
    context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
    // Restrict all content fetching — this is a pure API, no browser resources needed.
    context.Response.Headers["Content-Security-Policy"] = "default-src 'none'";
    if (app.Environment.IsProduction())
    {
        context.Response.Headers["Strict-Transport-Security"] = "max-age=63072000; includeSubDomains; preload";
    }
    await next();
});

app.UseHttpsRedirection();

app.UseRateLimiter();
app.UseRouting();
app.UseAuthentication();
app.UseCors("AllowFrontend");

app.UseAuthorization();

app.MapControllers();

app.UseSwaggerConfiguration();

app.UseExceptionHandler(options => { });

app.Map("/", () => app.Environment.IsProduction()
    ? Results.Ok(new { status = "healthy" })
    : Results.Redirect("/swagger"));

app.Run();

public partial class Program { }

