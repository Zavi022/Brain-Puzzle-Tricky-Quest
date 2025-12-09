using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionInfo : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        myIndex = transform.GetSiblingIndex();
    }
    public int seats;//容积
    [HideInInspector]
    public int occupied;//占据多少
    [HideInInspector]
    public int myIndex;

    // Update is called once per frame
    void Update()
    {
        
    }
}
