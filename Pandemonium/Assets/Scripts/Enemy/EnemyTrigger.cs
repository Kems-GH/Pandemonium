using Unity.Netcode;
using UnityEngine;

public class EnemyTrigger: NetworkBehaviour
{
    public delegate void TriggerEventHandlerCollider(Collider collider);
    public event TriggerEventHandlerCollider OnTriggerEnterEvent;
    private void OnTriggerEnter(Collider collider)
    {
        if (!IsServer) return;

        OnTriggerEnterEvent?.Invoke(collider);
    }
}
