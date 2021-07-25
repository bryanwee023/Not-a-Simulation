using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq;

public class PlayerController : MonoBehaviour
{
    #region Singleton

    public static PlayerController instance;

    private void Awake()
    {
        if (instance != null) { Debug.Log("More than one instance of PlayerController found!"); }
        else instance = this;
        
        PlayerState.player = this.transform;
    }

    private void OnDestroy()
    {
        foreach (Ability ability in abilities)
            ability.Terminate();
    }
    #endregion

    public Weapon weapon;

    //FOR TESTING PURPOSES
    public bool testMode;

    [Range(15.0f, 30.0f)]
    public float speed;

    [SerializeField]
    public Transform mesh;
    [SerializeField]
    public Transform rightHand;

    [HideInInspector]
    public Animator animator { get; private set; }

    [Header("Abilities")]
    public Ability[] abilities;

    [Header("VFX")]
    [SerializeField]
    private GameObject damageText;

    public SkillTree tree;

    public bool paused = false;
    public bool isDead { get; private set; }

    [Header("Testing")]
    public string[] presetAbilities;

    static PlayerController() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);

        this.gameObject.AddComponent<PushBody>();
        this.gameObject.AddComponent<PlayerStats>();
        this.animator = this.GetComponentInChildren<Animator>();

        this.isDead = false;

        ConfigurePlayerToScene();

        if (testMode)
        {
            foreach(Ability x in abilities)
                x.Initialize();

        } else this.tree = new SkillTree(this.weapon);  //CYCLIC


        for (int i = 0; i < presetAbilities.Length; i++)
        {
            SkillTree.instance.Unlock(presetAbilities[i]);
        }
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Random Dungeon"
            || scene.name == "Hugo Battle" || scene.name == "Leonard Battle")
        {
            instance.ConfigurePlayerToScene();
        }
            
    }

    private void ConfigurePlayerToScene()
    {
        ActionPanelUI.instance.UpdateMaxHealth(PlayerStats.instance.maxHealth);
        ActionPanelUI.instance.UpdateHealthbar(PlayerStats.instance.health);
        CameraRig.AssignTarget(this.transform);
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead || paused || Time.time < PlayerState.nextAttack) { return ; }
        else if (Time.time > PlayerState.nextMove && Time.time > PlayerState.dashingUntil)
        {
            //Move character
            Vector3 dir =  new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
            dir = Quaternion.Euler(0, 225, 0) * dir;
            this.transform.position += dir * speed * Time.deltaTime;

            //Rotate character
            if (dir.magnitude != 0)
                this.transform.rotation = Quaternion.LookRotation(dir);

            //Handle Run Animations
            this.animator.SetFloat("Move", dir.normalized.magnitude);
        }

        //Attack, Cast or Dash
        if (Input.GetKeyDown("mouse 0"))
        {
            this.abilities[0].Trigger();

        } else if (Input.GetKeyDown("q") && Time.time > PlayerState.nextSpecial)
        {
            this.abilities[1].Trigger();

        } else if (Input.GetKeyDown("space") && Time.time > PlayerState.nextDash && Time.time > PlayerState.nextMove)
        {
            this.abilities[2].Trigger();
        }

    }

    public static void TakeDamage(int damage)
    {
        if (Time.time < PlayerState.immuneUntil || instance.isDead) { return; }

        bool alive = PlayerStats.TakeDamage(damage);

        instance.GenerateDamageText(damage);

        if (!alive) instance.Die();
    }

    private void GenerateDamageText(int healthTaken)
    {
        Vector3 pos = this.transform.position + new Vector3(Random.Range(-1, 1), Random.Range(7, 8), Random.Range(-1, 1));
        GameObject text = Instantiate(this.damageText, pos, Quaternion.identity);
        text.transform.LookAt(CameraRig.instance.transform);
        text.transform.Rotate(0, 180, 0);

        text.GetComponentInChildren<TextMeshPro>().color = Color.red;
        text.GetComponentInChildren<TextMeshPro>().SetText(((int)healthTaken).ToString());
    }

    public static void ToggleAgent(bool toggle)
    {
        instance.GetComponent<NavMeshAgent>().enabled = toggle;
    }

    // Returns time taken
    public static float MoveTo(Vector3 destination, bool toggleAgent = true)
    {
        instance.StartCoroutine(instance.MoveToRoutine(destination, toggleAgent));
        return Vector3.Distance(instance.transform.position, destination) / instance.speed;
    }

    public IEnumerator MoveToRoutine(Vector3 destination, bool toggleAgent)
    {
        if (toggleAgent) ToggleAgent(false);
        this.paused = true;

        destination.y = 0;
        this.transform.position = new Vector3(transform.position.x, 0.1f, transform.position.z);

        Vector3 start = this.transform.position;
        Vector3 path = destination - this.transform.position;
        path.y = 0;
        this.transform.rotation = Quaternion.LookRotation(path);

        while (Vector3.Distance(start, this.transform.position) < path.magnitude)
        {
            this.transform.position += path.normalized * speed * Time.deltaTime;
            this.animator.SetFloat("Move", path.normalized.magnitude);
            yield return null;
        }
        this.transform.position = destination;
        if (toggleAgent) ToggleAgent(true);
        this.paused = false;
    }

    public void Die()
    {
        this.isDead = true;
        Time.timeScale = 0.2f;
        this.animator.SetTrigger("Death");

        ActionPanelUI.instance.Die();   //2 seconds of animation
        Invoke("Restart", 1.5f);
    }

    public void Pause(bool toggle)
    {
        this.paused = toggle;
        if (toggle)this.animator.SetFloat("Move", 0);
    }

    private void Restart()
    {
        Time.timeScale = 1;
        Destroy(this.gameObject);
        SceneManager.LoadScene("Pick Weapon");
    }

    #region Ability Management

    public static void LoadAbility(int id, Ability ability, Dictionary<string, float> newAttributes)
    {
        Ability newAbility = Instantiate(ability);
        if (instance.abilities[id] != null)
        {
            newAbility.PortAttributes(instance.abilities[id]);
            instance.abilities[id].Terminate();
        }
        newAttributes.ToList().ForEach(x => newAbility.attributes[x.Key] = x.Value);
        newAbility.Initialize();

        instance.abilities[id] = newAbility;
       
    }

    public static void LoadAbility(int id, string path, Dictionary<string, float> newAttributes)
    {
        LoadAbility(id, Resources.Load<Ability>(path), newAttributes);
    }

    public static void UpgradeAbility(int id, string attributeName, float value)
    {
        instance.abilities[id].attributes[attributeName] = value;
    }

    #endregion

    public static void ClearSingleton()
    {
        PlayerController.instance = null;
        PlayerStats.instance = null;
    }
}
