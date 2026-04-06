using UnityEngine;
using UnityEngine.AI;

public class EnemyMotion : MonoBehaviour
{
    public Transform target;
    public NavMeshAgent agent;

    protected virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    protected virtual void Update()
    {
        if (target != null)
        {
            agent.SetDestination(target.position);
        }
    }
}



