using System;
using MessageService.Abstractions.Messages;
using MessageService.Connector;
using Refit;

var serverUrl = "http://localhost:5000";
var refitClient = RestService.For<IMessageRestClient>(serverUrl);
var number = 1;
// первый клиент пишет потоком произвольные (по контенту) сообщения в сервис (на одно сообщение один вызов к API)
while (true)
{
    var result = await refitClient.CreateUserSoundSettingAsync(new CreateMessageModel()
    {
        Number = number,
        Text = $"Сообщение №{number}"
    });
    Console.WriteLine($"Сообщение №{number} отправлено c id {result.Id}");
    number++;
}