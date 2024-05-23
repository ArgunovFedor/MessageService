using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MessageService.Core.Infrastructure;
using MessageService.Core.Requests.Messages;
using MessageService.Web.Infrastructure;
using MessageService.Abstractions;
using MessageService.Abstractions.Messages;
using Aeb.DigitalPlatform.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace MessageService.Web.Controllers;

[ServiceFilter(typeof(ApiExceptionFilterAttribute))]
[Authorize]
[Route("api/Message")]
public class MessageController : ControllerBase
{
    private readonly IMediator _mediator;

    public MessageController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Пропатчить запись `Сообщение` 
    /// </summary>
    /// <param name="id">Identifier</param>
    /// <param name="content">JSON Patch document for MessageModel</param>
    /// <returns>Modified object</returns>
    [HttpPatch("{id}")]
    [SwaggerOperation("PatchMessage")]
    public async Task<MessageModel> PatchMessageAsync(Guid id, [FromBody] JsonPatchDocument<MessageModel> content)
    {
        return await _mediator.Send(new PatchMessage(id, content));
    }

    /// <summary>
    /// Получить список записей `Сообщение`
    /// </summary>
    /// <returns>List of Messages</returns>
    [HttpGet]
    [SwaggerOperation("GetMessages")]
    public async Task<IEnumerable<MessageModel>> GetMessagesAsync()
    {
        return await _mediator.Send(new GetMessages());
    }

    /// <summary>
    /// Получить список записей `Сообщение` с пагинацией
    /// </summary>
    /// <param name="pageIndex">Required page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>List of Messages</returns>
    [HttpGet("page/{pageIndex}")]
    [SwaggerOperation("GetMessagesPage")]
    public async Task<PaginableContentModel<MessageModel>> GetMessagesAsync(int pageIndex, int pageSize = 30)
    {
        return await _mediator.Send(new GetMessagesPage(pageIndex, pageSize));
    }

    /// <summary>
    /// Получить запись `Сообщение`
    /// </summary>
    /// <param name="id">Identifier</param>
    /// <returns>MessageModel object</returns>
    [HttpGet("{id}")]
    [SwaggerOperation("GetMessage")]
    public async Task<MessageModel> GetMessageAsync(Guid id)
    {
        return await _mediator.Send(new GetMessage(id));
    }

    /// <summary>
    /// Обновить запись `Сообщение`
    /// </summary>
    /// <param name="id">Identifier</param>
    /// <param name="updateMessage">MessageModel object</param>
    /// <returns>Modified object</returns>
    [HttpPut("{id}")]
    [SwaggerOperation("UpdateMessage")]
    public async Task<MessageModel> UpdateMessageAsync(Guid id, [FromBody] UpdateMessage updateMessage)
    {
        if (id != updateMessage.Id)
        {
            throw new ServiceException("BAD_REQUEST");
        }
        return await _mediator.Send(updateMessage);
    }

    /// <summary>
    /// Создать новую запись `Сообщение`
    /// </summary>
    /// <param name="createMessage">MessageModel object</param>
    /// <returns>Created object</returns>
    [HttpPost]
    [SwaggerOperation("CreateMessage")]
    public async Task<MessageModel> CreateMessageAsync([FromBody] CreateMessage createMessage)
    {
        return await _mediator.Send(createMessage);
    }

    /// <summary>
    /// Удалить запись `Сообщение`
    /// </summary>
    /// <param name="id">Identifier</param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    [SwaggerOperation("DeleteMessage")]
    public async Task<IActionResult> DeleteMessageAsync(Guid id)
    {
        await _mediator.Send(new DeleteMessage(id));
        return StatusCode(204);
    }
}