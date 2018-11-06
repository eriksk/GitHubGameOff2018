

using UnityEditor;

[CustomEditor(typeof(AnimationTargetTest))]
public class AnimationTargetTestEditor : Editor
{
    
    void OnSceneGUI()
    {
        var t = (target as AnimationTargetTest);

        if(t == null) return;

        

    }
}