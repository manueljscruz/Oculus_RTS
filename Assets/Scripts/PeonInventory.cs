using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeonInventory : MonoBehaviour
{
    // Parameters
    [SerializeField] int inventorySize = 5;

    // Cached
    private PlayerResources playerResources;            // Player Resources pointer
    private ResourceClass[] resourceCarry;              // Resource Inventory

    // States

    // Start is called before the first frame update
    void Start()
    {
        // Instantiante Inventory
        resourceCarry = new ResourceClass[4]
        {
            new ResourceClass(ResourceType.Food, 0),
            new ResourceClass(ResourceType.Wood, 0),
            new ResourceClass(ResourceType.Gold, 0),
            new ResourceClass(ResourceType.Stone, 0)
        };

        //  new ResourceClass(ResourceType.Metal, 0),
        // Get Player Resources component
        playerResources = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerResources>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Gets current inventory status
    /// </summary>
    /// <returns></returns>
    public bool IsInventoryFull()
    {
        return CheckResourceInventory();
    }

    /// <summary>
    /// Checks if inventory is full
    /// </summary>
    private bool CheckResourceInventory()
    {
        int iCount = 0;
        // Check all Resource objects for quantities
        foreach (ResourceClass resource in resourceCarry)
        {
            iCount += resource.Quantity;
        }
        // if count matches the inventorySize, means its full
        return iCount == inventorySize;
    }

    /// <summary>
    /// Adds resource to inventory
    /// </summary>
    /// <param name="typeToAdd"></param>
    /// <param name="iQuantityToAdd"></param>
    public void AddResourceToInv(ResourceType typeToAdd, int iQuantityToAdd)
    {
        switch (typeToAdd)
        {
            case ResourceType.Food:
                resourceCarry[0].Quantity += iQuantityToAdd;
                break;

            case ResourceType.Wood:
                resourceCarry[1].Quantity += iQuantityToAdd;
                break;

            case ResourceType.Gold:
                resourceCarry[2].Quantity += iQuantityToAdd;
                break;

            case ResourceType.Stone:
                resourceCarry[3].Quantity += iQuantityToAdd;
                break;

            //case ResourceType.Metal:
            //    resourceCarry[4].Quantity += iQuantityToAdd;
            //    break;
        }

        //Debug.Log(IsInventoryFull());
    }

    public void DeliverResources()
    {
        foreach (ResourceClass resource in resourceCarry)
        {
            playerResources.AddResourcesToPlayer(resource.Type, resource.Quantity);
            resource.Quantity = 0;
        }
    }
}