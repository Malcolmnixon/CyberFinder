using UnityEngine;
using UnityEngine.Events;

public class ColliderTriggerEnter : MonoBehaviour
{
    public string filter;

    public UnityEvent onEnter;

    private void OnTriggerEnter(Collider c)
    {
        if (c.name.Contains(filter))
            onEnter?.Invoke();
    }
}
