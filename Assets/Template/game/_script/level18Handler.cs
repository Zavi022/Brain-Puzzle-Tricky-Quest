using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class level18Handler : MonoBehaviour
{

    [HideInInspector]
    public GameObject girlplayfire, girlfallback, girlback, girlrun, girlrelease,
        fireBig, fires_, Extinguisher, smoke;
    [HideInInspector]
    public GameObject angrymark, btnTurnLeft, btnTurnRight;



    TweenHandle flashFire;
    void Start()
    {

        //system varaible initialzation
        foreach (System.Reflection.MemberInfo m in typeof(level18Handler).GetMembers())
        {
            var tempVar = GameObject.Find(m.Name);
            if (tempVar != null)
            {
                this.GetType().GetField(m.Name).SetValue(this, tempVar);
            }
        }


        StartCoroutine("waitaframe");
        flashFire = fireBig.transform.DOScaleX(Random.Range(1f, 1.5f), 1f).SetEase(EaseType.Linear).SetLoops(-1,LoopType.Yoyo);
        flashFire = fireBig.transform.DOScaleY(Random.Range(1f, 1.2f), 1f).SetLoops(-1, LoopType.Yoyo);
        GameManager.instance.playMusic("bgcalmloop");
    }

    List<GameObject> fires = new List<GameObject>();
    IEnumerator waitaframe()
    {
        yield return new WaitForEndOfFrame();
        foreach (Transform tfire in fires_.transform)
        {
            fires.Add(tfire.gameObject);
            tfire.gameObject.SetActive(false);
        }

        StartCoroutine("dangerFire");


        //ItemInteractable tInteract = Extinguisher.GetComponent<ItemInteractable>();
        List<InteractiveTarget> interactiveTargets = new List<InteractiveTarget>();

        for (int i = 0; i < fires.Count; i++) {
            InteractiveTarget tTarget = new InteractiveTarget();
            tTarget.interactiveTarget = fires[i];
            tTarget.methodTarget = transform.root.gameObject;
            tTarget.methodName = "useItem";
            tTarget.param = "fireIndex_" + i;
            interactiveTargets.Add(tTarget);
        }

        Extinguisher.GetComponent<ItemInteractable>().interactiveTargets = interactiveTargets;
    }

    int[,] puzzles = {
        {1, 1, 0, 1, 0, 0, 1, 0},
        {0, 1, 0, 1, 1, 0, 1, 0},
        {0, 0, 1, 1, 0, 1, 0, 1},
        {1, 0, 1, 1, 1, 1, 1, 0}
    };
    IEnumerator dangerFire()
    {
        yield return new WaitForSeconds(1);
        fireBig.transform.DOScale(Vector3.one*1.5f, .1f);
        fireBig.transform.DOScale(1.5f, .1f);

        showHide(girlplayfire, false);
        showHide(girlfallback, true);
        int tRnd = (int)Random.Range(0, 4);
       
        for(int i = 0; i < fires.Count; i++)
        {
            bool isActive = puzzles[tRnd, i] == 0 ? false : true; 
            fires[i].GetComponent<BoxCollider2D>().enabled = false;
            fires[i].SetActive(isActive);
        }
        GameManager.instance.playSfx("fireexplode");
        GameManager.instance.playSfx("osh");
        GameManager.instance.playMusic("bgtense");
    }

    int checkTime = 0;
    void checkWin()
    {
        bool allDim = true ;
        foreach(GameObject tFire in fires)
        {
            if (tFire.activeSelf)
            {
                allDim = false;
                break;
            }
        }
        
        checkTime++;
        //使用次数以内
        if (checkTime < 7)
        {
            if (allDim)
            {
                GameData.instance.isLock = true;
                girlrun.transform.position = new Vector3(girlrun.transform.position.x, girlback.transform.position.y);
                GameData.instance.isLock = true;
                girlrun.GetComponent<SpriteRenderer>().flipX = false;
                girlrun.transform.DOMoveX(girlback.transform.position.x, 1f).
                    SetEase(Ease.Linear).OnComplete(() =>
                    {
                        showHide(girlback, true);
                        showHide(girlrun, false);
                        StartCoroutine(Util.DelayToInvokeDo(() =>
                        {
                            showHide(girlback, false);
                            showHide(girlrelease, true);
                            StartCoroutine("gameWin");
                            GameManager.instance.playSfx("release");
                        },1f));
                    });
                    
                       
            }
        }
        else
        {
            GameData.instance.isLock = true;
            SmoothTween.To(() => tAlpha, x => tAlpha = x, 1, .4f).SetEase(EaseType.Linear).OnComplete(() => {

            });
            StartCoroutine("gameFailed");
            GameManager.instance.playSfx("dangfail");
        }
    }

    bool isBig = false;
    // Update is called once per frame
    void Update()
    {

        if (Camera.main)
        {
            Camera.main.GetComponent<FilterFire>().Fade  = tAlpha;
        }
    }


    float tAlpha = 0;
    bool girlEscaped = false;
    public void useItem(string param)
    {
        
        if (GameData.instance.isLock) return;
        bool needCheck = true;
        if(param.Length > 10)
        {
            if(param.Substring(0,9) == "fireIndex")
            {
                needCheck = false;
                int fireIndex = int.Parse(param.Split('_')[1]);
                smoke.transform.position = fires[fireIndex].transform.position + new Vector3(0,.3f,0);
                GameData.instance.isLock = true;
                smoke.GetComponent<Animator>().SetTrigger("smoke");
                GameManager.instance.playSfx("zi");
                StartCoroutine(Util.DelayToInvokeDo(() =>
                {
                    GameData.instance.isLock = false;
                    fires[fireIndex].SetActive(false);
                    if(fireIndex > 0)
                    {
                        fires[fireIndex - 1].SetActive(!fires[fireIndex - 1].activeSelf);
                    }
                    if(fireIndex < fires.Count-1)
                    {
                        fires[fireIndex + 1].SetActive(!fires[fireIndex + 1].activeSelf);
                    }
                    smoke.transform.position = new Vector3(0, 1000f);
                    checkWin();
                }, .4f));
            }
        }
        if (!needCheck) return;
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
            case "tellLeave":
                showHide(girlfallback, false);
                girlrun.transform.position = girlfallback.transform.position + new Vector3(0,1,0);
                GameData.instance.isLock = true;
                girlrun.transform.DOMoveX(girlrun.transform.position.x - 3f, 1f).
                    SetEase(Ease.Linear).OnComplete(()=> {
                        
                    GameData.instance.isLock = false;
                        girlEscaped = true;
                        for (int i = 0; i < fires.Count; i++)
                        {
                            fires[i].GetComponent<BoxCollider2D>().enabled = true;
                            
                        }
                    });
                break;
            case "touchFire":
                GameData.instance.isLock = true;
                SmoothTween.To(() => tAlpha, x => tAlpha = x, 1, .4f).SetEase(EaseType.Linear).OnComplete(()=> {
                    
                });
                StartCoroutine("gameFailed");
                GameManager.instance.playSfx("dangfail");

                break;
            case "pickDis":
                if (!girlEscaped)
                {
                    GameData.instance.isLock = true;
                    changeScenePos(0);

                    SmoothTween.To(() => tAlpha, x => tAlpha = x, 1, .4f).SetEase(EaseType.Linear);
                    StartCoroutine("gameFailed");
                    GameManager.instance.playSfx("dangfail");
                }
                break;




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



