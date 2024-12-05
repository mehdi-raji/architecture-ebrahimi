using Architecture.Models;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common;
using ElmahCore.Mvc;
using Entities;
using Microsoft.AspNetCore.Mvc.Authorization;
using NLog;
using NLog.Web;
using Scalar.AspNetCore;
using Webframework.Configuration;
using Webframework.Middlewares;

var builder = WebApplication.CreateBuilder(args);

var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("init main");

try
{
    var siteSettings = builder.Configuration.GetSection(nameof(SiteSettings)).Get<SiteSettings>();
    builder.Services.Configure<SiteSettings>(builder.Configuration.GetSection(nameof(SiteSettings)));

    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

    builder.Host.ConfigureContainer<ContainerBuilder>(ConfigureAutofac);

    ConfigureServices(builder.Services, builder.Configuration, siteSettings);

    var app = builder.Build();

    ConfigurePipeline(app, app.Environment);

    app.Run();
}
catch (Exception ex)
{
    logger.Error(ex, "Stopped program because of exception");
    throw;
}
finally
{
    LogManager.Flush();
    LogManager.Shutdown();
}

void ConfigureServices(IServiceCollection services, IConfiguration configuration, SiteSettings siteSettings)
{
    services.AddControllers(options => options.Filters.Add(new AuthorizeFilter()));

	services.AddEndpointsApiExplorer();
	services.AddOpenApi();

	services.AddAutoMapper(config =>
    {
	    config.CreateMap<Post, PostDto>().ReverseMap();
    });

	services.AddDbContext(configuration);

    services.AddCustomInentity(siteSettings.IdentitySettings);

    services.AddJwtAuthentication(siteSettings.JwtSettings);

    services.AddElmah(configuration, siteSettings);

    services.AddMinimalMvc();
}

void ConfigureAutofac(ContainerBuilder builder)
{
    builder.AddServices();
}

void ConfigurePipeline(WebApplication app, IWebHostEnvironment env)
{
    app.UseCustomExceptionHandler();

    app.UseHsts(env);
    if (env.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference();

    }

    app.UseHttpsRedirection();

    app.UseElmah();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();
}
