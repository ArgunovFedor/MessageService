var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");
// через третий клиент пользователь может отобразить историю сообщений за последние 10 минут
app.Run();