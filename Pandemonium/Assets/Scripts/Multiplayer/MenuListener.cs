using UnityEngine;

public class MenuListener : MonoBehaviour
{
    [SerializeField] private bool isButtonHost = false;
    [SerializeField] private bool isButtonJoin = false;
    [SerializeField] private bool isButtonCancel = false;
    [SerializeField] private SessionUI sessionUI;

    private void OnTriggerEnter(Collider other) {
        if(isButtonHost) sessionUI.displayJoinCode();
        if(isButtonJoin) sessionUI.EnterCode();
        if(isButtonCancel) SessionManager.Instance.CancelSession();
    }
}
