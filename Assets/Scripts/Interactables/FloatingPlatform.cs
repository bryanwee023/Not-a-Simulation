using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FloatingPlatform : Interactable
{
    [SerializeField]
    private Transform mesh;

    private Vector3 start, end;

    public delegate void OnDestination();
    public OnDestination onDest;

    public override void Interact()
    {
        if (EnemyController.AllCleared())
            StartCoroutine(Transport(start, end));
        else Debug.Log("Clear chamber first!");
    }

    IEnumerator Transport(Vector3 start, Vector3 end)
    {
        Interactable.active = true;

        mesh.position = this.transform.position - 30 * Vector3.up;
        mesh.gameObject.SetActive(true);
        Vector3 dir = (end - start).normalized;
        float dist = Vector3.Distance(start, end);
        float speed = 20 + dist / 5;

        while (mesh.position.y < -1)
        {
            mesh.position += 50 * Vector3.up * Time.deltaTime;
            yield return null;
        }

        PlayerController.ToggleAgent(false);
        float timeTaken = PlayerController.MoveTo(start, false);

        for (float elapsed = 0; elapsed < timeTaken; elapsed += Time.deltaTime)
            yield return null;

        while (Vector3.Distance(PlayerState.player.position, start) < dist)
        {
            mesh.position += speed * dir * Time.deltaTime;
            PlayerState.player.position = mesh.position + 2 * Vector3.up;
            yield return null;
        }

        timeTaken = PlayerController.MoveTo(end - this.transform.forward * 5);
        for (float elapsed = 0; elapsed < timeTaken; elapsed += Time.deltaTime)
            yield return null;

        PlayerState.player.position = end;
 
        Interactable.active = false;
        onDest();

        while (mesh.position.y > -50)
        {
            mesh.position -= 30 * Vector3.up * Time.deltaTime;
            yield return null;
        }

        mesh.gameObject.SetActive(false);
    }

    public void SetPath(Vector3 start, Vector3 end, OnDestination onDest)
    {
        this.start = start;
        this.end = end;
        this.onDest = onDest;
    }




}
