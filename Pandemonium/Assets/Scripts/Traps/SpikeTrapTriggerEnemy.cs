using Unity.Netcode;
using UnityEngine;

public class SpikeTrapTriggerEnemy : NetworkBehaviour
{
    [SerializeField] private SpikeTrap spikeTrap;
    private void OnTriggerEnter(Collider other) 
    {
        if (!IsServer && !GameManager.Instance.IsSolo()) return;
        if (!other.CompareTag("Enemy")) return;
        spikeTrap.Activate();
    }



}
