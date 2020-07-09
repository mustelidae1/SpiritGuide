using System.Collections;
using System.Collections.Generic;
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

    public TextMeshProUGUI debugText;

    private void Start()
    {
        distances = new float[numRaycasts]; 
    }
    void Update()
    {
        int layer_mask = LayerMask.GetMask("Spatial Awareness");  // physics layer of the spatial mesh 

        RaycastHit hit;

        float increment = 360f / numRaycasts;  // angle between each raycast 
        float currentAngle = 0; 
        int currentRaycastIndex = 0; 

        // NOTE: The raycasts currently follow rotation of the head on all axes. Do we want that?
        // Ideally would we want it to follow body rotation instead? 
        while (currentAngle < 360)
        {
            Vector3 direction = new Vector3(Mathf.Sin(Mathf.Deg2Rad * currentAngle), 0, Mathf.Cos(Mathf.Deg2Rad * currentAngle)); 
            Debug.DrawLine(transform.position, transform.rotation * direction * 10, Color.red);
            if (Physics.Raycast(transform.position, transform.rotation * direction, out hit, Mathf.Infinity, layer_mask))
            {
                //debugText.text = "HIT " + hit.distance;
                distances[currentRaycastIndex] = hit.distance; 
            }
            else
            {
                //debugText.text = "NO HIT";
            }
            currentAngle += increment;
            currentRaycastIndex++; 
        }

        
    }
}
