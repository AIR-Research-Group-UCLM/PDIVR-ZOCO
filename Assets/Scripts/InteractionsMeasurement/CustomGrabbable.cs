using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OculusSampleFramework;

public class CustomGrabbable : OVRGrabbable
{
    public float holdTime = 0f;
    public int grabCount = 0;
    private bool isUserHolding = false;

    protected override void Start()
    {
        base.Start();
        
        LookAtManager.Instance.RegisterGrabbableObject(this);
    }

    public override void GrabBegin(OVRGrabber hand, Collider grabPoint)
    {
        base.GrabBegin(hand, grabPoint);
        isUserHolding = true;
        grabCount++;
    }

    public override void GrabEnd(Vector3 linearVelocity, Vector3 angularVelocity)
    {
        base.GrabEnd(linearVelocity, angularVelocity);
        isUserHolding = false;
    }

    void Update()
    {
        // If the user is holding this game object, increment the hold time
        if (isUserHolding)
        {
            holdTime += Time.deltaTime;
        }
    }
}
