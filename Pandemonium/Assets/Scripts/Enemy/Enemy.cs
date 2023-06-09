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
        this.animator = GetComponent<Animator>();
        if (!IsServer) return;

        this.life = new EnemyLife(this, maxHealth);

        EnemyTrigger trigger = GetComponent<EnemyTrigger>();

        trigger.OnTriggerEnterEvent += this.life.TriggerDamage;

        this.position = transform.position;

        
        this.movement = new EnemyMovement(this, GetComponent<NavMeshAgent>());
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
        
        this.SetDeathClientRpc();
        StartCoroutine(DestroyBones());
    }

    private IEnumerator DestroyBones()
    {
        yield return new WaitForSeconds(2f);
        this.GetComponent<NetworkObject>().Despawn(true);
    }

    private void Update() {
        if (!IsServer) return;
        this.position = transform.position;
        this.movement.Move();
    }

    [ClientRpc]
    public void SetTriggerAttackClientRpc()
    {
        this.animator.SetTrigger("Attack");
    }

    [ClientRpc]
    public void SetSpeedWalkClientRpc(int speed)
    {
        this.animator.SetInteger("Speed", speed);
    }

    [ClientRpc]
    public void SetDeathClientRpc()
    {
        this.animator.SetTrigger("Death");
    }

}
