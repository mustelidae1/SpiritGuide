using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Creates an equidistant set of raycasts emanating from the GameObject
/// and stores the distance between each raycast and the HoloLens 
/// spatial mesh. 
/// </summary>
public class Raycaster : MonoBehaviour
{
    public int numRaycasts; // configure this to set number of raycasts 
    private float[] distances; // array updated with distances to spatial mesh 
    private Vector3[] directions; 

    // we will send them as euler angles 

    public int maxDistance;
    public TextMeshProUGUI debugText;

    private float increment; // angle between each aycast 

    private void Start()
    {
        distances = new float[numRaycasts];
        directions = new Vector3[numRaycasts]; 
        increment = 360f / numRaycasts;
        float currentAngle = 0; 
        for (int i = 0; i < numRaycasts; i++)
        {
            directions[i] = new Vector3(Mathf.Sin(Mathf.Deg2Rad * currentAngle), 0, Mathf.Cos(Mathf.Deg2Rad * currentAngle));
            currentAngle += increment;
        }
    }
    void Update()
    {
        int layer_mask = LayerMask.GetMask("Spatial Awareness");  // physics layer of the spatial mesh 

        RaycastHit hit;
        for (int i = 0; i < numRaycasts; i++)
        {
            
            Quaternion headRotation = transform.rotation; // raw rotation of the user's head 
            Quaternion headRotationYOnly = Quaternion.Euler(0, headRotation.eulerAngles.y, 0);  // rotation of the user's head with only y euler angles                                                                                       
            Vector3 raycastRotation = headRotationYOnly * directions[i]; // Vector3 with correct angle from user's head for raycast 

            Debug.DrawLine(transform.position, raycastRotation * maxDistance, i == 0 ? Color.red : Color.blue);
            if (Physics.Raycast(transform.position, raycastRotation, out hit, maxDistance, layer_mask))
            {
                if (i == 0)
                {
                    debugText.text = "" + System.Math.Round(hit.distance, 2) + "m";
                }

                distances[i] = hit.distance; 
            }
            else
            {
                if (i == 0)
                {
                    debugText.text = "CLEAR";
                }
                distances[i] = -1; 
            }         
        }

        
    }
}
