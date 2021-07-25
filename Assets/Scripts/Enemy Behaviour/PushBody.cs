using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//I separated this script from enemy controller because might cause problems if enemy dies on push body
public class PushBody : MonoBehaviour
{

    //Pushes back body from a source in a specified direction
    public void Push(Vector3 dir, float force, float duration = 0.2f)
    {
        NavMeshAgent agent = this.GetComponent<NavMeshAgent>();
        if (agent != null) agent.velocity = dir.normalized * force;

        Invoke("Stop", duration);
    }

    private void Stop()
    {
        NavMeshAgent agent = this.GetComponent<NavMeshAgent>();
        if (agent != null) agent.velocity = Vector3.zero;
    }
}
