using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using Telegram.Bot;
using VideoStickerBot;
using VideoStickerBot.Database;
using VideoStickerBot.Services.TelegramIntegration;
using VideoStickerBot.Services.VideoResize;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(8080);
    //serverOptions.ListenAnyIP(8443, listenOptions => listenOptions.UseHttps());
});

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddControllers().AddNewtonsoftJson();

Serilog.Debugging.SelfLog.Enable(Console.WriteLine);

var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(
        new Uri(builder.Configuration["ElasticSearch:Uri"]))
    {
        AutoRegisterTemplate = true,
        OverwriteTemplate = true,
        BatchAction = ElasticOpType.Create,
        AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
        IndexFormat = $"{builder.Configuration["BOT_DOMAIN_NAME"]}-{DateTime.UtcNow:yyyy-MM}",
        ModifyConnectionSettings = x => x.BasicAuthentication(builder.Configuration["ElasticSearch:Login"],
                builder.Configuration["ElasticSearch:Password"])
        .ServerCertificateValidationCallback((sender, cert, chain, errors) => true),

        FailureCallback = e => Console.WriteLine(e.MessageTemplate),
        EmitEventFailure = EmitEventFailureHandling.WriteToSelfLog |
                                        EmitEventFailureHandling.WriteToFailureSink |
                                        EmitEventFailureHandling.RaiseCallback,
    })
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

builder.Services.Configure<AspNetCoreTraceInstrumentationOptions>(options =>
{
    // Filter out instrumentation of the Prometheus scraping endpoint.
    options.Filter = ctx => ctx.Request.Path != "/metrics";
});

builder.Services.AddOpenTelemetry()
    .ConfigureResource(b =>
    {
        b.AddService(builder.Configuration["BOT_DOMAIN_NAME"]!);
    })
    .WithTracing(b => b
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddEntityFrameworkCoreInstrumentation()
        .AddOtlpExporter())
    .WithMetrics(b => b
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddRuntimeInstrumentation()
        .AddProcessInstrumentation()
        .AddPrometheusExporter());

builder.Services.AddDbContext<VideoStikersBotContext>(
        options => options.UseNpgsql(builder.Configuration["ytTgBotDb"]), ServiceLifetime.Scoped);

builder.Services.AddScoped<IVideoResize>(p => new VideoResizeApi(builder.Configuration["VideoResizeApiUrl"]));
builder.Services.AddScoped<ITelegramBotClient, TelegramBotClient>((service) =>
{
    return new TelegramBotClient(builder.Configuration["VIDEOSTICK_BOT_TOKEN"]);
});
builder.Services.AddScoped<ITelegram, TelegramAdapter>();

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

app.UseOpenTelemetryPrometheusScrapingEndpoint();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();