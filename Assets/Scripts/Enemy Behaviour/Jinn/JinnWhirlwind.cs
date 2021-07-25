using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JinnWhirlwind : Projectile
{
    [SerializeField]
    private float zigzag;   //Angle of zigzag, in radians
    [SerializeField]
    private float lifeSpan;

    private float velocity_z;
    private float velocity_x;

    private float elapsed = 0;
    private ParticleSystem particles;

    private void Start()
    {
        this.transform.LookAt(PlayerController.instance.transform);

        this.velocity_z = this.speed * Mathf.Sin(zigzag);
        this.velocity_x = this.speed * Mathf.Cos(zigzag);

        particles = this.GetComponentInChildren<ParticleSystem>();
        Invoke("Stop", lifeSpan);
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position += this.transform.forward * velocity_z * Time.deltaTime;

        if ((int)elapsed % 2 == 0)
            this.transform.position += this.transform.right * velocity_x * Time.deltaTime;
        else
            this.transform.position -= this.transform.right * velocity_x * Time.deltaTime;

        elapsed += Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            PlayerController.TakeDamage(this.attack);
        else if (other.CompareTag("Enemy") || other.CompareTag("Projectile"))
        {
            if (!this.deflected) return;

            other.GetComponent<EnemyController>().TakeDamage(this.attack);
        }
        Stop();
    }

    private void Stop()
    {
        velocity_x /= 2;
        velocity_z /= 2;
        Destroy(this.GetComponent<Collider>());
        particles.Stop();

        Destroy(this.gameObject, 0.5f);
    }

    public override void Deflect(Vector3 direction, float multiplier)
    {
        base.Deflect(direction, multiplier);
        Stop();
    }
}
