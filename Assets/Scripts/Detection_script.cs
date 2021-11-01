using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Detection_script : MonoBehaviour
{
    public LayerMask layers;
    private NavMeshAgent navMesh;
    private Collider[] peonDetector;
    private GameObject[] townCenter;
    private GameObject closestNPC;

    private void Start()
    {
        InvokeRepeating("CheckClosest", 2f, 0.5f);
        navMesh = this.gameObject.GetComponent<NavMeshAgent>();
        townCenter = GameObject.FindGameObjectsWithTag("PlayerTC");
    }

    private void Update()
    {
        peonDetector = Physics.OverlapSphere(transform.position, 12.0f, layers);
    }

    public void CheckClosest()
    {
        if (peonDetector.Length > 0)
        {
            float minDistance = (transform.position - peonDetector[0].transform.position).magnitude;

            foreach (Collider hitcollider in peonDetector)
            {
                float currentDistance = (transform.position - hitcollider.transform.position).magnitude;

                if (currentDistance <= minDistance)
                {
                    minDistance = currentDistance;
                    closestNPC = hitcollider.gameObject;
                }
            }

            navMesh.SetDestination(closestNPC.transform.position);
            CheckKill(closestNPC);
        }

        else
        {
            GoToTownCenter();
        }

    }
    public void CheckKill(GameObject tagName)
    {
        if((transform.position - closestNPC.transform.position).magnitude <= navMesh.stoppingDistance && tagName.tag == "PlayerPeon")
        {
            Object.Destroy(closestNPC);
            FindObjectOfType<SoundManager>().PlaySound("UnitsFight");
        }

        else if ((transform.position - closestNPC.transform.position).magnitude <= navMesh.stoppingDistance && tagName.tag == "SoldierUnit")
        {
            tagName.GetComponent<Soldier_Script>().SoldierLoseHealth(4);
            FindObjectOfType<SoundManager>().PlaySound("UnitsFight");
        }
    }

    public void GoToTownCenter()
    {
        navMesh.SetDestination(townCenter[Random.Range(0, townCenter.Length)].transform.position);
    }

    void OnDrawGizmos()
    {
        // Draw a red sphere at the transform's position
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 12.0f);
    }
}