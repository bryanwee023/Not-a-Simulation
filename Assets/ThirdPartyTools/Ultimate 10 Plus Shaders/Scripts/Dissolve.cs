using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class Dissolve : MonoBehaviour
{
    private MeshRenderer meshRenderer;

    public float duration = 2;

    private void Start(){
        meshRenderer = this.GetComponent<MeshRenderer>();
        StartCoroutine(DissolveRoutine());
    }

    IEnumerator DissolveRoutine()
    {
        float t = 0f;
        Material[] mats = meshRenderer.materials;
        while (t <= duration)
        {
            mats[0].SetFloat("_Cutoff", Mathf.Sin(t / duration));
            t += Time.deltaTime;
        
            // Unity does not allow meshRenderer.materials[0]...
            meshRenderer.materials = mats;

            yield return null;
        }

        mats[0].SetFloat("_Cutoff", 1);
    }

}
