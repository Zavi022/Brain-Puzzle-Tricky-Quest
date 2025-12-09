using GoogleMobileAds.Api;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;
using UnityEngine.Events;
using System.Collections;
using GoogleMobileAds.Common;

public class AdsManager : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener, IUnityAdsInitializationListener
{
    
    #region AdsManagerInstance
    public static AdsManager Instance;
    #endregion

    public bool isAdsRemove=false;

    #region Ads_ID
    [Header("-----ADMOB IDS-----")]
    string admobAapID;
    string admobBannerID;
    string admobBigBannerID;
    string admobAdeptiveBannerID;
    string admobIntertestialID;
    string admobStaticIntertestialID;
    string admobRewadedAdId; 
    string appOpenId;
    [Header("-----Unity IDS-----")]
    string UnityAdsID;
    string InterstialsAdsPlaceMentID;
    string RewardedPlacementId;
    string _adUnitId;
    bool UnityTestAds;
    [Header("-----Ads Enabled-----")]
    bool adsEnabled;
    [Header("-----AppOpen Enabled-----")]
    bool showAppOpen;
    public bool resmueAppOpen;
    #endregion Ads_ID

    #region variables
    private BannerView bannerView;
    private BannerView bigbannerview;
    private InterstitialAd interstitial;
    private InterstitialAd staticinterstitial;
    private RewardedAd AdmobRewardVideo;
    public GameObject AdsBreak;
    public GameObject Adsfeature;
    public GameObject Adsreward;
    public Text showAdstime;

    public float timetoreach = 0;
    private bool Isloadrewarded = true;
    private bool isShowingAd = false;
    private bool isloadfailsAd = false;
    AdsIDS adsIds;
    // COMPLETE: Add loadTime field
    private DateTime loadTime;
    private AppOpenAd ad;
    #endregion variables
    public bool HasSmartbanner;
    public bool showBannerBackground;
   public BannerBgManager bannerBGScript;
    public AdPosition smallBanneradPosition;
    [HideInInspector]
    public AdPosition bigBanneradPosition;
    public AdPosition adeptiveBanneradPosition;
    
    public enum BannerAdTypes
    {
        BANNER, ADAPTIVE, NATIVE
    }
    #region AdsInitilization

    //Remove ads will check here
    void CheckAdsRemove()
    {
        if (PlayerPrefs.GetInt("REMOVE_ADS") == 1)
            isAdsRemove = true;
        else
            isAdsRemove = false;
    }

    void Awake()
    {
        Instance = this;
        adsIds = (AdsIDS)Resources.Load("Ad ids");
       
        DontDestroyOnLoad(this);
    }
    private void OnEnable()
    {
        CheckAdsRemove();

        if (adsIds != null)
        {
            if (adsIds.TestAds == true)
            {
                admobAapID = "ca-app-pub-3940256099942544~3347511713";
                admobBannerID = "ca-app-pub-3940256099942544/6300978111";
                admobBigBannerID = "ca-app-pub-3940256099942544/6300978111";
                admobAdeptiveBannerID = "ca-app-pub-3940256099942544/6300978111";
                admobIntertestialID = "ca-app-pub-3940256099942544/1033173712";
                admobStaticIntertestialID = "ca-app-pub-3940256099942544/1033173712";
                admobRewadedAdId = "ca-app-pub-3940256099942544/5224354917";
                appOpenId = "ca-app-pub-3940256099942544/9257395921";

                UnityAdsID = adsIds._unityAppID();
                InterstialsAdsPlaceMentID = adsIds._unityInterstialPlaceMentID();
                RewardedPlacementId = adsIds._unityRewardedPlacementID();
                _adUnitId = RewardedPlacementId;
                
                UnityTestAds = true;
            }
            else
            {
                admobAapID = adsIds._admobAppID();
                admobBannerID = adsIds._admobBannerID();
                admobBigBannerID = adsIds._admobBigBannerID();
                admobAdeptiveBannerID = adsIds._admobAdeptiveBannerID();
                admobIntertestialID = adsIds._admobIntertestialID();
                admobStaticIntertestialID = adsIds._admobStaticIntertestialID();
                admobRewadedAdId = adsIds._admobRewadedAdID();
                appOpenId = adsIds._admobAppOpenID();

                UnityAdsID = adsIds._unityAppID();
                InterstialsAdsPlaceMentID = adsIds._unityInterstialPlaceMentID();
                RewardedPlacementId = adsIds._unityRewardedPlacementID();
                _adUnitId = RewardedPlacementId;

                UnityTestAds = false;


            }
            adsEnabled = adsIds;
            showAppOpen = adsIds.OpenApp;
          

        }


        if (!adsEnabled)
        {
            return;
        }
        else
        {
            ////////////////Admob/////////////////
            MobileAds.Initialize(initStatus => { });
            ////////////////Unity/////////////////
            Advertisement.Initialize(UnityAdsID, UnityTestAds, this);

        }

    }
 
