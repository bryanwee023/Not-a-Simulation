using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndCanvas : MonoBehaviour
{
    [SerializeField]
    private Text timer;

    // Start is called before the first frame update
    void Start()
    {
        AudioManager.instance.ambientNoise.Stop();

        float totalTime = Time.time - WorldState.startTime;
        timer.text = string.Format("{0:F0}:{1:F0}", totalTime / 60, totalTime % 60);

        PlayerPrefs.SetInt("RunCount", PlayerPrefs.GetInt("RunCount", 0) + 1);
    }

    public void Retry()
    {
        Time.timeScale = 1;
        Destroy(PlayerController.instance.gameObject);
        SceneManager.LoadScene("Pick Weapon");
    }

    public void MainMenu()
    {
        Time.timeScale = 1;
        Destroy(PlayerController.instance.gameObject);
        SceneManager.LoadScene("Start Menu");
    }
}
