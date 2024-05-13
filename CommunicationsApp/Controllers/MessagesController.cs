using CommunicationsApp.Application.Operations.Messages.Commands.CreateMessage;
using CommunicationsApp.Application.Operations.Messages.Commands.DeleteMessage;
using CommunicationsApp.Application.Operations.Messages.Queries.GetAllMessagesForChannel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CommunicationsApp.Web.Controllers;

[Authorize]
public class MessagesController : BaseController
{
    public MessagesController(IConfiguration configuration) : base(configuration)
    {
    }

    [HttpGet("channels/{id:int}/messages")]
    public async Task<IActionResult> Get(int id, int cursor)
    {
        GetAllMessagesForChannelQuery query = new(id, GetCurrentUserId(), GetStandardPaginationSize(), cursor);
        var queryResult = await Sender.Send(query);

        return queryResult.Failed
               ? BadRequest(queryResult.Error.Description)
               : Ok(queryResult.Value);
    }

    [HttpPost("channels/{id:int}/messages")]
    public async Task<IActionResult> Create(int id, [FromBody, MinLength(1), MaxLength(1000)] string message)
    {
        CreateMessageCommand command = new(message, GetCurrentUserId(), id);
        var createResult = await Sender.Send(command);

        return createResult.Succeded
               ? Created(string.Empty, createResult.Value)
               : BadRequest(createResult.Error.Description);
    }

    public async Task<IActionResult> Delete(int id)
    {
        DeleteMessageCommand command = new(id, GetCurrentUserId());
        var deleteResult = await Sender.Send(command);

        return deleteResult.Succeded 
               ? NoContent() 
               : BadRequest(deleteResult.Error.Description);
    }
}
