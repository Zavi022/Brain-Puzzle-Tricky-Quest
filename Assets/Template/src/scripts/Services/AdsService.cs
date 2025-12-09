using System;
using UnityEngine;
using UnityEngine.Advertisements;

public sealed class AdsService : IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
	private static AdsService _instance;
	public static AdsService Instance => _instance ?? (_instance = new AdsService());

	private AdsService() {}

	[Serializable]
	private class AdsConfigData
	{
		public string androidGameId;
		public string iosGameId;
		public string rewardedPlacementId = "rewardedVideo";
		public string interstitialPlacementId = "video";
		public string bannerPlacementId = "banner";
		public bool testMode = true;
	}

	private AdsConfigData _config;
	private string _gameId;
	private bool _isInitialized;
	private Action _onRewardCompleted;
	private Action _onRewardSkipped;
	private Action<string> _onRewardFailed;

	public bool IsInitialized => _isInitialized && Advertisement.isInitialized;

	public void InitializeFromResources()
	{
		if (IsInitialized) return;

		var ta = Resources.Load<TextAsset>("Template/src/Resources/AdsConfig");
		if (ta == null)
		{
			Debug.LogWarning("AdsService: Missing Resources/AdsConfig.json. Using defaults/test mode.");
			_config = new AdsConfigData();
		}
		else
		{
			try
			{
				_config = JsonUtility.FromJson<AdsConfigData>(ta.text);
			}
			catch (Exception e)
			{
				Debug.LogWarning("AdsService: Failed parsing AdsConfig.json: " + e.Message + ". Using defaults/test mode.");
				_config = new AdsConfigData();
			}
		}

#if UNITY_ANDROID
		_gameId = _config.androidGameId;
#else
		_gameId = _config.iosGameId;
#endif

#if UNITY_EDITOR
		if (string.IsNullOrEmpty(_gameId) && _config.testMode)
		{
			// Use Unity Ads sample GAME_ID to enable Editor test ads UI.
			_gameId = "3003911";
			Debug.LogWarning("AdsService: Using sample GAME_ID for Editor test ads. Please set your own Game ID in AdsConfig.json for real testing.");
		}
#endif
		if (string.IsNullOrEmpty(_gameId))
		{
			Debug.LogWarning("AdsService: Game ID is empty. Please fill AdsConfig.json.");
		}
		if (!Advertisement.isInitialized)
		{
			Advertisement.Initialize(_gameId, _config.testMode, this);
		}
		else
		{
			_isInitialized = true;
		}
	}

	public void ShowRewardedAd(Action onCompleted, Action onSkipped = null, Action<string> onFailed = null)
	{
		InitializeFromResources();
		_onRewardCompleted = onCompleted;
		_onRewardSkipped = onSkipped;
		_onRewardFailed = onFailed;

		if (!IsInitialized)
		{
			_onRewardFailed?.Invoke("NotInitialized");
			return;
		}
		Advertisement.Load(_config.rewardedPlacementId, this);
	}

	// IUnityAdsInitializationListener
	public void OnInitializationComplete()
	{
		_isInitialized = true;
	}

	public void OnInitializationFailed(UnityAdsInitializationError error, string message)
	{
		_isInitialized = false;
		Debug.LogWarning($"Ads init failed: {error} {message}");
	}

	// IUnityAdsLoadListener
	public void OnUnityAdsAdLoaded(string placementId)
	{
		if (placementId == _config.rewardedPlacementId)
		{
			Advertisement.Show(_config.rewardedPlacementId, this);
		}
	}

	public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
	{
		_onRewardFailed?.Invoke($"LoadFailed:{error}:{message}");
	}

	// IUnityAdsShowListener
	public void OnUnityAdsShowStart(string placementId) {}
	public void OnUnityAdsShowClick(string placementId) {}

	public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
	{
		switch (showCompletionState)
		{
			case UnityAdsShowCompletionState.COMPLETED:
				_onRewardCompleted?.Invoke();
				break;
			case UnityAdsShowCompletionState.SKIPPED:
				_onRewardSkipped?.Invoke();
				break;
			default:
				_onRewardFailed?.Invoke("Unknown");
				break;
		}
		_onRewardCompleted = null;
		_onRewardSkipped = null;
		_onRewardFailed = null;
	}

	public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
	{
		_onRewardFailed?.Invoke($"ShowFailed:{error}:{message}");
		_onRewardCompleted = null;
		_onRewardSkipped = null;
		_onRewardFailed = null;
	}
} 