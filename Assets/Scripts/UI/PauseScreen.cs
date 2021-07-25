using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseScreen : MonoBehaviour
{
    bool listening = false;
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 0;
        PlayerController.instance.paused = true;
        WorldState.paused = true;
    }

    // Update is called once per frame
    void Update()
    {
            
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (!listening) listening = true;

            else
            {
                PlayerController.instance.paused = false;
                Destroy(this.gameObject);
            }
        }
    }

    public void QuitGame()
    {
        Destroy(PlayerController.instance.gameObject);
        SceneManager.LoadScene("Start Menu");
    }

    private void OnDestroy()
    {
        Time.timeScale = 1;
        WorldState.paused = false;
    }
}
