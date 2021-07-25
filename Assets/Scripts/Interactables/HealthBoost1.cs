using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealPickup : Interactable
{
    public int amount;
    public override void Interact()
    {
        PlayerStats.Heal(this.amount);
        AudioManager.instance.consumeHealth.Play();
        Destroy(this.gameObject);
    }
}
