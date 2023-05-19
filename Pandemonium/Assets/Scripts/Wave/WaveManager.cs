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
    [SerializeField] private bool isActiveWave = true;

    private int currentWave = 0;
    private int nbWave = 0;
    private StartWaveTrigger startWaveTrigger;
    public static WaveManager Instance;

    private void Awake() 
    {
        // TODO: Change if we have other level
        if (Instance == null) Instance = this;
        else 
        {
            Debug.LogError("Multiple WaveManager in scene");
            Destroy(this.gameObject);
            return;
        }

        this.nbWave = waves.Count;
    }

    private void Start() {
        if (!IsServer) return;
        startWaveTrigger = GetComponent<StartWaveTrigger>();
        startWaveTrigger.OnTiriggerEnter += StartWave;
    }

    public void StartWave()
    {
        if (!IsServer) return;
        isActiveWave = true;
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
        if(!isActiveWave) return;
        if(WaveChecker.CheckWaveFinished())
        {
            EndWave();
            return;
        }
    }

    private void EndWave()
    {
        if (!IsServer) return;
        isActiveWave = false;

        foreach (Spawner spawner in spawners)
        {
            spawner.DeactivateClientRpc();
        }
        if (currentWave < nbWave)
        {
            startWaveTrigger.ActivateClientRpc();
        }
        else
        {
            this.Victory();
        }
    }

    public void StopWave()
    {
        if (!IsServer) return;

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
