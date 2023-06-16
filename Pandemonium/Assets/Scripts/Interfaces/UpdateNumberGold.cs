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

        if(manager == null)
        {
            Debug.LogError("GoldManager not found");
            Destroy(this);
        }
    
    }

    void Update()
    {
        this.numberGold.text = manager.GetNbGold().ToString();
    }
}
