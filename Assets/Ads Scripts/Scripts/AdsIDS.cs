#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[CreateAssetMenu(fileName = "Ad ids", menuName = "AdsManager/Create Ad ids Data", order = 1)]
public class AdsIDS : ScriptableObject
{
    #region AdsIdsInstance
    public static AdsIDS instance;
    const string databaseName = "Ad ids";

    public static AdsIDS Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load(databaseName) as AdsIDS;
            }
            return instance;
        }
    }

    #region Editor
#if UNITY_EDITOR
    [MenuItem("Window/AdsManager/Show Ad Sequence #s")]
    public static void Edit()
    {
        Selection.activeObject = Instance;
    }
#endif
    #endregion

    #endregion


    #region UnityFunctions

  
    private void OnEnable()
    {
        instance = this;
    }
    #endregion

    #region AdsEnables

    [Header("Enable Admob and Unity ads in Project")]
    public bool adsEnabled = true;
    [Header("Enable Test Ads for Admob and Unity")]
    [SerializeField]
    public bool TestAds = true;
    [Header("Enable Open APP AD")]
    [SerializeField]
    public bool OpenApp = true;

    #endregion

    #region Ads_ID

    //[Header("-----ADMOB IDS-----")]
    //public string admobAapID;
    //public string admobBannerID;
    //public string admobBigBannerID;
    //public string admobAdeptiveBannerID;
    //public string admobIntertestialID;
    //public string admobStaticIntertestialID;
    //public string admobRewadedAdId;
    //public string appOpenID;
    //[Header("-----Unity IDS-----")]
    //public string UnityAdsID;
    //public string InterstialPlaceMentID;
    //public string RewardedPlacementId;
    //public string placementIdBanner = "Banner_Android";
    string _adUnitId = null;
   
  
    [Header("if Check just one Interstial Request will be send")]
    [Header("-----AdMob Single Request-----")]
    public bool AdmobSingelInterstialRequest;


    #endregion Ads_ID

    #region Ad Id Return Methods

    public string _admobAppID()
    {
#if UNITY_ANDROID

        return "";

#elif UNITY_IOS

        return "ca-app-pub-8739685313196856~4586918787";

#endif
    }

    public string _admobBannerID()
    {
#if UNITY_ANDROID

        return "";

#elif UNITY_IOS

        return "ca-app-pub-8739685313196856/3205554351";

#endif
    }

    public string _admobBigBannerID()
    {
#if UNITY_ANDROID

        return "";

#elif UNITY_IOS

        return "ca-app-pub-8739685313196856/3205554351";

#endif
    }

    public string _admobAdeptiveBannerID()
    {
#if UNITY_ANDROID

        return "";

#elif UNITY_IOS

        return "ca-app-pub-8739685313196856/3205554351";

#endif
    }

    public string _admobIntertestialID()
    {
#if UNITY_ANDROID

        return "";

#elif UNITY_IOS

        return "ca-app-pub-8739685313196856/2595465673";

#endif
    }

    public string _admobStaticIntertestialID()
    {
#if UNITY_ANDROID

        return "";

#elif UNITY_IOS

        return "ca-app-pub-8739685313196856/2595465673";

#endif
    }

    public string _admobRewadedAdID()
    {
#if UNITY_ANDROID

        return "";

#elif UNITY_IOS

        return "ca-app-pub-8739685313196856/7688010253";

#endif
    }

    public string _admobAppOpenID()
    {
#if UNITY_ANDROID

        return "";

#elif UNITY_IOS

        return "ca-app-pub-8739685313196856/5061846916";

#endif
    }

    public string _unityAppID()
    {
#if UNITY_ANDROID

        return "";

#elif UNITY_IOS

        return "5972669";

#endif
    }

    public string _unityInterstialPlaceMentID()
    {
#if UNITY_ANDROID

        return "Interstitial_Android";

#elif UNITY_IOS

        return "Interstitial_iOS";

#endif
    }

    public string _unityRewardedPlacementID()
    {
#if UNITY_ANDROID

        return "Rewarded_Android";

#elif UNITY_IOS

        return "Rewarded_iOS";

#endif
    }

    public string _unityplacementIdBannerID()
    {
#if UNITY_ANDROID

        return "Banner_Android";

#elif UNITY_IOS

        return "Banner_iOS";

#endif
    }



    #endregion

}
