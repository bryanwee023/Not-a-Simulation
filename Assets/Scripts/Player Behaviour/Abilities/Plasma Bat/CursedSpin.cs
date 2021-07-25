using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Special", menuName = "Ability/Plasma Bat/Special/Curse")]
public class CursedSpin : BatSpin
{

    protected override void DealDamage(EnemyController enemy, int damage)
    {
        enemy.TakeDamage(damage);
        enemy.Curse(attributes["curseEffect"], attributes["curseDuration"]);
    }
}
