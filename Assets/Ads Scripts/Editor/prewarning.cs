

 #if UNITY_EDITOR
 using UnityEditor;
 using UnityEditor.Build;
using UnityEngine;

[System.Obsolete]
class prewarning : IPreprocessBuild
{
    public int callbackOrder { get { return 0; } }
    public void OnPreprocessBuild(BuildTarget target, string path)
    {
        
        if (AdsIDS.instance && AdsIDS.instance.TestAds)
        {
            if (EditorUtility.DisplayDialog("WARNING", "ADS TEST MODE ACTIVE \n \n DO YOU WANT TO BUILD ? ", "YES"))
            {
                Debug.Log("YOU MAKE BUILD WITH TESTMODE...");
            }
            else
            {
                throw new UnityEditor.Build.BuildFailedException("YOU CANCELL BUILD DUE TO TESTMODE ACTIVE");
            }
        }
        else
        {
           
            return;
        }
    }
}
#endif
