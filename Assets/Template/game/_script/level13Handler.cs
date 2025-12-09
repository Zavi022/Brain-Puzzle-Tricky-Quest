using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



public class level13Handler : MonoBehaviour
{

    [HideInInspector]
    public GameObject girlNormal, girlback, girlangryback, girlbacktouch, girlstandhappy, girlUnHappy,
        girlslap1, girlslap2, girlstand, girlwalk,dropPos, girlFinalPos, touchStart;
    [HideInInspector]
    public GameObject angrymark, btnTurnLeft, btnTurnRight;
  


    List<Level13Items> items;
    List<PositionInfo> seats;
    void Start()
    {

        //system varaible initialzation
        foreach (System.Reflection.MemberInfo m in typeof(level13Handler).GetMembers())
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

    IEnumerator waitaframe()
    {
        yield return new WaitForEndOfFrame();

        Transform seatsT = GameObject.Find("seats").transform;
        seats = new List<PositionInfo>();
        List<PositionInfo> seatsCopy = new List<PositionInfo>();
        for(int i = 0;i<seatsT.childCount;i++)
        {
            Transform tSeat = seatsT.GetChild(i);

            PositionInfo tpos = tSeat.GetComponent<PositionInfo>();
            tpos.myIndex = i;
            seats.Add(tpos);
            seatsCopy.Add(tpos);
        }


        items = new List<Level13Items>();
        foreach (Transform tItem in GameObject.Find("itemsPut").transform)
        {
            Level13Items level13Item = tItem.GetComponent<Level13Items>();
           items.Add(level13Item);  
        }




       
        for (int i = 0; i < items.Count; i++)
        {
            bool settled = false;
            int n = 0;
            Bounds tbounds = items[i].GetComponent<SpriteRenderer>().bounds;
            while (!settled)
            {
                int tIndex = Random.Range(0, seats.Count);
                n++;
                if (n > 1000)
                {
                    break;
                }
                int cItemSize = items[i].Occupy;
                int mySeats = seats[tIndex].seats;
                if (mySeats >= cItemSize)//this position must bigger or equal to the item size
                {
                    //the rest place must bigger or equal to the item size;
                    if (mySeats - seats[tIndex].occupied >= cItemSize)
                    {
                        items[i].seatIndex = seats[tIndex].myIndex;
                        items[i].seatPos = seats[tIndex].occupied;
                        items[i].cSeatIndex = items[i].seatIndex;
                        items[i].cSeatPos = items[i].seatPos;
                        items[i].transform.position = seats[tIndex].transform.position +
                            new Vector3(seats[tIndex].occupied * .37f, 0, -.1f) +
                            new Vector3(tbounds.size.x/2f, tbounds.size.y/2f); 
                        seats[tIndex].occupied += cItemSize; 
                        
                        settled = true;
                    }
                }
            }
        }




        foreach (Transform tItem in GameObject.Find("itemsPut").transform)
        {
            tItem.gameObject.AddComponent<ItemInteractable>();
            Level13Items level13Items = tItem.GetComponent<Level13Items>();

           
            tItem.GetComponent<ItemInteractable>().pickable = true;
            tItem.GetComponent<ItemInteractable>().Icon = tItem.GetComponent<SpriteRenderer>().sprite;
            List<InteractiveTarget> interactiveTargets = new List<InteractiveTarget>();
           
            for (int i = 0; i < seats.Count; i++)
            {
                InteractiveTarget tTarget = new InteractiveTarget();
                tTarget.interactiveTarget = seats[i].gameObject;
                tTarget.methodTarget = this.transform.root.gameObject;
                tTarget.methodName = "useItem";
                //tell the seat which seat should I be so that we can judge whether it is right or not
                tTarget.param = "items"+"_"+level13Items.myIndex+"_"+i;//i代表触发区的标号
                
                tTarget.consumable = true;
                interactiveTargets.Add(tTarget);
                
            }
            tItem.GetComponent<ItemInteractable>().interactiveTargets = interactiveTargets;
        }

        //先禁止操作，过一会全部撤柜
        foreach(var tItem in items)
        {
            tItem.transform.GetComponent<BoxCollider2D>().enabled = false;
        }
        //StartCoroutine("initGame");
    }

    void initGame()
    {
        

        transform.root.DOShakePosition(.5f, .3f, 10);
      

        foreach (Level13Items item in items)
        {
            //item.transform.DOLocalRotate(new Vector3(0, 0, Random.Range(90, 360)), .3f);
            item.transform.DOMove(new Vector3(item.transform.position.x, dropPos.transform.position.y, 0),Random.Range(.4f, 1f)).SetEase(EaseType.OutBounce).SetDelay(.2f).OnComplete(() => {
                    item.GetComponent<BoxCollider2D>().enabled = true;
                });
        }
        foreach (var seat in seats)
        {
            seat.occupied = 0;
        }



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
        bool requireDetect = true;
        string[] infos;
        if(param.Length > 5)
        {
            if(param.Substring(0,6) == "items_")
            {
                requireDetect = false;
                infos = param.Split('_');
             
                int currentItemIndex = int.Parse(infos[1]);
                int currentPlaceSeatIndex = int.Parse(infos[2]);

               
                PositionInfo tSeat = seats[currentPlaceSeatIndex];
                Transform tItem = items[currentItemIndex].transform;
                tItem.gameObject.SetActive(true);
                Bounds tbounds = tItem.GetComponent<SpriteRenderer>().bounds;

                if(seats[currentPlaceSeatIndex].seats - seats[currentPlaceSeatIndex].occupied >=
                    items[currentItemIndex].Occupy)
                {
                    tItem.transform.position = seats[currentPlaceSeatIndex].transform.position +
                            new Vector3(seats[currentPlaceSeatIndex].occupied * .37f, 0, -.1f) +
                            new Vector3(tbounds.size.x / 2f, tbounds.size.y / 2f);
                    items[currentItemIndex].cSeatPos = tSeat.occupied;
                    seats[currentPlaceSeatIndex].occupied += items[currentItemIndex].Occupy;
                    items[currentItemIndex].GetComponent<BoxCollider2D>().enabled = false;//放下就不能动了
                    items[currentItemIndex].cSeatIndex = tSeat.myIndex;
                    
                    checkCorrect();
                }
                else
                {
                    items[currentItemIndex].GetComponent<ItemInteractable>().fakeClick();
                }
                
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

            case "touchStart":
                initGame();
                touchStart.SetActive(false);
                GameManager.instance.playSfx("clapse");
                break;



        }

    }


    int nPut = 0;
    void checkCorrect()
    {
        nPut++;
        if(nPut == items.Count)
        {
            changeScenePos(0);
            showHideUI(false);
            GameData.instance.isLock = true;
            showHide(girlwalk, true);
            girlwalk.transform.DOMoveX(girlFinalPos.transform.position.x, 2f).SetEase(EaseType.Linear).OnComplete(()=> {
                girlback.transform.position = girlwalk.transform.position;
                showHide(girlwalk, false);
                StartCoroutine(Util.DelayToInvokeDo(() =>
                {
                    bool wrong = false;
                    //check whether all correct
                    foreach(var item in items)
                    {
                        //print(item.cSeatPos + "_" + item.seatPos);
                        if(item.seatIndex != item.cSeatIndex || item.cSeatPos != item.seatPos)
                        {
                            wrong = true;
                            break;
                        }
                    }
                    if (wrong)
                    {
                        showHide(girlback, false);
                        girlangryback.transform.position = girlback.transform.position;
                        showHide(girlangryback, true);
                        showHide(angrymark, true);
                        angrymark.transform.position = girlFinalPos.transform.position + new Vector3(-.4f, 1.6f, 0);
                        StartCoroutine("gameFailed");
                        GameManager.instance.playSfx("angrydoubt"); 
                    }
                    else
                    {
                        showHide(girlback, false);
                        showHide(girlstandhappy, true);
                        girlstandhappy.transform.position = girlback.transform.position;
                        StartCoroutine("gameWin");
                        GameManager.instance.playSfx("wow");
                    }
                }, 1f));

            });
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
        GameManager.instance.playSfx("giveheart");
       
        tsp.transform.position = girlstandhappy.transform.position + new Vector3(0,1f,0);
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



