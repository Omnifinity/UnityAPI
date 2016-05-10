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
 * Class for calculating tracking data
 * 
 */

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using Omnifinity;

namespace Omnifinity {

	namespace Omnitrack {

		public class OmnitrackSensor : MonoBehaviour {

			#region PUBLIC_VARIABLES

			/// <summary>
			/// Enums for different trackers
			/// </summary>
			public SensorConf sensorType = SensorConf.BODY_WORLDCOORDINATES;

            public bool isPositionEnabled = true;
            public bool isRotationEnabled = true;

            /// <summary>
            /// For manual debugging
            /// </summary>
            public bool printCoords = false;

			#endregion PUBLIC_VARIABLES

			#region PRIVATE_VARIABLES           
			/// <summary>
			/// Communication object for tracking data, initialized upon start
			/// </summary>
			private OmnitrackCommunication communicationObject;
            private GameObject omnitrackCommunicationGameObject;

			/// <summary>
			/// Whether to use right-hand-side to left-hand-side coordinate system conversion conversion or not
            /// This is controlled externally by the DataHandler-script
			/// </summary>
			private bool ConvertRHS_to_LHS = true;

			/// <summary>
			/// Various tracking data
			/// </summary>
			private float io = 0;
			private float x = 0.0f;
			private float y = 0.0f; 
			private float z = 0.0f;
			private float qx = 0.0f; 
			private float qy = 0.0f; 
			private float qz = 0.0f;
			private float qw = 0.0f;

			private Vector3 sensorPosition;
			private Vector3 sensorVelocity;
			private Vector3 sensorDeltaPosition;
			private Quaternion sensorQuat;

			/// <summary>
			/// For sensor smoothing
			/// </summary>
			private Quaternion prevQuat, newQuat;
			
			/// <summary>
			/// Scale the translation values upon desire to allow greater movement
			/// </summary>
			private Vector3 factorTrans = new Vector3(1, 1, 1);

			/// <summary>
			/// Sensor object for parsing
			/// </summary>
			private TrackingStructure.Sensor sensor;

			/// <summary>
			/// New and current position of a tracked object, for smoothing / relative position calculation
			/// </summary>
			private Vector3 newPos;
			private Vector3 prevPos;
			
			private bool isPressed = false;
			private bool isFiring = false;
			
			private bool isExternallyControlled = false;

			private bool isStartSuccessful = false;

            // Various variables

            /// <summary>
            ///  If data should be smoothened or not - overwridden by the OmnitrackDataHandler script
            ///  </summary>
            private bool usePositionSlerpSmoothing = true;
            private float slerpPositionFactor = 0.9f;

            private bool useRotationSlerpSmoothing = true;
            private float slerpRotationFactor = 0.9f;

            // enables manual adjustment to the received data
            private bool useMuliplyingFactorOnXYZ = false;

			#endregion PRIVATE_VARIABLES

			#region PUBLIC_METHODS

            /// <summary>
            /// Convert data from Right-Hand-Side to Left-Hand-Side coordinate system or nog
            /// </summary>
            /// <param name="mode"></param>
			public void setConvertRHS_to_LHS (bool mode) { ConvertRHS_to_LHS = mode; }

			/// <summary>
			/// Use multiplying factor or not
			/// </summary>
			/// <param name="mode"></param>
			public void setUseMultiplyingFactorOnXYZ (bool mode) { useMuliplyingFactorOnXYZ = mode; }
			public bool getUseMultiplyingFactorOnXYZ () { return useMuliplyingFactorOnXYZ; }

			public void setMultiplyingFactorsOnXYZ (Vector3 factors) { factorTrans = factors; }

            /// <summary>
            /// Set position and orientation slerp factors, is goverend from the OmnitrackDataHandler script
            /// </summary>
            /// <param name="mode"></param>
			public void setUsePositionSlerpSmoothing(bool mode) { usePositionSlerpSmoothing = mode; }
            public void setUseRotationSlerpSmoothing(bool mode) { useRotationSlerpSmoothing = mode; }
            public void setPositionSlerpSmoothingFactor(float val) { slerpPositionFactor = val;  }
            public void setRotationSlerpSmoothingFactor(float val) { slerpRotationFactor = val; }

			/// <summary>
			/// Gets the type of the sensor.
			/// </summary>
			/// <returns>The SensorConf type.</returns>
			public SensorConf getSensorType() { return sensorType; }

			/// <summary>
			/// Setter and Getter of sensor position, velocity and quaternion
			/// </summary>
			private void setSensorPosition (Vector3 pos) { 
				// calculate velocity
				sensorVelocity = (pos - sensorPosition) / Time.deltaTime;

				//Debug.Log ("Deltapos: " + (pos - sensorPosition));

				sensorDeltaPosition = pos - sensorPosition;

				// store new position
				sensorPosition = pos;
			}
			private void setSensorQuat (Quaternion quat) { sensorQuat = quat; }

