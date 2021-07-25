using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Attack", menuName = "Ability/Plasma Bat/Attack/Tidal")]
public class TidalSwing : BatSwing
{
    [Header("Knockback Effect")]
    [SerializeField]
    private float knockback;

    protected override void DealDamage(EnemyController enemy, int damage)
    {
        float multiplier = (100 + attributes["attackMultiplier"]) / 100;
        base.DealDamage(enemy, (int)(damage * multiplier * COMBO_MULTIPLIER[comboState]));

        if (enemy.canPushback)
        {
            Vector3 dir = enemy.transform.position - player.position;
            dir.y = 0;
            enemy.GetComponent<PushBody>().Push(dir, knockback, 0.15f);
        }
    }


}
