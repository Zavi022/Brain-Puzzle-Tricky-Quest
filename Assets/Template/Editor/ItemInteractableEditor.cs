using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;
using System.Linq;
using System;
using System.Reflection;

[CustomEditor(typeof(ItemInteractable))]
public class ItemInteractableEditor : Editor
{



    ItemInteractable self;
    Texture2D checkOn = null;
    Texture2D checkOff = null;

    public override void OnInspectorGUI()
    {

        self = target as ItemInteractable;

        Undo.RecordObject(self, "ItemInteractable");




        //pickup action
        GUIStyle gs = new GUIStyle();
        EditorGUILayout.BeginHorizontal(GUI.skin.box);
        gs.alignment = TextAnchor.MiddleLeft;
        gs.fixedWidth = 80;
        //gs.normal.textColor = Color.cyan;
        GUILayout.Label("pick Action", gs, GUILayout.Width(80));

        if (self.pickUpInteractive == null)
        {
            self.pickUpInteractive = new PickUpInteractive();
        }
        //self.pickUpInteractive.methodTarget = (GameObject)EditorGUILayout.ObjectField(self.pickUpInteractive.methodTarget, typeof(GameObject), true, GUILayout.Width(80));

        //List<string> methodStr = new List<string>();
        //if (self.pickUpInteractive.methodTarget != null)
        //{
        //    Component[] myComponents = self.pickUpInteractive.methodTarget.GetComponents(typeof(Component));
        //    foreach (Component myComp in myComponents)
        //    {
        //        Type myObjectType = myComp.GetType();

        //        foreach (var tMethod in myComp.GetType().GetMethods())
        //        {

        //            try
        //            {
        //                methodStr.Add(tMethod.Name);
        //            }
        //            catch (Exception e)
        //            {
        //                Debug.LogError(e);
        //            }
        //        }
        //    }


            //if (methodStr.Count > 0)
            //{
            //    string[] ts = new string[methodStr.Count];
            //    for (int ii = 0; ii < ts.Length; ii++)
            //    {
            //        ts[ii] = methodStr[ii];
            //    }

                
                //self.pickUpInteractive.methodIndex = EditorGUILayout.Popup(self.pickUpInteractive.methodIndex, ts, GUILayout.Width(80));
                self.pickUpInteractive.methodName = EditorGUILayout.TextArea(self.pickUpInteractive.methodName, GUI.skin.textArea, GUILayout.Width(80));
        //methodStr[self.pickUpInteractive.methodIndex];
                                                                                                                                                   //self.pickUpInteractive.methodTarget = (GameObject)EditorGUILayout.ObjectField(self.pickUpInteractive.methodTarget, typeof(GameObject), true, GUILayout.Width(80));
GUILayout.Label("param", GUILayout.Width(40));
        self.pickUpInteractive.param = EditorGUILayout.TextArea(self.pickUpInteractive.param, GUI.skin.textArea, GUILayout.Width(80));

            //}

        //}


        EditorGUILayout.EndHorizontal();

        //lock scene index
        EditorGUILayout.BeginHorizontal(GUI.skin.box);
        GUILayout.Label("LockScene");
        self.pickUpInteractive.isLock = GUILayout.Toggle(self.pickUpInteractive.isLock, self.pickUpInteractive.isLock ? checkOn : checkOff);

        EditorGUI.BeginDisabledGroup(!self.pickUpInteractive.isLock);
        GUILayout.Label("sceneIndex");
        self.pickUpInteractive.lockIndex = EditorGUILayout.IntField(self.pickUpInteractive.lockIndex, GUI.skin.textArea, GUILayout.Width(40));

        EditorGUI.EndDisabledGroup();


        GUILayout.Label("pickable", GUILayout.Width(60));
        self.pickable = GUILayout.Toggle(self.pickable, self.pickable ? checkOn : checkOff);

        EditorGUILayout.EndHorizontal();

        if (!self.pickable) return;
        //interactives

        gs.normal.background = Texture2D.whiteTexture;
        gs.alignment = TextAnchor.MiddleCenter;


        if (self.interactiveTargets == null)
        {
            self.interactiveTargets = new List<InteractiveTarget>();

        }
        EditorGUILayout.BeginHorizontal(GUI.skin.box);

        EditorGUILayout.LabelField("Interactives",GUILayout.Width(80));
        GUILayout.Label("Icon", GUI.skin.box);
        self.Icon = (Sprite)EditorGUILayout.ObjectField(self.Icon, typeof(Sprite), true, GUILayout.Width(80));

        

        if (GUILayout.Button("Add", GUI.skin.button))
        {

            self.interactiveTargets.Add(new InteractiveTarget());
        }

        if (GUILayout.Button("clear", GUI.skin.button))
        {

            self.interactiveTargets = new List<InteractiveTarget>();
        }
        EditorGUILayout.EndHorizontal();


       



        for (int i = 0; i < self.interactiveTargets.Count; i++)
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.BeginHorizontal(GUI.skin.box);
            gs.normal.textColor = Color.red;
            //gs.stretchWidth = true ;
            gs.fixedWidth = 50;


            GUILayout.Label("target",gs);
            gs.normal.textColor = Color.blue;

            self.interactiveTargets[i].interactiveTarget = (GameObject)EditorGUILayout.ObjectField(self.interactiveTargets[i].interactiveTarget, typeof(GameObject),true, GUILayout.Width(80));



            GUILayout.Label("LockScene");
            self.interactiveTargets[i].isLock = GUILayout.Toggle(self.interactiveTargets[i].isLock, self.interactiveTargets[i].isLock ? checkOn : checkOff);

            EditorGUI.BeginDisabledGroup(!self.interactiveTargets[i].isLock);
            GUILayout.Label("sceneIndex");
            self.interactiveTargets[i].lockIndex = EditorGUILayout.IntField(self.interactiveTargets[i].lockIndex, GUI.skin.textArea);

            EditorGUI.EndDisabledGroup();

            if (self.interactiveTargets[i].isLock)
            {
                //tshowHides.show_hide[j].object2Show = self.gameObject;
            }





            EditorGUILayout.EndHorizontal();



           
            EditorGUILayout.BeginHorizontal(GUI.skin.box);
            gs.alignment = TextAnchor.MiddleLeft;
            gs.fixedWidth = 80;
            GUILayout.Label("Interactive", gs);


            self.interactiveTargets[i].methodTarget = (GameObject)EditorGUILayout.ObjectField(self.interactiveTargets[i].methodTarget, typeof(GameObject), true, GUILayout.Width(80));

            //List<MemberInfo> methodInfo = new List<MemberInfo>();
            //methodStr = new List<string>();
            //if (self.interactiveTargets[i].methodTarget != null)
            //{
            //    Component[] myComponents = self.interactiveTargets[i].methodTarget.GetComponents(typeof(Component));
            //    foreach (Component myComp in myComponents)
            //    {
            //        Type myObjectType = myComp.GetType();
            //        //foreach (var tMethod in myComp.GetType().GetRuntimeMethods())

            //        foreach (var tMethod in myComp.GetType().GetMethods())
            //        {

            //            try
            //            {

            //                methodStr.Add(tMethod.Name);

            //                //Debug.Log("Component:  " + myComp.name + "        Var Name:  " + thisVar.Name + "         Type:  " + thisVar.PropertyType + "       Value:  " + thisVar.GetValue(myComp, null) + "\n");
            //            }
            //            catch (Exception e)
            //            {
            //                Debug.LogError(e);
            //            }
            //        }
            //    }


                //if (methodStr.Count > 0)
                //{
                //    string[] ts = new string[methodStr.Count];
                //    for (int ii = 0; ii < ts.Length; ii++)
                //    {
                //        ts[ii] = methodStr[ii];
                //    }


                    //self.interactiveTargets[i].methodIndex = EditorGUILayout.Popup(self.interactiveTargets[i].methodIndex, ts,GUILayout.Width(80));
                    self.interactiveTargets[i].methodName = EditorGUILayout.TextArea(self.interactiveTargets[i].methodName, GUI.skin.textArea, GUILayout.Width(80));
            //methodStr[self.interactiveTargets[i].methodIndex];
            GUILayout.Label("param",GUILayout.Width(40));
            self.interactiveTargets[i].param = EditorGUILayout.TextArea(self.interactiveTargets[i].param, GUI.skin.textArea, GUILayout.Width(80));

            //    }

            //}

            EditorGUILayout.EndHorizontal();

            //cosumable checkbox
            EditorGUILayout.BeginHorizontal(GUI.skin.box);
            gs.normal.textColor = Color.green;
            gs.fixedWidth = 80;
            GUILayout.Label("Consumable", gs);
            gs.normal.textColor = Color.blue;
            self.interactiveTargets[i].consumable = GUILayout.Toggle(self.interactiveTargets[i].consumable, self.interactiveTargets[i].consumable ? checkOn : checkOff);



            //close button
            if (GUILayout.Button("X", GUI.skin.button, GUILayout.Width(20)))
            {

                self.interactiveTargets.RemoveAt(i);

            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }



        EditorUtility.SetDirty(target);
        if (GUI.changed)
        {

            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(self.gameObject.scene);

        }
    }
}

