using System.ComponentModel.DataAnnotations.Schema;

namespace CRMBackend.Requests;

public class SendNotificationRequest
{
    public string Content { get; set; }
}
