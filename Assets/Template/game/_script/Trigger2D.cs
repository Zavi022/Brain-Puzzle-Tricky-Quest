using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Trigger2D : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.name)
        {
            case "boxClose":
                if(gameObject.name == "crosscircle") {
                    collision.gameObject.GetComponent<DragMove>().canDrag = false;
                    transform.root.SendMessage("boxPlaced");
                }
                GetComponent<BoxCollider2D>().enabled = false;
                collision.gameObject.transform.DOMove(transform.position, .3f);
                GameManager.instance.playSfx("push");
                break;
            case "mopCollider":
                transform.root.SendMessage("beCollided", transform.gameObject);
                break;
            case "booklv12":
                transform.root.SendMessage("beCollided", transform.gameObject);
                break;
            case "floorHeight":
                transform.root.SendMessage("beCollided", transform.gameObject);
                break;
            case "packupon":
                transform.root.SendMessage("beCollided", collision.transform.gameObject);
                break;
            case "tree1":
                transform.root.SendMessage("beCollided", collision.transform.gameObject);
                break;
           
        }
    }

}
