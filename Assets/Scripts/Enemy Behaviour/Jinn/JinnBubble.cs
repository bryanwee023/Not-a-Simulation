using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JinnBubble : Projectile
{
    [SerializeField]
    private float lifespan;

    [SerializeField]
    private GameObject explosion;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("Burst", lifespan);
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position += this.transform.forward * this.speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController.TakeDamage(this.attack);
        }
        else if (other.CompareTag("Enemy") || other.CompareTag("Projectile"))
        {
            if (!this.deflected) return;

            var enemy = other.GetComponent<EnemyController>();
            if (enemy != null) enemy.TakeDamage(this.attack);
        }
        this.Burst();
    }

    private void Burst()
    {
        Instantiate(this.explosion, this.transform.position, this.transform.rotation);
        Destroy(this.gameObject);
    }

    public override void Deflect(Vector3 direction, float multiplier)
    {
        this.speed *= 2f;
        base.Deflect(direction, multiplier);
    }

}
