using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public int maxHealth;

    public float damageMultiplier = 1;
    public int currentHealth { get; private set; }

    public int attack;

    private HealthbarManager hbManager;
    

    private void Start()
    {
        this.maxHealth = (int)(this.maxHealth * WorldState.hpScale );
        this.attack = (int)(this.attack * WorldState.atkScale);

        this.currentHealth = maxHealth;

        this.hbManager = this.GetComponentInChildren<HealthbarManager>();

        this.hbManager.initialise(maxHealth);
    }

    public int TakeDamage(int damage)
    {
        return TakeDamage(damage, Color.white);
    }

    public int TakeDamage(int damage, Color color)
    {
        damage = (int)(damage * damageMultiplier);
        this.hbManager.UpdateHealthbar(damage, color);
        currentHealth -= damage;
        return currentHealth;
    }
}