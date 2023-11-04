using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBase : MonoBehaviour
{
    public MaterialPropertyBlock propertyBlock;
    public Color baseColor;
    /// <summary>
    /// gridType表示当前格子的类型
    /// </summary>
    /// 这个地方应该有问题。。。 NormalGrid指的是普通可以解锁的格子， 并且它初始化的时候有解锁和没解锁两种，
    public enum GridType
    {
        NormalGrid,
        IcedGrid,
        Unlocked,
    }

    public GridType gridType;
    // Start is called before the first frame update
    public void Start()
    {
        string scenename = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        if (scenename == "IntroScene0000") return;
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
