namespace CRMBackend.Requests;

public class SubmitFeedbackRequest
{
    public string Username { get; set; }
    public string FeedbackType { get; set; }
    public string FeedbackContent { get; set; }
}


