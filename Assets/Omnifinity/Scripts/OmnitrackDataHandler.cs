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
 * Class for assembling and accessing sensor data
 * 
 */

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using Omnifinity;

namespace Omnifinity {

	namespace Omnitrack {

		public class OmnitrackDataHandler : MonoBehaviour {

			#region PUBLIC_VARIABLES
			/// <summary>
			/// Whether to use right-hand-side to left-hand-side coordinate system conversion conversion or not
			/// </summary>
			private bool ConvertRHS_to_LHS = true;

            #endregion PUBLIC_VARIABLES

            #region PRIVATE_VARIABLES
            /// <summary>
            /// The OmnitrackCharacterController holding a set of sensors
            /// </summary>
            private OmnitrackCharacterController charController;

            /// <summary>
            /// The GameObject holding a set of sensors
            /// </summary>
            private GameObject characterWithAttachedSensors;

            /// <summary>
			/// The sensor objects (attached as sub-gameobjects to this gameobject)
			/// </summary>
			private List<OmnitrackSensor> sensorObjects;

			/// <summary>
			/// The sensor hashtable for dynamic organisation of sensors that are
			/// attached as sub-gameobjects to this gameobject
			/// </summary>
			private Hashtable sensorHashtable;
			
			/// <summary>
			///  If data should be smoothened or not - will overwrite selection in the OmnitrackSensor script
			/// </summary>
			private bool usePositionSlerpSmoothing = false;
			private float slerpPositionFactor = 0.9f;
			
			private bool useRotationSlerpSmoothing = false;
			private float slerpRotationFactor = 0.9f;

			// enables manual adjustment to the received data
			private bool useMuliplyingFactorOnXYZ = false;

			#endregion PRIVATE_VARIABLES

			#region PUBLIC_METHODS
			/// <summary>
			/// Get sensor Position based on sensor type
			/// </summary>
			/// <returns>The sensor position vector.</returns>
			/// <param name="sensorType">SensorConf type.</param>
			public Vector3 getSensorPosition(SensorConf sensorType) {
				OmnitrackSensor sens = (OmnitrackSensor) sensorHashtable [sensorType];
				return sens.getSensorPosition();
			}

			/// <summary>
			/// Get sensor Velocity based on sensor type
			/// </summary>
			/// <returns>The sensor velocity vector.</returns>
			/// <param name="sensorType">SensorConf type.</param>
			public Vector3 getSensorVelocity(SensorConf sensorType) {
				OmnitrackSensor sens = (OmnitrackSensor) sensorHashtable [sensorType];
				return sens.getSensorVelocity();
			}

			/// <summary>
			/// Get sensor position based on sensor type
			/// </summary>
			/// <returns>The sensor quaternion.</returns>
			/// <param name="sensorType">SensorConf type.</param>
			public Quaternion getSensorQuat(SensorConf sensorType) {
				OmnitrackSensor sens = (OmnitrackSensor) sensorHashtable [sensorType];
				return sens.getSensorQuat ();
			}
			#endregion PUBLIC_METHODS

			#region PRIVATE_METHODS
			void Start () {
				// setup hashtable of sensors
				sensorHashtable = new Hashtable ();

				if (!characterWithAttachedSensors) {
                    charController = FindObjectOfType<OmnitrackCharacterController>();
                    characterWithAttachedSensors = charController.gameObject;

					if (!characterWithAttachedSensors) {
                        Debug.LogError("[Omnitrack] Unable to find an OmnitrackSensor object. Please add a sensor object on the gameobject representing the main player movement object. Aborting.", gameObject);
						return;
					}
				} else {
					Debug.Log ("[Omnitrack] Found OmnitrackSensor object through user drag-drop assignment in GUI", gameObject);
				}

				OmnitrackSensor [] sens = characterWithAttachedSensors.GetComponentsInChildren<OmnitrackSensor> ();
                if (sens.Length > 0)
                {
                    Debug.Log("[Omnitrack] Found " + sens.Length + " OmnitrackSensor objects attached to the " + characterWithAttachedSensors + " gameobject", gameObject);
                } else {
                    Debug.LogError("[Omnitrack] Could not find any OmnitrackSensor objects", gameObject);
                    return;
                }

				for (int i = 0; i < sens.Length; i++) {
					SensorConf sensorType = sens[i].getSensorType();
					sensorHashtable[sensorType] = sens[i];
				}

				// Pass on some default values
				for (int i = 0; i < sens.Length; i++) {
					sens[i].setConvertRHS_to_LHS(ConvertRHS_to_LHS);
					sens[i].setUsePositionSlerpSmoothing(usePositionSlerpSmoothing);
                    sens[i].setUseRotationSlerpSmoothing(usePositionSlerpSmoothing);
                    sens[i].setPositionSlerpSmoothingFactor(slerpPositionFactor);
                    sens[i].setRotationSlerpSmoothingFactor(slerpRotationFactor);
				}
			}

			#endregion PRIVATE_METHODS
		}
	}
}