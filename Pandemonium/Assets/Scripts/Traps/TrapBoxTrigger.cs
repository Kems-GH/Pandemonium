using UnityEngine;

public class TrapBoxTrigger : MonoBehaviour
{
    public delegate void TriggerEventHandler();
    public delegate void TriggerEventHandlerCollider(Collider collider);
    public event TriggerEventHandlerCollider OnTriggerEnterEvent;
    public event TriggerEventHandlerCollider OnTriggerExitEvent;
    public event TriggerEventHandler OnGrabEvent;
    public event TriggerEventHandler OnDragEvent;
    public event TriggerEventHandler OnReleaseEvent;

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag(TrapZone.TAG) && collider.GetComponent<TrapZone>().isFree)
        {
            OnTriggerEnterEvent?.Invoke(collider);
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if(collider.CompareTag(TrapZone.TAG))
        {
            OnTriggerExitEvent?.Invoke(collider);
        }
    }

    public void OnGrab()
    {
        OnGrabEvent?.Invoke();
    }

    public void OnRelease()
    {
        OnReleaseEvent?.Invoke();
    }
}
