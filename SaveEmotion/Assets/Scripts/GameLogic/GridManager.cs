using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;

public class GridManager : Singleton<GridManager>
{
    private GameObject bricksprefab;
    private GameObject upperLeft;
    private GameObject lowerRight;
    private Bounds bricksBounds;

    /// <summary>
    /// 用来记录初始化的格子数据
    /// </summary>
    public Dictionary<int, GridBase> gridDic = new Dictionary<int, GridBase>();
    public Dictionary<int, GridBase> LockedNormalGridDic = new Dictionary<int, GridBase>();
    public Dictionary<int, GridBase> UnlockedNormalGridDic = new Dictionary<int, GridBase>();
    public List<int> gridIDList;

    // Start is called before the first frame update

    void Start()
    {
        

//         var bricksInfo = GetComponent<BricksSetter>();
// //        bricksprefab = bricksInfo.bricksprefab;
//         // upperLeft = bricksInfo.upperLeft;
//         // lowerRight = bricksInfo.lowerRight;
//         // calculate Bounds:
//        // bricksBounds = bricksprefab.GetComponent<MeshRenderer>().bounds;
//         Debug.Log(bricksBounds.size);
//         float width = Mathf.Abs(lowerRight.transform.position.x - upperLeft.transform.position.x);
//         float height = Mathf.Abs(lowerRight.transform.position.z - upperLeft.transform.position.z);
//         Debug.Log(width);
//         Debug.Log(height);
//
//         int xNum = Mathf.FloorToInt(width / bricksBounds.size.x);
//         int yNum = Mathf.FloorToInt(height / bricksBounds.size.z);
//         Debug.Log(xNum);
//         Debug.Log(yNum);
//         //for (int i = 0; i < xNum; i++)
//         //{
//         //    for (int j = 0; j < yNum; j++)
//         //    {
//         //        GameObject newBricks = Instantiate(bricksprefab);
//         //        newBricks.transform.parent = transform;
//         //        newBricks.transform.position = new Vector3(upperLeft.transform.position.x + bricksBounds.size.x * i, this.transform.position.y, upperLeft.transform.position.z - bricksBounds.size.z * j);
//         //    }
//         //}

    }

    /// <summary>
    /// 释放技能的时候调用
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="_width"></param>
    /// <param name="_height"></param>
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

    public void RegisteGrid(int instanceID, GridBase script)
    {
        //if (gridDic == null) 
        //{
        //    gridDic = new Dictionary<int, GridBase>();
        //    gridIDList = new List<int>();
        //}
        //if (gridDic.ContainsKey(instanceID))
        //{
        //    Debug.LogError("We should not have two same instanceID, something is wrong here");
        //}
        //else 
        //{
        //    gridIDList.Add(instanceID);
        //    gridDic[instanceID] = script;
        //    if (script.gridType == GridBase.GridType.NormalGrid) 
        //    {
        //        var normalGrid = script as NormalGrid;
        //        if (normalGrid.gridState == NormalGrid.NormalGridLockState.Locked)
        //        {
        //            LockedNormalGridDic[instanceID] = script;
        //        }
        //        else
        //        {
        //            UnlockedNormalGridDic[instanceID] = script;
        //        }
        //    }

        //}

    }

    /// <summary>
    /// 这个函数用来处理，snall 技能触发的格子改变，触发了之后，看有多少个格子，这些格子依次变色。
    /// 中间应该还会需要加入一些特效， 先用gameobecet来处理
    /// </summary>
    /// <param name=""></param>
    /// <param name=""></param>
    /// <param name=""></param>
    public void LockGridBySnallSkill(int revertBlocksNum, Vector3 snallPos)
    {
        int count = revertBlocksNum;
        while (count > 0) 
        {
            
            count--;
        
        }
    }

    /// <summary>
    /// 这个接口主要是给蜗牛使用的， 告诉蜗牛一个可以行进的地方，这个地方是所有格子里面随机的一个。
    /// </summary>
    public Vector3 GetRandomGrid()
    {
        int nextIndex = UnityEngine.Random.Range(0, gridIDList.Count);
        return gridDic[gridIDList[nextIndex]].gameObject.transform.position;
    }

    public NormalGrid GetRandomLockedGrid()
    {
        if (LockedNormalGridDic.Count <= 0) 
        {
            Debug.Log("No LockedNormalGridDic here...");
            return null;
        }
        int nextIndex = UnityEngine.Random.Range(0, LockedNormalGridDic.Count);
        var script = LockedNormalGridDic.ElementAt(nextIndex).Value as NormalGrid;
        LockedNormalGridDic.Remove(LockedNormalGridDic.ElementAt(nextIndex).Key);
        return script;
    }

    public NormalGrid GetRandomUnlockedGrid()
    {
        if (UnlockedNormalGridDic.Count <= 0)
        {
            Debug.Log("No UnlockedNormalGridDic here...");
            return null;
        }
        int nextIndex = UnityEngine.Random.Range(0, UnlockedNormalGridDic.Count);
        var script = UnlockedNormalGridDic.ElementAt(nextIndex).Value as NormalGrid;
        UnlockedNormalGridDic.Remove(UnlockedNormalGridDic.ElementAt(nextIndex).Key);
        return script;
    }

    public void LockNormalGrid(int instanceID, GridBase script)
    {
        if (UnlockedNormalGridDic.ContainsKey(instanceID))
        {
            UnlockedNormalGridDic.Remove(instanceID);
        }

        if (!LockedNormalGridDic.ContainsKey(instanceID))
        {
            LockedNormalGridDic[instanceID] = script;
        }
    }

    public void UnlockNormalGrid(int instanceID, GridBase script)
    {
        if (LockedNormalGridDic.ContainsKey(instanceID))
        {
            LockedNormalGridDic.Remove(instanceID);
        }        
        
        if (!UnlockedNormalGridDic.ContainsKey(instanceID))
        {
            UnlockedNormalGridDic[instanceID] = script;
        }
    }


#if UNITY_EDITOR
    public void TestRegisterWork() 
    {
        Debug.Log(gridDic.Count);
    }

    public void RandomizeGrid() 
    {
        foreach (var item in gridDic)
        {
            GridBase fatherScript = item.Value;
            if (fatherScript.gridType == GridBase.GridType.NormalGrid) 
            {
                NormalGrid childScript = fatherScript as NormalGrid;
                float randomValue = UnityEngine.Random.Range(0, 1f);
                //if (randomValue > 0.5f)
                //{
                //    childScript.UnlockThisGrid();
                //}
                //else 
                //{
                //    childScript.LockThisGrid();
                //}
            }
        }
    }

#endif

}


#if UNITY_EDITOR
[CustomEditor(typeof(GridManager))]
public class GridManagerEditor : Editor 
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GridManager script = target as GridManager;
        if (GUILayout.Button("Test Grid Dic data")) 
        {
            script.TestRegisterWork();
        }

        if (GUILayout.Button("Randomize Grid"))
        {
            script.RandomizeGrid();
        }

    }
}
#endif