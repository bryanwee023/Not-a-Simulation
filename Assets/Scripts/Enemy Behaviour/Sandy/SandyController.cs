using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SandyController : EnemyController
{
    private const float PRE_SPIN_TIME = 0.6f;
    private const float PRE_FIRE_TIME = 1.2f;

    [Header("Spin Attack")]
    public float attackVelocity;    //Linear velocity when Sandy spins
    public float spinTime;          //Duration in which Sandy spins for

    [Header("Laser")]
    public bool laserVariant;
    public float laserDamage;
    public float fireTime;
    public GameObject laser;
    public Transform eye;

    private Vector3 attackDirection;
    private Vector3 laserTarget;
    private float laserDistance;

    private bool isSpinning = false;
    private bool isFiring = false;
    private bool hit = false;

    private GameObject currentLaser;

    private int nextMove = 0;

    protected override void InitAttack()
    {
        this.isAttacking = true;

        //Stop Movement (Messes with spin animation)
        this.agent.ResetPath();
        this.agent.velocity = Vector3.zero;

        if (nextMove == 0 || !this.laserVariant)
        {
            this.animator.Attack();

            this.attackDirection = (this.target.position - this.transform.position).normalized;
            Invoke("Spin", PRE_SPIN_TIME);
            nextQueuedAttack = Time.time + PRE_SPIN_TIME;

        } else
        {
            ((SandyAnimator)this.animator).Fire();
            Invoke("Fire", PRE_FIRE_TIME);
            nextQueuedAttack = Time.time + PRE_FIRE_TIME;
        }

        nextMove = (nextMove + 1) % 2;
    }

    protected override void Attack()
    {
        if (this.isSpinning)
        {
            this.transform.Rotate(new Vector3(0, 720 * Time.deltaTime, 0));
            this.transform.position += (this.attackDirection * attackVelocity * Time.deltaTime);

            if (PlayerWithinReach() && !hit)
            {
                hit = true;
                PlayerController.TakeDamage(this.stats.attack);
            }
        } else if (this.isFiring)
        {
            this.FaceTarget(0.05f);

            Vector3 aimDir = Vector3.Scale(this.transform.forward, new Vector3(1, 0, 1));
            Vector3 dest = this.transform.position + this.laserDistance * aimDir;
            dest.y = 0;
            this.currentLaser.transform.LookAt(dest);

            this.laserDistance += 9 * Time.deltaTime;
        }
    }

    protected override void Move()
    {
        this.FaceTarget();
        base.Move();
    }

    private void Spin()
    {
        if (Time.time < staggeredUntil || isDead)
        {
            EndAttack();
            return;
        }

        this.canStagger = false;
        this.isSpinning = true;
        this.hit = false;
        ((SandyAnimator)this.animator).StartSpin();
        Invoke("EndAttack", spinTime);
    }

    private void Fire()
    {
        if (Time.time < staggeredUntil || isDead)
        {
            EndAttack();
            return;
        }

        this.canStagger = false;
        this.isFiring = true;
        this.currentLaser = Instantiate(this.laser, this.eye);
        this.currentLaser.GetComponent<SandyLaser>().SetAttack(10);

        this.laserDistance = 30;

        Invoke("EndAttack", fireTime);
    }

    protected override void EndAttack()
    {
        this.canStagger = true;

        if (isSpinning)
        {
            this.isSpinning = false;
            ((SandyAnimator)this.animator).EndSpin();

        } else if (isFiring)
        {
            this.isFiring = false;
            ((SandyAnimator)this.animator).EndFire();
            Destroy(this.currentLaser);
            this.currentLaser = null;
        }
        base.EndAttack();
    }

    public override void Die()
    {
        if (this.currentLaser != null)
            Destroy(this.currentLaser);
        base.Die();
    }
}
