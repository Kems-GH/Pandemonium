using UnityEngine;
using Unity.Netcode;
public class SkeletonHeadTrigger : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other) 
    {
        if(!IsServer) return;

        if(other.gameObject.tag == "Hand")
        {
            WaveManager.Instance.StartWave();
        }
    }
}
