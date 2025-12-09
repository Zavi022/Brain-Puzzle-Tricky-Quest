using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneFall : MonoBehaviour
{
    public Vector3 startPos;
    // Start is called before the first frame update
    void Start()
    {
        transform.localEulerAngles = new Vector3(0, 0, Random.Range(0, 360f));
        transform.localScale = Vector3.one * Random.Range(.5f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.y < -10)
        {
            transform.position = startPos + new Vector3(Random.Range(-.3f,.3f), Random.Range(-.3f, .3f),0);
            GetComponent<Rigidbody2D>().isKinematic = true;
            transform.localEulerAngles = new Vector3(0, 0, Random.Range(0, 360f));
            transform.localScale = Vector3.one * Random.Range(.5f, 1f);
        }
    }
}
