using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



public class level6Handler : MonoBehaviour
{

    [HideInInspector]
    public GameObject girlNormal, girlBack, girlangryback, girlbacktouch, girlHappy, girlUnHappy,girlcook,
        girlslap1,girlslap2,girlstand, girlTalkPhone,girlwalk , girlCheckPhone, girlNoway;
    [HideInInspector]
    public GameObject boyflower,boyphoneHappy, boyphoneAnger,boyphoneDoubt,table,radarComplete,radarInBox, angrymark;
    
    public GameObject closet,doorOpen;




    
    void Start()
    {

        //system varaible initialzation
        foreach (System.Reflection.MemberInfo m in typeof(level6Handler).GetMembers())
        {
            var tempVar = GameObject.Find(m.Name);
            if (tempVar != null)
            {
                this.GetType().GetField(m.Name).SetValue(this,tempVar);
            }
        }

        GameManager.instance.playMusic("bgmusic1");

        StartCoroutine("phoneOver");

    }

    bool _boxPlaced = false;
    public void boxPlaced()
    {
        _boxPlaced = true;
        float tdis = girlTalkPhone.transform.position.x - girlCheckPhone.transform.position.x;
        float tTime = tdis / 3f;

        //girl walk to window;
        showHide(girlTalkPhone, false);
        showHide(boyphoneHappy, false);
        showHide(girlwalk, true);
        girlwalk.transform.position = girlTalkPhone.transform.position;
        girlwalk.GetComponent<SpriteRenderer>().flipX = true;


        girlTalkPhone.transform.position = new Vector3(girlCheckPhone.transform.position.x - .2f,girlTalkPhone.transform.position.y,0);
        StopCoroutine("phoneOver");
        girlwalk.transform.DOMoveX(girlTalkPhone.transform.position.x, tTime).SetEase(EaseType.Linear).OnComplete(() =>
        {
            girlslap1.transform.position = girlTalkPhone.transform.position;
            girlslap2.transform.position = girlTalkPhone.transform.position;
            showHide(girlwalk, false);
            showHide(girlTalkPhone, true);
            showHide(boyphoneHappy, true);
            boyphoneHappy.transform.position = boyphoneDoubt.transform.position;

            GameManager.instance.playSfx("giggle");

            StartCoroutine("phoneOver");
        });
    }

    IEnumerator phoneOver()
    {
        yield return new WaitForSeconds(30);
        GameData.instance.isLock = true;
        doorOpen.SetActive(false);
        changeScenePos(3);
        StartCoroutine(Util.DelayToInvokeDo(() =>
        {
            GameData.instance.isLock = true;
            girlwalk.transform.position = girlTalkPhone.transform.position;
            float tdis = doorOpen.transform.position.x - girlwalk.transform.position.x;
            float tTime = tdis / 3f;

            //girl walk to open door;
            showHide(girlTalkPhone, false);
            showHide(boyphoneHappy, false);
            showHide(girlwalk, true);
            girlwalk.GetComponent<SpriteRenderer>().flipX = false;
            girlwalk.transform.DOMoveX(doorOpen.transform.position.x, tTime).SetEase(EaseType.Linear).OnComplete(() =>
            {
                //reach door
                showHide(girlbacktouch, true);
                showHide(girlwalk, false);
                //opendoor
                StartCoroutine(Util.DelayToInvokeDo(() =>
                {
                    doorOpen.SetActive(true);
                    GameManager.instance.playSfx("kata");
                    showHide(boyflower, true);
                    GameManager.instance.playSfx("hi");
                    GameManager.instance.playSfx("wow");
                    SpriteRenderer tsp = GameObject.Find("heart").GetComponent<SpriteRenderer>();
                    tsp.enabled = true;
                    //tsp.transform.DOScale(10, 2f);
                    tsp.gameObject.transform.DOScale(Vector3.one*10, 2f);
                    tsp.DOFade(0, 2);
                    
                    StartCoroutine("gameFailed");
                }, 1f));
            });
            
           
        }, .3f));

    }

    private void FixedUpdate()
    {

    }




    int n = 0;
    IEnumerator loop()
    {
        while (true)
        {
            n++;
        }
    }


  


    // Update is called once per frame
    void Update()
    {

    }




