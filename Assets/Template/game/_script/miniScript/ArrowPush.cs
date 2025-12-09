using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ArrowPush : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        GetComponent<Image>().fillAmount = 0;
    }



    // Update is called once per frame
    void FixedUpdate()
    {
        GetComponent<Image>().fillAmount += .02f;
        
    }
}
