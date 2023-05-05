using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using Pandemonium.Assets.Scripts;
using System.Collections;
using UnityEngine.SceneManagement;

public class WaveManager : NetworkBehaviour
{
    [SerializeField] private List<Spawner> spawners;
    [SerializeField] private List<Wave> waves;
    [SerializeField] private GameObject activator;
    [SerializeField] private GameObject endGameMenu;
    [SerializeField] private TMPro.TMP_Text textEndGame;

    private int currentWave = 0;
    private int nbWave = 0;
    public static WaveManager Instance;

    private void Awake() 
    {
        // TODO: Change if we have other level
        if (Instance == null) Instance = this;
        else Debug.LogError("Multiple WaveManager in scene");
        nbWave = waves.Count;
    }

    public void StartWave()
    {
        if (!IsServer && !GameManager.Instance.IsSolo()) return;
        this.DeactivateActivatorClientRpc();
        if (currentWave < nbWave)
        {
            Wave wave = waves[currentWave];
            StartCoroutine(SpawnEnemy(wave));
            currentWave++;
        }
    }

    [ClientRpc]
    public void DeactivateActivatorClientRpc()
    {
        this.activator.SetActive(false);
    }
    [ClientRpc]
    public void ActivateActivatorClientRpc()
    {
        this.activator.SetActive(true);
    }
    private IEnumerator SpawnEnemy(Wave wave)
    {
        if (!IsServer && !GameManager.Instance.IsSolo()) yield break;

        Spawner[] ActifSpawner = new Spawner[waves[currentWave].nbSpawner];

        // Randomize spawner and select the number of spawner needed
        Tools.ShuffleList(spawners);
        spawners.CopyTo(0, ActifSpawner, 0, wave.nbSpawner < spawners.Count ? wave.nbSpawner : spawners.Count);

        // Activate spawner
        for(int i = 0; i < ActifSpawner.Length; i++)
        {
            ActifSpawner[i].ActivateClientRpc();
        }
        yield return new WaitForSeconds(wave.spawnRate);
        int nbEnemyWave = wave.nbEnemy;
        while(nbEnemyWave > 0)
        {
            yield return new WaitForSeconds(wave.spawnRate);
            // Spawn enemy on spawner
            int nbEnemy = Mathf.Min(Random.Range(wave.minNbSpawn, wave.maxNbSpawn), nbEnemyWave);
            foreach (Spawner spawner in ActifSpawner)
            {
                for (int i = 0; i < nbEnemy; i++)
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
            }
            nbEnemyWave -= nbEnemy;
        }
        InvokeRepeating(nameof(CheckWaveFinished), 1f, 1f);
    }

    private void CheckWaveFinished()
    {
        if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
        {
            CancelInvoke(nameof(CheckWaveFinished));
            EndWave();
        }
    }

    private void EndWave()
    {
        if (!IsServer && !GameManager.Instance.IsSolo()) return;

        foreach (Spawner spawner in spawners)
        {
            spawner.DeactivateClientRpc();
        }
        if (currentWave < nbWave)
        {
            this.ActivateActivatorClientRpc();
        }
        else
        {
            this.Victory();
        }
    }

    public void StopWave()
    {
        if (!IsServer && !GameManager.Instance.IsSolo()) return;

        StopAllCoroutines();
        CancelInvoke(nameof(CheckWaveFinished));
        foreach (Spawner spawner in spawners)
        {
            spawner.DeactivateClientRpc();
        }
    }

    public void Defeat()
    {
        if (!IsServer && !GameManager.Instance.IsSolo()) return;
        WaveManager.Instance.StopWave();

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            if (IsServer) enemy.GetComponent<NetworkObject>().Despawn(true);
            else Destroy(enemy);
        }

        DisplayEndGameMenuClientRpc("Game Over");
    }

    public void Victory()
    {
        if (!IsServer && !GameManager.Instance.IsSolo()) return;

        DisplayEndGameMenuClientRpc("Victory");
    }

    [ClientRpc]
    private void DisplayEndGameMenuClientRpc(string textEnd = "")
    {
        this.textEndGame.text = textEnd;
        this.endGameMenu.SetActive(true);
    }
    [ClientRpc]
    private void HideEndGameMenuClientRpc()
    {

        this.endGameMenu.SetActive(false);
    }

    public void Restart()
    {
        this.currentWave = 0;
        GameObject.FindGameObjectWithTag("Heart").GetComponent<Core>().ResetHealth();
        this.HideEndGameMenuClientRpc();
        this.ActivateActivatorClientRpc();
    }

    public void LoadLobby()
    {
        SceneManager.LoadScene("Lobby");
    }
}
