using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



public class level11Handler : MonoBehaviour
{

    [HideInInspector]
    public GameObject girlNormal, girlBack, girlangryback, girlbacktouch, girlstandhappy, girlUnHappy, girlcook,
        girlslap1, girlslap2, girlstand, girlwalk,girlScareDown,girlscare, girltrample1, girltrample2, girlcallout;
    [HideInInspector]
    public GameObject angrymark,tree1,tree2,Insecticide,insectAnim,smoke, insectSpray,
        spider1,spider2,spider3,spider4,footpos,btnTurnLeft,btnTurnRight, bloodstain,bloodstains;




    List<Vector3> locations1 = new List<Vector3>();
    List<Vector3> locations2 = new List<Vector3>();
    List<Vector3> locations3 = new List<Vector3>();
    int currentnode1,currentnode2,currentnode3;
    void Start()
    {

        //system varaible initialzation
        foreach (System.Reflection.MemberInfo m in typeof(level11Handler).GetMembers())
        {
            var tempVar = GameObject.Find(m.Name);
            if (tempVar != null)
            {
                this.GetType().GetField(m.Name).SetValue(this, tempVar);
            }
        }
        
        // Initialize tween variables
        tween1 = null;
        tween2 = null;
        tween3 = null;
        
        // Start path movement only if spiders are valid
        if (spider1 != null && spider1.activeInHierarchy)
            tween1 = PlayPath(spider1,locations1, currentnode1,"path1");
        if (spider2 != null && spider2.activeInHierarchy)
            tween2 = PlayPath(spider2, locations2, currentnode2,"path2");
        if (spider3 != null && spider3.activeInHierarchy)
            tween3 = PlayPath(spider3, locations3, currentnode3, "path3");
            
        GameManager.instance.playMusic("bgmusic1");

    }

    TweenHandle PlayPath(GameObject target, List<Vector3> locations,int currentNode,string _path)
    {

        GameObject cpath = GameObject.Find(_path);
        float dis;

        locations.Add(target.transform.position);
        for (int i = 0; i < cpath.transform.childCount; i++)
        {
            var path = cpath.transform.GetChild(i);
            locations.Add(path.position);
        }
        locations.Add(locations[0]);
        
        // Start the path movement
        return StartPathMovement(target, locations, 0, _path);

    }

    private TweenHandle StartPathMovement(GameObject target, List<Vector3> locations, int startIndex, string _path)
    {
        // Check if spider is still alive and touchable before starting any movement
        if (target == null || !target.activeInHierarchy)
            return null;
            
        // Check if this specific spider is still touchable (not killed)
        if ((target == spider1 && !spider1Touchable) ||
            (target == spider2 && !spider2Touchable) ||
            (target == spider3 && !spider3Touchable))
        {
            return null;
        }
        
        if (startIndex >= locations.Count - 1)
        {
            // Path completed, restart only if spider is still alive and touchable
            if (target != null && target.activeInHierarchy)
            {
                return StartPathMovement(target, locations, 0, _path);
            }
            return null;
        }

        float dis = Vector2.Distance(locations[startIndex], locations[startIndex + 1]);
        Vector3 diff = locations[startIndex + 1] - locations[startIndex];
        float angle = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        
        if (startIndex == 0)
            target.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        var tween = target.transform.DOMove(locations[startIndex + 1], dis).SetEase(EaseType.Linear).OnComplete(() => {
            // Check if target still exists and is active
            if (target == null || !target.activeInHierarchy)
                return;

            // Stop continuing if this spider is no longer touchable or already on ground
            if ((target == spider1 && (!spider1Touchable || spider1OnGround)) ||
                (target == spider2 && (!spider2Touchable || spider2OnGround)) ||
                (target == spider3 && (!spider3Touchable || spider3OnGround)))
            {
                return;
            }
            
            // Rotate to next direction
            if (startIndex + 1 < locations.Count - 1)
            {
                Vector3 nextDiff = locations[startIndex + 2] - locations[startIndex + 1];
                float nextAngle = Mathf.Atan2(nextDiff.y, nextDiff.x) * Mathf.Rad2Deg;
                target.transform.rotation = Quaternion.AngleAxis(nextAngle, Vector3.forward);
            }
            
            // Continue to next point and update tween handle reference
            var nextHandle = StartPathMovement(target, locations, startIndex + 1, _path);
            if (target == spider1) tween1 = nextHandle;
            else if (target == spider2) tween2 = nextHandle;
            else if (target == spider3) tween3 = nextHandle;
        });

        // Save current tween handle so it can be killed immediately when needed
        if (target == spider1) tween1 = tween;
        else if (target == spider2) tween2 = tween;
        else if (target == spider3) tween3 = tween;

        return tween;
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
     if(currentnode1 == -1 && spider1 != null && spider1.activeInHierarchy)
        {
            rerotate(spider1);
            print("spider1");
        }
     if(currentnode2 == -1 && spider2 != null && spider2.activeInHierarchy)
        {
            rerotate(spider2);

        }
     if(currentnode3 == -1 && spider3 != null && spider3.activeInHierarchy)
        {
            rerotate(spider3);
            
        }
    }




