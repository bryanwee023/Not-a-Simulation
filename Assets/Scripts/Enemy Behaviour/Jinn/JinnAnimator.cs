using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JinnAnimator : EnemyAnimator
{
    public override void Die()
    {
        this.transform.LookAt(PlayerController.instance.transform.position);
        this.transform.Rotate(0, 180, 0);
        base.Die();
    }
}
