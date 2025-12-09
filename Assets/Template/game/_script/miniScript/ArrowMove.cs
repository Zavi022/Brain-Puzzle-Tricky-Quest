using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ArrowMove : MonoBehaviour
{
    public float XOffset, YOffset;
    // Start is called before the first frame update
    void Start()
    {
        transform.DOMove(new Vector3(transform.position.x + XOffset,transform.position.y+ YOffset) ,1f).SetLoops(-1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
