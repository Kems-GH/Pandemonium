using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartHost : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        if (!other.gameObject.CompareTag("Hand"))
        {
            return;
        }
        // load the scene
        MultiplayerParameters.isHost = true;
        UnityEngine.SceneManagement.SceneManager.LoadScene("DemoScene");
    }
}
