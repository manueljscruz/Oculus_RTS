using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeonRangeHandler : MonoBehaviour
{
    // Parameters

    // Cached
    private Gatherer gatherer;

    // States


    // Start is called before the first frame update
    void Start()
    {
        gatherer = transform.parent.GetComponent<Gatherer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Resources
        if (other.gameObject.tag == "Resource")
            gatherer.onResourceDetected(other.gameObject);
        
        // Town Centers
        if(other.gameObject.tag == "PlayerTC")
            gatherer.onBuildingDetected(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Resource") 
            gatherer.onResourceEscape(other.gameObject);

        if (other.gameObject.tag == "PlayerTC")
            gatherer.onBuildingEscape(other.gameObject);
    }
}
