using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpPickUp : Interactable
{
    public int amount = 40;

    public override void Interact()
    {
        PlayerStats.AddExp(amount);
        Destroy(this.gameObject);
    }
}
