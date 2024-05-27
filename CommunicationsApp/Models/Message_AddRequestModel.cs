using FluentValidation;

namespace CommunicationsApp.Web.Models;

public class Message_AddRequestModel
{
    public string? Text { get; set; }
    public IFormFileCollection? Media { get; set; }
}

internal class MessageValidator : AbstractValidator<Message_AddRequestModel>
{
    public MessageValidator()
    {
        string[] acceptableExtensions = {
            ".zip", ".json", ".pdf", ".xml", ".mp3", 
            ".mp4", ".gif", ".png", ".jpg", ".jpeg", 
            ".html", ".js", ".txt", ".docx"};

        RuleFor(e => e.Text)
            .MaximumLength(1000);

        RuleFor(e => e.Media)
            .Must(e => e.Count <= 6)
            .WithMessage("Cannot upload more than 6 files")
            .Must(e =>
            {
                var filesExtensions = e.Select(file => Path.GetExtension(file.FileName).ToLower())
                                      .Distinct()
                                      .ToList();

                return !filesExtensions.Except(acceptableExtensions).Any();
            })
            .WithMessage("Unaccepted file type")
            .Must(e => e.All(file => file.Length <= 1_048_576))
            .WithMessage("File size too big")
            .When(e => e.Media != null);
    }
}