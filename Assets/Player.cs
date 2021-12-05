using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class Player : Agent
{
    public float spinX = 100f;
    public float spinY = 100f;
    public float spinZ = 100f;
    public float forceX = 20f;
    public float forceZ = 20f;
    // public float angularVelocity = 100f;

    private Vector3 oldPosition;
    private Quaternion oldRot;

    // x val -> 10 to 28
    // z val -> -11 to 11
    // y val -> same as initial y pos
    private float planeXmax = 28f;
    private float planeXmin = 10f;
    private float planeZmax = 11;
    private float planeZmin = -11f;

    Rigidbody rBody;

    [HideInInspector] public Pin[] _pins;

    void Start()
    {
        rBody = GetComponent<Rigidbody>();
        // rBody.maxAngularVelocity = angularVelocity;
        oldPosition = transform.position;
        oldRot = transform.rotation;
        storeGameObjects();
    }


    void storeGameObjects()
    {
        if ((_pins = FindObjectsOfType<Pin>()) == null || _pins.Length < 1)
        {
            Debug.LogError("No pin founded.");
        }
        else
        {
            Debug.Log("Found " + _pins.Length + "pins!");
        }
    }

    void FixedUpdate()
    {
        // if (Input.GetKeyDown("space"))
        // {
        //     rBody.AddForce(new Vector3(forceX, 0f, forceZ), ForceMode.VelocityChange);
        //     rBody.AddTorque(new Vector3(spinX, spinY, spinZ), ForceMode.Force);
        // }
    }

    void Update()
    {
        // TODO: remove this
        // float xforce = Input.GetAxis("Horizontal");
        // float zforce = Input.GetAxis("Vertical");
        // rBody.AddForce(new Vector3(xforce, 0f, zforce), ForceMode.VelocityChange);
    }

    private void OnTriggerEnter(Collider other)
    {
        // check pins are standing, if any have fallen set reward and end episode
        int numberOfPinsKnocked = 0;
        foreach(Pin pin in _pins){
            if (pin.hasMoved)
            {
                numberOfPinsKnocked += 1;
            }
        }
        Debug.Log("Number of pins knocked -> "+numberOfPinsKnocked);
        if (numberOfPinsKnocked > 0){
            SetReward(numberOfPinsKnocked * 1.0f );
            EndEpisode();
        }
    }

    public override void OnEpisodeBegin()
    {
        rBody.angularVelocity = Vector3.zero;
        rBody.velocity = Vector3.zero;

        this.transform.localPosition = new Vector3(
                UnityEngine.Random.Range(planeXmin, planeXmax),
                oldPosition.y,
                UnityEngine.Random.Range(planeZmin, planeZmax)
            );

        foreach(Pin pin in _pins)
        {
            pin.RestorePosition();
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // add balls position - 3 params
        sensor.AddObservation(this.transform.localPosition);

        // add velocity - 3 params
        sensor.AddObservation(rBody.velocity);

        // add angular velocity - 3 params
        sensor.AddObservation(rBody.angularVelocity);

    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // check if the x, x lie within the initial plane, then take action and roll the ball
        if (isBallInPlane())
        {
            // collect the action value
            float _forceX = actionBuffers.ContinuousActions[0];
            float _forceZ = actionBuffers.ContinuousActions[1];
            float _spinX = actionBuffers.ContinuousActions[2];
            float _spinY = actionBuffers.ContinuousActions[3];
            float _spinZ = actionBuffers.ContinuousActions[4];

            // perform the action
            rBody.AddForce(new Vector3(_forceX, 0f, _forceZ), ForceMode.VelocityChange);
            rBody.AddTorque(new Vector3(_spinX, _spinY, _spinZ), ForceMode.Force);
        }
    }


    public bool isBallInPlane()
    {
        // check if the ball is off the platform
        if (this.transform.localPosition.y < 0)
        {
            EndEpisode();
            return false;
        }
        // TODO: check ball is within the range
        if (this.transform.localPosition.x < planeXmin || this.transform.localPosition.x > planeXmax || this.transform.localPosition.z < planeZmin || this.transform.localPosition.z > planeZmax)
        {
            return false;
        }
        return true;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }


}
