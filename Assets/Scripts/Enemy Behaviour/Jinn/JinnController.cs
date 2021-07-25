using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JinnController : EnemyController
{
    [SerializeField]
    private GameObject spell;

    protected override void InitAttack()
    {
        Vector3 dirToPlayer = (PlayerController.instance.transform.position - this.transform.position).normalized; 
        GameObject currentSpell = Instantiate(spell, this.transform.position + dirToPlayer * 2, Quaternion.LookRotation(dirToPlayer));
        currentSpell.GetComponent<Projectile>().SetAttack(this.stats.attack);
        base.InitAttack();
    }
}
