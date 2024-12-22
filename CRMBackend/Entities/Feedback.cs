using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace CRMBackend.Entities;

[Table("feedback")]
public class Feedback
{
    [Key]
    [Column("feedback_id")]
    public int Id { get; set; }

    public string Name { get; set; }
    public string Email { get; set; }
    public string Message { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
