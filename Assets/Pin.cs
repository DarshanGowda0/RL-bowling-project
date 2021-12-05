using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pin : MonoBehaviour
{
    private Vector3 _originalPosition;
    private Quaternion _originalRotation;
    // Start is called before the first frame update

    public bool hasMoved = false;
    void Start()
    {
        _originalPosition = transform.position;
        _originalRotation = transform.rotation;
    }
    private bool HasMoved()
    {
        return transform.position != _originalPosition || transform.rotation != _originalRotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        hasMoved = HasMoved();
    }

    public void RestorePosition(){
        this.transform.position = _originalPosition;
        this.transform.rotation = _originalRotation;
    }

}
