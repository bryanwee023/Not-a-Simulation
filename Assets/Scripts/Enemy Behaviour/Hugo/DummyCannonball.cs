using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyCannonball : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject, 3f);
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position += 60 * Time.deltaTime * Vector3.up;
    }
}
