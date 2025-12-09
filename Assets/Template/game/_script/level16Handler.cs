using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



public class level16Handler : MonoBehaviour
{

    [HideInInspector]
    public GameObject girlNormal, girlback, girlangryback, girlbacktouch, girlstandhappy, girlUnHappy,
        girlslap1, girlslap2, girlstand, girlwalk, girlblind, girlslapshelf1, girlslapshelf2, girlkillmos,
        girlrelease, girleyeL, girleyeR, eyeSocketL, eyeSocketR, mosquitoe, bloodstain,
        minuteNeedle, hourNeedle, secondNeedle, mosCenter;
    [HideInInspector]
    public GameObject angrymark, btnTurnLeft, btnTurnRight;



    void Start()
    {

        //system varaible initialzation
        foreach (System.Reflection.MemberInfo m in typeof(level16Handler).GetMembers())
        {
            var tempVar = GameObject.Find(m.Name);
            if (tempVar != null)
            {
                this.GetType().GetField(m.Name).SetValue(this, tempVar);
            }
        }

        GameManager.instance.playMusic("bgmusic1");
        StartCoroutine("waitaframe");

    }

    IEnumerator waitaframe()
    {
        yield return new WaitForEndOfFrame();
        int hourAngle = (int)Random.Range(0, 360f);
        int minuteAngle = (int)Random.Range(0, 360f);
        int secondAngle = (int)Random.Range(0, 360f);
        hourNeedle.transform.localEulerAngles = new Vector3(0, 0, hourAngle);
        minuteNeedle.transform.localEulerAngles = new Vector3(0, 0, minuteAngle);
        secondNeedle.transform.localEulerAngles = new Vector3(0, 0, secondAngle);

        //stuck = true;
    }


    bool stuck;

    // Update is called once per frame

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
    void Update()
    {
        mosCenter.transform.localEulerAngles = new Vector3(0, 0, mosCenter.transform.localEulerAngles.z - 20f*Time.deltaTime); 
        mosquitoe.GetComponent<SpriteRenderer>().flipX = Mathf.Sign(mosquitoe.transform.position.x - girlblind.transform.position.x) == 1 ? true : false;
        if (mosquitoe.transform.parent != null)
        {
            mosquitoe.transform.localEulerAngles = -mosCenter.transform.localEulerAngles;
        }
        lookIt(girleyeL, eyeSocketL, mosquitoe);
        lookIt(girleyeR, eyeSocketR, mosquitoe);

        if (!stuck)
        {
            float nextSAngle = secondNeedle.transform.localEulerAngles.z - 1f;
            //print(nextSAngle);
            
            secondNeedle.transform.localEulerAngles = new Vector3(0, 0, nextSAngle);
            if((int)nextSAngle == 90f)
            {
                float nextMAngle = minuteNeedle.transform.localEulerAngles.z - 6f;
                minuteNeedle.transform.localEulerAngles = new Vector3(0, 0, nextMAngle);
                if((int)nextMAngle == 90f)
                {
                    float nextHAngle = hourNeedle.transform.localEulerAngles.z - 6f;
                    hourNeedle.transform.localEulerAngles = new Vector3(0, 0, nextHAngle);
                }
            }
            //if(secondNeedle.transform.local)
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
            stopClock();
        }
    }


    void stopClock()
    {
        stuck = true;
    }

    void lookIt(GameObject eye,GameObject eyeSocket,GameObject target)
    {
        float cos = 0.000000f;
        float arg = 0.000000f;
        cos = Mathf.Abs(eyeSocket.transform.position.x - target.transform.position.x) /
           Vector3.Distance(eyeSocket.transform.position, target.transform.position);
        arg = Mathf.Acos(cos);

        eye.transform.position = eyeSocket.transform.position;
        float xSign = Mathf.Sign(target.transform.position.x - eyeSocket.transform.position.x);
        float ySign = Mathf.Sign(target.transform.position.y - eyeSocket.transform.position.y);
        float toffsetX = xSign * Mathf.Abs(Mathf.Cos(arg) * .026f);
        float toffsetY = ySign * Mathf.Abs(Mathf.Sin(arg) * .026f);
        eye.transform.Translate(toffsetX, toffsetY, 0f);
    }

    int radarParts = 0;
    public void useItem(string param)
    {
        if (GameData.instance.isLock) return;
       
        switch (param)
        {
            case "touchMe":
                GameData.instance.isLock = true;
                showHide(girlblind, false);
                showHide(girlslap1, true);

                transform.root.DOShakePosition(.5f, .3f, 10);
                StartCoroutine(Util.DelayToInvokeDo(() =>
                {
                    showHide(girlslap2, true);
                    showHide(girlslap1, false);
                    StartCoroutine("gameFailed");
                }, .1f));
                GameManager.instance.playSfx("slap");
                GameManager.instance.stopMusic("mosquito");
                mosquitoe.GetComponent<Mosquito>().stopFly();
                break;
            case "slapMos":
                GameData.instance.isLock = true;
                showHideUI(false);


                bool canKill = checkMos();
                if (canKill)
                {
                    GameManager.instance.playSfx("slap");
                    GameManager.instance.stopMusic("mosquito");
                    mosquitoe.GetComponent<Mosquito>().stopFly();
                    mosquitoe.SetActive(false);
                    mosquitoe.transform.parent = null;
                    StartCoroutine(Util.DelayToInvokeDo(() =>
                    {
                        showHide(girlblind, false);
                        showHide(girlrelease, true);
                        StartCoroutine("gameWin");
                        GameManager.instance.playSfx("release");
                    }, 1f));
                       return;
                }
                mosquitoe.transform.parent = null;
                mosquitoe.transform.localEulerAngles = Vector3.zero;
                mosquitoe.transform.DOMove((eyeSocketL.transform.position + eyeSocketR.transform.position) / 2f,2);
                StartCoroutine(Util.DelayToInvokeDo(()=>{
                    showHide(girlblind, false);
                    eyeSocketL.SetActive(false);
                    eyeSocketR.SetActive(false);
                    showHide(girleyeL, false);
                    showHide(girleyeR, false);
                    girlslapshelf1.transform.position = girlblind.transform.position;
                    StartCoroutine(Util.DelayToInvokeDo(() =>
                    {
                        showHide(girlslapshelf1, false);

                        GameManager.instance.playSfx("slap");
                        GameManager.instance.stopMusic("mosquito");
                        mosquitoe.GetComponent<Mosquito>().stopFly();
                        mosquitoe.SetActive(false);

                        girlslapshelf2.transform.position = girlslapshelf1.transform.position;
                        StartCoroutine(Util.DelayToInvokeDo(() =>
                        {
                            showHide(girlslapshelf2, false);
                            girlkillmos.transform.position = girlslapshelf2.transform.position;
                            StartCoroutine("gameFailed");
                            GameManager.instance.playSfx("ah");
                        }, 1f));

                        
                    }, .1f));
                },1.5f));

                break;





        }

    }

    bool checkMos()
    {
        bool _cankill = false;

        float mosAngle = mosCenter.transform.localEulerAngles.z;
        float needleAngle = secondNeedle.transform.localEulerAngles.z;
        //print(mosAngle + "___" + needleAngle);

        if(stuck && Mathf.Abs(mosAngle - needleAngle) < 15f)
        {
            _cankill = true;
        }

        return _cankill;
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



