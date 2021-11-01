using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceHoverUIHandler : MonoBehaviour
{
    // Parameters
    public Sprite[] resourceSprites;
    public GameObject resourceIconImage;
    public GameObject resourceTypeObject;
    public GameObject resourceQntyObject;


    // Cached
    private Camera cameraToLookAt;
    private ResourceHandler resourceHandler;
    private Image resourceIcon;
    private TMP_Text resourceTypeText;
    private TMP_Text resourceQuantityText;

    // State

    // Start is called before the first frame update
    void Start()
    {
        resourceHandler = this.GetComponentInParent<ResourceHandler>();   // Get Resource Type
        cameraToLookAt = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        // If object is enabled
        if (this.enabled)
        {
            RotateUI();
        }
        // Check if is facing our camera
    }

    /// <summary>
    /// When this componet is enabled
    /// </summary>
    private void OnEnable()
    {
        
    }

    #region Methods

    public void SetupResourceUI(string strResourceType, string strQuantity)
    {
        // Get Components
        resourceTypeText = resourceTypeObject.GetComponent<TMP_Text>();
        resourceQuantityText = resourceQntyObject.GetComponent<TMP_Text>();
        resourceIcon = resourceIconImage.GetComponent<Image>();

        // Set Texts
        resourceTypeText.text = strResourceType;
        resourceQuantityText.text = strQuantity;

        // Set Image
        foreach(Sprite resourceSprite in resourceSprites)
        {
            string strSpriteName = resourceSprite.name.ToLower();
            if (strSpriteName.Contains(strResourceType.ToLower())) {
                resourceIcon.sprite = resourceSprite;
                break;
            }
        }
    }

    public void UpdateQuantityUI(string strQuantity)
    {
        resourceQuantityText.text = strQuantity;
    }

    private void RotateUI()
    {
        Vector3 v = cameraToLookAt.transform.position - transform.position;
        v.x = v.z = 0.0f;
        transform.LookAt(cameraToLookAt.transform.position - v);
        transform.Rotate(0, 180, 0);
    }

    #endregion
}
