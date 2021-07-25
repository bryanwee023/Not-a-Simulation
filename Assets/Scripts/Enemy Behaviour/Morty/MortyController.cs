using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MortyController : EnemyController
{
    [SerializeField]
    private GameObject targetRune;
    [SerializeField]
    private GameObject mortyHit;

    private Vector3 targetPos;
    private GameObject currentRune;

    private float nextMove = 0;
    private Vector3[] directions = {Vector3.left, Vector3.right, Vector3.forward, Vector3.back };
    
    protected override void Move()
    {
        
        if (Time.time > nextMove)
        {
            nextMove = Time.time + 4;
            Vector3 dest;
            NavMeshHit hit;
            do
            {
                dest = this.transform.position + 20 * directions[Random.Range(0, 4)];
            } while (!NavMesh.SamplePosition(dest, out hit, 0.5f, NavMesh.AllAreas));

            this.agent.SetDestination(hit.position);
        }
    }
    
    protected override void InitAttack()
    {
        this.isAttacking = true;

        NavMeshHit hit;

        do targetPos = this.target.position + new Vector3(Random.Range(-20, 20), 0.33f, Random.Range(-20, 20));
        while (!NavMesh.SamplePosition(targetPos, out hit, 1f, NavMesh.AllAreas));

        this.animator.Attack();
        this.currentRune = Instantiate(targetRune, hit.position, Quaternion.Euler(90, 0, 0));
        Invoke("Hit", 0.5f);
    }

    private void Hit()
    {
        
        GameObject hit = Instantiate(mortyHit, targetPos + 40 * Vector3.up, mortyHit.transform.rotation);
        hit.GetComponent<MortyHit>().SetAttack(this.stats.attack);
        hit.GetComponent<MortyHit>().SetTargetRune(this.currentRune);

        EndAttack();
    }

    protected override bool CanAttack()
    {
        return true;
    }


}
