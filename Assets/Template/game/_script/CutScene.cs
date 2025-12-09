
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class CutScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("waitaframe");
        StartCoroutine("tick");
        Localization.Instance.SetLanguage(GameData.getInstance().GetSystemLaguage()); 
        GameManager.getInstance().playMusic("bgMorning");
        for (int i = 0; i < 5; i++)
        {
            Image tImage = GameObject.Find("pic" + i).GetComponent<Image>();
            pics.Add(tImage);
            tImage.GetComponent<Image>().color = new Color(1, 1, 1, 0);

            if (i < 4)
            {
                GameObject tbubble = tImage.transform.Find("bubble").gameObject;
                tbubble.GetComponent<Image>().enabled = false;
                GameObject tText = tbubble.transform.Find("Text").gameObject;
                tText.GetComponent<Text>().enabled = false;
                tText.GetComponent <Text>().text = Localization.Instance.GetString("startCut" + i);
            }
            else
            {
                GameObject tbubble = tImage.transform.Find("bubble0").gameObject;
                tbubble.GetComponent<Image>().enabled = false;
                GameObject tText = tbubble.transform.Find("Text").gameObject;
                tText.GetComponent<Text>().enabled = false;
                tText.GetComponent<Text>().text = Localization.Instance.GetString("startCut" + i+"_0");

                tbubble = tImage.transform.Find("bubble1").gameObject;
                tbubble.GetComponent<Image>().enabled = false;
                tText = tbubble.transform.Find("Text").gameObject;
                tText.GetComponent<Text>().enabled = false;
                tText.GetComponent<Text>().text = Localization.Instance.GetString("startCut" + i+"_1");
            }
        }
        startStep();

    }

    int ticks = 0;
    IEnumerator tick()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            ticks++;
        }
    }

    List<Image> pics = new List<Image>();
   
    IEnumerator waitaframe()
    {
        yield return new WaitForEndOfFrame();
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    int n = 0;
    void startStep()
    {
        
        pics[n].GetComponent<Image>().DOFade(1, 1);
        StartCoroutine(Util.DelayToInvokeDo(() =>
        {
            showText();
            StartCoroutine(Util.DelayToInvokeDo(() =>
            {
                GameObject dingdong = GameObject.Find("dingdong");
                dingdong.GetComponent<Image>().enabled = true; 
                dingdong.GetComponent<RectTransform>().DOLocalMoveX(dingdong.GetComponent<RectTransform>().localPosition.x + 100, 3f).SetDelay(.2f);
                dingdong.GetComponent<Image>().DOFade(0, 2).SetDelay(.2f);

                GameManager.instance.playSfx("dingdong"); 
                StartCoroutine("step2"); 
            }, 2f)); 
        }, 2f));

       
        
    }
    
    void showText()
    {
        if (n < 4)
        {
            pics[n].transform.Find("bubble").GetComponent<Image>().enabled = true;
            pics[n].transform.Find("bubble").GetComponentInChildren<Text>().enabled = true;
        }
        else
        {
            pics[n].transform.Find("bubble0").GetComponent<Image>().enabled = true;
            pics[n].transform.Find("bubble0").GetComponentInChildren<Text>().enabled = true;

            StartCoroutine(Util.DelayToInvokeDo(() =>
            {
                pics[n].transform.Find("bubble1").GetComponent<Image>().enabled = true;
                pics[n].transform.Find("bubble1").GetComponentInChildren<Text>().enabled = true;
                print("ticks" + ticks);
                //GameManager.getInstance().stopBGMusic();

                StartCoroutine("tutorial");

            }, 2f));

    }
            
    }

    IEnumerator step2()
    {
        yield return new WaitForSeconds(1);
        n++;
        pics[n].GetComponent<Image>().DOFade(1, 1);
        GameManager.instance.playSfx("enman");
        StartCoroutine(Util.DelayToInvokeDo(() =>
        {
            
            showText();
            StartCoroutine("step3");
        }, 1f));
    }
    IEnumerator step3()
    {
        yield return new WaitForSeconds(1);
        n++;
        pics[n].GetComponent<Image>().DOFade(1, 1);
        
        StartCoroutine(Util.DelayToInvokeDo(() =>
        {
            GameManager.instance.playSfx("erman");
            showText();
            StartCoroutine("step4");
        }, .5f));
    }
    IEnumerator step4()
    {
        yield return new WaitForSeconds(1f);
        n++;
        pics[n].GetComponent<Image>().DOFade(1, 1);
        GameManager.instance.playSfx("hiyou");
        StartCoroutine(Util.DelayToInvokeDo(() =>
        {
            showText();
            StartCoroutine("step5");
        }, 1f));
    }

    IEnumerator step5()
    {
        yield return new WaitForSeconds(2);
        n++;
        pics[n].GetComponent<Image>().DOFade(2, 1);
        
        StartCoroutine(Util.DelayToInvokeDo(() =>
        {
            GameManager.instance.playSfx("boysmile");
            showText();
        }, 1f));
    }

    IEnumerator tutorial()
    {
        yield return new WaitForSeconds(5);
        Image mask = GameObject.Find("mask").GetComponent<Image>();
        mask.enabled = true;
        mask.color = new Color(0, 0, 0, 0);
        mask.DOFade(1, 2).OnComplete(()=> {
            if(PlayerPrefs.GetInt("StartCutPlayed") == 1)
            {
                SceneManager.LoadScene("LevelMenu");
            }
            else
            {
                SceneManager.LoadScene("Level0");
            }
            PlayerPrefs.SetInt("StartCutPlayed", 1);
        });
    }

}
