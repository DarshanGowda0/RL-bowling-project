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
    private int numberOfPinsKnocked = 0;
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
            Debug.LogError("No pins found.");
        }
    }

    void checkForGameEnd()
    {
        foreach (Pin pin in _pins)
        {
            if (pin.HasMoved())
            {
                numberOfPinsKnocked += 1;
            }
        }
        arePinsDown();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "pins")
        {
            Invoke("checkForGameEnd", 2);
        }
    }

    public override void OnEpisodeBegin()
    {
        Debug.Log("On Episode begin: resetting!");
        rBody.angularVelocity = Vector3.zero;
        rBody.velocity = Vector3.zero;

        this.transform.localPosition = new Vector3(
                UnityEngine.Random.Range(planeXmin, planeXmax),
                oldPosition.y,
                UnityEngine.Random.Range(planeZmin, planeZmax)
            );
        // this.transform.localPosition = oldPosition;
        numberOfPinsKnocked = 0;
        foreach (Pin pin in _pins)
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

    void arePinsDown()
    {
        if (numberOfPinsKnocked > 0)
        {
            Debug.Log("Number of pins knocked -> " + numberOfPinsKnocked);
            SetReward(numberOfPinsKnocked * 1.0f);
            EndEpisode();
        }
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

            // Debug.Log("Applying force " + forceX + ", " + forceZ);
            // perform the action
            rBody.AddForce(new Vector3(_forceX, 0f, _forceZ), ForceMode.VelocityChange);
            rBody.AddTorque(new Vector3(_spinX, _spinY, _spinZ), ForceMode.Force);

        }
    }


    public bool isBallInPlane()
    {
        // check if the ball is off the platform
        if (this.transform.localPosition.y < -50)
        {
            Debug.Log("Ball y below 5, end episode!");
            SetReward(-5.0f);
            EndEpisode();
            return false;
        }
        return true;
        // TODO: check ball is within the range
        if (this.transform.localPosition.x < planeXmin || this.transform.localPosition.x > planeXmax || this.transform.localPosition.z < planeZmin || this.transform.localPosition.z > planeZmax)
        {
            Debug.Log("Ball not in plane!");
            return false;
        }
        return true;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // Debug.Log("Calling heuristic!");
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }


}
