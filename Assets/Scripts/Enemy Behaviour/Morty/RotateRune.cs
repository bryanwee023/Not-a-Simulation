using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RotateRune : MonoBehaviour
{

    [SerializeField]
    private float FadeRate = 1;
    private Image image;

    private void Start()
    {
        this.image = this.GetComponentInChildren<Image>();
        StartCoroutine(FadeIn());
    }
    // Update is called once per frame
    void Update()
    {
        this.transform.Rotate(Vector3.forward, 0.2f);
    }

    IEnumerator FadeIn() {
        float targetAlpha = 1.0f;
        Color curColor = this.image.color;

        while(Mathf.Abs(curColor.a - targetAlpha) > 0.0001f)
        {
            curColor.a = Mathf.Lerp(curColor.a, targetAlpha, FadeRate * Time.deltaTime);
            image.color = curColor;
            yield return null;
        }
    }
}
