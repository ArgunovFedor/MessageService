var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");
// второй клиент при считывает по веб-сокету поток сообщений от сервера и отображает их в порядке прихода с сервера (с отображением метки времени и порядкового номера)
app.Run();