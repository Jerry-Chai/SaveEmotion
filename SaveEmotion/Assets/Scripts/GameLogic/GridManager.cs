using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : Singleton<GridManager>
{
    private GameObject bricksprefab;
    private GameObject upperLeft;
    private GameObject lowerRight;
    private Bounds bricksBounds;
    // Start is called before the first frame update

    void Start()
    {


        var bricksInfo = GetComponent<BricksSetter>();
        bricksprefab = bricksInfo.bricksprefab;
        upperLeft = bricksInfo.upperLeft;
        lowerRight = bricksInfo.lowerRight;
        // calculate Bounds:
        bricksBounds = bricksprefab.GetComponent<MeshRenderer>().bounds;
        Debug.Log(bricksBounds.size);
        float width = Mathf.Abs(lowerRight.transform.position.x - upperLeft.transform.position.x);
        float height = Mathf.Abs(lowerRight.transform.position.z - upperLeft.transform.position.z);
        Debug.Log(width);
        Debug.Log(height);

        int xNum = Mathf.FloorToInt(width / bricksBounds.size.x);
        int yNum = Mathf.FloorToInt(height / bricksBounds.size.z);
        Debug.Log(xNum);
        Debug.Log(yNum);
        //for (int i = 0; i < xNum; i++)
        //{
        //    for (int j = 0; j < yNum; j++)
        //    {
        //        GameObject newBricks = Instantiate(bricksprefab);
        //        newBricks.transform.parent = transform;
        //        newBricks.transform.position = new Vector3(upperLeft.transform.position.x + bricksBounds.size.x * i, this.transform.position.y, upperLeft.transform.position.z - bricksBounds.size.z * j);
        //    }
        //}

    }

    public void TriggerSkill(Vector3 pos, int _width, int _height) 
    {
        int posWidth = Mathf.CeilToInt(Mathf.Abs(pos.x - upperLeft.transform.position.x)/bricksBounds.size.x);
        int posHeight = Mathf.CeilToInt(Mathf.Abs(pos.z - upperLeft.transform.position.z)/bricksBounds.size.z);


        float width = Mathf.Abs(lowerRight.transform.position.x - upperLeft.transform.position.x);
        float height = Mathf.Abs(lowerRight.transform.position.z - upperLeft.transform.position.z);
        Debug.Log(width);
        Debug.Log(height);

        int xNum = Mathf.FloorToInt(width / bricksBounds.size.x);
        int yNum = Mathf.FloorToInt(height / bricksBounds.size.z);
        Debug.Log(xNum);
        Debug.Log(yNum);

        int index = yNum * (posWidth - 1) + posHeight;
        Debug.Log(posHeight);
        Debug.Log(posWidth);
        Debug.Log(index);
        if (index > xNum * yNum) return;
        for (int i = -Mathf.Min(_height, posHeight); i <= Mathf.Min(_height, yNum - posHeight); i++) 
        {
            for (int j = -_width; j <= _width; j++)
            {
                int currIndex = index + i + j * yNum;
                int gridX = currIndex / yNum;
                var Go = this.transform.GetChild(index + i + j * yNum).gameObject;
                if (Go.activeSelf) 
                {
                    GameManager.Instance.UpdateBrickNum(-1);
                    Go.gameObject.SetActive(false);
                }

            }
        }
      
    }

}
