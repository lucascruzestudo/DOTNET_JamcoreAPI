namespace Project.Infrastructure.Email;

public class EmailSettings
{
    public required string Server { get; set; }
    public required int Port { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required bool EnableSsl { get; set; }
    public string? ResendApiKey { get; set; }
    public string ResendFromEmail { get; set; } = "noreply@jamcore.app";
}