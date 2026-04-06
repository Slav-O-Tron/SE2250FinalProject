using UnityEngine;
using UnityEngine.AI;

public class ZombieMotion: EnemyMotion
{
     protected override void Update()
    {
        base.Update();

        if (Vector3.Distance(transform.position, target.position) < 2f)
        {
            Debug.Log("The zombie is groaning!");
        }

    }

}