    int radarParts = 0;
    bool tree1Moved;
    bool spider1Touchable=true, spider2Touchable=true, spider3Touchable=true;
    bool spider1OnGround, spider2OnGround, spider3OnGround;
    public void useItem(string param)
    {
        if (GameData.instance.isLock) return;
        switch (param)
        {
            case "touchMe":
                GameData.instance.isLock = true;
                showHide(girlslap1, true);
                showHide(girlScareDown, false);
                transform.root.DOShakePosition(.5f, .3f, 10);
                StartCoroutine(Util.DelayToInvokeDo(() =>
                {
                    showHide(girlslap2, true);
                    showHide(girlslap1, false);
                    StartCoroutine("gameFailed");
                }, .1f));
                GameManager.instance.playSfx("slap");
                break;
            case "moveTree1":
                
                if (tree1Moved) return;
                tree1.transform.DOMoveX(tree1.transform.position.x - 1,1f);
                tree1Moved = true;
                Insecticide.GetComponent<BoxCollider2D>().enabled = true;
                break;
            case "moveTree2":
                GameData.instance.isLock = true;
                tree2.transform.Translate(-1f, -.3f, 0);
                tree2.transform.DORotate(new Vector3(0, 0, 80), 1f).SetEase(EaseType.OutBounce);
                showHide(spider4, true);
                //spider4.transform.DOScale(3, .3f);
                spider4.transform.DOScale(Vector3.one*3, .3f);

                GameManager.instance.stopBGMusic();
                GameManager.instance.playSfx("break");

                jumpHeight = 3; jumpTime = .3f;
                StartCoroutine("jump");
                spider4.transform.DOMove(new Vector3(spider4.transform.position.x, 0, 0), .3f);
                //spider4.transform.DOJump(new Vector3(spider4.transform.position.x, 0, 0),2,1, .3f).OnComplete(
                //    () => {
                //        if (tween1 != null) tween1.Kill();
                //        if (tween2 != null) tween2.Kill();
                //        if (tween3 != null) tween3.Kill();
                //        spider4.transform.DOMoveY(-10, 100f);
                //    }
                //    );






                transform.root.DOShakePosition(.5f, .5f, 10);
                StartCoroutine("gameFailed");
                break;
            case "killSpider1":
                if (!spider1OnGround)
                {
                    if (!spider1Touchable) return;
                    killSpider(spider1);
                    
                }
                break;
            case "smashSpider1":
                if (spider1OnGround)
                {
                    smashSpider(spider1);
                    GameManager.instance.playSfx("cai");
                }
                break;

            case "killSpider2":
                if (!spider2OnGround)
                {
                    if (!spider2Touchable) return;
                    killSpider(spider2);

                }
                break;
            case "smashSpider2":
                if (spider2OnGround)
                {
                    smashSpider(spider2);

                }
                break;

            case "killSpider3":
                if (!spider3OnGround)
                {
                    if (!spider3Touchable) return;
                    killSpider(spider3);

                }
                break;
            case "smashSpider3":
                if (spider3OnGround)
                {
                    smashSpider(spider3);

                }
                break;
            case "spraygirl":
                if (tween1 != null)
                {
                    tween1.Kill();
                }
                if (tween2 != null)
                {
                    tween2.Kill();
                }
                if (tween3 != null)
                {
                    tween3.Kill();
                   
                }
                GameData.instance.isLock = true;
                showSpray(girlScareDown);
                GameManager.instance.playSfx("zi");
                GameManager.instance.playSfx("ei");


                showHide(girlScareDown,false);
                showHide(girlscare, true);
                showHide(girltrample1, false);
                showHide(girltrample2, false);
                StopCoroutine("trampleSpide");
                StartCoroutine(Util.DelayToInvokeDo(() =>
                {
                    showHide(girlscare, false);
                    showHide(girlcallout, true);
                    StartCoroutine("gameFailed");
                    GameManager.instance.playSfx("angrydoubt");

                }, 1f));
                    break;


        }

    }

