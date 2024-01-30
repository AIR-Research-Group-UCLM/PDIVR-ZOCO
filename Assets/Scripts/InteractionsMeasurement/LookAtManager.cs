using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using System.IO;

public class LookAtManager : MonoBehaviour
{
    public static LookAtManager Instance { get; private set; }

    private Transform cameraTransform;
    private List<LookAtObject> objectsToTrack;
    private float timer = 0f;
    private List<CustomGrabbable> objectsToGrab;

    private void Awake()
    {
        // Ensure there is only one instance of LookAtManager
        if (Instance == null)
        {
            Instance = this;
            objectsToTrack = new List<LookAtObject>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Get the main camera transform from the XR Device
        cameraTransform = Camera.main.transform;
    }

    public void RegisterObject(LookAtObject obj)
    {
        objectsToTrack.Add(obj);
    }

    public void RegisterGrabbableObject(CustomGrabbable obj)
    {
        objectsToGrab.Add(obj);
    }

    void Update()
    {
        // Define a ray from the main camera forward
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        RaycastHit hit;

        // Perform the raycast
        if (Physics.Raycast(ray, out hit))
        {
            // If the raycast hits a tracked object, notify it that the user is looking
            LookAtObject hitObject = hit.transform.GetComponent<LookAtObject>();
            if (hitObject != null)
            {
                hitObject.UserLookAt();
            }

            // Notify all other objects that the user is not looking at them
            foreach (LookAtObject obj in objectsToTrack)
            {
                if (obj != hitObject)
                {
                    obj.UserLookAway();
                }
            }
        }
        else
        {
            // If the raycast hits nothing, notify all objects that the user is not looking
            foreach (LookAtObject obj in objectsToTrack)
            {
                obj.UserLookAway();
            }
        }

        // Increment timer
        timer += Time.deltaTime;

        // If a minute has passed, log data and reset timer
        if (timer >= 30f)
        {
            LogData();
            timer = 0f;
        }
    }

    private void LogData()
    {
        // Initialize a new dictionary to hold our data
        Dictionary<string, object> data = new Dictionary<string, object>();

        // Loop through all tracked objects
        foreach (LookAtObject obj in objectsToTrack)
        {
            // Create a new dictionary for this object's data
            Dictionary<string, float> objectData = new Dictionary<string, float>();
            objectData["lookTime"] = obj.lookTime;
            objectData["lookCount"] = obj.lookCount;

            // Add this object's data to the main data dictionary
            data[obj.name] = objectData;
        }

        // Loop through all grabbable objects
        //foreach (CustomGrabbable obj in objectsToGrab)
        //{
        //    // Create a new dictionary for this object's data
        //    Dictionary<string, float> objectData = new Dictionary<string, float>();
        //    objectData["holdTime"] = obj.holdTime;
        //    objectData["grabCount"] = obj.grabCount;

        //    // Add this object's data to the main data dictionary
        //    data[obj.name] = objectData;
        //}

        // Convert the data dictionary to a JSON string
        string jsonData = JsonUtility.ToJson(new Wrapper { m_Data = data });

        Debug.Log(jsonData);
        // Log the JSON data to a file
        string fname = System.DateTime.Now.ToString("HH-mm-ss") + "-log.json";
        string path = Path.Combine(Application.persistentDataPath, fname);
        File.WriteAllText(path, jsonData);
    }

    [System.Serializable]
    private class Wrapper
    {
        public Dictionary<string, object> m_Data;
    }
}
