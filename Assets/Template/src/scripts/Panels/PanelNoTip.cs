using UnityEngine;
using System.Collections;

using System;
using UnityEngine.UI;
public class PanelNoTip : MonoBehaviour {

	private void Start()
	{
		transform.Find("bg").Find("Text").GetComponent<Text>().text = Localization.Instance.GetString("noTipNow");
	}

	public void close()
	{
		gameObject.SetActive(false);
	}	
}
