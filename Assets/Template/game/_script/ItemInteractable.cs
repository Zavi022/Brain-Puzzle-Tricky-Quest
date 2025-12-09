using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemInteractable : MonoBehaviour, IPointerDownHandler,IPointerUpHandler
{

    public bool pickable;
    public Sprite Icon;
    public string param;//for adding use;


    public List<InteractiveTarget> interactiveTargets;
    public PickUpInteractive pickUpInteractive;
    SceneContainer sceneContainer;
    void Start()
    {
        sceneContainer = GameObject.Find("sceneContainer").GetComponent<SceneContainer>();
        //eventName = new string[interactiveTarget.Length];
        addPhysics2DRaycaster();
    }

    // Update is called once per frame
    void Update()
    {
        //if (!Application.isEditor)
        //{
        //    if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        //    {
        //        if (IsPointerOverGameObject(Input.GetTouch(0).fingerId))
        //        {
        //            Debug.Log("Hit UI, Ignore Touch");
        //        }
        //        else
        //        {
        //            Debug.Log("Handle Touch Began");
        //            _OnMouseDown();
        //        }
        //    }
        //    if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
        //    {
        //        if (IsPointerOverGameObject(Input.GetTouch(0).fingerId))
        //        {
        //            Debug.Log("Hit UI, Ignore Touch");
        //        }
        //        else
        //        {
        //            Debug.Log("Handle Touch End");
        //            _OnMouseUp();
        //        }
        //    }



        //    isMobile = true;
        //}
        //else
        //{
        //    print(isMobile + "ismobile");
        //    isMobile = false;
        //}
        
    }

    bool IsPointerOverGameObject(int fingerId)
    {
        EventSystem eventSystem = EventSystem.current;
        return (eventSystem.IsPointerOverGameObject(fingerId)
            && eventSystem.currentSelectedGameObject != null);
    }


    public void fakeClick()
    {
        _OnMouseDown();
    }

    public void fakeUp()
    {
        _OnMouseUp();
    }

    bool isMobile;
    //private void OnMouseDown()
    public void OnPointerDown(PointerEventData eventData)
    {
        
        //if (isMobile) return;
        //if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        //    return;
        _OnMouseDown();
    }


    void _OnMouseDown()
    {
        if (GameData.getInstance().isLock) return;
        SpriteRenderer sp = transform.GetComponent<SpriteRenderer>();
        if (sp != null)
        {
            if (sp.enabled == false) return;
        }
        if (pickUpInteractive != null)
        {

            //if must on a certain scene page even the item is able to be seen.
            if (pickUpInteractive.isLock)
            {
                if (sceneContainer.cPage != pickUpInteractive.lockIndex)
                {
                    print("Object is interactive with this, but not allowed on this page.");
                    return;
                }
            }
            //
            pickUpInteractive.methodTarget = transform.root.gameObject;
            if (pickUpInteractive.methodTarget != null)
            {
                if (pickUpInteractive.param == null) pickUpInteractive.param = "";//test
                string tparam = pickUpInteractive.param.Trim();
                if (tparam != null && tparam != "")
                {
                    pickUpInteractive.methodTarget.SendMessage(pickUpInteractive.methodName, pickUpInteractive.param);
                }
                else
                {
                    if (pickUpInteractive != null && pickUpInteractive.methodName != null)
                    {
                        string tMethodName = pickUpInteractive.methodName.Trim();
                        if (tMethodName != null && tMethodName != "")
                        {
                            pickUpInteractive.methodTarget.SendMessage(pickUpInteractive.methodName);
                        }
                    }
                }


            }

        }

        if (pickable)
        {
            if (GameData.getInstance().itemPicked.Count < 6)
            {
                GameData.getInstance().itemPicked.Add(gameObject);
                GameObject.Find("items").GetComponent<UIItemBar>().refreshUI();
                gameObject.SetActive(false);
                				//sfx test
				GameManager.getInstance().playSfx("pop");
            }
            else
            {
                Image tBarImage = GameObject.Find("items").GetComponent<Image>();
                GameData.getInstance().isLock = true;
                Color tCColor = tBarImage.color;
                Sequence tseq = new Sequence();
                tseq.Append(tBarImage.DOColor(new Color(1, 0, 0, .5f), .4f).SetUpdate(true));
                tseq.Append(tBarImage.DOColor(tCColor, .4f).SetUpdate(true));
                tseq.Play().OnComplete(() => { GameData.getInstance().isLock = false; });
            }
        }
    }


    void addPhysics2DRaycaster()
    {
        Physics2DRaycaster physicsRaycaster = GameObject.FindObjectOfType<Physics2DRaycaster>();
        if (physicsRaycaster == null)
        {
            Camera.main.gameObject.AddComponent<Physics2DRaycaster>();
        }
    }

    //private void OnMouseUp()
    public void OnPointerUp(PointerEventData eventData)
    {
        //if (isMobile) return;
        //if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        //    return;

        _OnMouseUp();   
    }

    void _OnMouseUp()
    {
        
        if (GameData.getInstance().isLock) return;
        SpriteRenderer sp = transform.GetComponent<SpriteRenderer>();
        if (sp != null)
        {
            if (sp.enabled == false) return;
        }
        if (pickUpInteractive != null)
        {

            //if must on a certain scene page even the item is able to be seen.
            if (pickUpInteractive.isLock)
            {
                if (sceneContainer.cPage != pickUpInteractive.lockIndex)
                {
                    print("Object is interactive with this, but not allowed on this page.");
                    return;
                }
            }
            //
            pickUpInteractive.methodTarget = transform.root.gameObject;
            if (pickUpInteractive.methodTarget != null)
            {
                string tparam = pickUpInteractive.param.Trim();
                if (tparam != null && tparam != "")
                {
                    pickUpInteractive.methodTarget.SendMessage(pickUpInteractive.methodName, pickUpInteractive.param + "_up");
                }
                else
                {
                    if (pickUpInteractive != null)
                    {
                        string tMethodName = pickUpInteractive.methodName.Trim();
                        if (tMethodName != null && tMethodName != "")
                        {
                            pickUpInteractive.methodTarget.SendMessage(pickUpInteractive.methodName);
                        }
                    }
                }


            }

        }
    }


}

[System.Serializable]
public class InteractiveTarget
{
    public GameObject interactiveTarget;
    public int lockIndex;
    public bool isLock;
    public GameObject methodTarget;
    public int methodIndex;
    public string methodName;
    public string param;
    public bool consumable;
}


[System.Serializable]
public class PickUpInteractive
{
    public int lockIndex;
    public bool isLock;
    public GameObject interactiveTarget;
    public GameObject methodTarget;
    public int methodIndex;
    public string methodName;
    public string param;
    public bool consumable;
}