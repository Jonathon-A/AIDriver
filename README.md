# AIDriver
Using reinforcement and imitation learning to make an agent drive a car with somewhat realistic physics in Unity

Car controller used (after modification) from: https://assetstore.unity.com/packages/tools/physics/ms-vehicle-system-free-version-90214

Path creator used (after modification) from: https://assetstore.unity.com/packages/tools/utilities/b-zier-path-creator-136082

The setup:

At the start of the episode:

-Spawn at start position with some randomness and facing forwards with zero velocity and in gear 0


Observations:

-12 spherecasts equally spaced around the vehicle that detect barriers

-3 spherecasts equally spaced infront of the vehicle that detect waypoints

-Signed angle between car and waypoint foward vectors

-Velocity of car


Actions:

-Acceleration (continuous)

-Turning (continuous)


Rewards:

-Very small reward for forward velocity

-Large reward for crossing correct waypoint

-Very Large reward for finishing lap (larger reward for faster completion)

-Negative reward for hitting barrier

-Small negative reward for contacting barrier


Some images and videos of the project:

![alt text](https://github.com/Jonathon-A/AIDriver/blob/main/Images/CarRays.png)

![alt text](https://github.com/Jonathon-A/AIDriver/blob/main/Images/CarTraining.png)

https://user-images.githubusercontent.com/61558176/142739632-59fd4f77-1170-4887-b1fe-b6d3e0505da4.mp4

https://user-images.githubusercontent.com/61558176/145684394-e3849086-95b2-4b14-9c30-c8fa34aec573.mp4
