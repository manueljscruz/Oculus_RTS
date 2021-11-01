using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class SkeletonCombat : MonoBehaviour
{
    // Parameters
    [SerializeField] public SkeletonType skeletonType;
    [SerializeField] public int attackDamage = 8;
    [SerializeField] private float timeBetweenAttacks = 1.5f;
    [SerializeField] float rotationSpeed = 1f;
    public LayerMask layers;

    // Cached
    private SkeletonCombatState currentCombatState;                                 // Current Skeleton Combat State
    private List<GameObject> enemyTargetsInRange;                                   // Enemy targets in Range of the skeleton
    private List<GameObject> playerTownCenters;
    [SerializeField]
    private GameObject currentEnemyTarget;                                          // Current Enemy target of the skeleton
    private List<Collider> enemyDetector;
    // private Collider[] enemyDetector;                                               // List of colliders found in the range field
    private NavMeshAgent navMeshAgentComponent;                                     // Nav Mesh Component of the Soldier
    private SkeletonAnimationStateController skeletonAnimationStateController;      // Skeleton Animation State Controller component
    private float time;                                                             // Time elapsed since scene started
    private float nextAttackTime = 0f;                                              // Next time the Skeleton can attack the enemy
    private Vector3 previousAdjustment;

    // States
    public enum SkeletonType
    {
        Defensive,
        Eradicator
    }

    public enum SkeletonCombatState
    {
        Idle,
        SearchAndDestroy,
        MovingToTarget,
        Attacking
    }

    // Start is called before the first frame update
    void Start()
    {
        Setup();
        navMeshAgentComponent = GetComponent<NavMeshAgent>();
        enemyTargetsInRange = new List<GameObject>();
        enemyDetector = new List<Collider>();
        skeletonAnimationStateController = GetComponent<SkeletonAnimationStateController>();
        playerTownCenters = GameObject.FindGameObjectsWithTag("PlayerTC").ToList();
    }

    // Update is called once per frame
    void Update()
    {
        time = Time.time;
        ClearNulls();
        if (skeletonType == SkeletonType.Defensive) HandleDefensiveStateControl();
        else HandleEradicatorStateControl();
    }

    #region Methods

    #region Setup

    /// <summary>
    /// Setup Initial Combat States based on Skeleton Types
    /// </summary>
    private void Setup()
    {
        if(skeletonType == SkeletonType.Defensive)
        {
            currentCombatState = SkeletonCombatState.Idle;
        }
        else
        {
            currentCombatState = SkeletonCombatState.SearchAndDestroy;
            playerTownCenters = GameObject.FindGameObjectsWithTag("PlayerTC").ToList();
        }
    }

    #endregion

    #region State Control

    /// <summary>
    /// Handles Defensive Skeleton Behaviors
    /// </summary>
    private void HandleDefensiveStateControl()
    {
        switch (currentCombatState)
        {
            case SkeletonCombatState.Idle:
                CheckClosestEnemy();
                break;

            case SkeletonCombatState.MovingToTarget:
                if(currentEnemyTarget != null)
                {
                    if (CurrentTargetInRange())
                    {
                        if (skeletonAnimationStateController.CheckIfMoving()) navMeshAgentComponent.velocity = Vector3.zero;
                        currentCombatState = SkeletonCombatState.Attacking;
                    }
                    else
                    {
                        ComplexMoveSkeleton(currentEnemyTarget);
                        AdjustAngle(); // ComplexMoveSkeleton(currentEnemyTarget);
                    }
                }
                else
                {
                    currentCombatState = SkeletonCombatState.Idle;
                }
                break;

            case SkeletonCombatState.Attacking:
                AdjustAngle();
                HandleCombat();
                break;
        }
    }


    /// <summary>
    /// Handles Eradicators Skeleton Behaviors
    /// </summary>
    private void HandleEradicatorStateControl()
    {
        switch (currentCombatState)
        {
            case SkeletonCombatState.SearchAndDestroy:
                CheckClosestEnemy();
                if (currentEnemyTarget == null) GoToTownCenter();
                break;

            case SkeletonCombatState.MovingToTarget:

                CheckClosestEnemy();

                if (currentEnemyTarget != null)
                {
                    if (CurrentTargetInRange())
                    {
                        if (skeletonAnimationStateController.CheckIfMoving()) navMeshAgentComponent.velocity = Vector3.zero;
                        currentCombatState = SkeletonCombatState.Attacking;
                    }
                    else
                    {
                        AdjustAngle();
                        // CheckClosestEnemy();
                    }
                }
                else
                {
                    currentCombatState = SkeletonCombatState.SearchAndDestroy;
                }
                break;

            case SkeletonCombatState.Attacking:
                AdjustAngle();
                HandleCombat();
                break;
        }
    }

    #endregion

    #region Handle Combat

    /// <summary>
    /// Handles Combat for both types of enemys
    /// </summary>
    private void HandleCombat()
    {
        if(currentEnemyTarget != null)
        {
            if (CurrentTargetInRange())
            {
                if (nextAttackTime == 0 || time >= nextAttackTime)
                {
                    // Get health component from the enemy
                    Health targetHealthComponent = currentEnemyTarget.GetComponent<Health>();

                    // send a message to do attack animation
                    // skeletonAnimationStateController.PerformAttack();

                    // deliver damage
                    targetHealthComponent.LoseHealth(attackDamage);

                    // send a message to do attack animation
                    skeletonAnimationStateController.PerformAttack();

                    //Sound effect for attack
                    FindObjectOfType<SoundManager>().ChangeClip("UnitsFight");
                    FindObjectOfType<SoundManager>().PlaySound("UnitsFight", transform.position);

                    // register attack time
                    nextAttackTime = time + timeBetweenAttacks;
                }
            }
            else
            {
                // Check if target still exists
                if (currentEnemyTarget != null)
                {
                    // Move to it
                    ComplexMoveSkeleton(currentEnemyTarget);
                    currentCombatState = SkeletonCombatState.MovingToTarget;
                }
                // Target Destroyed, go back to Idle
                // TO DO: Consider Switching Targets if there are anymore in range
                else
                {
                    switch (skeletonType)
                    {
                        case SkeletonType.Defensive:
                            currentCombatState = SkeletonCombatState.Idle;
                            break;

                        case SkeletonType.Eradicator:
                            currentCombatState = SkeletonCombatState.SearchAndDestroy;
                            break;
                    }
                }
            }
        }
        else
        {
            skeletonAnimationStateController.StopAttack();

            switch (skeletonType)
            {
                case SkeletonType.Defensive: 
                    currentCombatState = SkeletonCombatState.Idle;
                    break;

                case SkeletonType.Eradicator:
                    currentCombatState = SkeletonCombatState.SearchAndDestroy;
                    break;
            }
        }
    }

    #endregion

    #region Check Closest Enemy

    /// <summary>
    /// Searches for nearest enemys
    /// </summary>
    private void CheckClosestEnemy()
    {
        // Move to it
        // Change state to moving to target
        enemyDetector = Physics.OverlapSphere(transform.position, 10.0f, layers).ToList();

        // If there is an enemy or more nearby
        if (enemyDetector.Count > 0)
        {
            // Assign minimum distance and current enemy target to the first instance
            float minDistance = (transform.position - enemyDetector[0].transform.position).magnitude;
            currentEnemyTarget = enemyDetector[0].gameObject;

            // Check 
            // int index = 0;
            foreach (Collider enemyCollider in enemyDetector)
            {
                //if (index != 0)
                //{
                    float currentDistance = (transform.position - enemyCollider.transform.position).magnitude;

                    if (currentDistance < minDistance)
                    {
                        minDistance = currentDistance;
                        currentEnemyTarget = enemyCollider.gameObject;
                    }
                //}
                //index++;
            }

            ComplexMoveSkeleton(currentEnemyTarget);
            currentCombatState = SkeletonCombatState.MovingToTarget;
        }
    }

    #endregion

    #region Go To Town Center

    private void GoToTownCenter()
    {
        playerTownCenters = GameObject.FindGameObjectsWithTag("PlayerTC").ToList();
        GameObject townCenter = playerTownCenters[Random.Range(0, playerTownCenters.Count)];

        ComplexMoveSkeleton(townCenter);
        currentEnemyTarget = townCenter;
        currentCombatState = SkeletonCombatState.MovingToTarget;
    }

    #endregion

    #region On Enemy In Range

    /// <summary>
    /// When an enemy enters my range of attack, add to my list
    /// </summary>
    /// <param name="target"></param>
    public void OnEnemyInRange(GameObject target)
    {
        if (enemyTargetsInRange.Find(x => x == target) == null)
        {
            enemyTargetsInRange.Add(target);
        }
    }

    #endregion

    #region On Enemy Escape Range

    /// <summary>
    /// When an enemy escapes my range of attack, take it of my list
    /// </summary>
    /// <param name="target"></param>
    public void OnEnemyEscapeRange(GameObject target)
    {
        if (enemyTargetsInRange.Find(x => x == target) != null)
        {
            enemyTargetsInRange.Remove(target);
        }
    }

    #endregion

    #region Current Target In Range

    /// <summary>
    ///  Check if the current Target is in range of an attack
    /// </summary>
    /// <returns></returns>
    private bool CurrentTargetInRange()
    {
        foreach (GameObject enemyTarget in enemyTargetsInRange)
        {
            if (enemyTarget == currentEnemyTarget)
            {
                return true;
            }
        }

        return false;
    }

    #endregion

    #region Move Skeleton

    /// <summary>
    /// Move Skeleton to a position on the map
    /// </summary>
    /// <param name="pointDestination"></param>
    public void MoveSkeleton(Vector3 pointDestination)
    {
        navMeshAgentComponent.SetDestination(pointDestination);
    }

    #endregion

    #region Complex Move Skeleton

    /// <summary>
    /// Move Skeleton to a specific target location
    /// </summary>
    /// <param name="currentTarget"></param>
    public void ComplexMoveSkeleton(GameObject currentTarget)
    {
        Collider targetCollider = currentTarget.GetComponent<Collider>();                 // Get Collider
        Vector3 closestPoint = targetCollider.ClosestPoint(this.transform.position);      // Get Closest Point
        MoveSkeleton(closestPoint);                                                         // Move Agent
    }

    #endregion

    private void AdjustAngle()
    {
        if(currentEnemyTarget != null)
        {
            Quaternion lookRotation;
            Vector3 direction;

            direction = (currentEnemyTarget.transform.position - transform.position).normalized;
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
    void OnDrawGizmos()
    {
        // Draw a red sphere at the transform's position
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 12.0f);
    }

    private void ClearNulls()
    {
        if(enemyDetector.Count != 0)
        {
            for (int i = 0; i < enemyDetector.Count; i++)
            {
                if (enemyDetector[i] == null) enemyDetector.RemoveAt(i);
            }
        }
        
    }

    #endregion

}
