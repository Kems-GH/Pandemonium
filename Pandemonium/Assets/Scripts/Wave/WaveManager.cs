using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class WaveManager : NetworkBehaviour
{
    [SerializeField] private List<Spawner> spawners;
    [SerializeField] private List<Wave> waves;
    [SerializeField] private GameObject activator;
    [SerializeField] private GameObject endGameMenu;
    [SerializeField] private TMPro.TMP_Text textEndGame;
    [field:SerializeField] public bool isWaveRunning { get; set; } = false;

    private int currentWave = 0;
    private int nbWave = 0;
    private StartWaveTrigger startWaveTrigger;

    private void Awake() 
    {
        this.nbWave = waves.Count;
        startWaveTrigger = GetComponentInChildren<StartWaveTrigger>();
        startWaveTrigger.OnTriggerEnterEvent += StartWave;
    }

    public void StartWave()
    {
        if (!IsServer) return;
        isWaveRunning = true;
        startWaveTrigger.DeactivateClientRpc();
        if (currentWave < nbWave)
        {
            Wave wave = waves[currentWave];
            StartCoroutine(SpawnManager.SpawnEnemy(wave, spawners));
            currentWave++;
            InvokeRepeating(nameof(CheckWaveFinished), 5f, 1f);
        }
    }

    private void CheckWaveFinished()
    {
        if(!isWaveRunning) return;
        if(WaveChecker.CheckWaveFinished())
        {
            EndWave();
            return;
        }
    }

    private void EndWave()
    {
        if (!IsServer) return;
        isWaveRunning = false;

        foreach (Spawner spawner in spawners)
        {
            spawner.DeactivateClientRpc();
        }
        if (currentWave < nbWave) {
            startWaveTrigger.ActivateClientRpc();
        }
        else {
            this.Victory();
        }
    }

    public void StopWave()
    {
        if (!IsServer) return;

        isWaveRunning = false;

        StopAllCoroutines();
        CancelInvoke(nameof(CheckWaveFinished));
        foreach (Spawner spawner in spawners)
        {
            spawner.DeactivateClientRpc();
        }
    }

    public void Defeat()
    {
        if (!IsServer) return;
        this.StopWave();

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            enemy.GetComponent<NetworkObject>().Despawn(true);
        }

        DisplayEndGameMenuClientRpc("Game Over");
    }

    public void Victory()
    {
        if (!IsServer) return;

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
        startWaveTrigger.ActivateClientRpc();
    }

    public void LoadLobby()
    {
        SceneManager.LoadScene("Lobby");
    }
}
