using UnityEngine;

public class StartClient : MonoBehaviour
{
    private void OnCollisionEnter(Collision other) {
        if (!other.gameObject.CompareTag("Hand")) {
            return;
        }

        // load the scene
        MultiplayerParameters.isClient = true;
        UnityEngine.SceneManagement.SceneManager.LoadScene("DemoScene");

    }
}
