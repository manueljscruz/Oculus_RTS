using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SoldierCombat : MonoBehaviour
{
    // Parameters
    [SerializeField] public int attackDamage = 10;                      // Amount of damage to be inflicted in enemy
    [SerializeField] private float timeBetweenAttacks = 1.5f;           // Time Interval between attacks  
    [SerializeField] float rotationSpeed = 1f;
    public LayerMask layers;                                            // Search for enemy units in a specific Unity Layer

    // Cached
    private List<GameObject> enemyTargetsInRange;                       // Enemy Targets in Range

    public GameObject currentEnemyTarget;                              // Current Enemy of interest
    private NavMeshAgent navMeshAgentComponent;                         // Nav Mesh Component of the Soldier
    private SoldierControlAgent soldierControlAgentComponent;           // Soldier Control Agent Component
    public SoldierCombatState currentCombatState;                       // Current Combat State of the Soldier
    private SoldierAnimationStateController soldierAnimationStateController;
    private Collider[] enemyDetector;                                   // List of colliders found in the range field
    private Vector3 previousAdjustment;

    private float time;                                                 // Time elapsed since scene started
    private float nextAttackTime = 0f;                                  // Next time the soldier can attack the enemy

    // States
    public enum SoldierCombatState
    {
        Idle,
        MovingToTarget,
        Attacking
    }

    // Start is called before the first frame update
    void Start()
    {
        currentCombatState = SoldierCombatState.Idle;
        enemyTargetsInRange = new List<GameObject>();
        navMeshAgentComponent = GetComponent<NavMeshAgent>();
        soldierControlAgentComponent = GetComponent<SoldierControlAgent>();
        soldierAnimationStateController = GetComponent<SoldierAnimationStateController>();
    }

    // Update is called once per frame
    void Update()
    {
        time = Time.time;
        // if (currentEnemyTarget != null) RotateToFaceTarget();
        StateControl();
        
    }

    #region Methods

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

    #region Check Closest Enemy

    /// <summary>
    /// Checks in the range field for closest enemy target
    /// </summary>
    private void CheckClosestEnemy()
    {
        // Move to it
        // Change state to moving to target
        enemyDetector = Physics.OverlapSphere(transform.position, 10.0f, layers);

        // If there is an enemy or more nearby
        if(enemyDetector.Length > 0)
        {
            // Assign minimum distance and current enemy target to the first instance
            float minDistance = (transform.position - enemyDetector[0].transform.position).magnitude;
            currentEnemyTarget = enemyDetector[0].gameObject;

            // Check 
            int index = 0;
            foreach(Collider enemyCollider in enemyDetector)
            {
                if(index != 0)
                {
                    float currentDistance = (transform.position - enemyCollider.transform.position).magnitude;

                    if (currentDistance < minDistance)
                    {
                        minDistance = currentDistance;
                        currentEnemyTarget = enemyCollider.gameObject;
                    }
                }
                index++;
            }

            Debug.Log(string.Format("{0} is targeting {1}", this.name, currentEnemyTarget.name));
            soldierControlAgentComponent.ComplexMoveAgent(currentEnemyTarget);
            currentCombatState = SoldierCombatState.MovingToTarget;
        }
    }

    #endregion

    #region State Control

    /// <summary>
    /// Do stuff based on current combat state
    /// </summary>
    private void StateControl()
    {
        switch (currentCombatState)
        {
            case SoldierCombatState.Idle:
                CheckClosestEnemy();
                break;

            case SoldierCombatState.MovingToTarget:
                if(currentEnemyTarget != null)
                {
                    // CheckClosestEnemy();

                    if (CurrentTargetInRange())
                    {
                        if (CheckIfMoving()) navMeshAgentComponent.velocity = Vector3.zero;
                        currentCombatState = SoldierCombatState.Attacking;
                    }

                    else
                    {
                        soldierControlAgentComponent.ComplexMoveAgent(currentEnemyTarget);
                        AdjustAngle();
                    }
                }
                //else
                //{
                //    currentCombatState = SoldierCombatState.Idle;
                //}
                break;

            case SoldierCombatState.Attacking:
                AdjustAngle();
                HandleCombat();
                break;
        }
    }

    #endregion

    private void HandleCombat()
    {
        if(currentEnemyTarget != null)
        {
            // If Current Target is still in range
            if (CurrentTargetInRange())
            {
                if (nextAttackTime == 0 || time >= nextAttackTime)
                {
                    // Get health component from the enemy
                    Health targetHealthComponent = currentEnemyTarget.GetComponent<Health>();

                    // send a message to do attack animation
                    soldierAnimationStateController.PerformAttack();

                    //Sound effect for attack
                    FindObjectOfType<SoundManager>().ChangeClip("UnitsFight");
                    FindObjectOfType<SoundManager>().PlaySound("UnitsFight", transform.position);

                    // deliver damage
                    targetHealthComponent.LoseHealth(attackDamage);

                    // Debug.Log(string.Format("{0} delivered 10 damage to {1}", this.name, currentEnemyTarget.name));

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
                    soldierControlAgentComponent.ComplexMoveAgent(currentEnemyTarget);
                    currentCombatState = SoldierCombatState.MovingToTarget;
                }
                // Target Destroyed, go back to Idle
                // TO DO: Consider Switching Targets if there are anymore in range
                else
                {
                    currentCombatState = SoldierCombatState.Idle;
                }
            }
        }
        else
        {
            soldierAnimationStateController.StopAttack();
            currentCombatState = SoldierCombatState.Idle;
        }
        
    }

    #region Return Combat State

    /// <summary>
    /// To be Used for the animation Controls???
    /// </summary>
    /// <returns></returns>
    /// 
    /*
    public static SoldierCombatState ReturnCombatState()
    {
        return currentCombatState;
    }
    */
    #endregion

    #region Current Target In Range

    /// <summary>
    ///  Check if the current Target is in range of an attack
    /// </summary>
    /// <returns></returns>
    private bool CurrentTargetInRange()
    {
        foreach(GameObject enemyTarget in enemyTargetsInRange)
        {
            if(enemyTarget == currentEnemyTarget)
            {
                return true;
            }
        }

        return false;
    }

    #endregion

    #region Check if Moving

    /// <summary>
    /// Check if Soldier is Moving
    /// </summary>
    /// <returns></returns>
    bool CheckIfMoving()
    {
        if (navMeshAgentComponent.velocity != Vector3.zero) return true;
        else return false;
    }

    #endregion

    void OnDrawGizmos()
    {
        // Draw a red sphere at the transform's position
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 10.0f);
    }

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

    #endregion
}
