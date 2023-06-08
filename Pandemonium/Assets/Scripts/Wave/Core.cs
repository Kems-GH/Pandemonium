using UnityEngine;
using Unity.Netcode;
using TMPro;

public class Core : NetworkBehaviour
{
    [SerializeField] private TMP_Text text;
    private NetworkVariable<int> health = new NetworkVariable<int>(100);
    private WaveManager waveManager;
    private Vector3 position;

    public override void OnNetworkSpawn()
    {
        health.OnValueChanged += (int oldValue, int newValue) => { UpdateText(); };
    }

    private void Awake() {
        this.waveManager = FindObjectOfType<WaveManager>();
        this.position = this.transform.position;
    }

    private void Start() 
    {
        this.UpdateText();
    }

    public void TakeDamage(int damage)
    {
        if (!IsServer) return;

        health.Value -= damage;
        this.UpdateText();

        if(health.Value <= 0)
        {
            this.waveManager.Defeat();
        }
    }

    void UpdateText()
    {
        text.text = health.Value.ToString();
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
