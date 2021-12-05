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
    // [SerializeField] float m_Thrust = 20f;
    // Start is called before the first frame update
    void Start()
    {
        rBody = GetComponent<Rigidbody>();
        // rBody.maxAngularVelocity = angularVelocity;
        oldPosition = transform.position;
        oldRot = transform.rotation;
    }

    void FixedUpdate()
    {
        if (Input.GetKeyDown("space"))
        {
            rBody.AddForce(new Vector3(forceX, 0f, forceZ), ForceMode.VelocityChange);
            rBody.AddTorque(new Vector3(spinX, spinY, spinZ), ForceMode.Force);
        }
    }

    void Update()
    {

        // check pins are standing, if any have fallen set reward and end episode
        
    }

    public override void OnEpisodeBegin()
    {
        //  reset position to random position
        if (transform.localPosition.y < 0)
        {

        }

        rBody.angularVelocity = Vector3.zero;
        rBody.velocity = Vector3.zero;

        this.transform.localPosition = new Vector3(
                UnityEngine.Random.Range(planeXmin, planeXmax),
                oldPosition.y,
                UnityEngine.Random.Range(planeZmin, planeZmax)
            );
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // add balls position - 3 params
        sensor.AddObersvation(this.transform.localPosition);

        // add velocity - 3 params
        sensor.AddObservation(rBody.velocity);

        // add angular velocity - 3 params
        sensor.AddObservation(rBody.angularVelocity);

    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // check if the ball is off the platform
        if (this.transform.localPosition.y < 0)
        {
            EndEpisode();
        }

        // check if the x, x lie within the initial plane, then take action and roll the ball
        if (isBallInPlane())
        {
            // collect the action value
            float _forceX = actionBuffers.ContinousActions[0];
            float _forceZ = actionBuffers.ContinousActions[1];
            float _spinX = actionBuffers.ContinousActions[2];
            float _spinY = actionBuffers.ContinousActions[3];
            float _spinZ = actionBuffers.ContinousActions[4];

            // perform the action
            rBody.AddForce(new Vector3(_forceX, 0f, _forceZ), ForceMode.VelocityChange);
            rBody.AddTorque(new Vector3(_spinX, _spinY, _spinZ), ForceMode.Force);
        }
    }


    public bool isBallInPlane()
    {
        // TODO: check ball is within the range
        return true;
    }


}
