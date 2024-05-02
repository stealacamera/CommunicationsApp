using CommunicationsApp.Application.DTOs;
using CommunicationsApp.Application.Operations.Messages.Commands.CreateMessage;
using CommunicationsApp.Application.Operations.Messages.Commands.DeleteMessage;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CommunicationsApp.Web.Controllers
{
    public class MessagesController : BaseController
    {
        [HttpPost("channels/{id:int}/messages")]
        public async Task<IActionResult> Create(int id, [FromBody, MinLength(1), MaxLength(1000)] string message)
        {
            CreateMessageCommand command = new(message, GetCurrentUserId(), id);
            var createResult = await Sender.Send(command);

            return createResult.Succeded
                   ? Created(string.Empty, createResult.Value)
                   : BadRequest(createResult.Error.Description);
        }

        public async Task<IActionResult> Delete(int messageId)
        {
            DeleteMessageCommand command = new(messageId, GetCurrentUserId());
            var deleteResult = await Sender.Send(command);

            return deleteResult.Succeded 
                   ? NoContent() 
                   : BadRequest(deleteResult.Error.Description);
        }
    }
}
