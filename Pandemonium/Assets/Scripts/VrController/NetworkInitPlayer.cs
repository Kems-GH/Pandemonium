using UnityEngine;
using Unity.Netcode;
public class NetworkInitPlayer : NetworkBehaviour
{
    private void Start() {

        if (IsOwner) return;
        
        // Disable all components on the player object
        // except for the NetworkObject and NetworkTransform
        foreach (var component in GetComponentsInChildren<MonoBehaviour>())
        {
            Debug.Log("toDisabel : " + component.GetType().Name);

            if (component is NetworkBehaviour)
            {
                continue;
            }
            component.enabled = false;
            
        }

        foreach (var component in GetComponentsInChildren<Camera>())
        {
            Debug.Log("toDisabel : " + component.GetType().Name);

            component.enabled = false;

        }

        foreach (var component in GetComponentsInChildren<AudioListener>())
        {
            Debug.Log("toDisabel : " + component.GetType().Name);

            Destroy(component);

        }
    }
}
