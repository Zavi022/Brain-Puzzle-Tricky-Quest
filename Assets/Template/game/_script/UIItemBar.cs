using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIItemBar : MonoBehaviour
{
    // Start is called before the first frame update
    List<Transform> slots;
    const int maxSlot = 6;
    GameObject specialSp;
    GameObject specialCam;
    float[] scales = new float[maxSlot];
    SceneContainer sceneContainer;
    float barHeight;
    float UIScreenHeight;

    float oriWidth, oriHeight;
    void Start()
    {
        sceneContainer = GameObject.Find("sceneContainer").GetComponent<SceneContainer>();
        slots = new List<Transform>();
        for (int i = 0; i < maxSlot; i++)
        {
            Transform t = transform.GetChild(i);
            slots.Add(t);

        }
        specialCam = GameObject.Find("specialCam");
        specialSp = GameObject.Find("__specialSp");
        float _scaleFactor = transform.root.GetComponent<Canvas>().scaleFactor;

        UIScreenHeight = transform.root.GetComponent<RectTransform>().rect.height;
        barHeight = GetComponent<RectTransform>().rect.height;
        //print(barHeight);


        for (int i = 0; i < maxSlot; i++)
        {
            Transform t = transform.GetChild(i);
            slots[i].GetComponent<Image>().enabled = false;

            Image timg = slots[i].GetComponent<Image>();


            var tRectTransform = timg.transform as RectTransform;
            oriWidth = tRectTransform.sizeDelta.x;
            oriHeight = tRectTransform.sizeDelta.y;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    bool startDrag = false;
    public void OnMouseDown(GameObject g)
    {
        if (GameData.instance.isLock) return;
        startDrag = true;
        g.GetComponent<Image>().enabled = false;

        specialSp.GetComponent<SpriteRenderer>().enabled = true;
       
     
        Vector3 v3 = Input.mousePosition;
        v3 = specialCam.GetComponent<Camera>().ScreenToWorldPoint(v3);
        specialSp.transform.position = v3;


        Sprite tsp = g.GetComponent<Image>().sprite;

        //make the dragging sprite looks same size of its icon on slot.
       specialSp.GetComponent<SpriteRenderer>().sprite = tsp;
        int tIndex = g.transform.GetSiblingIndex();
        specialSp.transform.localScale = Vector3.one * scales[tIndex];
       
    }
    public void OnMouseUp(GameObject g)
    {
        if (GameData.instance.isLock) return;
        startDrag = false;
        g.GetComponent<Image>().enabled = true;

        specialSp.GetComponent<SpriteRenderer>().enabled = false;

        //detect where you drop the item
 



        RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        
        for (int ii = 0; ii < hits.Length; ii++)
        {
            bool breakloop = false;
            if (hits[ii].collider != null)
            {
                int tIndex = g.transform.GetSiblingIndex();

                ItemInteractable tItem = GameData.instance.itemPicked[tIndex].GetComponent<ItemInteractable>();
                List<InteractiveTarget> targets = tItem.interactiveTargets;
               
                for (int i = 0; i < targets.Count; i++) 
                {
                    if (targets[i].interactiveTarget == (hits[ii].collider.gameObject))
                    {
						// 目标不可见时忽略交互（不触发，不消耗）
						var sr = hits[ii].collider.GetComponent<SpriteRenderer>();
						if (sr != null && !sr.enabled)
						{
							break;
						}

                        breakloop = true;

                        //if must on a certain scene page even the item is able to be seen.
                        if (targets[i].isLock)
                        {
                            if (sceneContainer.cPage != targets[i].lockIndex)
                            {
                                print("Object is interactive with this, but not allowed on this page.");
                                
                                break;
                            }
                        }

                        if (targets[i].methodTarget != null)
                        {
                            string tparam = targets[i].param.Trim();
                            if (tparam != null && tparam != "")
                            {
                                targets[i].methodTarget.SendMessage(targets[i].methodName, targets[i].param);
                            }
                            else
                            {
                                targets[i].methodTarget.SendMessage(targets[i].methodName);
                            }
                            if (targets[i].consumable)
                            {
                                specialSp.GetComponent<SpriteRenderer>().enabled = false;
                                GameData.instance.itemPicked.RemoveAt(tIndex);
                                refreshUI();
                            }
                        }

                        //print(hit.collider.gameObject.name);
                        break;
                    }
                }
                
            }
            if (breakloop)
            {
                break;
            }
        }
    }

    public void OnMouseDrag(GameObject g)
    {
        if (GameData.instance.isLock) return;
        Vector3 v3 = Input.mousePosition;
        v3 = specialCam.GetComponent<Camera>().ScreenToWorldPoint(v3);
        specialSp.transform.position = v3;
    }


    bool isFolded = true;
    public void refreshUI()
    {

        
        for (int i = 0; i < maxSlot; i++)
        {
            Transform t = transform.GetChild(i);
            slots[i].GetComponent<Image>().enabled = false;
        }
        for (int i = 0; i < GameData.instance.itemPicked.Count; i++)
        {
            GameObject tItemPicked = GameData.instance.itemPicked[i];
            slots[i].GetComponent<Image>().enabled = true;



            Image timg = slots[i].GetComponent<Image>();

            var tRectTransform = timg.transform as RectTransform;
         


            slots[i].GetComponent<Image>().sprite =tItemPicked.GetComponent<ItemInteractable>().Icon;

            


            slots[i].GetComponent<Image>().SetNativeSize();
            float cWidth = tRectTransform.sizeDelta.x;
            float cHeight = tRectTransform.sizeDelta.y;


            float tAspect = timg.sprite.bounds.size.y / timg.sprite.bounds.size.x;

            float tFullWidth = tRectTransform.sizeDelta.x;
            float tScale = cWidth / tFullWidth;

            //keep icon fit each slot's width
            if (cWidth > cHeight)
            {
                tRectTransform.sizeDelta = new Vector2(oriWidth,oriWidth*tAspect);
            }
            else
            {
                tRectTransform.sizeDelta = new Vector2(oriHeight / tAspect, oriHeight);
            }

            //record the scale
            scales[i] = tScale;
            timg.color = Color.white;

        }


        
        if (!isFolded)
        {
            if (GameData.instance.itemPicked == null || GameData.instance.itemPicked.Count == 0)
            {
                gameObject.GetComponent<RectTransform>().DOLocalMoveY(0 - UIScreenHeight / 2 - barHeight / 2, .2f);
                isFolded = true;

            }
           
           
        }
        else
        {

            if (GameData.instance.itemPicked != null && GameData.instance.itemPicked.Count > 0)
            {
                gameObject.GetComponent<RectTransform>().DOLocalMoveY(0-UIScreenHeight/2 + barHeight/2, .2f);
                isFolded = false;
            }

        }
    }
}
