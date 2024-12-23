using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CRMBackend.Entities;

[Table("game")]
public class Game
{
    [Key]
    [Column("game_id")]
    public int Id { get; set; }
    [Column("game_name")]
    public string Name { get; set; }
    [Column("game_genre")]
    public string Genre { get; set; }
}