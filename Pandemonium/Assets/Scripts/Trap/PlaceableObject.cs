using Oculus.Interaction.Surfaces;
using System;
using System.Collections;
using UnityEngine;

public class PlaceableObject : MonoBehaviour
{
    [SerializeField] private GameObject trap;
    [SerializeField] private float minusY;
    
    private Vector3[] cornersZone;
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
            InstantiateGhostTrap(collider);
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

    private void OnTriggerStay(Collider collider)
    {
        if(collider.CompareTag(tagZone) && (isGrabFirstHand || isGrabSecondHand) && !isPreview)
        {
            InstantiateGhostTrap(collider);
        }
    }

    private void InstantiateGhostTrap(Collider collider)
    {
        // Security, but will never happen
        if (collider.GetType() != typeof(BoxCollider)) return;

        GetFourCorners((BoxCollider) collider);
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
        
        Vector3 point = zoneCollider.ClosestPoint(transform.position);

        //Check if the point is in the collider
        Vector3 positionGhost = new Vector3();
        Renderer rend = ghostTrap.GetComponent<Renderer>();

        // TODO check if corner is inside a the boxCollider

        positionGhost = new Vector3(point.x, zoneCollider.transform.position.y - minusY, point.z);
        ghostTrap.transform.position = positionGhost;
    }

    public void OnRelease()
    {

        if (isGrabFirstHand) isGrabFirstHand = false;
        else if (isGrabSecondHand) isGrabSecondHand = false;

        StartCoroutine(nameof(PlaceTrap));
    }

    IEnumerator PlaceTrap()
    {
        yield return new WaitForSeconds(1);
        if (!isGrabFirstHand && !isGrabSecondHand)
        {

            isPreview = false;

            // TODO : Poser le piège s'il est dans une position valide
            // Sinon, le remettre à sa place
            foreach (GameObject zone in zonesPlacement)
            {
                PlacementManager manager = zone.GetComponent<PlacementManager>();
                manager.SetZoneVisible(false);
            }

            Vector3 point = zoneCollider.ClosestPoint(transform.position);
            Vector3 positionGhost = new Vector3(point.x, zoneCollider.transform.position.y - minusY, point.z);

            ghostTrap.transform.position = positionGhost;

            Destroy(this.gameObject);
        }
    }

    private void GetFourCorners(BoxCollider collider)
    {
        cornersZone = new Vector3[4];
        cornersZone[0] = collider.center + new Vector3(-collider.size.x, -collider.size.y, -collider.size.z) * 0.5f;
        cornersZone[1] = collider.center + new Vector3(collider.size.x, -collider.size.y, -collider.size.z) * 0.5f;
        cornersZone[2] = collider.center + new Vector3(collider.size.x, -collider.size.y, collider.size.z) * 0.5f;
        cornersZone[3] = collider.center + new Vector3(-collider.size.x, -collider.size.y, collider.size.z) * 0.5f;
    }
}
