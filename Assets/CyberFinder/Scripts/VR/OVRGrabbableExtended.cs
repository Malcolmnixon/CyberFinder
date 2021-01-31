using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class GrabBeginEvent : UnityEvent<OVRGrabbableExtended, OVRGrabber, Collider>
{
}

[Serializable]
public class GrabEndEvent : UnityEvent<OVRGrabbableExtended, Vector3, Vector3>
{
}

public class OVRGrabbableExtended : OVRGrabbable
{
    [Tooltip("Events to fire on grab begin")]
    public GrabBeginEvent onGrabBegin;

    [Tooltip("Events to fire on grab end")]
    public GrabEndEvent onGrabEnd;

    public override void GrabBegin(OVRGrabber hand, Collider grabPoint)
    {
        base.GrabBegin(hand, grabPoint);
        onGrabBegin?.Invoke(this, hand, grabPoint);
    }

    public override void GrabEnd(Vector3 linearVelocity, Vector3 angularVelocity)
    {
        base.GrabEnd(linearVelocity, angularVelocity);
        onGrabEnd?.Invoke(this, linearVelocity, angularVelocity);
    }
}
