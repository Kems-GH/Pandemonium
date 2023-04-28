using UnityEngine;

public class PlacementManager : MonoBehaviour
{
    [SerializeField] private GameObject zoneVisible;

    private void Awake()
    {
        zoneVisible.SetActive(false);
        this.tag = "Placement Zone";
    }

    public void SetZoneVisible(bool visibleZone)
    {
        this.zoneVisible.SetActive(visibleZone);
    }
}
