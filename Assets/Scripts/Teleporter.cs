using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class Teleporter : MonoBehaviour
{
    [SerializeField] private float teleportTime = 3f;

    private GameObject XRRig;
    private XRRayInteractor LRayInteractor;
    private RigMoveScript rigMoveController;
    private InputDevice thisController;

    // Start is called before the first frame update
    void Start()
    {
        rigMoveController = GameObject.Find("LeftHand Controller").GetComponent<RigMoveScript>();
        LRayInteractor = rigMoveController.ReturnCurrentLRay();
        thisController = rigMoveController.ReturnInputDevice();
        XRRig = GameObject.FindGameObjectWithTag("PlayerRig");
        this.transform.Rotate(0f, 0f, 90f, Space.Self);
    }

    // Update is called once per frame
    void Update()
    {
        HandleRaycast();
        HandleInput();
    }

    void HandleRaycast()
    {
        LRayInteractor.GetCurrentRaycastHit(out RaycastHit raycastHit);
        if (raycastHit.collider.gameObject.layer == 8)
        {
            transform.position = raycastHit.point;
            this.transform.Rotate(0f, 0f, 90f, Space.Self);
        }
    }

    void HandleInput()
    {
        // Trigger 
        if (thisController.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue) && triggerValue > 0.1f)
        {
            Vector3 newPosition = new Vector3(gameObject.transform.position.x, XRRig.transform.position.y, gameObject.transform.position.z);
            LeanTween.move(XRRig, newPosition, teleportTime).setEase(LeanTweenType.easeInOutQuad).setDelay(0f);

            Destroy(gameObject);
        }

        // B Button Press
        if (thisController.TryGetFeatureValue(CommonUsages.secondaryButton, out bool pressed) && pressed)
        {
            Destroy(gameObject);
        }
    }
}
