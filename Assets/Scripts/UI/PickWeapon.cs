using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickWeapon : Interactable
{
    public GameObject weaponCharacter;
    public GameObject guideUI;

    public GameObject counterpart;

    public override void Interact()
    {
        Vector3 position = PlayerState.player.position;
        Quaternion rotation = PlayerState.player.rotation;

        Destroy(PlayerController.instance.gameObject);
        PlayerController.ClearSingleton();

        Instantiate(this.weaponCharacter, position, rotation);
        Instantiate(this.guideUI);

        this.gameObject.SetActive(false);
        this.counterpart.SetActive(true);
        
    }
}
