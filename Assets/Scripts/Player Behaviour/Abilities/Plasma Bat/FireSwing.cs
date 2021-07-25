using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Attack", menuName = "Ability/Plasma Bat/Attack/Fire")]
public class FireSwing : BatSwing
{

    [Header("Burn Effect")]
    [SerializeField]
    private float burnDuration;
    [SerializeField]
    private GameObject burnMarker;
    [SerializeField]
    private GameObject forwardFlameVFX;

    protected override void Swing(int damage, Vector2 dimensions)
    {
        if (attributes["forwardFlames"] > 0 && this.comboState == 0)
        {
            GameObject flames = Instantiate(this.forwardFlameVFX, this.player.position + 3 * this.player.forward, this.player.rotation);
            flames.GetComponent<ForwardFlame>().SetAttack((int)attributes["forwardFlames"]);
        }
        base.Swing(damage, dimensions);
    }

    protected override void DealDamage(EnemyController enemy, int damage)
    {
        enemy.TakeDamage((int)(damage * COMBO_MULTIPLIER[comboState]));
        enemy.Burn((int)attributes["burnDamage"], Time.time + burnDuration, this.burnMarker);
    }
}
