using System;
using UnityEngine;

public class PlaceableObject : MonoBehaviour
{
    [SerializeField] private GameObject trap;
    
    private Vector3 basePosition;
    private GameObject[] zonesPlacement;
    private Vector3[] corners;
    private GameObject ghostTrap;
    private bool isPreview;

    private const string tagZone = "Placement Zone";
    
    private void Awake()
    {
        basePosition = transform.position;
        zonesPlacement = GameObject.FindGameObjectsWithTag(tagZone);
    }

    private void Start()
    {
        GetFourCorners();
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag(tagZone))
        {
            ghostTrap = Instantiate(trap);
            ghostTrap.transform.position = collider.transform.position;
            ghostTrap.transform.rotation = Quaternion.identity;
            isPreview = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag(tagZone))
        {
            isPreview = false;
            Destroy(ghostTrap);
        }
    }

    public void OnGrab()
    {
        foreach (GameObject zone in zonesPlacement)
        {
            PlacementManager manager = zone.GetComponent<PlacementManager>();
            manager.SetZoneVisible(true);
        }
    }

    public void OnDrag()
    {
        // TODO :  Afficher le fantôme de l'objet sur la zone de placement
    }

    public void OnRelease()
    {
        // TODO : Poser le piège s'il est dans une position valide
        // Sinon, le remettre à sa place
        foreach (GameObject zone in zonesPlacement)
        {
            PlacementManager manager = zone.GetComponent<PlacementManager>();
            manager.SetZoneVisible(false);
        }

        isPreview = false;
        Destroy(ghostTrap);
    }

    private void GetFourCorners()
    {
        BoxCollider box = gameObject.GetComponent<BoxCollider>();

        corners = new Vector3[4];
        corners[0] = box.center + new Vector3(-box.size.x, -box.size.y, -box.size.z) * 0.5f;
        corners[1] = box.center + new Vector3(box.size.x, -box.size.y, -box.size.z) * 0.5f;
        corners[2] = box.center + new Vector3(box.size.x, -box.size.y, box.size.z) * 0.5f;
        corners[3] = box.center + new Vector3(-box.size.x, -box.size.y, box.size.z) * 0.5f;
    }
}
