using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelTips : MonoBehaviour
{
    RectTransform bg;
    Button btnClose;
    Vector3 startPos;
    List<Text> tipTexts;
    List<Button> tipButtons;
    List<GameObject> locked;
    public GameObject panelNoTip;
    void Start()
    {


    }

    private void OnDisable()
    {
        Time.timeScale = 1;
        
    }

    private void OnEnable()
    {
        Time.timeScale = 0;
        if (tipTexts == null)
        {
            tipTexts = new List<Text>();
            tipButtons = new List<Button>();
            locked = new List<GameObject>();
            bg = transform.Find("bg").GetComponent<RectTransform>();
            bg.transform.Find("Header").Find("Text 1").GetComponent<Text>().text = Localization.Instance.GetString("tipTitle");
            bg.transform.Find("Header").Find("Text 2").GetComponent<Text>().text = Localization.Instance.GetString("tipTitle");
            for (int i = 0; i < 3; i++)
            {
                
                startPos = bg.transform.localPosition;
                Transform tbar = bg.Find("bar" + i);
                Text tTip = tbar.transform.Find("Text").GetComponent<Text>();
                tipTexts.Add(tTip);

                Button tButton = tbar.transform.Find("btnOpenTip").GetComponent<Button>();
                tipButtons.Add(tButton);

                tButton.onClick.AddListener(delegate ()
                {
                    clickTipDetail(tButton.gameObject);
                });

                GameObject tLocked = tbar.transform.Find("locked").gameObject;
                locked.Add(tLocked);
            }

            btnClose = bg.transform.Find("btnClose").GetComponent<Button>();
            btnClose.onClick.AddListener(clickClose);
        }

        bg.DOLocalMoveY(1, .2f).SetUpdate(true);

        


        GameManager.instance.playSfx("menuDrop");

        refreshTips();
    }

    public void refreshTips()
    {
        int clevelTips = PlayerPrefs.GetInt("level" + GameData.getInstance().cLevel + "tips");
        for (int i = 0; i < 3; i++)
        {
            if (i >= clevelTips)
            {
                
                
                if (i > (clevelTips))
                {
                    locked[i].SetActive(true);
                    tipButtons[i].gameObject.SetActive(false);
                    tipTexts[i].text = Localization.Instance.GetString("tipLocked");// "此项需先解锁上面一个提示。";
                }
                else
                {
                    tipButtons[i].gameObject.SetActive(true);
                    locked[i].SetActive(false);
                    tipTexts[i].text = Localization.Instance.GetString("tipAds"); //"点击右边按钮，观看一段广告后即可解锁。";
                }
            }
            else
            {
                string tstr = "level" + GameData.instance.cLevel + "tips" + i;
                tipTexts[i].text = Localization.Instance.GetString(tstr);
                tipButtons[i].gameObject.SetActive(false);

                locked[i].SetActive(false);

            }
        }
    }

        void clickClose()
        {
            GameManager.instance.playSfx("menuUp");
            bg.DOLocalMoveY(startPos.y, .2f).SetUpdate(true).OnComplete(() => {
                    gameObject.SetActive(false);
                });
        }

        void clickTipDetail(GameObject g)
        {
           GameManager.instance.showGoogleRewardedAd();
        }

        public void noReward()
        {
            if (panelNoTip != null)
            {
                panelNoTip.SetActive(true);
            }
        }

        public void rewardError()
        {
            if (panelNoTip != null)
            {
                panelNoTip.SetActive(true);
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        //public void getReward()
        //{
        //    int clevelTips = PlayerPrefs.GetInt("level" + GameData.getInstance().cLevel + "tips");
        //    clevelTips++;
        //    PlayerPrefs.SetInt("level" + GameData.getInstance().cLevel + "tips", clevelTips);
        //    refreshTips();
        //}
    }
