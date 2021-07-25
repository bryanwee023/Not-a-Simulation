using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandyAnimator : EnemyAnimator
{
    [SerializeField]
    private GameObject spinParticles;
    private GameObject currentParticles;

    public void StartSpin()
    {
        this.currentParticles = Instantiate(this.spinParticles, this.transform);
    }

    public void EndSpin()
    {
        if (this.currentParticles != null)
        {
            this.currentParticles.GetComponentInChildren<ParticleSystem>().Clear();
            Destroy(this.currentParticles);
            this.currentParticles = null;
        }
        this.animator.SetTrigger("Idle");
    }

    public void Fire()
    {
       this.animator.SetTrigger("Fire");
    }

    public void EndFire()
    {
        this.animator.SetTrigger("Idle");
    }

    /*
    public void Dizzy()
    {
        Destroy(this.currentParticles);
        this.animator.SetTrigger("Dizzy");

    }

    public void Undizzy()
    {
        this.animator.SetTrigger("Undizzy");
    }
    */
    public override void Die()
    {
        this.EndSpin();
        base.Die();
    }
}
