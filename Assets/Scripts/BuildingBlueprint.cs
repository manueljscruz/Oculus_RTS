using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class BuildingBlueprint : MonoBehaviour
{
    // Parameters
    public GameObject prefab;
    public Building building;
    public GameObject MainUI;

    // Cached
    private XRRayInteractor RRayInteractor;
    private HandAnimationController controller;
    private InputDevice thisController;
    private GameObject playerBuildingsObject;
    private ResourceClass[] buildingCosts;
    private PlayerResources playerResourcesComponent;

    private float time;
    private float btnInterval = 1f;
    private float nextBtnTime = 0f;


    // States
    public enum Building
    {
        TownCenter,
        Farm,
        Barracks,
        FireTower
    }

    // Start is called before the first frame update
    void Start()
    {
        controller = GameObject.Find("RightHand Controller").GetComponent<HandAnimationController>();
        RRayInteractor = controller.ReturnCurrentRRay();
        thisController = controller.ReturnInputDevice();
        playerBuildingsObject = GameObject.FindGameObjectWithTag("PlayerBuildingsHolder");
        playerResourcesComponent = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerResources>();
        // MainUI.SetActive(false);
        buildingCosts = new ResourceClass[4]
        {
            new ResourceClass(ResourceType.Food, 0),
            new ResourceClass(ResourceType.Wood, 0),
            new ResourceClass(ResourceType.Gold, 0),
            new ResourceClass(ResourceType.Stone, 0)
        };
        SetBuildingCosts(building);
    }

    // Update is called once per frame
    void Update()
    {
        time = Time.time;
        HandleRaycast();

        HandleInput();
    }

    void HandleInput()
    {
        // Trigger 
        if (thisController.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue) && triggerValue > 0.1f && time > nextBtnTime)
        {
            nextBtnTime = time + btnInterval;
            GameObject buildingGameObject = Instantiate(prefab, transform.position, transform.rotation) as GameObject;
            buildingGameObject.transform.parent = playerBuildingsObject.transform;
            playerResourcesComponent.DrainResources(buildingCosts);
            Destroy(gameObject);
        }

        // B Button Press
        if (thisController.TryGetFeatureValue(CommonUsages.secondaryButton, out bool pressed) && pressed && time > nextBtnTime)
        {
            nextBtnTime = time + btnInterval;
            Destroy(gameObject);
            MainUI.SetActive(true);
        }
    }

    void HandleRaycast()
    {
        RRayInteractor.GetCurrentRaycastHit(out RaycastHit raycastHit);
        if(raycastHit.collider.gameObject.layer == 8)
        {
            transform.position = raycastHit.point;
        }
    }

    private void SetBuildingCosts(Building buildingToSpawn)
    {
        switch (buildingToSpawn)
        {
            case Building.TownCenter:
                buildingCosts[1].Quantity = 125;
                break;

            case Building.Farm:
                buildingCosts[1].Quantity = 25;
                break;

            case Building.Barracks:
                buildingCosts[1].Quantity = 80;
                buildingCosts[3].Quantity = 40;
                break;

            case Building.FireTower:
                buildingCosts[1].Quantity = 125;
                buildingCosts[3].Quantity = 80;
                break;
        }
    }
}
