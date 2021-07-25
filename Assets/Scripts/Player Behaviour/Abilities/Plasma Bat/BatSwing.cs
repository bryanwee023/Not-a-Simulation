using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Attack", menuName = "Ability/Plasma Bat/Attack/Basic")]
public class BatSwing : Ability
{
    private static Vector2[] SWING_DIMENSIONS = new Vector2[]{ new Vector2(12, 14.5f), new Vector2(9.5f, 14.5f), new Vector2(8, 14.5f) };
    private static float COMBO_WINDOW = 0.6f;
    protected static float[] COMBO_MULTIPLIER = new float[]{ 0.8f, 1, 1.2f };

    [Header("Attributes")]
    [SerializeField]
    protected float baseCooldown;

    [Header("VFX")]
    [SerializeField]
    private GameObject[] slashes;
    [SerializeField]
    private GameObject dashStrikeVFX;
    [SerializeField]
    private GameObject staminaBar;

    private StaminaBar stamina;

    protected int comboState;
    private float lastAttack;

    public override void Initialize()
    {
        //this.attributes.Add("damage", 0);
        //this.attributes.Add("cooldown", 0.25f);
        //this.attributes.Add("dashstrike", 0);
        this.stamina = Instantiate(staminaBar).GetComponent<StaminaBar>();
        DontDestroyOnLoad(this.stamina.gameObject);

        this.comboState = 0;
        this.lastAttack = 0;
        base.Initialize();
    }

    public override void Trigger()
    {
        //Check is stamina available
        if (!stamina.SpendStamina(0.25f))
        {
            comboState = 0;
            return;
        }

        if (Time.time > lastAttack + COMBO_WINDOW)
            comboState = 0;

        if (attributes["dashstrike"] > 0 && Time.time < PlayerState.dashingUntil)
        {
            DashStrike();
        }
        else if (Time.time > PlayerState.dashingUntil)
        {
            player.rotation = Quaternion.LookRotation(GetAimDirection());

            //Damage enemies and deflect projectiles
            this.Swing((int)attributes["damage"], SWING_DIMENSIONS[comboState]);

            if (comboState == 2)
                LungeForward(25f, 0.1f);

            this.PlayVFX();

            PlayerState.nextAttack = Time.time + attributes["cooldown"];
            PlayerState.nextMove = Time.time + attributes["cooldown"];

            comboState = (comboState + 1) % 3;
            lastAttack = Time.time;

            if (attributes["dashstrike"] > 0)
                PlayerController.instance.StartCoroutine(this.WaitForDash(attributes["cooldown"]));
         }
    }

    protected virtual void Swing(int damage, Vector2 dimensions)
    {
        bool hit = false;
        foreach (Collider c in this.Sweep(dimensions))
        {
            if (c.CompareTag("Enemy"))
            {
                this.DealDamage(c.GetComponent<EnemyController>(), damage);
                hit = true;
            }

            else if (c.CompareTag("Projectile"))
            {
                Vector3 dir = (c.transform.position - player.position).normalized;
                c.GetComponent<Projectile>().Deflect(dir, 3f);
            }
            
        }

        if (hit)
        {
            CameraRig.Shake(0.3f, 0.15f);
            AudioManager.instance.hit.Play();
        }
    }

    protected virtual void DealDamage(EnemyController enemy, int damage)
    {
        enemy.TakeDamage((int)(damage * COMBO_MULTIPLIER[comboState]));

        if (enemy.canPushback)
        {
            Vector3 dir = enemy.transform.position - player.position;
            dir.y = 0;
            enemy.GetComponent<PushBody>().Push(dir, 60, 0.05f);
        }
    }

    public override void Terminate()
    {
        if (this.stamina != null) Destroy(this.stamina.gameObject);
    }

    #region DashStrike

    IEnumerator WaitForDash(float time)
    {
        for (float elapsed = 0; elapsed < time; elapsed += Time.deltaTime)
        {
            if (Input.GetKeyDown("space"))
            {
                DashStrike();
                PlayerController.instance.abilities[2].Trigger();
                break;
            }
            yield return null;
        }
    }

    protected void DashStrike()
    {
        this.animator.SetInteger("Combo", 2);
        this.animator.SetTrigger("Attack");

        Instantiate(this.dashStrikeVFX, player);
        this.Swing((int)(attributes["damage"] * 1.2f), new Vector2(10, 10));
    }

    #endregion

    private void PlayVFX()
    {
        this.animator.SetInteger("Combo", this.comboState);
        this.animator.SetTrigger("Attack");
        
        Instantiate(slashes[this.comboState], player.position, player.rotation);
    }
}
