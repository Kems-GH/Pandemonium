using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class TrapZoneManager : NetworkBehaviour 
{
    private List<TrapZone> zonesPlacement; 
    private void Awake()
    {
        this.zonesPlacement = new List<TrapZone>(GameObject.FindObjectsOfType<TrapZone>());
        if(this.zonesPlacement == null) Debug.LogError("No TrapZone found");
    }

    public void setAllVisible(bool visible)
    {
        foreach (TrapZone zone in zonesPlacement)
        {
            zone.setVisible(visible);
        }
    }

    public void PlaceTrap(TrapZone trapZone)
    {
        this.zonesPlacement.Remove(trapZone);
        trapZone.isFree = false;
        trapZone.gameObject.SetActive(false);
    }
}
