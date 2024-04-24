namespace CommunicationsApp.Infrastructure.Email;

public sealed record EmailOptions
{
    public static string ConfigurationName = "EmailSettings";

    public string Host { get; set; } = null!;
    public int Port { get; set; }

    public string InfoDeskName { get; set; } = null!;
    public string InfoDeskEmail { get; set; } = null!;
    public string InfoDeskPassword { get; set; } = null!;
}
