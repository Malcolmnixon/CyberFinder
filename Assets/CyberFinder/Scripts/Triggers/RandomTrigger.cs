using UnityEngine;
using UnityEngine.Events;

public class RandomTrigger : MonoBehaviour
{
    [Tooltip("Minimum time between random triggers")]
    public float minTime;

    [Tooltip("Range of randomness")]
    public float maxTime;

    [Tooltip("Active flag")]
    public bool active;

    [Tooltip("Repeat random trigger")]
    public bool repeat;

    [Tooltip("Random trigger event")]
    public UnityEvent onRandom;

    /// <summary>
    /// Calculated next random time
    /// </summary>
    private float _nextTime = -1F;

    /// <summary>
    /// Integrated current time
    /// </summary>
    private float _integrate;

    private void Update()
    {
        // Skip if not active
        if (!active)
            return;

        // Prepare if not ready
        if (_nextTime < minTime || _nextTime > maxTime)
        {
            _nextTime = Random.Range(minTime, maxTime);
            _integrate = 0F;
        }

        // Track time to next random
        _integrate += Time.deltaTime;
        if (_integrate < _nextTime)
            return;

        // Update state for next
        if (repeat)
            _nextTime = -1F;
        else
            active = false;

        // Deliver event
        onRandom?.Invoke();
    }
}
