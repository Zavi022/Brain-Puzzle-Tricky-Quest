using System;
using System.Collections;
using UnityEngine.Advertisements;
using UnityEngine;
using UnityEngine.Events;

public class AdMobScript : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
    string GAME_ID = null; //replace with your gameID from dashboard. note: will be different for each platform.

#if UNITY_ANDROID
    string BANNER_PLACEMENT = "Banner_Android";
     string VIDEO_PLACEMENT = "Interstitial_Android";
     string REWARDED_VIDEO_PLACEMENT = "Rewarded_Android";

#elif UNITY_IOS
    string BANNER_PLACEMENT = "Banner_iOS";
     string VIDEO_PLACEMENT = "Interstitial_iOS";
     string REWARDED_VIDEO_PLACEMENT = "Rewarded_iOS";
#endif


    string _adUnitId = null;
    //[SerializeField] private BannerPosition bannerPosition = BannerPosition.BOTTOM_CENTER;

    public bool testMode;
    private bool showBanner = false;

    //utility wrappers for debuglog
    public delegate void DebugEvent(string msg);
    public static event DebugEvent OnDebugLog;
    public static AdMobScript Instance;
    public void Initialize()
    {
        if (Advertisement.isSupported)
        {
            DebugLog(Application.platform + " supported by Advertisement");
        }
        Advertisement.Initialize(GAME_ID, testMode, this);
        Advertisement.Load(_adUnitId, this);

    }
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        LoadNonRewardedAd();
        _adUnitId = REWARDED_VIDEO_PLACEMENT;
        if (AdsIDS.instance)
        {
            Debug.Log("AdsIDS.instance Has Been Found");
            GAME_ID = AdsIDS.instance._unityAppID();
            VIDEO_PLACEMENT = AdsIDS.instance._unityInterstialPlaceMentID();
            REWARDED_VIDEO_PLACEMENT = AdsIDS.instance._unityRewardedPlacementID();
            BANNER_PLACEMENT = AdsIDS.instance._unityplacementIdBannerID();
        }
        else
        {
            Debug.Log("AdsIDS.instance cannot be found");
            AdsIDS adsIds = (AdsIDS)Resources.Load("Ad ids");
            if (adsIds)
            {
                GAME_ID = adsIds._unityAppID();
                VIDEO_PLACEMENT = adsIds._unityInterstialPlaceMentID();
                REWARDED_VIDEO_PLACEMENT = adsIds._unityRewardedPlacementID();
                BANNER_PLACEMENT = adsIds._unityplacementIdBannerID();
                Debug.Log("AdsIDS.instance Has Been Re Assigned and Managed Successfully");
            }
        }



    }
    private void Start()
    {
        Initialize();
        Instance = this;
       
        
      
    }
    public void ToggleBanner() 
    {
        showBanner = !showBanner;

        if (showBanner)
        {
           // Advertisement.Banner.SetPosition(bannerPosition);
            Advertisement.Banner.Show(BANNER_PLACEMENT);
        }
        else
        {
            Advertisement.Banner.Hide(false);
        }
    }

    public void LoadRewardedAd()
    {
        Advertisement.Load(REWARDED_VIDEO_PLACEMENT, this);
    }

    public void ShowRewardedAd()
    {
        Advertisement.Show(REWARDED_VIDEO_PLACEMENT, this);
    }

    public void LoadNonRewardedAd()
    {
        Advertisement.Load(VIDEO_PLACEMENT, this);
    
    }

    public void ShowIntertitialUnity()
    {
        
        Advertisement.Show(VIDEO_PLACEMENT, this);

        LoadNonRewardedAd();
    }


#region Interface Implementations
    public void OnInitializationComplete()
    {
        DebugLog("Init Success");
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        DebugLog($"Init Failed: [{error}]: {message}");
    }

    public void OnUnityAdsAdLoaded(string placementId)
    {
        DebugLog($"Load Success: {placementId}");
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        DebugLog($"Load Failed: [{error}:{placementId}] {message}");
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        DebugLog($"OnUnityAdsShowFailure: [{error}]: {message}");
    }

    public void OnUnityAdsShowStart(string placementId)
    {
        DebugLog($"OnUnityAdsShowStart: {placementId}");
    }

    public void OnUnityAdsShowClick(string placementId)
    {
        DebugLog($"OnUnityAdsShowClick: {placementId}");
    }

    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        if (adUnitId.Equals(_adUnitId) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
        {
            Debug.Log("Unity Ads Rewarded Ad Completed");
            //AdsManager.Instance.resmueAppOpen = false;

            // Grant a reward.
            RewardManager.Instance.OnEndRewardedAds();
            // Load another ad:
            Advertisement.Load(_adUnitId, this);
        }

    }
#endregion

    public void OnGameIDFieldChanged(string newInput)
    {
        GAME_ID = newInput;
    }

 

    //wrapper around debug.log to allow broadcasting log strings to the UI
    void DebugLog(string msg)
    {
        OnDebugLog?.Invoke(msg);
        Debug.Log(msg);
    }
#region UnityRewardAds
    public UnityAction OnUserEarnedReward;
    public void ShowUnityRewardedVideo()
    {
        Advertisement.Show(REWARDED_VIDEO_PLACEMENT);
    }



#endregion UnityRewardAds
}
