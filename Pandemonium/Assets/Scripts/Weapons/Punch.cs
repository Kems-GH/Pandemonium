using UnityEngine;

public class Punch : MonoBehaviour, IGiveDamage
{
    private readonly int damage = 25;

    public int GetAmountDamage()
    {
        return damage;
    }
}
