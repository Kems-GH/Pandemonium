using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlaceableObject : NetworkBehaviour
{
    [SerializeField] private GameObject trap;
    
    private Vector3 basePosition;
    private Quaternion baseRotation;

    private GameObject[] zonesPlacement;
    private GameObject ghostTrap;

    private Collider zoneCollider;

    private bool isPreview;
    private int nbGrab = 0;
    private TrapZoneManager trapZoneManager;
    private WaveManager waveManager;
    private int timeLastRelease = 0;

    private void Awake()
    {
        this.basePosition = transform.position;
        this.baseRotation = transform.rotation;
        this.zonesPlacement = GameObject.FindGameObjectsWithTag(TrapZone.TAG);
        this.trapZoneManager = GameObject.FindObjectOfType<TrapZoneManager>();
        this.waveManager = GameObject.FindObjectOfType<WaveManager>();

        if(this.zonesPlacement == null) Debug.LogError("No TrapZone found");
        if(this.trapZoneManager == null) Debug.LogError("TrapZoneManager not found");
        if(this.waveManager == null) Debug.LogError("WaveManager not found");

        TrapBoxTrigger trigger = GetComponentInChildren<TrapBoxTrigger>();
        trigger.OnTriggerEnterEvent += OnTriggerEnterEvent;
        trigger.OnTriggerExitEvent += OnTriggerExitEvent;
        trigger.OnGrabEvent += OnGrab;
        trigger.OnReleaseEvent += OnRelease;
    
    }

    private void Update()
    {
        if (!IsServer) return;

        if (this.nbGrab == 0 && Time.time - this.timeLastRelease > 10f)
        {
            this.ReplaceTrapBox();
        }
    }

    private void OnTriggerEnterEvent(Collider collider)
    {
        if(!IsServer) return;

        if(this.zoneCollider != null) DestroyGhostTrapServerRpc();
        this.zoneCollider = collider;

        CreateGhostTrapServerRpc();
    }

    private void OnTriggerExitEvent(Collider collider)
    {
        if(!IsServer) return;
        if(this.zoneCollider != collider) return;
        DestroyGhostTrapServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void CreateGhostTrapServerRpc()
    {
        this.isPreview = true;

        ghostTrap = Instantiate(trap, zoneCollider.transform.position, Quaternion.identity);
        ghostTrap.GetComponent<NetworkObject>().Spawn(true);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroyGhostTrapServerRpc()
    {
        this.zoneCollider = null;
        isPreview = false;
        ghostTrap.GetComponent<NetworkObject>().Despawn(true);
    }

    public void OnGrab()
    {
        ChangeOwnershipServerRpc();

        OnGrabServerRpc();

        if(this.waveManager.isWaveRunning) return;
        this.trapZoneManager.setAllVisible(true);
    }

    [ServerRpc(RequireOwnership = false)]
    public void OnGrabServerRpc()
    {
        this.nbGrab++;
    }

    public void OnRelease()
    {
        // Deactivate all Placement Zone
        this.trapZoneManager.setAllVisible(false);

        this.OnReleaseServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    public void OnReleaseServerRpc()
    {
        nbGrab--;
        timeLastRelease = (int)Time.time;

        if(this.waveManager.isWaveRunning) return;
        if(this.nbGrab == 0 && this.isPreview) StartCoroutine(nameof(PlaceTrap));
    }

    [ServerRpc(RequireOwnership = false)]
    public void ChangeOwnershipServerRpc(ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;
        this.gameObject.GetComponent<NetworkObject>().ChangeOwnership(clientId);
    }

    IEnumerator PlaceTrap()
    {
        yield return new WaitForSeconds(1f);
        PlaceTrapServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void PlaceTrapServerRpc()
    {
        if (this.nbGrab != 0) return;
        isPreview = false;

        trapZoneManager.PlaceTrap(zoneCollider.gameObject.GetComponent<TrapZone>());
        ghostTrap.GetComponent<NetworkObject>().Despawn(true);

        GameObject newTrap = Instantiate(trap, zoneCollider.transform.position, Quaternion.identity);
        newTrap.GetComponent<NetworkObject>().Spawn(true);

        this.GetComponent<NetworkObject>().Despawn(true);
    }

    private void ReplaceTrapBox()
    {
        this.transform.position = basePosition;
        this.transform.rotation = baseRotation;
    }
}
