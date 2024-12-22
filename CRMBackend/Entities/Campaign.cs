using System.Reflection.Metadata;

namespace CRMBackend.Entities;

public class Campaign
{
    public int Id { get; set; }
    public string Info { get; set; }
    public bool HasReward { get; set; }
    public string RewardInfo { get; set; }
}
