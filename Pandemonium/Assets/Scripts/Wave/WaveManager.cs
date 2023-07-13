using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class WaveManager : NetworkBehaviour
{
    [SerializeField] private List<Spawner> spawners;
    [SerializeField] private List<Wave> waves;
    [SerializeField] private GameObject endGameMenu;
    [SerializeField] private TMPro.TMP_Text textEndGame;
    [field:SerializeField] public NetworkVariable<bool> isWaveRunning { get; set; } = new NetworkVariable<bool>(false);

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
        isWaveRunning.Value = true;
        this.DisplayActivatorClientRpc(false);
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
        if(!isWaveRunning.Value) return;
        if(WaveChecker.CheckWaveFinished())
        {
            EndWave();
            return;
        }
    }

    private void EndWave()
    {
        if (!IsServer) return;
        isWaveRunning.Value = false;
        StopAllCoroutines();
        CancelInvoke(nameof(CheckWaveFinished));

        foreach (Spawner spawner in spawners)
        {
            spawner.DeactivateClientRpc();
        }
        if (currentWave < nbWave) {
            this.DisplayActivatorClientRpc(true);
        }
        else {
            this.Victory();
        }
    }

    public void StopWave()
    {
        if (!IsServer) return;

        isWaveRunning.Value = false;

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

    [ServerRpc (RequireOwnership = false)]
    public void RestartServerRpc()
    {
        this.currentWave = 0;
        GameObject.FindGameObjectWithTag("Core").GetComponent<Core>().ResetHealth();
        this.HideEndGameMenuClientRpc();
        this.DisplayActivatorClientRpc(true);
    }

    public void LoadLobby()
    {
        if(IsServer) NetworkManager.SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
    }

    public bool IsWaveRunning()
    {
        return isWaveRunning.Value;
    }

    [ClientRpc]
    public void DisplayActivatorClientRpc(bool display)
    {
        this.startWaveTrigger.Display(display);
    }
}
