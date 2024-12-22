using CRMBackend.Entities;
namespace CRMBackend.Services;

public class FeedbackServices
{
    /*
    private readonly AppDbContext _context;
    public FeedbackServices(AppDbContext context)
    {
        _context = context;
    }
    public List<Feedback> GetFeedbacks()
    {
        return _context.Feedbacks.ToList();
    }
    public Feedback? GetFeedbackById(int id)
    {
        return _context.Feedbacks.FirstOrDefault(f => f.Id == id);
    }
    public Feedback CreateFeedback(Feedback feedback)
    {
        _context.Feedbacks.Add(feedback);
        _context.SaveChanges();
        return feedback;
    }
    public void DeleteFeedback(int id)
    {
        var feedback = _context.Feedbacks.FirstOrDefault(f => f.Id == id);
        if (feedback != null)
        {
            _context.Feedbacks.Remove(feedback);
            _context.SaveChanges();
        }
    }
    */
}
