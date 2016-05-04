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
			/// Various tags, gameobjects and components that need to be properly assigned.
			/// </summary>

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
                    //charController = playerController.GetComponent<CharacterController>();

					if (!charController) {
						Debug.LogError ("Unable to get hold of the CharacterController. Attach me to an object with a Unity CharacterController.", gameObject);
					}

					// find height adjustment object to get correct height between Omnideck and Unity Game.
					bodyHeightAdjusterObject = GameObject.FindObjectOfType<OmnitrackPlatformHeightAdjuster> ();
					if (!bodyHeightAdjusterObject) {
						Debug.LogError ("Unable to get hold of the body height ground lever adjuster gameobject", gameObject);
					}

					// Adjust with skinwidth to get really accurate, the value is the default CharacterController skinwidth
					// which is not available for query in Unity 5.1
					float charControllerskinWidth = 0.08f;
					bodyHeightAdjusterObject.transform.localPosition = new Vector3 (0, -charController.height / 2.0f - charControllerskinWidth, 0);
					// CharacterController.skinWidth not avail until Unity 5.2 according to:
					// http://forum.unity3d.com/threads/skinwidth-parameter-of-a-character-controller-inaccessible.18064/
					//bodyHeightAdjusterObject.transform.localPosition = new Vector3 (0, -(charController.height - charController.skinWidth) / 2, 0);
				}
			}

			/// <summary>
			/// Update the object based on movement and input
			/// </summary>
			public override void Update () {
				ParseData ();
				MoveObject ();
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
