using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragMove : MonoBehaviour
{
    // Start is called before the first frame update
    Camera mainCam;

    public float min, max;
    public bool lockX, lockY;
    Vector3 startPos;
    public bool canDrag = true;
    void Start()
    {
        mainCam = GameObject.Find("Main Camera").GetComponent<Camera>();
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnMouseDrag()
    {
        if (GameData.instance.isLock) return;
        if (!canDrag) return;
        if (startDrag)
        {
            Vector3 newMousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);

            if (lockX)
            {
                newMousePos.x = startPos.x;
                offsetPos.x = 0;
                
            }
            if (lockY)
            {
                newMousePos.y = startPos.y;
                offsetPos.y = 0;
            }


            if (min != 0 || max != 0)
            {
                if (lockX)
                {

                    newMousePos.y = Mathf.Clamp(newMousePos.y - offsetPos.y, startPos.y + min, startPos.y + max);
                }
                if (lockY)
                {

                    newMousePos.x = Mathf.Clamp(newMousePos.x - offsetPos.x, startPos.x + min, startPos.x + max);
                }
            }
            else
            {
                newMousePos = newMousePos - offsetPos;
            }

            newMousePos.z = 0;offsetPos.z = 0;
            //Vector3 tnewPos = newMousePos - offsetPos;

           

            transform.position = newMousePos;
        }
        
    }
    bool startDrag = false;
    Vector3 offsetPos = Vector3.zero;
    private void OnMouseDown()
    {
        if (GameData.instance.isLock) return;
        startDrag = true;
        Vector3 newMousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
   

        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //RaycastHit hit;
        //Physics.Raycast(ray, out hit);//函数是对射线碰撞的检测

        //offsetPos = hit.point - transform.position;

        offsetPos = new Vector3(newMousePos.x - transform.position.x,
            newMousePos.y - transform.position.y, 0);


    }

    private void OnMouseUp()
    {
        startDrag = false;
    }

}
