using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class GameManager : NetworkBehaviour {
    [SerializeField] private Transform HeadPrefab;
    [SerializeField] private int maxEnemyOnMap;


    public static GameManager Instance;

    private int nbEnemy = 0;
    [SerializeField] private bool isSolo = true;

    private void Awake() {
        Instance = this;
    }

    public override void OnNetworkSpawn() {
        SpawnPlayerServerRpc();
        isSolo = false;
    }

    public bool IsSolo()
    {
        return isSolo;
    }

    public void AddEnnemy()
    {
        this.nbEnemy++;
    }

    public void RemoveEnemy()
    {
        this.nbEnemy--;
    }

    public int GetNbEnemy()
    {
        return nbEnemy;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnPlayerServerRpc(ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;

        Transform head = Instantiate(HeadPrefab);
        head.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
    }
}
