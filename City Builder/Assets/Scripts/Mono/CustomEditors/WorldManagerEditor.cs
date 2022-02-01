using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WorldManager))]
public class WorldManagerEditor : Editor
{

    /*public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if(GUILayout.Button("Generate"))
        {
            WorldManager wm = target as WorldManager;
            if(wm.testMode)
            {
                wm.Regenerate();
            }
        }
    }*/

}
