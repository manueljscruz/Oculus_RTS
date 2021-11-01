using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingUnitBtnHandlers : MonoBehaviour
{
    // Parameters
    [SerializeField] public PlayerUnits unitToBeSpawn;

    // Cached
    private BuildingSpawnUnit buildSpawnUnitComponent;
    private PlayerResources playerResourcesComponent;
    private WarningUIHandler warningUIHandlerComponent;
    private ResourceClass[] unitCosts;

    // State
    public enum PlayerUnits
    {
        Peon,
        Soldier
    }

    // Start is called before the first frame update
    void Start()
    {
        buildSpawnUnitComponent = GetComponentInParent<BuildingSpawnUnit>();
        playerResourcesComponent = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerResources>();
        unitCosts = new ResourceClass[4]
        {
            new ResourceClass(ResourceType.Food, 0),
            new ResourceClass(ResourceType.Wood, 0),
            new ResourceClass(ResourceType.Gold, 0),
            new ResourceClass(ResourceType.Stone, 0)
        };
        SetUnitCosts(unitToBeSpawn);
        warningUIHandlerComponent = GameObject.FindGameObjectWithTag("WarningUI").GetComponent<WarningUIHandler>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Spawn()
    {
        if (playerResourcesComponent.CheckForResourcesAvailabilty(unitCosts))
        {
            buildSpawnUnitComponent.AddToQueue(unitCosts);
        }
        else
        {
            warningUIHandlerComponent.SetupWarningUI("Error", "Not enough resources.", true);
        }
    }

    private void SetUnitCosts(PlayerUnits unitToBeSpawn)
    {
        switch (unitToBeSpawn)
        {
            case PlayerUnits.Peon:
                unitCosts[0].Quantity = 50;
                break;

            case PlayerUnits.Soldier:
                unitCosts[0].Quantity = 50;
                unitCosts[2].Quantity = 30;
                break;
        }
    }


}
