using CRMBackend.Entities;
using CRMBackend.Requests;
using Microsoft.AspNetCore.Mvc;
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
    public Player? GetPlayerByUsername(string username)
    {
        return _context.Players.FirstOrDefault(x => x.Username == username);
    }

    public Player? GetPlayerById(int id)
    {
        return _context.Players.FirstOrDefault(x => x.Id == id);
    }

    public Player CreatePlayer(Player player)
    {
        player.LastOnline = DateTime.Now;  
        player.Id = 1;
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
