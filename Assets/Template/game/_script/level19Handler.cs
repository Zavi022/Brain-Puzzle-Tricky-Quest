using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



public class level19Handler : MonoBehaviour
{

    [HideInInspector]
    public GameObject girlNormal, girlScareDown, girlrun, girlback, girlcouches, girlcouch1, girlcouch2, girlcouch3,girlcallout;
    [HideInInspector]
    public GameObject crackers, bigBG, coverBG, hammer, wallhole, wallholemask,stone;
    [HideInInspector]
    public GameObject angrymark, btnTurnLeft, btnTurnRight;



   
    void Start()
    {

        //system varaible initialzation
        foreach (System.Reflection.MemberInfo m in typeof(level19Handler).GetMembers())
        {
            var tempVar = GameObject.Find(m.Name);
            if (tempVar != null)
            {
                this.GetType().GetField(m.Name).SetValue(this, tempVar);
            }
        }

        StartCoroutine("waitaframe");
        GameManager.instance.playMusic("bgtense");

    }
    List<GameObject> girlCouchs = new List<GameObject>();
    List<GameObject> crackers_ = new List<GameObject>();
    IEnumerator waitaframe()
    {
        yield return new WaitForEndOfFrame();
        foreach(Transform tCrack in crackers.transform)
        {
            crackers_.Add(tCrack.gameObject);
        }

        foreach (Transform girlcouch in girlcouches.transform)
        {
            girlCouchs.Add(girlcouch.gameObject);
        }

        StartCoroutine("tick");

        
        bigBG.transform.DOMoveX(bigBG.transform.position.x + strength, .1f).SetLoops(-1);
    }

    int[] damage = { 0, 0, 0,0,0 };
    bool cantick = true;
    float strength = .03f;
    int lastDamager = -1;
    IEnumerator tick()
    {
        while (cantick)
        {
            yield return new WaitForSeconds(1);
            int tRnd = (int)Random.Range(0, crackers_.Count);
            GameObject tCracker = GameObject.Instantiate(crackers_[tRnd], bigBG.transform);
            tCracker.transform.localEulerAngles = new Vector3(0, 0, Random.Range(0, 360));
            int tRndPosX = (int)Random.Range(0, 4);
            while(tRndPosX == lastDamager)
            {
                tRndPosX = (int)Random.Range(0, 4);
            }
            lastDamager = tRndPosX;
            float tx = GameObject.Find("loc" + tRndPosX).transform.position.x;
            tCracker.transform.position = new Vector3(tx+Random.Range(-1f,1f), -2f, 0);
            damage[tRndPosX]++;

            int trnd = (int)Random.Range(0, 2);
            GameManager.instance.playSfx("crack" + trnd);

            if(damage[tRndPosX] == 5)
            {
                cantick = false;
                StartCoroutine("soonCallpse");
                
            }
        }
    }


    IEnumerator soonCallpse()
    {
        yield return new WaitForSeconds(5);
        GameData.instance.isLock = true;
        showHideUI(false);
        changeScenePos(4);
        createStone();
        StartCoroutine("fadeFail");
        coverBG.GetComponent<SpriteRenderer>().DOFade(1, 5).SetDelay(3);

    }

    IEnumerator fadeWin()
    {
        yield return new WaitForSeconds(3);
        
        StartCoroutine("gameWin");
    }

