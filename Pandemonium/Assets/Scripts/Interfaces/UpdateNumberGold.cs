using UnityEngine;
using TMPro;

public class UpdateNumberGold : MonoBehaviour
{
    private TMP_Text numberGold;

    void Awake()
    {
        this.numberGold = GetComponent<TMP_Text>();
    }

    void Update()
    {
        this.numberGold.text = GoldManager.instance.GetNbGold().ToString();
    }
}
