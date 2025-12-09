using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenuScene : MonoBehaviour
{
    [HideInInspector]
    public GameObject girlhello, girlsitfront, girlsitside, girlwalk, girlstandease,pos0,pos1,pos2;
    // Start is called before the first frame update
    void Start()
    {
        //system varaible initialzation
        foreach (System.Reflection.MemberInfo m in typeof(StartMenuScene).GetMembers())
        {
            var tempVar = GameObject.Find(m.Name);
            if (tempVar != null)
            {
                this.GetType().GetField(m.Name).SetValue(this, tempVar);
            }
        }

        girlsitside.SetActive(false);
        trnd = (int)Random.Range(4f, 7f);
        StartCoroutine("startPlay");
    }

    int trnd;
    IEnumerator startPlay()
    {
        
        yield return new WaitForSeconds(trnd);
        girlsitside.SetActive(false);
        girlsitfront.SetActive(false);
        girlwalk.transform.position = new Vector3(girlsitside.transform.position.x,girlwalk.transform.position.y,0);
        girlwalk.SetActive(true);

        trnd = (int)Random.Range(0, 2);
        if (trnd == 1)
        {
            girlwalk.transform.DOMoveX(pos1.transform.position.x, 1f).SetEase(EaseType.Linear).OnComplete(()=> {
                girlwalk.SetActive(false);
                trnd = Random.Range(0, 2);
                if(trnd == 1)
                {
                    girlstandease.SetActive(true);
                    girlstandease.transform.position = new Vector3(girlwalk.transform.position.x, girlstandease.transform.position.y, 0);
                    trnd = (int)Random.Range(4f, 7f);
                    StartCoroutine("leaveStand");
                }
                else
                {
                    girlhello.SetActive(true);
                    girlhello.transform.position = new Vector3(girlwalk.transform.position.x,girlhello.transform.position.y,0);
                    trnd = (int)Random.Range(4f, 7f);
                    StartCoroutine("leaveStand");
                }
            });
        }
        else
        {
            girlwalk.transform.DOMoveX(pos2.transform.position.x, 5f).SetEase(EaseType.Linear).OnComplete(()=> {
                walktoend();
            });
        }
    }

    IEnumerator leaveStand()
    {
        yield return new WaitForSeconds(trnd);
        girlhello.SetActive(false);
        girlstandease.SetActive(false);
        girlwalk.SetActive(true);
        girlwalk.transform.DOMoveX(pos2.transform.position.x, 2f).SetEase(EaseType.Linear).OnComplete(() =>
        {
            walktoend();
        });

    }

    void walktoend()
    {
        girlwalk.transform.position = new Vector3(pos0.transform.position.x, girlwalk.transform.position.y, 0);
        girlwalk.transform.DOMoveX(girlsitside.transform.position.x, 2.5f).SetEase(EaseType.Linear).OnComplete(() => { 
            girlwalk.SetActive(false);
            girlsitside.SetActive(true);
            StartCoroutine(Util.DelayToInvokeDo(() =>
            {
                girlsitside.SetActive(false);
                girlsitfront.SetActive(true);
                trnd = (int)Random.Range(4f, 7f);
                StartCoroutine("startPlay");
            }, 1f));
        });
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
