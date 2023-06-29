using System.Collections;
using UnityEngine;

public class ShopTeleport : MonoBehaviour
{
    [SerializeField] private bool isShopTeleport = false;
    [SerializeField] private bool isMapTeleport = false;

    private GameObject player;
    private GameObject playerSpawn;
    private GameObject shopSpawn;

    private string PLAYER_TAG = "SlideInteractor";
    private string SHOP_TELEPORT_TAG = "ShopSpawn";
    private string MAP_TELEPORT_TAG = "PlayerSpawn";

    private void Start()
    {
        playerSpawn = GameObject.FindGameObjectWithTag(MAP_TELEPORT_TAG);
        shopSpawn = GameObject.FindGameObjectWithTag(SHOP_TELEPORT_TAG);
        player = GameObject.FindGameObjectWithTag(PLAYER_TAG);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hand"))
        {
            if (isShopTeleport) player.transform.position = shopSpawn.transform.position;
            else if (isMapTeleport) player.transform.position = playerSpawn.transform.position;
        }
    }
}
