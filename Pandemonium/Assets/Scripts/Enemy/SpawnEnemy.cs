using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class SpawnEnemy : NetworkBehaviour
{
    [SerializeField] private GameObject enemyPrefab;

    private void Start() {
        if (!IsServer && !GameManager.Instance.IsSolo()) return;

        InvokeRepeating(nameof(SpawnEnemyInstance), 0.2f, 4f);
    }

    void SpawnEnemyInstance()
    {
        if (!IsServer && !GameManager.Instance.IsSolo()) return;
        if(GameManager.Instance.GetNbEnemy() < 5)
        {
            GameManager.Instance.AddEnnemy();
            Vector3 randPos = Random.insideUnitSphere * 3;
            // We create a new Vector3 to keep the y value
            Vector3 pos = new Vector3(randPos.x + transform.position.x, transform.position.y, randPos.z + transform.position.z);

            GameObject enemyGameObject = Instantiate(enemyPrefab, pos, transform.rotation);
            enemyGameObject.GetComponent<NetworkObject>().Spawn(true);
        }
    }
}
