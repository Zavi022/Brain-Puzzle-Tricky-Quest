using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class level20Handler : MonoBehaviour
{

    [HideInInspector]
    public GameObject girlNormal, girlback, girlangryback, girlbacktouch, girlstandhappy, girlUnHappy,
        girlslap1, girlslap2, girlstand, girlwalk,nextArea,cover, girldizzy,newBG,girlcry, girlcryOver,girlhug;
    [HideInInspector]
    public GameObject angrymark, btnTurnLeft, btnTurnRight;



    Vector3 startScale;
    void Start()
    {

        //system varaible initialzation
        foreach (System.Reflection.MemberInfo m in typeof(level20Handler).GetMembers())
        {
            var tempVar = GameObject.Find(m.Name);
            if (tempVar != null)
            {
                this.GetType().GetField(m.Name).SetValue(this, tempVar);
            }
        }


        StartCoroutine("waitaframe");
        GameManager.instance.playMusic("bgcalmloop");
    }


    int girlFate = 5;
    IEnumerator waitaframe()
    {
        yield return new WaitForEndOfFrame();
        //GameManager.getInstance().playMusic("bgmusic");
        startScale = girlback.transform.localScale;
        //testGirl();
        randomGirlPos();
    }

    int cGirl = 0;
    string[] correctSounds = { "kind0", "kind1","kind2" };
    string[] badSounds = { "evil0", "evil1","evil2"};
    //void testGirl()
    //{
    //    int tRnd = (int)Random.Range(0, 3);
    //    cGirl = tRnd;
    //    girlback.transform.position = new Vector3(GameObject.Find("loc" + tRnd).transform.position.x , -.5f, 0); 
    //    cCorrect = (int)Random.Range(0, 3);
    //}
    void randomGirlPos()
    {
        int tRnd = (int)Random.Range(0, 3);
        while(cGirl == tRnd)
        {
            tRnd = (int)Random.Range(0, 3);
        }
        cGirl = tRnd;
        girlback.transform.position = new Vector3(GameObject.Find("loc" + tRnd).transform.position.x, -.5f, 0);
    }

    bool isCorrect = false;
    int cIndex = 0;
    void indexChanged(int cPos)
    {
        if (cGirl == cPos)
        {
            isCorrect = Random.Range(0, 10) < 3f ? true : false;
            if (isCorrect)
            {
                GameManager.instance.playSfx(correctSounds[(int)Random.Range(0, correctSounds.Length)]);
            }
            else
            {
                GameManager.instance.playSfx(badSounds[(int)Random.Range(0, badSounds.Length)]);
            }
        }
        cIndex = cPos;

    }
    float accelerometerUpdateInterval = 1.0f / 60.0f;
    // The greater the value of LowPassKernelWidthInSeconds, the slower the
    // filtered value will converge towards current input sample (and vice versa).
    float lowPassKernelWidthInSeconds = 1.0f;
    // This next parameter is initialized to 2.0 per Apple's recommendation,
    // or at least according to Brady! ;)
    float shakeDetectionThreshold = 2.0f;

    float lowPassFilterFactor;
    Vector3 lowPassValue;


    // Update is called once per frame
    int shakeTime = 0;
    bool endCutScene = false;
    void Update()
    {
        if (Camera.main && stopDistort) {
            Camera.main.GetComponent<FilterWave> ().WaveIntensity = distort;
            
        }

        Vector3 acceleration = Input.acceleration;
        lowPassValue = Vector3.Lerp(lowPassValue, acceleration, lowPassFilterFactor);
        Vector3 deltaAcceleration = acceleration - lowPassValue;

        if (deltaAcceleration.sqrMagnitude >= shakeDetectionThreshold)
        {
            // Perform your "shaking actions" here. If necessary, add suitable
            // guards in the if check above to avoid redundant handling during
            // the same shake (e.g. a minimum refractory period).
            Debug.Log("Shake event detected at time " + Time.time);
            shakeTime++;
        }

        if (isDizzy && shakeTime == 3 && !endCutScene)
        {
            endCutScene = true;
            StartCoroutine(Util.DelayToInvokeDo(() =>
            {
                GameData.instance.isLock = true;
                showHide(girldizzy, false);
                showHide(girlcry, true);
                girlcry.transform.position = girldizzy.transform.position;
                newBG.transform.position = GameObject.Find("loc" + cIndex).transform.position;
                newBG.GetComponent<SpriteRenderer>().DOFade(1, 1);
                StartCoroutine(Util.DelayToInvokeDo(() =>
                {
                    showHide(girlcry, false);
                    showHide(girlcryOver, true);
                    girlcryOver.transform.position = girlcry.transform.position;
                    StartCoroutine(Util.DelayToInvokeDo(() =>
                    {
                        girlhug.transform.position = girlcryOver.transform.position + new Vector3(0, -.2f, 0);
                        transform.root.DOShakePosition(.5f, .3f, 10);
                        showHide(girlcryOver, false);
                        showHide(girlhug, true);

                        GameManager.instance.playMusic("bglastwin"); 

                        StartCoroutine(Util.DelayToInvokeDo(() =>
                        {
                            girlhug.GetComponent<SpriteRenderer>().DOFade(0, 2);
                            //newBG.GetComponent<SpriteRenderer>().DOFade(0, 2).OnComplete(() => {
                               
                            //});
                            StartCoroutine("gameWin");
                        }, 2f));

                       
                    }, 2f));
                }, 1f));
                
            }, 1f));
        }
    }




    int radarParts = 0;
    int nPush = 0;
    bool isWrong = false;
    float distort,dream;
    bool stopDistort = false;
    bool isDizzy = false;
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

                break;
            case "pushMe":
                if(!isCorrect)
                {
                    isWrong = true;
                }
                nPush++;

                GameData.instance.isLock = true;
                showHideUI(false);
                girlForward();

                if(nPush == 5)
                {
                    GameData.instance.isLock = true;
                    showHideUI(false);
                    if (isWrong)//有选错门
                    {
                        StartCoroutine(Util.DelayToInvokeDo(() =>
                        {
                            GameManager.instance.playSfx("evil0");
                            StartCoroutine("gameFailed");
                        }, 2f));
                    }
                    else//没有选错门
                    {
                        StartCoroutine(Util.DelayToInvokeDo(() =>
                        {
                            stopDistort = true;
                            distort = Camera.main.GetComponent<FilterWave> ().WaveIntensity;



                            SmoothTween.To(() => dream, x => dream = x, 0f, 3f).SetEase(EaseType.Linear);
                            SmoothTween.To(() => distort, x => distort = x, 0f, 3f).SetEase(EaseType.Linear).OnComplete(() => {
                                Camera.main.GetComponent<FilterWave> ().enabled = false;

                                nextArea.GetComponent<BoxCollider2D>().enabled = true;
                                GameData.instance.isLock = false;
                            });

                        }, 2f));
                    }

                }
                
                break;
            case "nextScene":
                GameData.instance.isLock = true;
                girlback.GetComponent<BoxCollider2D>().enabled = false;
                nextArea.GetComponent<BoxCollider2D>().enabled = false;
                cover.GetComponent<SpriteRenderer>().enabled = true;
                cover.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
                cover.GetComponent<SpriteRenderer>().DOFade(1,2).OnComplete(()=> {
                    girldizzy.transform.position = GameObject.Find("loc" + cIndex).transform.position + new Vector3(0,-2f,0);
                    girldizzy.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1,0);
                    girldizzy.GetComponent<SpriteRenderer>().DOFade(1, 1).OnComplete(()=> {
                        isDizzy = true;
                        GameData.instance.isLock = false;
                    });
                    
                }); 
                break;

        }

    }
    void girlForward()
    {
        girlback.GetComponent<SpriteRenderer>().DOFade(0, .4f).OnComplete(() => {
            girlback.transform.localScale *= .6f;
            girlback.transform.Translate(0, .4f, 0);
            girlback.GetComponent<SpriteRenderer>().DOFade(1, .4f);
        });
        nFw = 0;
        StartCoroutine("girlFwRepeat");
        
    }

    int nFw = 0;
    IEnumerator girlFwRepeat()
    {
        while (nFw < 2)
        {
            yield return new WaitForSeconds(1);
            


            nFw++;

            if (nFw == 2)
            {
                girlback.GetComponent<SpriteRenderer>().DOFade(0, .4f).OnComplete(() =>
                {
                    StopCoroutine("girlFwRepeat");
                    if (!stopDistort)
                    {
                        showHideUI(true);
                        GameData.instance.isLock = false;
                        girlback.transform.position = new Vector3(0, -1000, 0);
                        girlback.GetComponent<SpriteRenderer>().color = Color.white;
                        girlback.transform.localScale = startScale;
                        randomGirlPos();
                    }
                });

            }
            else
            {
                girlback.GetComponent<SpriteRenderer>().DOFade(0, .4f).OnComplete(() =>
                {
                    girlback.transform.localScale *= .9f;
                    girlback.transform.Translate(0, .03f, 0);
                    girlback.GetComponent<SpriteRenderer>().DOFade(1, .4f);
                });
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


        //SpriteRenderer tsp = GameObject.Find("heart").GetComponent<SpriteRenderer>();
        //tsp.transform.position = girlstandhappy.transform.position + new Vector3(0,1f,0);
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



