using MeditationCountBot.Services;
using MeditationCountBot.Telegram;
using MeditationCountBot.Workers;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddTransient<ITelegramMessageSender, TelegramMessageSender>();
builder.Services.AddTransient<IJsonLoader, JsonLoader>();
builder.Services.AddTransient<IJsonLogger, JsonLogger>();
builder.Services.AddTransient<ICalculateContinuouslyService, CalculateContinuouslyService>();
builder.Services.AddTransient<ITimeFormatter, TimeFormatter>();
builder.Services.AddTransient<IMessageFormer, MessageFormer>();
builder.Services.AddSingleton<ICounterService, CounterService>();
builder.Services.AddSingleton<ITelegramBotService, TelegramBotService>();

// builder.Services.AddHostedService<CounterWorker>();

var telegramBotKey = Environment.GetEnvironmentVariable("TELEGRAM_BOT_KEY");

var app = builder.Build();
var telegramBotService = app.Services.GetRequiredService<ITelegramBotService>();
var counterService = app.Services.GetRequiredService<ICounterService>();
telegramBotService.Initialize(telegramBotKey);
await counterService.Initialize();
app.MapControllers();
app.MapGet("/", () => "Hello World!");
app.MapGet("/message", () => "Ok");

app.Run();