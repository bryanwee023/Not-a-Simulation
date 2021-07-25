using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class SpideyController : EnemyController
{
    private const float LUNGE_DURATION = 0.5f;
    private const float LUNGE_SPEED = 45f;
    private const float CROUCH_DURATION = 0.6f;

    protected override void InitAttack()
    {
        this.isAttacking = true;
        this.animator.Attack();
        this.agent.velocity = Vector3.zero;

        this.agent.radius = 0;

        ((SpideyAnimator)this.animator).LightUp(CROUCH_DURATION + LUNGE_DURATION);
        Invoke("Pounce", CROUCH_DURATION);
        nextQueuedAttack = Time.time + CROUCH_DURATION;
    }

    private void Pounce()
    {
        if (Time.time > this.staggeredUntil)
            StartCoroutine(LungeForward());
        else this.EndAttack();
    }

    IEnumerator LungeForward()
    {
        if (isDead) yield break;

        ((SpideyAnimator)this.animator).Pounce();

        this.agent.velocity = LUNGE_SPEED * this.transform.forward;

        bool hit = false;
        for (float elapsed = 0; elapsed < LUNGE_DURATION; elapsed += Time.deltaTime)
        {
            if (!hit && PlayerWithinReach(3.5f))
            {
                hit = true;
                PlayerController.TakeDamage(this.stats.attack);
                this.GetComponent<AudioSource>().Play();
            }

            yield return null;
        }

        if (!this.isDead) this.agent.velocity = Vector3.zero;
        this.EndAttack();
    }

    protected override void EndAttack()
    {
        if (isDead) return;
        this.agent.radius = 4;

        base.EndAttack();
    }
}
