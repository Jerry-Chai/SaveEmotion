using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundValueSetter : MonoBehaviour
{
    // Start is called before the first frame update
    public MaterialPropertyBlock propertyBlock;
    public float _MoveRange = 1.0f;
    public float _TimeScale = 0.3f;
    void Start()
    {
        //create propertyblock only if none exists
        if (propertyBlock == null)
            propertyBlock = new MaterialPropertyBlock();
        //Get a renderer component either of the own gameobject or of a child
        Renderer renderer = GetComponentInChildren<Renderer>();
        //set the color property
        var position = this.transform.position;
        position.x += Random.Range(-1000.0f , 1000.0f);
        propertyBlock.SetVector("_WorldPos", position);
        propertyBlock.SetFloat("_MoveRange", _MoveRange);
        propertyBlock.SetFloat("_TimeScale", _TimeScale);
        //apply propertyBlock to renderer
        renderer.SetPropertyBlock(propertyBlock);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetColor(Color color) 
    {
        if (propertyBlock == null)
            propertyBlock = new MaterialPropertyBlock();
        Renderer renderer = GetComponentInChildren<Renderer>();
        propertyBlock.SetColor("_BaseColor", color);
        renderer.SetPropertyBlock(propertyBlock);

    }
}
