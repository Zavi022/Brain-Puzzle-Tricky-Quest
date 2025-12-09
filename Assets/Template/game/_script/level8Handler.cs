using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



public class level8Handler : MonoBehaviour
{

    [HideInInspector]
    public GameObject tolietNormal, tolietAngry, doorOpenSmall,doorOpenBig, doorclose;
    [HideInInspector]
    public GameObject paperplace, placedpaper, thankyou, aghh;
        
    public GameObject closet;

    



    void Start()
    {

        //system varaible initialzation
        foreach (System.Reflection.MemberInfo m in typeof(level8Handler).GetMembers())
        {
            var tempVar = GameObject.Find(m.Name);
            if (tempVar != null)
            {
                this.GetType().GetField(m.Name).SetValue(this, tempVar);
            }
        }

        GameManager.instance.playMusic("bgmusic1");

        // 未开门时禁止门边放置点交互
        SetPaperplaceInteractable(false);

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



    int nKnock;
    
    public void useItem(string param)
    {
        if (GameData.instance.isLock) return;
        switch (param)
        {
            case "putPaper":
                // 只有门小开状态（已正确敲门）才允许放厕纸
                if (doorOpenSmall == null || !doorOpenSmall.GetComponent<SpriteRenderer>().enabled)
                {
                    return;
                }
                // 接受放置后立刻关闭交互以防重复触发
                SetPaperplaceInteractable(false);
                showHide(placedpaper, true);
                GameData.instance.isLock = true;
                StartCoroutine(Util.DelayToInvokeDo(() =>
                {
                    showHide(placedpaper, false);
                    showHide(doorOpenSmall, false);
                    showHide(doorclose, true);
                    showHide(thankyou,true);
                    GameManager.instance.playSfx("thankyou");
                    StartCoroutine("gameWin");
                    thankyou.transform.DORotate(new Vector3(0, 0, 10), .3f).SetLoops(5, LoopType.Yoyo).OnComplete(()=> {
                        showHide(thankyou, false);
                    });
                }, 1f));
                break;

            case "knockDoor":
                nKnock++;
                GameManager.instance.playSfx("knock");
                if (nKnock == 1)
                {
                    StartCoroutine("waitNextKnock");
                }
                break;


        }

    }

    IEnumerator waitNextKnock()
    {
        yield return new WaitForSeconds(.5f);
        if (nKnock < 3)
        {
            GameData.instance.isLock = true;
            showHide(doorclose, false);
            showHide(doorOpenBig, true);
            showHide(tolietNormal, true);
            
            StartCoroutine(Util.DelayToInvokeDo(() =>
            {
                showHide(tolietNormal, false);
                showHide(tolietAngry, true);
                showHide(aghh, true);
                transform.root.DOShakePosition(.5f, .3f, 10);
                aghh.transform.DOScale(Vector3.one*2, .1f).SetLoops(6,LoopType.Yoyo);
                //ATween.ScaleTo(aghh, ATween.Hash("scale", Vector3.one*1.5f, "time", .1f, "loopType",ATween.LoopType.pingPong));
               


                GameManager.instance.playSfx("shutdoor");
                GameManager.instance.playSfx("aghh");
                StartCoroutine("gameFailed");
            }, 1f));
        }
        else
        {
            showHide(doorclose, false);
            showHide(doorOpenSmall, true);
            // 开启门小缝后允许在门边放置
            SetPaperplaceInteractable(true);
            
        }
        GameManager.instance.playSfx("kata");
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

	void SetPaperplaceInteractable(bool enable)
	{
		if (paperplace == null) return;
		var col = paperplace.GetComponent<Collider2D>();
		if (col != null) col.enabled = enable;
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
}



