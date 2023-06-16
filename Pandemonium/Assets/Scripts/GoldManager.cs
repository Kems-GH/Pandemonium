using Unity.Netcode;
using UnityEngine;

public class GoldManager : NetworkBehaviour
{
    private NetworkVariable<int> gold = new NetworkVariable<int>(0);

    public static GoldManager instance;

    private void Awake()
    {
        instance = this;
    }

    public int GetNbGold()
    {
        return gold.Value;
    }

    [ServerRpc(RequireOwnership = false)]
    public void AddGoldServerRpc(int amount)
    {
        this.gold.Value += amount;
    }

    [ServerRpc(RequireOwnership = false)]
    public void RemoveGoldServerRpc(int amount)
    {
        this.gold.Value -= amount;
    }
}
