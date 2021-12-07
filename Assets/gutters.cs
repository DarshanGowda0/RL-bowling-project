using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gutters : MonoBehaviour
{
    public float force = 20f;
    private Collider _collider;
    void OnValidate()
    {
        _collider = GetComponent<Collider>();
    }


    private void Awake()
    {
        _collider.isTrigger = true;
    }
    // Update is called once per frame
    private void OnTriggerStay(Collider other)
    {
        if (other == this) return;
        Player ball = other.gameObject.GetComponent<Player>();
        if (ball == null) return;
        ball.rBody.AddForce(_collider.transform.up.normalized * ball.rBody.mass * force);
    }
}