    void rerotate(GameObject target)
    {
        // Don't rerotate if spider is dead or on ground
        if (target == null || !target.activeInHierarchy)
            return;
            
        var dis = Vector2.Distance(target.transform.position, footpos.transform.position);
        var diff = footpos.transform.position - target.transform.position;
        var angle = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        target.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
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
            spider4.transform.localPosition = new Vector3(spider4.transform.localPosition.x, height, 0);
            timer += Time.deltaTime;
            yield return 0;
        }

        //catSmile.transform.localPosition = Vector3.zero;
        jumping = false;

        //complete event
        SmoothTween.KillAll();
        spider4.transform.DOMoveY(-10, 100f);


    }

        TweenHandle tween1, tween2, tween3;
    GameObject cDeadSpider;
    void killSpider(GameObject spider)
    {
        showSpray(spider);
        GameManager.instance.playSfx("zi");

        // Stop spider movement but keep collider enabled for clicking
        var rb = spider.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.Sleep();
            rb.velocity = Vector2.zero; // Stop any remaining velocity
        }

        spider.transform.localEulerAngles = Vector3.zero - new Vector3(0, 0, 90f);

        // Stop only the specific spider's path animation
        switch (spider.name)
        {
            case "spider1":
                if (tween1 != null)
                {
                    tween1.Kill();
                    tween1 = null;
                }
                spider1Touchable = false;
                break;
            case "spider2":
                if (tween2 != null)
                {
                    tween2.Kill();
                    tween2 = null;
                }
                spider2Touchable = false;
                break;
            case "spider3":
                if (tween3 != null)
                {
                    tween3.Kill();
                    tween3 = null;
                }
                spider3Touchable = false;
                break;
        }

        spider.transform.DOMoveY(-2f, 1f).OnComplete(() => {
            // Check if spider is still valid and not destroyed
            if (spider == null || !spider.activeInHierarchy)
                return;
                
            Vector3 dir = footpos.transform.position - spider.transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            float tdis = Vector3.Distance(spider.transform.position , footpos.transform.position);
            
            TweenHandle tween = spider.transform.DOMove(footpos.transform.position, tdis).SetEase(EaseType.Linear).OnComplete(() => {
                if (spider == null || !spider.activeInHierarchy)
                    return;
                    
                changeScenePos(0);
                showHide(girlScareDown, false);
                showHide(girltrample1, true);
               showHide(girlscare, false);
               showHide(spider,false) ;
               StartCoroutine("trampleSpide");
               showHide(bloodstains, true);
               bloodstains.GetComponent<Animator>().SetTrigger("blood");
               
               GameData.instance.isLock = true;
                showHideUI(false);
                GameManager.instance.playSfx("cai");
                GameManager.instance.playSfx("aghh");
            });
            if (spider == spider1)
            {
                spider1OnGround = true;
                if (tween1 != null)
                    tween1.Kill();
                tween1 = tween;
                currentnode1 = -1;
                Debug.Log("Spider1 is now on ground and can be smashed");
            }
            if (spider == spider2)
            {
                spider2OnGround = true;
                if (tween2 != null)
                    tween2.Kill();
                tween2 = tween;
                currentnode2 = -1;
                Debug.Log("Spider2 is now on ground and can be smashed");
            }
            if (spider == spider3)
            {
                spider3OnGround = true;
                if (tween3 != null)
                    tween3.Kill();
                tween3 = tween;
                currentnode3 = -1;
                Debug.Log("Spider3 is now on ground and can be smashed");
            }
           
        });
    }

    void showSpray(GameObject g)
    {
        showHide(insectAnim, true);
        showHide(smoke, true);
        smoke.GetComponent<Animator>().SetTrigger("smoke");
        insectSpray.transform.position = g.transform.position;
        StartCoroutine(Util.DelayToInvokeDo(() =>
        {
            showHide(insectAnim, false);
            showHide(smoke, false);
        }, .4f));
    }

    int nSmashed = 0;
    void smashSpider(GameObject spider)
    {
        Debug.Log("Smashing spider: " + spider.name);
        spider.GetComponent<BoxCollider2D>().enabled = false;
        GameObject tBlood = Instantiate(bloodstain);
        tBlood.transform.position = spider.transform.position;
        spider.GetComponent<SpriteRenderer>().DOFade(0, .3f);
        Destroy(spider.GetComponent<Animation>());

        GameManager.instance.playSfx("cai");

        if (spider == spider1)
        {
            if (tween1 != null)
                tween1.Kill();
            tween1 = null;
        }
        if (spider == spider2)
        {
            if (tween2 != null)
                tween2.Kill();
            tween2 = null;
        }
        if (spider == spider3)
        {
            if (tween3 != null)
                tween3.Kill();
            tween3 = null;
        }
        nSmashed++;
        if(nSmashed == 3)
        {
            GameData.instance.isLock = true;
            StartCoroutine("gameWin");
            showHide(girlScareDown, false);
            showHide(girlstandhappy, true);
            GameManager.instance.playSfx("wow");
        }
        tBlood.GetComponent<SpriteRenderer>().DOFade(0,1).SetDelay(.3f).OnComplete(()=> {
            Destroy(tBlood);
        });
    }


        int nTrample = 0;
    IEnumerator trampleSpide()
    {
        while (nTrample < 10)
        {
            yield return new WaitForSeconds(.1f);
            if (nTrample % 2 == 0)
            {
                showHide(girltrample1, false);
                showHide(girltrample2, true);
            }
            else
            {
                showHide(girltrample1, true);
                showHide(girltrample2, false);
            }
           
            
            
            nTrample++;
            if(nTrample == 10)
            {
                StopCoroutine("trampleSpide");
                StartCoroutine("gameFailed");
                showHide(girltrample1, false);
                showHide(girltrample2, false);
                showHide(girlscare, true);
                nTrample = 0;
            }
        }
    }

    void changeScenePos(int index, bool keeplock = false)
    {
        GameObject.Find("sceneContainer").GetComponent<SceneContainer>().manualScene(index, keeplock);
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
        changeScenePos(0);
        
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
    
    // Force stop all spider path animations
    void StopAllSpiderPaths()
    {
        if (tween1 != null)
        {
            tween1.Kill();
            tween1 = null;
        }
        if (tween2 != null)
        {
            tween2.Kill();
            tween2 = null;
        }
        if (tween3 != null)
        {
            tween3.Kill();
            tween3 = null;
        }
    }
}



