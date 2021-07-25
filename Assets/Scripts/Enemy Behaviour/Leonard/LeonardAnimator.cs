using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeonardAnimator : EnemyAnimator
{
    [SerializeField]
    private GameObject[] hands;
    [SerializeField]
    private Transform centreFront;

    [Header("VFX")]
    [SerializeField]
    private GameObject projectile;
    [SerializeField]
    private GameObject laser;
    [SerializeField]
    private GameObject smashParticles;
    [SerializeField]
    private GameObject blood;
    [SerializeField]
    private GameObject voxelDeath;

    [Header("SFX")]
    [SerializeField]
    private AudioSource smashSFX;

    private LeonardProjectile currentProj;

    private void Update() { }

    public void Fire(int attack, float delay)
    {
        this.animator.SetTrigger("Fire");

        foreach (GameObject hand in hands)
            hand.SetActive(false);

        this.currentProj = Instantiate(this.projectile, centreFront.position, Quaternion.identity)
            .GetComponent<LeonardProjectile>();
        this.currentProj.SetAttack(attack);

        Invoke("ReleaseProjectile", delay);
    }

    public void AimLaser()
    {
        this.animator.SetTrigger("Laser");
        Invoke("Pause", 0.2f);
    }

    private void Pause()
    {
        this.animator.speed = 0;
    }

    public void FireLaser(int attack, float duration)
    {
        StartCoroutine(LaserRoutine(attack, duration));
    }

    IEnumerator LaserRoutine(int attack, float duration)
    {
        //Lower body
        this.transform.position -= 3 * Vector3.up;

        //Instantiate lasers
        GameObject[] lasers = new GameObject[4];

        for(int i = 0; i < 4; i++)
        {
            GameObject laser = Instantiate(this.laser, hands[i].transform);
            laser.GetComponent<SandyLaser>().SetAttack(attack);

            lasers[i] = laser;
            Destroy(laser, duration);
        }

        //Rotate arms
        this.animator.speed = 1.5f / duration;

        //Orientate lasers
        for (float elapsed = 0; elapsed < duration; elapsed += Time.deltaTime)
        {
            Vector3 baseDir = hands[0].transform.position - this.transform.position;
            baseDir.y = 0;
            Quaternion baseRot = Quaternion.LookRotation(baseDir);

            for (int i = 0; i < 4; i++)
            {
                lasers[i].transform.rotation = baseRot;
                lasers[i].transform.Rotate(new Vector3(0, 1, 0), -90 * i);
            }

            yield return null;
        }

        this.transform.position += 3 * Vector3.up;
        this.animator.speed = 1;
        this.animator.SetTrigger("Idle");

    }

    private void ReleaseProjectile()
    {
        if (isDead)
        {
            Destroy(this.currentProj);
            return;
        }

        this.currentProj.Release();
        this.currentProj = null;

        foreach (GameObject hand in hands)
            hand.SetActive(true);
    }


    public void PreSmash()
    {
        this.animator.SetTrigger("Pre Smash");
    }

    public void Smash()
    {
        this.smashSFX.Play();
        this.animator.SetTrigger("Smash");
        Instantiate(this.smashParticles, this.centreFront.transform);
        CameraRig.Shake(0.7f, 0.5f);
    }

    public void Idle()
    {
        this.animator.SetTrigger("Idle");
    }

    public override void Die()
    {
        CameraRig.Shake(0.5f, 4f);

        if (this.currentProj != null)
            Destroy(this.currentProj.gameObject);

        foreach (GameObject hand in hands)
            Destroy(hand);

        this.blood.SetActive(true);
        base.Die();

        Vector3 pos = this.transform.position - 75 * Vector3.up + this.transform.forward * 10;
        Instantiate(this.voxelDeath, pos, this.transform.rotation);
    }
}
