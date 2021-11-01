using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerResources : MonoBehaviour
{
    // Parameters
    [SerializeField] int initialFood;
    [SerializeField] int initialWood;
    [SerializeField] int initialGold;
    [SerializeField] int initialStone;
    // [SerializeField] int initialMetal;

    public GameObject foodResourceTextObject;
    public GameObject woodResourceTextObject;
    public GameObject goldResourceTextObject;
    public GameObject stoneResourceTextObject;

    // Cached
    public ResourceClass[] playerResources;
    private TMP_Text foodResourceCountText;
    private TMP_Text woodResourceCountText;
    private TMP_Text goldResourceCountText;
    private TMP_Text stoneResourceCountText;
    // public DebugTownCenterUI debugTownCenterUI;

    // States 


    // Start is called before the first frame update
    void Start()
    {
        // Instantiate Initial Player Resources
        playerResources = new ResourceClass[4]
        {
            new ResourceClass(ResourceType.Food, initialFood),
            new ResourceClass(ResourceType.Wood, initialWood),
            new ResourceClass(ResourceType.Gold, initialGold),
            new ResourceClass(ResourceType.Stone, initialStone)
        }; // new ResourceClass(ResourceType.Metal, initialMetal),

        foodResourceCountText = foodResourceTextObject.GetComponent<TMP_Text>();
        woodResourceCountText = woodResourceTextObject.GetComponent<TMP_Text>();
        goldResourceCountText = goldResourceTextObject.GetComponent<TMP_Text>();
        stoneResourceCountText = stoneResourceTextObject.GetComponent<TMP_Text>();
        //debugTownCenterUI = GameObject.FindGameObjectWithTag("PlayerTC").GetComponent<DebugTownCenterUI>();
        //debugTownCenterUI.UpdateText();
        UpdateResourceCountsInUI();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Add Resources to Players Bank
    /// </summary>
    /// <param name="typeResource"></param>
    /// <param name="quantity"></param>
    public void AddResourcesToPlayer(ResourceType typeResource, int quantity)
    {
        switch (typeResource)
        {
            case ResourceType.Food:
                playerResources[0].Quantity += quantity;
                break;

            case ResourceType.Wood:
                playerResources[1].Quantity += quantity;
                break;

            case ResourceType.Gold:
                playerResources[2].Quantity += quantity;
                break;

            case ResourceType.Stone:
                playerResources[3].Quantity += quantity;
                break;

                //case ResourceType.Metal:
                //    playerResources[4].Quantity += quantity;
                //    break;
        }

        UpdateResourceCountsInUI();
        // Update Town Center UI
        // debugTownCenterUI.UpdateText();
    }

    /// <summary>
    /// Returns the current player resources counts
    /// </summary>
    /// <returns></returns>
    public ResourceClass[] GetCurrentValues()
    {
        return playerResources;
    }

    public void DrainResources(int resourceType, int resourcesToTake)
    {
        playerResources[resourceType].Quantity -= resourcesToTake;
        // debugTownCenterUI.UpdateText();
    }

    private void UpdateResourceCountsInUI()
    {
        foodResourceCountText.SetText(playerResources[0].Quantity.ToString());   // Food
        woodResourceCountText.SetText(playerResources[1].Quantity.ToString());   // Wood
        goldResourceCountText.SetText(playerResources[2].Quantity.ToString());   // Gold
        stoneResourceCountText.SetText(playerResources[3].Quantity.ToString());   // Stone
    }

    public bool CheckForResourcesAvailabilty(ResourceClass[] resourcesCost)
    {
        bool bResourceAvailable = true;

        foreach (ResourceClass resourceCost in resourcesCost)
        {
            switch (resourceCost.Type)
            {
                case ResourceType.Food:
                    if (playerResources[0].Quantity - resourceCost.Quantity < 0) return false;
                    break;

                case ResourceType.Wood:
                    if (playerResources[1].Quantity - resourceCost.Quantity < 0) return false;
                    break;

                case ResourceType.Gold:
                    if (playerResources[2].Quantity - resourceCost.Quantity < 0) return false;
                    break;

                case ResourceType.Stone:
                    if (playerResources[3].Quantity - resourceCost.Quantity < 0) return false;
                    break;
            }

        }

        return bResourceAvailable;
    }

    public void DrainResources(ResourceClass[] resourcesCost)
    {
        foreach (ResourceClass resourceCost in resourcesCost)
        {
            switch (resourceCost.Type)
            {
                case ResourceType.Food:
                    playerResources[0].Quantity -= resourceCost.Quantity;
                    break;

                case ResourceType.Wood:
                    playerResources[1].Quantity -= resourceCost.Quantity;
                    break;

                case ResourceType.Gold:
                    playerResources[2].Quantity -= resourceCost.Quantity;
                    break;

                case ResourceType.Stone:
                    playerResources[3].Quantity -= resourceCost.Quantity;
                    break;
            }

        }
        UpdateResourceCountsInUI();
    }
}