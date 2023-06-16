using UnityEngine;

public class TrapDamage : MonoBehaviour, IGiveDamage
{
    [SerializeField] private int _damage;

    public int GetAmountDamage() => _damage;
}