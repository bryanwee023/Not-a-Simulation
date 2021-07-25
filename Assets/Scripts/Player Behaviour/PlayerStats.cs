using UnityEngine;

public class PlayerStats: MonoBehaviour
{
    #region Singleton

    public static PlayerStats instance;

    private void Awake()
    {
        if (instance != null) { Debug.Log("More than one instance of PlayerStats found!"); }
        else instance = this;

        this.maxHealth = 100;
        this.health = maxHealth;
        this.exp = 0;
    }

    #endregion

    [SerializeField]
    public int maxHealth { get; private set; }
    public int health { get; private set; }

    public int exp { get; private set; }

    public static bool TakeDamage(int amount)
    {
        instance.health = Mathf.Max(0, instance.health - amount);
        ActionPanelUI.instance.UpdateHealthbar(instance.health);
        ActionPanelUI.instance.DamageAnimation();

        return instance.health > 0;
    }

    public static void Heal(int amount)
    {
        instance.health = Mathf.Min(instance.maxHealth, instance.health + amount);
        ActionPanelUI.instance.UpdateHealthbar(instance.health);
    }

    public static void AddHealth(int amount)
    {
        instance.maxHealth += amount;
        ActionPanelUI.instance.UpdateMaxHealth(instance.maxHealth);

        Heal(amount);
    }

    public static void AddExp(int amount)
    {
        instance.exp += amount;

        ActionPanelUI.instance.UpdateExpBar(amount);
    }

}
