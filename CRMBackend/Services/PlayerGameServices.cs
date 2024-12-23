using CRMBackend.Entities;

namespace CRMBackend.Services;

public class PlayerGameServices
{
    private readonly AppDbContext _context;
    public PlayerGameServices(AppDbContext context)
    {
        _context = context;
    }
    public List<PlayerGame>? GetPlayerGamesByPlayerId(int playerId)
    {
        return _context.PlayerGames.Where(pg => pg.PlayerId == playerId).ToList();
    }
    public PlayerGame? GetPlayerGameByPlayerAndGameId(int playerId, int gameId)
    {
        return _context.PlayerGames.FirstOrDefault(pg => pg.PlayerId == playerId && pg.GameId == gameId);
    }
    public PlayerGame CreatePlayerGame(PlayerGame playerGame)
    {
        _context.PlayerGames.Add(playerGame);
        _context.SaveChanges();
        return playerGame;
    }
    public void DeletePlayerGame(int playerId, int gameId)
    {
        var playerGame = _context.PlayerGames.FirstOrDefault(pg => pg.PlayerId == playerId && pg.GameId == gameId);
        if (playerGame != null)
        {
            _context.PlayerGames.Remove(playerGame);
            _context.SaveChanges();
        }
    }

    public void UpdatePlayerGame(PlayerGame playerGame)
    {
        _context.PlayerGames.Update(playerGame);
        _context.SaveChanges();
    }

}
