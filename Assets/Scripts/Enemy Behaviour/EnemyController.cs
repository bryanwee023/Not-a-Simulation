using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    private static int counter = 0;

    protected Transform target;
    protected NavMeshAgent agent;

    public float stopRadius;
    public float speed;
    public float angularSpeed;
    public float attackWindow;
    public float attackAngle;
    public float attackRadius;
    public float despawnTime = 3f;

    protected EnemyStats stats;
    protected EnemyAnimator animator;
    protected HealthbarManager hbManager;
    protected GameObject mesh;

    protected bool isAttacking = false;
    protected float nextAttack;
    protected float nextQueuedAttack;

    public bool isDead { get; private set; }

    public bool canStagger;
    public bool canPushback;
    public bool spawnAnimation;
    protected float staggeredUntil = 0;

    protected bool activated = false;

    public static bool AllCleared() { return counter <= 0; }
    public static void ResetCount() { counter = 0; }

    // Start is called before the first frame update
    void Awake()
    {
        counter++;

        this.agent = GetComponent<NavMeshAgent>();
        this.agent.stoppingDistance = stopRadius;
        this.agent.speed = this.speed;
        this.agent.areaMask = 1;

        this.stats = this.GetComponent<EnemyStats>();
        this.animator = this.GetComponentInChildren<EnemyAnimator>();
        this.hbManager = this.GetComponentInChildren<HealthbarManager>();

        this.mesh = GetComponentInChildren<EnemyAnimator>().gameObject;

        //Physics.IgnoreCollision(this.GetComponent<Collider>(), PlayerState.player.GetComponent<Collider>());

        this.nextAttack = Time.time + Random.Range(2.5f, 3.5f);

        if (canPushback) this.gameObject.AddComponent<PushBody>();

        if (spawnAnimation)
        {
            this.ToggleHitbox(false);
            this.animator.Spawn();

            Invoke("Spawn", 2f);
        }
    }

    private void Start()
    {
        this.target = PlayerState.player;
    }
    private void Spawn()
    {
        this.ToggleHitbox(true);
        this.activated = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!activated || isDead || Time.time <= this.staggeredUntil) { return; }
        if (this.isAttacking) { Attack(); }
        else
        {
            Move();

            if (CanAttack() && Time.time > nextAttack)
            {
                FaceTarget();
                float angleToPlayer = Vector3.Angle(this.transform.forward, target.transform.position - this.transform.position);;
                if (Mathf.Abs(angleToPlayer) < attackAngle)
                {
                    InitAttack();
                }
            }
        }

        this.StatusUpdate();
    }

    protected virtual bool CanAttack()
    {
        float distance = Vector3.Distance(this.transform.position, target.transform.position);
        return distance <= agent.stoppingDistance;
    }

    protected virtual void Move()
    {
        agent.SetDestination(this.target.position);
    }

    protected virtual void FaceTarget(float modifier=1)
    {
        Vector3 direction = (this.target.position - this.transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * angularSpeed * 1);
    }

    protected virtual void InitAttack(){
        this.isAttacking = true;
        this.animator.Attack();
        EndAttack();
    }

    protected virtual void Attack() { }

    protected virtual void EndAttack()
    {
        this.isAttacking = false;
        this.nextAttack = Time.time + this.attackWindow;
    }

    public virtual void Die()
    {
        this.animator.Die();
        this.ToggleHitbox(false);
        this.isDead = true;
        Destroy(this.stats);
        Destroy(this.agent);
        this.animator.isDead = true;

        Destroy(this.gameObject, this.despawnTime);

        counter--;
    }

    public virtual void TakeDamage(int damage, bool stagger=true)
    {
        TakeDamage(damage, Color.white, stagger);
    }

    public virtual void TakeDamage(int damage, Color color, bool stagger=true)
    {
        if (this.stats.TakeDamage(damage, color) <= 0)
        {
            this.Die();
        } else if (stagger && canStagger)
        {
            this.Stagger();
        }
    }

    public virtual void TakeDamage(int damage, string tag, Color color)
    {
        this.hbManager.GenerateDamageText(tag, color);
        this.TakeDamage(damage);
    }

    protected void Stagger()
    {
            this.animator.FlashWhite();
            this.animator.Stagger();
            this.staggeredUntil = Mathf.Max(nextQueuedAttack + 0.1f, Time.time + 0.6f);
            this.isAttacking = false;
            this.agent.ResetPath();
    }

    protected void ToggleHitbox(bool enabled)
    {
        this.GetComponent<Collider>().enabled = enabled;
    }

    protected bool PlayerWithinReach(float radius = -1)
    {
        if (radius == -1) radius = this.attackRadius;
        return Vector3.Distance(this.transform.position, this.target.position) <= radius;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, this.attackRadius);
    }

    #region debuffs

    private static readonly Color ORANGE = new Color(1f, 0.4f, 0);
    private static readonly Color PURPLE = new Color(0.33f, 0, 0.7f);
    
    private bool isSlowed;

    public void EnableSlow(float time)
    {
        if (!isSlowed && !isDead)
        {
            this.attackWindow *= 1.5f;
            this.hbManager.GenerateDamageText("Slowed", Color.cyan);
            Invoke("DisableSlow", time);
        }
    }

    private void DisableSlow()
    {
        this.attackWindow /= 1.5f;
        this.isSlowed = false;
    }

    private float cursedUntil;
    private int curseStack;

    public void Curse(float multiplier, float duration)
    {
        if (this.curseStack >= 3) return;

        if (this.curseStack == 0 && this.hbManager != null) this.hbManager.GenerateDamageText("Cursed", PURPLE);

        this.cursedUntil = (Time.time > this.cursedUntil) ? Time.time + duration : cursedUntil + duration;

        this.curseStack++;
        this.stats.damageMultiplier += multiplier / 100;

        this.GetComponentInChildren<HealthbarManager>().CurseUntil(cursedUntil);
    }

    private float burnedUntil;
    private int burnDamage;
    private float lastBurned = 0;
    private GameObject burnMarker;

    public void Burn(int damage, float until, GameObject burnMarker)
    {
        if (Time.time > burnedUntil) this.hbManager.GenerateDamageText("Burned", ORANGE);

        burnedUntil = Mathf.Max(until, burnedUntil);
        burnDamage = damage;

        if (this.burnMarker == null) this.burnMarker = Instantiate(burnMarker, this.transform);
    }

    private void StatusUpdate()
    {
        if (Time.time < burnedUntil && Time.time - lastBurned > 0.5f)
        {
            this.TakeDamage(burnDamage / 2, ORANGE, false);
            lastBurned = Time.time;
        } else if (this.burnMarker != null && Time.time > burnedUntil)
        {
            Destroy(this.burnMarker);
        }

        if (Time.time > cursedUntil && this.curseStack != 0)
        {
            this.curseStack = 0;
            this.stats.damageMultiplier = 1;

            //Destroy(this.curseMarker);
            //this.curseMarker = null;
        }
    }

    #endregion
}
