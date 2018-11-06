
using System;
using UnityEngine;

public class Skateboard : MonoBehaviour
{
    public float Speed = 2f;
    public WheelConfig[] Wheels;
    public LayerMask Ground;
    public float GroundCheckDistance = 0.2f;

    private Rigidbody _rigidbody;

    public bool Grounded;

    public float AirTime;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Grounded = false;
        foreach(var wheel in Wheels)
        {
            RaycastHit hit;
            wheel.Grounded = Physics.Raycast(wheel.Model.position, -wheel.Model.up, out hit, GroundCheckDistance, Ground, QueryTriggerInteraction.Ignore);

            if(wheel.Grounded)
            {
                wheel.GroundNormal = hit.normal;
                Grounded = true;
            }
        }

        if(Grounded && AirTime > 2f)
        {
            ObjectLocator.Stats.Landed(transform.position, AirTime);
        }

        if(!Grounded)
        {
            AirTime += Time.deltaTime;
        }
        if(Grounded)
        {
            AirTime = 0f;
        }
    }

    void FixedUpdate()
    {
        var forwardForce = 0f;
        foreach(var wheel in Wheels)
        {
            if(!wheel.Grounded) continue;

            forwardForce += 0.25f;
            
            var velocity = _rigidbody.GetPointVelocity(wheel.Model.position);
            var velocityDirection = velocity.normalized;
            var velocityMag = velocity.magnitude;

            var lateralDirection = Vector3.Cross(transform.forward, velocityDirection);
            var lateralMag = Vector3.Dot(transform.forward, lateralDirection);
            lateralDirection = Vector3.Cross(transform.forward, lateralDirection);

            // Debug.DrawLine(wheel.Model.position, wheel.Model.position + lateralDirection, Color.red, 0.1f);
            _rigidbody.AddForceAtPosition(
                lateralDirection * 
                velocityMag *
                50f * 
                lateralMag * 
                _rigidbody.mass, wheel.Model.position);
                
            _rigidbody.AddForceAtPosition(
                -wheel.GroundNormal *
                10f *
                _rigidbody.mass, wheel.Model.position);
        }

        _rigidbody.AddForce(transform.forward * forwardForce * _rigidbody.mass);
    }
}

[Serializable]
public class WheelConfig
{
    public string Name;
    public Transform Model;
    public bool Grounded;
    public Vector3 GroundNormal;
}