using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class EnemyTakeDamage : NetworkBehaviour
{
    [SerializeField] private NetworkVariable<int> health = new NetworkVariable<int>(100);

    [SerializeField] private Slider slider;
    private bool canTakeDamage = true;

    private void Start()
    {
        UpdateSlider();
    }

    public override void OnNetworkSpawn()
    {
        health.OnValueChanged += (int oldValue, int newValue) => { UpdateSlider(); };
    }

    void OnTriggerEnter(Collider collision)
    {
        if (!IsServer && !GameManager.Instance.IsSolo()) return;

        if(collision.transform.CompareTag("Trap") && canTakeDamage)
        {
            TakeDamage(50);
        }

        if (collision.transform.CompareTag("Heart"))
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
        health.Value -= damage;

        UpdateSlider();
        this.canTakeDamage = false;

        if (health.Value <= 0)
        {
            Die();
        }
        StartCoroutine(ChangeTakeDamage());
    }

    void Die()
    {
        GameManager.Instance.RemoveEnemy();
        this.GetComponent<NetworkObject>().Despawn(true);
    }

    void UpdateSlider()
    {
        slider.value = health.Value;
    }

    IEnumerator ChangeTakeDamage()
    {
        yield return new WaitForSeconds(0.5f);
        this.canTakeDamage = true;
    }
}
