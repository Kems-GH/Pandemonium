using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class TrapSpawn : NetworkBehaviour
{
    [SerializeField] public GameObject trapBoxPrefab;

    private bool isSpawned = false;

    void Update()
    {
        if(IsServer && !isSpawned && GameManager.Instance.GetTrapCount() > 0)
        {
            GameObject trapBox = Instantiate(trapBoxPrefab, transform.position, Quaternion.identity);
            trapBox.GetComponent<NetworkObject>().Spawn();
            GameManager.Instance.RemoveTrapCount();
            isSpawned = true;
        }
    }

    public void ResetSpawn()
    {
        isSpawned = false;
    }
}
