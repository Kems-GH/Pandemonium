using Oculus.Interaction.Surfaces;
using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlaceableObject : NetworkBehaviour
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
        if (collider.CompareTag(tagZone) && !collider.gameObject.GetComponent<PlacementManager>().GetIsFree())
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

        if (!IsServer && !GameManager.Instance.IsSolo()) yield break;

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
        if (!isPreview || ghostTrap == null) return;

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
        if (isGrabFirstHand || isGrabSecondHand)
        {
            yield break;
        }

        if (isPreview)
        {
            Destroy(ghostTrap);
            PlaceTrapServerRpc();
            zoneCollider.gameObject.GetComponent<PlacementManager>().SetFreePlace(true);
        }
        else
        {
            // We wait 10 seconds before the Trap box go his base position
            yield return new WaitForSeconds(10f);
            ReplaceTrapBoxServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void PlaceTrapServerRpc()
    {
        isPreview = false;
        Vector3 positionGhost = zoneCollider.transform.position;

        GameObject newTrap = Instantiate(trap);
        newTrap.GetComponent<NetworkObject>().Spawn(true);
        trap.transform.position = positionGhost;

        if (!IsServer && !GameManager.Instance.IsSolo()) return;

        if(IsServer) this.GetComponent<NetworkObject>().Despawn(true);
        else Destroy(this.gameObject);
    }

    [ServerRpc]
    private void ReplaceTrapBoxServerRpc()
    {
        this.transform.position = basePosition;
    }
}
