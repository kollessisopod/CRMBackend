using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CRMBackend.Entities;

[Table("notification")]
public class Notification
{
    [Key]
    [Column("not_id")]
    public int NotificationId { get; set; }
    [Column("reciever_id")]
    public int PlayerId { get; set; }
    [Column("not_info")]
    public string Content { get; set; }
    [Column("is_read")]
    public bool IsRead { get; set; }
}
