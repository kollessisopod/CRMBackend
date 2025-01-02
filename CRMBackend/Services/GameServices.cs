using CRMBackend.Entities;

namespace CRMBackend.Services;

public class GameServices
{
    private readonly AppDbContext _context;
    public GameServices(AppDbContext context)
    {
        _context = context;
    }
    public List<Game> GetGames()
    {
        return _context.Games.ToList();
    }
    public Game? GetGameByName(string name)
    {
        return _context.Games.FirstOrDefault(g => g.Name == name);
    }
    public Game? GetGameById(int id)
    {
        return _context.Games.FirstOrDefault(g => g.Id == id);
    }
    public Game CreateGame(Game game)
    {
        _context.Games.Add(game);
        _context.SaveChanges();
        return game;
    }
    public void DeleteGame(string name)
    {
        var game = _context.Games.FirstOrDefault(g => g.Name == name);
        if (game != null)
        {
            _context.Games.Remove(game);
            _context.SaveChanges();
        }
    }
}
