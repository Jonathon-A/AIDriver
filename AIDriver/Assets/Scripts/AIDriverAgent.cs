using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;


public class AIDriverAgent : Agent
{
    

    public Vector2 StartPositionVariance;
    private Vector3 StartPosition;
    private Vector3 StartRotation;
    private Rigidbody RB;
    private bool Spawned = true;
    private bool NewSpawn = true;
    public override void OnEpisodeBegin()
    {

        RB.velocity = Vector3.zero;

        RB.angularVelocity = Vector3.zero;
        transform.rotation = Quaternion.Euler(StartRotation);
        // transform.position = new Vector3(StartPosition.x + Random.Range(-Mathf.Abs(StartPositionVariance.x), Mathf.Abs(StartPositionVariance.x)), 
        //      StartPosition.y,
        //     StartPosition.z + Random.Range(-Mathf.Abs(StartPositionVariance.y), Mathf.Abs(StartPositionVariance.y)));
        ControlsScript.ResetWheels();
       
        transform.position = new Vector3(StartPosition.x + (Random.Range(-2, 4) * StartPositionVariance.x),
            StartPosition.y,
            StartPosition.z + (Random.Range(-1, 2) * StartPositionVariance.y));

        
        // transform.position = new Vector3(StartPosition.x,
        //      StartPosition.y,
        //      StartPosition.z);


        Spawned = true;
        NewSpawn = true;

        LapNext = false;
        NextIndex = 0;
        StaySteps = 0;

        StillCount = 0;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        Vector3 WaypointForward = WaypointsList[NextIndex].transform.right;
        float DirectionAngle = Vector3.SignedAngle(transform.forward, WaypointForward, Vector3.up);
    //    Debug.Log(DirectionAngle);
        sensor.AddObservation(DirectionAngle);

        Vector3 localVelocity = transform.InverseTransformDirection(RB.velocity);
        float forwardSpeed = localVelocity.z;
        sensor.AddObservation(forwardSpeed);
       // Debug.Log(DirectionAngle);
    }

    public MSSceneControllerFree ControlsScript;

    public override void OnActionReceived(ActionBuffers actions)
    {
        //float Acceleration = actions.ContinuousActions[0];
        //float Turning = actions.ContinuousActions[1];

        float Acceleration = 0;

        switch(actions.DiscreteActions[0])
        {
            case (0):
                Acceleration = 0;
                break;
            case (1):
                Acceleration = 1;
                break;
            case (2):
                Acceleration = -1;
                break;
        }

        float Turning = actions.ContinuousActions[0];


        ControlsScript.SetControlInputs(Acceleration, Turning);
     

        Vector3 localVelocity = transform.InverseTransformDirection(RB.velocity);
        float forwardSpeed = localVelocity.z;

      //  Debug.Log(Acceleration + " " + Turning);

        AddReward(forwardSpeed * 0.0005f);

    }
   // public float InputThrottle;

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> ContinuousActions = actionsOut.ContinuousActions;
        //ContinuousActions[0] = Input.GetAxis("Vertical");
        //ContinuousActions[1] = Input.GetAxis("Horizontal");



        //     ContinuousActions[0] = InputThrottle;

        ContinuousActions[0] = Input.GetAxis("Horizontal");


        ActionSegment<int> DiscreteActions = actionsOut.DiscreteActions;

