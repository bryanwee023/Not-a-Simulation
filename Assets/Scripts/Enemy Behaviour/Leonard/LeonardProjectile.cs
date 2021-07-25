using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeonardProjectile : Projectile
{
    [SerializeField]
    private float radius;
    [SerializeField]
    private float lifespan;
    [SerializeField]
    private GameObject orb;

    private bool released = false;
    private float nextHit  = 0;

    private void Start()
    {
        Invoke("Burst", lifespan);
    }

    public void Release()
    {
        this.released = true;

        Vector3 lookDir = PlayerState.player.position - this.transform.position;
        lookDir.y = 0;
        this.transform.rotation = Quaternion.LookRotation(lookDir);

        GameObject clone = Instantiate(this.gameObject, this.transform.position, Quaternion.Euler(0, 45, 0) * this.transform.rotation);
        clone.GetComponent<LeonardProjectile>().released = true;
        clone.GetComponent<LeonardProjectile>().SetAttack(this.attack);

        clone = Instantiate(this.gameObject, this.transform.position, Quaternion.Euler(0, -45, 0) * this.transform.rotation);
        clone.GetComponent<LeonardProjectile>().released = true;
        clone.GetComponent<LeonardProjectile>().SetAttack(this.attack);
    }

    private void Update()
    {
        if (released)
            this.transform.position += this.speed * Time.deltaTime * this.transform.forward;

        Vector3 pos1 = this.transform.position;
        Vector3 pos2 = PlayerState.player.position;

        float planarDist = Vector2.Distance(new Vector2(pos1.x, pos1.z), new Vector2(pos2.x, pos2.z));
        if (planarDist < this.radius)
        {
            if (Time.time > nextHit)
            {
                PlayerController.TakeDamage(this.attack / 4);
                nextHit = Time.time + 0.5f;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Enemy") || other.CompareTag("Projectile"))
            return;

        Burst();
    }

    private void Burst()
    {
        //Burst into 5 children
        for (int i = 0; i < 5; i++)
        {
            GameObject orb = Instantiate(this.orb, this.transform.position, this.transform.rotation);
            orb.transform.Rotate(0, i * 72, 0);
            orb.GetComponent<Projectile>().SetAttack(this.attack / 3);
        }

        Destroy(this.gameObject);
    }

    public override void Deflect(Vector3 direction, float multiplier) { }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, this.radius);
    }
}
