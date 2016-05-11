# Omnitrack Game Client
The **Omnitrack Game Client** handles communication between the Omnideck ([http://www.omnifinity.se](http://www.omnifinity.se "Omnifinity")), HTC Vive Lighthouse tracking system and the Unity Game Engine.


You use the software to make sure that your simulation/game will work with the Omnideck. It works by simply offsetting a Character Controller (which should containthe HTC Vive Camera as a child object) using a 2D velocity vector originating from the Omnideck.

This is the absolute bare minimum piece of software you need to be able to interact with the Omnideck using the HTC Vive. The Omnideck was designed to be used in a networked environment, thus the reason for using UDP packets. 

## Requirements
Make sure you have included **openvr_api.dll** in the same directory as the executable.  

## How to use
A batch file for windows have been included, adjust it to match the IP-adress of your computer and choose a suitable UDP port. Make sure that the port number matches the port you choose in Unity.

Refer to the main documentation (it is under construction) to learn more on how to use it.
 
## Attributions to third party software
**openvr_api.dll**
Copyright (c) 2015, Valve Corporation
All rights reserved. 