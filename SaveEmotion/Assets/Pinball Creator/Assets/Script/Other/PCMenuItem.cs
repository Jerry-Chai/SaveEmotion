#if (UNITY_EDITOR)
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PCMenuItem : MonoBehaviour
{
	// Add a menu item named "Do DeletePlayerPrefs" to Pinball Creator in the menu bar that delete all PlayerPrefs.
	[MenuItem("Pinball Creator/Init PlayerPrefs")]
	static void DeletePlayerPrefs()
	{
		PlayerPrefs.DeleteAll ();
		Debug.Log("Pinball Creator : PlayerPrefs have been deleted");
	}

	// Add a menu item named "Do DeletePlayerPrefs" to Pinball Creator in the menu bar that break Prefab Connection.
	/*[MenuItem("Pinball Creator/Break Prefab Connection (Selected GameObject)")]
	static void BreakPrefabConnection()
	{
		Undo.RegisterFullObjectHierarchyUndo (Selection.activeGameObject, Selection.activeGameObject.ToString());
		PrefabUtility.DisconnectPrefabInstance(Selection.activeGameObject);
		Debug.Log("Pinball Creator : Break Prefab connection");
	}*/ 
}
#endif