using UnityEngine;
using Unity.Netcode;
using UnityEngine.AI;
using System.Collections;

public abstract class Enemy : NetworkBehaviour
{
    private Animator animator;
    private EnemyLife life;
    private EnemyMovement movement;
    public Vector3 position { get; private set; }

    /**
     * Stats of the enemy
     * Can be overrided by the child class
     */
    public abstract int maxHealth { get; }
    public abstract float distanceNearHeart { get; }
    public abstract float speedAttack { get; }
    public abstract float timeForFirstAttack { get; }
    public abstract int goldEarnedAfterDeath { get; }
    public abstract int damageInflicted { get; }
    public abstract int radiusAggro { get;}
    public abstract int speed { get;}

    private void Start() {
        if (!IsServer) return;

        this.life = new EnemyLife(this, maxHealth);

        EnemyTrigger trigger = GetComponent<EnemyTrigger>();
        Debug.Log("EnemyTrigger : " + trigger);
        trigger.OnTriggerEnterEvent += this.life.TriggerDamage;

        this.position = transform.position;

        this.animator = GetComponent<Animator>();
        this.movement = new EnemyMovement(this, this.animator, GetComponent<NavMeshAgent>());
    }


    /**
     * The Enemy die
     */
    public void Die()
    {
        if (!IsServer) return;
        GoldManager.instance.AddGoldServerRpc(goldEarnedAfterDeath);
        StopAllCoroutines();
        CancelInvoke();
        this.movement.StopMovement();
        
        this.animator.SetTrigger("Death");
        StartCoroutine(DestroyBones());
    }

    private IEnumerator DestroyBones()
    {
        yield return new WaitForSeconds(2f);
        if (IsServer) this.GetComponent<NetworkObject>().Despawn(true);
        else Destroy(this.gameObject);
    }

    private void Update() {
        if (!IsServer) return;
        this.position = transform.position;
        this.movement.Move();
    }
    
}
