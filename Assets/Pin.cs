using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pin : MonoBehaviour
{
    private Vector3 _originalPosition;
    private Quaternion _originalRotation;
    Rigidbody rBody;

    private float _min_dist = 0.01f;
    public bool hasMoved = false;
    void Start()
    {
        _originalPosition = transform.position;
        _originalRotation = transform.rotation;
        rBody = GetComponent<Rigidbody>();
    }
    public bool HasMoved()
    {
        return Vector3.Distance(transform.position, _originalPosition) > _min_dist;
    }

    private void OnTriggerEnter(Collider other)
    {
        hasMoved = HasMoved();
    }

    public void RestorePosition()
    {
        rBody.angularVelocity = Vector3.zero;
        rBody.velocity = Vector3.zero;
        this.transform.position = _originalPosition;
        this.transform.rotation = _originalRotation;
    }

}
