using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeonardOrb : Projectile
{
    private void Start()
    {
        Destroy(this.gameObject, 8f);
        Vector3 pos = this.transform.position;
        this.transform.position = new Vector3(pos.x, 3, pos.z);
        this.attack = 3;
    }
    // Update is called once per frame
    void Update()
    {
        this.transform.position += speed * Time.deltaTime * this.transform.forward;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!deflected && other.CompareTag("Player"))
        {
            PlayerController.TakeDamage(this.attack);

        } else if (other.CompareTag("Projectile"))
            return;

        Destroy(this.gameObject);
    }
}
