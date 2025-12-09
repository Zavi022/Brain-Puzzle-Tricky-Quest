using UnityEngine;
using UnityEngine.UI;

public sealed class TipsService
{
	private static TipsService _instance;
	public static TipsService Instance => _instance ?? (_instance = new TipsService());
	private TipsService() {}

	public int GetCurrentLevelTips()
	{
		int lv = GameData.getInstance().cLevel;
		return PlayerPrefs.GetInt("level" + lv + "tips", 0);
	}

	public void IncrementCurrentLevelTips(int amount = 1)
	{
		int lv = GameData.getInstance().cLevel;
		int v = PlayerPrefs.GetInt("level" + lv + "tips", 0) + amount;
		PlayerPrefs.SetInt("level" + lv + "tips", v);
		RefreshTipsPanelIfOpen();
	}

	public void RefreshTipsPanelIfOpen()
	{
		GameObject tpanel = GameObject.Find("PanelTips");
		if (tpanel != null)
		{
			var p = tpanel.GetComponent<PanelTips>();
			if (p != null)
			{
				p.refreshTips();
			}
		}
	}

	public void NotifyNoReward()
	{
		GameObject tpanel = GameObject.Find("PanelTips");
		if (tpanel != null)
		{
			var p = tpanel.GetComponent<PanelTips>();
			if (p != null)
			{
				p.noReward();
			}
		}
	}

	public void NotifyRewardError()
	{
		GameObject tpanel = GameObject.Find("PanelTips");
		if (tpanel != null)
		{
			var p = tpanel.GetComponent<PanelTips>();
			if (p != null)
			{
				p.rewardError();
			}
		}
	}
} 