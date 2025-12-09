using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class level0Handler : MonoBehaviour
{

    [HideInInspector]
    public GameObject girlAwkard, girlbow, txtIntro1, txtIntro2, txtIntro3, txtIntro4, txtIntro5, txtIntro6,txtIntro7,txtIntro8,
        Intro1, Intro2, Intro3, Intro4, Intro5, Intro6, Intro7,Intro8,
        txtNext1, txtNext2, txtNext3, txtNext4, txtNext5,txtNext6,txtNext7,txtNext8;
    [HideInInspector]
    public GameObject hand4_1, hand4_2,hand7_1,hand7_2, arrowPush1,arrowPush2,tree1,tree1fake,key, askKey;
    [HideInInspector]
    public GameObject btnTurnLeft, btnTurnRight;



   
    void Start()
    {

        //system varaible initialzation
        foreach (System.Reflection.MemberInfo m in typeof(level0Handler).GetMembers())
        {
            var tempVar = GameObject.Find(m.Name);
            if (tempVar != null)
            {
                this.GetType().GetField(m.Name).SetValue(this, tempVar);
            }
        }

        StartCoroutine("waitaframe");
        
    }
    IEnumerator waitaframe()
    {
        yield return new WaitForEndOfFrame();
        //step1 隐藏UI
        showHideUI(false);
        GameManager.getInstance().playMusic("bgmusic1");
        GameData.instance.isLock = true;
        txtIntro1.GetComponent<Text>().text = Localization.Instance.GetString("txtIntro1");
        txtIntro2.GetComponent<Text>().text = Localization.Instance.GetString("txtIntro2");
        txtIntro3.GetComponent<Text>().text = Localization.Instance.GetString("txtIntro3");
        txtIntro4.GetComponent<Text>().text = Localization.Instance.GetString("txtIntro4");
        txtIntro5.GetComponent<Text>().text = Localization.Instance.GetString("txtIntro5");
        txtIntro6.GetComponent<Text>().text = Localization.Instance.GetString("txtIntro6");
        txtIntro7.GetComponent<Text>().text = Localization.Instance.GetString("txtIntro7");
        txtIntro8.GetComponent<Text>().text = Localization.Instance.GetString("txtIntro8");

        txtNext1.GetComponent<Text>().text = Localization.Instance.GetString("txtNext1");
        //txtNext2.GetComponent<Text>().text = Localization.Instance.GetString("txtNext");
        txtNext3.GetComponent<Text>().text = Localization.Instance.GetString("txtNext3");
        //txtNext4.GetComponent<Text>().text = Localization.Instance.GetString("txtNext4");
        //txtNext5.GetComponent<Text>().text = Localization.Instance.GetString("txtNext5");
        txtNext8.GetComponent<Text>().text = Localization.Instance.GetString("txtNext8");

        Intro1.SetActive(false);
        Intro2.SetActive(false);
        Intro3.SetActive(false);
        Intro4.SetActive(false);
        Intro5.SetActive(false);
        Intro6.SetActive(false);
        Intro7.SetActive(false);
        Intro8.SetActive(false);

        step1();
    }

    int cStep = 0;
    void step1()
    {
        Intro1.SetActive(true);
        cStep = 1;
    }
    void step2()
    {
        Intro1.SetActive(false);
        Intro2.SetActive(true);
        showHideUI(true);
        cStep = 2;
        GameData.instance.isLock = false;
    }
    void step3()
    {
        Intro2.SetActive(false);
        Intro3.SetActive(true);
        showHideUI(false);
        cStep = 3;
        GameData.instance.isLock = true;
    }

    TweenHandle handMove1;
    Vector3 hand2StartPos;
   
    void step4()
    {
        if (isCollided)
        {
            if (handMove1 != null)
                handMove1.Kill();
            StopCoroutine("showhand2");
            return;
        }
        Intro3.SetActive(false);
        Intro4.SetActive(true);
        cStep = 4;
        showHideUI(false);
        GameData.instance.isLock = false;
        hand2StartPos = hand4_2.transform.position;
        hand4_1.SetActive(true);
        hand4_2.SetActive(false);
        arrowPush1.SetActive(false);
        StartCoroutine("showhand2");
    }


    IEnumerator showhand2()
    {
        yield return new WaitForSeconds(1);
        if (isCollided)
        {
            if (handMove1 != null)
                handMove1.Kill();
            StopCoroutine("showhand2");

        }
        else
        {
            hand4_2.SetActive(true);
            hand4_1.SetActive(false);
            arrowPush1.SetActive(true);
            if (handMove1 != null)
                handMove1.Kill();
            hand4_2.transform.position = hand2StartPos;
            handMove1 = hand4_2.transform.DOMoveX(hand2StartPos.x - 1f, 1).SetEase(EaseType.Linear).OnComplete(() =>
            {

                hand4_2.transform.position = hand2StartPos;
                if (!isCollided)
                {
                    if (handMove1 != null)
                        handMove1.Kill();
                    StopCoroutine("showhand2");
                    hand2StartPos = hand4_2.transform.position;
                    hand4_1.SetActive(true);
                    hand4_2.SetActive(false);
                    arrowPush1.SetActive(false);
                    StartCoroutine("showhand2");
                }
            });
        }
    }

    void step5()
    {
        cStep = 5;
        Intro5.SetActive(true);
        key.GetComponent<BoxCollider2D>().enabled = true;
    }

    void step6()
    {
        cStep = 6;
        showHideUI(true);
        Intro6.SetActive(true);
    }


    bool isGiven = false;
    void step7()
    {
        cStep = 7;
        showHideUI(false);
        Intro7.SetActive(true);
        hand7_1.SetActive(true);
        hand7_2.SetActive(false);
        arrowPush2.SetActive(false);
        hand2StartPos = hand7_2.transform.position;
        StartCoroutine("giveGirl");
    }

    void step8()
    {
        cStep = 8;
        Intro8.SetActive(true);
       
    }

    IEnumerator giveGirl()
    {
        yield return new WaitForSeconds(1);
        hand7_2.SetActive(true);
        hand7_1.SetActive(false);
        arrowPush2.SetActive(true);
        handMove1 = hand7_2.transform.DOMove(new Vector3(hand2StartPos.x + 2f, hand2StartPos.y + 2f, 0), 1f).SetEase(EaseType.Linear).OnComplete(() =>
        {
            if (!isGiven)
            {
                hand7_1.SetActive(true);
                hand7_2.SetActive(false);
                arrowPush2.SetActive(false);
                hand7_2.transform.position = hand2StartPos;
                StartCoroutine("giveGirl");
            }

        });
    }

    IEnumerator intro4Anim()
    {
        yield return new WaitForSeconds(1);
       
    }


    public void stepOver(GameObject g)
    {
        switch (g.name)
        {
            case "btnNext1":
                step2();
                break;
            case "btnNext3":
                step4();
                break;
            case "btnNext8":
                     if(AdsManager.Instance)
                     {
                         AdsManager.Instance.ShowInterstialAds();
                     }
                    g.transform.root.GetComponent<MainScript>().mask.gameObject.SetActive(true);
                    g.transform.root.GetComponent<MainScript>().mask.DOFade(1, 2).OnComplete(() =>
                    {
                        SceneManager.LoadScene("LevelMenu");
                    });
                break;
        }
    }
   



    // Update is called once per frame
    void Update()
    {
        
    }

    bool isCollided = false;
    void beCollided(GameObject g)
    {
        if(g.name == "tree1")
        {
            tree1.GetComponent<DragMove>().canDrag = false;
            tree1fake.SetActive(false);
            
            if (handMove1 != null)
                handMove1.Kill();
            StopCoroutine("showhand2");
            isCollided = true;
            step5();
            Intro4.SetActive(false);
        }
    }

  
    public void useItem(string param)
    {
        if (GameData.instance.isLock) return;
       
        switch (param)
        {
            case "touchMe":
                //GameData.instance.isLock = true;
                //showHide(girlslap1, true);

                //transform.root.DOShakePosition(.5f, .3f, 10);
                //StartCoroutine(Util.DelayToInvokeDo(() =>
                //{
                //    showHide(girlslap2, true);
                //    showHide(girlslap1, false);
                //    StartCoroutine("gameFailed");
                //}, .1f));
                break;
            case "pickKey":
                Intro5.SetActive(false);
                step6();
                break;
            case "giveKey":
                GameManager.instance.playSfx("ding");
                Intro7.SetActive(false);
                isGiven = true;
                StopCoroutine("giveGirl");
                            if (handMove1 != null)
                handMove1.Kill();
            showHide(girlAwkard, false);
                showHide(girlbow, true);
                showHide(askKey, false);
                GameManager.instance.playSfx("xiexie");
                step8();
                break;
        


        }

    }


    bool isCorrect = false;
    int cIndex = 0;
    void indexChanged(int cPos)
    {
       
        cIndex = cPos;
        if(cStep == 2)
        {
            step3();
        }else if(cStep == 6)
        {
            Intro6.SetActive(false);
            step7();
        }

    }


    IEnumerator gameFailed()
    {
        yield return new WaitForSeconds(1);
        StopAllCoroutines();
        GameData.instance.main.gameFailed();
    }

    IEnumerator gameWin()
    {
        yield return new WaitForSeconds(1);
        StopAllCoroutines();


        SpriteRenderer tsp = GameObject.Find("heart").GetComponent<SpriteRenderer>();
        //tsp.transform.position = girlstandhappy.transform.position + new Vector3(0,1f,0);
        tsp.enabled = true;
        tsp.transform.DOMoveY(1, 2f);
        tsp.DOFade(0, 2);
        GameData.instance.main.gameWin();
    }



    public void showHide(GameObject g, bool showOrHide)
    {
        g.GetComponent<SpriteRenderer>().enabled = showOrHide;
        Transform tshadow = g.transform.Find("shadow");
        if (tshadow != null)
        {
            if (tshadow.GetComponent<MeshRenderer>() != null)
            {
                tshadow.GetComponent<MeshRenderer>().enabled = (showOrHide);
            }
            else
            {
                tshadow.GetComponent<SpriteRenderer>().enabled = showOrHide;
            }
        }
    }


    void changeScenePos(int index)
    {
        GameObject.Find("sceneContainer").GetComponent<SceneContainer>().manualScene(index);
    }



    void removeItem(GameObject g)
    {
        List<GameObject> tItemPicked = GameData.instance.itemPicked;
        List<GameObject> tIndexs = new List<GameObject>();
        for (int i = 0; i < tItemPicked.Count; i++)
        {
            if (tItemPicked[i].name == g.name)

            {
                GameData.instance.itemPicked.Remove(g);
                break;
            }
        }

        GameObject.Find("items").GetComponent<UIItemBar>().refreshUI();
    }


    void showHideUI(bool showHide)
    {
        if (btnTurnLeft != null)
        {
            btnTurnLeft.SetActive(showHide);

        }
        else
        {
            btnTurnLeft = GetComponent<SceneContainer>().btnLeft.gameObject;
            btnTurnLeft.SetActive(showHide);
        }
        if (btnTurnRight != null)
        {
            btnTurnRight.SetActive(showHide);
        }
        else
        {
            btnTurnRight = GetComponent<SceneContainer>().btnRight.gameObject;
            btnTurnRight.SetActive(showHide);
        }
        if (showHide)
        {
            SendMessage("checkArrow");
        }
    }


}



