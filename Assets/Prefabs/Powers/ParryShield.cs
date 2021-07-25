using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParryShield : Power
{
    private float multiplier = 1;

    private float elapsed = float.MaxValue;
    private const float INTERVAL = 0.4f;

    private void Start()
    {
        PlayerState.immuneUntil = Time.time + duration / 3;
        Destroy(this.gameObject, duration);
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Rotate(0, 50 * Time.deltaTime, 0);

        if (elapsed < INTERVAL)
        {
            elapsed += Time.deltaTime;
            return;
        }

        foreach (Collider c in this.RadialSweep())
        {
            Vector3 dir = (c.transform.position - this.transform.position).normalized;
            dir.y = 0;
            if (c.CompareTag("Projectile"))
            {
                c.GetComponent<Projectile>().Deflect(dir, this.multiplier);

            } else if (c.CompareTag("Enemy"))
            {
                //c.GetComponent<EnemyController>().TakeDamage(this.damage);
                PushBody pb = c.GetComponent<PushBody>();
                if (pb != null) pb.Push(dir, 30, INTERVAL);
            }
        }

        elapsed = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        Vector3 dir = (other.transform.position - this.transform.position).normalized;
        if (other.CompareTag("Projectile"))
        {
            other.GetComponent<Projectile>().Deflect(dir, this.multiplier);

        } else if (other.CompareTag("Enemy"))
        {
            PushBody pb = other.GetComponent<PushBody>();
            if (pb != null) pb.Push(dir, 30, 0.2f);
        }
    }

    public void SetMultiplier(float multiplier)
    {
        this.multiplier = multiplier;
    }
}
