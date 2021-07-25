using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CannonballController : EnemyController
{
    //Jump parameters
    private const float CROUCH_DELAY = 0.24f;   //Always a third of JUMP_TIME
    public const float JUMP_TIME = 0.75f;      //To change JUMP_TIME, change Smash's speed in animator accordingly (2 / JUMP_TIME)
    private const float JUMP_HEIGHT = 40f;
    //Blast parameters
    private const float STAND_DELAY = 1f;

    [SerializeField]
    private int bombCount = 5;
    [SerializeField]
    private float summonRadius = 50;

    [SerializeField]
    private GameObject minion;
    [SerializeField]
    private GameObject shadow;

    private int currentMove = 2;

    public void Activate()
    {
        this.activated = true;
        this.GetComponentInChildren<HealthbarManager>().Activate();
        this.nextAttack = Time.time + 2f;
    }

    private void Update()
    {
        if (this.isAttacking || this.isDead || !this.activated) { return;}
        else if (Time.time > this.nextAttack)
        {
            this.isAttacking = true;
            
            if (this.currentMove == 0) { Smash(); }
            else if (this.currentMove == 1) { SummonBombs(); }
            else { Roar(); }
            
            this.currentMove = this.RollNextMove();

        }
    }

    private void Roar()
    {

        ((CannonballAnimator)this.animator).Roar(this.stats.attack);

        Invoke("EndAttack", 3f);
    }

    private void Smash()
    {
        this.ToggleHitbox(false);

        ((CannonballAnimator)this.animator).Leap();

        Invoke("JumpToPlayer", CROUCH_DELAY);
    }

    private void JumpToPlayer()
    {
        Vector3 targetPos = PlayerController.instance.transform.position;
        StartCoroutine(JumpTowards(targetPos, targetPos - this.transform.position));
    }

    IEnumerator JumpTowards(Vector3 targetPos, Vector3 lookDir)
    {
        if (lookDir.magnitude < 0.5f) lookDir = targetPos - this.transform.position;

        float elapsed = 0;

        bool inAir = false;

        float g = 8 * JUMP_HEIGHT / Mathf.Pow(JUMP_TIME, 2);
        float u = Mathf.Sqrt(2 * g * JUMP_HEIGHT);

        Transform mesh = this.transform.GetChild(0);

        Vector3 startPosition = this.transform.position;
        Quaternion startRotation = this.transform.rotation;

        Quaternion endRotation = lookDir.magnitude == 0 ?
            startRotation : Quaternion.LookRotation(lookDir);

        while (elapsed < JUMP_TIME) {
            if (elapsed >= 0)
            {
                if (!inAir)
                {
                    this.shadow.SetActive(true);
                    this.agent.enabled = false;
                    this.ToggleHitbox(false);
                    inAir = true;
                }

                float velocity_y = u - g * elapsed;
                mesh.position += Vector3.up * velocity_y * Time.deltaTime;

                this.transform.position = Vector3.Lerp(startPosition, targetPos, elapsed / JUMP_TIME);
                this.transform.rotation = Quaternion.Slerp(startRotation, endRotation, elapsed / JUMP_TIME);
            }
            elapsed += Time.deltaTime;

            yield return null;
        }

        if (PlayerWithinReach())
            PlayerController.TakeDamage(this.stats.attack);

        ((CannonballAnimator)this.animator).Smash();

        //Reposition
        mesh.position = Vector3.Scale(mesh.position, new Vector3(1, 0, 1));
        this.transform.position = targetPos;
        this.transform.rotation = endRotation;

        this.shadow.SetActive(false);
        this.agent.enabled = true;
        this.ToggleHitbox(true);
        this.EndAttack();
    }

    private void SummonBombs()
    {
        ((CannonballAnimator)this.animator).SummonBombs(bombCount, STAND_DELAY);

        for (int i = 0; i < bombCount; i++)
        {
            Vector3 pos = this.RandomPoint() + (120 + (20 * i)) * Vector3.up;
            Instantiate(this.minion, pos, Quaternion.identity);
        }

        Invoke("EndAttack", STAND_DELAY + 1f);
    }

    private Vector3 RandomPoint()
    {
        Vector3 pos;
        NavMeshHit hit;
        do
        {
            pos = new Vector3(
                Random.Range(-summonRadius, summonRadius),
                0,
                Random.Range(-summonRadius, summonRadius)
            );
        } while (!NavMesh.SamplePosition(pos, out hit, 0.1f, 1));

        return hit.position;
    }

    private int RollNextMove()
    {
        float randomFloat = Random.Range(0.0f, 1.0f);

        if (randomFloat <= 0.4f) { return 0; }  //Leap
        else if (randomFloat <= 0.8f) { return 1; }  //Summon Bombs
        else return 2;  //Roar
    }

    public override void Die()
    {
        base.Die();
        Invoke("KillAllMinions", 1f);
    }

    private void KillAllMinions()
    {
        HugoBomb.KillAll();
    }
}