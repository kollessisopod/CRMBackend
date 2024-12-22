using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CRMBackend.Entities;

[Table("player")]
public class Player
{
    [Key]
    [Column("player_id")]
    public int Id { get; set; }
    [Column("p_name")]
    public string Username { get; set; }
    [Column("p_password")]
    public string Password { get; set; }
    [Column("p_email")]
    public string Email { get; set; }
    [Column("p_pnumber")]
    public string PhoneNumber { get; set; }
    [Column("last_online")]
    public DateTime LastOnline { get; set; }
}
