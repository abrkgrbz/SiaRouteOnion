using Application;
using Application.Interfaces;
using Identity;
using Identity.Context;
using Identity.Models;
using Identity.Seeds;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Persistence;
using Persistence.Contexts;
using Persistence.Seeds;
using Serilog;
using Shared;
using SiaRoute.WebApp.Extensions;
using SiaRoute.WebApp.Helpers;
using SiaRoute.WebApp.Middlewares;
using SiaRoute.WebApp.Services;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

var builder = WebApplication.CreateBuilder(args);
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();

// Add services to the container.
 
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();  
builder.Services.AddSharedInfrastructure();
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration)); 
builder.Services.AddApplicationLayer();
builder.Services.AddIdentityInfrastructure(configuration);
builder.Services.AddPersistenceInfrastructure(configuration);
builder.Services.AddScoped<IAuthenticatedUserService, AuthenticatedUserService>();
builder.Services.AddControllers();
builder.Services.AddCors(options => options.AddPolicy("AllowAll", p => p.AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader()));
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy =>
        policy.RequireClaim("roles", "Admin","SuperAdmin"));
    options.AddPolicy("BasicUserPolicy", policy =>
        policy.RequireClaim("roles", "Basic","Admin","SuperAdmin","Moderator"));
});
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

#region Scope Seeds Data

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var userManager = services.GetRequiredService<UserManager<SiaRouteUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var identityDbContext = services.GetRequiredService<IdentityContext>();
        var dbContext = services.GetRequiredService<SiaRouteDbContext>();
        await DefaultSCM.SeedAsync(dbContext);
        await DefaultDepartment.SeedAsync(identityDbContext); 
        await DefaultRoles.SeedAsync(userManager, roleManager);
        await DefaultSuperAdmin.SeedAsync(identityDbContext, userManager, roleManager);
        await DefaultBasicUser.SeedAsync(identityDbContext, userManager, roleManager);
        await DefaultAdminUser.SeedAsync(identityDbContext, userManager, roleManager);

        Log.Information("Finished Seeding Default Data");
    }
    catch (Exception ex)
    {
        Log.Warning(ex, "An error occurred seeding the DB");
    }
    finally
    {
        Log.CloseAndFlush();
    }
}

#endregion



app.UseHttpsRedirection();
app.UseStaticFiles(); 
app.UseRouting();
app.UseAuthentication();
app.UseSerilogRequestLogging();
app.UseAuthorization();
app.ConfigureExceptionHandler<Program>(app.Services.GetRequiredService<ILogger<Program>>());
app.UseMiddleware<ErrorHandlerMiddleware>();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
