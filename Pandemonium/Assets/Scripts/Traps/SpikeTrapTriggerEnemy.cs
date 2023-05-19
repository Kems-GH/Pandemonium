using Unity.Netcode;
using UnityEngine;

public class SpikeTrapTriggerEnemy : NetworkBehaviour
{
    [SerializeField] private SpikeTrap spikeTrap;
    private void OnTriggerEnter(Collider other) 
    {
        if (!IsServer) return;
        if (!other.CompareTag("Enemy")) return;
        spikeTrap.Activate();
    }
}
