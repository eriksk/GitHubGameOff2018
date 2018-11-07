using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Skater : MonoBehaviour
{
    public Animator Animator;
    public Transform AnimationRoot;
    public RagdollAnimator RagdollAnimator;
    public Skateboard Skateboard;
    public Transform LeftFoot, RightFoot, RagdollRoot;
    public CameraController CamController;

    public SkaterState State;
    public bool ConnectToSkateboard = true;
    public float FeetStickyBreakForce = 15f;

    public float SpinForce = 1f;
    public float PitchForce = 1f;
    public float JumpForce = 5f;

    private Rigidbody _rootRigidbody;

    private float _leanHorizontal;
    private float _leanVertical;

    private FixedJoint[] _boardJoints;

    private Rigidbody[] _rigidbodies;

    public Vector3 CurrentPosition
    {
        get
        {
            return _rootRigidbody.position;
        }
    }

    void Start()
    {

        // Put on the freaking skateboard
        if(ConnectToSkateboard)
        {
            transform.SetParent(Skateboard.transform);
            transform.localPosition = new Vector3(-0.16f, 0.23f, 0f);
            transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
            transform.SetParent(null);
        }

        State = SkaterState.OnBoard;

        RagdollAnimator.Create();

        if(ConnectToSkateboard)
        {
            _boardJoints = new[]
            {
                RagdollAnimator.Ragdoll.FindDeep("foot_left").gameObject.gameObject.AddComponent<FixedJoint>(),
                RagdollAnimator.Ragdoll.FindDeep("foot_right").gameObject.gameObject.AddComponent<FixedJoint>()
            };

            foreach(var joint in _boardJoints)
            {
                joint.connectedBody = Skateboard.GetComponent<Rigidbody>();
                joint.breakForce = FeetStickyBreakForce;
            }
        }

        _rootRigidbody = RagdollAnimator.Ragdoll.GetComponentInChildren<Rigidbody>();
        _rigidbodies = RagdollAnimator.Ragdoll
            .ChildrenDeep()
            .Select(x => x.gameObject.GetComponent<Rigidbody>())
            .Where(x => x != null)
            .ToArray();

    }

    public void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.relativeVelocity.magnitude);
    }

    public void Fall()
    {
        // TODO: Calculate center of character instead
        CamController.Target = _rootRigidbody;
        ObjectLocator.Stats.PlayerFallen();
        State = SkaterState.Fallen;
        transform.SetParent(null);
        RagdollAnimator.Pinned = false; // Ragdoll on
        foreach(var joint in _boardJoints)
        {
            Destroy(joint);
        }
        _boardJoints = new FixedJoint[0];
    }

    public void Jump()
    {
        if(!Skateboard.Grounded) return;

        var rigidbody = Skateboard.GetComponent<Rigidbody>();

        var groundNormal = Skateboard.GroundNormal;

        rigidbody.AddForce(groundNormal * JumpForce * rigidbody.mass, ForceMode.Impulse);

        foreach(var body in _rigidbodies)
        {
            body.AddForce(groundNormal * JumpForce * body.mass, ForceMode.Impulse);
        }
    }

    void FixedUpdate()
    {
        if(State == SkaterState.Fallen)
            return;

        var rigidbody = Skateboard.GetComponent<Rigidbody>();

        // Torque relative to board space
        var relativeHorizontal = Skateboard.transform.TransformDirection(new Vector3(0f, 1f, 0f));
        var relativeVertical = Skateboard.transform.TransformDirection(new Vector3(1f, 0f, 0f));

        rigidbody.AddTorque(relativeHorizontal * _leanHorizontal * SpinForce);
        rigidbody.AddTorque(relativeVertical * _leanVertical * PitchForce);
        
        // Also twist body a bit
        _rootRigidbody.AddTorque(relativeHorizontal * _leanHorizontal * SpinForce * 0.5f);
        _rootRigidbody.AddTorque(relativeVertical * _leanVertical * PitchForce * 0.5f);
    }

    void Update()
    {
        if(State == SkaterState.OnBoard)
        {
            if(Input.GetKey(KeyCode.Space))
            {
                RagdollAnimator.Pinned = true;
                RagdollAnimator.Pin = 0.5f;
            }
            else
            {
                RagdollAnimator.Pinned = Skateboard.Grounded;
                RagdollAnimator.Pin = 1f;
            }

            if(_boardJoints.All(x => x == null))
            {
                Fall();
            }
        }

        
        if(State == SkaterState.Fallen)
        {
            _leanVertical = 0f;
            _leanHorizontal = 0f;
            UpdateAnimatorParameters();
            return;
        }

        _leanVertical = Input.GetAxis("Vertical");
        _leanHorizontal = Input.GetAxis("Horizontal");

        if(Input.GetButtonDown("Jump"))
        {
            Jump();
        }

        UpdateAnimatorParameters();
    }

    void UpdateAnimatorParameters()
    {
        Animator.SetFloat("lean_horizontal", _leanHorizontal);
        Animator.SetFloat("lean_vertical", _leanVertical);
    }
}

public enum SkaterState
{
    OnBoard,
    Fallen
}