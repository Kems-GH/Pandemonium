using UnityEngine;

public class PlacementManager : MonoBehaviour
{
    [SerializeField] private BoxCollider collider;
    [SerializeField] private GameObject placementVisible;

    private void Awake()
    {
        placementVisible.SetActive(false);
        this.tag = "Placement Zone";
    }

    public void SetZoneVisible(bool visibleZone)
    {
        this.placementVisible.SetActive(visibleZone);
    }
}
