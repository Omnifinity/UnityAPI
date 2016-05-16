using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Omnifinity.Omnitrack;

public class OmnitrackHandController : MonoBehaviour
{

    private OmnitrackCharacterController omnitrackCharacterController;

    private Valve.VR.EVRButtonId gripButton = Valve.VR.EVRButtonId.k_EButton_Grip;
    private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;
    private Valve.VR.EVRButtonId touchpadButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad;

    private SteamVR_TrackedObject trackedObject;

    // get the most newest object index
    private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)trackedObject.index); } }
    
    private bool temporaryDisabledInteractingItemCollider = false;

    public GameObject omnitrackDebugPrefab;
    private GameObject goOmnitrackDebug;

    void Start()
    {
        // reference to the tracked object
        trackedObject = GetComponent<SteamVR_TrackedObject>();

        omnitrackCharacterController = FindObjectOfType<OmnitrackCharacterController>();
        omnitrackCharacterController.setIsPositionEnabled(false);
    }

    void Update()
    {
        if (controller == null)
        {
            Debug.Log("Controller not initialized");
            return;
        }

        // If user is gripping the handcontroller the Omnitrack software will add a 10% translation offset
        // in XZ-plane to the Omnitrack Character Controller. This is very subtle.
        // This is how you simulate movement in your game when you do not have access to an Omnideck.
        if (controller.GetPressDown(gripButton))
        {
            // flip mode
            omnitrackCharacterController.flipIsPositionEnabled();
            if (omnitrackCharacterController.getIsPositionEnabled())
            {
                omnitrackCharacterController.setIsVelocityCalculationBasedOnPositionEnabled(true);

                // show Omnifinity debug gfx at hand controller
                goOmnitrackDebug = Instantiate(omnitrackDebugPrefab, transform.position, transform.rotation) as GameObject;
                goOmnitrackDebug.transform.parent = transform;
                goOmnitrackDebug.transform.Rotate(Vector3.right, 45f, Space.Self);
                goOmnitrackDebug.transform.Translate(Vector3.up * 0.1f, Space.Self);
            }
            else
            {
                if (goOmnitrackDebug != null)
                {
                    Destroy(goOmnitrackDebug);
                }
            }
        }

        // Allow flying
        if (controller.GetPress(triggerButton))
        {
            omnitrackCharacterController.setIsPositionEnabled(true);
            omnitrackCharacterController.setIsVelocityCalculationBasedOnPositionEnabled(false);
            Vector3 vel = transform.rotation * Vector3.forward;
            omnitrackCharacterController.setSensorVelocity(vel);
        }

        // Disable flying
        if (controller.GetPressUp(triggerButton))
        {
            omnitrackCharacterController.setIsVelocityCalculationBasedOnPositionEnabled(false);
            omnitrackCharacterController.setSensorVelocity(new Vector3(0, 0, 0));
        }


        if (controller.GetPress(touchpadButton))
        {
        }

        if (controller.GetPressUp(touchpadButton))
        {
        }
    }

    void OnTriggerEnter(Collider collider)
    {
    }

    void OnTriggerExit(Collider collider)
    {
    }
}
