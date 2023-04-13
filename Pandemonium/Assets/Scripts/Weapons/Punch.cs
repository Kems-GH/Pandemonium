using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Punch : MonoBehaviour, IWeapon
{
    private readonly float damage = 25;

    float IWeapon.GetAmountDamage()
    {
        return damage;
    }
}
