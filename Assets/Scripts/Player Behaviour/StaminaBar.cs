using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    static readonly Vector3 OFFSET = new Vector3(-0.7f, 6, 3);
    static readonly Quaternion FROM_CAM = Quaternion.LookRotation(new Vector3(-1, -1, -1));

    #region Singleton

    public static StaminaBar instance;

    private void Awake()
    {
        if (instance != null) { Debug.Log("More than one instance of Staminabar found! Replacing stamina bar"); }

        instance = this;
    }

    #endregion

    [SerializeField]
    private GameObject barUI;
    [SerializeField]
    private Image fill;

    public float rechargeTime;

    private Transform player;
    private bool full;

    private void Start()
    {
        this.player = PlayerState.player;
    }

    // Update is called once per frame
    void Update()
    {
        if (full) return;

        this.transform.position = this.player.position + OFFSET;
        this.transform.rotation = FROM_CAM;

        this.fill.fillAmount += Time.deltaTime / rechargeTime;
        if (this.fill.fillAmount >= 1)
        {
            full = true;
            this.barUI.SetActive(false);
        }
    }

    public bool SpendStamina(float amount)
    {
        if (full)
        {
            full = false;
            this.barUI.SetActive(true);
        }

        if (this.fill.fillAmount < amount) return false;

        this.fill.fillAmount -= amount;
        return true;
    }
}
