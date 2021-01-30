using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(NavMeshAgent))]
public class NavMeshAgentActions : MonoBehaviour
{
    public UnityEvent movingStart;

    public UnityEvent movingEnd;

    private NavMeshAgent _agent;

    private bool _moving;

    // Start is called before the first frame update
    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    private void Update()
    {
        // Check for moving
        var moving = _agent.hasPath || _agent.pathPending;

        // Skip if no change
        if (moving == _moving)
            return;

        // Update state to match change
        _moving = moving;

        // Report event for change
        if (moving)
            movingStart?.Invoke();
        else
            movingEnd?.Invoke();
    }

    public void SetDestination(Transform destination)
    {
        _agent.SetDestination(destination.position);
    }
}
