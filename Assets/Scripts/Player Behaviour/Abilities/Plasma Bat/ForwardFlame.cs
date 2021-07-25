using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForwardFlame : MonoBehaviour
{
    public int damage;
    public float speed;

    private static Color ORANGE = new Color(1f, 0.4f, 0);

    private void Start()
    {
        Destroy(this.gameObject, 1);
    }

    public void SetAttack(int attack)
    {
        this.damage = attack;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position += this.transform.forward * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<EnemyController>().TakeDamage(damage, ORANGE, false);
        }
    }
}
