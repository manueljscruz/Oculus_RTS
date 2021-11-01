using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PeonControlAgent : MonoBehaviour
{
    // Parameters
    [SerializeField] float rotationSpeed = 1f;

    // Cached
    private NavMeshAgent navMeshAgent;
    private Transform currentPosition;
    private Gatherer gatherer;
    private PeonInventory inventory;
    private static GameObject currentTarget;


    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // currentState = PeonState.Idle;
        gatherer = GetComponent<Gatherer>();
    }

    // Update is called once per frame
    void Update()
    {

    }

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

        if (currentTarget.tag == "Resource")
        {
            // Assign target in Gatherer 
            gatherer.AssignResourceTarget(currentTarget);

            MoveAgent(hitInfo.point);
            gatherer.ChangeGathererState(Gatherer.GatherStates.MoveToResource);
                
        }
    }


    public void MoveAgent(Vector3 pointDestination)
    {
        navMeshAgent.SetDestination(pointDestination);
    }

}
