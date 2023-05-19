using System.Collections;
using Unity.Netcode;
using UnityEngine;
[System.Serializable]
public class Spawner : NetworkBehaviour
{
    private const float rangeSpawn = 1f;

    [SerializeField] private ParticleSystem spawnEffect;
    public void Spawn(GameObject enemyPrefab)
    {
        if (!IsServer) return;

        Vector3 randPos = Random.insideUnitSphere * rangeSpawn;
        // We create a new Vector3 to keep the y value
        Vector3 pos = new Vector3(randPos.x + transform.position.x, transform.position.y, randPos.z + transform.position.z);

        GameObject enemyGameObject = Instantiate(enemyPrefab, pos, transform.rotation);
        if(IsServer) enemyGameObject.GetComponent<NetworkObject>().Spawn(true);
        
    }
    [ClientRpc]
    public void ActivateClientRpc()
    {
        spawnEffect.Play();
    }
    [ClientRpc]
    public void DeactivateClientRpc()
    {
        spawnEffect.Stop();
    }
}
