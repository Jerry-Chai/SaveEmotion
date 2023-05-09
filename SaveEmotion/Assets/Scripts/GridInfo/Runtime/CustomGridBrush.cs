using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace UnityEditor.Tilemaps
{
#if UNITY_EDITOR
    [CustomGridBrush(true, false, true, "CustomBrush")]
    public class CustomGridBrush : GridBrush
    {
        //public bool lineStartActive = false;
        //public Vector3Int lineStart = Vector3Int.zero;
        public override void Paint(GridLayout grid, GameObject brushTarget, Vector3Int position)
        {

            //GridInfo info = brushTarget.GetComponentInParent<GridInfo>();
            //if (info.addNewPoint(new Vector3Int(position.x, position.y, position.z)))
            {
                base.Paint(grid, brushTarget, position);
            }

            var tilemap = brushTarget.GetComponent<Tilemap>();
            var tile = tilemap.GetTile(position) as Tile;
            Debug.Log(tile.name +" , " + tile.sprite);

        }

        /// <summary>Erases tiles and GameObjects from given bounds within the selected layers.</summary>
        /// <param name="gridLayout">Grid to erase data from.</param>
        /// <param name="brushTarget">Target of the erase operation. By default the currently selected GameObject.</param>
        /// <param name="position">The bounds to erase data from.</param>
        // public override void BoxErase(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
        // {
        //     if (brushTarget == null)
        //         return;
        //
        //     Tilemap map = brushTarget.GetComponent<Tilemap>();
        //     if (map == null)
        //         return;
        //
        //     var emptyTiles = new TileBase[position.size.x * position.size.y * position.size.z];
        //     map.SetTilesBlock(position, emptyTiles);
        //     foreach (Vector3Int location in position.allPositionsWithin)
        //     {
        //         map.SetTransformMatrix(location, Matrix4x4.identity);
        //         map.SetColor(location, Color.white);
        //     }
        // }

        /// <summary>Erases tiles and GameObjects in a given position within the selected layers.</summary>
        /// <param name="gridLayout">Grid used for layout.</param>
        /// <param name="brushTarget">Target of the erase operation. By default the currently selected GameObject.</param>
        /// <param name="position">The coordinates of the cell to erase data from.</param>
        public override void Erase(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
        {
            base.Erase(gridLayout, brushTarget, position);
            //GridInfo info = brushTarget.GetComponentInParent<GridInfo>();
            //info.removePoint(new Vector3Int(position.x, position.y, position.z));
            //Debug.Log("test");
            //Vector3Int min = position - pivot;
            //BoundsInt bounds = new BoundsInt(min, Vector3Int.one);
            //BoxErase(gridLayout, brushTarget, bounds);
        }
    }

    [CustomEditor(typeof(CustomGridBrush))]
    public class TestBrushEditor : GridBrushEditor
    {
        private CustomGridBrush testBrush { get { return target as CustomGridBrush; } }
        public override void OnPaintSceneGUI(GridLayout grid, GameObject brushTarget, BoundsInt position, GridBrushBase.Tool tool, bool executing)
        {
            base.OnPaintSceneGUI(grid, brushTarget, position, tool, executing);
        }
    }
#endif
}