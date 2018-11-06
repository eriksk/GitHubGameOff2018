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
        var rigidbody = Skateboard.GetComponent<Rigidbody>();
        rigidbody.AddForce(new Vector3(0f, 1f, 0f) * JumpForce * rigidbody.mass, ForceMode.Impulse);

        foreach(var body in _rigidbodies)
        {
            body.AddForce(new Vector3(0f, 1f, 0f) * JumpForce * body.mass, ForceMode.Impulse);
        }
    }

    void FixedUpdate()
    {
        if(State == SkaterState.Fallen)
            return;

        var rigidbody = Skateboard.GetComponent<Rigidbody>();

        rigidbody.AddRelativeTorque(new Vector3(0f, 1f, 0f) * _leanHorizontal * SpinForce);
        rigidbody.AddRelativeTorque(new Vector3(1f, 0f, 0f) * _leanVertical * PitchForce);
    }

    void Update()
    {
        if(State == SkaterState.OnBoard)
        {
            RagdollAnimator.Pinned = Skateboard.Grounded;

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