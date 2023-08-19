using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockedGrid : GridBase
{
    public enum LockedState 
    {
        Locked,
        Unlocked
    }

    //The color of the object
    public Color MaterialColor;

    //The material property block we pass to the GPU
    private MaterialPropertyBlock propertyBlock;
    public LockedState lockedState = LockedState.Locked;
    public BoxCollider boxCollider;

    void Awake()
    {
        gridType = GridType.IcedGrid;
    }

    // Start is called before the first frame update
    void Start()
    {
        MaterialColor = Color.white;
        lockedState = LockedState.Locked;
        boxCollider = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("Test!");
        //this.gameObject.SetActive(false);
        //GameManager.Instance.UpdateBrickNum(-1);

        if (collision.body.tag == "Ball")
        {

            //Debug.Log("Test!");
            //this.gameObject.SetActive(false);
            //GameManager.Instance.UpdateBrickNum(-1);
            OnChangeColor();
            //boxCollider.isTrigger = true;
            lockedState = LockedState.Unlocked;

            StartCoroutine(DelaySetTrigger());
        }
    }

    public override void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Ball")
        {

            //Debug.Log("Test!");
            //GameManager.Instance.UpdateBrickNum(-1);
            //OnChangeColor();
            Debug.Log("Go through");
            this.gameObject.SetActive(false);

        }

    }

    // OnValidate is called in the editor after the component is edited
    void OnChangeColor()
    {
        //create propertyblock only if none exists
        if (propertyBlock == null)
            propertyBlock = new MaterialPropertyBlock();
        //Get a renderer component either of the own gameobject or of a child
        Renderer renderer = GetComponentInChildren<Renderer>();
        //set the color property
        propertyBlock.SetColor("_BaseColor", MaterialColor);
        //apply propertyBlock to renderer
        renderer.SetPropertyBlock(propertyBlock);
    }

    IEnumerator DelaySetTrigger() 
    {
        yield return new WaitForSeconds(0.1f);
        boxCollider.isTrigger = true;
    
    }
}
