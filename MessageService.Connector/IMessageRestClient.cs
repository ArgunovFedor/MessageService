using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MessageService.Abstractions.Messages;
using Refit;

namespace MessageService.Connector;

[Headers("Content-Type: application/json")]
public interface IMessageRestClient
{
    [Post("/api/Main")]
    Task<MessageModel> CreateUserSoundSettingAsync(CreateMessageModel createUserSoundSetting);
    
    [Get("/api/Main")]
    Task<IEnumerable<MessageModel>> GetPushSoundsAsync(DateTime? selectStart, DateTime? selectEnd);
}