using AutoMapper;
using MessageService.Core.Entities;
using MessageService.Core.Requests.Messages;
using MessageService.Abstractions.Messages;

namespace MessageService.Core.AutoMapper;

public class MessageProfile : Profile
{
    public MessageProfile()
    {
        CreateMap<Message, MessageModel>().ReverseMap();
        CreateMap<Message, MessageShortModel>();
        CreateMap<CreateMessage, Message>();
        CreateMap<UpdateMessage, Message>();
    }
}
