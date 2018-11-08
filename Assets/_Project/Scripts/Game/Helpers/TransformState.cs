
using UnityEngine;

public class TransformState
{
    public Vector3 Position;
    public Quaternion Rotation;
    
    public Vector3 LocalPosition;
    public Quaternion LocalRotation;

    public TransformState(Transform transform)
    {
        Position = transform.position;
        Rotation = transform.rotation;
        LocalPosition = transform.localPosition;
        LocalRotation = transform.localRotation;
    }

    public void RestoreWorld(Transform transform)
    {
        transform.position = Position;
        transform.rotation = Rotation;
    }
    
    public void RestoreLocal(Transform transform)
    {
        transform.localPosition = LocalPosition;
        transform.localRotation = LocalRotation;
    }
}