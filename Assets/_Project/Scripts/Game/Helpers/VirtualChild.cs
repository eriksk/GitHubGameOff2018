
using UnityEngine;

public class VirtualChild : MonoBehaviour
{
    public Transform Parent;
    public Vector3 RotationOffset;
    public bool Rotation = false;
    public bool Position = false;

    void Update()
    {
        if(Parent == null) return;

        if(Position)
        {
            transform.position = Parent.position;
        }
        
        if(Rotation)
        {
            transform.rotation = Quaternion.Euler(RotationOffset) * Parent.rotation;
        }
    }
}