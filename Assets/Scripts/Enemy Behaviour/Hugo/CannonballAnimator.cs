using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonballAnimator : EnemyAnimator
{
    [SerializeField]
    private GameObject smashImpact;
    [SerializeField]
    private GameObject roarAura;
    [SerializeField]
    private GameObject projectile;
    [SerializeField]
    private GameObject lavaPool;

    [SerializeField]
    private Transform Muzzle;
    [SerializeField]
    private GameObject MuzzleFlare;
    [SerializeField]
    private AudioSource jumpSFX;
    [SerializeField]
    private AudioSource roarSFX;

    private void Update() { }

    public void Roar(int attack)
    {
        this.roarSFX.Play();
        GameObject aura = Instantiate(roarAura, this.transform);
        aura.GetComponent<Projectile>().SetAttack(attack);
        this.animator.SetTrigger("Roar");

        CameraRig.Shake(0.5f, 2f, delay: 0.8f); 
    }

    public void Idle()
    {
        this.animator.SetTrigger("Idle");
    }

    public void Leap()
    {
        this.animator.SetTrigger("Leap");
        this.jumpSFX.Play();
    }

    public void Smash()
    {
        Instantiate(this.smashImpact, this.transform.parent.position + 1 * Vector3.up, this.transform.rotation);
        CameraRig.Shake(0.7f, 0.5f);
    }

    public void SummonBombs(int shots, float delay)
    {
        this.animator.SetTrigger("Stand");
        StartCoroutine(ShootRoutine(shots, delay));
    }

    IEnumerator ShootRoutine(int shots, float delay)
    {
        int fired = 0;

        for (float elapsed = 0; elapsed < delay; elapsed += Time.deltaTime)
            yield return null;

        while (fired < shots)
        {
            for (float elapsed = 0; elapsed < 0.33f; elapsed += Time.deltaTime)
                yield return null;

            Instantiate(this.projectile, this.Muzzle.position, this.Muzzle.rotation);
            Instantiate(this.MuzzleFlare, this.Muzzle.position, this.Muzzle.rotation);
            CameraRig.Shake(0.5f, 0.2f);

            fired++;
        }

        this.animator.SetTrigger("Idle");
    }

    public void Shoot()
    {
        Instantiate(this.projectile, this.Muzzle.position, this.Muzzle.rotation);
        Instantiate(this.MuzzleFlare, this.Muzzle.position, this.Muzzle.rotation);
    }

    public override void Die()
    {
        this.roarSFX.Play();
        base.Die();
        Instantiate(this.lavaPool, this.transform.position, this.transform.rotation);
        this.gameObject.AddComponent<HugoSink>();
        CameraRig.Shake(0.5f, 6f);

        this.isDead = true;
    }
}
