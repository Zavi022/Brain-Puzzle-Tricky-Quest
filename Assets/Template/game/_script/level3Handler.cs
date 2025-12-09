using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class level3Handler : MonoBehaviour
{

    public GameObject bird;
    public GameObject worm;
    public GameObject[] windows;
    public GameObject window;
    public GameObject girlNormal, girlSearch, girlBack, girlBackAngry, girlScare, girlHappy, girlUnHappy;
    public GameObject heart;
    public GameObject girlSlap1, girlSlap2;
    public GameObject tree;
    public GameObject key;
    GameObject paths;
    Animator birdAnim;
    public float wormSpeed = 1f; // 单位/秒
    public float wormTurnSpeed = 360f; // 每秒转向角速度（度）



    void Start()
    {
        //StartCoroutine("loop");
        //sleepBubble.transform.DOScale(1.2f, 1).SetLoops(-1,LoopType.Yoyo);
        birdAnim = bird.GetComponent<Animator>();
        paths = GameObject.Find("paths");
        StartCoroutine("wormMove");
        GameManager.instance.playMusic("bgmusic1");
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


    int currentNode;
    IEnumerator wormMove()
    {
        yield return new WaitForSeconds(10);
        worm.SetActive(true);
        List<Vector3> locations = new List<Vector3>();
        for (int i = 0; i < paths.transform.childCount; i++)
        {
            var path = paths.transform.GetChild(i);
            locations.Add(path.position);
        }

        // 从当前点开始依次移动到每个目标点
        for (int i = 0; i < locations.Count; i++)
        {
            Vector3 startPos = worm.transform.position;
            Vector3 targetPos = locations[i];
            float distance = Vector3.Distance(startPos, targetPos);
            if (distance <= 0.0001f)
            {
                continue;
            }

            // 目标朝向角
            Vector3 dir = (targetPos - startPos).normalized;
            float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            float duration = distance / Mathf.Max(0.0001f, wormSpeed);
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                worm.transform.position = Vector3.Lerp(startPos, targetPos, t);
                // 平滑转向
                float currentZ = worm.transform.eulerAngles.z;
                float newZ = Mathf.MoveTowardsAngle(currentZ, targetAngle, wormTurnSpeed * Time.deltaTime);
                worm.transform.rotation = Quaternion.Euler(0f, 0f, newZ);
                yield return null;
            }
            // 纠正到终点
            worm.transform.position = targetPos;
            worm.transform.rotation = Quaternion.Euler(0f, 0f, targetAngle);
        }
        window.GetComponent<BoxCollider2D>().enabled = false;
        if (!isWindowOpen)
        {
            worm.GetComponent<BoxCollider2D>().enabled = false;
        }

    }


    void wormArrived()
    {
        worm.SetActive(false);
    }


    // Update is called once per frame
    void Update()
    {
        
    }


    bool isWindowOpen = false;
    bool windowUnlock = false;

    bool meTouched = false;
    int nShake = 0;
    public void useItem(string param)
    {
        if (GameData.instance.isLock) return;
        switch (param)
        {
            case "touchMe":
                GameData.instance.isLock = true;
                if (!meTouched)
                {
                    meTouched = true;

                    girlNormal.SetActive(false);
                    StopCoroutine("loop");
                   
                    girlSlap1.SetActive(true);
                    StartCoroutine("girlslap");
                    GameManager.instance.playSfx("slap");
                }
                break;
            case "touchTree":
                if (nShake < 5)
                {
                    GameData.instance.isLock = true;
                    tree.transform.DOShakeRotation(.3f, 10, 100).OnComplete(() => {
                        GameData.instance.isLock = false;
                    });
                    nShake++;
                    if (nShake == 5)
                    {
                        GameData.instance.isLock = true;
                        key.SetActive(true);
                        key.GetComponent<Rigidbody2D>().AddForce(new Vector2(120, 0));
                        StartCoroutine(Util.DelayToInvokeDo(() =>
                        {
                            GameManager.instance.playSfx("coindrop");
                        }, .5f));
                    }

                    GameManager.instance.playSfx("grass");
                }
                break;
            case "touchWindow":
                if (windowUnlock)
                {
                    if (!isWindowOpen)
                    {
                        windows[2].SetActive(true);
                        windows[3].SetActive(true);
                        windows[1].SetActive(false);
                        windows[0].SetActive(false);
                        isWindowOpen = true;
                    }
                    else
                    {
                        windows[2].SetActive(false);
                        windows[3].SetActive(false);
                        windows[1].SetActive(true);
                        windows[0].SetActive(true);
                        isWindowOpen = false;
                    }
                    GameManager.instance.playSfx("kata");
                }
                else
                {
                    GameManager.instance.playSfx("openfail"); 
                }
                break;
            case "touchBird":
                Sequence tseq = new Sequence();
                GameData.instance.isLock = true;
                if (!isWindowOpen)
                {
                    birdAnim.SetTrigger("fly");
                    GameManager.instance.playSfx("wing");
                    GameManager.instance.playSfx("ei");
                    bird.GetComponent<SpriteRenderer>().flipX = true;
                    tseq = new Sequence();
                    tseq.Append(bird.transform.DOMoveY(bird.transform.position.y + .4f, .3f));
                    tseq.Append(bird.transform.DOMoveX(girlNormal.transform.position.x + .5f, .3f).OnComplete(
                        () =>
                        {

                            bird.GetComponent<SpriteRenderer>().flipX = false;
                            girlNormal.SetActive(false);
                            girlScare.SetActive(true);
                            StartCoroutine("gameFailed");

                            
                        }));
                    tseq.Append(bird.transform.DOMoveX(girlNormal.transform.position.x + 5, 2f));
                }
                else
                {
                    birdEscape();
                }
                break;
            case "unlockWindow":
                windowUnlock = true;
                GameManager.instance.playSfx("unlock");
                break;
            case "wormPicked":
                window.GetComponent<BoxCollider2D>().enabled = true;
                break;
            case "wormArrive":
                window.GetComponent<BoxCollider2D>().enabled = true;
                break;
            case "feedbad":
                GameData.instance.isLock = true;
                birdAnim.SetTrigger("no");
                StartCoroutine("gameFailed");

                GameManager.instance.playSfx("wrong");
                break;
            case "feedGood":
                GameData.instance.isLock = true;
                birdAnim.SetTrigger("eat");
                if (isWindowOpen)
                {
                    StartCoroutine("birdEscapeWait"); 
                }
                else
                {
                    StartCoroutine("gameWin");
                    girlNormal.SetActive(false);
                    girlSearch.SetActive(false);
                    girlHappy.SetActive(true);
                    GameManager.instance.playSfx("wow");
                }
                break;
            case "girlWorm":
                GameData.instance.isLock = true;
                if (!meTouched)
                {
                    meTouched = true;
                    girlScare.SetActive(true);
                    girlSearch.SetActive(false);
                    girlNormal.SetActive(false);
                    StopCoroutine("loop");
                
                    StartCoroutine("gameFailed");
                    GameManager.instance.playSfx("ah");
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

    IEnumerator birdEscapeWait()
    {
        yield return new WaitForSeconds(2);
        birdEscape();
    }

    void birdEscape()
    {
        Sequence tseq = new Sequence();
        birdAnim.SetTrigger("fly");
        bird.GetComponent<SpriteRenderer>().flipX = true;
        tseq = new Sequence();
        tseq.Append(bird.transform.DOMove(window.transform.position + new Vector3(.2f, .2f, 0), 1f).OnComplete(
            () => {
                girlNormal.SetActive(false);
                girlBack.SetActive(true);
            }));
        //tseq.Append(bird.transform.DOScale(.1f, 1));
        bird.transform.DOScale(Vector3.one, .1f);
        tseq.Play().OnComplete(() =>
        {
            bird.SetActive(false);
            StartCoroutine("girlComeAngry");
        });

        GameManager.instance.playSfx("wing");
    }

    void girlComeAngry()
    {
        girlBack.SetActive(false);
        girlNormal.SetActive(false);
        girlBackAngry.SetActive(true);
        StartCoroutine("gameFailed");

        GameManager.instance.playSfx("angrydoubt");
    }

    IEnumerator girlBlurCome()
    {
        yield return new WaitForSeconds(1);
        girlBack.SetActive(true);
        girlBack.transform.DOMoveX(girlBackAngry.transform.position.x, .4f).SetUpdate(true).SetEase(EaseType.Linear)
            .OnComplete(() => {
                girlBack.SetActive(true);
                StartCoroutine("girlTurnBack");
            });
    }

    IEnumerator girlTurnBack()
    {
        yield return new WaitForSeconds(1);
        girlBack.SetActive(false);
        girlBackAngry.SetActive(true);
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
        
        SpriteRenderer tsp = GameObject.Find("heart").GetComponent<SpriteRenderer>();
        tsp.enabled = true;
        tsp.transform.DOMoveY(1, 2f);
        tsp.DOFade(0, 2);
        GameData.instance.main.gameWin();
        GameManager.instance.playSfx("giveheart");


        

    }

    



}
