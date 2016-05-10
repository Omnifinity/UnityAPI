/*
 * Omnifinity Unity API - Beta
 * Copyright 2014-2016 Omnifinity
 * All rights reserved. 
 * 
 * License terms can be found in LICENSE.MD
 * 
 */

/* TODO: 
 * Migrate character controller from OVR to HTC Vive to allow for ground plane collision.
 * Append proper error handling throughout the whole repository.
 */

/*
 *
 * Description:
 * Class for access to typically used data from Omnitrack
 * 
 */

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using Omnifinity;

namespace Omnifinity {

	namespace Omnitrack {
		
		public class Omnitrack : MonoBehaviour {

			#region PUBLIC_VARIABLES
			/// <summary>
			/// Print general debugging information of your preference (see the Update() routine).
			/// </summary>
			private bool printSensorData = false;
			#endregion PUBLIC_VARIABLES

			/********************************************************
			* 
			* Omnitrack API
			* START OF PUBLIC METHODS FOR YOUR USAGE
			* 
			********************************************************/
			#region PUBLIC_METHODS
			/// <summary>
			/// Get World position [m], movement speed [m/s] and quaternion of the body
			/// </summary>
			/// <returns>The movement vector.</returns>
			public Vector3 getBodyWorldPosition() { return omnitrackDataHandler.getSensorPosition (SensorConf.BODY_WORLDCOORDINATES); }
			public Vector3 getBodyWorldVelocity() { return omnitrackDataHandler.getSensorVelocity (SensorConf.BODY_WORLDCOORDINATES); }
			public Quaternion getBodyWorldQuaternion() { return omnitrackDataHandler.getSensorQuat (SensorConf.BODY_WORLDCOORDINATES); }

			/// <summary>
			/// Get Local Omnideck-platform position [m], movement speed [m/s] and quaternion
			/// </summary>
			/// <returns>The movement vector based on the base of the Omnideck platform.</returns>
			public Vector3 getBodyPosition() { return omnitrackDataHandler.getSensorPosition (SensorConf.BODY); }
			public Vector3 getBodyVelocity() { return omnitrackDataHandler.getSensorVelocity (SensorConf.BODY); }
			public Quaternion getBodyQuaternion() { return omnitrackDataHandler.getSensorQuat (SensorConf.BODY); }

			/// <summary>
			/// Gets corresponding data from all other sensors
			/// </summary>
			public Vector3 getHeadPosition() { return omnitrackDataHandler.getSensorPosition (SensorConf.HEAD); }
			public Vector3 getHeadVelocity() { return omnitrackDataHandler.getSensorVelocity (SensorConf.HEAD); }
			public Quaternion getHeadQuaternion() { return omnitrackDataHandler.getSensorQuat (SensorConf.HEAD); }

			public Vector3 getLeftHandPosition() { return omnitrackDataHandler.getSensorPosition (SensorConf.LEFTHAND); }
			public Vector3 getLeftHandVelocity() { return omnitrackDataHandler.getSensorVelocity (SensorConf.LEFTHAND); }
			public Quaternion getLeftHandQuaternion() { return omnitrackDataHandler.getSensorQuat (SensorConf.LEFTHAND); }

			public Vector3 getRightHandPosition() { return omnitrackDataHandler.getSensorPosition (SensorConf.RIGHTHAND); }
			public Vector3 getRightHandVelocity() { return omnitrackDataHandler.getSensorVelocity (SensorConf.RIGHTHAND); }
			public Quaternion getRightHandQuaternion() { return omnitrackDataHandler.getSensorQuat (SensorConf.RIGHTHAND); }

			public Vector3 getLeftLegPosition() { return omnitrackDataHandler.getSensorPosition (SensorConf.LEFTLEG); }
			public Vector3 getLeftLegVelocity() { return omnitrackDataHandler.getSensorVelocity (SensorConf.LEFTLEG); }
			public Quaternion getLeftLegQuaternion() { return omnitrackDataHandler.getSensorQuat (SensorConf.LEFTLEG); }

			public Vector3 getRightLegPosition() { return omnitrackDataHandler.getSensorPosition (SensorConf.RIGHTLEG); }
			public Vector3 getRightLegVelocity() { return omnitrackDataHandler.getSensorVelocity (SensorConf.RIGHTLEG); }
			public Quaternion getRightLegQuaternion() { return omnitrackDataHandler.getSensorQuat (SensorConf.RIGHTLEG); }
			#endregion PUBLIC_METHODS
			/********************************************************
			* 
			* Omnitrack API
			* END OF PUBLIC METHODS FOR YOUR USAGE
			* 
			********************************************************/

			#region PRIVATE_VARIABLES
			/// <summary>
			/// An Omnitrack game object. Either use the tag above or assigned through drag-drop.
			/// </summary>
			private GameObject omnitrackDataObject;

			/// <summary>
			/// OmnitrackDataHandler component that is attached to the Omnitrack gameobject.
			/// </summary>
			private OmnitrackDataHandler omnitrackDataHandler;

			// flag to tell success or not
			private bool isStartSuccessful = false;
			#endregion PRIVATE_VARIABLES
			
			#region #PRIVATE_METHODS
			void Start () {
				Debug.Log ("[Omnitrack] Version: " + OmnitrackDefinitions.getVersionString (), gameObject);

				// find tracking data communication object
				if (!omnitrackDataObject) {
                    // find the data handler object
                    OmnitrackDataHandler dataHandler = FindObjectOfType<OmnitrackDataHandler>();
                    omnitrackDataObject = dataHandler.gameObject;

					if (!omnitrackDataObject) {
						Debug.LogError ("Error: Unable to find an OmnitrackDataHandler object. Aborting.", gameObject);
						isStartSuccessful = false;
						return;
					} else {
						Debug.Log ("[Omnitrack] Found OmnitrackDataHandler through type", gameObject);
					}
				} else {
					Debug.Log ("[Omnitrack] Found OmnitrackDataHandler object through user drag-drop assignment in GUI", gameObject);
				}

				// find tracking data communication component
				omnitrackDataHandler = omnitrackDataObject.GetComponent <OmnitrackDataHandler> ();
				if (omnitrackDataHandler == null) {
					Debug.LogError ("[Omnitrack] Unable to find OmnitrackDataHandler component. Please add it.", gameObject);
				}
			}

			void Update () {
				// Print some sensor information
				if (printSensorData) {
					Debug.Log ("Body world pos:" + getBodyWorldPosition ());
                    Debug.Log("Body pos:" + getBodyPosition());
					Debug.Log ("Head pos:" + getHeadPosition ());
					Debug.Log ("Left Hand pos:" + getLeftHandPosition ());
					Debug.Log ("Right Hand pos:" + getRightHandPosition ());
					Debug.Log ("Left Leg pos:" + getLeftLegPosition ());
					Debug.Log ("Right Leg pos:" + getRightLegPosition ());
				}
			}
			#endregion #PRIVATE_METHODS
		}
	}
}