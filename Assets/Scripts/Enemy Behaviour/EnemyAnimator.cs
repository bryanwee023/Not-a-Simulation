using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAnimator : MonoBehaviour
{
    protected NavMeshAgent agent;
    protected Animator animator;

    [HideInInspector]
    public bool isDead = false;
    private bool hasDissolver;

    private void Start()
    {
        this.agent = this.transform.parent.GetComponent<NavMeshAgent>();
        this.animator = this.GetComponentInChildren<Animator>();
    }

    public virtual void Attack()
    {
        this.animator.SetTrigger("Attack");
    }

    public virtual void Die()
    {
        this.Unflash();
        this.animator.SetTrigger("Death");

        if (hasDissolver)
            Invoke("Despawn", 1f);
    }

    public void Stagger()
    {
        this.animator.SetTrigger("Stagger");
    }

    public void Spawn()
    {
        this.gameObject.AddComponent<Dissolver>();
        this.hasDissolver = true;
    }

    private void Despawn()
    {
        this.gameObject.GetComponent<Dissolver>().Dissolve();
    }

    public void FlashWhite()
    {
        foreach (SkinnedMeshRenderer r in this.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            r.material.SetFloat("_HighIntensity", 3.5f);
            r.material.SetFloat("_LowIntensity", 3.5f);
        }
        Invoke("Unflash", 0.1f);
    }

    public void Unflash()
    {
        foreach (SkinnedMeshRenderer r in this.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            r.material.SetFloat("_HighIntensity", 1.5f);
            r.material.SetFloat("_LowIntensity", 1.0f);
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (isDead) return;

        float normalisedSpeed = this.agent.velocity.magnitude / this.agent.speed;
        if (agent.speed > 0 || agent.angularSpeed < 0)
            this.animator.SetFloat("Motion", normalisedSpeed);
    }
}
