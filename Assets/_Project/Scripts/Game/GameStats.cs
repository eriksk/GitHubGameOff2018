
using System;
using UnityEngine;

public class GameStats : MonoBehaviour
{
    public float FinalDistance;
    public float CurrentDistance;
    public float AirTime;

    public Transform StartPosition;
    public Skater Skater;
    public Skateboard Skateboard;

    private JumpState _jumpState = JumpState.Init;
    private Vector3 _jumpPosition, _landPosition;
    public bool Fallen;

    public RecordIcon WorldRecordIcon;
    public RecordIcon PersonalRecordIcon;

    public void Start()
    {
        Restart();
    }

    public void Restart()
    {
        _jumpState = JumpState.Init;
        _jumpPosition = Vector3.zero;
        _landPosition = Vector3.zero;
        AirTime = 0f;
        CurrentDistance = 0f;
        FinalDistance = 0f;
        Fallen = false;
        
        Skateboard.Restart(StartPosition);
        Skater.Restart(StartPosition);
        Time.timeScale = 1f;
    }

    public void StartJump(GameObject gameObject)
    {
        StartJump(gameObject.transform.position);
    }

    public void StartJump(Vector3 position)
    {
        if(_jumpState != JumpState.Init) return;

        _jumpState = JumpState.Jumped;
        _jumpPosition = position;
    }

    public void Landed(Vector3 position, float airTime)
    {
        if(_jumpState != JumpState.Jumped) return;

        _jumpState = JumpState.Landed;
        _landPosition = position;
        FinalDistance = Vector3.Distance(_jumpPosition, _landPosition);
        AirTime = airTime;
        PersonalRecordIcon.SetDistance(_jumpPosition, Vector3.Distance(_jumpPosition, _landPosition));
    }

    public void PlayerFallen()
    {
        Time.timeScale = 0.65f;
        Fallen = true;
        _landPosition = ObjectLocator.Player.CurrentPosition;
    }

    void Update()
    {
        if(_jumpState != JumpState.Init)
        {
            CurrentDistance = Vector3.Distance(_jumpPosition, ObjectLocator.Player.CurrentPosition);
        }

        if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            Restart();
        }
    }
}

public enum JumpState
{
    Init,
    Jumped,
    Landed
}