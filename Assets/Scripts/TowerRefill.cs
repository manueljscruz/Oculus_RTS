using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerRefill : MonoBehaviour
{
    private int maxFuel;
    public int currentFuel;
    private int resoucesTaken;
    private int typesparse;
    private int numberOfResources;
    private GameObject playerResources;

    // Start is called before the first frame update
    void Start()
    {
        maxFuel = 50;
        currentFuel = 20;
        playerResources = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnMouseDown()
    {
        numberOfResources = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerResources>().playerResources[(int)ResourceType.Wood].Quantity;

        if (currentFuel < maxFuel && numberOfResources > 0)
        {
            Timer();
        }
    }

    void Timer()
    {
        Invoke("Refill", 0.1f/*(maxFuel - currentFuel) / 10*/);
    }

    void Refill()
    {
        int difer = Mathf.Abs(currentFuel - numberOfResources);
        currentFuel += difer;
        numberOfResources -= difer;
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerResources>().playerResources[(int)ResourceType.Wood].Quantity -= difer;
        // GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerResources>().debugTownCenterUI.UpdateText();
    }
}