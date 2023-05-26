using System.Collections.Generic;
using UnityEngine;

public class TrapZoneManager : MonoBehaviour
{

    private List<TrapZone> zonesPlacement; 
    private void Awake()
    {
        this.zonesPlacement = new List<TrapZone>(GetComponents<TrapZone>());
    }

    public void setAllVisible(bool visible)
    {
        foreach (TrapZone zone in zonesPlacement)
        {
            zone.gameObject.SetActive(visible);
        }
    }

    public void PlaceTrap(TrapZone trapZone)
    {
        this.zonesPlacement.Remove(trapZone);
        trapZone.isFree = false;
        trapZone.gameObject.SetActive(false);
    }
}
