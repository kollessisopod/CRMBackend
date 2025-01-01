using CRMBackend.Entities;
using CRMBackend.Requests;

namespace CRMBackend.Services;

public class FeedbackServices
{
    
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
        try
        {
        feedback.Id = 1;
        feedback.FeedbackDate = DateTime.Now;
        if (feedback.FeedbackDate.Kind != DateTimeKind.Utc)
        {
            feedback.FeedbackDate = feedback.FeedbackDate.ToUniversalTime();
        }
        _context.Feedbacks.Add(feedback);
        _context.SaveChanges();
        } catch (Exception ex)
        {
            Console.WriteLine(ex);
            return null;
        }
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
}
