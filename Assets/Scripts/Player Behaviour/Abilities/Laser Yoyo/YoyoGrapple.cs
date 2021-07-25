using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "New Special", menuName = "Ability/Laser Yoyo/Special/Basic")]
public class YoyoGrapple : Ability
{

    [Header("Attributes")]
    [SerializeField]
    private float delay;
    [SerializeField]
    private float specialTime;
    [SerializeField]
    private float cooldown;
    [SerializeField]
    private GameObject bombPower;

    [Header("VFX")]
    [SerializeField]
    private GameObject specialIndicator;
    [SerializeField]
    private GameObject chargeVFX;
    [SerializeField]
    private GameObject yoyoDuplicate;
    [SerializeField]
    private GameObject yoyoLine;

    private Transform hand;
    private GameObject tempIndicator;
    private GameObject tempCharge;

    public override void Initialize()
    {
        base.Initialize();

        this.tempCharge = Instantiate(this.chargeVFX, player);
        this.tempCharge.SetActive(false);

        this.tempIndicator = Instantiate(this.specialIndicator, player);
        this.tempIndicator.SetActive(false);

        this.hand = PlayerController.instance.rightHand;
    }

    public override void Trigger()
    {
        if (attributes["isAdrenaline"] > 0)
            Time.timeScale = 0.8f;

        PlayerController.instance.StartCoroutine(ChargeRoutine());
        this.PlayChargeVFX();

        PlayerState.nextSpecial = float.MaxValue;
        PlayerState.nextMove = float.MaxValue;
        PlayerState.nextAttack = float.MaxValue;
    }

    public override void Terminate()
    {
        Destroy(this.tempIndicator);
        Destroy(this.tempCharge);
        base.Terminate();
    }
    IEnumerator ChargeRoutine()
    {
        while(!Input.GetKeyUp("q"))
        {
            Vector3 aimPosition;
            if (CameraRig.MouseToWorldPosition(out aimPosition))
            {
                player.LookAt(aimPosition);
                this.tempIndicator.transform.position = aimPosition + 0.1f * Vector3.up;
            }

            yield return null;

        }

        if (attributes["isAdrenaline"] > 0) Time.timeScale = 1;

        //Check if destination is a valid one.
        Vector3 dest = this.tempIndicator.transform.position;
        NavMeshPath path = new NavMeshPath();
        player.GetComponent<NavMeshAgent>().CalculatePath(dest, path);

        if (path.status == NavMeshPathStatus.PathComplete)
            PlayerController.instance.StartCoroutine(this.Grapple(this.tempIndicator.transform.position));
        else
        {
            PlayerState.nextSpecial = Time.time;
            PlayerState.nextMove = Time.time;
            PlayerState.nextAttack = Time.time;

            this.tempIndicator.SetActive(false);
            this.tempCharge.SetActive(false);
        }
    }

    protected virtual IEnumerator Grapple(Vector3 dest)
    {
        PlayerState.nextSpecial = Time.time + specialTime + cooldown;
        PlayerState.immuneUntil = Time.time + specialTime;
        PlayerState.nextMove = Time.time + specialTime;
        PlayerState.nextAttack = Time.time + specialTime;

        PlayGrappleVFX(dest);

        Transform duplicate = Instantiate(yoyoDuplicate).transform;
        LineRenderer line = Instantiate(yoyoLine).GetComponent<LineRenderer>();

        //Throw Yoyo VFX
        for (float elapsedTime = 0; elapsedTime < delay / 2; elapsedTime += Time.deltaTime)
        {
            duplicate.position = Vector3.Lerp(duplicate.position, dest, 0.2f);
            line.SetPosition(0, hand.position);
            line.SetPosition(1, duplicate.position);

            yield return null;
        }
        duplicate.position = dest;

        AudioManager.instance.grapplingHit.Play();
        for (float elapsedTime = 0; elapsedTime < delay / 2; elapsedTime += Time.deltaTime)
            yield return null;

        AudioManager.instance.grapplingReel.Play();

        //Reel player
        Vector3 originalPos = player.position;

        float elapsed = 0.0f;
        float nextBomb = 0;

        PlayerController.ToggleAgent(false);
        while (elapsed <= this.specialTime)
        {
            player.position = Vector3.Lerp(originalPos, dest, elapsed / specialTime);

            if (elapsed >= nextBomb)
            {
                this.CastPower(bombPower, player.position, attributes["specialPower"]);
                nextBomb += this.specialTime / attributes["bombCount"];
            }

            elapsed += Time.deltaTime;

            line.SetPosition(0, hand.position);
            line.SetPosition(1, duplicate.position);
            yield return null;
        }

        AudioManager.instance.grapplingReel.Stop();
        PlayerController.ToggleAgent(true);

        Destroy(duplicate.gameObject);
        Destroy(line.gameObject);
    }

    private void PlayChargeVFX()
    {
        this.animator.SetTrigger("Charge");
        this.tempCharge.SetActive(true);
        this.tempIndicator.SetActive(true);
    }

    private void PlayGrappleVFX(Vector3 dest)
    {
        this.animator.SetTrigger("Grapple");
        this.tempIndicator.SetActive(false);
        this.tempCharge.SetActive(false);
    }
}
