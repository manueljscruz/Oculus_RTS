using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn_Units : MonoBehaviour
{
    public GameObject unitToSpawn;
    private int resoucesTaken;
    private int typesparse;
    private GameObject playerResources;
    private int foodRequirement;

    // Start is called before the first frame update
    void Start()
    {
        foodRequirement = 20;
        playerResources = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseDown()
    {
        if (GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerResources>().playerResources[(int)ResourceType.Food].Quantity > foodRequirement)
        {
            Timer();
        }
    }

    void Timer()
    {
        playerResources.GetComponent<PlayerResources>().DrainResources((int)ResourceType.Food, foodRequirement);
        Invoke("Spawn", 2);
    }

    void Spawn()
    {
        Vector3 vec3 = transform.position;
        Vector3 rando = new Vector3(Random.Range(2, 5f), Random.Range(2, 5f), Random.Range(2, 5f));

        Instantiate(unitToSpawn, vec3 + rando, Quaternion.identity);
    }
}