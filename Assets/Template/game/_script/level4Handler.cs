using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class level4Handler : MonoBehaviour
{


    bool vaseIsMoved = false;
    bool plugIsSet = true;
    bool TvIsFixed = false;
    bool girlTouched = false;
    bool sofaMoved = false;
   
    public GameObject remote;
    public GameObject plugin, plugout;
    public GameObject girlStand, girlScare, girlUnhappy, girlHappy, girlBack, girlRemote;
    public GameObject TVsnow, TVyouka,TVfbi;
    public GameObject toolusing, toolEletric, bigElectric;
    public GameObject sofa,tool;
    public GameObject girlSlap1, girlSlap2;


    private void Start()
    {
        GameManager.instance.playMusic("bgmusic1");
    }

    public void useItem(string param)
    {
        if (GameData.instance.isLock) return;
        switch (param)
        {
            case "remoteOnTv":
                if (plugIsSet)
                {
                    GameData.instance.isLock = true;
                    if (TvIsFixed)
                    {
                        TVfbi.SetActive(true);
                        StartCoroutine("waitUnhappy");
                    }
                    else
                    {
                        TVsnow.SetActive(true);
                        StartCoroutine("waitUnhappy");
                        GameManager.instance.playSfx("tvnoise");
                    }
                }
                else
                {
                    
                }
                break;
            case "touchVase":
                if (vaseIsMoved) return;
                GameData.instance.isLock = true;
                Transform vase = GameObject.Find("vase").transform;
                vase.DOMoveX(vase.position.x - .5f, .3f).SetEase(EaseType.Linear).OnComplete(() => {
                    vaseIsMoved = true; GameData.instance.isLock = false;
                });
                break;
            case "plugout":
                if (vaseIsMoved && !plugIsSet)
                {
                    plugout.SetActive(false);
                    plugin.SetActive(true);
                    plugIsSet = true;
                    GameManager.instance.playSfx("pop");
                }
                break;
            case "plugin":
                if (vaseIsMoved && plugIsSet)
                {
                    plugout.SetActive(true);
                    plugin.SetActive(false);
                    plugIsSet = false;
                    GameManager.instance.playSfx("pop");
                }
                break;
            case "touchgirl":
                 GameData.instance.isLock = true;
                if (!girlTouched)
                {
                    girlStand.SetActive(false);
                    girlSlap1.SetActive(true);
                    StartCoroutine("girlslap");
                    GameManager.instance.playSfx("slap");
                    
                }
                break;
            case "giveGirlRemote":
                GameData.instance.isLock = true;
                if (!plugIsSet)
                {
                    girlStand.SetActive(false);
                    girlRemote.SetActive(true);
                    StartCoroutine("girlTVNoSig");
                }
                else
                {
                    if (!TvIsFixed)
                    {
                        girlStand.SetActive(false);
                        girlRemote.SetActive(true);
                        StartCoroutine("girlTVBad");
                    }
                    else
                    {
                        girlStand.SetActive(false);
                        girlRemote.SetActive(true);
                        StartCoroutine("girlTVYouga");
                    }
                }
                break;
            case "touchSofa":
                if (sofaMoved) return;
                GameData.instance.isLock = true;
                GameManager.instance.playSfx("push");
                sofa.transform.DOMoveX(sofa.transform.position.x + 2f, 1f).SetEase(EaseType.Linear).OnComplete(() => {
                    //tool can not be picked up before sofa is moved.Even it is visible.
                    tool.GetComponent<BoxCollider2D>().enabled = true;
                    GameData.instance.isLock = false;
                    sofaMoved = true;
                });
                break;
            case "useTool":
                if (TvIsFixed) return;
                GameData.instance.isLock = true;
                GameManager.instance.playSfx("screw");
                if (plugIsSet)
                {
                    toolusing.SetActive(true);
                    toolEletric.SetActive(true);
                    //Sequence tseq = DOTween.Sequence();
                    //tseq.Append(tool.transform.DORotate(new Vector3(0, 0, 45),.3f));
                    //tseq.Append(tool.transform.DORotate(new Vector3(0, 0, -45), .3f));
                    toolusing.transform.DORotate(new Vector3(0, 0, toolusing.transform.localEulerAngles.z + 30), .5f).SetEase(EaseType.Linear)
                        .SetLoops(5, LoopType.Yoyo).OnComplete(() =>
                        {
                            toolusing.SetActive(false); toolEletric.SetActive(false);
                            bigElectric.SetActive(true);
                            StartCoroutine("girlScared");
                        });
                }
                else
                {
                    toolusing.SetActive(true);
                    toolusing.transform.DORotate(new Vector3(0, 0, toolusing.transform.localEulerAngles.z + 30), .5f).SetEase(EaseType.Linear)
                      .SetLoops(5, LoopType.Yoyo).OnComplete(() =>
                      {
                          toolusing.SetActive(false);
                          TvIsFixed = true;
                          GameData.instance.isLock = false;
                      });
                }
                break;
        }
    }

    IEnumerator girlslap()
    {
        yield return new WaitForEndOfFrame();
        girlSlap1.SetActive(false);
        girlSlap2.SetActive(true);
        transform.root.DOShakePosition(.5f,.3f,10);
        StartCoroutine("waitFailed");
       
    }
    IEnumerator waitFailed()
    {
        yield return new WaitForSeconds(.5f);
        GameData.instance.main.gameFailed();
    }


        IEnumerator girlTVNoSig()
    {
        yield return new WaitForSeconds(2);
        StartCoroutine("waitUnhappy");
       
    }

    IEnumerator girlTVBad()
    {
        yield return new WaitForSeconds(1);
        TVsnow.SetActive(true);
        StartCoroutine("waitUnhappy");
        GameManager.instance.playSfx("tvnoise");
       
    }

    IEnumerator girlTVYouga()
    {
        yield return new WaitForSeconds(1);
        TVyouka.SetActive(true);
        StartCoroutine("gameWin");
        GameManager.instance.playSfx("1234");
        

    }

    IEnumerator girlScared()
    {
        yield return new WaitForSeconds(1);
        girlStand.SetActive(false);
        girlScare.SetActive(true);
        GameData.instance.isLock = false;
        GameData.instance.main.gameFailed();

        GameManager.instance.playSfx("ah");
        GameManager.instance.playSfx("electric");

    }
        IEnumerator waitUnhappy()
    {
        yield return new WaitForSeconds(2);
        girlStand.SetActive(false);
        girlRemote.SetActive(false);
        girlUnhappy.SetActive(true);
        GameData.instance.isLock = false;
        GameData.instance.main.gameFailed();

        GameManager.instance.playSfx("sigh");
    }
    IEnumerator gameWin()
    {
        yield return new WaitForSeconds(2);
        girlRemote.SetActive(false);
        girlHappy.SetActive(true);
        GameManager.instance.playSfx("wow");
        SpriteRenderer tsp = GameObject.Find("heart").GetComponent<SpriteRenderer>();
        tsp.enabled = true;
        tsp.transform.DOMoveY(1, 2f);
        tsp.DOFade(0, 2);
        GameData.instance.main.gameWin();
        GameManager.instance.playSfx("giveheart");
        
    }
}
