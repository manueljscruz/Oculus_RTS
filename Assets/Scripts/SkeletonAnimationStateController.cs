using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SkeletonAnimationStateController : MonoBehaviour
{
    // Parameters

    // Cached
    private Animator animatorController;
    private NavMeshAgent navMeshAgent;
    private SkeletonMotionState currentSkeletonMotionState;

    private Vector3 previousPosition;
    private float curSpeed = 0f;

    // States 
    enum SkeletonMotionState
    {
        Idle,
        Moving,
        Attacking
    }

    // Start is called before the first frame update
    void Start()
    {
        previousPosition = transform.position;
        animatorController = GetComponentInChildren<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        currentSkeletonMotionState = SkeletonMotionState.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        HandleMotion();
    }

    void HandleMotion()
    {
        // If its moving and current status is idle, move
        if (CheckIfMoving() && currentSkeletonMotionState == SkeletonMotionState.Idle)
        {
            currentSkeletonMotionState = SkeletonMotionState.Moving;
            animatorController.SetBool("IsWalking", true);

        }

        // If no longer moving and current status is moving, go back to idle
        else if (!CheckIfMoving() && currentSkeletonMotionState == SkeletonMotionState.Moving)
        {
            currentSkeletonMotionState = SkeletonMotionState.Idle;
            animatorController.SetBool("IsWalking", false);
        }
    }

    public bool CheckIfMoving()
    {
        if (navMeshAgent.velocity != Vector3.zero) return true;
        else return false;
    }

    public void PerformAttack()
    {
        currentSkeletonMotionState = SkeletonMotionState.Attacking;
        animatorController.SetBool("IsAttacking", true);
    }

    public void StopAttack()
    {
        currentSkeletonMotionState = SkeletonMotionState.Idle;
        animatorController.SetBool("IsAttacking", false);
        animatorController.SetBool("IsWalking", false);
    }
}