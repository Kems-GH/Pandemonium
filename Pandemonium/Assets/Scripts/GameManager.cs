using Unity.Netcode;
using UnityEngine;


public class GameManager : NetworkBehaviour {
    [SerializeField] private Transform HeadPrefab;
    [SerializeField] private GameObject skeletonPrefab;
    [SerializeField] private GameObject bossPrefab;
    public static GameManager Instance;

    private void Awake() {
        Instance = this;

        GameObject playerSpawn = GameObject.FindGameObjectWithTag("PlayerSpawn");
        GameObject.FindGameObjectWithTag("SlideInteractor").transform.position = playerSpawn.transform.position;
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
        SessionManager.Instance.CancelSession();
    }

    public GameObject GetSkeletonPrefab(){return skeletonPrefab;}
    public GameObject GetBossPrefab(){return bossPrefab;}
}
