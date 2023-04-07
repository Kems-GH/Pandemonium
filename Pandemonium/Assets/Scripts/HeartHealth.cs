using UnityEngine;
using Unity.Netcode;
using TMPro;

public class HeartHealth : NetworkBehaviour
{
    [SerializeField] private TMP_Text text;

    private NetworkVariable<int> damageTaken = new NetworkVariable<int>(0);

    public override void OnNetworkSpawn()
    {
        damageTaken.OnValueChanged += (int oldValue, int newValue) => { UpdateText(); };
    }

    public void TakeDamage(int damage)
    {
        damageTaken.Value += damage;
        UpdateText();
    }

    void UpdateText()
    {
        text.text = damageTaken.Value.ToString();
    }
}
