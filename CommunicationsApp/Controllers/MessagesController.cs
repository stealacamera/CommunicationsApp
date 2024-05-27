using CommunicationsApp.Application.Behaviour.Operations.Messages.Commands.CreateMessage;
using CommunicationsApp.Application.Behaviour.Operations.Messages.Commands.DeleteMessage;
using CommunicationsApp.Application.Behaviour.Operations.Messages.Queries.GetAllMessagesForChannel;
using CommunicationsApp.Application.DTOs;
using CommunicationsApp.Application.Behaviour.Operations.Multimedia.Queries.GetAllForMessage;
using CommunicationsApp.Domain.Common.Enums;
using CommunicationsApp.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using FluentValidation.AspNetCore;

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

    #region API
    [HttpGet("channels/{id:int}/messages")]
    public async Task<IActionResult> Get(int id, [Range(0, int.MaxValue)] int cursor)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        GetAllMessagesForChannelQuery query = new(id, GetCurrentUserId(), StandardPaginationSize, cursor);
        var queryResult = await Sender.Send(query);

        return ConvertResultToResponse(queryResult, () => Ok(queryResult.Value));
    }

    [HttpPost("channels/{id:int}/messages")]
    public async Task<IActionResult> Create(int id, Message_AddRequestModel message)
    {
        var validationResult = await new MessageValidator().ValidateAsync(message);

        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            return BadRequest(ModelState);
        }

        CreateMessageCommand command = new(message.Text, GetCurrentUserId(), id, message.Media);
        var createResult = await Sender.Send(command);

        return ConvertResultToResponse(
            createResult,
            () =>
            {
                SaveFiles(message.Media, createResult.Value.Media);
                return Created(string.Empty, createResult.Value);
            });
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(int id)
    {
        GetAllMediaForMessageQuery mediaQuery = new(id);
        var mediaResult = await Sender.Send(mediaQuery);

        if (mediaResult.Failed)
            return ConvertFailureToResponse(mediaResult);

        DeleteMessageCommand command = new(id, GetCurrentUserId());
        var deleteResult = await Sender.Send(command);

        return ConvertResultToResponse(
            deleteResult,
            () =>
            {
                RemoveFiles(mediaResult.Value);
                return NoContent();
            });
    }
    #endregion

    private void RemoveFiles(IList<Media> files)
    {
        if (files == null)
            return;

        foreach (var file in files)
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

    private void SaveFiles(IFormFileCollection? files, IList<Media>? media)
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
