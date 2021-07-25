using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeonardController : EnemyController
{
    private enum sequence { SMASH, FIRE, LASER };

    //Smash constants
    private float AIM_TIME = 0.3f;
    private float DODGE_WINDOW = 0.3f;
    private float RUSH_TIME = 0.4f;
    private float RUSH_SPEED = 100;

    //Fire constants
    private float CHARGE_TIME = 1f;

    //Laser constants
    private float LASER_TIME = 2f;

    private bool closingIn;
    private int smashCounter = 0;

    protected override void InitAttack()
    {
        this.isAttacking = true;
        this.agent.ResetPath();
        this.agent.velocity = Vector3.zero;

        sequence next = RollSequence();

        if (next == sequence.SMASH) Smash();

        else if (next == sequence.FIRE) Fire();

        else Laser();
        
    }

    public void Activate()
    {
        this.activated = true;
        this.GetComponentInChildren<HealthbarManager>().Activate();
        this.nextAttack = Time.time + 2f;
        this.ToggleHitbox(true);
    }

    private void Smash()
    {
        StartCoroutine(SmashRoutine());
    }

    IEnumerator SmashRoutine()
    {
        ((LeonardAnimator)this.animator).PreSmash();
        for (float elapsed = 0; elapsed < AIM_TIME; elapsed += Time.deltaTime)
        {
            this.FaceTarget();
            yield return null;
        }
        for (float elapsed = 0; elapsed < DODGE_WINDOW; elapsed += Time.deltaTime)
            yield return null;

        ((LeonardAnimator)this.animator).Smash();
        
        bool hit = false;
        for (float elapsed = 0; elapsed < RUSH_TIME; elapsed += Time.deltaTime)
        {
            this.transform.position += RUSH_SPEED * Time.deltaTime * this.transform.forward;

            if (PlayerWithinReach() && !hit)
            {
                hit = true;
                PlayerController.TakeDamage(this.stats.attack);
            }

            yield return null;
        }

        ((LeonardAnimator)this.animator).Idle();
        this.EndAttack();
    }

    private void Fire()
    {
        ((LeonardAnimator)this.animator).Fire(this.stats.attack, CHARGE_TIME);
        Invoke("EndAttack", 2 * CHARGE_TIME);
    }

    private void Laser()
    {
        this.agent.radius = 16;
        StartCoroutine(RotateTowards(45));
        ((LeonardAnimator)this.animator).AimLaser();
        Invoke("FireLaser", DODGE_WINDOW);
    }

    private void FireLaser()
    {
        ((LeonardAnimator)this.animator).FireLaser(this.stats.attack, LASER_TIME);
        Invoke("EndAttack", LASER_TIME);
    }

    IEnumerator RotateTowards(float angle)
    {
        float rotated = 0;
        while (rotated < angle)
        {
            this.transform.Rotate(0, -120 * Time.deltaTime, 0);
            rotated += 120 * Time.deltaTime;
            yield return null;
        }

        if (!isDead) this.agent.radius = 8;
    }

    private sequence RollSequence()
    {
        if (smashCounter > 0)
        {
            smashCounter--;
            return sequence.SMASH;
        }

        if (closingIn)
        {
            smashCounter = 2;
            return sequence.SMASH;
        } else
        {
            if (Random.Range(0f, 1f) < 0.67f)
                return sequence.FIRE;
            else
                return sequence.LASER;
        }

    }

    protected override bool CanAttack()
    {
        return true;
    }

    protected override void EndAttack()
    {
        if (smashCounter == 0)
        {
            closingIn = Random.Range(0f, 1f) < 0.4f;
            base.EndAttack();
        } else
        {
            this.isAttacking = false;
            this.nextAttack = Time.time + 0.3f;
        }
    }

    protected override void Move()
    {
        if (closingIn)
            base.Move();
        else
        {
            Vector3 dir = this.transform.position - PlayerState.player.position;
            this.agent.SetDestination(this.transform.position + dir);
        }
    }
}
