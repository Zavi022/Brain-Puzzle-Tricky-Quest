using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level13Items : MonoBehaviour
{
    public int Occupy;//size of me

    [HideInInspector]
    public int seatIndex;
    [HideInInspector]
    public int seatPos;
    [HideInInspector]
    public int myIndex;
    [HideInInspector]
    public int cSeatIndex;
    [HideInInspector]
    public int cSeatPos;

    void Start()
    {
        myIndex = transform.GetSiblingIndex();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
