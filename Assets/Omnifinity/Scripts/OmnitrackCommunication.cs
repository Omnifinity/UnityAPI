/*
 * Omnifinity Unity API - Beta
 * Copyright 2014-2016 Omnifinity
 * All rights reserved. 
 * 
 * License terms can be found in LICENSE.MD
 * 
 */

/*
 * Description:
 * Class for communicating with Omnitrack through the UDP-protocol
 * 
 */

using UnityEngine;
using System.Collections;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using Omnifinity;

namespace Omnifinity {
	
	namespace Omnitrack {

		public class OmnitrackCommunication : MonoBehaviour {

			#region PUBLIC_VARIABLES
			/// <summary>
			/// omnitrackDataPort guy world coordinates
			/// </summary>
			public int omnitrackDataPort = 11016;

			/// <summary>
			/// Prints packet statistics
			/// </summary>
			public bool printPacketStats = true;
			
			/// <summary>
			/// Various tracking structures
			/// </summary>
			public TrackingStructure.OmnitrackVersion omnitrackVersion;
			public TrackingStructure.OmnitrackDataFrame trackingData;

			#endregion

			#region PRIVATE_VARIABLES
			/// <summary>
			/// Network protocol parameters
			/// </summary>
			private bool raiseVersionWarning = false;

			private byte[] recieveBuffer;
			private int bufferSize = 4096;
			private EndPoint bindEndPoint;
			private EndPoint bindSendEndPoint;

			/// <summary>
			/// Tracking data that gets filled with data by Omnitrack
			/// </summary>
			private TrackingStructure trackingObject;

			/// <summary>
			/// For Frames-per-second debugging in log window
			/// </summary>
			//public bool printFPS = true;
			private long ticksIncoming;
			private long ticksLastIncoming;
			private long deltaTicksIncoming;
			
			/// <summary>
			/// Communication client and various sockets
			/// </summary>
			private UdpClient client;
			
			/// <summary>
			/// Socket for receiving omnitrack world coordinate
			/// </summary>
			private Socket omnitrackSocket;
			private EndPoint bindOmnitrackEndPoint;

			/// <summary>
			/// Packet counter for statistics
			/// </summary>
			private long packetCount = 0;
			private long currPacketCount, prevPacketCount;

			/// <summary>
			/// Timing and info for statistics
			/// </summary>
			private float timeStart, timeEnd;
			private float timeRecv, timeDelta, timePrev;

			#endregion PRIVATE_VARIABLES
			
			#region PUBLIC_METHODS
			public bool isVersionWarningRaised() { return raiseVersionWarning; }
			#endregion PUBLIC_METHODS

			#region PRIVATE_METHODS
			/// <summary>
			/// Initialize receive of custom data
			/// </summary>
			void Start () {
				Debug.Log ("[Omnitrack] Setting up communication of tracking data on port " + omnitrackDataPort, gameObject);
				
				recieveBuffer = new Byte[bufferSize];
				
				SetupCommunication ();
				Debug.Log ("[Omnitrack] Tracking data communication successful. Awaiting Omnideck tracking data...", gameObject);
				
				trackingObject = new TrackingStructure();
				
				// start timer for debugging purposes
				timeStart = Time.time;
				
				StartCoroutine ("CalculatePacketStats", 1.0f);
			}
			
			/// <summary>
			/// Print stats, cleanup and quit
			/// </summary>
			void OnApplicationQuit() { 
				
				timeEnd = Time.time;
				float timeRunning = timeEnd - timeStart;
				Debug.Log("[Omnitrack] Received #" + packetCount + " packets in " + timeEnd + " seconds (" + packetCount / timeRunning + " packets/second)");

				if (omnitrackSocket != null) {
					omnitrackSocket.Close ();
					print ("[Omnitrack] Data receive socket for port " + omnitrackDataPort + " closed");
				}
			}
			
			/// <summary>
			/// Setup the communication ports, bind, call receive thread
			/// </summary>
			void SetupCommunication() {

				// grab a socket
				omnitrackSocket = new Socket(AddressFamily.InterNetwork, 
				                           SocketType.Dgram, ProtocolType.Udp);

				// bind socket
				bindOmnitrackEndPoint = new IPEndPoint(IPAddress.Any, omnitrackDataPort);
				omnitrackSocket.Bind(bindOmnitrackEndPoint);

				// start thread for socket receive
				omnitrackSocket.BeginReceiveFrom(recieveBuffer, 0, bufferSize, SocketFlags.None, ref bindOmnitrackEndPoint, 
				                               new AsyncCallback(MessageOmnitrackReceivedCallback), (object)this);
			}

			/// <summary>
			///  Update and show packet receive stats
			/// </summary>
			/// <returns>The packet stats.</returns>
			/// <param name="delayTime">Delay time for when stats is being calculated. Defaults to once per second.</param>
			IEnumerator CalculatePacketStats(float delayTime = 1.0f) {
				while (true) {
					float packetFPS = packetCount - prevPacketCount;
					prevPacketCount = packetCount;

					if (printPacketStats) {
						Debug.Log ("[Omnitrack] Received #" + packetFPS + " packets per seconds on port " + omnitrackDataPort);
					}
					yield return new WaitForSeconds (delayTime);
				}
			}

			/// <summary>
			/// Wait for and parse incoming tracking packets
			/// </summary>
			void MessageOmnitrackReceivedCallback(IAsyncResult result) {
				EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

				try {
					// wait for data to arrive
					int bytesRead = omnitrackSocket.EndReceiveFrom (result, ref remoteEndPoint);

					// Very ugly way of receiving Omnitrack version number
					// TODO: Will become completedly deprecated when the API switches to VRPN
					if (bytesRead == System.Runtime.InteropServices.Marshal.SizeOf(omnitrackVersion)) {
						omnitrackVersion = trackingObject.getSenderVersion(recieveBuffer);
						if (OmnitrackDefinitions.getMajor() != omnitrackVersion.major ||
							OmnitrackDefinitions.getMinor() != omnitrackVersion.minor ||
							OmnitrackDefinitions.getPatch() != omnitrackVersion.patch){
								Debug.LogWarning("Warning! The tracking structure versions in the game and server do not match. This is usually not a problem.");
								Debug.LogWarning("Game   Version: " + OmnitrackDefinitions.getMajor() + "." + OmnitrackDefinitions.getMinor() + "." + OmnitrackDefinitions.getPatch());
								Debug.LogWarning("Server Version: " + omnitrackVersion.major + "." + omnitrackVersion.minor+ "." + omnitrackVersion.patch);
								raiseVersionWarning = true;
						} else {
							Debug.Log("[Omnitrack] All good. The versions of the TrackingStructure of this Game and the Tracking-server match eachother.");
						}
					} else if (bytesRead == System.Runtime.InteropServices.Marshal.SizeOf(trackingData)) {

						// get hold of new tracking data for the custom position frame and a tracking sensor frame
						trackingData = trackingObject.getODF (recieveBuffer);

						// increase packet counter
						packetCount++;
                    }
                    else
                    {
                        Debug.LogError("[Omnitrack] Error: Unsupported receive data", gameObject);
                    }
					
				} catch (SocketException e) {
					Debug.LogError ("[Omnitrack] Error:" + e.ErrorCode.ToString () + " " + e.Message.ToString ());
				}

				// listen for next package
				omnitrackSocket.BeginReceiveFrom(recieveBuffer, 0, bufferSize, SocketFlags.None, ref bindOmnitrackEndPoint, 
				                                  new AsyncCallback(MessageOmnitrackReceivedCallback), (object)this);
			}
			#endregion PRIVATE_METHODS
		}
	}
}