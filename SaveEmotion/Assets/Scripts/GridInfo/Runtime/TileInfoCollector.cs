using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class TileInfoCollector : MonoBehaviour
{

    public GameObject rootGO;
    
    public List<GameObject> prefabList;
    public Texture2D backgroundImage;
    public Dictionary<string, GameObject> prefabDic;

    private int bgTex_Height;
    private int bgTex_Width;
    // Start is called before the first frame update
    void Start()
    {

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CollectTileInfo()
    {
        foreach (var VARIABLE in rootGO.GetComponentsInChildren<Transform>(true))
        {
            if (VARIABLE.gameObject.name == rootGO.name) continue;
            DestroyImmediate(VARIABLE.gameObject);        
        }
        
        // 在这个地方生成一个字典吧
        prefabDic = new Dictionary<string, GameObject>();
        foreach (var VARIABLE in prefabList)
        {
            prefabDic[VARIABLE.name] = VARIABLE;
        }
        Debug.Log("CollectTileInfo");

        Grid currGrid = GetComponent<Grid>();
        float gridSize = currGrid.cellSize.x;

        foreach (var VARIABLE in GetComponentsInChildren<Transform>(true))
        {
            Tilemap currTilemap = VARIABLE.gameObject.GetComponent<Tilemap>();
            if (!currTilemap) continue;
            Debug.Log(currTilemap.name);

            Vector3 upperLeftBound = new Vector3(int.MaxValue, int.MinValue, 0.0f);
            Vector3 lowerRightBound = new Vector3(int.MinValue, int.MaxValue, 0.0f);
            // foreach (var pos in currTilemap.cellBounds.allPositionsWithin)
            // { 
            //     
            //     Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);
            //     upperLeftBound.x = upperLeftBound.x > localPlace.x ? localPlace.x : upperLeftBound.x;
            //     upperLeftBound.y = upperLeftBound.y < localPlace.y ? localPlace.y : upperLeftBound.y;
            //
            //     lowerRightBound.x = lowerRightBound.x < localPlace.x ? localPlace.x : lowerRightBound.x;
            //     lowerRightBound.y = lowerRightBound.y > localPlace.y ? localPlace.y : lowerRightBound.y;
            //
            // }


            List<GameObject> objList = new List<GameObject>();
            foreach (var pos in currTilemap.cellBounds.allPositionsWithin)
            {
                Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);
                if (currTilemap.HasTile(localPlace))
                {

                    var name = currTilemap.GetTile(localPlace).name;
                    //Debug.Log(localPlace + "," + name);
                    //tileWorldLocations.Add(place);
                    objList.Add(GeneratePrefab(localPlace, name, gridSize));
                }
            }

            foreach (GameObject obj in objList)
            {
                var pos = obj.transform.position;
//                Debug.Log(pos);
                Vector3 localPlace = new Vector3(pos.x, pos.y, pos.z);
                upperLeftBound.x = upperLeftBound.x > localPlace.x ? localPlace.x : upperLeftBound.x;
                upperLeftBound.y = upperLeftBound.y < localPlace.z ? localPlace.z : upperLeftBound.y;
                
                lowerRightBound.x = lowerRightBound.x < localPlace.x ? localPlace.x : lowerRightBound.x;
                lowerRightBound.y = lowerRightBound.y > localPlace.z ? localPlace.z : lowerRightBound.y;
            }   
            Debug.Log(upperLeftBound);
            Debug.Log(lowerRightBound);
            
            foreach (GameObject obj in objList)
            {

                MeshRenderer renderer = obj.GetComponentInChildren<MeshRenderer>();
                MaterialPropertyBlock temp_propertyBlock = new MaterialPropertyBlock();
        
                //Get a renderer component either of the own gameobject or of a child
                //set the color property
                // texture uv convertion
                var localPlace = obj.transform.position;
                int x = Mathf.FloorToInt((localPlace.x - upperLeftBound.x) / (lowerRightBound.x - upperLeftBound.x) * backgroundImage.width);
                int z = Mathf.FloorToInt((localPlace.z - lowerRightBound.y) / (upperLeftBound.y - lowerRightBound.y)  * backgroundImage.height);

                //Debug.Log(backgroundImage.GetPixel(x, z));


                UnityEngine.Color tempColor = backgroundImage.GetPixel(z, x);
                    
                temp_propertyBlock.SetColor("_BaseColor", tempColor);
                obj.GetComponent<GridBase>().baseColor = tempColor;
                //apply propertyBlock to renderer
                renderer.SetPropertyBlock(temp_propertyBlock);
            }
            
            
            
            //Debug.Log("upperLeftBound : " + upperLeftBound);
            //Debug.Log("lowerRightBound : " + lowerRightBound);
        }
    }

    public GameObject GeneratePrefab(Vector3Int localPlace, string name, float scale)
    {
        if (!prefabDic.ContainsKey(name)) return null;
        // 分成 0 列和第一列， 这两列排布是不一样的
        var tempPos = new Vector3(localPlace.x, localPlace.y, localPlace.z);
        if (localPlace.y % 2 == 0)
        {
            tempPos.x -= 0.5f;
        }
        else
        {
            //tempPos.x += 0.5f;
        }

        tempPos *= scale;
        tempPos.y *= (3.0f / 4.0f);

        //tempPos.y *= (float)Math.Sqrt(3) / 2.0f;
        GameObject tempObj = Instantiate(prefabDic[name]);
        tempObj.transform.parent = rootGO.transform;
        tempObj.transform.localPosition = tempPos;


        return tempObj;
    }
    
}

#if UNITY_EDITOR
[CustomEditor(typeof(TileInfoCollector))]
public class TileInfoCollectorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var script = target as TileInfoCollector;
        if (GUILayout.Button("CollectTileInfo"))
        {
            script.CollectTileInfo();
        }
    }
}

#endif
