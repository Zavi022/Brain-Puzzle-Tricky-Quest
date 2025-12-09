
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using GoogleMobileAds.Api;
public class RewaredAdsEvents : MonoBehaviour
{
    public UnityEvent onRewardedAdsDisplayed;
    protected void OnRewardedAdsDisplayed()
    {
        onRewardedAdsDisplayed?.Invoke();
    }
    private void OnEnable()
    {
        Invoke("UserEnared", 1f);

       
    }

    private void OnDisable()
    {
        AdsManager.Instance.OnUserEarnedReward -= OnRewardedAdsDisplayed;
    }

    //public void ShowUnityRewaredAd()
    //{
    //  AdsManager.Instance.ShowUnityRewardedVideo();
    //}

    //public void ShowAdmobRewaredAd()
    //{
    //  AdsManager.Instance.ShowAdmobRewardAds();
    //}
    public void ShowRewaredAd()
    {
      AdsManager.Instance.ShowRewardedAds();
    }


    void UserEnared()
    { 
        //if (TryGetComponent<Button>(out Button button))
        //{
        //    button.interactable = AdsManager.Instance.RewardedAdsAvailable();
        //}
        AdsManager.Instance.OnUserEarnedReward += OnRewardedAdsDisplayed;

    }
    //public void AddCoins(int amount)
    //{
    //    GMPlayerPrefs.SaveInt(GMPlayerPrefs.LoadInt("ToTAl_-_CoINs") + amount, "ToTAl_-_CoINs");
    //    CoinsDisplay coinsDisplay = FindObjectOfType<CoinsDisplay>();
    //    if (coinsDisplay != null)
    //    {
    //        coinsDisplay.UpdateUI();
    //    }
    //}

}
