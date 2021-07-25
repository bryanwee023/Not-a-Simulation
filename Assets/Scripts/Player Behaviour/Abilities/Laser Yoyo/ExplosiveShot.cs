using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Attack", menuName = "Ability/Laser Yoyo/Attack/Explosive")]
public class ExplosiveShot : YoyoStrike
{

    [Header("Explosive Effect")]
    [SerializeField]
    private GameObject dynamite;

    private const int ENEMY_MASK = 1 << 7;
    protected override void Shoot()
    {
        float chargedRange = Mathf.Min(this.range, (charge / attributes["chargeTime"]) * this.range);
        float normalizedDamage = Mathf.Pow(chargedRange / this.range, 0.5f);
        int chargedDamage = Mathf.RoundToInt(normalizedDamage * attributes["damage"]);


        //Deal Damage
        Ray ray  = new Ray(player.position + Vector3.up, player.transform.forward);
        RaycastHit hit;
        if (Physics.SphereCast(ray, 6, out hit, chargedRange, ENEMY_MASK))
        {
            EnemyController enemy = hit.collider.GetComponent<EnemyController>();
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

            this.CastPower(dynamite, hit.point, attributes["attackPower"]);

            chargedRange = Vector3.Distance(player.position, hit.point);
        }

        PlayShootVFX(chargedRange / this.range);

        this.charge = 0;
        PlayerState.nextAttack = Time.time + this.cooldown;
        PlayerState.nextMove = Time.time + this.cooldown;
    }
}
