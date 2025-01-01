using CRMBackend.Entities;
namespace CRMBackend.Services;

public class CampaignServices
{
    private readonly AppDbContext _context;
    public CampaignServices(AppDbContext context)
    {
        _context = context;
    }
    public List<Campaign> GetCampaigns()
    {
        return _context.Campaigns.ToList();
    }
    public Campaign? GetCampaignById(int id)
    {
        return _context.Campaigns.FirstOrDefault(c => c.Id == id);
    }
    public Campaign CreateCampaign(Campaign campaign)
    {
        campaign.Id = 1;
        _context.Campaigns.Add(campaign);
        _context.SaveChanges();
        return campaign;
    }
    public void DeleteCampaign(int id)
    {
        var campaign = _context.Campaigns.FirstOrDefault(c => c.Id == id);
        if (campaign != null)
        {
            _context.Campaigns.Remove(campaign);
            _context.SaveChanges();
        }
    }


}
