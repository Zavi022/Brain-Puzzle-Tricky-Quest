using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class loadNxtScene : MonoBehaviour
{
    public string sceneName;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(laodScene());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator laodScene()
    {
        yield return new WaitForSeconds(4f);
        SceneManager.LoadScene(sceneName);
    }
}
