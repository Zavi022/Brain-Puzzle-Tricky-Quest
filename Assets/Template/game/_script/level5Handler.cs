using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class level5Handler : MonoBehaviour
{

    [HideInInspector]
    public GameObject girlNormal, girlSearch, girlBack, girlangryback, girlbacktouch, girlScare, girlHappy, girlUnHappy,girlcook,
        girlslap1,girlslap2,girlstand,girlpuke,girlpraise;
    [HideInInspector]
    public GameObject cookover, table,fridge,switcher,cookBubble1,cookBubble2,cookBubble3,fire,angrymark;
    
    public GameObject fridgeOpen;





    Color[] allColor = new Color[] { new Color(116f/255f, 88f/255f, 23f/255f),new Color(240f/255f, 117f/255f, 20f/255f), 
        new Color(112f/255f,46f/255f ,142f/255f ), 
        Color.yellow,Color.green,Color.red ,new Color(119f/255f,40f/255f,54f/255f ) };
    Transform calendar;
    Transform[] dayData = new Transform[4];
    Transform[] riliNum = new Transform[4];
    int[] bgColorIndex = new int[4];
    Color[] numBgColor = new Color[4];//
    bool[] correct = new bool[4];
    SpriteRenderer numBgSp;
    void Start()
    {


        foreach (System.Reflection.MemberInfo m in typeof(level5Handler).GetMembers())
        {
            var tempVar = GameObject.Find(m.Name);
            if (tempVar != null)
            {
                this.GetType().GetField(m.Name).SetValue(this,tempVar);
            }
        }
        
        for (int i = 0; i < 4; i++)
        {
            int tColorIndex = -1;
            bool isRepeat = true;
            while (isRepeat)
            {
                isRepeat = false;
                tColorIndex = Mathf.FloorToInt(Random.Range(1, 8));
                foreach (int tIndex in bgColorIndex)
                {
                    if (tIndex == tColorIndex)
                    {
                        isRepeat = true;
                        break;
                    }
                }
            }
            bgColorIndex[i] = tColorIndex;
        }

        string tstr = "";
        for (int i = 0; i < 4; i++)
        {
            tstr += bgColorIndex[i];
        }
        print("colorIndex"+tstr);


        calendar = GameObject.Find("calendar").transform;
        numBgSp = calendar.Find("rilibg2").GetComponent<SpriteRenderer>();
        for (int i = 0; i < 4; i++)
        {
            dayData[i] = calendar.transform.Find("date" + i);
            riliNum[i] = calendar.transform.Find("rilibg2").Find("month" + i);
            //numBgColor[i] = Random.ColorHSV();
            numBgColor[i] = allColor[bgColorIndex[i]-1];
            dayData[i].gameObject.SetActive(false);
            riliNum[i].gameObject.SetActive(false);
        }
        dayData[0].gameObject.SetActive(true);
        riliNum[0].gameObject.SetActive(true);
        numBgSp.color = numBgColor[0];

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


  


    // Update is called once per frame
    void Update()
    {

    }




    bool fridgeIsOpen = false;
    int cDate;
    int nDroped = 0;
    public void useItem(string param)
    {
        if (GameData.instance.isLock) return;
        switch (param)
        {
            case "touchMe":
                GameData.instance.isLock = true;
                showHide(girlslap1, true);
                showHide(girlstand, false);
                transform.root.DOShakePosition(.5f, .3f, 10);
                StartCoroutine(Util.DelayToInvokeDo(() =>
                {
                    showHide(girlslap2, true);
                    showHide(girlslap1, false);
                    StartCoroutine("gameFailed");
                }, .1f));
                GameManager.instance.playSfx("slap");
                break;
            case "openFridges":
                fridgeOpen.SetActive(true);
                fridge.SetActive(false);
                fridgeIsOpen = true;
                GameManager.instance.playSfx("kata");
                break;
            case "closeFridge":
                fridgeOpen.SetActive(false);
                fridge.SetActive(true);
                fridgeIsOpen = false;
                GameManager.instance.playSfx("kata");
                break;
            case "openFire":
                GameData.instance.isLock = true;
                switcher.transform.eulerAngles = new Vector3(0, 0, -90f);
                showHide(fire, true);
                showHide(cookBubble1, true);

                GameManager.instance.playSfx("boil");

                StartCoroutine(Util.DelayToInvokeDo(() =>
                {
                    showHide(cookBubble2, true);
                }, 0.1f));
                StartCoroutine(Util.DelayToInvokeDo(() =>
                {
                    showHide(cookBubble3, true);
                }, 0.2f));
                StartCoroutine(Util.DelayToInvokeDo(() =>
                {
                    showHide(girlstand, false);
                    showHide(girlcook, true);

                }, 1f));

                if (nDroped != 4)
                {
                    //not enough receip or too much
                    StartCoroutine(Util.DelayToInvokeDo(() =>
                    {
                        showHide(girlcook, false) ;
                        showHide(girlbacktouch, true);
                        
                        jumpTime = .4f;
                        jumpHeight = 1;
                        StartCoroutine("jump");
                        girlbacktouch.transform.DOMove(girlbacktouch.transform.position + new Vector3(1f, 0, 0), .4f);
                        
                        //girlbacktouch.transform.DOJump(girlbacktouch.transform.position + new Vector3(1f, 0, 0), 2, 1, .4f).OnComplete
                        //(() =>
                        //{
                        //    showHide(girlbacktouch, false);
                        //    showHide(girlangryback, true);
                        //    showHide(cookover, true);
                        //    cookover.GetComponent<Animator>().SetTrigger("cookover");
                        //    showHide(angrymark,true);
                        //    StartCoroutine("gameFailed");
                        //    GameManager.instance.playSfx("exceed");
                        //    fire.SetActive(false);
                        //    GameManager.instance.playSfx("angrydoubt");
                        //});

                    }, 4f));
                }
                else//put 4 recipt,check whether all correct
                {

                    bool tAllCorrect = true;
                    foreach (bool tCorrect in correct)
                    {
                        print(tCorrect);
                        if (!tCorrect)
                        {
                            
                            StartCoroutine(Util.DelayToInvokeDo(() =>
                            {
                                showHide(girlcook, false);
                                showHide(girlpuke, true);
                                StartCoroutine("gameFailed");
                                GameManager.instance.stopMusic("boil");
                                GameManager.instance.playSfx("err");

                            }, 4f));
                            tAllCorrect = false;
                            break;
                        }
                    }

                    if (tAllCorrect)
                    {

                        StartCoroutine(Util.DelayToInvokeDo(() =>
                        {
                            showHide(girlcook, false);
                            showHide(girlpraise, true);
                            StartCoroutine("gameWin");
                            GameManager.instance.stopMusic("boil");
                            GameManager.instance.playSfx("en");
                        }, 4f));


                    }
                }
                break;
            case "touchRiLi":
                cDate++;
                if (cDate >= 4)
                {
                    cDate = 0;
                }
                for (int i = 0; i < 4; i++)
                {
                    dayData[i].gameObject.SetActive(false);
                    riliNum[i].gameObject.SetActive(false);
                }
                dayData[cDate].gameObject.SetActive(true);
                riliNum[cDate].gameObject.SetActive(true);
                numBgSp.color = numBgColor[cDate];
                GameManager.instance.playSfx("paper");
                break;
            case "dropCola":
                dropFood(1);
                break;
            case "dropCarrot":
                dropFood(2);
                break;
            case "dropQiezi":
                dropFood(3);
                break;
            case "dropBanana":
                dropFood(4);
                break;
            case "dropLajiao":
                dropFood(5);
                break;
            case "dropTomato":
                dropFood(6);
                break;
            case "dropWine":
                dropFood(7);
                break;

        }

    }
    bool jumping;
    float jumpTime;
    float jumpHeight;
    IEnumerator jump()
    {
        jumping = true;
        float timer = 0.0f;

        while (timer <= jumpTime)
        {
            float height = Mathf.Sin(timer / jumpTime * Mathf.PI) * jumpHeight;
            girlbacktouch.transform.localPosition = new Vector3(girlbacktouch.transform.localPosition.x, height, 0);
            timer += Time.deltaTime;
            yield return 0;
        }

        //catSmile.transform.localPosition = Vector3.zero;
        jumping = false;

   
        //complete event
        showHide(girlbacktouch, false);
        showHide(girlangryback, true);
        showHide(cookover, true);
        cookover.GetComponent<Animator>().SetTrigger("cookover");
        showHide(angrymark, true);
        StartCoroutine("gameFailed");
        GameManager.instance.playSfx("exceed");
        fire.SetActive(false);
        GameManager.instance.playSfx("angrydoubt");
    }


    void dropFood(int foodIndex)
    {
        for (int i = 0; i < 4; i++)
        {

            if (foodIndex == bgColorIndex[i])
            {
                correct[i] = true;
                break;
            }
        }

        //drop over 4,must be something wrong.(because only require 4)
        nDroped++;
        GameManager.instance.playSfx("putong");

        //string tstr = "";
        //for (int i = 0; i < 4; i++)
        //{
        //    tstr += correct[i];
        //}
        //print("colorIndex" + tstr);
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



    public void showHide(GameObject g,bool showOrHide)
    {
        g.GetComponent<SpriteRenderer>().enabled = showOrHide;
        Transform tshadow = g.transform.Find("shadow");
        if ( tshadow!= null)
        {
            tshadow.GetComponent<MeshRenderer>().enabled = (showOrHide);
        }
    }


}



