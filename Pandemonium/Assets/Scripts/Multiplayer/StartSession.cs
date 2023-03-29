using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class StartSession : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if(MultiplayerParameters.isHost) NetworkManager.Singleton.StartHost();
        if(MultiplayerParameters.isClient) NetworkManager.Singleton.StartClient();
    }

}
