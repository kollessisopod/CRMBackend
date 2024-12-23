namespace CRMBackend.Requests;

public class RateGameRequest
{
    public required string Username { get; set; }
    public int GameId { get; set; }
    public int Score { get; set; }
}
