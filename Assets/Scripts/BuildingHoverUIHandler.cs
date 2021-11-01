using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingHoverUIHandler : MonoBehaviour
{
    // Parameters

    // Cached
    private Camera cameraToLookAt;
    private Canvas canvas;

    // State

    // Start is called before the first frame update
    void Start()
    {
        cameraToLookAt = Camera.main;
        canvas = GetComponent<Canvas>();
        canvas.worldCamera = cameraToLookAt;
    }

    // Update is called once per frame
    void Update()
    {
        // If object is enabled
        if (this.enabled)
        {
            RotateUI();
        }
    }

    private void RotateUI()
    {
        Vector3 v = cameraToLookAt.transform.position - transform.position;
        v.x = v.z = 0.0f;
        transform.LookAt(cameraToLookAt.transform.position - v);
        transform.Rotate(0, 180, 0);
    }
}
