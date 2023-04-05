using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using Unity.Netcode;

public class EnemyMovement : NetworkBehaviour
{
    [SerializeField] private LayerMask layerMaskPlayer;
    [SerializeField] private LayerMask layerMaskHeart;

    private Collider[] collidersPlayer;
    private NavMeshAgent agent;
    private Transform heart;
    private HeartHealth heartHealth;
    private bool isNearHeart = false;

    public override void OnNetworkSpawn()
    {
        heart = GameObject.FindGameObjectWithTag("Heart").transform;
        agent = GetComponent<NavMeshAgent>();

        StartCoroutine(GoToHeart());
    }

    void Update()
    {
        if(!IsServer && !GameManager.Instance.IsSolo()) return;
        
        collidersPlayer = Physics.OverlapSphere(this.transform.position, 5, layerMaskPlayer);

        if (collidersPlayer.Length > 0)
        {
            ChasePlayer(collidersPlayer[0]);
        }
        else
        {
            StartCoroutine(GoToHeart());
        }

        if (Vector3.Distance(this.heart.transform.position, this.transform.position) < 2.1f)
        {
            Debug.Log("Near Heart");
            this.heartHealth = this.heart.GetComponent<HeartHealth>();
            
            if(!IsInvoking(nameof(AttackHeart)))
            {
                InvokeRepeating(nameof(AttackHeart), 0.1f, 3f);
            }
        }
        else
        {
            isNearHeart = false;
        }
    }

    void ChasePlayer(Collider player)
    {
        agent.SetDestination(player.transform.position);
    }

    void AttackHeart()
    {
        Debug.Log("Attack Heart");
        this.heartHealth.TakeDamage(5);
    }


    IEnumerator GoToHeart()
    {
        yield return new WaitForSeconds(1f);
        agent.SetDestination(this.heart.transform.position);
    }
}
