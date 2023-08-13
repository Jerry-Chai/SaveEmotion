using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundValueSetter : MonoBehaviour
{
    // Start is called before the first frame update
    public MaterialPropertyBlock propertyBlock;
    void Start()
    {
        //create propertyblock only if none exists
        if (propertyBlock == null)
            propertyBlock = new MaterialPropertyBlock();
        //Get a renderer component either of the own gameobject or of a child
        Renderer renderer = GetComponentInChildren<Renderer>();
        //set the color property
        propertyBlock.SetVector("_WorldPos", this.transform.position);
        //apply propertyBlock to renderer
        renderer.SetPropertyBlock(propertyBlock);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
