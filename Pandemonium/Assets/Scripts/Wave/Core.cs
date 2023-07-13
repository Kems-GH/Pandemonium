using UnityEngine;
using Unity.Netcode;
using TMPro;

public class Core : NetworkBehaviour
{
    private NetworkVariable<int> health = new NetworkVariable<int>(100);
    private WaveManager waveManager;
    private Vector3 position;

    [SerializeField] private ParticleSystem sphereEffect;
    [SerializeField] private ParticleSystem ligthingEffect;

    private void Awake() {
        this.waveManager = FindObjectOfType<WaveManager>();
        this.position = this.transform.position;
    }

    public void TakeDamage(int damage)
    {
        if (!IsServer) return;

        SetTriggerDamageClientRpc();
        health.Value -= damage;

        if (health.Value <= 0)
        {
            sphereEffect.Stop();
            ligthingEffect.Stop();
            this.waveManager.Defeat();
        }
    }

    [ClientRpc]
    private void SetTriggerDamageClientRpc()
    {
        Animator animator = GetComponent<Animator>();
        animator.SetTrigger("onHit");
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
