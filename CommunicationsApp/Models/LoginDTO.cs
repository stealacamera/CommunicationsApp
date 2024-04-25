using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace CommunicationsApp.Web.Models;

public record LoginDTO
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    [PasswordPropertyText]
    public string Password { get; set; } = null!;

    [Required]
    public bool RememberMe { get; set; } = true;
}
