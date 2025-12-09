using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



public class level7Handler : MonoBehaviour
{

    [HideInInspector]
    public GameObject girlNormal, girlBack, girlangryback, girlbacktouch, girlHappy, girlUnHappy, girlslipdown,
        girlslap1,girlslap2,girlstand, girlwalk, girlpraise,girlback, __specialSp;
    [HideInInspector]
    public GameObject angrymark,tree1, waters,mop, mopUnuse;





    Transform[] colliders = new Transform[2];
    void Start()
    {

        //system varaible initialzation
        foreach (System.Reflection.MemberInfo m in typeof(level7Handler).GetMembers())
        {
            var tempVar = GameObject.Find(m.Name);
            if (tempVar != null)
            {
                this.GetType().GetField(m.Name).SetValue(this,tempVar);
            }
        }

        for(int i =0;i<waters.transform.childCount;i++)
        {
            Transform tCollider = waters.transform.GetChild(i);
            colliders[i] = tCollider;
        }

        StartCoroutine("girlCome");
        GameManager.instance.playMusic("bgmusic1");
    }
    int nCollider = 0;
    int cleanTimes = 0;
    public void beCollided(GameObject g)
    {
        foreach(Transform tcollider in colliders)
        {
            if (tcollider == g.transform)
            {
                tcollider.GetComponent<BoxCollider2D>().enabled = false;
                nCollider++;
                if (nCollider == 2)
                {
                    foreach (Transform tcollider_ in colliders)
                    {
                        tcollider_.GetComponent<BoxCollider2D>().enabled = true;
                        
                    }
                    cleanTimes++;
                    nCollider = 0;
                    if(cleanTimes > 3)
                    {
                        GameManager.instance.playSfx("mop");
                        SpriteRenderer tWaterSp = waters.GetComponent<SpriteRenderer>();
                        float talpha = tWaterSp.color.a - .05f;
                        tWaterSp.color = new Color(tWaterSp.color.r, tWaterSp.color.g, tWaterSp.color.b, talpha);
                        if(talpha < .02f)
                        {
                            tWaterSp.color = new Color(tWaterSp.color.r, tWaterSp.color.g, tWaterSp.color.b, 0);
                            removeItem(mop);
                            GameObject.Find("__specialSp").GetComponent<SpriteRenderer>().enabled = false;
                            showHide(mopUnuse,true);
                            //GameData.instance.isLock = true;

                            waterCleaned = true;
                            StopCoroutine("girlCome");
                            TimesUp();
                           
                        }
                    }
                    //print(cleanTimes + "clean");
                }
                break;
            }
        }
    }
    bool waterCleaned = false;
    IEnumerator girlCome()
    {
        yield return new WaitForSeconds(10);
        TimesUp();
    }
    void TimesUp()
    {
        GameData.instance.isLock = true;
        mop.GetComponent<ItemInteractable>().fakeUp();
        mop.SetActive(false);
        mopUnuse.SetActive(true);
        showHide(mopUnuse, true);
        __specialSp.SetActive(false); 


        girlwalk.transform.DOMoveX(waters.transform.position.x, 3).SetEase(EaseType.Linear).OnComplete(() =>
        {
            if (!waterCleaned)
            {
                showHide(girlwalk, false);
                showHide(girlslipdown, true);
                girlslipdown.GetComponent<Animator>().SetTrigger("slip"); 
                transform.root.DOShakePosition(.5f, .3f, 10);
                StartCoroutine("gameFailed");
                GameManager.instance.playSfx("aiyo");
            }
            else
            {
                showHide(girlback, true);
                showHide(girlwalk, false);
                StartCoroutine(Util.DelayToInvokeDo(() =>
                {
                    showHide(girlback, false);
                    showHide(girlpraise, true);
                    StartCoroutine("gameWin");
                    GameManager.instance.playSfx("en");
                }, 1f));

            }
        });
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
             
                transform.root.DOShakePosition(.5f, .3f, 10);
                StartCoroutine(Util.DelayToInvokeDo(() =>
                {
                    showHide(girlslap2, true);
                    showHide(girlslap1, false);
                    StartCoroutine("gameFailed");
                }, .1f));
                GameManager.instance.playSfx("slap");
                break;
           
          

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


        SpriteRenderer tsp = GameObject.Find("heart").GetComponent<SpriteRenderer>();
        tsp.enabled = true;
        tsp.transform.DOMoveY(1, 2f);
        tsp.DOFade(0, 2);
        GameData.instance.main.gameWin();
        GameManager.instance.playSfx("giveheart");
        
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




    void removeItem(GameObject g)
    {
        List<GameObject> tItemPicked = GameData.instance.itemPicked;
        List<GameObject> tIndexs = new List<GameObject>();
        for (int i = 0; i < tItemPicked.Count; i++)
        {
            //print(i + "===" + tItemPicked[i].name);
            if (tItemPicked[i].name == g.name)

            {
                GameData.instance.itemPicked.Remove(g);
                break;
            }
        }

        GameObject.Find("items").GetComponent<UIItemBar>().refreshUI();
    }
}


