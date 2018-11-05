
using UnityEngine;

[RequireComponent(typeof(SkinnedMeshRenderer))]
public class DisplayBones : MonoBehaviour
{
    public Transform[] Bones { get { return GetComponent<SkinnedMeshRenderer>().bones; } }
}