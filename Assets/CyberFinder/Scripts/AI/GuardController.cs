using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
public class GuardController : MonoBehaviour
{
    private enum State
    {
        // Waiting at way-point before walking to the next patrol point
        DwellingAtWayPoint,

        // Walking to next patrol way-point
        WalkingToWayPoint,

        // Actively pursuing player the guard knows the location of
        PlayerPursuit,

        // Attacking the player
        PlayerAttack,

        // Lost player, but still pursuing to last location
        HistoricPlayerPursuit,

        // Lost player, waiting to see if the player sticks his head out
        LostPlayer
    }

    [Tooltip("Player to search for")]
    public GameObject player;

    [Tooltip("Eye location of guard")]
    public Transform guardEye;

    [Tooltip("List of visible target hits on the player")]
    public Transform[] visibleTargets;

    [Tooltip("List of points for the patrol route")]
    public Transform[] patrolRoute;

    [Tooltip("How long to wait at each patrol way-point")]
    public float wayPointDwell = 5F;

    [Tooltip("How long to search after player has evaded guard")]
    public float searchTimeout = 15F;

    [Tooltip("Range of the guards vision")]
    public float visionRange = 10F;

    [Tooltip("Guard field of view")]
    public float fieldOfView = 70F;

    [Tooltip("Range where the guard can attack")]
    public float attackRange = 1F;

    [Tooltip("Speed at which guard patrols")]
    public float patrolSpeed = 1F;

    [Tooltip("Speed at which guard pursues player")]
    public float pursueSpeed = 2F;

    [Tooltip("Duration guard can track player without seeing them")]
    public float trackingDuration = 5F;

    [Tooltip("Event for beginning pursuit")]
    public UnityEvent beginPursutEvent;

    [Tooltip("Event for beginning attack")]
    public UnityEvent beginAttackEvent;

    private Animator _animator;

    private NavMeshAgent _agent;

    private State _state = State.DwellingAtWayPoint;

    private int _nextPatrol = 0;

    private float _stateTime = 0F;

    // Start is called before the first frame update
    private void Start()
    {
        _animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    private void Update()
    {
        // Advance time
        _stateTime += Time.deltaTime;

        // Get the vector to the player
        var toPlayer = player.transform.position - transform.position;
        var playerDistance = toPlayer.magnitude;

        // Evaluate if we are in visible range
        var inVisibleRange = playerDistance < visionRange;

        // Evaluate if we are in attack range
        var inAttackRange = playerDistance < attackRange;

        // Evaluate if we are facing player
        var facingPlayer = Vector3.Angle(transform.forward, toPlayer) < fieldOfView;

        // Evaluate if we can see the player
        var canSeePlayer = inAttackRange;
        if (facingPlayer && !canSeePlayer)
            foreach (var target in visibleTargets)
                if (Physics.Raycast(guardEye.position, toPlayer, out var hit))
                    if (hit.distance < visionRange && hit.collider.transform.IsChildOf(player.transform))
                    {
                        canSeePlayer = true;
                        break;
                    }

        // Handle seeing the player
        if (canSeePlayer)
        {
            // Transition to pursuing player and reset timer
            if (_state != State.PlayerPursuit && _state != State.PlayerAttack)
            {
                _state = State.PlayerPursuit;
                _animator.SetBool("isWalking", false);
                _animator.SetBool("isPursuing", true);
                _agent.speed = pursueSpeed;

                // Invoke the begin-pursuit event
                beginPursutEvent?.Invoke();
            }

            // Clear timer
            _stateTime = 0F;
        }

        switch (_state)
        {
            case State.DwellingAtWayPoint:
                // Skip if more time
                if (_stateTime < wayPointDwell)
                    return;

                // Skip if no patrol route
                if (patrolRoute == null || patrolRoute.Length == 0)
                    return;

                // Set the destination and advance the next patrol point
                _agent.speed = patrolSpeed;
                _agent.SetDestination(patrolRoute[_nextPatrol].position);
                _nextPatrol = (_nextPatrol + 1) % patrolRoute.Length;
                _state = State.WalkingToWayPoint;
                _stateTime = 0F;

                // Tell the animator we are walking
                _animator.SetBool("isWalking", true);
                break;

            case State.WalkingToWayPoint:
                // Skip if still moving
                if (_agent.hasPath || _agent.pathPending)
                    return;

                // Tell the animator we have stopped walking
                _animator.SetBool("isWalking", false);

                // Start dwelling
                _state = State.DwellingAtWayPoint;
                _stateTime = 0F;
                break;

            case State.PlayerPursuit:
                // Test if we've lost track of the player
                if (_stateTime > trackingDuration)
                {
                    _state = State.HistoricPlayerPursuit;
                    _stateTime = 0F;
                    return;
                }

                // Attack if close
                if (inAttackRange)
                {
                    _animator.SetBool("isAttacking", true);
                    _state = State.PlayerAttack;
                    _stateTime = 0F;

                    // Report attacking
                    beginAttackEvent?.Invoke();
                }

                // Still tracking the player
                _agent.SetDestination(player.transform.position);
                break;

            case State.PlayerAttack:
                // If player is out of range then transition back to pursuit and stop attacking
                if (!inAttackRange)
                {
                    _animator.SetBool("isAttacking", false);
                    _state = State.PlayerPursuit;
                    _stateTime = 0F;
                }

                // Still tracking the player
                _agent.SetDestination(player.transform.position);
                break;

            case State.HistoricPlayerPursuit:
                // Skip if still moving
                if (_agent.hasPath || _agent.pathPending)
                    return;

                // Got to the last player location
                _animator.SetBool("isPursuing", false);
                _state = State.LostPlayer;
                _stateTime = 0F;
                break;

            case State.LostPlayer:
                // Skip if still searching
                if (_stateTime < searchTimeout)
                    return;

                // Act like the end of a way-point dwell
                _state = State.DwellingAtWayPoint;
                _stateTime = wayPointDwell;
                break;
        }
    }
}
