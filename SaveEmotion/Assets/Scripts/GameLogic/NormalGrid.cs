using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalGrid : GridBase
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Ball")
        {
            //Debug.Log("Test!");
            this.gameObject.SetActive(false);
            GameManager.Instance.UpdateBrickNum(-1);
        }

    }
}
