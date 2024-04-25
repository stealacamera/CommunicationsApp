﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CommunicationsApp.Web.Models;

public record SignUpDTO
{
    [Required]
    public string Username { get; set; } = null!;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    [PasswordPropertyText]
    public string Password { get; set; } = null!;
}
