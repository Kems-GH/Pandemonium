using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using Unity.Netcode;

public class EnemyMovement : NetworkBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    private Transform heart;
    private bool BegunPath = false;

    public override void OnNetworkSpawn()
    {
        heart = GameObject.FindGameObjectWithTag("Heart").transform;

        StartCoroutine(AgentMovement());
    }

    private void Update()
    {
        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if ((!agent.hasPath || agent.velocity.sqrMagnitude <= 0.5f) && BegunPath)
                {
                    Destroy(this.gameObject);
                }
            }
        }
    }

    IEnumerator AgentMovement()
    {
        yield return new WaitForSeconds(1f);
        Vector3 newPos = new Vector3(this.heart.position.x, this.transform.position.y, this.heart.position.z);
        agent.SetDestination(newPos);
        BegunPath = true;
    }
}
