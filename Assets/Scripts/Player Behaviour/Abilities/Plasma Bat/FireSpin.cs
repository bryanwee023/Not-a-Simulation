using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Special", menuName = "Ability/Plasma Bat/Special/Fire")]
public class FireSpin : BatSpin
{
    [Header("Burn VFX")]
    [SerializeField]
    private GameObject burnMarker;

    protected override void DealDamage(EnemyController enemy, int damage)
    {
        base.DealDamage(enemy, damage);
        enemy.Burn((int)attributes["burnDamage"], Time.time + attributes["burnDuration"], this.burnMarker);
    }
}
