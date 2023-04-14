using UnityEngine;
using Unity.Netcode;
using UnityEngine.AI;
using System.Collections;

public class Enemy : NetworkBehaviour
{
    [SerializeField] private LayerMask layerMaskPlayer;
    [SerializeField] private int damageInflicted;
    [SerializeField] private int radiusAggro;

    private Collider[] collidersPlayer;
    private NavMeshAgent navAgent;
    private Transform heart;
    private HeartHealth heartHealth;
    private bool isChasingPlayer = false;

    protected NetworkVariable<int> health = new NetworkVariable<int>(100);
    protected float distanceNearHeart = 2.1f;
    protected float speedAttack = 3f;
    protected float timeForFirstAttack = 0.1f;
    protected int goldEarnedAfterDeath = 0;

    private void Update()
    {
        CheckNearPlayer();

        if (!IsServer && !GameManager.Instance.IsSolo()) return;
        CheckNearHeart();

        if (!isChasingPlayer)
        {
            StartCoroutine(GoToHeart());
        }
    }

    public override void OnNetworkSpawn()
    {
        heart = GameObject.FindGameObjectWithTag("Heart").transform;
        navAgent = GetComponent<NavMeshAgent>();

        StartCoroutine(GoToHeart());
    }

    /**
     * The Enemy take some damage
     */
    public void TakeDamage(int damage)
    {
        this.health.Value -= damage;

        if(health.Value <= 0)
        {
            Die();
        }
    }

    /**
     * The Enemy die
     */
    private void Die()
    {
        GameManager.Instance.RemoveEnemy();
        GoldManager.instance.AddGold(goldEarnedAfterDeath);
        this.GetComponent<NetworkObject>().Despawn(true);
    }

    /**
     * Will chase after the player
     */
    [ServerRpc(RequireOwnership = false)]
    private void ChasePlayerServerRpc(Vector3 player)
    {
        navAgent.SetDestination(player);
    }

    /**
     * Check if the Enemy is near a player
     */
    private void CheckNearPlayer()
    {
        collidersPlayer = Physics.OverlapSphere(this.transform.position, radiusAggro, layerMaskPlayer);

        if (collidersPlayer.Length > 0)
        {
            isChasingPlayer = true;
            // Will chase the nearest player
            ChasePlayerServerRpc(collidersPlayer[0].transform.position);
        }
        else
        {
            isChasingPlayer = false;
        }
    }

    /**
     * Check if the Enemy is near the heart
     */
    private void CheckNearHeart()
    {
        if (isChasingPlayer)
        {
            if(IsInvoking(nameof(AttackHeart)))
            {
                CancelInvoke(nameof(AttackHeart));
            }

            return;
        }


        if (Vector3.Distance(this.heart.transform.position, this.transform.position) < distanceNearHeart)
        {
            this.heartHealth = this.heart.GetComponent<HeartHealth>();

            if (!IsInvoking(nameof(AttackHeart)))
            {
                InvokeRepeating(nameof(AttackHeart), timeForFirstAttack, speedAttack);
            }
        }
    }

    /**
     * The Enemy will inflict damage to the heart
     */
    private void AttackHeart()
    {
        if (!IsServer && !GameManager.Instance.IsSolo()) return;

        this.heartHealth.TakeDamage(damageInflicted);
    }

    /**
     * The Enemy will go in the direction of the Heart
     */
    private IEnumerator GoToHeart()
    {
        yield return new WaitForSeconds(1f);
        navAgent.SetDestination(this.heart.transform.position);
    }
}
