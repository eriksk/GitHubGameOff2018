using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Rigidbody Target;
    public LayerMask Ground;

    public float Height = 5f;
    public float Distance = 15f;
    public float Damping = 5f;

    void Update()
    {
        if(Target == null) return;


        // RaycastHit hit;
        // var groundHeight = Target.position.y;
        // if(Physics.Raycast(transform.position + Vector3.up * 20f, Vector3.down, out hit, 1000f, Ground, QueryTriggerInteraction.Ignore))
        // {
        //     if(hit.point.y > groundHeight)
        //     {
        //         groundHeight = hit.point.y;
        //     }
        // }

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