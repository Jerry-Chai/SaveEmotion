using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackRootRotate : MonoBehaviour
{

    public float rootSpeed = 5;

    private float m_RotateDegree;
    // Start is called before the first frame update
    void Start()
    {
        this.transform.rotation = Quaternion.Euler(0, 0, 0);
        m_RotateDegree = 0;
    }

    // Update is called once per frame
    void Update()
    {
        m_RotateDegree += rootSpeed * Time.deltaTime;
        m_RotateDegree = m_RotateDegree > 360 ? m_RotateDegree - 360 : m_RotateDegree;
        this.transform.rotation = Quaternion.Euler(0, m_RotateDegree, 0);
    }
}
