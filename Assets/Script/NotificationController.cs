using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if Notification

#if UNITY_ANDROID
using UnityEngine.Android;
using Unity.Notifications.Android;

# elif UNITY_IOS
using Unity.Notifications.iOS;

#endif

#endif
public class NotificationController : MonoBehaviour
{
#if Notification

#if UNITY_ANDROID
    public void RequestAuthorization()
    {
        if (!Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATION"))
        {
            Permission.RequestUserPermission("android.permission.POST_NOTIFICATION");
        }
    }

    public void RegisterNotificationChannel()
    {
        var channel = new AndroidNotificationChannel()
        {
            Id = "default_channel",
            Name = "Default Channel",
            Importance = Importance.Default,
            Description = "Generic notification"

        };

        AndroidNotificationCenter.RegisterNotificationChannel(channel);
    }



    public void SendNotification(string title,string body, int showTime)
    {
        var notification = new AndroidNotification();

        notification.Title = title;
        notification.Text = body;
        notification.SmallIcon = "icon_2";
        //notification.LargeIcon = "icon_1";
        notification.FireTime = System.DateTime.Now.AddMinutes(showTime);

        AndroidNotificationCenter.SendNotification(notification, "default_channel");
        
    }

#elif UNITY_IOS
    public IEnumerator RequestAuthorization()
    {
        using var req = new AuthorizationRequest(AuthorizationOption.Alert |
            AuthorizationOption.Badge,
            true);

        while(!req.IsFinished)
        {
            yield return null;
        }

    }

    public void SendNotification(string title, string body,string subtitle,int showtime)
    {
        var timeTrigger = new iOSNotificationTimeIntervalTrigger()
        {
            TimeInterval = new System.TimeSpan(showtime, 0, 0),
            Repeats = false
        };

        var notification = new iOSNotification()
        {
            Identifier = "Hello Ahmad",
            Title = title,
            Body = body,
            Subtitle = subtitle,
            ShowInForeground = true,
            ForegroundPresentationOption = (PresentationOption.Alert | PresentationOption.Sound),
            CategoryIdentifier = "default_category",
            Trigger = timeTrigger
        };

        iOSNotificationCenter.ScheduleNotification(notification);



    }

#endif

#endif

}
