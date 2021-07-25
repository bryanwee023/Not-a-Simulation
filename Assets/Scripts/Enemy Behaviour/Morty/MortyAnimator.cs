using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MortyAnimator : EnemyAnimator
{
    [SerializeField]
    private GameObject muzzleFlare;
    [SerializeField]
    private GameObject dustCloud;

    public override void Attack()
    {
        Instantiate(this.muzzleFlare, this.transform.position + 10 * Vector3.up, this.transform.rotation);
        base.Attack();
    }

    public override void Die()
    {
        Instantiate(this.dustCloud, this.transform.position, this.transform.rotation);
        base.Die();
    }
}
