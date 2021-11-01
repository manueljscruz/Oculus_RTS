using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class RigMoveScript : MonoBehaviour
{
    // Parameters
    public InputDeviceCharacteristics controllerType;
    public InputDevice thisController;
    public GameObject rigObject;
    public GameObject teleportPrefab;

    [SerializeField] public float speedMove = 5f;       // Movement
    [SerializeField] public float sensitivity = 2f;     // Rotation

    // Cached
    private float rotationY = 0f;
    private float rotationX = 0f;
    private XRRayInteractor LRayInteractor;

    // State
    public enum RigMovementType
    {
        Static,
        Freestyle
    }
    public RigMovementType rigMovementType;

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
        LRayInteractor = GetComponent<XRRayInteractor>();
    }

    // Update is called once per frame
    void Update()
    {
        // Check if there is a device. If not go get it
        if (thisController == null) Initialize();
        else
        {
            HandleMovementInput();
            HandleTeleport();
        }
    }

    #region Methods

    /// <summary>
    /// Initialize Device Controllers
    /// This is called in update if no device is found
    /// </summary>
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


    /// <summary>
    /// Handles the input of the controllers for the movement of the rig
    /// </summary>
    void HandleMovementInput()
    {
        switch (rigMovementType)
        {
            case RigMovementType.Static:

                break;

            case RigMovementType.Freestyle:
                // Left Hand -> Movement
                if (thisController.role == InputDeviceRole.LeftHanded)
                {
                    if (thisController.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 values) && ((values.x != 0) || (values.y != 0)))
                    {
                        // Debug.Log(string.Format("X: {0}  Y: {1}", values.x, values.y));
                        MoveRig(values);
                    }
                }
                // Right Hand -> Rotation
                else
                {
                    if (thisController.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 rotationValues) && ((rotationValues.x != 0) || (rotationValues.y != 0)))
                    {
                        RotateRig(rotationValues);
                    }
                }

                break;
        }
    }

    void HandleTeleport()
    {
        // Left Hand -> Movement
        if (thisController.role == InputDeviceRole.LeftHanded)
        {
            if (thisController.TryGetFeatureValue(CommonUsages.primaryButton, out bool btn_A_Pressed) && btn_A_Pressed)
            {
                Debug.Log("X pressed");
                if (LRayInteractor.GetCurrentRaycastHit(out RaycastHit raycastHit))
                {
                    Debug.Log("Raycast hit, creating teleport");
                    // 
                    //selectedObject = raycastHit.collider.gameObject;
                    //Select(selectedObject);
                    // Instantiate(teleportPrefab, raycastHit.transform);
                    Instantiate(teleportPrefab);
                }
            }
        }
    }


    void MoveRig(Vector2 movementValues)
    {
        rigObject.transform.position += transform.forward * (movementValues.y * speedMove) * Time.deltaTime;
        rigObject.transform.position += transform.right * (movementValues.x * speedMove) * Time.deltaTime;
    }

    void MoveRig(Vector3 coordinateValue)
    {

    }

    void RotateRig(Vector2 rotationValues)
    {
        rotationY += rotationValues.x * sensitivity; // X Axis
        // rotationX += rotationValues.y * sensitivity;

        // rotationX = Mathf.Clamp(rotationX, -60, 60);

        rigObject.transform.localEulerAngles = new Vector3(transform.rotation.x, rotationY, transform.rotation.y);
    }

    public XRRayInteractor ReturnCurrentLRay()
    {
        return LRayInteractor;
    }

    public InputDevice ReturnInputDevice()
    {
        return thisController;
    }

    #endregion
}
