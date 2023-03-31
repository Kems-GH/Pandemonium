using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class GameManager : NetworkBehaviour {
    [SerializeField] private Transform OVRControllerVisualLeftPrefab;
    [SerializeField] private Transform OVRControllerVisualRightPrefab;
    [SerializeField] private Transform HandVisualsLeftPrefab;
    [SerializeField] private Transform HandVisualsRightPrefab;
    [SerializeField] private Transform HeadPrefab;

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
        var clientId = serverRpcParams.Receive.SenderClientId;
        Transform controllerLeft = Instantiate(OVRControllerVisualLeftPrefab);
        Transform controllerRight = Instantiate(OVRControllerVisualRightPrefab);
        Transform handLeft = Instantiate(HandVisualsLeftPrefab);
        Transform handRight = Instantiate(HandVisualsRightPrefab);
        Transform head = Instantiate(HeadPrefab);
        
        controllerLeft.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
        controllerRight.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
        handLeft.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
        handRight.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
        head.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
    }
}
