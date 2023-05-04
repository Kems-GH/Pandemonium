using UnityEngine;
using Unity.Netcode;
using UnityEngine.AI;
using System.Collections;

public class Enemy : NetworkBehaviour
{
    [SerializeField] private LayerMask layerMaskPlayer;
    [SerializeField] private int damageInflicted;
    [SerializeField] private int radiusAggro;
    [SerializeField] private Animator animator;

    private Collider[] collidersPlayer;
    private NavMeshAgent navAgent;
    private Transform heart;
    private Core core;
    private bool isChasingPlayer = false;

    private bool canTakeDamage = true;

    [SerializeField] protected  NetworkVariable<int> health;
    protected virtual float distanceNearHeart { get; } = 2.1f;
    protected virtual float speedAttack { get; } = 3f;
    protected virtual float timeForFirstAttack { get; } = 0.1f;
    protected virtual int goldEarnedAfterDeath { get; } = 0;

    private void OnTriggerEnter(Collider collider)
    {
        if (!IsServer && !GameManager.Instance.IsSolo()) return;

        int amountDamage = 0;

        if (collider.CompareTag("Hand"))
        {
            amountDamage = (int)collider.gameObject.GetComponent<IWeapon>().GetAmountDamage();
        } else if(collider.CompareTag("Trap"))
        {
            amountDamage = (int)collider.gameObject.GetComponent<ITrap>().GetAmountDamage();
        } else {
            return;
        }

        this.TakeDamage(amountDamage);
    }

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

    private void Start() {
        if (!IsServer && !GameManager.Instance.IsSolo()) return;

        heart = GameObject.FindGameObjectWithTag("Heart").transform;
        navAgent = GetComponent<NavMeshAgent>();
        this.health.Value = 100;

        StartCoroutine(GoToHeart());
    }

    /**
     * The Enemy take some damage
     */
    public void TakeDamage(int damage)
    {   
        if(!this.canTakeDamage) return;

        Debug.Log(this + ": " + damage );
        this.health.Value -= damage;

        this.canTakeDamage  = false;
        StartCoroutine(Invulnerable());
        
        if(health.Value <= 0)
        {
            Die();
        }
    }

    private IEnumerator Invulnerable()
    {
        yield return new WaitForSeconds(0.5f);
        this.canTakeDamage = true;
    }

    /**
     * The Enemy die
     */
    private void Die()
    {
        if (!IsServer && !GameManager.Instance.IsSolo()) return;
        GoldManager.instance.AddGoldServerRpc(goldEarnedAfterDeath);
        StopAllCoroutines();
        CancelInvoke();
        navAgent.isStopped = true;
        
        this.animator.SetTrigger("Death");
        StartCoroutine(DestroyBones());
    }

    private IEnumerator DestroyBones()
    {
        yield return new WaitForSeconds(2f);
        if (IsServer) this.GetComponent<NetworkObject>().Despawn(true);
        else Destroy(this.gameObject);
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
            this.core = this.heart.GetComponent<Core>();

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
        this.animator.SetBool("IsAttacking", true);
        this.core.TakeDamage(damageInflicted);
    }

    /**
     * The Enemy will go in the direction of the Heart
     */
    private IEnumerator GoToHeart()
    {
        yield return new WaitForSeconds(1f);
        this.animator.SetInteger("Speed", 15);
        navAgent.SetDestination(this.heart.transform.position);
    }
}
