using CommunicationsApp.Web.Common.Attributes;
using System.ComponentModel.DataAnnotations;

namespace CommunicationsApp.Web.Models;

public class Message_AddRequestModel
{
    [StringLength(1000, ErrorMessage = "Message cannot be longer than 1000 characters")]
    public string? Text { get; set; }

    [MaxFormFileMBSize(10)]
    [FormFileExtensions(".png, .jpg, .jpeg, .gif, .mp4, .pdf, .txt")]
    [MaxLength(6, ErrorMessage = "Cannot send more than 6 files")]
    public IFormFileCollection? Media { get; set; }
}
