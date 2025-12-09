using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class level2Handler : MonoBehaviour
{

    public GameObject pillow1, key, bra, ipod;
    public GameObject girlSearch, girlBack, girlBackAngry, girlScare, girlHappy, girlUnHappy,girlMusic;
    public GameObject girlSlap1, girlSlap2,girlrun;
    public GameObject heart;
    

    public GameObject[] closets;
    public GameObject catSleep,catAwake,sleepBubble,sleepZZZ,catSmile;
    public GameObject braDrop;
    
    void Start()
    {
        StartCoroutine("loop");
        //sleepBubble.transform.DOScale(1.2f, 1).SetLoops(-1,LoopType.Yoyo);
        // Simple repeating scale animation
        StartCoroutine(ScaleBubbleLoop());
        GameManager.instance.playMusic("bgmusic1");
    }


    private void FixedUpdate()
    {
        
    }



    bool dreaming = false;
    int n = 0;
    IEnumerator loop()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            if (n == 0 || n % 3 == 0)
            {
                int trnd = (int)Random.Range(0, 2);
               if(trnd == 1)
                {
                    dreaming = true;
                    sleepZZZ.SetActive(true);
                }
                else
                {
                    dreaming = false;
                    sleepZZZ.SetActive(false);
                }
                
            }
            n++;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
 

    bool[] closetOpen = new bool[3];
    bool catTouched = false;
    bool meTouched = false;
    bool pillow1Moved = false;
    bool keyGetted = false;
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
                   
                    girlSlap1.SetActive(true);
                    StartCoroutine("girlslap");
                    GameManager.instance.playSfx("slap"); 
                }
                break;
           
          
            case "movePillow1":
                if (!pillow1Moved)
                {
                    GameData.instance.isLock = true;
                    pillow1Moved = true;
                    bra.SetActive(true);
                   
                    if(!keyGetted)key.SetActive(true);
                    pillow1.transform.DOMoveX(pillow1.transform.position.x + 1f, .4f).OnComplete(() =>
                    { GameData.instance.isLock = false; });
                }
                else
                {
                    GameData.instance.isLock = true;
                    pillow1Moved = false;
                    
                    pillow1.transform.DOMoveX(pillow1.transform.position.x - 1f, .4f).OnComplete(() =>
                    { GameData.instance.isLock = false;
                        bra.SetActive(false);
                        key.SetActive(false);
                    });
                }
                break;
         
            case "touchCat":
                GameData.instance.isLock = true;
                catAwaken();
                break;
            case "touchCloset1":
                if (!closets[0].activeSelf)
                {
                    GameManager.instance.playSfx("openCloset"); 
                }
                for (int i = 0; i < closets.Length; i++)
                {
                    closets[i].SetActive(false);
                    closetOpen[i] = false;
                }
                closetOpen[0] = true;
                closets[0].SetActive(true);
 
                break;
            case "touchCloset2":
                if (!closets[1].activeSelf)
                {
                    GameManager.instance.playSfx("openCloset");
                }
                for (int i = 0; i < closets.Length; i++)
                {
                    closets[i].SetActive(false);
                    closetOpen[i] = false;
                }
                closetOpen[1] = true;
                closets[1].SetActive(true);
                
                break;
            case "touchCloset3":
                GameData.instance.isLock = true;
                closets[0].transform.parent.DOShakeRotation(.3f, 30, 100).OnComplete(
                    ()=> {
                        catAwaken();
                    });
                GameManager.instance.playSfx("openfail");
                break;
            case "openDrawer":
                if (!closets[2].activeSelf)
                {
                    GameManager.instance.playSfx("openCloset");
                }
                for (int i = 0; i < closets.Length; i++)
                {
                    closets[i].SetActive(false);
                    closetOpen[i] = false;

                }
                closets[2].SetActive(true);
                closetOpen[2] = true;
                
                break;
            case "getKey":
                keyGetted = true;
                break;
            case "touchBra":
                GameData.instance.isLock = true;
                girlComeAngry();
                break;
            case "touchIpod":
                if (!dreaming)
                {
                    GameData.instance.isLock = true;
                    catAwaken();
                }
                break;
            case "giveIpod":
                girlSearch.SetActive(false);
                girlMusic.SetActive(true);
                GameData.instance.isLock = true;
                if (pillow1Moved)
                {
                    StartCoroutine("catStartJump");
                }
                else
                {
                    StartCoroutine("musicWin"); 
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


    void catAwaken()
    {
        GameManager.instance.playSfx("catunhappy");
        transform.root.DOShakePosition(.5f, .3f, 10);

        catSleep.SetActive(false);
        catAwake.SetActive(true);
        sleepBubble.SetActive(false);
        sleepZZZ.SetActive(false);
        StopCoroutine("loop");
        StartCoroutine("gameFailed");
    }

    IEnumerator catStartJump()
    {
        yield return new WaitForSeconds(1);
        catSmile.SetActive(true);
        GameManager.instance.playSfx("cathappy");
        jumpHeight = 3;jumpTime = .6f;
        StartCoroutine("catJump");
        catSmile.transform.DOMove(braDrop.transform.position + new Vector3(0, .5f, 0), .6f);
        //catSmile.transform.DOJump(braDrop.transform.position + new Vector3(0, .5f, 0), 3, 1, .6f).OnComplete(() => {
        //    StartCoroutine("catJumpOver");
        //    GameManager.instance.playSfx("sigh");
        //});
    }
    bool jumping;
    float jumpTime;
    float jumpHeight;
    IEnumerator catJump()
    {
        jumping = true;
        float timer = 0.0f;

        while (timer <= jumpTime)
        {
            float height = Mathf.Sin(timer / jumpTime * Mathf.PI) * jumpHeight;
            catSmile.transform.localPosition = new Vector3(catSmile.transform.localPosition.x, height, 0);
            timer += Time.deltaTime;
            yield return 0;
        }

        //catSmile.transform.localPosition = Vector3.zero;
        jumping = false;

        //complete event
        StartCoroutine("catJumpOver");
    }

    IEnumerator catJumpOver()
    {
        yield return new WaitForSeconds(.5f);
        catSmile.transform.GetChild(0).gameObject.SetActive(false);
        braDrop.SetActive(true);
        braDrop.transform.DOMoveY(braDrop.transform.position.y - .8f, .3f).OnComplete(
            ()=> {
                girlMusic.SetActive(false);
                girlUnHappy.SetActive(true);
                StartCoroutine("gameFailed");
            }
            );

    }

    IEnumerator musicWin()
    {
        yield return new WaitForSeconds(1f);
        girlMusic.SetActive(false);
        girlHappy.SetActive(true);
        StartCoroutine("gameWin");
        GameManager.getInstance().playSfx("wow");
    }

        void girlComeAngry()
    {
        girlSearch.SetActive(false);
        StartCoroutine("girlBlurCome");
    }

    IEnumerator girlBlurCome()
    {
        yield return new WaitForSeconds(1);
        girlrun.SetActive(true);
        girlrun.transform.DOMoveX(girlBackAngry.transform.position.x, .4f).SetUpdate(true).SetEase(EaseType.Linear)
            .OnComplete(() => {
                girlrun.SetActive(false);
                girlBack.SetActive(true);
                StartCoroutine("girlTurnBack");
            });
    }

    IEnumerator girlTurnBack()
    {
        yield return new WaitForSeconds(1);
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

    IEnumerator gameWin()
    {
        yield return new WaitForSeconds(1);
        StopCoroutine("loop");
      
        girlSearch.SetActive(false);
        girlHappy.SetActive(true);
        SpriteRenderer tsp = GameObject.Find("heart").GetComponent<SpriteRenderer>();
        tsp.enabled = true;
        tsp.transform.DOMoveY(1, 2f);
        tsp.DOFade(0, 2);
        GameData.instance.main.gameWin();

        GameManager.getInstance().playSfx("giveheart");
    }

    IEnumerator ScaleBubbleLoop()
    {
        while (sleepBubble != null && sleepBubble.activeInHierarchy)
        {
            sleepBubble.transform.DOScale(Vector3.one * 1.2f, 1f).SetEase(EaseType.InOutSine).OnComplete(() => {
                if (sleepBubble != null && sleepBubble.activeInHierarchy)
                    sleepBubble.transform.DOScale(Vector3.one, 1f).SetEase(EaseType.InOutSine);
            });
            yield return new WaitForSeconds(2f);
        }
    }

   
}
