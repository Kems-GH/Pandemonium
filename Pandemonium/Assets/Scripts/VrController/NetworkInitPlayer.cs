using UnityEngine;
using Unity.Netcode;
public class NetworkInitPlayer : NetworkBehaviour
{
    private void Awake() {
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start() {

        if (IsOwner) return;
        
        // Disable all components on the player object
        // except for the NetworkObject and NetworkTransform
        foreach (var component in GetComponentsInChildren<MonoBehaviour>())
        {
            if (component is NetworkBehaviour)
            {
                continue;
            }
            component.enabled = false;
            
        }

    }
}
