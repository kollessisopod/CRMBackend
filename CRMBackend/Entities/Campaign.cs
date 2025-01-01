using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace CRMBackend.Entities;

[Table("campaign")]
public class Campaign
{
    [Key]
    [Column("campaign_id")]
    public int Id { get; set; }
    [Column("campaign_info")]
    public string Info { get; set; }
    [Column("hasReward")]
    public bool HasReward { get; set; }
    [Column("reward_info")]
    public string? RewardInfo { get; set; }
}
