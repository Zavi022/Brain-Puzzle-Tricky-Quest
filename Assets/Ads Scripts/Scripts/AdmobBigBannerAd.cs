using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;

public class AdmobBigBannerAd : MonoBehaviour
{
    [Header("Banner Ad")]
    public AdPosition adPosition;
    [Header("Ad Size")]
    public adsize bannerstyle;
    public AdSize size;
    public static AdmobBigBannerAd instance;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        instance = this;
    }
    void setBannerSize()
    {
        switch (bannerstyle)
        {
            case adsize.banner:
                size = AdSize.Banner;
                break;
            case adsize.smartbanner:
                size = AdSize.SmartBanner;
                break;
            case adsize.bigbanner:
                size = AdSize.MediumRectangle;
                break;
        }
    }

    private void OnEnable()
    {
        if (PlayerPrefs.GetInt("ADSUNLOCK", 0) == 0)
        {
            setBannerSize();
            AdsManager.Instance.bigBanneradPosition = adPosition;
            //AdsManager.Instance.bannerBGScript?.SetDefaultNativeSize();
            //AdsManager.Instance.Show_Big_Banner();
            Debug.Log("AdmobBigBannerAd Script " + size);
            AdsManager.Instance.RequestBanner(size, adPosition);
        }
    }
    private void OnDisable()
    {
        AdsManager.Instance.DestorybIGBanner();
        AdsManager.Instance.bannerBGScript?.DestroyNativeG();
    }
    public void BigBanner()
    {
        if (PlayerPrefs.GetInt("ADSUNLOCK", 0) == 0)
        {
            setBannerSize();
            AdsManager.Instance.bigBanneradPosition = adPosition;
            AdsManager.Instance.bannerBGScript?.SetDefaultNativeSize();
            Debug.Log("AdmobBigBannerAd Script " + size);
            AdsManager.Instance.RequestBanner(size, adPosition);
        }
        
    }
}

public enum adsize
{
    banner, smartbanner, bigbanner,
};

