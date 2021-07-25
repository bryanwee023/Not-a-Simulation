using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FloorExit : Interactable
{

    public override void Interact()
    {
        if (EnemyController.AllCleared()) 
        {
            ActionPanelUI.instance.FadeOut();
            Invoke("NextScene", 1);
        } else 
        {
            Debug.Log("Enemies nearby!");
        }

    }


    private void NextScene()
    {
        PlayerController.ToggleAgent(false);

        if (WorldState.worldLevel == 3 )
            SceneManager.LoadScene("Leonard Battle");
        else if (WorldState.worldLevel == 7)
            SceneManager.LoadScene("Hugo Battle");
        else
            SceneManager.LoadScene("Random Dungeon");
    }
}
