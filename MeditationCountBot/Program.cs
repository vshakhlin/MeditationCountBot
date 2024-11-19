using MeditationCountBot.Services;
using MeditationCountBot.Telegram;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddLogging(b =>
{
    var logger = new LoggerConfiguration()
        .MinimumLevel.Debug()
        .WriteTo.File(Path.Combine("../logs", "log.txt"), rollingInterval: RollingInterval.Day)
        .CreateLogger();

    b.AddSerilog(logger);
});
builder.Services.AddControllers();
builder.Services.AddTransient<ITelegramMessageSender, TelegramMessageSender>();
builder.Services.AddTransient<IJsonLoader, JsonLoader>();
builder.Services.AddTransient<ICalculateContinuouslyService, CalculateContinuouslyService>();
builder.Services.AddTransient<ITimeFormatter, TimeFormatter>();
builder.Services.AddTransient<IMessageFormer, MessageFormer>();
builder.Services.AddTransient<ICalculateResultService, CalculateResultService>();
builder.Services.AddTransient<IPraiseAndCheerMessage, PraiseAndCheerMessage>();
builder.Services.AddSingleton<IMessagesStore, MessagesStore>();
builder.Services.AddSingleton<IDateTimeService, DateTimeService>();
builder.Services.AddSingleton<ICounterService, CounterService>();
builder.Services.AddSingleton<ITelegramBotService, TelegramBotService>();

// builder.Services.AddHostedService<CounterWorker>();

var telegramBotKey = Environment.GetEnvironmentVariable("TELEGRAM_BOT_KEY");
var telegramBotUsername = Environment.GetEnvironmentVariable("TELEGRAM_BOT_USERNAME");
var timeZoneOffsetHours = int.Parse(Environment.GetEnvironmentVariable("TIME_ZONE_OFFSET_HOURS"));

var app = builder.Build();
var dateTimeService = app.Services.GetRequiredService<IDateTimeService>();
dateTimeService.Initialize(timeZoneOffsetHours);
var telegramBotService = app.Services.GetRequiredService<ITelegramBotService>();
telegramBotService.Initialize(telegramBotKey, telegramBotUsername);
var messagesStore = app.Services.GetRequiredService<IMessagesStore>();
await messagesStore.Initialize();
var counterService = app.Services.GetRequiredService<ICounterService>();
await counterService.Initialize();

app.MapControllers();
app.MapGet("/", () => "Service is started");
app.MapGet("/message", () => "Ok");

app.Run();