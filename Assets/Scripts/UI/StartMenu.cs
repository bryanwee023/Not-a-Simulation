using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
    public Camera cam;
    public Image fadeBlack;
    public AudioSource whooshSFX;
    public CanvasGroup ui;
    public GameObject creditsScreen;

    public void PlayGame()
    {
        StartCoroutine(StartRoutine());
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OpenCredits()
    {
        this.creditsScreen.SetActive(true);
    }

    IEnumerator StartRoutine()
    {
        float elapsed = 0;
        Vector3 oldPos = cam.transform.position;
        Vector3 newPos = new Vector3(0, 14, oldPos.z);

        while (elapsed < 0.5)
        {
            cam.transform.position = Vector3.Lerp(oldPos, newPos, elapsed / 0.5f);
            ui.alpha = elapsed / 0.5f;
            elapsed += Time.deltaTime;
            yield return null;
        }

        ui.alpha = 0;
        cam.transform.position = newPos;

        for (float pause = 0; pause < 0.33f; pause += Time.deltaTime)
            yield return null;

        whooshSFX.Play();
        while (cam.orthographicSize > 0)
        {
            cam.orthographicSize -= 270 * Time.deltaTime;

            Color black = fadeBlack.color;
            black.a += Time.deltaTime * 3;
            fadeBlack.color = black;

            yield return null;
        }

        for (float pause = 0; pause < 0.5f; pause += Time.deltaTime)
            yield return null;

        SceneManager.LoadScene("Pick Weapon");

    }
}
