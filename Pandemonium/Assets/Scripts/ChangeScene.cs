using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using UnityEngine;

public class ChangeScene : NetworkBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Hand")
        {
            NetworkManager.SceneManager.LoadScene("Tutorial", LoadSceneMode.Single);
        }
    }
}
