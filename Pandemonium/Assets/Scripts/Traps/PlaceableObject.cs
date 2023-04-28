using Oculus.Interaction.Surfaces;
using System;
using System.Collections;
using UnityEngine;

public class PlaceableObject : MonoBehaviour
{
    [SerializeField] private GameObject trap;
    
    private Vector3 basePosition;

    private GameObject[] zonesPlacement;
    private GameObject ghostTrap;

    private Collider zoneCollider;

    private bool isPreview;
    private bool isGrabFirstHand;
    private bool isGrabSecondHand;

    private const string tagZone = "Placement Zone";
    
    private void Awake()
    {
        basePosition = transform.position;
        zonesPlacement = GameObject.FindGameObjectsWithTag(tagZone);
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag(tagZone))
        {
            StartCoroutine(InstantiateGhostTrap(collider));
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if(collider.CompareTag(tagZone))
        {
            isPreview = false;
            Destroy(ghostTrap);
        }
    }

    IEnumerator InstantiateGhostTrap(Collider collider)
    {
        // We wait for the OnTriggerExit event to be launch
        yield return new WaitForSeconds(0.2f);

        // Security, but will never happen
        if (collider.GetType() != typeof(BoxCollider)) yield break;
        zoneCollider = collider;

        ghostTrap = Instantiate(trap);
        ghostTrap.transform.position = collider.transform.position;
        ghostTrap.transform.rotation = Quaternion.identity;
        isPreview = true;
    }

    public void OnGrab()
    {
        if(!isGrabFirstHand) isGrabFirstHand = true;
        else if(!isGrabSecondHand) isGrabSecondHand = true;
        
        foreach (GameObject zone in zonesPlacement)
        {
            PlacementManager manager = zone.GetComponent<PlacementManager>();
            manager.SetZoneVisible(true);
        }
    }

    public void OnDrag()
    {
        if (!isPreview) return;

        ghostTrap.transform.position = zoneCollider.transform.position;
    }

    public void OnRelease()
    {
        // Deactivate all Placement Zone
        foreach (GameObject zone in zonesPlacement)
        {
            PlacementManager manager = zone.GetComponent<PlacementManager>();
            manager.SetZoneVisible(false);
        }

        if (isGrabFirstHand) isGrabFirstHand = false;
        else if (isGrabSecondHand) isGrabSecondHand = false;

        StartCoroutine(nameof(PlaceTrap));
    }

    IEnumerator PlaceTrap()
    {
        yield return new WaitForSeconds(1f);
        if (!isGrabFirstHand && !isGrabSecondHand)
        {
            isPreview = false;
            Vector3 point = zoneCollider.ClosestPoint(transform.position);
            Vector3 positionGhost = zoneCollider.transform.position;

            ghostTrap.transform.position = positionGhost;

            Destroy(this.gameObject);
        }

        // We wait 10 seconds before the Trap box go his base position
        yield return new WaitForSeconds(10f);
        this.transform.position = basePosition;
    }
}
