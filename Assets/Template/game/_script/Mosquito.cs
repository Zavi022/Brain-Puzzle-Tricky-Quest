using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mosquito : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("waitaframe");
        GameManager.instance.stopMusic("mosquito");
    }

    IEnumerator waitaframe()
    {
        yield return new WaitForEndOfFrame();
        GameManager.instance.playSfx("mosquito");
        StartCoroutine("loopMosSound");
    }
    IEnumerator loopMosSound()
    {
        while (true)
        {
            yield return new WaitForSeconds(4.545f);
            GameManager.instance.playSfx("mosquito");
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void stopFly()
    {
        GameManager.instance.stopMusic("mosquito");
        StopCoroutine("loopMosSound");
    }
}
