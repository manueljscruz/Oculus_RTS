using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower_Script : MonoBehaviour
{
    public LayerMask layers;

    private Collider[] enemyDetector;
    private Collider[] enemyKiller;
    private GameObject closestEnemy;
    private GameObject rotator;
    private ParticleSystem flames;
    //private int currentFuel;

    public bool fireSoundActive;

    // Start is called before the first frame update
    void Start()
    {
        fireSoundActive = false;
        rotator = this.transform.GetChild(7).gameObject;
        flames = this.transform.GetChild(7).GetChild(0).gameObject.GetComponent<ParticleSystem>();

        InvokeRepeating("FireTower", 1, 0.1f);
    }

    // Update is called once per frame
    void Update()
    {
        enemyDetector = Physics.OverlapSphere(transform.position, 15.0f, layers);
    }

    public void FireTower()
    {
        if (enemyDetector.Length > 0 /*&& gameObject.GetComponent<TowerRefill>().currentFuel > 0*/)
        {
            float minDistance = (transform.position - enemyDetector[0].transform.position).magnitude;

            foreach (Collider hitCollider in enemyDetector)
            {
                float currentDistance = (transform.position - hitCollider.transform.position).magnitude;

                if (currentDistance <= minDistance)
                {
                    minDistance = currentDistance;
                    closestEnemy = hitCollider.gameObject;
                }
            }

            if (fireSoundActive == false)
            {
                FindObjectOfType<SoundManager>().PlaySound("Flamethrower");
                fireSoundActive = true;
            }

            rotator.transform.LookAt(closestEnemy.transform.position, Vector3.up);
            flames.Play();
            //gameObject.GetComponent<TowerRefill>().currentFuel -= 1;
            closestEnemy.gameObject.GetComponent<Health>().LoseHealth(4);
        }

        else
        {
            FindObjectOfType<SoundManager>().StopSound("Flamethrower");
            fireSoundActive = false;
            flames.Stop();
        }
    }

    void OnDrawGizmos()
    {
        // Draw a green sphere at the transform's position
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 15.0f);
    }
}