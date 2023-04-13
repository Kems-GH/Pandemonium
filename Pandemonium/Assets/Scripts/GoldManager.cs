using Unity.Netcode;
using UnityEngine;

public class GoldManager : NetworkBehaviour
{
    private NetworkVariable<int> gold;

    public static GoldManager instance;

    private void Awake()
    {
        instance = this;
    }

    public int GetNbGold()
    {
        return gold.Value;
    }

    public void AddGold(int amount)
    {
        this.gold.Value += amount;
    }

    public void RemoveGold(int amount)
    {
        this.gold.Value -= amount;
    }
}
