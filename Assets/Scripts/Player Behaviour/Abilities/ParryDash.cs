using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dash", menuName = "Ability/Plasma Bat/Dash")]
public class ParryDash : BasicDash
{
    [Header("Power")]
    [SerializeField]
    private GameObject shield;

    private GameObject tempShield;
    private int dashCount = 0;

    public override void Trigger()
    {
        if (Time.time >= lastDash + dashCooldown)
            comboState = 0;

        this.CastPower(this.power, player.position, 0);

        if (dashCount == 0)
            this.tempShield = Instantiate(this.shield, player);

        PlayerController.instance.StartCoroutine(DashRoutine(dashTime));
        Instantiate(this.dashVFX, player);

        if (comboState >= attributes["burst"] - 1)
            PlayerState.nextDash = Time.time + dashCooldown;

        PlayerState.nextMove = Time.time + dashTime;
        PlayerState.dashingUntil = Time.time + dashTime;
        this.comboState++;
        this.lastDash = Time.time;
    }

    public override void Terminate()
    {
        Destroy(this.tempShield);
    }

    protected override IEnumerator DashRoutine(float duration)
    {
        dashCount++;

        yield return base.DashRoutine(duration);

        if (--dashCount == 0)
            Destroy(this.tempShield);
    }
}