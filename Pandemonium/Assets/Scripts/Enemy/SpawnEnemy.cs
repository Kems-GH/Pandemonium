using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class SpawnEnemy : NetworkBehaviour
{
    [SerializeField] private GameObject enemyPrefab;

    public override void OnNetworkSpawn()
    {
        if (!IsServer && !GameManager.Instance.IsSolo()) return;

        InvokeRepeating(nameof(SpawnEnemyInstance), 0.2f, 4f);
    }

    void SpawnEnemyInstance()
    {
        if(GameManager.Instance.GetNbEnemy() < 5)
        {
            GameManager.Instance.AddEnnemy();
            GameObject enemyGameObject = Instantiate(enemyPrefab, this.transform);
            enemyGameObject.GetComponent<NetworkObject>().Spawn(true);
        }
    }
}
