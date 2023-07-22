using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalGrid : GridBase
{
    public enum NormalGridLockState 
    {
        Locked,
        Unlocked,
    }


    //The material property block we pass to the GPU
    private MaterialPropertyBlock propertyBlock;
    public Color activateColor;
    public Color deactivateColor;
    private Renderer renderer;

    public NormalGridLockState gridState;

    // OnValidate is called in the editor after the component is edited
    void OnValidate()
    {

         renderer = GetComponentInChildren<Renderer>();

    }

    void Awake()
    {
        gridType = GridType.Normal;
    }
    // Start is called before the first frame update
    public void Start()
    {
        base.Start();
        //create propertyblock only if none exists
        if (propertyBlock == null)
            propertyBlock = new MaterialPropertyBlock();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        Debug.Log("NormalGrid OnTriggerEnter");
        if (other.tag == "Ball")
        {
            if (gridState == NormalGridLockState.Unlocked) return;
            //Debug.Log("Test!");
            UnlockThisGrid();
            GameManager.Instance.UpdateBrickNum(-1);
        }
        else if (other.CompareTag("Boss")) 
        {
            if (gridState == NormalGridLockState.Locked) return;
            Debug.Log("Boss locked this grid !!");
            LockThisGrid();
        }


    }

    public void UnlockThisGrid() 
    {
        gridState = NormalGridLockState.Unlocked;
        //Get a renderer component either of the own gameobject or of a child
        //set the color property
        propertyBlock.SetColor("_BaseColor", deactivateColor);
        //apply propertyBlock to renderer
        renderer.SetPropertyBlock(propertyBlock);
    }

    public void LockThisGrid()
    {
        gridState = NormalGridLockState.Locked;
        //Get a renderer component either of the own gameobject or of a child
        //set the color property
        propertyBlock.SetColor("_BaseColor", activateColor);
        //apply propertyBlock to renderer
        renderer.SetPropertyBlock(propertyBlock);
    }
}