    int radarParts = 0;
    public void useItem(string param)
    {
        if (GameData.instance.isLock) return;
        switch (param)
        {
            case "touchMe":
                GameData.instance.isLock = true;
                showHide(girlslap1, true);
                showHide(girlTalkPhone, false);
                showHide(boyphoneHappy, false);
                transform.root.DOShakePosition(.5f, .3f, 10);
                GameManager.instance.playSfx("slap");
                StopCoroutine("phoneOver");
                StopCoroutine("waitBoyLeave");
                StartCoroutine(Util.DelayToInvokeDo(() =>
                {
                    showHide(girlslap2, true);
                    showHide(girlslap1, false);
                    StartCoroutine("gameFailed");
                }, .1f));

                break;
            case "openDoor":
                if (!doorOpen.activeSelf)
                {
                    GameManager.instance.playSfx("kata");
                }
                doorOpen.SetActive(true);
                
                break;
            case "openCloset":
                if (!closet.activeSelf)
                {
                    GameManager.instance.playSfx("openCloset");
                }
                closet.SetActive(true);
               
                break;
            case "getUp":
                radarParts++;
                checkRadarComplete();
                break;
            case "getMiddle":
                radarParts++;
                checkRadarComplete();
                break;
            case "getBottom":
                radarParts++;
                checkRadarComplete();
                doorOpen.transform.parent.GetComponent<BoxCollider2D>().enabled = false;
                doorOpen.SetActive(false);
                GameManager.instance.playSfx("kata");
                break;
            case "putRadar":
                showHide(radarInBox,true);
                StopCoroutine("phoneOver");
                StartCoroutine("waitBoyLeave");
                GameData.instance.isLock = true;
                GameManager.instance.playSfx("radar");
                break;

        }

    }

    IEnumerator waitBoyLeave()
    {
        yield return new WaitForSeconds(3);
        showHide(boyphoneHappy, false);
        showHide(boyphoneDoubt, true);
        showHide(girlCheckPhone, true);
        showHide(girlTalkPhone, false);
        GameManager.instance.playSfx("ei");

        StartCoroutine(Util.DelayToInvokeDo(() =>
        {
            showHide(boyphoneDoubt, true);
            StartCoroutine(Util.DelayToInvokeDo(() =>
            {
                showHide(boyphoneDoubt, false);
                showHide(boyphoneAnger, true);
                StartCoroutine(Util.DelayToInvokeDo(() =>
                {
                    
                    showHide(boyphoneAnger, false);

                    StartCoroutine(Util.DelayToInvokeDo(() =>
                    {
                        showHide(girlCheckPhone, false);
                        showHide(girlNoway, true) ;
                        StartCoroutine("gameWin");
                        GameManager.instance.stopMusic("radar");
                        GameManager.instance.playSfx("ah");
                        
                    }, 1f));
                }, 1f));

            }, 1f));


        }, 2f));

    }


        void checkRadarComplete()
    {
        if (radarParts == 3)
        {
            GameData.instance.isLock = true;
            StartCoroutine(Util.DelayToInvokeDo(() =>
            {

                List<GameObject> tItemPicked = GameData.instance.itemPicked;
                List<GameObject> tIndexs = new List<GameObject>();
                for (int i = 0; i < tItemPicked.Count; i++)
                {
                    //print(i + "===" + tItemPicked[i].name);
                    if (tItemPicked[i].name == "radarpart1" || tItemPicked[i].name == "radarpart2"
                    || tItemPicked[i].name == "radarpart3")
                    {
                        tIndexs.Add(tItemPicked[i]);
                    }
                }
                foreach (GameObject tItem in tIndexs)
                {
                    GameData.instance.itemPicked.Remove(tItem);
                }
                GameObject.Find("items").GetComponent<UIItemBar>().refreshUI();
                
                
                StartCoroutine(Util.DelayToInvokeDo(() =>
                {
                    GameData.instance.isLock = false;
                    radarComplete.GetComponent<ItemInteractable>().fakeClick();

                    //GameObject.Find("items").GetComponent<UIItemBar>().refreshUI();

                },.1f));
            }, .1f));
        }
       
    }

 


        IEnumerator gameFailed()
    {
        yield return new WaitForSeconds(1);
        GameData.instance.main.gameFailed();
    }

    IEnumerator gameWin()
    {
        yield return new WaitForSeconds(1);
       
        //SpriteRenderer tsp = GameObject.Find("heart").GetComponent<SpriteRenderer>();
        //tsp.enabled = true;
        //tsp.transform.DOMoveY(1, 2f);
        //tsp.DOFade(0, 2);
        GameData.instance.main.gameWin();
    }



    public void showHide(GameObject g,bool showOrHide)
    {
        g.GetComponent<SpriteRenderer>().enabled = showOrHide;
        Transform tshadow = g.transform.Find("shadow");
        if ( tshadow!= null)
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

}



