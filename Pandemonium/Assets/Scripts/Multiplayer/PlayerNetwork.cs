using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour {
    
    private NetworkVariable<int> randomNumber = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private void  Update() {
        Debug.Log(OwnerClientId + " ;" + randomNumber.Value);
        if (!IsOwner) return;

        if (Input.GetKeyDown(KeyCode.T)) {
            randomNumber.Value = Random.Range(1, 100);
        }

        Vector3 movDir = new Vector3(0, 0, 0);

        if(Input.GetKey(KeyCode.Z)) movDir.z = +1f;
        if(Input.GetKey(KeyCode.S)) movDir.z = -1f;
        if(Input.GetKey(KeyCode.Q)) movDir.x = -1f;
        if(Input.GetKey(KeyCode.D)) movDir.x = +1f;

        float speed = 3f;
        transform.Translate(movDir * speed * Time.deltaTime);

    }
}
