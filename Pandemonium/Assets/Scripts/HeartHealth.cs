using UnityEngine;
using Unity.Netcode;
using TMPro;

public class HeartHealth : NetworkBehaviour
{
    [SerializeField] private TMP_Text text;

    private int damageTaken = 0;

    public void TakeDamage(int damage)
    {
        damageTaken += damage;
        UpdateText();
    }

    void UpdateText()
    {
        text.text = damageTaken.ToString();
    }
}
