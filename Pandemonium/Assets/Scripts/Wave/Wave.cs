using UnityEngine;

[System.Serializable] 
public class Wave
{
    [Tooltip("The number of enemy to spawn by spawner")]
    public int nbEnemy = 10;
    [Tooltip("The time between each spawn")]
    public float spawnRate = 1f;
    [Tooltip("The number of spawner to activate for this wave")]
    public int nbSpawner = 1;
    [Tooltip("If true, the boos will spawn at the start of the wave")]
    public bool isBossWave = false;
    [Tooltip("The minimum number of enemy to spawn by batch")]
    public int minNbSpawn = 3;
    [Tooltip("The maximum number of enemy to spawn by batch")]
    public int maxNbSpawn = 6;
    [Tooltip("The probability to spawn a normal skeleton")]
    [Range(0, 100)]
    public int skeletonProba = 100;
    [Tooltip("The probability to spawn a normal boss")]
    [Range(0, 100)]
    public int bossProba = 100;

}
