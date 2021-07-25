using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpUI : MonoBehaviour
{
    private float startTime;

    private void Start()
    {
        startTime = Time.time;
        PlayerController.instance.paused = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown && Time.time - startTime > 0.5f)
        {
            PlayerController.instance.paused = false;
            Destroy(this.gameObject);
        }
    }
}
