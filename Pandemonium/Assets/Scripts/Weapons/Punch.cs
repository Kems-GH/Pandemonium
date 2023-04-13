using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Punch : MonoBehaviour, IWeapon
{
    [SerializeField] private float damage;

    float IWeapon.GetDamage()
    {
        return damage;
    }
}
