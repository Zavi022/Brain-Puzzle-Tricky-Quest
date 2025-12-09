using UnityEngine;

#if USE_FIREBASE
using Firebase.Analytics;
#endif


public class AnalyticsManager : MonoBehaviour
{
    public static AnalyticsManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public enum Event_Triggers
    {
        RemoveAds_Clicked,

        RemoveAds_Purchase,

        SkipLevelReward_Watched,

        Home_Clicked,

        LevelSelection_Clicked,

        LevelFailed,

        LevelComplete,

        LevelRestart,

        Reward_AD_Clicked,

        Reward_AD_Complete,
    }


    public enum Event_State
    {
        Menu_Events,
        Gameplay_Events,
        InApp_Events
    }

    public void Event_Transition(Event_Triggers _event_Triggers,Event_State _event_State)
    {
        if(PlayerPrefs.GetInt("firebasse_" + _event_Triggers, 0) == 0)
        {
            PlayerPrefs.GetInt("firebasse_" + _event_Triggers, 1);

#if USE_FIREBASE
            FirebaseAnalytics.LogEvent("Event_Transition",
                new Parameter(_event_State.ToString(), _event_Triggers.ToString()));
#endif

        }
    }

}
