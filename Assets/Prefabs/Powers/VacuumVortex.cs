using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VacuumVortex : Power
{
    private float interval = 0.5f;
    private float vacuumPower = 3;
    private float elapsed = 0;

    [SerializeField]
    private float vacuumRadius;

    private void Start()
    {
        Destroy(this.gameObject, this.duration);
    }

    // Update is called once per frame
    void Update()
    {
        //Suck enemies into the middle
        foreach (Collider c in this.RadialSweep())
        {
            if (c.CompareTag("Enemy") && c.GetComponent<EnemyController>().canPushback)
            {
                Vector3 dir = (this.transform.position - c.transform.position).normalized;
                c.transform.position += vacuumPower * dir * Time.deltaTime;
            }
        }

        //Deal damage every 0.5s
        if (elapsed >= interval)
        {
            foreach (Collider c in this.RadialSweep())
            {
                if (c.CompareTag("Enemy"))
                {
                    c.GetComponent<EnemyController>().TakeDamage(this.damage / 2);
                }
            }
            elapsed = 0;
        }
            
        elapsed += Time.deltaTime;
    }
}
