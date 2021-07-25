using UnityEngine;

public class SlowingWater : Power
{
    [SerializeField]
    private float slowDuration;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Collider c in this.RadialSweep())
        {
            if (c.CompareTag("Enemy"))
            {
                c.GetComponent<EnemyController>().EnableSlow(slowDuration);
                c.GetComponent<EnemyController>().TakeDamage(this.damage);
            }
        }
        Destroy(this.gameObject, this.duration);
    }
}
