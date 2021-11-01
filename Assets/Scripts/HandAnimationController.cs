using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class HandAnimationController : MonoBehaviour
{
    // Parameters
    public InputDeviceCharacteristics controllerType;
    public InputDevice thisController;
    public GameObject MainUI;

    // Cached
    private XRRayInteractor RRayInteractor;
    private GameObject selectedObject;
    private List<GameObject> listSelectedUnitObjects;
    private GameObject selectedBuildingObject;
    private float time;
    private float btnInterval = 1f;
    private float nextBtnTime = 0f;

    // State
    private enum BuildState
    {
        Enabled,
        Disabled
    }

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
        RRayInteractor = GetComponent<XRRayInteractor>();
        listSelectedUnitObjects = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        time = Time.time;
        if (thisController == null) Initialize();
        else
        {
            ClearAllNulls();
            HandleInput();
        }

    }

    void Initialize()
    {
        List<InputDevice> controllerDevices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(controllerType, controllerDevices);

        // If there are no devices activated
        if (controllerDevices.Count.Equals(0))
        {
            Debug.Log("List is Empty");
        }
        else
            thisController = controllerDevices[0];
    }

    void HandleInput()
    {
        //if(currentBuildState == BuildState.Disabled)
        //{
        // Primary Button (A) - Select
        if (thisController.TryGetFeatureValue(CommonUsages.primaryButton, out bool btn_A_Pressed) && btn_A_Pressed)
        {
            if (RRayInteractor.GetCurrentRaycastHit(out RaycastHit raycastHit))
            {
                selectedObject = raycastHit.collider.gameObject;
                Select(selectedObject);
            }
        }

        // Trigger - Command
        else if (thisController.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue) && triggerValue > 0.1f)
        {
            RRayInteractor.GetCurrentRaycastHit(out RaycastHit raycastHit);
            Command(raycastHit);
        }
        //}

        // Secondary Button (B) - Trigger UI
        else if(thisController.TryGetFeatureValue(CommonUsages.secondaryButton, out bool btn_B_Pressed) && btn_B_Pressed && time > nextBtnTime)
        {
            nextBtnTime = time + btnInterval;
            if (MainUI.activeSelf) MainUI.SetActive(false);
            else MainUI.SetActive(true);
        }

        


        //else
        //{

        //}


    }

    #region Select

    /// <summary>
    /// Select objects - VR version
    /// </summary>
    /// <param name="selected"></param>
    private void Select(GameObject selected)
    {
        // Check if selection is a unit
        if (selected.tag == "PlayerPeon" || selected.tag == "SoldierUnit")
        {
            if (!AlreadySelected(selected))
            {
                listSelectedUnitObjects.Add(selected);
                selectedObject.GetComponent<Outline>().enabled = true;
                FindObjectOfType<SoundManager>().PlaySound("ButtonPress");
            }
        }

        else if (selected.tag == "PlayerBarracks" || selected.tag == "PlayerTC")
        {
            if (selectedBuildingObject == null)
            {
                selectedBuildingObject = selected;
                selectedBuildingObject.transform.Find("BuildingUI").gameObject.SetActive(true);
            }
            else if (selected != selectedBuildingObject)
            {
                selectedBuildingObject.transform.Find("BuildingUI").gameObject.SetActive(false);
                selectedBuildingObject = selected;
                selectedBuildingObject.transform.Find("BuildingUI").gameObject.SetActive(true);
            }
        }

        else if (selectedObject.layer == 8)
        {
            ClearSelection();
        }
    }

    #endregion

    #region Command

    /// <summary>
    /// Sends commands to units based on raycast hit
    /// </summary>
    /// <param name="hitInfo"></param>
    private void Command(RaycastHit hitInfo)
    {
        if (listSelectedUnitObjects.Count != 0)
        {
            foreach (GameObject unitSelected in listSelectedUnitObjects)
            {
                if (unitSelected.tag == "PlayerPeon") unitSelected.GetComponent<PeonControlAgent>().HandleCommand(hitInfo);
                else if (unitSelected.tag == "SoldierUnit") unitSelected.GetComponent<SoldierControlAgent>().HandleCommand(hitInfo);
            }
        }
    }

    #endregion

    //private void Move(Vector2 values)
    //{
    //    RigObject.transform.position += transform.forward * (values.y * speedMove) * Time.deltaTime;
    //    RigObject.transform.position += transform.right * (values.x * speedMove) * Time.deltaTime;
    //}

    #region Clear Selection

    /// <summary>
    /// Deselects all units
    /// </summary>
    private void ClearSelection()
    {
        // Check if there is anything selected
        if (selectedObject != null || listSelectedUnitObjects.Count != 0)
        {
            foreach (GameObject selectedUnit in listSelectedUnitObjects)
            {
                Outline outline = selectedUnit.GetComponent<Outline>();
                if (outline)
                {
                    outline.enabled = false;
                }
            }
            // Empty selections
            selectedObject = null;
            listSelectedUnitObjects.Clear();
        }

        if (selectedBuildingObject != null)
        {
            selectedBuildingObject.transform.Find("BuildingUI").gameObject.SetActive(false);
            selectedBuildingObject = null;
        }
    }

    #endregion

    #region Check If Unit Already Selected

    private bool AlreadySelected(GameObject newSelection)
    {
        foreach (GameObject selected in listSelectedUnitObjects)
        {
            if (newSelection == selected) return true;
        }
        return false;
    }

    #endregion

    public XRRayInteractor ReturnCurrentRRay()
    {
        return RRayInteractor;
    }

    public InputDevice ReturnInputDevice()
    {
        return thisController;
    }

    private void ClearAllNulls()
    {
        listSelectedUnitObjects.RemoveAll(item => item == null);
    }


    #region Previous Code
    //    if(thisController.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue) && triggerValue > 0.1f)
    //        {
    //            Debug.Log("Trigger Press");
    //        }

    //if (thisController.TryGetFeatureValue(CommonUsages.grip, out float gripValue) && gripValue > 0.1f)
    //{
    //    Debug.Log("Grip Press " + gripValue);
    //}
    #endregion

    #region Not Used

    /// <summary>
    /// Handle Left Mouse Clicks
    /// </summary>
    /// <param name="raycast"></param>
    //private void Select(Ray raycast)
    //{
    //    // If raycast collides with something
    //    if (Physics.Raycast(raycast, out RaycastHit hitInfo))
    //    {
    //        selectedObject = hitInfo.collider.gameObject;

    //        // Check if selection is a unit
    //        if (selectedObject.tag == "PlayerPeon" || selectedObject.tag == "SoldierUnit")
    //        {
    //            listSelectedObjects.Add(selectedObject);
    //            selectedObject.GetComponent<Outline>().enabled = true;
    //        }
    //        // 
    //        else if (selectedObject.layer == 8)
    //        {
    //            ClearSelection();
    //        }
    //    }
    //}

    /// <summary>
    /// Handle Right Mouse Clicks
    /// </summary>
    /// <param name="raycast"></param>
    //private void Command(Ray raycast)
    //{
    //    // If raycast collides with something
    //    if (Physics.Raycast(raycast, out RaycastHit hitInfo))
    //    {
    //        selectedObject = hitInfo.collider.gameObject;

    //        if (listSelectedObjects.Count != 0)
    //        {
    //            foreach (GameObject unitSelected in listSelectedObjects)
    //            {
    //                if (unitSelected.tag == "PlayerPeon") unitSelected.GetComponent<PeonControlAgent>().HandleCommand(hitInfo);
    //                else if (unitSelected.tag == "SoldierUnit") unitSelected.GetComponent<SoldierControlAgent>().HandleCommand(hitInfo);
    //            }
    //        }
    //    }
    //}

    #endregion
}
