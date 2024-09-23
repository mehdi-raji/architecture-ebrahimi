using Data;
using Data.Contracts;
using Data.Repositories;
using ElmahCore.Mvc;
using ElmahCore.Sql;
using Microsoft.EntityFrameworkCore;
using NLog.Web;
using Webframework.Middlewares;
using System.Net;
using NLog;
using Services.Services;
using Webframework.Configuration;
using Microsoft.AspNetCore.Mvc.Authorization;
using Common;

var builder = WebApplication.CreateBuilder(args);

// NLog: setup the logger
var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("init main");

try
{
	var siteSettings = builder.Configuration.GetSection(nameof(SiteSettings)).Get<SiteSettings>();

	builder.Services.Configure<SiteSettings>(builder.Configuration.GetSection(nameof(SiteSettings)));

	// Add services to the container.

	builder.Services.AddControllers(options=> options.Filters.Add(new AuthorizeFilter()));

	// Swagger configuration
	builder.Services.AddEndpointsApiExplorer();
	builder.Services.AddSwaggerGen();

	// Database context configuration with SQL Server
	builder.Services.AddDbContext<ApplicationDbContext>(options =>
		options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")));

	// Dependency Injection for repositories
	builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
	builder.Services.AddScoped<IUserRepository, UserRepository>();
	builder.Services.AddScoped<IJwtService, JwtService>();
	builder.Services.AddJwtAuthentication(siteSettings.JwtSettings);
	// NLog configuration
	builder.Logging.ClearProviders();
	builder.Host.UseNLog();

	// Elmah configuration
	builder.Services.AddElmah<SqlErrorLog>(options =>
	{
		options.ConnectionString = builder.Configuration.GetConnectionString("Elmah");
		options.Path = siteSettings.ElmahPath;
	});

	// Build the app
	var app = builder.Build();

	// Custom middleware to handle exceptions
	app.UseCustomExceptionHandler();

	// Swagger and UI in Development mode
	if (app.Environment.IsDevelopment())
	{
		app.UseSwagger();
		app.UseSwaggerUI();
	}

	// Elmah middleware for error logging
	app.UseElmah();

	// Other middleware
	app.UseHttpsRedirection();
	app.UseAuthentication();

	app.UseAuthorization();

	// Mapping controllers
	app.MapControllers();

	// Run the application
	app.Run();
}
catch (Exception ex)
{
	// NLog: catch setup errors
	logger.Error(ex, "Stopped program because of exception");
	throw;
}
finally
{
	// Ensure to flush and stop internal timers/threads before application-exit
	NLog.LogManager.Shutdown();
}
