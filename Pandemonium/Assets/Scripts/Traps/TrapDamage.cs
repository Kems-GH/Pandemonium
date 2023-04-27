using UnityEngine;

public class TrapDamage : MonoBehaviour, ITrap
{
    [SerializeField] private int _damage;

    public int GetAmountDamage() => _damage;
}

internal interface ITrap
{
    int GetAmountDamage();
}