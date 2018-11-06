

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Skater))]
public class SkaterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if(GUILayout.Button("Fall"))
        {
            (target as Skater).Fall();
        }
    }
}