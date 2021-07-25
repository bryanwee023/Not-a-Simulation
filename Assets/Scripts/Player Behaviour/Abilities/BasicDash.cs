using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dash", menuName = "Ability/Dash")]
public class BasicDash : Ability
{

    [Header("Attributes")]
    [SerializeField]
    protected float dashForce;
    [SerializeField]
    protected float dashCooldown;
    [SerializeField]
    protected float dashTime;
    [SerializeField]
    protected GameObject power;

    [Header("VFX")]
    [SerializeField]
    protected GameObject dashVFX;

    protected float lastDash;
    protected int comboState;

    public override void Initialize()
    {
        this.comboState = 0;
        base.Initialize();
    }

    public override void Trigger()
    {
        if (Time.time >= lastDash + dashCooldown)
            comboState = 0;

        this.CastPower(this.power, player.position, attributes["dashPower"]);

        PlayerController.instance.StartCoroutine(DashRoutine(dashTime));
        Instantiate(this.dashVFX, player);

        if (comboState >= attributes["burst"] - 1)
            PlayerState.nextDash = Time.time + dashCooldown;

        PlayerState.dashingUntil = Time.time + dashTime;
        if (this.comboState >= attributes["burst"])
            PlayerState.nextMove = Time.time + dashTime;
        PlayerState.immuneUntil = Time.time + 0.1f;
        this.comboState++;
        this.lastDash = Time.time;
    }

    protected virtual IEnumerator DashRoutine(float duration)
    {
        float elapsed = 0.0f;
        this.animator.SetFloat("Move", 1);

        while (elapsed <= duration)
        {
            player.position += player.forward * dashForce * Time.deltaTime;
            elapsed += Time.deltaTime;

            yield return null;
        }
    }
}
