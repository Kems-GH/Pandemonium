using UnityEngine;
using Unity.Netcode;
public class StartWaveTrigger : NetworkBehaviour
{
    public delegate void TriggerEventHandler();
    public event TriggerEventHandler OnTiriggerEnter;
    private void OnTriggerEnter(Collider other) 
    {
        if(!IsServer) return;

        if(other.gameObject.tag == "Hand")
        {
            OnTiriggerEnter?.Invoke();
        }
    }

    [ClientRpc]
    public void DeactivateClientRpc()
    {
        this.gameObject.SetActive(false);
    }

    [ClientRpc]
    public void ActivateClientRpc()
    {
        this.gameObject.SetActive(false);
    }
}
