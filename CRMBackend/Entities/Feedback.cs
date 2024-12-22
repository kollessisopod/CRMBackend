using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace CRMBackend.Entities;

[Table("feedback")]
public class Feedback
{
    [Key]
    [Column("feedback_id")]
    public int Id { get; set; }

    [Column("sender_id")]
    public int PlayerId { get; set; }

    [Column("sender_name")]
    public string PlayerUsername { get; set; }

    [Column("feedback_type")]
    public string FeedbackType { get; set; }

    [Column("feedback_info")]
    public string FeedbackContent { get; set; }

}
