using UnityEngine;

public class SetAsChildOnStart : MonoBehaviour
{
    public Transform Parent;
    public Vector3 LocalStartPosition;
    public Vector3 LocalStartRotation;

    public void Start()
    {
        transform.SetParent(Parent);
        transform.localPosition = LocalStartPosition;
        transform.localRotation = Quaternion.Euler(LocalStartRotation);
    }
}
