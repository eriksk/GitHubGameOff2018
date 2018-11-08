using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Rigidbody Target;
    public LayerMask Ground;

    public float Height = 5f;
    public float Distance = 15f;
    public float Damping = 5f;
    public float RotationDamping = 10f;

    public float FovChangeSpeed = 5f;

    private Camera _cam;
    private float _fov = 60f;

    void Start()
    {
        _cam = GetComponent<Camera>();
        _fov = 60f;
    }

    public void FixFov(float fov)
    {
        _fov = fov;
    }

    void Update()
    {
        _cam.fieldOfView = Mathf.Lerp(_cam.fieldOfView, _fov, FovChangeSpeed * Time.deltaTime);

        if(Target == null) return;

        var targetTransform = Target.transform;
        var forwardDirection = Target.velocity.normalized;

        if(Target.velocity.magnitude < 0.2f)
        {
            forwardDirection = targetTransform.forward;
        }

        var targetPosition = 
            (targetTransform.position + Vector3.up * 1f) + 
            (-forwardDirection * Distance) +
            (Vector3.up * Height);

        transform.position = Vector3.Lerp(transform.position, targetPosition, Damping * Time.deltaTime);

        var directionToTarget = (Target.position - transform.position).normalized;

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(directionToTarget, Vector3.up),
            RotationDamping * Time.deltaTime
        );
    }
}