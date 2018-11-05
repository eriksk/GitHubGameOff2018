using System;
using System.Collections.Generic;
using UnityEngine;

public class Skater : MonoBehaviour
{
    public Animator Animator;
    public Transform AnimationRoot;
    public Skateboard Skateboard;
    public Transform LeftFoot, RightFoot, RagdollRoot;

    public SkaterState State;

    private Rigidbody _rigidBody;

    private float _leanHorizontal;
    private float _leanVertical;

    void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();

        CreateAnimationCopy();
        
        transform.SetParent(Skateboard.transform);
        transform.localPosition = new Vector3(0f, 0.23f, 0f);
        transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
        transform.SetParent(null);

        State = SkaterState.OnBoard;

        // var joints = new[]
        // {
        //     LeftFoot.gameObject.AddComponent<FixedJoint>(),
        //     RightFoot.gameObject.AddComponent<FixedJoint>()
        // };

        // foreach(var joint in joints)
        // {
        //     joint.connectedBody = Skateboard.GetComponent<Rigidbody>();
        // }
    }

    private void CreateAnimationCopy()
    {

    }

    public void Fall()
    {
        State = SkaterState.OnBoard;
        transform.SetParent(null);
        // TODO: activate ragdoll
    }

    void FixedUpdate()
    {
    }

    void Update()
    {
        _leanVertical = Input.GetAxis("Vertical");
        _leanHorizontal = Input.GetAxis("Horizontal");

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