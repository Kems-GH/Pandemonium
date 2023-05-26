using UnityEngine;

public class TrapZone : MonoBehaviour
{
    public const string TAG = "Placement Zone";
    public bool isFree { get; set; } = true;

    private void Awake() {
        this.gameObject.SetActive(false);
    }
}
