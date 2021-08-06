# AIDriver
Using reinforcement and imitation learning to make an agent drive a car with somewhat realistic physics in Unity

Car controller used (after modification) from: assetstore.unity.com/packages/tools/physics/ms-vehicle-system-free-version-90214

Path creator used (after modification) from: https://assetstore.unity.com/packages/tools/utilities/b-zier-path-creator-136082

The setup:

At the start of the episode:

-Spawn at start position with some randomness and facing forwards


Observations:

-12 spherecasts equally spaced around the vehicle


Actions:

-Acceleration (continuous)

-Turning (continuous)


Rewards:

-Very small reward for forward velocity

-Large reward for crossing xorrect waypoint

-Very Large reward for finishing lap (larger reward for faster completion)

-Negative reward for hitting barrier

-Small negative reward for contacting barrier


Some images of the project:

![alt text](https://github.com/Jonathon-A/AIDriver/blob/main/Images/CarRays.png)

![alt text](https://github.com/Jonathon-A/AIDriver/blob/main/Images/CarTraining.png)
