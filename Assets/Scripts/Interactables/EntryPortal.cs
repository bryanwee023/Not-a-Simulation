using UnityEngine;
using UnityEngine.SceneManagement;

public class EntryPortal : Interactable
{
    [Range(1, 8)]
    public int startLevel = 1;

    public override void Interact()
    {
        ActionPanelUI.instance.FadeOut();
        Invoke("LoadScene", 1);
    }

    private void LoadScene()
    {
        WorldState.Reset();
        WorldState.worldLevel = startLevel - 1;
        if (WorldState.worldLevel == 3)
            SceneManager.LoadScene("Leonard Battle");
        else if (WorldState.worldLevel == 7)
            SceneManager.LoadScene("Hugo Battle");
        else
            SceneManager.LoadScene("Random Dungeon");
    }
}

