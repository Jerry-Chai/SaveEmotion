using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiquidShake : MonoBehaviour
{
    Vector3 lastPos;
    Vector3 velocity;
    Vector3 angularVelocity;
    Quaternion lastRot;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        velocity = (transform.position - lastPos) / Time.deltaTime;
        angularVelocity
        lastPos = transform.position;
    }
}
