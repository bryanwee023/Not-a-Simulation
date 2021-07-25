using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Instantiate On Hit
public class PoweredHit : Power
{
    public void Explode(EnemyController enemy)
    {
        enemy.TakeDamage(this.damage);
    }
}
