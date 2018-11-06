
using UnityEngine;

public class Skateboard : MonoBehaviour
{
    public float Speed = 2f;
    private Rigidbody _rigidBody;

    void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // _rigidBody.AddForce(transform.forward * Speed);
        // _rigidBody.AddForce(Vector3.down * Speed);
    }
}