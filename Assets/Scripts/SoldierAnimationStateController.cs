using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SoldierAnimationStateController : MonoBehaviour
{
    // Parameters

    // Cached 
    private Animator animatorController;
    private NavMeshAgent navMeshAgent;
    private SoldierMotionState currentSoldierMotionState;


    private Vector3 previousPosition;
    private float curSpeed = 0f;

    // States
    enum SoldierMotionState
    {
        Idle,
        Moving,
        Attacking
    }


    // Start is called before the first frame update
    void Start()
    {
        previousPosition = transform.position;
        animatorController = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        currentSoldierMotionState = SoldierMotionState.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        HandleMotion();
    }

    void HandleMotion()
    {
        // If its moving and current status is idle, move
        if (CheckIfMoving() && currentSoldierMotionState == SoldierMotionState.Idle)
        {
            currentSoldierMotionState = SoldierMotionState.Moving;
            animatorController.SetBool("IsWalking", true);

        }

        // If no longer moving and current status is moving, go back to idle
        else if(!CheckIfMoving() && currentSoldierMotionState == SoldierMotionState.Moving)
        {
            currentSoldierMotionState = SoldierMotionState.Idle;
            animatorController.SetBool("IsWalking", false);
        }
    }

    bool CheckIfMoving()
    {
        if (navMeshAgent.velocity != Vector3.zero) return true;
        else return false;
    }

    public void PerformAttack()
    {
        currentSoldierMotionState = SoldierMotionState.Attacking;
        animatorController.SetBool("IsAttacking", true);
    }

    public void StopAttack()
    {
        currentSoldierMotionState = SoldierMotionState.Idle;
        animatorController.SetBool("IsAttacking", false);
        animatorController.SetBool("IsWalking", false);
    }
}