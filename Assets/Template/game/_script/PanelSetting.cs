
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelSetting : MonoBehaviour
{
    // Start is called before the first frame update
    Transform bg;
    float startY;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }


    public void changeVolume(Slider slider)
    {
        GameManager.getInstance().changeVolume(slider.value);
        PlayerPrefs.SetFloat("volume", slider.value);
    }
    PanelMain panelMain;
    private void OnEnable()
    {
        if (bg == null)
        {
            panelMain = GameObject.Find("PanelMain").GetComponent<PanelMain>();
            bg = transform.Find("bg");
            startY = bg.GetComponent<RectTransform>().localPosition.y;
            refresh();
        }
        
        islock = true;

        


        

        float tVolume = PlayerPrefs.GetFloat("volume", 1f);
       // GameObject.Find("Slider").GetComponent<Slider>().value = tVolume;
        GameManager.instance.changeVolume(tVolume);

        GameData.instance.isSoundOn = PlayerPrefs.GetInt("sound", 0);
        GameData.instance.isSfxOn = PlayerPrefs.GetInt("sfx", 0);
        GameObject.Find("ToggleMusic").GetComponent<Toggle>().isOn = GameData.instance.isSoundOn == 0 ? true : false;
        GameObject.Find("ToggleSfx").GetComponent<Toggle>().isOn = GameData.instance.isSfxOn == 0 ? true : false;


        int cLan = GameData.instance.GetSystemLaguage();
        
//         GameObject.Find("Radio " + cLan).GetComponent<Toggle>().isOn = true;
        

        bg.GetComponent<RectTransform>().DOLocalMoveY(0, .2f).OnComplete(()=> {
            islock = false;
        });
        GameManager.instance.playSfx("menuDrop");
    }

    void initText()
    {
        bg.Find("Header").Find("Text 1").GetComponent<Text>().text = Localization.Instance.GetString("settingTitle");
        bg.Find("Header").Find("Text 2").GetComponent<Text>().text = Localization.Instance.GetString("settingTitle");

        //GameObject.Find("settingMusic").GetComponent<Text>().text = Localization.Instance.GetString("settingMusic");
        //GameObject.Find("settingSfx").GetComponent<Text>().text = Localization.Instance.GetString("settingSfx");
        //GameObject.Find("settingVolume").GetComponent<Text>().text = Localization.Instance.GetString("settingVolume");
        //GameObject.Find("settingLanguage").GetComponent<Text>().text = Localization.Instance.GetString("settingLanguage");
    }

    private void OnDisable()
    {
        
    }
    bool islock = false;
    public void OnClick(GameObject g)
    {
        if (islock) return;
        switch (g.name)
        {
            case "btnYes":

                break;
            case "btnNo":
                islock = true;
                GameManager.instance.playSfx("menuUp");
                bg.GetComponent<RectTransform>().DOLocalMoveY(startY, .2f).OnComplete(()=> {
                    gameObject.SetActive(false);
                    islock = false;
                });
                break;
        }
    }

    public void OnToggle(Toggle toggle)
    {
        switch (toggle.gameObject.name)
        {
            case "ToggleMusic":
                GameManager.getInstance().playSfx("click");
                GameData.getInstance().isSoundOn = toggle.isOn ? 0 : 1;

                if (!toggle.isOn)
                {
                    GameManager.getInstance().stopBGMusic();
                }
                else
                {
                    GameManager.getInstance().playMusic("bgmusic");
                }
                PlayerPrefs.SetInt("sound", GameData.getInstance().isSoundOn);

                break;
            case "ToggleSfx":
                GameManager.getInstance().playSfx("click");
                GameData.getInstance().isSfxOn = toggle.isOn ? 0 : 1;
                if (!toggle.isOn)
                {
                    GameManager.getInstance().stopAllSFX();
                }

                PlayerPrefs.SetInt("sfx", GameData.getInstance().isSfxOn);
                break;
            case "Radio 0":
                GameManager.getInstance().playSfx("click");
                if (GameObject.Find("Radio 0").GetComponent<Toggle>().isOn)
                {
                    PlayerPrefs.SetInt("language", 0);
                    refresh();
                }
                break;
            case "Radio 1":
                GameManager.getInstance().playSfx("click");
                if(GameObject.Find("Radio 1").GetComponent<Toggle>().isOn){
                    PlayerPrefs.SetInt("language", 1);
                    refresh();
                }
                break;
        }
    }

    void refresh()
    {
        int clan = GameData.getInstance().GetSystemLaguage();
        Localization.Instance.SetLanguage(clan);
        initText();
        panelMain.initView();
    }
}
