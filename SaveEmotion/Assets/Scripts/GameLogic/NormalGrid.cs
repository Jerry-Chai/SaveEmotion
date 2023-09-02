using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class NormalGrid : GridBase
{
    public enum NormalGridLockState 
    {
        Locked,
        Unlocked,
    }


    //The material property block we pass to the GPU

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
        gridType = GridType.NormalGrid;
    }
    // Start is called before the first frame update
    public void Start()
    {
        base.Start();
        //create propertyblock only if none exists
        renderer = GetComponentInChildren<MeshRenderer>();
        if (propertyBlock == null)
        {
            propertyBlock = new MaterialPropertyBlock();
           
        }
        // //apply propertyBlock to renderer
        propertyBlock.SetColor("_BaseColor", baseColor);
        renderer.SetPropertyBlock(propertyBlock);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        if (other.tag == "Ball")
        {
            Debug.Log("Ball Hit this block");
            if (gridState == NormalGridLockState.Unlocked) return;
            //Debug.Log("Test!");
            StopCoroutine("Dissolve");
            UnlockThisGrid();
            //GameManager.Instance.UpdateBrickNum(-1);
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
        Vector2 ballDir = GameManager.Instance.ballScript.ballDir;
        //StopCoroutine("Dissolve");
        //StartCoroutine(Dissolve(0, 200, 2, ballDir));
        AsyncDissolve(0, 200, 2, ballDir);
        //propertyBlock.SetColor("_BaseColor", deactivateColor);
        //apply propertyBlock to renderer
        //renderer.SetPropertyBlock(propertyBlock);
    }
    

    /// <summary>
    /// 假设shader中的数值为200， 这边直接做一个硬算
    /// </summary>
    /// <param name="fromValue"></param>
    /// <param name="toValue"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    public IEnumerator Dissolve(float fromValue, float toValue, float time, Vector2 objDir)
    {
        float change = toValue - fromValue;
        change /= time;
        float dissove = fromValue;
        
        propertyBlock.SetVector("_DissolveDirection", objDir);
        while (time >= 0.0f)
        {
            time -= Time.deltaTime;
            dissove += Time.deltaTime * change;
            propertyBlock.SetFloat("_ControlValue", dissove);
            renderer.SetPropertyBlock(propertyBlock);
            yield return null;
        }
    }
    
    async void AsyncDissolve(float fromValue, float toValue, float time, Vector2 objDir)
    {
//        Debug.Log("Async Task Started");
        float change = toValue - fromValue;
        change /= time;
        float dissove = fromValue;
        
        propertyBlock.SetVector("_DissolveDirection", objDir);
        while (time >= 0.0f)
        {
//            Debug.Log("Async Task Running");
            time -= Time.deltaTime;
            dissove += Time.deltaTime * change;
            propertyBlock.SetFloat("_ControlValue", dissove);
            renderer.SetPropertyBlock(propertyBlock);
            await Task.Yield();
        }
        // This task will finish, even though it's object is destroyed
  //      Debug.Log("Async Task Ended");
        //GridManager.
        //lock
        if (fromValue > toValue)
        {
            GridManager.Instance.LockNormalGrid(this.gameObject.GetInstanceID(), this);
        }
        else 
        {
            GridManager.Instance.UnlockNormalGrid(this.gameObject.GetInstanceID(), this);

        }
    }

    public void LockThisGrid()
    {
        gridState = NormalGridLockState.Locked;
        //Get a renderer component either of the own gameobject or of a child
        //set the color property
        //StopCoroutine("Dissolve");
        Vector2 snallDir = GameManager.Instance.snallBehaviour.moveDir;
        //StartCoroutine(Dissolve(200, 0 , 2, snallDir));
        AsyncDissolve(200, -15 , 2, snallDir);
        //apply propertyBlock to renderer
    }

    public void LockThisGridBySnallSkill(Vector2 dir)
    {
        gridState = NormalGridLockState.Locked;
        //Get a renderer component either of the own gameobject or of a child
        //set the color property
        //StopCoroutine("Dissolve");
        //StartCoroutine(Dissolve(200, 0 , 2, snallDir));
        AsyncDissolve(200, -15, 2, dir);
        //apply propertyBlock to renderer
    }
}
