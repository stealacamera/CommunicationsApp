using CommunicationsApp.Application.DTOs;
using CommunicationsApp.Application.Operations.Messages.Commands.CreateMessage;
using CommunicationsApp.Application.Operations.Messages.Commands.DeleteMessage;
using CommunicationsApp.Application.Operations.Messages.Queries.GetAllMessagesForChannel;
using CommunicationsApp.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CommunicationsApp.Web.Controllers;

[Authorize]
public class MessagesController : BaseController
{
    private readonly static string _baseMediaFolder = "media",
        _imagesFolder = $"{_baseMediaFolder}/images",
        _documentsFolder = $"{_baseMediaFolder}/documents",
        _videosFolder = $"{_baseMediaFolder}/videos";

    private readonly IWebHostEnvironment _hostEnv;

    public MessagesController(IWebHostEnvironment hostEnv, IConfiguration configuration) : base(configuration)
    {
        _hostEnv = hostEnv;
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
    public async Task<IActionResult> Create(int id, Message_AddRequestModel message)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (message.Message.IsNullOrEmpty() && message.Media.IsNullOrEmpty())
            return BadRequest("Cannot send empty message");

        CreateMessageCommand command = new(message.Message, GetCurrentUserId(), id, message.Media);
        var createResult = await Sender.Send(command);

        if(createResult.Failed)
            return BadRequest(createResult.Error.Description);

        SaveFiles(message.Media, createResult.Value.Media);
        return Created(string.Empty, createResult.Value);
    }

    public async Task<IActionResult> Delete(int id)
    {
        DeleteMessageCommand command = new(id, GetCurrentUserId());
        var deleteResult = await Sender.Send(command);

        return deleteResult.Succeded 
               ? NoContent() 
               : BadRequest(deleteResult.Error.Description);
    }

    private void SaveFiles(IFormFileCollection files, IList<Media> media)
    {
        for (int i = 0; i < files.Count; i++)
        {
            string folderPath;
            var currFile = files[i];

            if (currFile.ContentType.Contains("image"))
                folderPath = _imagesFolder;
            else if (currFile.ContentType.Contains("video"))
                folderPath = _videosFolder;
            else
                folderPath = _documentsFolder;

            SaveFileToRootPath(_hostEnv.WebRootPath, folderPath, currFile, media[i].Filename);
        }
    }

    private void SaveFileToRootPath(string rootPath, string folderPath, IFormFile file, string filename)
    {
        var uploadPath = Path.Combine(rootPath, folderPath);
        var extension = Path.GetExtension(filename);

        Directory.CreateDirectory(uploadPath);
        using (var fileStr = new FileStream(Path.Combine(uploadPath, filename + extension), FileMode.Create))
        {
            file.CopyTo(fileStr);
        }
    }
}
