using System.Collections;
using System.Collections.Generic;
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

    public Dictionary<string, GameObject> prefabDic;
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
            
            foreach (var pos in currTilemap.cellBounds.allPositionsWithin)
            {   
                Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);
                //Vector3 place = tilemap.CellToWorld(localPlace);
                if (currTilemap.HasTile(localPlace))
                {
                    var name = currTilemap.GetTile(localPlace).name;
                    Debug.Log(localPlace + "," + name);
                    //tileWorldLocations.Add(place);
                    GeneratePrefab(localPlace, name, gridSize);
                }
            }
        }
    }

    public void GeneratePrefab(Vector3Int localPlace, string name, float scale)
    {
        if (!prefabDic.ContainsKey(name)) return;
        GameObject tempObj = Instantiate(prefabDic[name]);
        tempObj.transform.parent = rootGO.transform;
        tempObj.transform.localPosition =  scale * new Vector3(localPlace.x,localPlace.y, localPlace.z);
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
