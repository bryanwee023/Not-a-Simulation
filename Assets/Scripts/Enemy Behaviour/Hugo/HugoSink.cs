using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HugoSink : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (this.transform.position.y >= -30)
        {
            this.transform.position -= 3 * Vector3.up * Time.deltaTime;
        }
    }
}
