using UnityEngine;
using Unity.Netcode;
using TMPro;

public class Core : NetworkBehaviour
{
    private NetworkVariable<int> health = new NetworkVariable<int>(100);
    private WaveManager waveManager;
    private Vector3 position;

    public void TakeDamage(int damage)
    {
        if (!IsServer) return;

        health.Value -= damage;

        if(health.Value <= 0)
        {
            this.waveManager.Defeat();
        }
    }

    public void ResetHealth()
    {
        health.Value = 100;
    }

    public Vector3 GetPosition()
    {
        return this.position;
    }

    public int GetLife()
    {
        return health.Value;
    }
}
