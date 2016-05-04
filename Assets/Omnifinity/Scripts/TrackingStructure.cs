/*
LICENSE TERMS

Omnifinity Unity API - Beta
Copyright 2014-2016 Omnifinity
All rights reserved.

You are ALLOWED to:
 - Freely download and use this Omnifinity Unity API software in whole or part 
   for personal, company internal or commercial purposes.
 - Distribute this software in packages or distributions that you've created.

Your are FORBIDDEN to:
- Redistribute any pieces of this software without proper attribution.
- Use any marks owned by Omnifinity in any way that might state that Omnifinity
  endourses your distribution or implying that you have created the Omnifinity
  software in question.

You are REQUIRED to:
- Distribute this LICENCE-file in any package or redistribution that you
  have created.
- Provide clear attribution to Omnifinity for any redistribution that include
  the Omnifinity software.
  
It DOES NOT REQUIRE you to:
- Include the source of the Omnifinity software or any modifications that 
  you have made to it in any redistribution you assemble.
- Share alterations of the source code to Omnifinity - although we encourage it.

END OF LICENSE TERMS
*/

/*
 *
 * Description:
 * Class containing various structured used when communicating with Omnitrack
 */

using UnityEngine;

using System;
using System.Collections;
using System.Runtime.InteropServices;

using Omnifinity;

namespace Omnifinity {
	
	namespace Omnitrack {

		public class TrackingStructure  {

			public struct OmnitrackVersion {
				public int major;
				public int minor;
				public int patch;
			}

			/// <summary>
			/// Various floor parameters
			/// </summary>
			public struct Floor {
				public float angle;
				public float speed;
			} // 8 bytes
			
			
			/// <summary>
			/// Position and rotation data
			/// </summary>	
			public struct PosAndRot {
				public float x;
				public float y;
				public float z;
				public float qx;
				public float qy;
				public float qz;
				public float qw;
			} // 28 bytes

			/// <summary>
			/// A tracking sensor with io, position and orientation
			/// </summary>	
			public struct Sensor {
				public float io;
				public PosAndRot pr;
			} // 32 bytes

			/// <summary>
			/// Collection of a sensor dataframe
			/// </summary>	
			public struct OmnitrackDataFrame {
				public Floor floor;					// Various Omnideck data
				public Sensor bodyWorldCoord;		// World position of characters body - calculated from human movement
				public Sensor body;					// Local position of character body relative to floor base
				public Sensor tool;					// Local position of a tool relative to floor base
				public Sensor head;					// Local position of head - this is where you attach the VR camera (needs height adjustment based on where sensor is mounted on the human)
				public Sensor rightHand;			// Local position of hands and so on relative to floor base
				public Sensor leftHand;
				public Sensor rightLeg;
				public Sensor leftLeg;
			} // 264 bytes
			
			/// <summary>
			/// Data frame for game-controlled feedback control (disabled)
			/// </summary> 
			public struct ReplyDataFrame {
				public float speed;
			} // 4 bytes

			/// <summary>
			/// Copy data to an OmnitrackDataFrame struct
			/// </summary>
			/// <returns>An OmnitrackDataFrame for further parsing</returns>
			/// <param name="dataArr">Data arr with data to be copied.</param>
			public OmnitrackDataFrame getODF( byte[] dataArr) {
				OmnitrackDataFrame odf = new OmnitrackDataFrame();
				
				int size = Marshal.SizeOf(odf);
				IntPtr ptr = Marshal.AllocHGlobal(size);
				Marshal.Copy (dataArr, 0, ptr, size);
				
				odf = (OmnitrackDataFrame) Marshal.PtrToStructure(ptr, odf.GetType());
				Marshal.FreeHGlobal(ptr);
				
				return odf;
			}

			/// <summary>
			/// Copy data to an OmnitrackVersion struct
			/// </summary>
			/// <returns>An OmnitrackVersion struct for further parsing</returns>
			/// <param name="dataArr">Data arr with data to be copied.</param>
			public OmnitrackVersion getSenderVersion( byte[] dataArr) {
				OmnitrackVersion omnitrackVersion = new OmnitrackVersion();

				int size = Marshal.SizeOf(omnitrackVersion);
				IntPtr ptr = Marshal.AllocHGlobal(size);
				Marshal.Copy (dataArr, 0, ptr, size);

				omnitrackVersion = (OmnitrackVersion) Marshal.PtrToStructure(ptr, omnitrackVersion.GetType());
				Marshal.FreeHGlobal(ptr);

				return omnitrackVersion;
			}
		}
	}
}