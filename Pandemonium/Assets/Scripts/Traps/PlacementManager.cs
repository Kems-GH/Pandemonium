using Unity.Netcode;
using UnityEngine;

public class PlacementManager : MonoBehaviour
{
    [SerializeField] private GameObject zoneVisible;
    private NetworkVariable<bool> isFree = new NetworkVariable<bool>(true);
    
    private void Awake()
    {
        zoneVisible.SetActive(false);
        tag = "Placement Zone";
    }

    public void SetZoneVisible(bool visibleZone)
    {
        this.zoneVisible.SetActive(visibleZone);
    }

    public void SetFreePlace(bool isFreePlace)
    {
        this.isFree = new NetworkVariable<bool>(isFreePlace);
    }

    public bool GetIsFree()
    {
        return isFree.Value;
    }
}
