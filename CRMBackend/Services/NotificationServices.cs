using CRMBackend.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CRMBackend.Services;

public class NotificationServices
{
    private readonly AppDbContext _context;
    public NotificationServices(AppDbContext context)
    {
        _context = context;
    }
    public Notification? GetNotificationById(int notificationId)
    {
        return _context.Notifications.FirstOrDefault(n => n.NotificationId == notificationId);
    }
    public Notification? GetNotificationsByPlayerId(int playerId)
    {
        return _context.Notifications.FirstOrDefault(n => n.PlayerId == playerId);
    }
    public List<Notification>? GetUnreadNotificationsByPlayerId(int playerId)
    {
        return _context.Notifications.Where(n => n.PlayerId == playerId && !n.IsRead).ToList();
    }

    public Notification MarkNotificationRead(Notification notification)
    {
        notification.IsRead = !notification.IsRead;
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

    public int GetUnreadNotificationCount(int playerId)
    {
        return _context.Notifications.Count(n => n.PlayerId == playerId && !n.IsRead);
    }

    public int GetNotificationCount(int playerId)
    {
        return _context.Notifications.Count(n => n.PlayerId == playerId);
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
