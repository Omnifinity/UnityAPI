## RELEASE NOTES
**Omnitrack Unity API**

**V1.15.11**

- Removed Oculus VR specific code (height adjustment etc).
- Added SteamVR CameraRig height adjustment to make sure that it is placed at the bottom of the OmnitrackCharacterController (and thus being aligned with ground platform).
- Added OmnitrackCharacterController prefab.
- Added OmnitrackGameClient executable that is what users should use to acquire tracking data from the Omnideck when using the HTC Vive Lighthouse tracking system.



**V1.15.10**

- Removed obsolete OVR head height adjustment code.
- Removed reference to gameObject in Debug.Log/LogError output in async callback.

**V1.15.9**

- Initial commit to GitHub. 
