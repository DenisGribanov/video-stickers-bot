using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using NLog.Web;
using Telegram.Bot;
using VideoStickerBot;
using VideoStickerBot.Bot;
using VideoStickerBot.Database;
using VideoStickerBot.Services;
using VideoStickerBot.Services.TelegramIntegration;
using VideoStickerBot.Services.VideoResize;

var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings(Directory.GetCurrentDirectory()).GetCurrentClassLogger();
logger.Debug("init main");

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.WebHost.UseKestrel(serverOptions =>
    {
        serverOptions.ListenAnyIP(8080);
        //serverOptions.ListenAnyIP(8443, listenOptions => listenOptions.UseHttps());
    });

    // Add services to the container.
    builder.Services.AddControllersWithViews();

    builder.Services.AddControllers().AddNewtonsoftJson();

    // NLog: Setup NLog for Dependency injection
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();
    builder.Services.AddDbContext<VideoStikersBotContext>(
         options => options.UseNpgsql(builder.Configuration["ytTgBotDb"]), ServiceLifetime.Scoped);

    builder.Services.AddScoped<IVideoResize>(p => new VideoResizeApi(builder.Configuration["VideoResizeApiUrl"]));
    builder.Services.AddScoped<ITelegram>(p => new TelegramAdapter(new TelegramBotClient(builder.Configuration["VIDEOSTICK_BOT_TOKEN"])));

    if (builder.Environment.IsDevelopment())
    {
        builder.Configuration.AddEnvironmentVariables(prefix: "Development");
    }

    var app = builder.Build();

    Variables.InitInstance(builder.Configuration);


    //app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthorization();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");


    app.Run();
}
catch (Exception exception)
{
    // NLog: catch setup errors
    logger.Error(exception, "Stopped program because of exception");
    throw;
}
finally
{
    // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
    NLog.LogManager.Shutdown();
}