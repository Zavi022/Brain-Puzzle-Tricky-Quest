using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class level1Handler : MonoBehaviour
{
    public GameObject bubble;
    public GameObject lip, cell, lace;
    public GameObject pillow1, pillow2, tree1;
    public GameObject draw;
    public GameObject girlSearch, girlBack, girlBackAngry, girlScare, girlHappy, girlUnHappy,girlSlap1,girlSlap2;
    public GameObject heart;
    public GameObject girlRun;
    void Start()
    {
        StartCoroutine("loop");
        GameManager.getInstance().playMusic("bgmusic1");
    }
    bool[] given = new bool[] { false, false, false };
    int currentRequirement;
    int n = 0;
    IEnumerator loop()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            if (n == 0 || n % 3 == 0)
            {

                currentRequirement = (int)Random.Range(0, 3);

                while (given[currentRequirement])
                {
                    currentRequirement = (int)Random.Range(0, 3);
                }
                for (int i = 0; i < 3; i++)
                {
                    Transform tRequire = bubble.transform.GetChild(i);
                    tRequire.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
                }
                Transform cRequire = bubble.transform.GetChild(currentRequirement);
                bubble.SetActive(true);
                bubble.transform.localScale = Vector3.zero;
                //bubble.transform.DOScale(.5f, .3f).SetEase(Ease.OutElastic);
                bubble.transform.DOScale(new Vector3(.5f,.5f,1), .3f).SetEase(EaseType.OutElastic);
                //bubble.transform.localScale = Vector3.one;
                cRequire.GetComponent<SpriteRenderer>().DOColor(new Color(1, 1, 1, 1), 1);
            }
            n++;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    bool meTouched = false;
    bool treeMoved = false;
    bool pillow1Moved = false;
    bool pillow2Moved = false;
    bool drawTouched = false;
    bool giveLip, giveCell, giveLace;
    public void useItem(string param)
    {
        if (GameData.instance.isLock) return;
        switch (param)
        {
            case "touchMe":
                if (!meTouched)
                {
                    meTouched = true;
     
                    girlSearch.SetActive(false);
                    StopCoroutine("loop");
                    bubble.SetActive(false);
                    girlSlap1.SetActive(true);
                    StartCoroutine("girlslap");
                    GameManager.instance.playSfx("slap");

                }
                break;
            case "moveTree":
                if (!treeMoved)
                {
                    GameData.instance.isLock = true;
                    treeMoved = true; 
                    lace.SetActive(true);
                    tree1.transform.DOMoveX(tree1.transform.position.x + .5f, .4f).OnComplete(() =>
                    { GameData.instance.isLock = false; });
                }
                break;
            case "movePillow1":
                if (!pillow1Moved)
                {
                    GameData.instance.isLock = true;
                    pillow1Moved = true;
                    cell.SetActive(true);
                    pillow1.transform.DOMoveX(pillow1.transform.position.x + .7f, .4f).OnComplete(() =>
                    { GameData.instance.isLock = false; }); 
                }
                break;
            case "movePillow2":
                if (!pillow2Moved)
                {
                    GameData.instance.isLock = true;
                    pillow2Moved = true;
                    lip.SetActive(true);
                    pillow2.transform.DOMoveX(pillow2.transform.position.x - .4f, .4f).OnComplete(() =>
                    { GameData.instance.isLock = false; });
                }
                break;
            case "touchDraw":
                if (!drawTouched)
                {
                    if (draw == null)
                    {
                        break;
                    }
                    if (!draw.activeInHierarchy)
                    {
                        draw.SetActive(true);
                    }

                    GameData.instance.isLock = true;
                    drawTouched = true;

                    Sequence tseq = new Sequence();
                    var h1 = draw.transform.DOShakeRotation(.3f, 30, 100).SetUpdate(true);
                    var h2 = draw.transform.DORotate(new Vector3(0, 0, -5), .2f).SetUpdate(true);
                    var h3 = draw.transform.DOMoveY(draw.transform.position.y - 1, .3f).SetUpdate(true);
                    tseq.Append(h1);
                    tseq.Append(h2);
                    tseq.Append(h3);
                    tseq.Play().OnComplete(() => {
                        girlComeAngry();
                    });

                    GameManager.instance.stopBGMusic();
                    GameManager.instance.playSfx("break");
                }
                break;
            case "giveCell":
                if (!giveCell)
                {
                    GameData.instance.isLock = true;
                    giveCell = true;
                    if (bubble.activeSelf && currentRequirement == 0)
                    {
                        given[0] = true;
                        bubble.SetActive(false);
                        GameManager.instance.playSfx("ding");
                        if (given[0] && given[1] && given[2])
                        {

                            showHeart();
                        }
                        else
                        {
                            GameData.instance.isLock = false;
                        }
                    }
                    else
                    {
                        GameManager.instance.playSfx("wrong");
                        GameManager.instance.playSfx("sigh");
                        girlSearch.SetActive(false);
                        girlUnHappy.SetActive(true);
                        bubble.SetActive(false);
                        StartCoroutine("gameFailed");
                    }

                }
                break;
            case "giveLip":
                if (!giveLip)
                {
                    GameData.instance.isLock = true;
                    giveLip = true;
                    if (bubble.activeSelf && currentRequirement == 1)
                    {
                        given[1] = true;
                        bubble.SetActive(false);
                        GameManager.instance.playSfx("ding");
                        if (given[0] && given[1] && given[2])
                        {
                            showHeart();
                        }
                        else
                        {
                            GameData.instance.isLock = false;
                        }
                    }
                    else
                    {
                        GameManager.instance.playSfx("wrong");
                        GameManager.instance.playSfx("sigh");
                        girlSearch.SetActive(false);
                        girlUnHappy.SetActive(true);
                        bubble.SetActive(false);
                        StartCoroutine("gameFailed");
                    }
                }
                break;
            case "giveLace":
                if (!giveLace)
                {
                    GameData.instance.isLock = true;
                    giveLace = true;
                    if (bubble.activeSelf && currentRequirement == 2)
                    {
                        given[2] = true;
                        bubble.SetActive(false);
                        GameManager.instance.playSfx("ding");
                        if (given[0] && given[1] && given[2])
                        {
                            showHeart();
                        }
                        else
                        {
                            GameData.instance.isLock = false;
                        }
                    }
                    else
                    {
                        GameManager.instance.playSfx("wrong");
                        GameManager.instance.playSfx("sigh");
                        girlSearch.SetActive(false);
                        girlUnHappy.SetActive(true);
                        bubble.SetActive(false);
                        StartCoroutine("gameFailed");
                    }
                }
                break;


        }

    }
    IEnumerator girlslap()
    {
        yield return new WaitForSeconds(.04f);
        girlSlap1.SetActive(false);
        girlSlap2.SetActive(true);
        transform.root.DOShakePosition(.5f, .3f, 10);
        StartCoroutine("waitFailed");

    }

    IEnumerator waitFailed()
    {
        yield return new WaitForSeconds(.5f);
        GameData.instance.main.gameFailed();
    }

    void girlComeAngry()
    {
        StartCoroutine("girlBlurCome");
    }

    IEnumerator girlBlurCome()
    {
        yield return new WaitForSeconds(1);
        if (girlRun == null || girlBackAngry == null)
        {
            yield break;
        }
        girlRun.SetActive(true);
        girlRun.transform.DOMoveX(girlBackAngry.transform.position.x, .7f).SetUpdate(true).SetEase(EaseType.Linear)
            .OnComplete(() => {
                girlBack.SetActive(true);
                girlRun.SetActive(false);
                StartCoroutine("girlTurnBack");
            });
    }

    IEnumerator girlTurnBack()
    {
        yield return new WaitForSeconds(1);
        girlRun.SetActive(false);
        girlBack.SetActive(false);
        girlBackAngry.SetActive(true);

        GameManager.instance.playSfx("angrydoubt");

        StartCoroutine("gameFailed");
    }

    IEnumerator gameFailed()
    {
        yield return new WaitForSeconds(1);
        StopCoroutine("loop");
        GameData.instance.main.gameFailed();
    }

    void showHeart()
    {
        GameData.instance.isLock = true;
        StopCoroutine("loop");
        bubble.SetActive(false);
        girlSearch.SetActive(false);
        girlHappy.SetActive(true);
        SpriteRenderer tsp = GameObject.Find("heart").GetComponent<SpriteRenderer>();
        tsp.enabled = true;
        tsp.transform.DOMoveY(1, 2f);

        tsp.DOFade(0, 2);
        StartCoroutine("gameWin");

        GameManager.getInstance().playSfx("giveheart");
        GameManager.getInstance().playSfx("wow");
       
    }

    IEnumerator gameWin()
    {
        yield return new WaitForSeconds(1);
       
        GameData.instance.main.gameWin();
    }

   
}
