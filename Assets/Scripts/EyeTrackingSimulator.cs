using System;
using TMPro;
using UnityEngine;

public class EyeTrackingSimulator : MonoBehaviour
{
    public GameObject rightEye;
    
    // Define a LayerMask for the table objects
    public LayerMask raycastable;
    
    private GameObject lastRightObject;

    public TextMeshProUGUI debugText;
    void Update()
    {
        Ray rightRay = new Ray(rightEye.transform.position, rightEye.transform.forward);

        RaycastHit hitInfo;

        // Include the layer mask as a parameter in the Physics.Raycast calls
        if (Physics.Raycast(rightRay, out hitInfo, Mathf.Infinity, raycastable))
        {
            if (lastRightObject != hitInfo.transform.gameObject)
            {
                lastRightObject = hitInfo.transform.gameObject;
                RegisterEyeContact("Right eye", lastRightObject);
            }
        }
        else
        {
            lastRightObject = null; // Reset when no object is in sight
            debugText.text = "";
        }
    }

    private void RegisterEyeContact(string eye, GameObject obj)
    {
        //InteractionRegistration.RegisterTeleportOrView(obj, "view")
        debugText.text = "";
    }
}

