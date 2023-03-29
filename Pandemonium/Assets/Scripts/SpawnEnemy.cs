using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class SpawnEnemy : NetworkBehaviour
{
    public GameObject enemyPrefab;

    public override void OnNetworkSpawn()
    {
        StartCoroutine(SpawnEnemyCoroutine());
    }

    IEnumerator SpawnEnemyCoroutine()
    {
        GameObject enemyGameObject = Instantiate(enemyPrefab, this.transform);
        enemyGameObject.GetComponent<NetworkObject>().Spawn(true);
        yield return new WaitForSeconds(4f);
        StartCoroutine(SpawnEnemyCoroutine());
    }
}
