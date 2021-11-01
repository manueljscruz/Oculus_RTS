using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SoldierControlAgent : MonoBehaviour
{
    // Parameters

    // Cached
    private NavMeshAgent navMeshAgent;
    private GameObject currentTarget;
    // Combat

    // States

    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Interprets an action based on a hit raycast
    /// </summary>
    /// <param name="hitInfo"></param>
    public void HandleCommand(RaycastHit hitInfo)
    {
        // Get Target of Command
        currentTarget = hitInfo.collider.gameObject;

        // Analyse target to assess what to do
        // If Ground
        if (currentTarget.layer == 8)
        {
            // Move Peon to destination
            MoveAgent(hitInfo.point);
        }

        else if(currentTarget.tag == "TowerDefense")
        {
            ComplexMoveAgent(hitInfo.collider.gameObject);
        }
    }

    public void MoveAgent(Vector3 pointDestination)
    {
        navMeshAgent.SetDestination(pointDestination);
    }

    public void ComplexMoveAgent(GameObject currentTarget)
    {
        Collider buildingCollider = currentTarget.GetComponent<Collider>();             // Get Collider
        Vector3 closestPoint = buildingCollider.ClosestPoint(this.transform.position);  // Get Closest Point
        MoveAgent(closestPoint);                                                        // Move Agent
    }
}