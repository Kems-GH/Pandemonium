using UnityEngine;
using TMPro;

public class UpdateNumberGold : MonoBehaviour
{
    private TMP_Text numberGold;
    private GoldManager manager;

    void Awake()
    {
        this.numberGold = GetComponent<TMP_Text>();
        manager = FindObjectOfType<GoldManager>();
    }

    void Update()
    {
        if(!manager) return;
        
        this.numberGold.text = manager.GetNbGold().ToString();
    }
}
