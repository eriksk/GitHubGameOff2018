
using System;
using System.Collections.Generic;
using UnityEngine;

public class Skater : MonoBehaviour
{
    public Animator Animator;

    private Rigidbody _rigidBody;

    void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
    }

    void Update()
    {
        UpdateAnimatorParameters();
    }

    void UpdateAnimatorParameters()
    {
    }
}