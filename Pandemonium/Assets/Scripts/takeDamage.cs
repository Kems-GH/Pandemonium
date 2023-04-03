using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class takeDamage : NetworkBehaviour
{
    [SerializeField] private int health = 100;

    [SerializeField] private Slider slider;
    private bool canTakeDamage = true;

    private void Start()
    {
        slider.value = health;
    }

    void OnTriggerEnter(Collider collision)
    {
        if (!IsServer) return;

        if(collision.transform.CompareTag("Trap") && canTakeDamage)
        {
            TakeDamage(50);
        }
        if(collision.transform.CompareTag("Heart"))
        {
            TakeDamage(100);
        }
        if (collision.transform.CompareTag("Hand") && canTakeDamage)
        {
            TakeDamage(50);
        }

    }

    private void TakeDamage(int damage)
    {
        health -= damage;
        
        slider.value = health;
        this.canTakeDamage = false;

        if (health <= 0)
        {
            this.GetComponent<NetworkObject>().Despawn(true);
        }
        StartCoroutine(ChangeTakeDamage());
    }

    IEnumerator ChangeTakeDamage()
    {
        yield return new WaitForSeconds(0.5f);
        this.canTakeDamage = true;
    }
}
