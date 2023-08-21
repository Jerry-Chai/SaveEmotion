using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundCubeCpuMovement : MonoBehaviour
{
    // Start is called before the first frame update
    public float _TimeScale = 5.0f;
    public float _MoveRange = 3.0f;
    public float originalPosY;
    public float originalPosX;
    public float originalPosZ;
    void Start()
    {
        originalPosY = transform.position.y;
        originalPosX = transform.position.x;
        originalPosZ = transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(Time.time);
        Vector3 position = transform.position;
        position.y = originalPosY + (Mathf.Sin((Time.time / 20.0f + originalPosX + originalPosZ) * _TimeScale) * _MoveRange);
        transform.position = position;
    }
}
