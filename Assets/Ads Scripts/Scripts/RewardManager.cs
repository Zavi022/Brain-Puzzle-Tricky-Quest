using UnityEngine;
using UnityEngine.Events;

public class RewardManager : MonoBehaviour
{
    public static RewardManager Instance;

    private static UnityAction OnProcessSuccessful = null;

    //public int _ID;
    private void Awake()
    {
        Instance = this;
    }

    public void ShowRewardAd(UnityAction OnSuccess)
    {
        OnProcessSuccessful = null;

        if (OnSuccess != null)
        {
           // AnalyticsManager.Instance.Event_Transition(AnalyticsManager.Event_Triggers.Reward_AD_Clicked, AnalyticsManager.Event_State.Menu_Events);

            OnProcessSuccessful = OnSuccess;
            AdsManager.Instance.ShowRewardedAds();


        }
        else
        {
            Debug.Log("No Action is subscribe");
        }
    }

    public void OnEndRewardedAds()
    {
        if (OnProcessSuccessful != null)
        {
           // AnalyticsManager.Instance.Event_Transition(AnalyticsManager.Event_Triggers.Reward_AD_Complete, AnalyticsManager.Event_State.Menu_Events);

            OnProcessSuccessful?.Invoke();

            Debug.Log("Function To Be Called After Purchase : " + OnProcessSuccessful.Method.Name);

            Debug.Log("Successfull Give reward");
        }

        OnProcessSuccessful = null;

        //switch (_ID)
        //{
        //    case 0:
        //        Debug.Log("Reward Claimed");
        //        break;
        //}
    }
    
}
