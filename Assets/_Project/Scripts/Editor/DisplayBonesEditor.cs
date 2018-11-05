using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DisplayBones))]
public class DisplayBonesEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();


        var bones = (target as DisplayBones).Bones;

        GUILayout.Label("Bones: " + bones.Length);
        foreach(var bone in bones)
        {
            GUILayout.Label(bone.name);
        }
    }
}