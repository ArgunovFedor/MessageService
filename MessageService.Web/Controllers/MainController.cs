using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Aeb.DigitalPlatform.Infrastructure;
using Aeb.DigitalPlatform.Infrastructure.ExceptionHandlers;
using MediatR;
using MessageService.Abstractions.Messages;
using MessageService.Core.Requests.Messages;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace MessageService.Web.Controllers;

[ServiceFilter(typeof(ApiExceptionFilterAttribute))]
[Route("api/Main")]
public class MainController: ControllerBase
{
    private readonly IMediator _mediator;

    public MainController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    /// <summary>
    /// Отправить одно сообщение
    /// </summary>
    /// <param name="createMessage">MessageModel object</param>
    /// <returns>Created object</returns>
    [HttpPost]
    [SwaggerOperation("Send message")]
    public async Task<MessageModel> CreateMessageAsync([FromBody] CreateMessage createMessage)
    {
        // TODO: добавить отправку через прокси сервер
        // сервис обрабатывает каждое сообщение, записывает его в базу 
        var result =  await _mediator.Send(createMessage);
        // и перенаправляет его второму клиенту по веб-сокету

        return result;
    }

    /// <summary>
    /// Получить список сообщений за диапазон дат
    /// </summary>
    /// <param name="pageIndex">Required page index</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="selectStart"></param>
    /// <param name="selectEnd"></param>
    /// <returns>List of Messages</returns>
    [HttpGet("")]
    [SwaggerOperation("GetMessages")]
    public async Task<IEnumerable<MessageModel>> GetMessagesAsync(DateTime? selectStart, DateTime? selectEnd)
    {
        return await _mediator.Send(new GetMessages());
    }
}