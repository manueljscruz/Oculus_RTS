using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceHandler : MonoBehaviour
{
    // Parameters
    [SerializeField] public ResourceType resourceType;
    [SerializeField] public int quantity;
    [SerializeField] GameObject resourceHoverUIHandler;
    [SerializeField] float timeForDisableUI = 3f;

    // Cached
    private ResourceClass resource;
    private ResourceHoverUIHandler ResourceHoverUI;
    private float time;                                         
    private float nextDisableTimer = 0f;
    private UIState currentUIState = UIState.Disabled;

    // private DebugResourceUI UI;
    // States
    private enum UIState
    {
        Enabled,
        Disabled
    }

    // Start is called before the first frame update
    void Start()
    {
        resource = new ResourceClass(resourceType, quantity);

        ResourceHoverUI = resourceHoverUIHandler.GetComponent<ResourceHoverUIHandler>();
        ResourceHoverUI.SetupResourceUI(resourceType.ToString(), quantity.ToString());
        
    }

    // Update is called once per frame
    void Update()
    {
        time = Time.time;

        if(currentUIState == UIState.Enabled)
        {
            CheckUITimer();
        }
    }

    public void Gather(int quantity)
    {
        if(resourceType == ResourceType.Food)
        {
            FindObjectOfType<SoundManager>().ChangePitch("Farming");
            FindObjectOfType<SoundManager>().PlaySound("Farming", transform.position);
        }

        if (resourceType == ResourceType.Stone || resourceType == ResourceType.Gold)
        {
            FindObjectOfType<SoundManager>().ChangePitch("Mining");
            FindObjectOfType<SoundManager>().PlaySound("Mining", transform.position);
        }

        if (resourceType == ResourceType.Wood)
        {
            FindObjectOfType<SoundManager>().ChangePitch("TreeCutting");
            FindObjectOfType<SoundManager>().PlaySound("TreeCutting", transform.position);
        }

        if (currentUIState == UIState.Disabled)
        {
            resourceHoverUIHandler.SetActive(true);
            currentUIState = UIState.Enabled;
        }
        nextDisableTimer = time + timeForDisableUI;
        resource.Quantity = resource.Quantity - quantity;
        ResourceHoverUI.UpdateQuantityUI(resource.Quantity.ToString());

        if (resource.Quantity == 0 || resource.Quantity < 0)
        {
            Destroy(this.gameObject);
        }
    }

    public int ReturnCurrentQuantity()
    {
        return resource.Quantity;
    }

    public ResourceType ReturnTypeResource()
    {
        return resource.Type;
    }


    private void CheckUITimer()
    {
        if(time >= nextDisableTimer)
        {
            resourceHoverUIHandler.SetActive(false);
            currentUIState = UIState.Disabled;
        }
    }
    
}