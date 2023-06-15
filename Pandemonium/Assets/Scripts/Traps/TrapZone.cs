using UnityEngine;

public class TrapZone : MonoBehaviour
{
    public const string TAG = "Placement Zone";
    public bool isFree { get; set; } = true;
    public GameObject visual;

    private void Awake()
    {
        this.visual = this.transform.GetChild(0).gameObject;
        this.visual.SetActive(false);
    }

    public void setVisible(bool visible)
    {
        this.visual.SetActive(visible);
    }
}