    public void TopBannerUnity()
    {
        
        Advertisement.Banner.SetPosition(BannerPosition.TOP_CENTER);
 
        Advertisement.Banner.Show(AdsIDS.Instance._unityplacementIdBannerID());
    }
    private void Start()
    {
        if (!adsEnabled)
        {
            return;
        }
        else
        {
            RequestInterstitial();
            //RequestStaticInterstitial();
            //RequestRewardedVideo();

            Advertisement.Load(InterstialsAdsPlaceMentID, this);
            Advertisement.Load(RewardedPlacementId, this);

            if (!showAppOpen)
            {
                return;
            }
            else
            {
                MobileAds.Initialize((initStatus) =>
                {
                    LoadAd();

                    Debug.Log("App Open Intilized");

                    //AppStateEventNotifier.AppStateChanged += OnAppStateChanged;
                });
            }
        }
     

    }

    //public void OnAppStateChanged(AppState state)
    //{
    //    Debug.Log("App Open State_1 : " + state);

    //    if (state == AppState.Foreground)
    //    {
    //        Debug.Log("App Open State_2 : " + state);

    //        if (Application.internetReachability != NetworkReachability.NotReachable)
    //        {
    //            Debug.Log("App Open State_3 : " + state);

    //            ShowAppOpen();

    //            //if (PlayerPrefs.GetInt("ADSUNLOCK") == 0)
    //            //{
    //            //    Debug.Log("App Open State_4 : " + state);

