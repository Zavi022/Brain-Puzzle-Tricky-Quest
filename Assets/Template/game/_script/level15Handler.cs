using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



public class level15Handler : MonoBehaviour
{

    [HideInInspector]
    public GameObject girlNormal, girlback, girlangryback, girlbacktouch, girlstandhappy, girlUnHappy,
        girlslap1, girlslap2, girlstand, girlUnhappy, girlwalk, windowframeIn, poles_,pole2, yanOri, yanpos;
    [HideInInspector]
    public GameObject angrymark, btnTurnLeft, btnTurnRight;


    void Start()
    {

        //system varaible initialzation
        foreach (System.Reflection.MemberInfo m in typeof(level15Handler).GetMembers())
        {
            var tempVar = GameObject.Find(m.Name);
            if (tempVar != null)
            {
                this.GetType().GetField(m.Name).SetValue(this, tempVar);
            }
        }

        StartCoroutine("waitaframe");
        GameManager.instance.playMusic("bgmusic1");
       
    }
    List<GameObject> places;
    List<GameObject> poles;
    string OrderStr = "";
    IEnumerator waitaframe()
    {
        yield return new WaitForEndOfFrame();
        places = new List<GameObject>();
        poles = new List<GameObject>();
        foreach (Transform tplace in windowframeIn.transform)
        {
            places.Add(tplace.gameObject);

        }
        int n = 0;
        foreach (Transform tpole in poles_.transform)
        {

            poles.Add(tpole.gameObject);
            ItemInteractable itemInteractive = new ItemInteractable();

            itemInteractive.interactiveTargets = new List<InteractiveTarget>();
            for (int i = 0; i < places.Count; i++)
            {
                InteractiveTarget tTarget = new InteractiveTarget();
                tTarget.interactiveTarget = places[i];
                tTarget.methodTarget = gameObject;
                tTarget.param = "putPole_" + n.ToString() + "_" + i.ToString();
                tTarget.methodName = "useItem";
                tTarget.consumable = true;
                itemInteractive.interactiveTargets.Add(tTarget);
               

            }

            tpole.gameObject.AddComponent<ItemInteractable>();
            tpole.gameObject.GetComponent<ItemInteractable>().pickable = true; 
            tpole.gameObject.GetComponent<ItemInteractable>().Icon = pole2.GetComponent<SpriteRenderer>().sprite;
            tpole.gameObject.GetComponent<ItemInteractable>().interactiveTargets = itemInteractive.interactiveTargets;
            PickUpInteractive tpickup = new PickUpInteractive();
            tpickup.methodTarget = gameObject;
            tpickup.methodName = "useItem";
            tpickup.param = "getPole_" + n;
            tpole.gameObject.GetComponent<ItemInteractable>().pickUpInteractive = tpickup;
            
            //第四根本来就是放好的。
            if (n == 3)
            {
                tpole.gameObject.GetComponent<ItemInteractable>().param = "3";
                tpole.gameObject.GetComponent<ItemInteractable>().pickable = false;

               
            }

            n++;
        }


        //init yan
        int[] arr = { 0, 1, 2, 4};
        for (int i = 0; i < arr.Length; i++)
        {
            //System.Random random = new System.Random();
            int tmp = arr[i];
            int r = Random.Range(i, arr.Length);
            arr[i] = arr[r];
            OrderStr += arr[i].ToString();
            arr[r] = tmp;
        }
        OrderStr = "3" + OrderStr;
        print(OrderStr);
        int nn = 0;
        foreach (Transform tPos in yanpos.transform)
        {
            for (int i = 0; i < arr[nn]+1; i++)
            {
                GameObject tyan = GameObject.Instantiate(yanOri, tPos.transform);
                tyan.transform.localPosition = Vector3.zero;
                tyan.transform.Translate(i * .3f, Random.Range(-.6f, .6f), 0);
            }

            tPos.transform.Translate(nn * 4f, 0, 0);
            nn++;

        }

        yanpos.transform.DOMoveX(-15, 20).SetEase(EaseType.Linear).SetLoops(-1, LoopType.Restart);
         //dropMe();
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
    // 新增：掉落后不再响应晃动
    bool hasTriggeredDrop = false;
    // Update is called once per frame
    void Update()
    {
        if (hasTriggeredDrop) return;
        Vector3 acceleration = Input.acceleration;
        lowPassValue = Vector3.Lerp(lowPassValue, acceleration, lowPassFilterFactor);
        Vector3 deltaAcceleration = acceleration - lowPassValue;

        if (deltaAcceleration.sqrMagnitude >= shakeDetectionThreshold)
        {
            // Perform your "shaking actions" here. If necessary, add suitable
            // guards in the if check above to avoid redundant handling during
            // the same shake (e.g. a minimum refractory period).
            Debug.Log("Shake event detected at time " + Time.time);
            dropMe();
        }
    }

    void dropMe()
    {
        if (hasTriggeredDrop) return;
        hasTriggeredDrop = true;

        StartCoroutine(Util.DelayToInvokeDo(() =>
        {
            GameManager.instance.playSfx("itemdrop");
        }, .2f));
        
        poles[4].GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        poles[4].GetComponent<Rigidbody2D>().AddForceAtPosition
                        (new Vector2(0, -20),
                        new Vector2(0, 1));
    }


    int[] placed = new int[5] { 0, 0, 0, 1, 0 };
    public void useItem(string param)
    {
        if (GameData.instance.isLock) return;
        bool requireDetect = true;
        string[] infos;
        if (param.Length > 8)
        {
            if (param.Substring(0, 8) == "putPole_")
            {
                requireDetect = false;
                infos = param.Split('_');
                int myIndex = int.Parse(infos[1]);
                int putIndex = int.Parse(infos[2]);


                GameObject cPole = poles[myIndex];//GameObject.Find(myIndex.ToString());
                print("当前你所放位置：" + putIndex);
                //如果这地方有放下，则拿回
                if (placed[putIndex] == 1)
                {
                    cPole.GetComponent<ItemInteractable>().fakeClick();
                    return;
                }

                //用iteminteractable的临时存储变量记录当前放在的窗格位置。
                cPole.GetComponent<ItemInteractable>().param = putIndex.ToString();
                cPole.transform.position = places[putIndex].transform.position;
                cPole.SetActive(true);
                cPole.transform.localRotation = places[putIndex].transform.localRotation;
                cPole.GetComponent<ItemInteractable>().pickable = false;
                cPole.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
                placed[putIndex] = 1;

                checkSeq(putIndex);
            }
            else
            {
                /*
                if (param.Substring(0, 8) == "getPole_")
                {

                    requireDetect = false;
                    infos = param.Split('_');
                    int myIndex = int.Parse(infos[1]);

                    string tPlacePosString = poles[myIndex].gameObject.GetComponent<ItemInteractable>().param;
                    if (tPlacePosString != null && tPlacePosString != "")
                    {
                        int cPlacedPos = int.Parse(tPlacePosString);
                        placed[cPlacedPos] = 0;
                        //print(cPlacedPos);
                    }
                }
                */
            }
        }
        if (!requireDetect) return;
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





        }

    }
    int posRemain = 4;
    string mySeq = "3";
    void checkSeq(int cPos)
    {
        //print(cPos);
        mySeq += cPos;
        posRemain--;    
        if (posRemain == 0)
        {

            print(mySeq + "__" + OrderStr);
            if (mySeq == OrderStr)
            {
                GameData.instance.isLock = true;
                print("right");
                showHide(girlback, false);
                showHide(girlstandhappy, true);
                GameManager.instance.playSfx("wow");
                StartCoroutine("gameWin");
            }
            else
            {
                print("wrong");
                GameData.instance.isLock = true;
                foreach(GameObject tpole in poles)
                {
                    tpole.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
                    tpole.GetComponent<Rigidbody2D>().AddForceAtPosition
                        (new Vector2(0, Random.Range(-200, 200)),
                        new Vector2(0, 1));
                }
                GameManager.instance.playSfx("break");
                GameManager.instance.playSfx("sigh");
                showHide(girlback, false);
                showHide(girlUnhappy, true);
                StartCoroutine("gameFailed");
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


        SpriteRenderer tsp = GameObject.Find("heart").GetComponent<SpriteRenderer>();
        tsp.transform.position = girlstandhappy.transform.position + new Vector3(0, 1f, 0);
        tsp.enabled = true;
        tsp.transform.DOMoveY(1, 2f);
        tsp.DOFade(0, 2);
        GameData.instance.main.gameWin();
        GameManager.instance.playSfx("giveheart");
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



