using CRMBackend.Entities;

namespace CRMBackend.Services;

public class NotificationServices
{
    private readonly AppDbContext _context;
    public NotificationServices(AppDbContext context)
    {
        _context = context;
    }
    public List<Notification>? GetNotificationById(int notificationId)
    {
        return _context.Notifications.Where(n => n.NotificationId == notificationId).ToList();
    }
    public Notification? GetNotificationsByPlayerId(int playerId)
    {
        return _context.Notifications.FirstOrDefault(n => n.PlayerId == playerId);
    }
    public Notification MarkNotificationRead(Notification notification)
    {
        notification.IsRead = true;
        _context.Notifications.Update(notification);
        _context.SaveChanges();
        return notification;
    }
    public Notification CreateNotification(Notification notification)
    {
        _context.Notifications.Add(notification);
        _context.SaveChanges();
        return notification;
    }
    public void DeleteNotification(int playerId, int notificationId)
    {
        var notification = _context.Notifications.FirstOrDefault(n => n.PlayerId == playerId && n.NotificationId == notificationId);
        if (notification != null)
        {
            _context.Notifications.Remove(notification);
            _context.SaveChanges();
        }
    }
    public void UpdateNotification(Notification notification)
    {
        _context.Notifications.Update(notification);
        _context.SaveChanges();
    }
}
