using System.Collections;
using System.Collections.Generic;
using Pandemonium.Assets.Scripts;
using UnityEngine;

public class SpawnManager
{
    public static IEnumerator SpawnEnemy(Wave wave, List<Spawner> spawners)
    {

        Spawner[] actifSpawner = SelectAndActivateSpawner(spawners, wave.nbSpawner);

        yield return new WaitForSeconds(wave.spawnRate);

        int nbEnemyWave = wave.nbEnemy;
        bool isBossSpawned = false;

        while (nbEnemyWave > 0)
        {
            yield return new WaitForSeconds(wave.spawnRate);

            // Spawn enemy on spawner
            int nbEnemy = Mathf.Min(Random.Range(wave.minNbSpawn, wave.maxNbSpawn), nbEnemyWave);

            foreach (Spawner spawner in actifSpawner)
            {
                if(!isBossSpawned)
                {
                    SpawnRandomBoss(spawner, wave);
                    isBossSpawned = true;
                }
                
                for (int i = 0; i < nbEnemy; i++)
                {
                    SpawnRandomEnemy(spawner, wave);
                }
            }
            nbEnemyWave -= nbEnemy;
        }
    }

    private static Spawner[] SelectAndActivateSpawner(List<Spawner> spawners, int nbActifSpawner)
    {
        nbActifSpawner = Mathf.Min(nbActifSpawner, spawners.Count);
        Spawner[] actifSpawner = new Spawner[nbActifSpawner];
        Tools.ShuffleList(spawners);
        spawners.CopyTo(0, actifSpawner, 0, nbActifSpawner);

        // Activate spawner
        for (int i = 0; i < actifSpawner.Length; i++)
        {
            actifSpawner[i].ActivateClientRpc();
        }

        return actifSpawner;
    }

    private static void SpawnRandomEnemy(Spawner spawner, Wave wave)
    {
        int rand = Random.Range(0, 100);

        if (rand < wave.skeletonProba)
        {
            spawner.Spawn(GameManager.Instance.GetSkeletonPrefab());
        }
        else
        {
            spawner.Spawn(GameManager.Instance.GetSkeletonPrefab());
        }
    }

    private static void SpawnRandomBoss(Spawner spawner, Wave wave) {
        int rand = Random.Range(0, 100);

        if (rand < wave.bossProba)
        {
            spawner.Spawn(GameManager.Instance.GetBossPrefab());
        }
        else
        {
            spawner.Spawn(GameManager.Instance.GetBossPrefab());
        }
    }
}
