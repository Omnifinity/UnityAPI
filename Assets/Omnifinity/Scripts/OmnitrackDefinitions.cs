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
 * Class to define basic properties
 * 
 */

using System;

namespace Omnifinity
{
	namespace Omnitrack {
		/// <summary>
		/// Enums for different sensors/trackers
		/// </summary>
		public enum SensorConf { BODY_WORLDCOORDINATES = 0, BODY, TOOL, HEAD, RIGHTHAND, LEFTHAND, RIGHTLEG, LEFTLEG};

		public static class OmnitrackDefinitions
		{
			
			/// <summary>
			/// Version notification of the data-structure in this build
			/// DO NOT CHANGE
			/// </summary>
			const int major = 1;
			const int minor = 15;
			const int patch = 9;

			public static int getMajor() {
				return major;
			}
			public static int getMinor() {
				return minor;
			}
			public static int getPatch() {
				return patch;
			}

			public static string getVersionString() {
				return string.Concat (getMajor (), ".", getMinor (), ".", getPatch());
			}

		}
	}
}