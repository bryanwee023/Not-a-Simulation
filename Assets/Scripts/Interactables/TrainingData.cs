using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingData : Interactable
{
    public override void Interact()
    {
        ActionPanelUI.instance.UnlockSkill();
        Destroy(this.gameObject);
    }
}
