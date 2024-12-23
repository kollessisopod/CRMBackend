using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CRMBackend.Entities;

[Table("player_game")]
public class PlayerGame
{
    [Column("p_id")]
    public int PlayerId { get; set; }
    [Column("g_id")]
    public int GameId { get; set; }
    [Column("score")]
    public int Score { get; set; }
}
