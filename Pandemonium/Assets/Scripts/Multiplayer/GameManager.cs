using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class GameManager : NetworkBehaviour {
    [SerializeField] private Transform OVRControllerVisualLeftPrefab;
    [SerializeField] private Transform OVRControllerVisualRightPrefab;
    [SerializeField] private Transform HandVisualsLeftPrefab;
    [SerializeField] private Transform HandVisualsRightPrefab;

    public static GameManager Instance;

    private void Awake() {
        Instance = this;
    }

    public override void OnNetworkSpawn() {
        SpawnPlayerServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnPlayerServerRpc(ServerRpcParams serverRpcParams = default)
    {
        Debug.Log("SpawnPlayerServerRpc");
        var clientId = serverRpcParams.Receive.SenderClientId;
        Transform controllerLeft = Instantiate(OVRControllerVisualLeftPrefab);
        Transform controllerRight = Instantiate(OVRControllerVisualRightPrefab);
        // Transform handLeft = Instantiate(HandVisualsLeftPrefab);
        // Transform handRight = Instantiate(HandVisualsRightPrefab);
        controllerLeft.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
        controllerRight.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
    }
}
