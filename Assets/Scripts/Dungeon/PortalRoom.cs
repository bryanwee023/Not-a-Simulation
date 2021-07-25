using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalRoom : MonoBehaviour
{
    public GameObject weaponPedestal;
    public GameObject dialogueUI;
    public Dialogue[] dialogues;

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.GetInt("RunCount", 0) == 0)
        {
            weaponPedestal.SetActive(false);
        }

        Invoke("StartDialogue", 1);
    }

    private void StartDialogue()
    {
        int runCount = PlayerPrefs.GetInt("RunCount", 0);

        if (runCount < dialogues.Length)
        {
        Instantiate(dialogueUI).GetComponent<DialogueManager>()
            .ParseDialogue(this.dialogues[runCount], null);
        }
    }
}
