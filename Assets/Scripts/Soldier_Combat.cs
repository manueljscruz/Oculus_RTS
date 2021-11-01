using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class Soldier_Combat : MonoBehaviour
{
    public LayerMask layers;
    private NavMeshAgent navMesh;
    private Collider[] enemyDetector;
    private GameObject closestEnemy;
    private SoldierControlAgent soldierControl;

    private void Start()
    {
        InvokeRepeating("CheckClosestEnemy", 1.5f, 0.5f);
        navMesh = this.gameObject.GetComponent<NavMeshAgent>();
        soldierControl = this.GetComponent<SoldierControlAgent>();
    }

    private void Update()
    {
        enemyDetector = Physics.OverlapSphere(transform.position, 10.0f, layers);
    }

    public void CheckClosestEnemy()
    {
        if (enemyDetector.Length > 0)
        {
            soldierControl.enabled = false;

            float minDistance = (transform.position - enemyDetector[0].transform.position).magnitude;

            foreach (Collider hitcollider in enemyDetector)
            {
                float currentDistance = (transform.position - hitcollider.transform.position).magnitude;

                if (currentDistance <= minDistance)
                {
                    minDistance = currentDistance;
                    closestEnemy = hitcollider.gameObject;
                }
            }

            navMesh.SetDestination(closestEnemy.transform.position);
            CheckDamage();
        }

        else
        {
            soldierControl.enabled = true;
        }
    }
    public void CheckDamage()
    {
        if ((transform.position - closestEnemy.transform.position).magnitude <= navMesh.stoppingDistance)
        {
            closestEnemy.GetComponent<Enemy_Script>().EnemyLoseHealth(5);
        }
    }

    void OnDrawGizmos()
    {
        // Draw a red sphere at the transform's position
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 10.0f);
    }
}