using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActionPanelUI : MonoBehaviour
{
    #region Singleton

    public static ActionPanelUI instance;

    private void Awake()
    {
        if (instance != null) { Debug.Log("More than one instance of Action Panel found!"); }
        else instance = this;

        this.healthText = this.GetComponentInChildren<TextMeshProUGUI>();
        this.transitionColor = Color.white;

        this.blackCanvas.SetActive(true);
    }

    private void OnDestroy()
    {
        instance = null;
    }

    #endregion

    private void Update()
    {
        if (this.transitionFill.color.a > 0 || this.bloodScreen.color.a > 0)
        {
            this.transitionColor.a -= 2f * Time.deltaTime;
            this.transitionFill.color = this.transitionColor;
            if (this.transitionFill.color.a <= 0)
                this.transitionFill.fillAmount = this.healthbar.normalizedValue;

            this.bloodColor.a -= 0.33f * Time.deltaTime;
            this.bloodScreen.color = this.bloodColor;
        }

        if (this.expbar.value < this.exp)
        {
            this.expbar.value += 100 * Time.deltaTime;
            if (this.expbar.value >= 100)
                this.LevelUp();
        }

        if (!WorldState.paused && Input.GetKeyDown(KeyCode.Escape))
        {
            Instantiate(this.pauseScreen);
        }


    }

    public GameObject blackCanvas;

    [Header("Healthbar")]
    [SerializeField]
    private Slider healthbar;
    [SerializeField]
    private Image transitionFill;
    [SerializeField]
    private Image bloodScreen;

    [Header("Exp Bar")]
    [SerializeField]
    private Slider expbar;
    [SerializeField]
    private AudioSource expSound;
    private int exp;

    [Header("UI Prefabs")]
    [SerializeField]
    private GameObject levelUpUI;
    [SerializeField]
    private GameObject newSkillUI;
    [SerializeField]
    private GameObject deathPopup;
    [SerializeField]
    private GameObject pauseScreen;


    private Color transitionColor;
    private Color bloodColor;

    private TextMeshProUGUI healthText = null;


    private void Start()
    {
       this.blackCanvas.SetActive(true);

        this.bloodColor = this.bloodScreen.color;
    }

    public void DamageAnimation()
    {
        this.transitionColor.a = 1f;
        this.transitionFill.color = this.transitionColor;

        this.bloodColor.a = 0.2f;
        this.bloodScreen.color = bloodColor;
    }

    public void UpdateMaxHealth(int totalHealth)
    {
        this.healthbar.maxValue = totalHealth;
        this.healthText.SetText(this.healthbar.value + "/" + totalHealth);
    }

    public void UpdateHealthbar(int health)
    {
        this.healthbar.value = health;
        this.healthText.SetText(this.healthbar.value + "/" + this.healthbar.maxValue);
    }

    public void LevelUp()
    {
        this.exp -= 100;
        this.expbar.value = 0;

        Instantiate(this.levelUpUI);
    }

    public void UnlockSkill()
    {
        Instantiate(this.newSkillUI);
    }

    public void UpdateExpBar(int increment)
    {
        //expSound.Play();
        this.exp += increment;
    }

    public void FadeOut()
    {
        this.blackCanvas.GetComponentInChildren<Animator>().speed = 1 / Time.timeScale;
        this.blackCanvas.GetComponentInChildren<Animator>().SetTrigger("Fade Out");
    }

    public void Die()
    {
        this.bloodColor.a = 1f;
        this.bloodScreen.color = bloodColor;

        AudioManager.instance.BGM.Stop();
        CameraRig.instance.Zoom(1.35f, 0.33f);

        GameObject popup = Instantiate(deathPopup);
        popup.GetComponent<Animation>()["Banner FadeIn"].speed = 1 / Time.timeScale;
        Invoke("FadeOut", 1f);

        PlayerPrefs.SetInt("RunCount", PlayerPrefs.GetInt("RunCount", 0) + 1);
    }

    [Header("Interactable")]

    [SerializeField]
    private GameObject action;
    [SerializeField]
    private TextMeshProUGUI actionText;

    public void UpdateAction(Interactable interactable)
    {
        if (interactable == null)
        {
            action.SetActive(false);
            return;
        }
        action.SetActive(true);
        actionText.SetText(interactable.actionName);
    }
}
