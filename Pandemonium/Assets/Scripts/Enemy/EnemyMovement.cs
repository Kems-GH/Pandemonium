using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement 
{
    private LayerMask layerMaskPlayer;
    private Enemy enemy;

    private Collider[] collidersPlayer;
    private NavMeshAgent navAgent;
    private Core core;
    private bool isChasingPlayer = false;
    private bool isAttacking = false;
    private float timeLastAttack = 0f;

    public EnemyMovement(Enemy enemy, NavMeshAgent navAgent)
    {
        this.enemy = enemy;
        this.core = GameObject.FindGameObjectWithTag("Core").GetComponent<Core>();
        this.layerMaskPlayer = LayerMask.GetMask("Player");
        this.navAgent = navAgent;
        this.navAgent.speed = enemy.speed;
        GoToHeart();
    }

    public void Move()
    {
        if(timeLastAttack + enemy.speedAttack < Time.time)
        {
            this.isAttacking = false;
        }
        
        if(!enemy.ignorePlayer) CheckNearPlayer();

        CheckNearHeart();

        if (!isChasingPlayer)
        {
            GoToHeart();
        }
    }

    /**
     * Will chase after the player
     */
    private void ChasePlayer(Vector3 player)
    {   
        navAgent.SetDestination(player);
    }

    /**
     * Check if the Enemy is near a player
     */
    private void CheckNearPlayer()
    {
        collidersPlayer = Physics.OverlapSphere(this.enemy.position, this.enemy.radiusAggro, layerMaskPlayer);

        if (collidersPlayer.Length > 0)
        {
            isChasingPlayer = true;
            // Will chase the nearest player
            ChasePlayer(collidersPlayer[0].transform.position);
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
        if (isChasingPlayer) return;

        if (Vector3.Distance(this.core.transform.position, this.enemy.position) < this.enemy.distanceNearHeart)
        {
            AttackHeart();
        }
    }

    /**
     * The Enemy will inflict damage to the heart
     */
    private void AttackHeart()
    {
        if(isAttacking) return;
        this.isAttacking = true;
        this.timeLastAttack = Time.time;
        this.enemy.SetTriggerAttackClientRpc();
        this.core.TakeDamage(this.enemy.damageInflicted);
        this.StopMovement();
    }

    /**
     * The Enemy will go in the direction of the Heart
     */
    private void GoToHeart()
    {
        this.navAgent.isStopped = false;
        this.enemy.SetSpeedWalkClientRpc(this.enemy.speed);
        navAgent.SetDestination(this.core.GetPosition());
    }

    public void StopMovement()
    {
        this.enemy.SetSpeedWalkClientRpc(0);
        navAgent.isStopped = true;
    }
}
