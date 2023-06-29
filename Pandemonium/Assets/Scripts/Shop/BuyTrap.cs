using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class BuyTrap : NetworkBehaviour
{
    [SerializeField] private int priceTrap;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hand"))
        {
            if(GoldManager.instance.GetNbGold() < priceTrap) return;
            
            GoldManager.instance.RemoveGoldServerRpc(priceTrap);
            GameManager.Instance.AddTrapCount();
        }
    }
}
