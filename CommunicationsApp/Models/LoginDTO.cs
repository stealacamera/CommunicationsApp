using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CommunicationsApp.Web.Models;

public record LoginDTO
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;

    [Required]
    [DisplayName("Remember me")]
    public bool RememberMe { get; set; } = false;

    [ValidateNever]
    public IEnumerable<AuthenticationScheme> AuthSchemes { get; set; }
}
