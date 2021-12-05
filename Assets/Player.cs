using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class Player : Agent
{
    Rigidbody rBody;
    [SerializeField] float m_Thrust = 20f;
    // Start is called before the first frame update
    void Start()
    {
        rBody = GetComponent<Rigidbody>();
    }

    // calling update

    void Update()
    {
        if(Input.GetKeyDown("space"))
        {
            rBody.AddForce(-m_Thrust, 0, 0, ForceMode.Impulse);
        }
    }

}
