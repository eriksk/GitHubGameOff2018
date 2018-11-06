
using System;
using UnityEngine;

public class GameStats : MonoBehaviour
{
    public float FinalDistance;
    public float CurrentDistance;
    public float AirTime;

    private JumpState _jumpState = JumpState.Init;
    private Vector3 _jumpPosition, _landPosition;
    public bool Fallen;

    public void StartJump(GameObject gameObject)
    {
        StartJump(gameObject.transform.position);
    }

    public void StartJump(Vector3 position)
    {
        if(_jumpState != JumpState.Init) return;

        _jumpState = JumpState.Jumped;
        _jumpPosition = position;
        Debug.Log("Started Jump");
    }

    public void Landed(Vector3 position, float airTime)
    {
        if(_jumpState != JumpState.Jumped) return;

        _jumpState = JumpState.Landed;
        _landPosition = position;
        FinalDistance = Vector3.Distance(_jumpPosition, _landPosition);
        AirTime = airTime;
        Debug.Log("Landed");
    }

    public void PlayerFallen()
    {
        Fallen = true;
        _landPosition = ObjectLocator.Player.CurrentPosition;
        Debug.Log("Fallen");
    }

    void Update()
    {
        if(_jumpState != JumpState.Init)
        {
            CurrentDistance = Vector3.Distance(_jumpPosition, ObjectLocator.Player.CurrentPosition);
        }
    }
}

public enum JumpState
{
    Init,
    Jumped,
    Landed
}