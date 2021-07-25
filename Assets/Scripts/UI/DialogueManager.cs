using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public delegate void EndDelegate();

    [SerializeField]
    private GameObject clickToContinue;
    [SerializeField]
    private Text nameText;
    [SerializeField]
    private Text dialogueText;
    [SerializeField]
    private AudioSource blipSFX;
    private EndDelegate onEnd;

    private void Start()
    {
        PlayerController.instance.Pause(true);
    }

    public void ParseDialogue(Dialogue dialogue, EndDelegate onEnd)
    {
        this.onEnd = onEnd;
        StartCoroutine(RunDialogue(dialogue));
    }

    IEnumerator RunDialogue(Dialogue dialogue)
    {
        nameText.text = dialogue.name;

        foreach (string str in dialogue.dialogue)
        {
            clickToContinue.SetActive(false);

            blipSFX.Play();
            dialogueText.text = "";

            string formattedStr = string.Format(str, PlayerName());
            foreach (char c in formattedStr.ToCharArray())
            {
                dialogueText.text += c;
                yield return null;
                yield return null;
            }

            blipSFX.Stop();
            clickToContinue.SetActive(true);

            while (!Input.GetKeyDown(KeyCode.Space))
                yield return null;
        }

        Destroy(this.gameObject);
    }

    private string PlayerName()
    {
        return "T-" + PlayerPrefs.GetInt("RunCount", 0);
    }

    private void OnDestroy()
    {
        PlayerController.instance.paused = false;
        if (this.onEnd != null) this.onEnd();
    }

}
