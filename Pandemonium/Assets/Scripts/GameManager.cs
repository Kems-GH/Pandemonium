using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class GameManager : NetworkBehaviour {
    [SerializeField] private Transform HeadPrefab;
    [SerializeField] private int maxEnemyOnMap;
    [SerializeField] private GameObject skeletonPrefab;
    [SerializeField] private bool isSolo = true;
    public static GameManager Instance;

    private void Awake() {
        Instance = this;
    }

    public override void OnNetworkSpawn() {
        SpawnPlayerServerRpc();
    }

    public bool IsSolo()
    {
        return isSolo;
    }

    public GameManager SetSolo(bool isSolo)
    {
        this.isSolo = isSolo;

        return this;
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
