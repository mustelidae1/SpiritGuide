using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Net;
using System.Security.Policy;
using System;

/// <summary>
/// Creates an equidistant set of raycasts emanating from the GameObject
/// and stores the distance between each raycast and the HoloLens 
/// spatial mesh. 
/// Author: Olivia Thomas
/// </summary>
public class Raycaster : MonoBehaviour
{
    public int numRaycasts; // configure this to set number of raycasts 
    private float[] distances; // array updated with distances to spatial mesh 
    private Vector3[] directions; 

    // we will send them as euler angles 

    public float maxDistance;
    public string beltUrl; 
    public TextMeshProUGUI debugTextFarRight;
    public TextMeshProUGUI debugTextRight;
    public TextMeshProUGUI debugTextLeft;
    public TextMeshProUGUI debugTextFarLeft;

    private float increment; // angle between each raycast 

    private void Start()
    {
        distances = new float[numRaycasts];
        directions = new Vector3[numRaycasts]; 

        // Hardcoded version for specific angles 
        float[] angles = new float[] { 80, 45, 315, 280 };
        numRaycasts = angles.Length; 
        for (int i = 0; i < angles.Length; i++)
        {
            float currentAngle = angles[i]; 
            directions[i] = new Vector3(Mathf.Sin(Mathf.Deg2Rad * currentAngle), 0, Mathf.Cos(Mathf.Deg2Rad * currentAngle));
        }

        // Use the code below to create equidistant rays instead of hardcoding it 
        //increment = 360f / numRaycasts;
        //float currentAngle = 0; 
        //for (int i = 0; i < numRaycasts; i++)
        //{
        //    directions[i] = new Vector3(Mathf.Sin(Mathf.Deg2Rad * currentAngle), 0, Mathf.Cos(Mathf.Deg2Rad * currentAngle));
        //    currentAngle += increment;
        //}
        Invoke("CallBeltEndpoints", 0.25f); 
    }
    void Update()
    {
        int layer_mask = LayerMask.GetMask("Spatial Awareness");  // physics layer of the spatial mesh 

        RaycastHit hit;
        for (int i = 0; i < numRaycasts; i++)
        {
            float smallestDistance = Mathf.Infinity; // if there are multiple rays for each direction, we use this value to store the shortest direction 
            for (int j = 5; j < 6; j++) // this loop can be used to cast rays at different distances from the head if desired (j = 0 is at head level, j = 5 is 1.25 meters down from head) 
            {
                Quaternion headRotation = transform.rotation; // raw rotation of the user's head 
                Quaternion headRotationYOnly = Quaternion.Euler(0, headRotation.eulerAngles.y, 0);  // rotation of the user's head with only y euler angles  
                
                Vector3 raycastRotation = headRotationYOnly * directions[i]; // Vector3 with correct angle from user's head for raycast 

                Debug.DrawLine(new Vector3(transform.position.x, transform.position.y - (j * 0.25f), transform.position.z), raycastRotation * maxDistance, i == 0 ? Color.red : Color.blue);
                // NOTE: All rays below head level are currently angled up. Worked okay for the demo but in the future should explore how to alter this to make the rays point straight forward. 

                if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y - (j * 0.25f), transform.position.z), raycastRotation, out hit, maxDistance, layer_mask))
                {
                    // text boxes hardcoded for now; should be refactored 
                    if (i == 0 && hit.distance < smallestDistance)
                    {
                        smallestDistance = hit.distance; 
                        debugTextFarRight.text = "" + System.Math.Round(hit.distance, 2) + "m";
                    }
                    else if (i == 1 && hit.distance < smallestDistance)
                    {
                        smallestDistance = hit.distance;
                        debugTextRight.text = "" + System.Math.Round(hit.distance, 2) + "m";
                    }
                    else if (i == 2 && hit.distance < smallestDistance)
                    {
                        smallestDistance = hit.distance;
                        debugTextLeft.text = "" + System.Math.Round(hit.distance, 2) + "m";
                    }
                    else if (i == 3 && hit.distance < smallestDistance)
                    {
                        smallestDistance = hit.distance;
                        debugTextFarLeft.text = "" + System.Math.Round(hit.distance, 2) + "m";
                    }

                    distances[i] = hit.distance;
                }
                else
                {
                    // text boxes hardcoded for now; should be refactored 
                    if (i == 0 && smallestDistance == Mathf.Infinity)
                    {
                        debugTextFarRight.text = "CLEAR";
                    }
                    else if (i == 1 && smallestDistance == Mathf.Infinity)
                    {
                        debugTextRight.text = "CLEAR";
                    }
                    else if (i == 2 && smallestDistance == Mathf.Infinity)
                    {
                        debugTextLeft.text = "CLEAR";
                    }
                    else if (i == 3 && smallestDistance == Mathf.Infinity)
                    {
                        debugTextFarLeft.text = "CLEAR";
                    }
                    distances[i] = -1;
                }
            }
                 
        }
    }

    void CallBeltEndpoints()
    {
        StartCoroutine(GetRequest(beltUrl + "/z")); // clear all buzzers 

        // This is hardcoded for our specific belt right now. Should be refactored to be more scalable.  
        for (int i = 0; i < distances.Length; i++)
        {
            float curDistance = distances[i];
            int endpointCharacterCode = i + (4 * i) + 65; // math to get to the right endpoint for our arbirary naming scheme 
            //if (curDistance > 2) // really slow - 2-2.5m 
            //{
            //    StartCoroutine(GetRequest(beltUrl + "/" + (char)endpointCharacterCode));
            //if (curDistance > 1.5) // slow - 1.5-2m 
            //{
            //    StartCoroutine(GetRequest(beltUrl + "/" + (char)(endpointCharacterCode + 1))); // adding an offset value, again just part of our alphabetical naming scheme
            if (curDistance > 0.5) // faster - 0.5-1m 
            {
                if (i == 0)
                {
                    debugTextFarRight.color = Color.yellow;
                }
                else if (i == 1)
                {
                    debugTextRight.color = Color.yellow;
                }
                else if (i == 2)
                {
                    debugTextLeft.color = Color.yellow;
                }
                else if (i == 3)
                {
                    debugTextFarLeft.color = Color.yellow;
                }
                StartCoroutine(GetRequest(beltUrl + "/" + (char)(endpointCharacterCode + 2)));
            } else if (curDistance > 0.25) // even faster - 0.25-0.5m 
            {
                if (i == 0)
                {
                    debugTextFarRight.color = new Color(255, 165, 0);
                }
                else if (i == 1)
                {
                    debugTextRight.color = new Color(255, 165, 0);
                }
                else if (i == 2)
                {
                    debugTextLeft.color = new Color(255, 165, 0);
                }
                else if (i == 3)
                {
                    debugTextFarLeft.color = new Color(255, 165, 0);
                }
                StartCoroutine(GetRequest(beltUrl + "/" + (char)(endpointCharacterCode + 3)));
            } else if (curDistance > 0) // extremely faster - 0-0.25m 
            {
                if (i == 0)
                {
                    debugTextFarRight.color = new Color32(255, 0, 0, 255); 
                }
                else if (i == 1)
                {
                    debugTextRight.color = new Color32(255, 0, 0, 255);
                }
                else if (i == 2)
                {
                    debugTextLeft.color = new Color32(255, 0, 0, 255);
                }
                else if (i == 3)
                {
                    debugTextFarLeft.color = new Color32(255, 0, 0, 255);
                }
                StartCoroutine(GetRequest(beltUrl + "/" + (char)(endpointCharacterCode + 4)));
            }
            else // clear - >2.5m 
            {
                // do nothing because all buzzers were reset at beginning of function call 
                if (i == 0)
                {
                    debugTextFarRight.color = Color.green;
                }
                else if (i == 1)
                {
                    debugTextRight.color = Color.green;
                }
                else if (i == 2)
                {
                    debugTextLeft.color = Color.green;
                }
                else if (i == 3)
                {
                    debugTextFarLeft.color = Color.green;
                }
            }
        }
        Invoke("CallBeltEndpoints", 0.25f); // invoke this function every 25ms 
    }

    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            yield return webRequest.SendWebRequest();
        }
    }
}
