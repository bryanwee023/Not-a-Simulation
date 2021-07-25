using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Special", menuName = "Ability/Plasma Bat/Special/Basic")]
public class BatSpin : Ability
{

    [Header("Attributes")]
    [SerializeField]
    private float specialTime;
    [SerializeField]
    private float cooldown;

    [Header("VFX")]
    [SerializeField]
    private GameObject spinVFX;

    public override void Initialize()
    {
        //spinVFX.transform.localScale *= this.radius / 17f;
        base.Initialize();
    }

    public override void Trigger()
    {
        if (!StaminaBar.instance.SpendStamina(0.3f)) return;

        PlayerController.instance.StartCoroutine(Spin());
        this.animator.SetTrigger("Special");

        //CameraRig.Shake(0.3f, 0.15f);

        PlayerState.nextSpecial = Time.time + this.specialTime + attributes["delay"] + this.cooldown;
        PlayerState.nextMove = Time.time + attributes["delay"] + this.specialTime;
    }


    IEnumerator Spin()
    {
        float delay = attributes["delay"];
        
        float elapsed = 0;
        float angularVelocity = 720f / specialTime + delay;

        while (elapsed <= delay)
        {
            mesh.Rotate(0, Time.deltaTime * angularVelocity, 0);
            elapsed += Time.deltaTime;

            yield return null;
        }

        GameObject spin = Instantiate(this.spinVFX, player.position + 3 * Vector3.up, player.rotation);
        spin.transform.localScale *= attributes["radius"] / 17f;

        bool hit = false;
        foreach (Collider c in this.RadialSweep(attributes["radius"]))
        {
            if (c.CompareTag("Enemy"))
            {
                hit = true;
                DealDamage(c.GetComponent<EnemyController>(), (int)attributes["damage"]);
            }
            else if (c.CompareTag("Projectile"))
            {
                Vector3 dir = (c.transform.position - player.position).normalized;
                c.GetComponent<Projectile>().Deflect(dir, 3f);
            }
        }

        if (hit)
        {
            CameraRig.Shake(0.5f, 0.15f);
            AudioManager.instance.hit.Play();
        }

        while (elapsed <= specialTime + delay)
        {
            mesh.Rotate(0, Time.deltaTime * angularVelocity, 0);
            elapsed += Time.deltaTime;

            yield return null;
        }

        mesh.localRotation = Quaternion.Euler(0, 270, 0);
    }

    protected virtual void DealDamage(EnemyController enemy, int damage)
    {
        enemy.TakeDamage(damage);

        if (enemy.canPushback)
        {
            Vector3 dir = enemy.transform.position - player.position;
            dir.y = 0;
            enemy.GetComponent<PushBody>().Push(dir, 45, 0.1f);
        }
    }
}
