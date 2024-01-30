using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtObject : MonoBehaviour
{
    public float lookTime = 0f;
    public int lookCount = 0;
    private bool isUserLooking = false;

    private void Start()
    {
        // Register this object with the LookAtManager
        LookAtManager.Instance.RegisterObject(this);
    }

    public void UserLookAt()
    {
        if (!isUserLooking)
        {
            lookCount++;
        }
        isUserLooking = true;
    }

    public void UserLookAway()
    {
        isUserLooking = false;
    }

    void Update()
    {
        // If the user is looking at this game object, increment the look time
        if (isUserLooking)
        {
            lookTime += Time.deltaTime;
        }
    }
}