        switch (Input.GetAxisRaw("Vertical"))
        {
            case (0):
                DiscreteActions[0] = 0;
                break;
            case (1):
                DiscreteActions[0] = 1;
                break;
            case (-1):
                DiscreteActions[0] = 2;
                break;
        }

    }

    public Transform WaypointTransform;
    private List<Transform> WaypointsList;

    public Collider ThisCol;
    public Collider[] WheelCols;
    private void Start()
    {


        Spawned = false;
        NewSpawn = false;
        RB = gameObject.GetComponent<Rigidbody>();
        StartPosition = transform.position;
        StartRotation = transform.eulerAngles;

        WaypointsList = new List<Transform>();

        foreach (Transform Waypoint in WaypointTransform)
        {

            WaypointsList.Add(Waypoint);
        }
    }

    Vector3 LastPosition = Vector3.zero;
    int StillCount = 0;
    void FixedUpdate()
    {
        if (GameController.ColAllowed)
        {
            if (NewSpawn)
            {
                NewSpawn = false;

            }
            else
            {
                if (Spawned)
                {

                    Spawned = false;

                }
            }

            if (transform.position.y > 2f && GameController.ColAllowed)
            {
                transform.rotation = Quaternion.Euler(StartRotation);
                transform.position = new Vector3(StartPosition.x + (Random.Range(-2, 4) * 10),
             StartPosition.y,
             StartPosition.z + (Random.Range(-1, 2) * 5));
                Spawned = true;
                NewSpawn = true;
            }
        }

        if (transform.position == LastPosition)
        {
            StillCount++;
            if (StillCount >= 250) {
                AddReward(-5);
                EndEpisode();
            }
        }
        else {
            LastPosition = transform.position;
            StillCount = 0;
        }

        //Debug.Log(GetCumulativeReward());

    }

    private int NextIndex = 0;
    private bool LapNext = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Waypoint"))
        {
            if (NextIndex == WaypointsList.IndexOf(other.transform))
            {
                AddReward(10f);
                //  Debug.Log(10f);
                if (LapNext)
                {

                    LapNext = false;

                    AddReward(100f * MaxStep/ StepCount);

               //    Debug.Log("Final end " + GetCumulativeReward());
                    EndEpisode();
                }

                NextIndex++;
                if (NextIndex == WaypointsList.Count)
                {
                    NextIndex = 0;
                    LapNext = true;
                }
            }
            //else {
            //    AddReward(-10f);

            //    // Debug.Log(-10f);
            //}
        }




        if (Spawned && other.CompareTag("CarDetection") && GameController.ColAllowed)
        {

            AIDriverAgent Script = other.gameObject.GetComponent<AIDriverAgent>();


            foreach (Collider Col in Script.GetWheelCols())
            {
                foreach (Collider WheelCol in WheelCols)
                {
                    Physics.IgnoreCollision(WheelCol, Col, true);
                }

                Physics.IgnoreCollision(ThisCol, Col, true);
            }

            foreach (Collider WheelCol in WheelCols)
            {
                Physics.IgnoreCollision(WheelCol, Script.GetCol(), true);
            }

            Physics.IgnoreCollision(ThisCol, Script.GetCol(), true);

            
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("CarDetection") && GameController.ColAllowed)
        {
            AIDriverAgent Script = other.gameObject.GetComponent<AIDriverAgent>();
            // Debug.Log(other.name);

            foreach (Collider Col in Script.GetWheelCols())
            {
                foreach (Collider WheelCol in WheelCols)
                {
                    Physics.IgnoreCollision(WheelCol, Col, false);
                }

                Physics.IgnoreCollision(ThisCol, Col, false);
            }

            foreach (Collider WheelCol in WheelCols)
            {
                Physics.IgnoreCollision(WheelCol, Script.GetCol(), false);
            }

            Physics.IgnoreCollision(ThisCol, Script.GetCol(), false);
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Barrier"))
        {
            AddReward(-4f);
            StaySteps = 0;

        }

        if (collision.gameObject.CompareTag("Car") && GameController.ColAllowed)
        {
           
            AddReward(-1f);
        }
        //if (collision.gameObject.CompareTag("Floor"))
        //{
        //    SetReward(-10000);
        //    EndEpisode();
        //}

    }

    public Collider[] GetWheelCols() {

        return WheelCols;
    }

    public Collider GetCol()
    {

        return ThisCol;
    }

    private int StaySteps = 0;

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Barrier"))
        {
            AddReward(-0.01f);
            StaySteps++;
            if (StaySteps >= 250)
            {
                //Debug.Log("Final wall stick " + GetCumulativeReward());
                EndEpisode();
            }

        }
        if (collision.gameObject.CompareTag("Car") && GameController.ColAllowed)
        {

            AddReward(-0.001f);
        }
    }

}
