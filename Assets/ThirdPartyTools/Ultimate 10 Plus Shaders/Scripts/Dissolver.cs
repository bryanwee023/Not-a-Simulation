using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dissolver : MonoBehaviour
{
    private const float HALF_PI = Mathf.PI / 2;

    private static Material dissolve;
    private static Material outliner;

    public SkinnedMeshRenderer outline;


    public float duration = 2;

    private void Awake()
    {
        if (dissolve == null)
            dissolve = Resources.Load<Material>("Materials/Dissolve");

        if (outliner == null)
            outliner = Resources.Load<Material>("Materials/Outliner");

        this.outline = this.transform.Find("Outline").GetComponent<SkinnedMeshRenderer>();

        StartCoroutine(SpawnRoutine());
    }

    private void MakeVisible(bool value)
    {
        foreach (var mesh in this.transform.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            if (mesh.gameObject.name == "Outline") continue;

            mesh.enabled = value;
        }
    }

    IEnumerator SpawnRoutine()
    {
        float t = 0;
        this.MakeVisible(false);

        outline.materials = new Material[1] { dissolve };
        Material dissolveInstance = outline.materials[0];

        while (t < duration)
        {
            dissolveInstance.SetFloat("_Cutoff", Mathf.Cos(HALF_PI * t / duration));
            t += Time.deltaTime;
        
            // Unity does not allow meshRenderer.materials[0]...
            outline.materials = new Material[1] { dissolveInstance };

            yield return null;
        }

        outline.materials = new Material[1] { outliner };
        this.MakeVisible(true);
    }

    public void Dissolve()
    {
        StartCoroutine(DissolveRoutine());
    }

    IEnumerator DissolveRoutine()
    {
        float t = 0f;

        this.MakeVisible(false);

        outline.materials = new Material[1] { dissolve };
        Material dissolveInstance = outline.materials[0];

        //Material dissolveInstance = dissolve.
        while (t <= duration)
        {
            dissolveInstance.SetFloat("_Cutoff", Mathf.Sin(t / duration));
            t += Time.deltaTime;
        
            // Unity does not allow meshRenderer.materials[0]...
            outline.materials = new Material[1] { dissolveInstance };

            yield return null;
        }

       dissolveInstance.SetFloat("_Cutoff", 1);
    }
}