using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthbarManager : MonoBehaviour
{
    private static Quaternion TO_CAM = Quaternion.LookRotation(new Vector3(1, 1, 1));
    private static Quaternion FROM_CAM = Quaternion.LookRotation(new Vector3(-1, -1, -1));

    [SerializeField]
    private GameObject healthbar;
    [SerializeField]
    private GameObject damageText;

    [Header("Curse GUI")]
    [SerializeField]
    private bool curseColor;
    [SerializeField]
    private GameObject curseMarker;

    [SerializeField]
    private bool dynamic = true;
    [SerializeField]
    private Transform textSource;
    
    private float currentHealth;

    private bool activated;

    private void Start()
    {
        this.healthbar.SetActive(false);

        if (textSource == null)
            textSource =  this.transform;
    }

    public void initialise(float totalHealth)
    {
        this.currentHealth = totalHealth;

        //TODO: Set healthbar size 

        this.healthbar.GetComponent<Slider>().maxValue = totalHealth;
        this.healthbar.GetComponent<Slider>().value = totalHealth;

    }

    public void UpdateHealthbar(int healthTaken, Color color)
    {
        if (!activated)
        {
            this.activated = true;
            this.healthbar.SetActive(true);
        }

        currentHealth -= healthTaken;
        this.healthbar.GetComponent<Slider>().value = currentHealth;

        GenerateDamageText(healthTaken.ToString(), color);

        if (currentHealth <= 0) { Destroy(this.gameObject); }
    }

    public void GenerateDamageText(string text, Color color)
    {
        Vector3 pos = this.textSource.position + new Vector3(Random.Range(-1, 1), 1.5f + Random.Range(-1, 1), Random.Range(-1, 1));
        GameObject damageText = Instantiate(this.damageText, pos, FROM_CAM);

        damageText.GetComponentInChildren<TextMeshPro>().SetText(text);
        damageText.GetComponentInChildren<TextMeshPro>().color = color;
    }

    public void Activate()
    {
        this.activated = true;
        this.healthbar.SetActive(true);
    }

    private float curseUntil;
    private bool cursed;

    private void Update()
    {
        if (this.dynamic) 
            this.transform.rotation = FROM_CAM;


        if (cursed && Time.time > curseUntil)
        {
            cursed = false;
            if (curseColor)
                this.curseMarker.GetComponent<Image>().color = new Color(0.725f, 0.18f, 0.165f);
            else
                this.curseMarker.SetActive(false);

        }
    }

    public void CurseUntil(float time)
    {
        if (!cursed)
        {
            cursed = true;
            if (curseColor)
                this.curseMarker.GetComponent<Image>().color = new Color(0.33f, 0, 0.7f);
            else
                this.curseMarker.SetActive(true);

        }

        curseUntil = time;
    }
}
