/*
 * Omnifinity Unity API - Beta
 * Copyright 2014-2016 Omnifinity
 * All rights reserved. 
 * 
 * License terms can be found in LICENSE.MD
 * 
 */

/*
*
* Description:
* Class for moving an OmnitrackCharacterController (basically an ordinary Unity Character Controller
* with some added code. 
* 
*/

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using Omnifinity;

namespace Omnifinity {

	namespace Omnitrack {

		// Inherits OmnitrackSensor properties
		public class OmnitrackCharacterController : OmnitrackSensor {

            //public GameObject playerController;


			#region PUBLIC_VARIABLES
			#endregion PUBLIC_VARIABLES

			#region PRIVATE_VARIABLES
            private bool isCharacterController = true;

			// The character controller
			private GameObject omnitrackCharacterBody;

			// The character controller should have a sensor attached to it
			private OmnitrackSensor omnitrackCharacterBodySensor;

            /// <summary>
            /// Character controller that handles movement and collision with objects
            /// </summary>
			private CharacterController charController;

            /// <summary>
            /// Movement speed
            /// </summary>
            private Vector3 moveSpeed;
			
			/// <summary>
			/// A body height adjustment gameobject that should be child of a body sensor object
			/// and parent of all other sensor objects
			/// </summary>
			private OmnitrackPlatformHeightAdjuster bodyHeightAdjusterObject;

            /// <summary>
            /// SteamVR Controller Manager
            /// </summary>
            private SteamVR_ControllerManager steamVRControllerManager;
            private bool isSteamVRCameraGrounded;

            /// <summary>
            /// TODO: Implement code for excessive initial movement
            /// </summary>
			private bool hasInitializedMovement = false;
			private float timeAssumeInitFinished;

			#endregion PRIVATE_VARIABLES

			#region PUBLIC_METHODS
			/// <summary>
			/// An OmnitrackCharacterController containing one tracking sensor object
			/// </summary>
			public override void Start () {
				base.Start ();

				// find character controller object
				if (isCharacterController) {
					charController = GetComponent<CharacterController> ();
                }

                // find steam vr controller manager
                steamVRControllerManager = FindObjectOfType<SteamVR_ControllerManager>();
                if (!steamVRControllerManager)
                {
                    Debug.LogError("[Omnitrack] Error: Unable to find the SteamVR_ControllerManager.");
                    return;
                }
			}

			/// <summary>
			/// Update the character based on movement and input
			/// </summary>
			public override void Update () {
				ParseData ();
				MoveObject ();

                if (!isSteamVRCameraGrounded)
                    EnsureSteamVRCameraIsGrounded();
			}

            /// <summary>
            /// Moves the SteamVR CameraRig to the bottom of the character controller upon initial ground collision
            /// </summary>
            private void EnsureSteamVRCameraIsGrounded() {
                if (charController.isGrounded)
                {
                    // cast ray against ground
                    RaycastHit rayCastHit;
                    bool hit = Physics.Raycast(transform.position, -Vector3.up, out rayCastHit);

                    // adjust height of the steamVRControllermanager
                    Vector3 posAdjuster = new Vector3 (0, -rayCastHit.distance, 0);
                    steamVRControllerManager.transform.localPosition += posAdjuster;

                    isSteamVRCameraGrounded = true;
                    Debug.Log("[Omnitrack] SteamVR camera has been grounded in relation to character controller (" + -rayCastHit.distance + " m)");
                }
            }

			/// <summary>
			/// Moves the character controller object.
			/// </summary>
			public override void MoveObject () {

				// if position is enabled
				if (isPositionEnabled) {
					if (!hasInitializedMovement) {
                        // TODO: ADD CODE
                        // unfinished code to account for excessive joystick movement
						hasInitializedMovement = true;

					} else {
						
                        // get movement velocity
                        moveSpeed = getSensorVelocity();

                        // take gravity into account (thus does not require a rigid body)
                        if (!charController.isGrounded) 
                            moveSpeed.y = Physics.gravity.y * Time.deltaTime;

                        // move character controller with velocity vector
                        charController.SimpleMove(moveSpeed);
					}
				} else { 
					// For debugging only:
					// when the position for a character controller is disabled it 
					// falls right through everything, use this to make it 
					// stay put 
					charController.SimpleMove (new Vector3(0,0,0));
				}
			}
			#endregion PUBLIC_METHODS

			#region PRIVATE_METHODS
			#endregion PRIVATE_METHODS
		}
	}
}
