using UnityEngine;
using UnityEngine.AI;

public class ZombieMotion: MonoBehaviour
{
    public Transform target;
    public NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if(target != null)
        {
            agent.SetDestination(target.position);
        }
    }
}


