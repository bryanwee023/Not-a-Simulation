using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TreeDemo : MonoBehaviour
{

    public GameObject FreeData;
    public GameObject FreeExp;
    public GameObject FreeHealth;

    public EntryPortal portal;
    public GameObject ui;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("1"))
        {
            Vector3 pos = PlayerState.player.position + PlayerState.player.forward * 5;
            GameObject exp = Instantiate(FreeExp, pos, Quaternion.identity);
            exp.GetComponent<ExpPickUp>().amount = 100;
        } else if (Input.GetKeyDown("2"))
        {
            Vector3 pos = PlayerState.player.position + PlayerState.player.forward * 5;
            GameObject data = Instantiate(FreeData, pos, Quaternion.identity);
        } else if (Input.GetKeyDown("3"))
        {
            Vector3 pos = PlayerState.player.position + PlayerState.player.forward * 5;
            GameObject health = Instantiate(FreeHealth, pos, Quaternion.identity);
        }

        if (portal != null && Input.GetKeyDown(KeyCode.P))
        {
            portal.startLevel++;
            if (portal.startLevel == 9) portal.startLevel = 1;

            string text  = string.Format("Difficulty at {0}", portal.startLevel);
            if (portal.startLevel == 4) text += " (Leonard)";
            else if (portal.startLevel == 8) text += " (Hugo)";

            Instantiate(ui).GetComponentInChildren<TextMeshProUGUI>().text = text;
        }
    }
}
