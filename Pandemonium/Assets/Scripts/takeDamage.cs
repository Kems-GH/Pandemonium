using System.Collections;
using UnityEngine;

public class takeDamage : MonoBehaviour
{
    [SerializeField]
    private int health = 100;
    private bool canTakeDamage = true;

    private void OnCollisionStay(Collision collision)
    {
        Debug.Log("Stay");
    }

    private void OnCollisionExit(Collision collision)
    {
        Debug.Log("Exit");
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Enter");
        if (collision.transform.CompareTag("Hand") && canTakeDamage)
        {
            health -= 100;
            this.canTakeDamage = false;

            if(health <= 0)
            {
                Destroy(this.gameObject);
            }
            StartCoroutine(ChangeTakeDamage());
        }
    }

    IEnumerator ChangeTakeDamage()
    {
        yield return new WaitForSeconds(1f);
        this.canTakeDamage = true;
    }
}
