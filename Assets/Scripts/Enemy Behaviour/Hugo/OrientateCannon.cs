using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientateCannon : MonoBehaviour
{
    [SerializeField]
    private Transform CannonStart;
    [SerializeField]
    private Transform CannonEnd;

    // Start is called before the first frame update
    void Start()
    {
        this.transform.rotation = Quaternion.LookRotation(CannonEnd.position - CannonStart.position);
    }
}