    IEnumerator fadeFail()
    {
        yield return new WaitForSeconds(3);
   
        StartCoroutine("gameFailed");

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

        


        }

    }

    public void hitWall(string param)
    {
        GameData.instance.isLock = true;
        GameManager.instance.playSfx("break");
        GameManager.instance.playSfx("clapse");

        StopCoroutine("soonCallpse");
        hammer.SetActive(true);
        hammer.transform.position = GameObject.Find("loc" + param).transform.position + new Vector3(0, -1.6f, 0);
        hammer.transform.localEulerAngles = new Vector3(0, 0, 15f);
        hammer.transform.DORotate(new Vector3(0, 0, 180f), .1f).OnComplete(()=> {
            transform.root.DOShakePosition(.5f,1,10);
            wallhole.transform.position = hammer.transform.position + new Vector3(0,1,0);
            hammer.SetActive(false);
            float tTime = Mathf.Abs((wallhole.transform.position.x - girlrun.transform.position.x)) / 10f;
            girlrun.transform.position = girlScareDown.transform.position;
            showHide(girlScareDown, false);
            showHide(girlNormal, false);
            showHide(girlrun, true);
            //是不是可以砸开的地方,如果不可以，游戏失败。
            if (damage[int.Parse(param)] != 5)
            {
                wallholemask.SetActive(false);

                girlrun.transform.DOMoveX(wallhole.transform.position.x, tTime).SetEase(EaseType.Linear).OnComplete(() => {
                    showHide(girlrun, false);
                    showHide(girlback, true);
                    girlback.transform.position = girlrun.transform.position;

                    StartCoroutine(Util.DelayToInvokeDo(() =>
                    {
                        girlcallout.transform.position = girlback.transform.position + new Vector3(-.4f, 0, 0);
                        showHide(girlback, false);
                        showHide(girlcallout, true);
                        StartCoroutine("gameFailed");
                        GameManager.instance.playSfx("angrydoubt");
                    }, 1f));
                });
            }
            else
            {
                girlrun.transform.DOMoveX(wallhole.transform.position.x, tTime).SetEase(EaseType.Linear).OnComplete(() =>
                {
                    showHide(girlrun, false);
                    showHide(girlback, true);
                    girlback.transform.position = girlrun.transform.position;

                    StartCoroutine("climb");
                });
            }
            
            
        });
    }

    int nClimb;
    IEnumerator climb()
    {
        
        while (nClimb < girlCouchs.Count)
        {
            yield return new WaitForSeconds(1);
            showHide(girlback, false);
            for (int i = 0; i < girlCouchs.Count; i++)
            {
                girlCouchs[i].SetActive(false);
            }
            girlCouchs[nClimb].SetActive(true);
            girlCouchs[nClimb].transform.position = wallhole.transform.position + new Vector3(0,-nClimb*.13f,0);
            girlCouchs[nClimb].transform.localScale = new Vector3(girlCouchs[nClimb].transform.localScale.x * Mathf.Pow(.8f, nClimb),
                girlCouchs[nClimb].transform.localScale.y * Mathf.Pow(.8f,nClimb), 1);
            
            nClimb++;
            if(nClimb == girlCouchs.Count)
            {
                
                createStone();
                StartCoroutine("fadeWin");
                GameData.instance.isLock = true;
                coverBG.GetComponent<SpriteRenderer>().DOFade(1, 5).SetDelay(1);
                showHideUI(false);
                

            }
        }
    }

    void createStone()
    {
        for (int i = 0; i < 30; i++)
        {
            GameObject tstone = GameObject.Instantiate(stone, bigBG.transform);
            tstone.transform.position = new Vector3(GameObject.Find("loc0").transform.position.x + i * Random.Range(.3f, 1f) - 1,
                Random.Range(3f, 4f));
            tstone.GetComponent<Rigidbody2D>().isKinematic = false;
            tstone.GetComponent<StoneFall>().enabled = true;
            tstone.GetComponent<StoneFall>().startPos = tstone.transform.position;
            tstone.GetComponent<Rigidbody2D>().gravityScale = Random.Range(.8f, 1.3f);
        }
        GameManager.instance.playMusic("bgCollapse");
    }

    IEnumerator gameFailed()
    {
        yield return new WaitForSeconds(1);
        StopAllCoroutines();
        GameData.instance.main.gameFailed();
        GameManager.instance.stopBGMusic();
    }

    IEnumerator gameWin()
    {
        yield return new WaitForSeconds(1);
        StopAllCoroutines();
        GameManager.instance.stopBGMusic();

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



