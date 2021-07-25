using UnityEngine;

public class RotateSlash : MonoBehaviour
{
    [SerializeField]
    private float angularSpeed;

    // Update is called once per frame
    void Update()
    {
        this.transform.Rotate(angularSpeed * Vector3.up);
    }
}
