using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeonardBattle : MonoBehaviour
{
    [Header("Dungeon Transforms")]
    [SerializeField]
    private Transform entryPlatform;
    [SerializeField]
    private Transform startPoint;
    [SerializeField]
    private Transform dynamicStage;
    [SerializeField]
    private float activationRadius;

    [Header("Dungeon Prefabs")]
    [SerializeField]
    private LeonardController leonard;
    [SerializeField]
    private GameObject exitPortal;
    [SerializeField]
    private GameObject block;
    [SerializeField]
    private GameObject reward;

    [Header("Dialogue")]
    [SerializeField]
    private GameObject dialogueUI;
    [SerializeField]
    private Dialogue startDialogue;
    [SerializeField]
    private Dialogue endDialogue;

    [Header("SFX")]
    [SerializeField]
    private AudioSource earthquakeSFX;

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

        PlayerController.instance.transform.position = entryPlatform.position;
        PlayerController.instance.transform.rotation = entryPlatform.rotation;
        CameraRig.AssignTarget(PlayerState.player);

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
        if (ready && !activated && Vector3.Distance(PlayerState.player.position, leonard.transform.position) < activationRadius)
        {
            activated = true;

            StartCoroutine(DropDynamicStage(true, () => {
                Instantiate(dialogueUI).GetComponent<DialogueManager>()
                    .ParseDialogue(
                        this.startDialogue,
                        () => {
                            this.leonard.Activate();
                            AudioManager.instance.BGM.Play();
                            AudioManager.instance.ambientNoise.Stop();
                        }
                    );
            }));
        }

        if (!cleared && EnemyController.AllCleared())
        {
            cleared = true;
            AudioManager.instance.BGM.Stop();
            AudioManager.instance.ambientNoise.Play();

            Invoke("DeathDialogue", 3f);

        }
        
    }

    private void DeathDialogue()
    {
        Instantiate(dialogueUI).GetComponent<DialogueManager>()
            .ParseDialogue(
                this.endDialogue,
                () => {
                    StartCoroutine(DropDynamicStage(false, () => exitPortal.SetActive(true)));
                    Vector3 spawnPoint = PlayerState.player.position + 6 * PlayerState.player.forward;
                    Instantiate(this.reward, spawnPoint, Quaternion.identity);
                }
            );

    }

    private const float DROP_HEIGHT = 40;
    private const float DROP_TIME = 3;

    delegate void Callback();

    IEnumerator DropDynamicStage(bool x, Callback callback)
    {
        this.earthquakeSFX.Play();
        CameraRig.Shake(0.25f, 3);
        block.SetActive(x);

        float velocity = x ? -DROP_HEIGHT / DROP_TIME : DROP_HEIGHT / DROP_TIME;

        for (float elapsed = 0; elapsed < DROP_TIME; elapsed += Time.deltaTime)
        {
            this.dynamicStage.position += velocity  * Time.deltaTime * Vector3.up;
            yield return null;
        }
        callback();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(Vector3.zero, activationRadius);
    }
}
