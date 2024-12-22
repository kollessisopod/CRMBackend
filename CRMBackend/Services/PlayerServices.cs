using CRMBackend.Entities;
namespace CRMBackend.Services;

public class PlayerServices
{
    private readonly AppDbContext _context;
    public PlayerServices(AppDbContext context)
    {
        _context = context;
    }
    public List<Player> GetPlayers()
    {
        return _context.Players.ToList();
    }
    public Player? GetPlayerById(string username)
    {
        return _context.Players.FirstOrDefault(x => x.Username == username);
    }
    public Player CreatePlayer(Player player)
    {
        _context.Players.Add(player);
        _context.SaveChanges();
        return player;
    }

    public bool DeletePlayer(string username)
    {
        var existingPlayer = _context.Players.FirstOrDefault(x => x.Username == username);
        if (existingPlayer == null) return false;
        _context.Players.Remove(existingPlayer);
        _context.SaveChanges();
        return true;
    }
}
