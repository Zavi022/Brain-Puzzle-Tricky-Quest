using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NativeNotificationController : MonoBehaviour
{
    public string[] notificationText;

    [SerializeField]
    private NotificationController notificationController;



    private void Start()
    {

#if AMAZON
        return;
#endif

#if Notification

#if UNITY_ANDROID
        notificationController.RequestAuthorization();

        notificationController.RegisterNotificationChannel();

        int ran = Random.Range(0, notificationText.Length);
        notificationController.SendNotification(Application.productName,
            notificationText[ran], 1300);

#elif UNITY_IOS
        StartCoroutine(notificationController.RequestAuthorization());

        int ran = Random.Range(0, notificationText.Length);

        notificationController.SendNotification(Application.productName,
            notificationText[ran], " ", 20);
        
#endif

#endif

    }

}
