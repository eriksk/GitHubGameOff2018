
using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RagdollAnimator))]
public class RagdollAnimatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }

    void OnSceneGUI()
    {
        var root = (target as RagdollAnimator).AnimationRoot;

        if(root == null) return;

        DrawLinesHierarcy(root);
    }

    private void DrawLinesHierarcy(Transform parent)
    {
        Handles.color = Color.green;

        for(var i = 0; i < parent.childCount; i++)
        {
            var child = parent.GetChild(i);
            Handles.DrawLine(parent.position, child.position);
            DrawLinesHierarcy(child);
        }
    }
}