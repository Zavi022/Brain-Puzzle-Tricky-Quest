using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class shrinkEffect : MonoBehaviour
{
    // Start is called before the first frame update
    Vector3 startScale;
    void Start()
    {
        startScale = transform.localScale;
        transform.DOScale(startScale*.8f, .1f).SetLoops(-1,LoopType.Yoyo);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
