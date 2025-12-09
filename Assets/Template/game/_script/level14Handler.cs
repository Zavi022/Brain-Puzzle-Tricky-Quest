using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



public class level14Handler : MonoBehaviour
{

    [HideInInspector]
    public GameObject girlNormal, girlback, girlangryback, girlbacktouch, girlstandhappy, girlUnHappy,
        girlslap1, girlslap2, girlstand, girlwalk, girlsweathappy, girlsweatunhappy, girlsleepcough1,
        girlCasualCough1, girlsweatcough, girlbikini, girlbikinisurpise, girlPlace,
        girlsleepunhappy, hotbagMask, hotbag, hotbagPlaced;
    public GameObject windowclose, windowOpenL, windowopenR, packupon,jeanspack;
    [HideInInspector]
    public GameObject angrymark, btnTurnLeft, btnTurnRight;
  


    void Start()
    {

        //system varaible initialzation
        foreach (System.Reflection.MemberInfo m in typeof(level14Handler).GetMembers())
        {
            var tempVar = GameObject.Find(m.Name);
            if (tempVar != null)
            {
                this.GetType().GetField(m.Name).SetValue(this, tempVar);
            }
        }

        GameManager.instance.playMusic("bgmusic1");
        StartCoroutine("waitaframe");

    }

    IEnumerator waitaframe()
    {
        yield return new WaitForEndOfFrame();

        hotbagMask.SetActive(false);
        jeanspack.GetComponent<ItemInteractable>().pickable = false;
    }

 

    // Update is called once per frame
    void Update()
    {

    }




    int radarParts = 0;
    bool isWindowClosed;
    public void useItem(string param)
    {
        if (GameData.instance.isLock) return;
       
        switch (param)
        {
            case "touchMe":
                GameData.instance.isLock = true;
                showHide(girlslap1, true);

                transform.root.DOShakePosition(.5f, .3f, 10);
                StartCoroutine(Util.DelayToInvokeDo(() =>
                {
                    showHide(girlslap2, true);
                    showHide(girlslap1, false);
                    StartCoroutine("gameFailed");
                }, .1f));
                GameManager.instance.playSfx("slap");
                break;
            case "closeWindow":
                if (isWindowClosed)
                {
                    showHide(windowOpenL, true);
                    showHide(windowopenR, true);
                    showHide(windowclose, false);
                    isWindowClosed = false;
                    GameManager.instance.playSfx("kata");
                }
                else
                {
                    showHide(windowOpenL, false);
                    showHide(windowopenR, false);
                    showHide(windowclose, true);
                    isWindowClosed = true;
                    GameManager.instance.playSfx("kata");
                }
                break;
            case "giveHotBag":
                GameData.instance.isLock = true;
                showHide(hotbagPlaced,true);
                if (!isWindowClosed)
                {
                    showHide(girlCasualCough1, false);
                    showHide(girlsleepcough1, false);
                    //showHide(girlsweathappy, false);
                    showHide(girlsweatcough, false);
                    showHide(girlsweatunhappy, true);
                    GameManager.instance.playSfx("sigh");
                    StartCoroutine("gameFailed");
                }
                else
                {
                    showHide(girlsweatcough, false);
                    showHide(girlsweathappy, true);
                    GameManager.instance.playSfx("giggle");
                    StartCoroutine("gameWin");
                }
                break;
            case "changeSleep":
                GameData.instance.isLock = true;
                hotbagMask.SetActive(false);
                showHide(girlCasualCough1, false);
                showHide(girlsleepcough1, true);
                
                StartCoroutine(Util.DelayToInvokeDo(() =>
                {
                    showHide(girlsleepcough1, false);
                    showHide(girlsleepunhappy, true);
                    GameManager.instance.playSfx("sigh");
                    StartCoroutine("gameFailed");
                }, 1f));
                break;
            case "changeBikini":
                GameData.instance.isLock = true;
                hotbagMask.SetActive(false);
                showHide(girlCasualCough1, false);
                showHide(girlbikini, true);
                
                StartCoroutine(Util.DelayToInvokeDo(() =>
                {
                    showHide(girlbikini, false);
                    showHide(girlbikinisurpise, true);
                    GameManager.instance.playSfx("aghh");
                    StartCoroutine("gameFailed");
                },1f));
                break;
            case "changeSweat":
                showHide(girlCasualCough1, false);
                showHide(girlsweatcough, true);
                hotbagMask.SetActive(true);
                StartCoroutine(Util.DelayToInvokeDo(() =>
                {
                    GameData.instance.isLock = true;
                    showHide(girlsweatcough, false);
                    showHide(girlsweatunhappy, true);
                    StartCoroutine("gameFailed");
                    GameManager.instance.playSfx("sigh");
                }, 3f));
                break;
          
                




        }

    }

    void beCollided(GameObject g) {

        if(g.name == "packupon")
        {
            g.GetComponent<BoxCollider2D>().enabled = false;
            g.GetComponent<DragMove>().canDrag = false;
            jeanspack.GetComponent<ItemInteractable>().pickable = true;
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
        GameManager.instance.playSfx("giveheart");

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



