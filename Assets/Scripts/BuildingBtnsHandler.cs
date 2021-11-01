using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingBtnsHandler : MonoBehaviour
{
    // Parameters
    [SerializeField] public PlayerBuildings buildingToSpawn;

    // Cached
    private PlayerResources playerResourcesComponent;
    private ResourceClass[] buildingCosts;

    // State
    public enum PlayerBuildings
    {
        TownCenter,
        Farm,
        Barracks,
        FireTower
    }

    // Start is called before the first frame update
    void Start()
    {
        playerResourcesComponent = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerResources>();
        buildingCosts = new ResourceClass[4]
        {
            new ResourceClass(ResourceType.Food, 0),
            new ResourceClass(ResourceType.Wood, 0),
            new ResourceClass(ResourceType.Gold, 0),
            new ResourceClass(ResourceType.Stone, 0)
        };
        SetBuildingCosts(buildingToSpawn);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DebugTests(string strButtonPressed)
    {
        Debug.Log("Pressed " + strButtonPressed + " button");
    }

    public void CreateBlueprint(GameObject prefab)
    {
        if (playerResourcesComponent.CheckForResourcesAvailabilty(buildingCosts)) Instantiate(prefab);
    }

    private void SetBuildingCosts(PlayerBuildings buildingToSpawn)
    {
        switch (buildingToSpawn)
        {
            case PlayerBuildings.TownCenter:
                buildingCosts[1].Quantity = 125;
                break;

            case PlayerBuildings.Farm:
                buildingCosts[1].Quantity = 25;
                break;

            case PlayerBuildings.Barracks:
                buildingCosts[1].Quantity = 80;
                buildingCosts[3].Quantity = 40;
                break;

            case PlayerBuildings.FireTower:
                buildingCosts[1].Quantity = 125;
                buildingCosts[3].Quantity = 80;
                break;
        }
    }
}
