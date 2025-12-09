using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using System;

public class AppOpenAdManager : MonoBehaviour
{
    AppOpenAd appOpenAd;
    AdsIDS adsIds;
    bool showAppOpen;

    public ScreenOrientation _screenOrientation;
    string _appOpenId;

    private void Awake()
    {
        adsIds = (AdsIDS)Resources.Load("Ad ids");

        Debug.Log("AppOpenAdManager() Ads Ids : " + adsIds);
    }

    private void OnEnable()
    {
        if (adsIds != null)
        {
            if (adsIds.TestAds == true)
            {
                _appOpenId = "ca-app-pub-3940256099942544/9257395921";

            }
            else
            {
                _appOpenId = adsIds._admobAppOpenID();

            }
        }

        showAppOpen = adsIds.OpenApp;
        Debug.Log("AppOpenAdManager() Show App Open : " + showAppOpen);

    }

    // Start is called before the first frame update
    void Start()
    {
        if (!showAppOpen)
        {
            Debug.Log("AppOpenAdManager() Cannot Show App Open : " + showAppOpen);

            return;
        }
        else
        {
            Debug.Log("AppOpenAdManager() Initilize app Open : " + showAppOpen);

            AppStateEventNotifier.AppStateChanged += OnAppStateChanged;

            MobileAds.Initialize((InitializationStatus initStatus) =>
            {
                Debug.Log("AppOpenAdManager() App Open Initialized --------------");

                LoadAppOpenAd();
            });
        }
    }

    private void OnDestroy()
    {
        AppStateEventNotifier.AppStateChanged -= OnAppStateChanged;
    }

    private void LoadAppOpenAd()
    {
        // Clean up the old ad before loading a new one.
        if (appOpenAd != null)
        {
            Debug.Log("AppOpenAdManager() Old App Open Instance has been cleaned : " + appOpenAd);

            appOpenAd.Destroy();
            appOpenAd = null;
        }

        Debug.Log("AppOpenAdManager() Loading the app open ad.");

        // Create our request used to load the ad.
        var adRequest = new AdRequest.Builder().Build();

        // send the request to load the ad.
        AppOpenAd.Load(_appOpenId, _screenOrientation, adRequest,
            (AppOpenAd ad, LoadAdError error) =>
            {
              // if error is not null, the load request failed.
              if (error != null || ad == null)
                {
                    Debug.LogError("app open ad failed to load an ad " +
                                   "with error : " + error);
                    return;
                }

                Debug.Log("App open ad loaded with response : "
                          + ad.GetResponseInfo());

                appOpenAd = ad;
                RegisterEventHandlers(ad);
            });
    }

    private void RegisterEventHandlers(AppOpenAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("App open ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("App open ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
            Debug.Log("App open ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("App open ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("App open ad full screen content closed.");
            LoadAppOpenAd();
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("App open ad failed to open full screen content " +
                           "with error : " + error);
            LoadAppOpenAd();

        };
    }

    private void OnAppStateChanged(AppState state)
    {
        Debug.Log("App State changed to : " + state);

        // if the app is Foregrounded and the ad is available, show it.
        if (state == AppState.Foreground)
        {
            StartCoroutine(ShowAppOpenAdWithDelay());
        }
    }

    IEnumerator ShowAppOpenAdWithDelay()
    {
        yield return new WaitForSeconds(0.2f);
        ShowAppOpenAd();
    }

    public void ShowAppOpenAd()
    {
        if (appOpenAd != null && appOpenAd.CanShowAd())
        {
            Debug.Log("Showing app open ad.");
            appOpenAd.Show();
        }
        else
        {
            Debug.LogError("App open ad is not ready yet.");
        }
    }
}
