using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    private Interactable closest;

    private void Start()
    {
        closest = null;
    }

    private void Update()
    {
        Interactable newClosest = null;
        if (Interactable.IsActive()) newClosest = null;

        else {
            float min = float.MaxValue;

            if (Input.GetKeyDown("e") && closest != null)
            {
                closest.Interact();
                ActionPanelUI.instance.UpdateAction(null);
            }

            foreach(Interactable x in Interactable.inMap)
            {
                float dist = Vector3.Distance(x.transform.position, this.transform.position);
                if (dist <= x.radius && dist < min)
                {
                    min = dist;
                    newClosest = x;
                }
            }
        }

        if (closest != newClosest)
        {
            closest = newClosest;
            ActionPanelUI.instance.UpdateAction(newClosest);
        }
    }
}
