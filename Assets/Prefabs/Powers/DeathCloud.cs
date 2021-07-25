using UnityEngine;

public class DeathCloud : Power
{
    [SerializeField]
    private float damageMultiplier;
    [SerializeField]
    private float curseDuration;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Collider c in this.RadialSweep())
        {
            if (c.CompareTag("Enemy"))
            {
                c.GetComponent<EnemyController>().Curse(damageMultiplier, curseDuration);
                c.GetComponent<EnemyController>().TakeDamage(this.damage);
            }
        }
        Destroy(this.gameObject, this.duration);
    }
}
