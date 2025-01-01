namespace CRMBackend.Requests;

public class PlayerLoginRequest
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}
