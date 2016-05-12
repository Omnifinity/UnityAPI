using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Omnifinity.Omnitrack;

public class OmnitrackHandController : MonoBehaviour
{
    private OmnitrackCharacterController omnitrackCharacterController;

    private Valve.VR.EVRButtonId gripButton = Valve.VR.EVRButtonId.k_EButton_Grip;
    private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;

    private SteamVR_TrackedObject trackedObject;

    // get the most newest object index
    private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)trackedObject.index); } }
    
    private bool temporaryDisabledInteractingItemCollider = false;

    public GameObject omnitrackDebugPrefab;
    private GameObject goOmnitrackDebug;

    // Use this for initialization
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

        if (controller.GetPressDown(gripButton))
        {
            // flip mode
            omnitrackCharacterController.flipIsPositionEnabled();
            if (omnitrackCharacterController.getIsPositionEnabled())
            {
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

    }

    void OnTriggerEnter(Collider collider)
    {
    }

    void OnTriggerExit(Collider collider)
    {
    }
}
