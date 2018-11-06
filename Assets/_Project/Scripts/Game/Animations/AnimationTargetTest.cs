
using System;
using UnityEngine;

public class AnimationTargetTest : MonoBehaviour
{
    public Rigidbody Source, Target;
    [Range(0f, 1f)]
    public float Pin = 1f;
    public float Damping = 0.5f;

    void Start()
    {
        Target.maxAngularVelocity = float.MaxValue;
    }

    void FixedUpdate()
    {
        RotateTowards(Target, Source.transform, Target.transform);
    }

    private void RotateTowards(Rigidbody body, Transform source, Transform target)
    {
        var targetForward = (target.transform.up);
        var sourceForward = (source.parent.InverseTransformDirection(source.transform.up));

		var targetDelta = (-targetForward).normalized;
 
		var angleDiff = Vector3.Angle(sourceForward, targetDelta);
		var cross = Vector3.Cross(sourceForward, targetDelta);

        var angleMag = Mathf.Clamp01(Mathf.Abs(angleDiff) / 180f);

        body.angularDrag = angleMag * 10f; // Fixed damping

        var torque = (cross * angleDiff) * body.mass;

        if(Pin >= 1f)
        {
            body.transform.localRotation = target.localRotation;
        }
        else
        {
            body.AddTorque(torque * Pin);
        }
    }
}