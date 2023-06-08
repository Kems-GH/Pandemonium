using UnityEngine;
using Unity.Netcode;
using TMPro;

public class Core : NetworkBehaviour
{
    private NetworkVariable<int> health = new NetworkVariable<int>(100);

    public void TakeDamage(int damage)
    {
        if (!IsServer) return;

        health.Value -= damage;

        if(health.Value <= 0)
        {
            WaveManager.Instance.Defeat();
        }
    }

    public void ResetHealth()
    {
        health.Value = 100;
    }
}
