using UnityEngine;
using Unity.Netcode;
public class StartWaveTrigger : NetworkBehaviour
{
    public delegate void TriggerEventHandler();
    public event TriggerEventHandler OnTriggerEnterEvent;
    private void OnTriggerEnter(Collider other) 
    {
        if(!IsServer) return;

        if(other.gameObject.tag == "Hand")
        {
            OnTriggerEnterEvent?.Invoke();
        }
    }

    public void Display(bool display)
    {
        this.gameObject.SetActive(display);
    }
}
