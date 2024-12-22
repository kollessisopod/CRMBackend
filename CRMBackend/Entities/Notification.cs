using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CRMBackend.Entities;

[Table("notification")]
public class Notification
{
    [Column("playerid")]
    public int PlayerId { get; set; }
    [Column("content")]
    public string Content { get; set; }
    [Column("is_read")]
    public bool IsRead { get; set; }
}
