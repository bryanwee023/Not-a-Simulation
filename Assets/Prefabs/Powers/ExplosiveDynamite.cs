using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveDynamite : Power
{
    [SerializeField]
    private GameObject explosion;
    [SerializeField]
    private GameObject dynamiteMesh;

    private float elapsed = 0;

    // Update is called once per frame
    void Update()
    {
        if (dynamiteMesh.transform.position.y > 0.1f)
        {
            dynamiteMesh.transform.position -= Vector3.up * 15 * Time.deltaTime;
        }

        if (elapsed >= this.duration)
        {
            Instantiate(this.explosion, this.transform.position + 3 * Vector3.up, this.transform.rotation).SetActive(true);
            Explode();
            Destroy(this.gameObject);
        }

        elapsed += Time.deltaTime;
    }

    private void Explode()
    {
        foreach (Collider c in this.RadialSweep())
        {
            if (c.CompareTag("Enemy"))
            {
                c.GetComponent<EnemyController>().TakeDamage(this.damage);
            } else if (c.CompareTag("Player"))
            {
                PlayerController.TakeDamage((int)(this.damage / 10f));
            }
        }
    }
}
