using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BricksSetter : MonoBehaviour
{
    public GameObject bricksprefab;

    public GameObject upperLeft;
    public GameObject lowerRight;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartTile()
    {
        Transform[] allChildren = GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            if (child.gameObject.name == gameObject.name) continue;
            DestroyImmediate(child.gameObject);
        }


        Bounds bricksBounds = bricksprefab.GetComponent<MeshRenderer>().bounds;
        Debug.Log(bricksBounds.size);
        float width = Mathf.Abs(lowerRight.transform.position.x - upperLeft.transform.position.x);
        float height = Mathf.Abs(lowerRight.transform.position.z - upperLeft.transform.position.z);
        Debug.Log(width);
        Debug.Log(height);
        
        int xNum = Mathf.FloorToInt(width / bricksBounds.size.x);
        int yNum = Mathf.FloorToInt(height / bricksBounds.size.z);
        Debug.Log(xNum);
        Debug.Log(yNum);
        for (int i = 0; i < xNum; i++) 
        {
            for (int j = 0; j < yNum; j++)
            {
                GameObject newBricks = Instantiate(bricksprefab);
                newBricks.transform.parent = transform;
                newBricks.transform.position = new Vector3(upperLeft.transform.position.x + bricksBounds.size.x * i, this.transform.position.y, upperLeft.transform.position.z - bricksBounds.size.z * j);
            }
        }
    }
}


[CustomEditor(typeof(BricksSetter))]
public class BricksSetterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Start Tiled"))
        {
            BricksSetter script = target as BricksSetter;
            script.StartTile();
        }
    }
}
