using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{

    protected static bool active = false;
    public static HashSet<Interactable> inMap = new HashSet<Interactable>();

    [Range(5.0f, 20.0f)]
    public float radius;

    public string actionName;

    private void OnEnable()
    {
        inMap.Add(this);
    }

    public virtual void Interact()
    {
        Debug.Log("Interacting with " + actionName);
    }

    private void OnDisable() {
        inMap.Remove(this);
    }

    public static bool IsActive() { return active; }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(this.transform.position, radius);
    }
}
