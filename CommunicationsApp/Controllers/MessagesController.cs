using CommunicationsApp.Application.DTOs;
using CommunicationsApp.Application.Operations.Messages.Commands.CreateMessage;
using CommunicationsApp.Application.Operations.Messages.Commands.DeleteMessage;
using CommunicationsApp.Application.Operations.Messages.Queries.GetAllMessagesForChannel;
using CommunicationsApp.Application.Operations.Multimedia.Queries.GetAllForMessage;
using CommunicationsApp.Domain.Common.Enums;
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

        if (message.Text.IsNullOrEmpty() && message.Media.IsNullOrEmpty())
            return StatusCode(StatusCodes.Status422UnprocessableEntity, "Cannot send empty message");

        CreateMessageCommand command = new(message.Text, GetCurrentUserId(), id, message.Media);
        var createResult = await Sender.Send(command);

        if (createResult.Failed)
            return BadRequest(createResult.Error.Description);

        SaveFiles(message.Media, createResult.Value.Media);
        return Created(string.Empty, createResult.Value);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(int id)
    {
        var messageMediaResult = await Sender.Send(new GetAllMediaForMessageQuery(id));

        if (messageMediaResult.Failed)
            return NotFound();

        DeleteMessageCommand command = new(id, GetCurrentUserId());
        var deleteResult = await Sender.Send(command);

        if (deleteResult.Succeded)
        {
            RemoveFiles(messageMediaResult.Value);

            Response.StatusCode = StatusCodes.Status204NoContent;
            return new JsonResult(deleteResult.Value.Id);
        }
        else
            return BadRequest(deleteResult.Error.Description);
    }

    // todo: move to inotification? possible event
    private void RemoveFiles(IList<Media> files)
    {
        if (files == null)
            return;

        foreach(var file in files)
        {
            string folder;

            if (file.Type == MediaType.Image)
                folder = _imagesFolder;
            else if (file.Type == MediaType.Video)
                folder = _videosFolder;
            else
                folder = _documentsFolder;

            RemoveFileFromRootPath(_hostEnv.WebRootPath, folder, file.Filename);
        }
    }

    private void RemoveFileFromRootPath(string rootPath, string folderPath, string fileName)
    {
        string path = Path.Combine(rootPath, folderPath, fileName);

        if (System.IO.File.Exists(path))
            System.IO.File.Delete(path);
    }

    private void SaveFiles(IFormFileCollection files, IList<Media> media)
    {
        if (files == null)
            return;

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

        Directory.CreateDirectory(uploadPath);
        using (var fileStr = new FileStream(Path.Combine(uploadPath, filename), FileMode.Create))
        {
            file.CopyTo(fileStr);
        }
    }
}
