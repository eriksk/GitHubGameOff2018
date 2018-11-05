using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Rigidbody Target;

    public float Height = 5f;
    public float Distance = 15f;
    public float Damping = 5f;

    void Update()
    {
        if(Target == null) return;

        var targetTransform = Target.transform;
        var forwardDirection = Target.velocity.normalized;

        if(Target.velocity.magnitude < 0.2f)
        {
            forwardDirection = targetTransform.forward;
        }

        var targetPosition = 
            targetTransform.position + 
            (-forwardDirection * Distance) + 
            (Vector3.up * Height);

        transform.position = Vector3.Lerp(transform.position, targetPosition, Damping * Time.deltaTime);
        transform.LookAt(Target.position);
    }
}