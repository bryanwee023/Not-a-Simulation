using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Attack", menuName = "Ability/Plasma Bat/Attack/Curse")]
public class CursedSwing : BatSwing
{

    protected override void DealDamage(EnemyController enemy, int damage)
    {
        enemy.TakeDamage((int)(damage * COMBO_MULTIPLIER[comboState]));

        if (Random.Range(0, 100) < attributes["curseChance"])
            enemy.Curse(attributes["curseEffect"], 2f);
    }
}
