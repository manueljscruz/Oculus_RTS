using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Gatherer : MonoBehaviour
{
    // Parameters
    [SerializeField] float timeBetweenGather = 2f;
    [SerializeField] int gatherQuantity = 1;
    [SerializeField] float resourceRangeSearch = 10f;
    [SerializeField] float rotationSpeed = 1f;
    public LayerMask resourceLayerMak;

    // Cached
    private PeonInventory inventory;
    private List<GameObject> resourceTargets;                   // Resource Targets in Range
    private List<GameObject> buildingTargets;                   // Player Buildings in Range
    private List<Collider> resourcesInSearchRange;              // Resources in Search Range
    private GameObject resourceTarget;                          // Current Resource Target
    private ResourceType lastResourceType;                      // Last Resource Type Gathered
    private GameObject buildingTarget;                          // Current Building Target
    private GatherStates currentGatherState;                    // Current Gatherer State
    private float time;                                         // Time elapsed since scene started
    private float nextGatherTime = 0f;                          // Next time the peon can gather a resource
    private PeonControlAgent controlAgent;                      // Instance Control Agent of the Peon
    private NavMeshAgent navMeshAgent;
    private Vector3 previousAdjustment;

    // States
    public enum GatherStates
    {
        Idle,
        MoveToResource,
        Gathering,
        MoveToDeposit,
        DeliverResources,
        SearchForResource
    }

    // Start is called before the first frame update
    void Start()
    {
        this.controlAgent = this.GetComponent<PeonControlAgent>();
        this.navMeshAgent = this.GetComponent<NavMeshAgent>();
        inventory = GetComponent<PeonInventory>();
        resourceTargets = new List<GameObject>();
        buildingTargets = new List<GameObject>();
        resourcesInSearchRange = new List<Collider>();
        currentGatherState = GatherStates.Idle;
        
    }

    private void Update()
    {
        time = Time.time;
        StateControl();
    }

    #region State Control

    /// <summary>
    /// Handles the State of the Gathering
    /// </summary>
    void StateControl()
    {
        switch (currentGatherState)
        {
            case GatherStates.Idle:
                break;

            case GatherStates.MoveToResource:
                // If there is a target
                if (resourceTarget != null)
                {
                    // If in Range
                    if (TargetResourceInRange(resourceTarget))
                    {
                        if (CheckIfMoving()) navMeshAgent.velocity = Vector3.zero;
                        // Switch State
                        ChangeGathererState(GatherStates.Gathering);
                    }
                    else AdjustAngle();
                }
                else
                {
                    ChangeGathererState(GatherStates.SearchForResource);
                }
                break;

            case GatherStates.Gathering:
                // StartCoroutine(GatherResource());
                if(resourceTarget != null)
                {
                    AdjustAngle();
                    GatherResource();
                }
                else
                {
                    ChangeGathererState(GatherStates.SearchForResource);
                }
                
                break;

            case GatherStates.MoveToDeposit:
                if(buildingTarget != null)
                {
                    if (TargetBuildingInRange(buildingTarget))
                    {
                        ChangeGathererState(GatherStates.DeliverResources);
                    }
                }
                break;

            case GatherStates.DeliverResources:
                if(buildingTarget != null && TargetBuildingInRange(buildingTarget))
                {
                    DeliverResource();
                    if(resourceTarget != null)
                    {
                        controlAgent.MoveAgent(resourceTarget.transform.position);
                        ChangeGathererState(GatherStates.MoveToResource);
                    }
                    else
                    {
                        ChangeGathererState(GatherStates.SearchForResource);
                    }
                }
                else
                {
                    return;
                }
                break;

            case GatherStates.SearchForResource:
                SearchForNearestResources();
                break;
        }
    }

    #endregion

    #region Gather Resource

    /// <summary>
    /// Execute Gather action
    /// </summary>
    public void GatherResource()
    {
        // Check if there is a target and the inventory is not full 
        if (!inventory.IsInventoryFull() && resourceTarget != null)
        {
            // If current target is in range
            if (TargetResourceInRange(resourceTarget))
            {
                // yield return new WaitForSeconds(nextGatherTime);
                // check if gather time is ready or next gather time is up
                
                if (nextGatherTime == 0 || time >= nextGatherTime)
                {
                
                    // Get Resource Handler of target
                    ResourceHandler targetResourceHandler = resourceTarget.GetComponent<ResourceHandler>();

                    // Get 
                    ResourceType resourceType = targetResourceHandler.ReturnTypeResource();             // Resource Type
                    int currentResourceQty = targetResourceHandler.ReturnCurrentQuantity();     // Current Resource Quantity

                    // Assess current inventory versus current quantity available at the resource node
                    // Versus current gathering capacity (1 per tick, 2 per tick )

                    if (!inventory.IsInventoryFull()) //&& currentEmptySlots >= currentResourceQty)
                    {
                        // Add to inventory the quantity respectivly and withraw 
                        targetResourceHandler.Gather(gatherQuantity);
                        inventory.AddResourceToInv(resourceType, gatherQuantity);
                        // string strMessage = string.Format("Gathered: {0} -> {1} units", resourceType.ToString(), gatherQuantity.ToString());
                        // Debug.Log(strMessage);
                        nextGatherTime = time + timeBetweenGather;
                        // string strMessage2 = string.Format("Next Gather Time: {0} ", nextGatherTime);
                        // Debug.Log(strMessage2);
                    }
                
                }
        
            }

        }
        else
        {
            // Debug.Log("Inventory Full");
            // Search for nearest repository
            // Once located, move peon
            nextGatherTime = 0;
            if(buildingTarget == null) SearchForNearestRepositories();

            Collider buildingCollider = buildingTarget.GetComponent<Collider>();
            Vector3 closestPoint = buildingCollider.ClosestPoint(this.transform.position);

            controlAgent.MoveAgent(closestPoint);
            ChangeGathererState(GatherStates.MoveToDeposit);
        }

        #region previous code
        /*
            // Get Resource Handler of target
                ResourceHandler targetResourceHandler = resourceTarget.GetComponent<ResourceHandler>();

                // Get 
                Type resourceType = targetResourceHandler.ReturnTypeResource();             // Resource Type
                int currentResourceQty = targetResourceHandler.ReturnCurrentQuantity();     // Current Resource Quantity

                // Gather
                // OPINION of Optimization???
                // If inventory empty slots can support gather quantities and current resource quantities
                // 3 slots vazios -> 2 resource quantities
                if (!inventory.IsInventoryFull() && currentEmptySlots >= currentResourceQty)
                {
                    // If there is enough resources to gather by the peon capacity
                    if ((currentResourceQty - gatherQuantity) >= 0)
                    {
                        // Add to inventory the quantity respectivly and withraw 
                        targetResourceHandler.Gather(gatherQuantity);
                        inventory.AddResourceToInv(resourceType, gatherQuantity);
                        string strMessage = string.Format("Gathered: {0} -> {1} units", resourceType.ToString(), gatherQuantity.ToString());
                        Debug.Log(strMessage);
                        nextGatherTime = time + timeBetweenGather;
                    }
                    else
                    {
                        // Add whats left of the resource
                        targetResourceHandler.Gather(currentResourceQty);
                        inventory.AddResourceToInv(resourceType, currentResourceQty);
                        string strMessage = string.Format("Gathered: {0} -> {1} units", resourceType.ToString(), gatherQuantity.ToString());
                        Debug.Log(strMessage);
                        nextGatherTime = 0;
                        Debug.Log("No more resource to gather");
                        // No more resource, so change for other state
                        ChangeGathererState(GatherStates.SearchForResource);
                    }
                }
                else
                {
                    // if current empty slots are less or the same as the current resource quantity
                    if (currentEmptySlots <= currentResourceQty)
                    {
                        // fill the last slots
                        targetResourceHandler.Gather(currentEmptySlots);
                        inventory.AddResourceToInv(resourceType, currentEmptySlots);
                        string strMessage = string.Format("Gathered: {0} -> {1} units", resourceType.ToString(), gatherQuantity.ToString());
                        Debug.Log(strMessage);
                        Debug.Log("Inventory is full, moving to Deposit");
                        ChangeGathererState(GatherStates.MoveToDeposit);
                    }
                    // Less resources than slots, add just the remaining quantity
                    else
                    {
                        targetResourceHandler.Gather(currentResourceQty);
                        inventory.AddResourceToInv(resourceType, currentResourceQty);
                        ChangeGathererState(GatherStates.SearchForResource);
                        Debug.Log("No more resource to gather");
                    }
                }
            }
        */
        #endregion


    }

    #endregion

    #region Deliver Resource

    /// <summary>
    /// Deliver Resources to the repository
    /// </summary>
    public void DeliverResource()
    {
        inventory.DeliverResources();
        // Debug.Log("Delivered Resources");
    }

    #endregion

    #region On Resource Detected

    /// <summary>
    /// When resource enters in range, add to list
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public bool onResourceDetected(GameObject target)
    {
        // this.controlAgent.ChangePeonControlState(PeonControlAgent.PeonState.Gathering);
        // if target exists in list
        if (resourceTargets.Find(x => x == target) == null)
        {
            resourceTargets.Add(target);
        }

        return true;
    }
    #endregion

    #region On Resource Escape
    /// <summary>
    /// When Resource leaves, add to list
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public bool onResourceEscape(GameObject target)
    {
        // Debug.Log("Resource Leaving");
        // if target exists in list
        if (resourceTargets.Find(x => x == target) != null)
        {
            resourceTargets.Remove(target);
        }

        return true;
    }
    #endregion

    #region On Building Detected

    public bool onBuildingDetected(GameObject target)
    {
        if(buildingTargets.Find(x => x == target) == null)
        {
            buildingTargets.Add(target);
        }
        return true;
    }

    #endregion

    #region On Building Escape

    /// <summary>
    /// When a building leaves the range of the gatherer
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public bool onBuildingEscape(GameObject target)
    {
        // Debug.Log("Building Leaving");
        // if target exists in list
        if (buildingTargets.Find(x => x == target) != null)
        {
            buildingTargets.Remove(target);
        }

        return true;
    }

    #endregion

    #region Assign Resource Target

    /// <summary>
    /// Assign resource target to gatherer
    /// </summary>
    /// <param name="target"></param>
    public void AssignResourceTarget(GameObject target)
    {
        resourceTarget = target;
        lastResourceType = target.GetComponent<ResourceHandler>().resourceType;
        // Debug.Log("Gatherer : Resource Locked");
    }

    #endregion

    #region Search For Nearest Repositories

    /// <summary>
    /// Search for nearest repositories for resource delivery 
    /// </summary>
    private void SearchForNearestRepositories()
    {
        GameObject[] repositories = GameObject.FindGameObjectsWithTag("PlayerTC");
        // if there are repositories
        if (repositories.Length != 0)
        {
            // Get all repositories and their distance to the peon current position
            Dictionary<GameObject, float> repositoriesDistances = new Dictionary<GameObject, float>();
            foreach(GameObject building in repositories)
            {
                float dist = Vector3.Distance(this.transform.position, building.transform.position);
                repositoriesDistances.Add(building, dist);
            }

            // Assign automatic minimum
            GameObject nearestRepository = repositoriesDistances.ElementAt(0).Key;
            float minimumValue = repositoriesDistances.ElementAt(0).Value;
            for(int i = 0; i < repositoriesDistances.Count; i++)
            {
                if(repositoriesDistances.ElementAt(i).Value < minimumValue)
                {
                    nearestRepository = repositoriesDistances.ElementAt(i).Key;
                    minimumValue = repositoriesDistances.ElementAt(i).Value;
                }
            }

            buildingTarget = nearestRepository;

        }
        // cant do nothing without a repository
        else ChangeGathererState(GatherStates.Idle);
    }
    #endregion

    public bool IsPeonGathering()
    {
        return currentGatherState == GatherStates.Gathering ? true : false;
    }

    #region Search For Nearest Resources

    /// <summary>
    /// Search for nearest resources of the same time
    /// </summary>
    private void SearchForNearestResources()
    {
        // Clear all
        resourcesInSearchRange.Clear();

        // Get all resources in range
        resourcesInSearchRange = Physics.OverlapSphere(transform.position, resourceRangeSearch, resourceLayerMak).ToList();

        List<GameObject> resourcesDetected = new List<GameObject>();
        foreach(Collider collision in resourcesInSearchRange)
        {
            if(collision.gameObject.tag == "Resource")
            {
                if (collision.gameObject.GetComponent<ResourceHandler>().resourceType == lastResourceType) resourcesDetected.Add(collision.gameObject);
            }
        }

        // Get Closest One, assign and change state
        if(resourcesDetected.Count != 0)
        {
            // Get all resources and their distance to the peon current position
            Dictionary<GameObject, float> repositoriesDistances = new Dictionary<GameObject, float>();
            foreach (GameObject resource in resourcesDetected)
            {
                float dist = Vector3.Distance(this.transform.position, resource.transform.position);
                repositoriesDistances.Add(resource, dist);
            }

            // Assign automatic minimum
            GameObject nearestResource = repositoriesDistances.ElementAt(0).Key;
            float minimumValue = repositoriesDistances.ElementAt(0).Value;
            for (int i = 0; i < repositoriesDistances.Count; i++)
            {
                if (repositoriesDistances.ElementAt(i).Value < minimumValue)
                {
                    nearestResource = repositoriesDistances.ElementAt(i).Key;
                    minimumValue = repositoriesDistances.ElementAt(i).Value;
                }
            }

            resourceTarget = nearestResource;
            controlAgent.MoveAgent(resourceTarget.transform.position);
            ChangeGathererState(GatherStates.MoveToResource);
        }
        else
        {
            ChangeGathererState(GatherStates.Idle);
        }
    }

    #endregion

    #region Target Resource in Range

    /// <summary>
    /// Checks if target resource is in range
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public bool TargetResourceInRange(GameObject target)
    {
        bool bResult = false;
        foreach (GameObject resource in resourceTargets)
        {
            if (resource == target)
            {
                bResult = true;
                break;
            }
        }
        return bResult;
    }
    #endregion

    #region Target Building in Range

    /// <summary>
    /// Checks if the current target building for resource delivery is in range
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public bool TargetBuildingInRange(GameObject target)
    {
        bool result = false;
        foreach(GameObject building in buildingTargets)
        {
            if (building == target)
            {
                result = true;
                break;
            }
        }
        return result;
    }

    #endregion

    #region Change Gatherer State

    /// <summary>
    /// Change Between Gathering states depending on necessity
    /// Apply changes if necessary
    /// </summary>
    /// <param name="nextGatherState"></param>
    public void ChangeGathererState(GatherStates nextGatherState)
    {
        if (this.currentGatherState == nextGatherState) return;
        switch (currentGatherState)
        {
            case GatherStates.Idle:
                break;

            case GatherStates.MoveToResource:
                break;

            case GatherStates.Gathering:
                nextGatherTime = 0;     // Reset Gather Time
                break;

            case GatherStates.MoveToDeposit:
                break;

            case GatherStates.DeliverResources:
                break;

            case GatherStates.SearchForResource:
                break;
        }
        currentGatherState = nextGatherState;   // Change Gather State
        // Debug.Log("Changed to ->: " + currentGatherState);
    }

    #endregion

    //private void RotateToFaceTarget()
    //{
    //    Vector3 v = resourceTarget.transform.position - transform.position;
    //    v.x = v.z = 0.0f;
    //    transform.LookAt(resourceTarget.transform.position - v);
    //    transform.Rotate(0, 180, 0);
    //}

    private void AdjustAngle()
    {
        if(resourceTarget != null)
        {
            Quaternion lookRotation;
            Vector3 direction;

            direction = (resourceTarget.transform.position - transform.position).normalized;
            lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

            if (previousAdjustment == Vector3.zero)
            {
                previousAdjustment = new Vector3(direction.x, direction.y, direction.z);
            }
            else
            {
                if (direction == previousAdjustment) ;// ChangePeonControlState(PeonState.Idle);
                else previousAdjustment = direction;
            }
        }
    }

    bool CheckIfMoving()
    {
        if (navMeshAgent.velocity != Vector3.zero) return true;
        else return false;
    }
}