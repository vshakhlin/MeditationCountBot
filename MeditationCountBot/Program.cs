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

if (!bool.TryParse(Environment.GetEnvironmentVariable("IS_HABIT"), out bool isHabit))
{
    isHabit = false;
}

builder.Services.AddControllers();
builder.Services.AddTransient<ITelegramMessageSender, TelegramMessageSender>();
builder.Services.AddTransient<IJsonLoader, JsonLoader>();
builder.Services.AddTransient<ICalculateContinuouslyService, CalculateContinuouslyService>();
builder.Services.AddTransient<IFreeTextMessageSender, FreeTextMessageSender>();
builder.Services.AddTransient<IReCalculateResultService, ReCalculateResultService>();
builder.Services.AddTransient<ILogReader, LogReader>();
builder.Services.AddTransient<ITimeFormatter, TimeFormatter>();
if (isHabit)
{
    builder.Services.AddTransient<IMessageProvider, HabitMessageProvider>();
}
else
{
    builder.Services.AddTransient<IMessageProvider, MeditationMessageProvider>();
}

builder.Services.AddTransient<IMessageFormer, MessageFormer>();
builder.Services.AddTransient<ICalculateResultService, CalculateResultService>();
builder.Services.AddTransient<IPraiseAndCheerMessage, PraiseAndCheerMessage>();
builder.Services.AddSingleton<IMessagesStore, MessagesStore>();
builder.Services.AddSingleton<IDateTimeService, DateTimeService>();
builder.Services.AddSingleton<ICounterService, CounterService>();
builder.Services.AddSingleton<ITelegramBotService, TelegramBotService>();
builder.Services.AddSingleton<IMigrationService, MigrationService>();

// builder.Services.AddHostedService<CounterWorker>();

var telegramBotKey = Environment.GetEnvironmentVariable("TELEGRAM_BOT_KEY");
var telegramBotUsername = Environment.GetEnvironmentVariable("TELEGRAM_BOT_USERNAME");

var app = builder.Build();
var telegramBotService = app.Services.GetRequiredService<ITelegramBotService>();
telegramBotService.Initialize(telegramBotKey, telegramBotUsername);
var messagesStore = app.Services.GetRequiredService<IMessagesStore>();
await messagesStore.Initialize();
var counterService = app.Services.GetRequiredService<ICounterService>();
await counterService.Initialize();
// var migrationService = app.Services.GetRequiredService<IMigrationService>();
// await migrationService.Migrate();

app.MapControllers();
app.MapGet("/", () => "Service is started");
app.MapGet("/message", () => "Ok");

app.Run();