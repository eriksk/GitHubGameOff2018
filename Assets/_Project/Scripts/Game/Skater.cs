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
    private Dictionary<int, TransformState> _states;
    private Transform[] _ragdollTransforms;

    public Vector3 CurrentPosition
    {
        get
        {
            return _rootRigidbody.position;
        }
    }

    void Start()
    {
        RagdollAnimator.Create();

        _rootRigidbody = RagdollAnimator.Ragdoll.GetComponentInChildren<Rigidbody>();
        _rigidbodies = RagdollAnimator.Ragdoll
            .ChildrenDeep()
            .Select(x => x.gameObject.GetComponent<Rigidbody>())
            .Where(x => x != null)
            .ToArray();
        
        _states = new Dictionary<int, TransformState>();
        _ragdollTransforms = new[]{RagdollAnimator.Ragdoll}.Concat(RagdollAnimator.Ragdoll.ChildrenDeep()).ToArray();

        foreach(var child in _ragdollTransforms)
        {
            _states.Add(child.GetInstanceID(), new TransformState(child));
        }

    }

    public void Restart(Transform startPosition)
    {
        transform.position = startPosition.position;
        transform.rotation = startPosition.rotation;
        
        // Put on the freaking skateboard
        if(ConnectToSkateboard)
        {
            transform.SetParent(Skateboard.transform);
            transform.localPosition = new Vector3(-0.16f, 0.23f, 0f);
            transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
            transform.SetParent(null);
        }
        
        foreach(var body in _rigidbodies)
        {
            body.ClearForces();
        }
        
        foreach(var child in _ragdollTransforms)
        {
            _states[child.GetInstanceID()].RestoreLocal(child);
        }

        State = SkaterState.OnBoard;

        if(_boardJoints != null)
        {
            foreach(var joint in _boardJoints)
            {
                if(joint == null) continue;
                Destroy(joint);
            }
        }
        
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
        
        CamController.Target = Skateboard.GetComponent<Rigidbody>();
        CamController.transform.position = startPosition.position + (Vector3.up * 5f) + (Vector3.back * 5f);
    }

    public void Fall()
    {
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
        CamController.FixFov(25f);
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
            if(Skateboard.Grounded)
            {
                CamController.FixFov(40);
            }
            else
            {
                CamController.FixFov(75);
            }

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