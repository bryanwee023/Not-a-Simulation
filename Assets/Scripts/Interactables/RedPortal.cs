using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedPortal : Interactable
{
    public GameObject endUI;

    public override void Interact()
    {
        StartCoroutine(Glitch(1));
    }

    IEnumerator Glitch(float duration)
    {
        Kino.AnalogGlitch glitch = CameraRig.instance.GetComponentInChildren<Kino.AnalogGlitch>();
        CanvasGroup cg = Instantiate(endUI).GetComponent<CanvasGroup>();

        cg.alpha = 0;
        glitch.scanLineJitter = 0.2f;
        glitch.verticalJump = 0.04f;
        glitch.colorDrift = 0.2f;

        for (float elapsed = 0; elapsed < duration; elapsed += Time.deltaTime)
        {
            cg.alpha = elapsed / duration;
            yield return null;
        }

        cg.alpha = 1;
        glitch.scanLineJitter = 0;
        glitch.verticalJump = 0;
        glitch.colorDrift = 0;
    }
}
