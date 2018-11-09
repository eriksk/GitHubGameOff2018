
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
    public Vector3 GroundNormal;

    public float AirTime;
    public float WheelieTime;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void Restart(Transform startPosition)
    {
        _rigidbody.ClearForces();
        transform.position = startPosition.position;
        transform.rotation = startPosition.rotation;
    }

    void Update()
    {
        Grounded = false;
        var wheelsOnGround = 0;
        foreach(var wheel in Wheels)
        {
            RaycastHit hit;
            wheel.Grounded = Physics.Raycast(wheel.Model.position, -wheel.Model.up, out hit, GroundCheckDistance, Ground, QueryTriggerInteraction.Ignore);

            if(wheel.Grounded)
            {
                wheelsOnGround++;
                wheel.GroundNormal = hit.normal;
                GroundNormal = hit.normal;
                Grounded = true;
            }
        }

        if(wheelsOnGround == 2)
        {
            WheelieTime += Time.deltaTime;
            if(WheelieTime > 0.5f)
            {
                // Debug.Log("Wheeeelie");
            }
            // ObjectLocator.Stats.Wheelie(wheelieTime);
        }
        else
        {
            WheelieTime = 0f;
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
        foreach(var wheel in Wheels)
        {
            if(!wheel.Grounded) continue;
            
            var normal = wheel.GroundNormal;

            var slopeDownRightDirection = Vector3.Cross(normal, Vector3.down);
            var slopeDownDirection = Vector3.Cross(slopeDownRightDirection, normal).normalized;
            var slopeUpDirection = -slopeDownDirection;

            var slopeDotProduct = Vector3.Dot(slopeDownDirection, transform.forward);

            var boardSlopeDirection = Vector3.Cross(transform.right, normal);
            var boardForward = (transform.forward * Mathf.Sign(slopeDotProduct)).normalized;

            var boardSlopeDotProduct = Vector3.Dot(slopeDownRightDirection, boardSlopeDirection);
            
            var carveCompensationDirection = boardSlopeDotProduct > 0f ? transform.right : -transform.right;
            var carveCompensationMagnitude = Mathf.Abs(boardSlopeDotProduct);

            Debug.DrawLine(wheel.Model.position, wheel.Model.position + carveCompensationDirection * carveCompensationMagnitude, Color.red, 3f);

            _rigidbody.AddForceAtPosition(
                carveCompensationDirection * 
                carveCompensationMagnitude *
                10f * 
                _rigidbody.mass, 
                wheel.Model.position);

            _rigidbody.AddForceAtPosition(
                -normal *
                5f *
                _rigidbody.mass, wheel.Model.position);
                
            _rigidbody.AddForce(
                transform.forward * 
                boardSlopeDotProduct *
                5f *
                _rigidbody.mass);
        }
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