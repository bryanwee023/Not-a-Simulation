using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HugoBattle : MonoBehaviour
{
    [Header("Dungeon Transforms")]
    [SerializeField]
    private Transform entryPlatform;
    [SerializeField]
    private Transform startPoint;
    [SerializeField]
    private float activationRadius;

    [Header("Dungeon Prefabs")]
    [SerializeField]
    private CannonballController hugo;
    [SerializeField]
    private GameObject redPortal;
    [SerializeField]
    private GameObject bluePortal;
    [SerializeField]
    private GameObject block;

    [Header("Dialogue")]
    [SerializeField]
    private GameObject dialogueUI;
    [SerializeField]
    private Dialogue startDialogue;
    [SerializeField]
    private Dialogue[] endDialogue;

    [Header("Debugging Tools")]
    [SerializeField]
    private GameObject testPlayer;

    // Start is called before the first frame update
    void Start()
    {
        WorldState.worldLevel++;

        //Testing purposes
        if (PlayerController.instance == null)
            Instantiate(testPlayer, startPoint.position, startPoint.rotation);


        PlayerState.player.position = entryPlatform.transform.position;
        StartCoroutine(DescendPlatform());

        AudioManager.instance.ambientNoise.Play();

    }

    IEnumerator DescendPlatform()
    {
        while (entryPlatform.position.y > 0)
        {
            entryPlatform.position -= 25 * Time.deltaTime * Vector3.up;
            PlayerState.player.position = entryPlatform.transform.position;
            yield return null;
        }

        PlayerController.MoveTo(startPoint.position);
        this.ready = true;
    }

    private bool ready = false;
    private bool activated = false;
    private bool cleared = false;

    // Update is called once per frame
    void Update()
    {
        if (ready && !activated && PlayerState.player.position.magnitude < activationRadius)
        {
            activated = true;
            block.SetActive(true);

            Instantiate(dialogueUI).GetComponent<DialogueManager>()
                .ParseDialogue(
                    startDialogue,
                    () => {
                        this.hugo.Activate();
                        AudioManager.instance.BGM.Play();
                        AudioManager.instance.ambientNoise.Stop();
                    }
                );
        }

        if (!cleared && EnemyController.AllCleared())
        {
            Invoke("EndDialogue", 7f);

            cleared = true;
            AudioManager.instance.BGM.Stop();
            AudioManager.instance.ambientNoise.Play();
        }

    }

    private void EndDialogue()
    {
        Instantiate(dialogueUI).GetComponent<DialogueManager>()
                .ParseDialogue(endDialogue[0], () =>
                    Instantiate(dialogueUI).GetComponent<DialogueManager>()
                        .ParseDialogue(endDialogue[1], () =>
                        {
                            block.SetActive(false);
                            redPortal.SetActive(true);
                            bluePortal.SetActive(true);
                        })
               );

        block.SetActive(false);
        redPortal.SetActive(true);
        bluePortal.SetActive(true);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(Vector3.zero, activationRadius);
    }
}
