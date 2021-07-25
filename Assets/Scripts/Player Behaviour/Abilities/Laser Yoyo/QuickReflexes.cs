using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Spread Shot", menuName = "Ability/Laser Yoyo/Attack/Quick Reflexes")]
public class QuickReflexes : YoyoStrike
{

    public override void Trigger()
    {
        Time.timeScale = 0.8f;
        base.Trigger();
    }

    protected override void Shoot()
    {
        Time.timeScale = 1;
        base.Shoot();
    }
}
