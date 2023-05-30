// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
//
// public class GridManager : MonoBehaviour
// {
//     // Start is called before the first frame update
//
//     public Dictionary<Vector2Int, GameObject> gridPos2Obj;
//     public GameObject ballGO;
//     public float cellSize = 6.0f;
//     public int oldX;
//     public int oldY;
//     void Start()
//     {
//         gridPos2Obj = new Dictionary<Vector2Int, GameObject>();
//         foreach (var transform in this.transform.GetComponentsInChildren<Transform>())
//         {
//             if (transform.name == this.gameObject.name) continue;
//             // todo :: seperate useless block;
//
//             var tempPos = new Vector2Int((int)((transform.position.x + 0.1f) / (cellSize * Mathf.Sqrt(3.0f)/ 2.0f)), (int)(transform.position.y/ (cellSize * 3.0f / 4.0f) ));
//             if(!gridPos2Obj.ContainsKey(tempPos))
//             {
//                 gridPos2Obj.Add(tempPos, transform.gameObject);
//             }
//
//             oldX = (int) tempPos.x;
//             oldY = (int) tempPos.y;
//         }
//         
//         
//
//     }
//
//     // Update is called once per frame
//     void Update()
//     {
//         // todo ： should be more accuracy...
//         // 现在经过的时候，会需要中心经过才会消除， 但是其实可以设置一个区域碰撞范围。。
//         var tempX = (int)((ballGO.transform.position.x + 0.1f) / (cellSize * Mathf.Sqrt(3.0f)/ 2.0f));
//         var tempY = (int)(ballGO.transform.position.y/ (cellSize * 3.0f / 4.0f) );
//         Debug.Log(tempX + " , "+ tempY);
//         if (tempX != oldX || tempY != oldY)
//         {
//             oldX = tempX;
//             oldY = tempY;
//             GameObject grid;
//             gridPos2Obj.TryGetValue(new Vector2Int(oldX, oldY), out grid);
//             if (!grid) return;
//             DoChangeEffect(grid);
//         }
//     }
//
//     public void DoChangeEffect(GameObject grid)
//     {
//         grid.SetActive(false);
//     }
// }
