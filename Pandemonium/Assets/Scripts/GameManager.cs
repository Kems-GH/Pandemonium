using Unity.Netcode;
using UnityEngine;


public class GameManager : NetworkBehaviour {
    [SerializeField] private Transform HeadPrefab;
    [SerializeField] private GameObject skeletonPrefab;
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

        Transform head = Instantiate(HeadPrefab);
        head.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
    }

    private void OnDisconnectedFromServer()
    {
        StartSession.instance.CancelSession();
    }

    public GameObject GetSkeletonPrefab(){return skeletonPrefab;}
}
