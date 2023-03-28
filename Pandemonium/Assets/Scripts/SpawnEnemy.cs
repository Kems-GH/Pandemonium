using System.Collections;
using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    public GameObject enemyPrefab;

    void Start()
    {
        StartCoroutine(SpawnEnemyCoroutine());
    }

    IEnumerator SpawnEnemyCoroutine()
    {
        Instantiate(enemyPrefab, this.transform);
        yield return new WaitForSeconds(4f);
        StartCoroutine(SpawnEnemyCoroutine());
    }
}
