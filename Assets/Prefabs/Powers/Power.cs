using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Power : MonoBehaviour
{
    [SerializeField]
    protected float radius;
    [SerializeField]
    protected int damage;
    [SerializeField]
    protected float duration;

    protected Collider[] RadialSweep(float radius=-1)
    {
        if (radius == -1) radius = this.radius;
        return Physics.OverlapSphere(this.transform.position + 3 * Vector3.up, radius);
    }

    public void SetDamage(int damage)
    {
        this.damage = damage;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, this.radius);
    }
}
