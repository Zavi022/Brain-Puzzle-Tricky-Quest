using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class level9Handler : MonoBehaviour
{

    [HideInInspector]
    public GameObject girlNormal, girlback, girlangryback, girlbacktouch, girlHappy, girlstandhappy,
     girlUnHappy, workerStand, workergive1, workergive2, workersideIdle, workfix, workershy,
     girlslap1, girlslap2, girlstand, girlwalk;
    [HideInInspector]
    public GameObject angrymark, heart,heart2, dooropen, portrait, portraitbuld, maskphoto, brokenBulb, newgoodbulb,buldgood, bulbBad, bulbfixedon,  coin, loliside, loliface,
        mansidebuld, manfacebuld, mangive, manFace, manside, loliContainer, manContainer,floor,crisp;
    [HideInInspector]
    public GameObject btnTurnLeft, btnTurnRight;

    public GameObject closet1, bulbbroken;




    void Start()
    {

        //system varaible initialzation
        foreach (System.Reflection.MemberInfo m in typeof(level9Handler).GetMembers())
        {
            var tempVar = GameObject.Find(m.Name);
            if (tempVar != null)
            {
                this.GetType().GetField(m.Name).SetValue(this, tempVar);

            }
        }

        crisp.SetActive(false);
        bulbBad.SetActive(false);
        workerStand.SetActive(false);
        StartCoroutine("loliPassby");
        StartCoroutine("manPassby");

        GameManager.instance.playMusic("bgmusic1");
    }

    TweenHandle loliWalk;
    TweenHandle loliUpDown;
    IEnumerator loliPassby()
    {
        yield return new WaitForSeconds(5);
        loliWalkAnim();
    }

    void loliWalkAnim()
    {
        float tTime = Mathf.Abs(closet1.transform.position.x - 1f - loliContainer.transform.position.x) / 1f;
        loliWalk = loliContainer.transform.DOMoveX(closet1.transform.position.x - 1f, tTime).SetEase(EaseType.Linear).OnComplete(() =>
        {
            showHide(loliside, false);
            loliWalk.Kill();
            loliUpDown.Kill();
            loliContainer.SetActive(false);
        });
        loliUpDown = loliside.transform.DOLocalMoveY(loliside.transform.localPosition.y + .1f, .3f).SetLoops(-1, LoopType.Yoyo).SetUpdate(true);
    }
    TweenHandle manWalk;
    TweenHandle manUpDown1, manUpDown2;
    IEnumerator manPassby()
    {
        yield return new WaitForSeconds(15);
        manWalkAnim();
    }
    void manWalkAnim()
    {
        float tTime = Mathf.Abs(closet1.transform.position.x - 1f - manContainer.transform.position.x) / 1f;
        manWalk = manContainer.transform.DOMoveX(closet1.transform.position.x - 1f, tTime).SetEase(EaseType.Linear).OnComplete(() =>
        {
            showHide(mansidebuld, false);
            showHide(manside, false);
            manUpDown1.Kill();
            manUpDown2.Kill();
            manWalk.Kill();
            manContainer.SetActive(false);
        });
        manUpDown1 = mansidebuld.transform.DOLocalMoveY(mansidebuld.transform.localPosition.y + .1f, .3f).SetLoops(-1, LoopType.Yoyo).SetUpdate(true);
        manUpDown2 = manside.transform.DOLocalMoveY(manside.transform.localPosition.y + .1f, .3f).SetLoops(-1, LoopType.Yoyo).SetUpdate(true);

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




    bool gotCoin;
    bool bulbOccupied = true;
    bool doorOpened;
    public void useItem(string param)
    {
        if (GameData.instance.isLock) return;
        switch (param)
        {
            case "touchMe":
                GameData.instance.isLock = true;
                showHide(girlslap1, true);
                GameManager.instance.playSfx("slap");
                transform.root.DOShakePosition(.5f, .3f, 10);
                StartCoroutine(Util.DelayToInvokeDo(() =>
                {
                    showHide(girlslap2, true);
                    showHide(girlslap1, false);
                    StartCoroutine("gameFailed");
                }, .1f));
                break;

            case "openCloset":
                if (!closet1.activeSelf)
                {
                    GameManager.instance.playSfx("openCloset");
                }
                closet1.SetActive(true);
                break;
            case "openDoor":
                doorOpened = true;
                showHide(dooropen, true);
                workerStand.SetActive(true);

                GameManager.instance.playSfx("kata");

                showHide(workerStand, true);
                if (!gotCoin)
                {
                    GameData.instance.isLock = true;
                    StartCoroutine(Util.DelayToInvokeDo(() =>
                    {
                        changeScenePos(1,true);
                        showHide(workerStand, false);
                        showHide(workersideIdle, true);
                        StartCoroutine(Util.DelayToInvokeDo(() =>
                        {
                            showHide(workersideIdle, false);
                            showHide(workfix, true);
                            GameManager.instance.playSfx("screw");
                            StartCoroutine(Util.DelayToInvokeDo(() =>
                            {
                                showHide(brokenBulb, false);
                                bulbBad.SetActive(false);
                                showHide(bulbfixedon,true);
                                workerStand.transform.position = workershy.transform.position;
                                showHide(workfix, false);
                                showHide(workerStand, true);
                                
                                StartCoroutine(Util.DelayToInvokeDo(() =>
                                {
                                    showHide(bulbfixedon, false);
                                    showHide(buldgood, true);
                                    showHide(girlbacktouch, true);
                                    girlbacktouch.GetComponent<SpriteRenderer>().flipX = true;
                                    showHide(girlback, false);
                                    GameManager.instance.playSfx("ding");
                                    StartCoroutine(Util.DelayToInvokeDo(() =>
                                    {
                                        showHide(workerStand, false);
                                        showHide(workershy, true);
                                        GameManager.instance.playSfx("boysmile");
                                        GameManager.instance.playSfx("wow");
                                        SpriteRenderer tsp = GameObject.Find("heart").GetComponent<SpriteRenderer>();
                                        tsp.enabled = true;
                                        //tsp.transform.DOScale(10, 1f);
                                        tsp.gameObject.transform.DOScale(Vector3.one*10f, 1f);
                                        tsp.DOFade(0, 2);
                                        StartCoroutine("gameFailed");
                                    }, 1));

                                }, 1f));

                            }, 1f));
                        }, 2f));
                    }, 1f));
                }

                break;
            case "getHair":
                showHide(portraitbuld, true);
                showHide(portrait, false);
                break;
            case "giveManHair":
                showHide(mansidebuld, false);
                showHide(manFace, true);
                manWalk.Kill();
                manUpDown1.Kill();
                manUpDown2.Kill();
                //GameData.instance.isLock = true;
                showHideUI(false);
                StartCoroutine(Util.DelayToInvokeDo(() =>
                {
                    showHide(manFace, false);
                    showHide(mangive, true);
                    GameManager.instance.playSfx("hehehehe");
                    //stop and give bulb
                    StartCoroutine(Util.DelayToInvokeDo(() =>
                    {
                        
                        GameData.instance.isLock = false;
                        coin.GetComponent<ItemInteractable>().fakeClick();
                        GameData.instance.isLock = true;
                        gotCoin = true;
                        showHide(manside, true);
                        showHide(mangive, false);
                        manWalkAnim();
                        GameData.instance.isLock = false;
                        showHideUI(true);

                        
                        GameManager.instance.playSfx("ding");
                    }, 1f));

                }, 2f));
                break;
            case "giveLoliHair":
                showHide(loliside, false);
                showHide(loliface, true);
                loliWalk.Kill();
                loliUpDown.Kill();
                GameManager.instance.playSfx("doubt");
                StartCoroutine(Util.DelayToInvokeDo(() =>
                {
                    showHide(loliside, true);
                    showHide(loliface, false);
                    loliWalkAnim();
                    StopCoroutine("manPassby");
                }, 1f));
                break;
            case "setGoodBulb":
                if (!bulbOccupied)
                {
                    GameData.instance.isLock = true;
                    bulbBad.SetActive(false);
                    bulbfixedon.SetActive(true);
                    showHide(bulbfixedon, true);
                    bulbOccupied = true;
                    removeItem(newgoodbulb);
                    StartCoroutine(Util.DelayToInvokeDo(() =>
                    {

                        showHide(bulbfixedon, false);
                        showHide(buldgood, true);
                        GameManager.instance.playSfx("ding");
                        StartCoroutine(Util.DelayToInvokeDo(() =>
                        {
                            showHide(girlback, false);
                            showHide(girlstandhappy, true);
                            StartCoroutine("gameWin");
                            GameManager.instance.playSfx("wow");
                            
                        }, 1f));
                    }, 1f));
                }
                break;
            case "setBadBulb":
                if (!bulbOccupied)
                {
                    GameData.instance.isLock = true;
                    bulbBad.SetActive(true);
                    bulbOccupied = true;
                    removeItem(bulbbroken);
                    StartCoroutine(Util.DelayToInvokeDo(() =>
                    {
                        StartCoroutine("blink");
                    }, 2f));
                    
                }
                break;
            case "getBrokenBulb":
                bulbOccupied = false;
                break;
            case "setBrokenBulb":
                if (!bulbOccupied)
                {
                    brokenBulb.SetActive(true);
                    removeItem(brokenBulb);
                    bulbOccupied = true;
                }
                break;
            case "giveMoney":
                if (doorOpened)
                {
                    showHide(workerStand, false);
                    showHide(workergive1, true);
                    showHideUI(false);
                    GameManager.instance.playSfx("kata");
                    StartCoroutine(Util.DelayToInvokeDo(() =>
                    {
                        showHide(workergive2, true);
                        showHide(workergive1, false);
                        
                        StartCoroutine(Util.DelayToInvokeDo(() =>
                        {
                            GameManager.instance.playSfx("ding");
                            newgoodbulb.GetComponent<ItemInteractable>().fakeClick();
                            showHide(workerStand,true);
                            showHide(workergive2, false);
                            showHideUI(true);
                        }, .5f));
                    }, 1f));
                }
                break;

        }

    }

    int nBlink = 0;
    IEnumerator blink()
    {
        while (nBlink < 20)
        {
            yield return new WaitForSeconds(.1f);
            
            showHide(buldgood, true);
            //bulbbad.SetActive(!bulbbad.activeSelf);
            buldgood.SetActive(!buldgood.activeSelf);
            nBlink++;
            if (nBlink == 20)
            {
                buldgood.SetActive(false);
                bulbBad.SetActive(true);
                //floor.SetActive(false);
                //bulbBad.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
                //showHide(bulbBad,false);
                bulbBad.SetActive(false);
                crisp.SetActive(true);


                explode(crisp);
                transform.root.DOShakePosition(.5f, .3f, 10);
                GameManager.instance.stopBGMusic();
                GameManager.instance.playSfx("break");
                StartCoroutine(Util.DelayToInvokeDo(() =>
                {
                    //showHide(bulbbroken, true);
                    showHide(girlback, false);
                    showHide(girlangryback, true);
                    showHide(angrymark, true);
                    StartCoroutine("gameFailed");
                    GameManager.instance.playSfx("angrydoubt");
                }, 1f));
            }
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

        SpriteRenderer tsp = GameObject.Find("heart2").GetComponent<SpriteRenderer>();
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


    void changeScenePos(int index,bool keeplock = false)
    {
        GameObject.Find("sceneContainer").GetComponent<SceneContainer>().manualScene(index, keeplock);
    }



    void removeItem(GameObject g)
    {
        List<GameObject> tItemPicked = GameData.instance.itemPicked;
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

    private Vector2 explosionPosition;
    public float radius = 5.0F;
    public float power = 10.0F;
    void explode(GameObject g)
    {
        Vector2 explosionPos = new Vector2(g.transform.position.x,g.transform.position.y);
        Collider2D[] colliders = Physics2D.OverlapCircleAll(explosionPos,radius);
        foreach (Collider2D hit in colliders)
        {
            Rigidbody2D rb = hit.GetComponent<Rigidbody2D>();
            
            if (rb != null)
                rb.AddForceAtPosition(new Vector2(Random.Range(-500,500),Random.Range(-500,500)), explosionPos);
        }
    }


    public float Force;
    //private void OnEndSplit(List<D2dDestructible> clones)
    //{
    //    // Go through all clones in the clones list
    //    for (var i = clones.Count - 1; i >= 0; i--)
    //    {
    //        var clone = clones[i];
    //        var rigidbody = clone.GetComponent<Rigidbody2D>();

    //        // Does this clone have a Rigidbody2D?
    //        if (rigidbody != null)
    //        {
    //            // Get the local point of the explosion that called this split event
    //            var localPoint = (Vector2)clone.transform.InverseTransformPoint(explosionPosition);

    //            // Get the vector between this point and the center of the destructible's current rect
    //            var vector = clone.AlphaRect.center - localPoint;

    //            // Apply relative force
    //            rigidbody.AddRelativeForce(vector * Force, ForceMode2D.Impulse);
    //        }
    //    }
    //}

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



