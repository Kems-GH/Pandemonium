using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class GrabEvent : NetworkBehaviour
{

    public void Grab()
    {
        ChangeOwnershipServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    public void ChangeOwnershipServerRpc(ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;
        this.gameObject.GetComponent<NetworkObject>().ChangeOwnership(clientId);
    }
}
