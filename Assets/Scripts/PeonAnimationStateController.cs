using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PeonAnimationStateController : MonoBehaviour
{
    // Parameters

    // Cached 
    private Animator animatorController;
    private NavMeshAgent navMeshAgent;
    private Gatherer gatheringController;
    private PeonMotionState currentPeonMotionState;


    private Vector3 previousPosition;
    private float curSpeed = 0f;

    // States
    enum PeonMotionState
    {
        Idle,
        Moving,
        Gathering
    }


    // Start is called before the first frame update
    void Start()
    {
        previousPosition = transform.position;
        animatorController = GetComponentInChildren<Animator>();
        gatheringController = GetComponent<Gatherer>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        currentPeonMotionState = PeonMotionState.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        HandleMotion();
        HandleGathering();
    }

    void HandleMotion()
    {
        // If its moving and current status is idle, move
        if (CheckIfMoving() && currentPeonMotionState == PeonMotionState.Idle)
        {
            currentPeonMotionState = PeonMotionState.Moving;
            animatorController.SetBool("IsRunning", true);

        }

        // If no longer moving and current status is moving, go back to idle
        else if(!CheckIfMoving() && currentPeonMotionState == PeonMotionState.Moving)
        {
            currentPeonMotionState = PeonMotionState.Idle;
            animatorController.SetBool("IsRunning", false);
        }
    }

    void HandleGathering()
    {
        if(!CheckIfMoving() && gatheringController.IsPeonGathering() && currentPeonMotionState == PeonMotionState.Idle)
        {
            currentPeonMotionState = PeonMotionState.Gathering;
            animatorController.SetBool("IsGathering", true);
        }

        else if(!gatheringController.IsPeonGathering() && currentPeonMotionState == PeonMotionState.Gathering)
        {
            currentPeonMotionState = PeonMotionState.Idle;
            animatorController.SetBool("IsGathering", false);
        }
    }

    bool CheckIfMoving()
    {
        if (navMeshAgent.velocity != Vector3.zero) return true;
        else return false;
    }
}
