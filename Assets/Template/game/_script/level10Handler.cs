using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



public class level10Handler : MonoBehaviour
{

    [HideInInspector]
    public GameObject girlNormal, girlBack, girlangryback, girlbacktouch, girlHappy, girlUnHappy, girlsitground,
        girlslap1, girlslap2, girlstand, girlwalk,girlchill,girlsign, ghostidle, ghostattack, ghostfail,windowframe,
        windowOpenL, windowOpenR, windowClosed,chair,btnTurnLeft,btnTurnRight;
    [HideInInspector]
    public GameObject angrymark;






    void Start()
    {

        //system varaible initialzation
        foreach (System.Reflection.MemberInfo m in typeof(level10Handler).GetMembers())
        {
            var tempVar = GameObject.Find(m.Name);
            if (tempVar != null)
            {
                this.GetType().GetField(m.Name).SetValue(this, tempVar);
            }
        }

        GameManager.instance.playMusic("bgcalmloop");
      

    }




    




    int n = 0;
    
    IEnumerator loop()
    {
        while (true)
        {
            n++;
        }
    }

    


    bool ghostMoving = true;
    float ghostspd = .01f;
    int nRound = 0;
    bool ghoststoped = false;
    bool isBlocking;
    // Update is called once per frame
    void FixedUpdate()
    {
        if (ghostidle!=null)
        {
            float tdis = windowframe.transform.position.x - ghostidle.transform.position.x;
            if (ghostMoving)
            {
                if (windowOpen)
                {
                    ghostMoving = false;
                    ghostidle.transform.DOMoveX(windowframe.transform.position.x, Mathf.Abs(tdis / 1f)).OnComplete(() =>
                    {
                        
                        ghoststoped = true;
                        checkWindow();
                    });
                }
                else
                {

                    if ((ghostidle.transform.position.x + ghostspd > windowframe.transform.position.x + 2)
                        || (ghostidle.transform.position.x + ghostspd < windowframe.transform.position.x - 2))
                    {
                        ghostspd *= -1;
                        nRound++;
                        if (nRound == 2)
                        {
                            ghostMoving = false;
                            ghostidle.transform.DOMoveX(windowframe.transform.position.x, Mathf.Abs(tdis / 1f)).OnComplete(() =>
                            {

                                ghoststoped = true;
                                checkWindow();
                            });

                        }
                    }


                    ghostidle.transform.Translate(ghostspd, 0, 0);
                }
            }

            isBlocking = Mathf.Abs(windowframe.transform.position.x - chair.transform.position.x) < 1f;
            
        }
    }

  



    bool windowOpen;
    bool startOpen;
    bool isHoldingOn;
    public void useItem(string param)
    {
        if (GameData.instance.isLock) return;
        switch (param)
        {
            case "touchMe":
                if (windowOpen) return;
                GameData.instance.isLock = true;
                showHide(girlslap1, true);
                showHide(girlchill, false);
                transform.root.DOShakePosition(.5f, .3f, 10);
                StartCoroutine(Util.DelayToInvokeDo(() =>
                {
                    showHide(girlslap2, true);
                    showHide(girlslap1, false);
                    StartCoroutine("gameFailed");
                }, .1f));
                ghostMoving = false;
                GameManager.instance.playSfx("slap");
                break;
            case "openWindow":
                startOpen = true;
                isHoldingOn = true;
               
                StartCoroutine(Util.DelayToInvokeDo(() =>
                {
                  
                    startOpen = false;
                }, .5f));
                break;
            case "openWindow_up":
                isHoldingOn = false;
                if (startOpen)
                {
                    GameData.instance.isLock = true;
                    showHideUI(false);
                    windowOpen = true;
                    showHide(windowClosed, false);
                    showHide(windowOpenL, true);
                    showHide(windowOpenR, true);
                    if (ghoststoped)
                    {
                        checkWindow();
                    }
                }

                if (isBumping)
                {
                    GameData.instance.isLock = true;
                    windowOpen = true;
                    bumpWindow.Kill();
                    nBump = 10;
                    showHide(windowClosed, false);
                    showHide(windowOpenL, true);
                    showHide(windowOpenR, true);
                    if (ghoststoped)
                    {
                        checkWindow();
                    }
                }

                break;


        }

    }



    TweenHandle bumpWindow;
    int nBump = 0;
    bool isBumping;
    bool ghostFailed;
    void checkWindow()
    {

        if (ghostFailed) return;
        if (windowOpen)
        {
            if (bumpWindow != null)
            {
                bumpWindow.Kill();
            }
            ghostidle.GetComponent<SpriteRenderer>().sortingOrder = 10;
            
            
            //showHideUI(false);
            changeScenePos(0);
            
            ghostidle.transform.DOScale(ghostidle.transform.localScale * 1.2f, 2f).OnComplete(() => {
                showHide(ghostidle, false);
                showHide(ghostattack, true);
                showHide(girlchill, false);
                showHide(girlsitground, true);
                transform.root.DOShakePosition(.5f, .3f, 10);

                GameManager.instance.playSfx("hahaevil");
                GameManager.instance.playSfx("aghh");
                ghostattack.transform.DOScale(ghostattack.transform.localScale * 1.3f, .2f).SetLoops(4, LoopType.Yoyo).OnComplete(() => {
                    
                    
                    StartCoroutine("gameFailed");
                    
                });
            });
        }
        else
        {
            //bump the window;
            isBumping = true;
            Vector3 startScale = ghostidle.transform.localScale;
            changeScenePos(0);
            GameManager.instance.playSfx("shutdoor");
            bumpWindow = ghostidle.transform.DOScale(startScale * 1.1f, .5f).SetEase(EaseType.InBack).SetDelay(.3f)
                .OnComplete(() => {
                    bumpWindow.Kill();
                    ghostidle.transform.localScale = startScale;
                    windowClosed.transform.DOShakePosition(.5f, .3f, 10);
                    
                    nBump++;
                    bool isSafe = isHoldingOn && isBlocking;
                    if (!isSafe)
                    {
                        if (nBump >= 5)
                        {
                            windowOpen = true;
                            bumpWindow.Kill();
                            showHide(windowClosed, false);
                            showHide(windowOpenL, true);
                            showHide(windowOpenR, true);
                            isBumping = false;
                            float blockdis = Mathf.Abs(windowframe.transform.position.x - chair.transform.position.x);
                            if (!isHoldingOn && blockdis < 1.4f)
                            {
                                
                                print(blockdis);
                               
                                chair.transform.DORotate(new Vector3(0, 0, -100f),.4f).SetEase(EaseType.OutBounce);
                                GameManager.instance.playSfx("clapse"); 

                            }

                            
                        }
                    }
                    else
                    {
                        if (nBump >= 5)
                        {
                            GameData.instance.isLock = true;
                            ghostFailed = true;
                            showHide(ghostidle, false);
                            showHide(ghostfail, true);
                            GameManager.instance.playSfx("ghostsigh");
                            ghostfail.transform.DOMoveX(windowframe.transform.position.x - 3, 5f).SetDelay(1f).OnComplete(() => {
                                StartCoroutine("gameWin");
                                Camera.main.GetComponent<MovieNoise> ().enabled = false;
                                showHide(girlsign, true);
                                showHide(girlchill, false);
                                GameManager.instance.playSfx("release");
                            });
                        }
                    }

                    checkWindow();
                });
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


    void changeScenePos(int index, bool keeplock = false)
    {
        GameObject.Find("sceneContainer").GetComponent<SceneContainer>().manualScene(index, keeplock);
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




