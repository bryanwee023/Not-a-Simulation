using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRig : MonoBehaviour
{
    #region Singleton

    public static CameraRig instance;

    private void Awake()
    {
        if (instance != null) { Debug.Log("More than one instance of CameraRig found!"); }
        else instance = this;
    }

    private void OnDestroy()
    {
        instance = null;
    }

    #endregion

    public Camera cam;
    [SerializeField]
    private float offset = 50;
    [SerializeField]
    private float dampen = 6f;
    [SerializeField]
    private LayerMask groundMask;

    private Transform target;

    // Start is called before the first frame update
    public static void AssignTarget(Transform target)
    {
        instance.target = target;
        instance.transform.position = instance.target.position + new Vector3(instance.offset, instance.offset, instance.offset);
        instance.cam.transform.LookAt(instance.target);
    }

    // Update is called once per frame
    void Update()
    {
        if (this.target == null)
        {
            //Debug.Log("Camera Rig has no target yet");
            return;
        }

        this.transform.position = Vector3.Lerp(
            this.transform.position,
            this.target.position + new Vector3(offset, offset, offset),
            dampen * Time.deltaTime
        );

    }

    public static void Shake(float magnitude, float duration, float delay=0)
    {
        instance.StartCoroutine(instance.CamShake(magnitude, duration, delay));
    }

    public IEnumerator CamShake(float magnitude, float duration, float delay)
    {
        Vector3 origPos = this.cam.transform.localPosition;

        float elapsed = -delay;

        while (elapsed < duration)
        {
            if (elapsed >= 0)
            {
                float x = Random.Range(-1.0f, 1.0f) * magnitude;
                float y = Random.Range(-1.0f, 1.0f) * magnitude;

                this.cam.transform.localPosition = new Vector3(x, y, origPos.z);
            }
            elapsed += Time.deltaTime;

            yield return null;
        }

        this.cam.transform.localPosition = origPos;
    }

    //default mask at ground
    public static bool MouseToWorldPosition(out Vector3 position, int mask = 64)
    {
        Ray ray = instance.cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000, mask))
        {
            position = hit.point;
            return true;
        }

        position = Vector3.zero;
        return false;
    }

    public void Zoom(float zoomRatio, float duration)
    {
        float newSize = this.cam.orthographicSize / zoomRatio;
        StartCoroutine(ZoomRoutine(newSize, dampen));
    }

    IEnumerator ZoomRoutine(float size, float duration)
    {
        float increment = (this.cam.orthographicSize - size) / duration;
        while (this.cam.orthographicSize > size)
        {
            this.cam.orthographicSize -= increment;
            yield return null;
        }
    }


}
