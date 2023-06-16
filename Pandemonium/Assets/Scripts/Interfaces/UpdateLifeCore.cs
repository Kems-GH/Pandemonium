using UnityEngine;
using TMPro;

public class UpdateLifeCore : MonoBehaviour
{
    private TMP_Text lifeValue;
    private Core core;

    void Awake()
    {
        this.lifeValue = GetComponent<TMP_Text>();
        this.core = FindObjectOfType<Core>();

        if(core == null) {
            Debug.LogError("Core not found");
            Destroy(this);
        }
    }

    void Update()
    {        
        this.lifeValue.text = this.core.GetLife() + " %";
    }
}
