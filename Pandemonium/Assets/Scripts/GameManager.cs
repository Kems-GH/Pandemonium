using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class GameManager : NetworkBehaviour {
    [SerializeField] private Transform HeadPrefab;
    [SerializeField] private GameObject skeletonPrefab;
    [SerializeField] private bool isSolo = true;
    public static GameManager Instance;

    private void Awake() {
        Instance = this;
    }

    public override void OnNetworkSpawn() {
        SpawnPlayerServerRpc();
    }

    public bool IsSolo(){return isSolo;}

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

    public void Defeat()
    {
        if (!IsServer && !GameManager.Instance.IsSolo()) return;

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            if (IsServer) enemy.GetComponent<NetworkObject>().Despawn(true);
            else Destroy(enemy);
        }

        Debug.Log("Defeat");
        WaveManager.Instance.StopWave();
    }

    public void Victory()
    {
        if (!IsServer && !GameManager.Instance.IsSolo()) return;

        Debug.Log("Victory");
    }

    public GameObject GetSkeletonPrefab(){return skeletonPrefab;}
}