			/// <summary>
			/// Get sensor position/velocity/quat based on sensor type
			/// </summary>
			/// <returns>The sensor position/velocity or quaternion.</returns>
			public Vector3 getSensorPosition() { return sensorPosition; }
			public Vector3 getSensorVelocity() { return sensorVelocity; }
			public Vector3 getSensorDeltaPosition() { return sensorDeltaPosition; }
			public Quaternion getSensorQuat() { return sensorQuat; }

			/// <summary>
			/// A tracking sensor object
			/// </summary>
			public virtual void Start () {
				// find tracking data communication Game Object
				if (!omnitrackCommunicationGameObject) {
                    // find tracking data communication component
                    communicationObject = FindObjectOfType<OmnitrackCommunication>();
                    if (communicationObject == null) {
                        Debug.LogError("Unable to find OmnitrackCommunication object. Please assign it.", gameObject);
                    }

                    omnitrackCommunicationGameObject = communicationObject.gameObject;
					if (!omnitrackCommunicationGameObject) {
						Debug.LogError ("Unable to find an OmnitrackCommunication object. Aborting.", gameObject);
						isStartSuccessful = false;
						return;
					}
				} else {
					Debug.Log ("Found Omnitrack Communication game object through user drag-drop assignment in GUI", gameObject);
				}

			}
			
			/// <summary>
			/// Update the object based on movement and input
			/// </summary>
			public virtual void Update () {
				ParseData ();
				MoveObject ();
			}

			public virtual void ParseData() {
				switch (sensorType) {
				case SensorConf.BODY_WORLDCOORDINATES:
					sensor = communicationObject.trackingData.bodyWorldCoord;
					break;
				case SensorConf.BODY:
					sensor = communicationObject.trackingData.body;
					break;
				case SensorConf.TOOL:
					sensor = communicationObject.trackingData.tool;
					break;
				case SensorConf.HEAD:
					sensor = communicationObject.trackingData.head;
					break;
				case SensorConf.RIGHTHAND:
					sensor = communicationObject.trackingData.rightHand;
					break;
				case SensorConf.LEFTHAND:
					sensor = communicationObject.trackingData.leftHand;
					break;
				case SensorConf.RIGHTLEG:
					sensor = communicationObject.trackingData.rightLeg;
					break;
				case SensorConf.LEFTLEG:
					sensor = communicationObject.trackingData.leftLeg;
					break;
				}

				// get io state, either digital or analog
				io = (float) sensor.io;

				// get position data
				if (!useMuliplyingFactorOnXYZ) {
					x = sensor.pr.x;
					y = sensor.pr.y;
					z = sensor.pr.z;
				} else {
					x = sensor.pr.x * factorTrans.x;
					y = sensor.pr.y * factorTrans.y;
					z = sensor.pr.z * factorTrans.z;
				}

				// get orientation
				qx = sensor.pr.qx;
				qy = sensor.pr.qy;
				qz = sensor.pr.qz;
				qw = sensor.pr.qw;

				if (ConvertRHS_to_LHS) {
					// right to left handed coordinate system conversion...
					z = -z;
					qz = -qz;
					qw = -qw;
				}
				// ... now the position and orientation is properly parsed.

				// calculate the new position
				if (!usePositionSlerpSmoothing)
					newPos = new Vector3 (x, y, z);
				else
					newPos = Vector3.Slerp (prevPos, newPos, slerpPositionFactor);

				// store Un-smoothened Position for external access
				setSensorPosition(newPos);

				// assign rotation
				if (!useRotationSlerpSmoothing)
					newQuat = new Quaternion(qx, qy, qz, qw);
				else
					// dampen the signal between old and new signal
					newQuat = Quaternion.Slerp(prevQuat, newQuat, slerpRotationFactor);
				
				// store Untiltered/Unsmoothened Quat for external access
				setSensorQuat(newQuat);
				
				// print various debugging information
				if (printCoords == true) 
					Debug.Log ("x: " + x + " y: " + y + " z: " + z + " qx: " + qx + " qy: " + qy + " qz:" + qz + " w: " + qw);

			}

			public virtual void MoveObject() {
				// if position is enabled
				if (isPositionEnabled) {
					// or set new position of the sensor object with the position
					gameObject.transform.localPosition = newPos;

					// store pos for next iteration
					prevPos = newPos;
				}

				// if rotation is enabled
				if (isRotationEnabled) {

					// set the rotation of this game object
					transform.rotation = newQuat;

					// store quat for next iteration
					prevQuat = newQuat;
				}
            }

            #endregion PUBLIC_METHODS
        }
	}
}