    //            //    ShowAppOpen();
    //            //}
    //        }
    //    }
    //}


    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            if (Application.internetReachability != NetworkReachability.NotReachable)
            {
                Debug.Log("Application is in focus");
                ShowAppOpen();
                // Perform actions when the application gains focus
            }
        }
        else
        {

            Debug.Log("Application lost focus");
            // Perform actions when the application loses focus
        }
    }


    public void ShowAppOpen()
    {

        if (isAdsRemove)
            return;


        //StartCoroutine(waitForFeature());

        Debug.Log("App Open Resume App Open : " + resmueAppOpen);

        if (resmueAppOpen)
            StartCoroutine(waitForFeature());
        else
            resmueAppOpen = true;


    }

    IEnumerator waitForFeature()
    {
        if (IsAdAvailable)
        {
            AdsBreak.SetActive(true);
            yield return new WaitForSecondsRealtime(2f);

            Debug.Log("App Open Ad Available to show : " + IsAdAvailable);

            ShowAdIfAvailable();

            yield return new WaitForSecondsRealtime(0.5f);

            AdsBreak.SetActive(false);
        }
        else
        {
            LoadAd();
        }

        
    }

    #endregion AdmobInstance

    #region AdmobBanner
    
    public void RequestBanner(AdSize size, AdPosition position)
    {
        if (isAdsRemove)
            return;

        if (!adsEnabled)
            return;

        string adUnitId = "";

        if (size == AdSize.Banner)
            adUnitId = admobBannerID;
        else
            adUnitId = admobBigBannerID;

        Debug.Log("ADmob Banenr Id  =" + adUnitId);
        Debug.Log("ADmob Banenr Size  =" + size);
        Debug.Log("ADmob Banenr Position  =" + position);
        bannerView = new BannerView(adUnitId, size, position);
        AdmobEvents();
        AdRequest request = new AdRequest();
        bannerView.LoadAd(request);
    }

    private void RequestAdeptiveBanner()
    {
        string adUnitId = null;
        if (adsIds.TestAds == true)
        {
            adUnitId = "ca-app-pub-3940256099942544/6300978111";
        }
        else
        {
            adUnitId = adsIds._admobAdeptiveBannerID();
        }


        // Clean up banner ad before creating a new one.
        if (this.bannerView != null)
        {
            this.bannerView.Destroy();
        }


        AdSize adaptiveSize = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);

        this.bannerView = new BannerView(adUnitId, adaptiveSize, adeptiveBanneradPosition);

        this.bannerView.OnBannerAdLoaded += this.HandleAdLoaded;
        this.bannerView.OnBannerAdLoadFailed += this.HandleAdFailedToLoad;
        this.bannerView.OnAdFullScreenContentOpened += this.HandleAdOpening;
        this.bannerView.OnAdFullScreenContentClosed += this.HandleAdClosed;

        AdRequest adRequest = new AdRequest();

        // Load a banner ad.
        this.bannerView.LoadAd(adRequest);
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            bannerBGScript?.SetAdaptivBannerSize(this.bannerView);


        }
    }

    public void Show_Big_Banner()
    {
        string adUnitId = admobBigBannerID;
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            bannerBGScript?.SetDefaultNativeSize();
        }
        this.bigbannerview = new BannerView(adUnitId, AdSize.MediumRectangle, AdPosition.BottomLeft);
        AdRequest request = new AdRequest();
        this.bigbannerview.LoadAd(request);
    }

    public void DestorySmallBanner()
    {
        if (bannerView != null)
            bannerView.Destroy();
        bannerBGScript?.DestroyBannerBG();
        bannerBGScript?.DestroyAdaptiveBannerBG();
    }

    public void DestorybIGBanner()
    {
        if (bigbannerview != null)
            bigbannerview.Destroy();
        bannerBGScript?.DestroyNativeG();
    }

    public void TopBannerAdmob()
    {
        if (!adsEnabled) return;
        DestoryBanner();
      
            bannerView = new BannerView(admobBannerID, AdSize.Banner, smallBanneradPosition);
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            bannerBGScript?.SetDefaultBannerSize();
        }
        AdmobEvents();
            AdRequest request = new AdRequest();
            bannerView.LoadAd(request);
        
    }

    public void TopAdeptiveBanner()
    {
        if (!adsEnabled) return;
        DestoryBanner();
        RequestAdeptiveBanner();
     

    }

    public void DestoryBanner()
    {
        if (bannerView != null)
            bannerView.Destroy();
     // bannerBGScript?.DestroyBannerBG();
    }

    #endregion AdmobBanner

    #region Adaptive Banner callback handlers

    public void HandleAdLoaded()
    {
        MonoBehaviour.print("AdaptiveBanner() HandleAdLoaded event received");
        MonoBehaviour.print(String.Format("Ad Height: {0}, width: {1}",
            this.bannerView.GetHeightInPixels(),
            this.bannerView.GetWidthInPixels()));
    }

    public void HandleAdFailedToLoad(object sender)
    {
          MonoBehaviour.print("AdaptiveBanner() HandleFailedToReceiveAd event received with message: ");
    }

    public void HandleAdOpening()
    {
        MonoBehaviour.print("AdaptiveBanner() HandleAdOpening event received");
    }

    public void HandleAdClosed()
    {
        MonoBehaviour.print("AdaptiveBanner() HandleAdClosed event received");
    }

    #endregion

    #region AdMObInterstial

    private void RequestInterstitial()
    {
        if (!adsEnabled) return;

        // Clean up the old ad before loading a new one.
        if (interstitial != null)
        {
            interstitial.Destroy();
            interstitial = null;
        }

        string adUnitId = admobIntertestialID;

        Debug.Log("Admob RequestInterstitial() Loading the interstitial ad.");

        AdRequest request = new AdRequest();

        InterstitialAd.Load(adUnitId, request,
          (InterstitialAd ad, LoadAdError error) =>
          {
              // if error is not null, the load request failed.
              if (error != null || ad == null)
              {
                  Debug.LogError("Admob RequestInterstitial() interstitial ad failed to load an ad " +
                                 "with error : " + error);
                  return;
              }

              Debug.Log("Admob RequestInterstitial() Interstitial ad loaded with response : "
                        + ad.GetResponseInfo());

              interstitial = ad;

              AdmobInterstialEvents();

          });



    }

    private void RequestStaticInterstitial()
    {
        if (!adsEnabled) return;

        string adUnitStaticId = admobStaticIntertestialID;

        Debug.Log("Admob RequestStaticInterstitial() Loading the interstitial ad.");

        AdRequest request = new AdRequest();

        InterstitialAd.Load(adUnitStaticId, request,
          (InterstitialAd ad, LoadAdError error) =>
          {
              // if error is not null, the load request failed.
              if (error != null || ad == null)
              {
                  Debug.LogError("Admob RequestStaticInterstitial() interstitial ad failed to load an ad " +
                                 "with error : " + error);
                  return;
              }

              Debug.Log("Admob RequestStaticInterstitial() Interstitial ad loaded with response : "
                        + ad.GetResponseInfo());

              staticinterstitial = ad;
          });

        AdmobStaticInterstialEvents();

    }

    void MainAdmob()
    {
        if (!adsEnabled) return;

        Debug.Log("Admob MainAdmob() Show Interstitial");

        if (interstitial!=null && interstitial.CanShowAd())
        {
            Debug.Log("Admob MainAdmob() Interstitial is Available and Showing");

            interstitial.Show();
        }
    }

    void StaticMainAdmob()
    {
        if (!adsEnabled) return;

        Debug.Log("Admob MainAdmob() Show Static Interstitial");

        if (staticinterstitial != null && staticinterstitial.CanShowAd())
        {
            Debug.Log("Admob MainAdmob() Static Interstitial is Available and Showing");

            staticinterstitial.Show();
        }
    }

    #endregion AdMObInterstial

    #region AdmobRewardAds

    private void RequestRewardedVideo()
    {
        if (!adsEnabled) return;

        AdRequest request = new AdRequest();

        Debug.Log("Admob RequestRewardedVideo() Loading the Reward ad.");

        RewardedAd.Load(admobRewadedAdId, request,
          (RewardedAd ad, LoadAdError error) =>
          {
              // if error is not null, the load request failed.
              if (error != null || ad == null)
              {
                  Debug.LogError("Admob RequestRewardedVideo() Rewarded ad failed to load an ad " +
                                 "with error : " + error);
                  return;
              }

              Debug.Log("Admob RequestRewardedVideo() Rewarded ad loaded with response : "
                        + ad.GetResponseInfo());

              AdmobRewardVideo = ad;

              RewardedVideoEvents();

          });


    }

    //It will Only show admob Reward ad
    public void ShowAdmobRewardAds()
    {
        if (!adsEnabled) return;

        Debug.Log("Admob ShowAdmobRewardAds() Show Reward Ad Call");

        RequestRewardedVideo();

        if (AdmobRewardVideo != null && AdmobRewardVideo.CanShowAd())
        {
            AdmobRewardVideo.Show((Reward reward) =>
            {
                // TODO: Reward the user.
                Debug.Log(String.Format(reward.Type, reward.Amount));

                Debug.Log("Admob ShowAdmobRewardAds() Reward ad Showed to User");

            });
        }
    }

    //It will check and show if unity/admob reward is available to show
    public void ShowRewardedAds()
    {
        // AdmobRewardVideo.Show();
        if (!adsEnabled) return;

        Debug.Log("reward Admob Ad clicked");
        Debug.Log("Is Reward Ad Loaded : "+Isloadrewarded);

        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            RequestRewardedVideo();

            if (Isloadrewarded)
            {
                StartCoroutine(WaitforRewardAds());
            }
        }
    }
    IEnumerator WaitforRewardAds()
    {
        AdsBreak.SetActive(true);

        yield return new WaitForSecondsRealtime(2f);

        Debug.Log("ShowRewardedAds() Show Reward Ad Call");

        if (AdmobRewardVideo != null && AdmobRewardVideo.CanShowAd())
        {
            AdmobRewardVideo.Show((Reward reward) =>
            {
                // TODO: Reward the user.

                Debug.Log("Admob ShowRewardedAds() Reward ad Showed to User");

                Debug.Log(String.Format(reward.Type, reward.Amount));

                //RequestRewardedVideo();

            });
        }
        else
        {
            Debug.Log("Unity ShowRewardedAds() Show Reward Ad Call");
            resmueAppOpen = false;
            ShowUnityRewardedVideo();
        }

        yield return new WaitForSecondsRealtime(0.5f);

        AdsBreak.SetActive(false);

    }
    #endregion AdmobRewardAds

    #region UnityRewardAds
    public UnityAction OnUserEarnedReward;
    public void ShowUnityRewardedVideo()
    {
        if (!adsEnabled) return;

        Advertisement.Show(RewardedPlacementId,this);
    }
    #endregion UnityRewardAds

    #region AllAdsEvents

    #region AdMobBannerEvents

    private void AdmobEvents()
    {
        bannerView.OnBannerAdLoaded += HandleOnAdLoadedBanner;
        bannerView.OnBannerAdLoadFailed += HandleOnAdFailedToLoadBanner;
        bannerView.OnAdFullScreenContentOpened += HandleOnAdOpenedBanner;
        bannerView.OnAdFullScreenContentClosed += HandleOnAdClosedBanner;
    }
    public void HandleOnAdLoadedBanner()
    {
        MonoBehaviour.print("AdmobBanner() HandleAdLoaded event received");
    }
    public void HandleOnAdFailedToLoadBanner(object sender)
    {
        MonoBehaviour.print("AdmobBanner() HandleFailedToReceiveAd event received with message: ");
    }
    public void HandleOnAdOpenedBanner()
    {
        MonoBehaviour.print("AdmobBanner() HandleAdOpened event received");
    }
    public void HandleOnAdClosedBanner()
    {
        MonoBehaviour.print("AdmobBanner() HandleAdClosed event received");
    }
    public void HandleOnAdLeavingApplicationBanner()
    {
        MonoBehaviour.print("AdmobBanner() HandleAdLeavingApplication event received");
    }

    #endregion AdMobBannerEvents

    #region AdmonInterstialEvents

    private void AdmobInterstialEvents()
    {

        //Debug.Log(interstitial);
        interstitial.OnAdClicked += HandleOnAdLoaded;
        interstitial.OnAdFullScreenContentFailed += HandleOnAdFailedToLoad;
        interstitial.OnAdFullScreenContentOpened += HandleOnAdOpened;
        interstitial.OnAdFullScreenContentClosed += HandleOnAdClosed;

    }
    private void AdmobStaticInterstialEvents()
    {
        staticinterstitial.OnAdClicked += HandleOnAdLoadedStatic;
        staticinterstitial.OnAdFullScreenContentFailed += HandleOnAdFailedToLoadStatic;
        staticinterstitial.OnAdFullScreenContentOpened += HandleOnAdOpenedStatic;
        staticinterstitial.OnAdFullScreenContentClosed += HandleOnAdClosedStatic;

    }
    public void HandleOnAdLoaded()
    {
        MonoBehaviour.print("AdmobInterstial() HandleAdLoaded event received");

    }
    public void HandleOnAdFailedToLoad(object sender)
    {
        MonoBehaviour.print("AdmobInterstial() HandleFailedToReceiveAd event received with message: ");
    }
    public void HandleOnAdOpened()
    {
        MonoBehaviour.print("AdmobInterstial() HandleAdOpened event received");
        resmueAppOpen = false;

    }

    public void HandleOnAdClosed()
    {
        if (adsIds.AdmobSingelInterstialRequest)
        {
            return;
        }
        else
        {
            RequestInterstitial();

        }
        MonoBehaviour.print("AdmobInterstial() HandleAdClosed event received");
    }

    public void HandleOnAdLeavingApplication(object sender, EventArgs args)
    {
        MonoBehaviour.print("AdmobInterstial() HandleAdLeavingApplication event received");
    }
    public void HandleOnAdLoadedStatic()
    {
        MonoBehaviour.print("AdmobStaticInterstial() HandleAdLoaded event received");
    }
    public void HandleOnAdFailedToLoadStatic(object sender)
    {
        MonoBehaviour.print("AdmobStaticInterstial() HandleFailedToReceiveAd event received with message: ");
    }
    public void HandleOnAdOpenedStatic()
    {
        MonoBehaviour.print("AdmobStaticInterstial() HandleAdOpened event received");
        resmueAppOpen = false;

    }

    public void HandleOnAdClosedStatic()
    {
        if (adsIds.AdmobSingelInterstialRequest)
        {
            return;
        }
        else
        {
            RequestStaticInterstitial();

        }
        MonoBehaviour.print("AdmobStaticInterstial() HandleAdClosed event received");
    }

    public void HandleOnAdLeavingApplicationStatic(object sender, EventArgs args)
    {
        MonoBehaviour.print("AdmobStaticInterstial() HandleAdLeavingApplication event received");
    }

    #endregion AdmonInterstialEvents

    #region AdmobRewardEvents

    private void RewardedVideoEvents()
    {
        AdmobRewardVideo.OnAdClicked += HandleRewardedAdLoaded;
        AdmobRewardVideo.OnAdFullScreenContentOpened += HandleRewardedAdOpening;
        AdmobRewardVideo.OnAdFullScreenContentFailed += HandleRewardedAdFailedToShow;
        AdmobRewardVideo.OnAdImpressionRecorded += HandleUserEarnedReward;
        AdmobRewardVideo.OnAdFullScreenContentClosed += HandleRewardedAdClosed;

    }
    public void HandleRewardedAdLoaded()
    {
        MonoBehaviour.print("AdmobRewardAd() HandleRewardedAdLoaded event received");
        Isloadrewarded = true;
    }

    public void HandleRewardedAdFailedToLoad()
    {
        Isloadrewarded = false;
        RequestRewardedVideo();
        MonoBehaviour.print("AdmobRewardAd() HandleRewardedAdFailedToLoad event received with message: ");
    }

    public void HandleRewardedAdOpening()
    {
        MonoBehaviour.print("AdmobRewardAd() HandleRewardedAdOpening event received");
    }

    public void HandleRewardedAdFailedToShow(object sender)
    {
        Isloadrewarded = false;
        RequestRewardedVideo();
        MonoBehaviour.print("AdmobRewardAd() HandleRewardedAdFailedToShow event received with message: ");
    }

    public void HandleRewardedAdClosed()
    {
        RequestRewardedVideo();
        MonoBehaviour.print("AdmobRewardAd() HandleRewardedAdClosed event received");
    }

    public void HandleUserEarnedReward()
    {

        //OnUserEarnedReward?.Invoke();
        MonoBehaviour.print("AdmobRewardAd() HandleUserEarnedReward event received");

        resmueAppOpen = false;

        RewardManager.Instance.OnEndRewardedAds();

    }

    #endregion AdmonRewardEvents

    #region UnityInterstialAdsEvent
    public void OnUnityAdsAdLoaded(string adUnitId)
    {

        MonoBehaviour.print("UnityInterstial() OnUnityAdsAdLoaded event received");

        // Optionally execute code if the Ad Unit successfully loads content.
    }

    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Error loading Ad Unit: {adUnitId} - {error.ToString()} - {message}");

        MonoBehaviour.print("UnityInterstial() OnUnityAdsFailedToLoad event received");

        // Optionally execite code if the Ad Unit fails to load, such as attempting to try again.
    }

    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        Debug.Log($"Error showing Ad Unit {adUnitId}: {error.ToString()} - {message}");

        MonoBehaviour.print("UnityInterstial() OnUnityAdsShowFailure event received");

        // Optionally execite code if the Ad Unit fails to show, such as loading another ad.
    }

    public void OnUnityAdsShowStart(string adUnitId) { }
    public void OnUnityAdsShowClick(string adUnitId) { }
    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
       
        if (adUnitId.Equals(_adUnitId) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
        {
            MonoBehaviour.print("UnityInterstial() OnUnityAdsShowComplete event received");

            resmueAppOpen = false;

            // Grant a reward.
            RewardManager.Instance.OnEndRewardedAds();
            // Load another ad:
            Advertisement.Load(_adUnitId, this);
        }
    }

    public void OnInitializationComplete()
    {
        MonoBehaviour.print("UnityInterstial() OnInitializationComplete event received");
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");

        MonoBehaviour.print("UnityInterstial() OnInitializationFailed event received");

    }
    #endregion

    #region UnityRewardedAdsEvent
    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        // Define conditional logic for each ad completion status:
        if (showResult == ShowResult.Finished)
        {
            MonoBehaviour.print("UnityRewarded() OnUnityAdsDidFinish event received");

            resmueAppOpen = false;

            OnUserEarnedReward?.Invoke();
            // Reward the user for watching the ad to completion.
        }
        else if (showResult == ShowResult.Skipped)
        {
            // Do not reward the user for skipping the ad.
        }
        else if (showResult == ShowResult.Failed)
        {

        }
    }

    public void OnUnityAdsReady(string placementId)
    {
        // If the ready Placement is rewarded, show the ad:
        if (placementId == RewardedPlacementId)
        {
            MonoBehaviour.print("UnityRewarded() OnUnityAdsReady event received");

            Advertisement.Show(RewardedPlacementId);
        }
    }

    public void OnUnityAdsDidError(string message)
    {

        MonoBehaviour.print("UnityRewarded() OnUnityAdsDidError event received");

        Debug.Log(message);
        // Log the error.
    }

    public void OnUnityAdsDidStart(string placementId)
    {
        // Optional actions to take when the end-users triggers an ad.
        MonoBehaviour.print("UnityRewarded() OnUnityAdsDidStart event received");

        Debug.Log("Optional actions to take when the end - users triggers an ad");
    }

    #endregion

    #endregion AllAdsEvents

    #region ShowAds


    #region UnityAds
    public void ShowIntertitialUnity()
    {
        if (!adsEnabled) return;
        if (Advertisement.isInitialized)
        {
            StartCoroutine(WaitForAd(1));
        }
    }
    #endregion

    public void DestroyBannerUnity()
    {
        if (Advertisement.Banner.isLoaded)
        {
            Advertisement.Banner.Hide();
        }
    }
    public void ShowInterstialAds()
    {
        if (isAdsRemove)
            return;

        if (!adsEnabled)
            return;

        if (interstitial != null && interstitial.CanShowAd())
        {
          
            StartCoroutine(WaitForAd(0));
        }
        else if (Advertisement.isInitialized)
        {
          
            StartCoroutine(WaitForAd(1));
        }
    }
    public void ShowInterstialStaticAds()
    {
        if (!adsEnabled) return;
        if (staticinterstitial != null && staticinterstitial.CanShowAd())
        {

            StaticMainAdmob();
        }
    
    }
    IEnumerator WaitForAd(int i)
    {
        AdsBreak.SetActive(true);
        yield return new WaitForSecondsRealtime(1f);
        switch (i)
        {
            case 0:
                MainAdmob();

                break;
            case 1:

                resmueAppOpen = false;

                AdMobScript.Instance.ShowIntertitialUnity();

                break;
        }
        yield return new WaitForSecondsRealtime(0.5f);
        AdsBreak.SetActive(false);
    }
    #endregion ShowAds

    #region Openapp


    //public static AppOpenAdManager Instance
    //{
    //    get
    //    {
    //        if (Instance == null)
    //        {
    //            Instance = new AppOpenAdManager();
    //        }

    //        return Instance;
    //    }
    //}

    private bool IsAdAvailable
    {
        get
        {
            // COMPLETE: Consider ad expiration
            return ad != null /* && (System.DateTime.UtcNow - loadTime).TotalHours < 4*/;
        }
    }

    public void LoadAd()
    {

        Debug.Log("App open ad loading ...");

        var request = new AdRequest();

        // Load an app open ad for portrait orientation
        AppOpenAd.Load(appOpenId, request,
            (AppOpenAd appOpenAd, LoadAdError error) =>
        {
            if (error != null)
            {
                // Handle the error.
                Debug.LogFormat("Failed to load the ad. (reason: {0})", error.GetMessage());
                return;
            }

            // App open ad is loaded
            ad = appOpenAd;
            Debug.Log("App open ad loaded");

            // COMPLETE: Keep track of time when the ad is loaded.
            //loadTime = DateTime.UtcNow;
        });
    }


    public void ShowAdIfAvailable()
    {
        if (!IsAdAvailable /* || isShowingAd*/)
        {
            return;
        }

        ad.OnAdFullScreenContentClosed += HandleAdDidDismissFullScreenContent;
        ad.OnAdFullScreenContentFailed += HandleAdFailedToPresentFullScreenContent;
        ad.OnAdFullScreenContentOpened += HandleAdDidPresentFullScreenContent;
        ad.OnAdImpressionRecorded += HandleAdDidRecordImpression;
        ad.OnAdPaid += HandlePaidEvent;

        ad.Show();
    }

    private void HandleAdDidDismissFullScreenContent()
    {
        Debug.Log("App Open Closed ad");
        // Set the ad to null to indicate that AppOpenAdManager no longer has another ad to show.
        ad = null;
        //isShowingAd = false;

        LoadAd();

        //LoadAd();
    }

    private void HandleAdFailedToPresentFullScreenContent(object sender)
    {
        Debug.LogFormat("App Open Failed to present the ad ");
        // Set the ad to null to indicate that AppOpenAdManager no longer has another ad to show.
        ad = null;

        LoadAd();

        //LoadAd();
    }

    private void HandleAdDidPresentFullScreenContent()
    {
        Debug.Log("App Open Displayed ad");
        //isShowingAd = true;
    }

    private void HandleAdDidRecordImpression()
    {
        Debug.Log("App Open Recorded ad impression");
    }

    private void HandlePaidEvent(object sender)
    {
        Debug.LogFormat("App Open Received paid event ");
    }
    #endregion openapp

    //This will call only on RemoveAds Purchase Button
    public void RemoveAds()
    {
        PlayerPrefs.SetInt("REMOVE_ADS", 1);

        AnalyticsManager.Instance.Event_Transition(AnalyticsManager.Event_Triggers.RemoveAds_Purchase, AnalyticsManager.Event_State.InApp_Events);

        isAdsRemove = true;

        DestoryBanner();
        DestorybIGBanner();
        DestorySmallBanner();

    }


}