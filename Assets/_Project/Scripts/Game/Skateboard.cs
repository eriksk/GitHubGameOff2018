
using UnityEngine;

public class Skateboard : MonoBehaviour
{
    private Rigidbody _rigidBody;

    void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        var velocity = _rigidBody.velocity;

        var velocityDirection = velocity.normalized;
        var speed = velocity.magnitude;
        var facingDirection = transform.forward;

        if(speed > 0.1f)
        {
            var crossDirection = Vector3.Cross(facingDirection, velocityDirection).normalized;

            Debug.DrawLine(transform.position, transform.position + velocityDirection, Color.red, 0.1f);
            Debug.DrawLine(transform.position, transform.position + facingDirection, Color.blue, 0.1f);
            Debug.DrawLine(transform.position, transform.position + crossDirection, Color.cyan, 0.1f);
            
        }
    }
}