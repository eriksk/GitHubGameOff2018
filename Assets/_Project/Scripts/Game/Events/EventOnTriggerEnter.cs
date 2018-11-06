
using UnityEngine;
using UnityEngine.Events;

public class EventOnTriggerEnter : MonoBehaviour
{
    public UnityEvent Event;

    void OnTriggerEnter(Collider collider)
    {
        if(Event == null) return;

        Event.Invoke();
    }
}