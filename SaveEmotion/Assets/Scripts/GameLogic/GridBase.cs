using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBase : MonoBehaviour
{
    /// <summary>
    /// gridType表示当前格子的类型
    /// </summary>
    public enum GridType
    {
        Normal,
        Locked,
        Unlocked,
    }

    public GridType gridType;
    // Start is called before the first frame update
    public void Start()
    {
        GridManager.Instance.RegisteGrid(this.gameObject.GetInstanceID(), this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void OnTriggerEnter(Collider other)
    {

    }
}
