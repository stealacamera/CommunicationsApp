using CommunicationsApp.Web.Common.Attributes;
using System.ComponentModel.DataAnnotations;

namespace CommunicationsApp.Web.Models;

public class Message_AddRequestModel
{
    [StringLength(1000)]
    public string? Message { get; set; }

    [MaxFormFileMBSize(10)]
    [FormFileExtensions(".png, .jpg, .jpeg, .gif, .mp4, .pdf")]
    [MaxLength(6)]
    public IFormFileCollection? Media { get; set; }
}
