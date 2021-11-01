using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WarningUIHandler : MonoBehaviour
{
    // Parameters
    [SerializeField] private float warningDuration = 3f;
    public GameObject warningHeaderObject;
    public GameObject warningMessageObject;
    public Color redBackgroundColor;
    public Color greenBackgroundColor;
    public Color redTextColor;
    public Color greenTextColor;

    // Cached
    private TMP_Text warningHeaderText;
    private TMP_Text warningMessageText;
    private Image imageComponent;

    private float time;
    private float timeToDisableUI = 0f;

    // States
    private bool isActive = false;

    private void Start()
    {
        imageComponent = this.GetComponent<Image>();
        warningHeaderText = warningHeaderObject.GetComponent<TMP_Text>();
        warningMessageText = warningMessageObject.GetComponent<TMP_Text>();
    }

    private void Update()
    {
        time = Time.time;
        if (isActive)
        {
            if(time > timeToDisableUI)
            {
                this.enabled = false;
                isActive = false;
            }
        }
    }

    public void SetupWarningUI(string strHeader, string strMessage, bool isWarning)
    {
        warningHeaderText.text = strHeader;
        warningMessageText.text = strMessage;

        // Color Scheme
        if (isWarning)
        {
            imageComponent.color = redBackgroundColor;
            warningHeaderText.color = redTextColor;
            warningMessageText.color = redTextColor;
        }
        else
        {
            imageComponent.color = greenBackgroundColor;
            warningHeaderText.color = greenTextColor;
            warningMessageText.color = greenTextColor;
        }
        this.enabled = true;
        isActive = true;
        timeToDisableUI = time + warningDuration;
    }
}
