using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HugoShadow : MonoBehaviour
{
    private static float originalSize;

    private float elapsed;
    private static float peakAt = CannonballController.JUMP_TIME / 2;

    public void Awake()
    {
        originalSize = this.transform.localScale.x;
    }

    private void OnEnable()
    {
        this.elapsed = 0;
    }

    // Update is called once per frame
    void Update()
    {
        float newSize = (1 - 0.3f * (Mathf.Abs(elapsed - peakAt) / peakAt)) * originalSize;
        this.transform.localScale = new Vector3(newSize, this.transform.localScale.y, newSize);
        elapsed += Time.deltaTime;
    }
}
