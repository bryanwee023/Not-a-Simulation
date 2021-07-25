using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Attack", menuName = "Ability/Laser Yoyo/Attack/Basic")]
public class YoyoStrike : Ability
{
    //private const float AIRTIME = 0.2f; //Airtime of projectile configured to 2 * 0.2s, to change, update YoyoStrike.cs and animation

    [Header("Attributes")]
    [SerializeField]
    protected float range;
    [SerializeField]
    protected float cooldown;

    [Header("VFX")]
    [SerializeField]
    protected GameObject aimIndicator;
    [SerializeField]
    protected GameObject strikeVFX;
    [SerializeField]
    protected GameObject chargeVFX;

    protected float charge;
    protected GameObject tempCharge;
    protected GameObject tempIndicator;

    public override void Initialize()
    {
        base.Initialize();
        
        this.tempCharge = Instantiate(this.chargeVFX, player);
        this.tempCharge.SetActive(false);

        this.tempIndicator = Instantiate(this.aimIndicator, player);
        this.tempIndicator.transform.localScale = new Vector3(1, 1, this.range / 20);
        this.tempIndicator.GetComponentInChildren<Animator>().speed = 1 / attributes["chargeTime"];
        this.tempIndicator.SetActive(false);
    }

    public override void Trigger()
    {
        PlayerController.instance.StartCoroutine(ChargeRoutine());

        this.PlayChargeVFX();

        PlayerState.nextMove = float.MaxValue;
    }

    public override void Terminate()
    {
        Destroy(this.tempIndicator);
        Destroy(this.tempCharge);
    }

    IEnumerator ChargeRoutine()
    {
        while (!Input.GetKeyUp("mouse 0"))
        {
            Vector3 aimPosition;
            if (CameraRig.MouseToWorldPosition(out aimPosition, 80))
            {
                Vector3 projectedPos = new Vector3(aimPosition.x, 0, aimPosition.z);
                player.LookAt(projectedPos);
            }
            charge += Time.deltaTime;
            yield return null;
        }
        this.Shoot();
    }

    protected virtual void Shoot()
    {

        if (charge / attributes["chargeTime"] >= 0.33f)
        {
            float chargedRange = Mathf.Min(this.range, (charge / attributes["chargeTime"]) * this.range);
            float normalizedDamage = Mathf.Pow(chargedRange / this.range, 0.5f);
            int chargedDamage = Mathf.RoundToInt(normalizedDamage * attributes["damage"]);

            bool hit = false;
            foreach (Collider c in this.Sweep(new Vector2(6, chargedRange)))
            {
                if (c.CompareTag("Enemy"))
                {
                    EnemyController enemy = c.GetComponent<EnemyController>();
                    if (attributes["backstab"] > 0 && this.isBehind(enemy.transform))
                    {
                        enemy.TakeDamage((int)(chargedDamage * 1.5f), "Backstab", Color.white);
                        
                    } else enemy.TakeDamage(chargedDamage);

                    if (enemy.canPushback)
                    {
                        Vector3 dir = player.transform.forward;
                        dir.y = 0;
                        enemy.GetComponent<PushBody>().Push(dir, 60, 0.05f);
                    }

                    hit = true;
                }
            }
            if (hit)
                AudioManager.instance.hit.Play();

            PlayShootVFX(chargedRange / this.range);
        } else PlayShootVFX(0);

        this.charge = 0;
        PlayerState.nextAttack = Time.time + this.cooldown;
        PlayerState.nextMove = Time.time + this.cooldown;
    }

    protected bool isBehind(Transform other)
    {
        Quaternion LookToPlayer = Quaternion.LookRotation(this.player.position - other.position);
        return Quaternion.Angle(LookToPlayer, other.transform.rotation) > 120;
    }

    private void PlayChargeVFX()
    {
        this.animator.SetTrigger("Charge");
        this.tempCharge.SetActive(true);
        this.tempIndicator.SetActive(true);
    }

    protected virtual void PlayShootVFX(float normalizedRange)
    {
        this.animator.SetTrigger("Shoot");
        this.tempCharge.SetActive(false);
        this.tempIndicator.SetActive(false);

        if (normalizedRange != 0)
        {
            GameObject strike = Instantiate(this.strikeVFX, player.position, player.rotation);
            strike.transform.localScale = Vector3.Scale(strike.transform.localScale, new Vector3(1, 1, 1.2f * normalizedRange));    //pre-built values
        }
    }
}
