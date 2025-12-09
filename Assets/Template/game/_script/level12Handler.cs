using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



public class level12Handler : MonoBehaviour
{

    [HideInInspector]
    public GameObject girlNormal, girlBack, girlangryback, girlbacktouch, girlstandhappy, girlUnHappy, girlcook,
        girlslap1, girlslap2, girlstand, girlwalk, booklv12, girllookside, girlbehitup, girlcallout,girlhead,floor2, floorHeight;
    [HideInInspector]
    public GameObject angrymark, btnTurnLeft, btnTurnRight;






    void Start()
    {

        //system varaible initialzation
        foreach (System.Reflection.MemberInfo m in typeof(level12Handler).GetMembers())
        {
            var tempVar = GameObject.Find(m.Name);
            if (tempVar != null)
            {
                this.GetType().GetField(m.Name).SetValue(this, tempVar);
            }
        }

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





 



    int radarParts = 0;
    public void useItem(string param)
    {
        if (GameData.instance.isLock) return;
        switch (param)
        {
            case "touchMe":
                GameData.instance.isLock = true;
                showHide(girlslap1, true);
                showHide(girllookside, false);
                transform.root.DOShakePosition(.5f, .3f, 10);
                StartCoroutine(Util.DelayToInvokeDo(() =>
                {

                    showHide(girlslap2, true);
                    showHide(girlslap1, false);
                    StartCoroutine("gameFailed");
                }, 1f));

                break;
            case "giveBook":
                showHide(girllookside, false);
                girlstandhappy.transform.position = girllookside.transform.position;
                showHide(girlstandhappy, true);
                StartCoroutine("gameWin");
                GameManager.instance.playSfx("wow");
                break;
            case "getBook":
                if (grounded)
                {
                    bookPicked = true;
                }
                break;

        }

    }

    bool bookPicked;
    float speed = 2.0f;
    bool test;
    void Update()   
    {
        
        if (bookPicked) return;

        if (booklv12.transform.position.y < -10f)
        {
            bookPicked = true;
            GameData.instance.isLock = true;
            showHide(girllookside, false);
            showHide(girlslap1, false);
            showHide(girlslap2, false);
            girlcallout.transform.position = girllookside.transform.position;
            showHide(girlcallout, true);

            StartCoroutine("gameFailed");
        }



        girllookside.GetComponent<SpriteRenderer>().flipX = (girllookside.transform.position.x < booklv12.transform.position.x);
        if (girlhead.transform.position.y > booklv12.transform.position.y) return;
        //if (!GameData.getInstance ().startgame)
        //return;

        Vector3 dir = Vector3.zero;

        // we assume that device is held parallel to the ground
        // and Home button is in the right hand

        // remap device acceleration axis to game coordinates:
        //  1) XY plane of the device is mapped onto XZ plane
        //  2) rotated 90 degrees around Y axis
        dir.x = Input.acceleration.x;

        if (!test)
        {
           if (Mathf.Abs(dir.x) <= .1f) return;//test
        }
        
        
        //		dir.z = Input.acceleration.x;

        // clamp acceleration vector to unit sphere
        if (dir.sqrMagnitude > 1)
            dir.Normalize();
        if (test)
        {
            dir = new Vector3(1, 0, 0);//test
        }
        // Make it move 10 meters per second instead of 10 meters per frame...
        dir *= Time.deltaTime;

        // Move object
        booklv12.transform.Translate(new Vector3(dir.x * speed, 0, 0));
        booklv12.transform.Rotate(new Vector3(0, 0, dir.x));

        
    }
    bool grounded = false;
    bool hitted = false;
    void beCollided(GameObject g)
    {
        if (hitted) return;
        if (g == girllookside && girlhead.transform.position.y < booklv12.transform.position.y)
        {
            transform.root.DOShakePosition(.5f, .3f, 10);
            GameData.instance.isLock = true;
            showHide(girllookside, false);
            showHide(girlslap1, false);
            showHide(girlslap2, false);
            showHide(girlbehitup, true);
            girlbehitup.transform.position = girllookside.transform.position;
            hitted = true;
            GameManager.instance.playSfx("aiyo");
            StartCoroutine(Util.DelayToInvokeDo(() =>
            {
                showHide(girlbehitup, false);
                showHide(girlcallout, true);
                StartCoroutine("gameFailed");
                GameManager.instance.playSfx("angrydoubt");
            }, 1f));
        }
        
        if(g == floorHeight && !hitted)
        {
            
            booklv12.GetComponent<ItemInteractable>().pickable = true;
            grounded = true;
        }
    }


    IEnumerator gameFailed()
    {
        yield return new WaitForSeconds(1);
        //StopAllCoroutines();
        GameData.instance.main.gameFailed();
    }

    IEnumerator gameWin()
    {
        yield return new WaitForSeconds(1);
        //StopAllCoroutines();

        SpriteRenderer tsp = GameObject.Find("heart").GetComponent<SpriteRenderer>();
        GameManager.instance.playSfx("giveheart");
        
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



