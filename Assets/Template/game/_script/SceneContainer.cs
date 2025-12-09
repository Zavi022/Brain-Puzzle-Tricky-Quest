using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class SceneContainer : MonoBehaviour
{

    public GameObject[] mapMarks;
    public int startMarkNo;
    [HideInInspector]
    public Button btnLeft, btnRight;
    [HideInInspector]
    public int cPage;
    GameObject mainCam;
    void Start()
    {
       
        StartCoroutine("waitframe");
    }


    IEnumerator waitframe()
    {
        yield return new WaitForEndOfFrame();
        btnLeft = GameObject.Find("btnTurnLeft").GetComponent<Button>();
        btnRight = GameObject.Find("btnTurnRight").GetComponent<Button>();
        btnLeft.onClick.AddListener(clickLeft);
        btnRight.onClick.AddListener(clickRight);

        cPage = startMarkNo;
        mainCam = GameObject.Find("Main Camera");


        checkArrow();

        GameData.getInstance().isLock = true;

        mainCam.transform.DOMoveX(mapMarks[cPage].transform.position.x, .1f).OnComplete(() => { GameData.getInstance().isLock = false; });

    }

    void checkArrow()
    {
        if (mapMarks.Length == 0)
        {
            btnLeft.gameObject.SetActive(false);
            btnRight.gameObject.SetActive(false);
        }
        else
        {
            if (cPage < mapMarks.Length - 1)
            {
                btnRight.gameObject.SetActive(true);

            }
            else
            {
                btnRight.gameObject.SetActive(false);
            }

            if (cPage > 0)
            {
                btnLeft.gameObject.SetActive(true);
            }
            else
            {
                btnLeft.gameObject.SetActive(false);
            }

        }
    }

    //bool isLock = false;
    private void clickLeft()
    {
       
        if (!GameData.getInstance().isLock)
        {
            GameManager.instance.playSfx("transition");
            cPage--;
            mainCam.transform.DOMoveX(mapMarks[cPage].transform.position.x, .1f).OnComplete(()=> { 
                GameData.getInstance().isLock = false;
                gameObject.SendMessage("indexChanged", cPage,SendMessageOptions.DontRequireReceiver);
            });
            GameData.getInstance().isLock = true;
        }
        checkArrow();

    }

    private void clickRight()
    {
 
        if (!GameData.getInstance().isLock)
        {
            cPage++;
            GameManager.instance.playSfx("transition");
            mainCam.transform.DOMoveX(mapMarks[cPage].transform.position.x, .1f).OnComplete(() => { 
                GameData.getInstance().isLock = false;
                gameObject.SendMessage("indexChanged", cPage,SendMessageOptions.DontRequireReceiver);
            });
            GameData.getInstance().isLock = true;
        }
        checkArrow();
    }

    



    // Update is called once per frame
    void Update()
    {
        
    }

    public void manualScene(int index,bool keepLock = false)
    {
        cPage = index;
        mainCam.transform.DOMoveX(mapMarks[cPage].transform.position.x, .3f).OnComplete(() => {
            if(!keepLock) GameData.getInstance().isLock = false;
            
        });
        checkArrow();
    }

}
