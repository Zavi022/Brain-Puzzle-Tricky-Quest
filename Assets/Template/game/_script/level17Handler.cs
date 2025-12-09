using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



public class level17Handler : MonoBehaviour
{

    [HideInInspector]
    public GameObject girlkick, girlrun, girlsurround, girlback;
    public GameObject boysearch, boyshootlook, boyhurt, boyenvade, boyknife, boyknifelook, boyfly,
        boyfakegunshake, boyaim;
    public GameObject cake, catpic, gardenanim, yujia,police;
    public GameObject shows, boyAnim, girlAnim,remote,door,dooropen,surperise;
    [HideInInspector]
    public GameObject angrymark, btnTurnLeft, btnTurnRight;
  



    void Start()
    {

        //system varaible initialzation
        foreach (System.Reflection.MemberInfo m in typeof(level17Handler).GetMembers())
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
    List<GameObject> shows_ = new List<GameObject>();
    IEnumerator waitaframe()
    {
        yield return new WaitForEndOfFrame();
        foreach(Transform tboy in boyAnim.transform)
        {
            showHide(tboy.gameObject, false);
        }
        foreach (Transform tgirl in girlAnim.transform)
        {
            showHide(tgirl.gameObject, false);
        }
        foreach (Transform tShow in shows.transform)
        {
            shows_.Add(tShow.gameObject);
            tShow.gameObject.SetActive(false);
        }
        yujia.SetActive(true);
        girlback.transform.position = remote.transform.position;
        showHide(girlback, true);

        remote.SetActive(false);
        StartCoroutine("enemyCome");




    }


    IEnumerator enemyCome()
    {
        yield return new WaitForSeconds(2f);
        door.transform.DOShakeScale(.3f, .1f, 100);
        GameManager.instance.playSfx("shutdoor");
            showHideUI(false);
        StartCoroutine(Util.DelayToInvokeDo(() =>//看电视时敲门
        {

            changeScenePos(1);
            showHideUI(false);
            StartCoroutine(Util.DelayToInvokeDo(() =>//跑步看门
            {

                showHide(girlback, false);
                showHide(girlrun, true);
                girlrun.transform.DOMoveX(girlsurround.transform.position.x, .5f).OnComplete(() =>
                {//跑到门边
                    remote.SetActive(true);
                    girlback.transform.position = girlrun.transform.position;
                    showHide(girlrun, false);
                    showHide(girlback, true);
                    GameManager.instance.playSfx("shutdoor");
                    door.transform.DOShakeScale(.3f, .1f, 100).OnComplete(() =>
                    {//踢门
                        //GameManager.instance.playSfx("shutdoor");
                        door.transform.DOShakeScale(.3f, .1f, 100).SetDelay(1f).OnComplete(() =>
                        {//踢门
                            GameManager.instance.playSfx("shutdoor");
                            door.transform.DOShakeScale(.3f, .1f, 100).SetDelay(1f).OnComplete(() =>
                            {//门被踢开
                                GameManager.instance.playSfx("shutdoor");
                                GameManager.instance.playSfx("kata");
                                showHide(door, false);
                                showHide(dooropen, true);
                                showHide(boyknife, true);
                                showHide(surperise, true);
                                
                                GameManager.instance.playSfx("threat");

                                GameManager.instance.playMusic("bgtense");
                                StartCoroutine(Util.DelayToInvokeDo(() =>//隐藏惊讶，恢复操作
                                {
                                    showHideUI(true);
                                    showHide(surperise, false);
                                }, 1f));
                            });
                        });
                    });
                }).SetEase(EaseType.Linear);

            }, 1f));
        }, 1f));
    }


    // Update is called once per frame
    void Update()
    {

    }




    int radarParts = 0;
    bool isWarning;
    int lastShow = 4;
    int nSwitched = 0;
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

                int cpage = GetComponent<SceneContainer>().cPage;
                if (cpage == 1 && dooropen.GetComponent<SpriteRenderer>().enabled)
                {

                    if (!boyhurt.GetComponent<SpriteRenderer>().enabled)
                    {
                        GameData.instance.isLock = true;
                        showHide(girlback, false);
                        showHide(girlsurround, true);
                        StartCoroutine("gameFailed");
                    }
                    GameManager.instance.playSfx("osh");
                }
                //踢飞
                if (cpage == 1 && boyhurt.GetComponent<SpriteRenderer>().enabled)
                {
                    GameData.instance.isLock = true;
                    showHide(girlback, false);
                    showHide(girlkick,true);
                    StopAllCoroutines();
                    showHide(boyhurt, false);
                    showHide(boyfly, true);
                    boyfly.transform.Translate(-.2f, 1, 0);


                    boyfly.transform.DOScale(Vector3.zero, 2f).OnComplete(() => { kickedfly(); });

                    //boyfly.transform.DOScale(0, 2).OnComplete(()=> {
                    //    StartCoroutine("gameWin");
                    //});
                    GameManager.instance.playSfx("yaa");
                    GameManager.instance.playSfx("boyfail");
                }
                break;
            case "hitDistract":
                StopAllCoroutines();
                boyknife.SetActive(false);
                showHide(boyknifelook, false);
                showHide(boyhurt, true);
                GameManager.instance.playSfx("eren");
                StartCoroutine("girlRevenge");
                break;
            case "touchKnife":
                GameData.instance.isLock = true;
                if (!isWarning)
                {
                    showHide(boyknife, false);
                    showHide(boyenvade, true);
                    reKnife();
                    GameManager.instance.playSfx("eiboy");
                }
                break;
            case "touchTV":
                nSwitched++;
                int startIndex = nSwitched > 5?0:1;
                
                int cShowIndex = (int)Random.Range(startIndex, shows_.Count);
                while(cShowIndex == lastShow)
                {
                    cShowIndex = (int)Random.Range(startIndex, shows_.Count);
                }

                GameManager.instance.playSfx("switch");

                bool isPolice = cShowIndex == 0;
                
                for (int i = 0; i < shows_.Count; i++)
                {
                    shows_[i].SetActive(false);
                }
                shows_[cShowIndex].gameObject.SetActive(true);
                lastShow = cShowIndex;
                if (isPolice)
                {

                    GameManager.instance.playSfx("policecar");
                    GameData.instance.isLock = true;
                    showHideUI(false);
                    
                    StartCoroutine(Util.DelayToInvokeDo(() =>
                    {
                        changeScenePos(1);
                        if (boyknife.GetComponent<SpriteRenderer>().enabled)
                        {
                            boyknife.SetActive(false);
                            showHide(boyknifelook, true);
                            GameData.instance.isLock = false;
                            StartCoroutine("boyDistract");
                            GameManager.instance.playSfx("doubtman");
                        }
                    }, 1));
                }
                break;





        }

    }

    void kickedfly()
    {
        StartCoroutine("gameWin");
    }

    void reKnife()
    {
        StartCoroutine(Util.DelayToInvokeDo(() =>//重新拿起刀
        {
            if (!boyknife.activeSelf || !boyknife.GetComponent<SpriteRenderer>().enabled)
            {
                GameManager.instance.playSfx("threat");
            }
            boyknife.SetActive(true);
            showHide(boyknife, true);
            showHide(boyenvade, false);
           

            StartCoroutine(Util.DelayToInvokeDo(() =>//被刀威胁
            {
                showHide(girlback, false);
                showHide(girlsurround, true);
                GameManager.instance.playSfx("osh");
                StartCoroutine("gameFailed");
            }, .5f));
        }, .5f));
    }

    IEnumerator girlRevenge()
    {
        yield return new WaitForSeconds(1);
        GameData.instance.isLock = true;
        
        //摸枪
        StartCoroutine(Util.DelayToInvokeDo(() =>
        {
            showHide(boyhurt, false);
            showHide(boysearch, true);
                StartCoroutine(Util.DelayToInvokeDo(() =>
                {//摸出枪
                    showHide(boysearch, false);
                    showHide(boyaim, true);
                    GameManager.instance.playSfx("threat");
                    StartCoroutine(Util.DelayToInvokeDo(() =>
                    {//认输
                        showHide(girlback, false);
                        showHide(girlsurround, true);
                        StartCoroutine("gameFailed");
                        GameManager.instance.playSfx("osh");
                    }, .5f));
                }, .5f));
        }, 1f));
       
    }

    IEnumerator boyDistract()
    {
        yield return new WaitForSeconds(2);
        boyknife.SetActive(true);
        showHide(boyknifelook, false);
        showHide(boyknife, true);
        GameData.instance.isLock = true;
        GameManager.instance.playSfx("threat");
        reKnife();
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
        GameObject.Find("sceneContainer").GetComponent<SceneContainer>().manualScene(index,true);
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



