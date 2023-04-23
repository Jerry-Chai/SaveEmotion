// MissionIndex.js : Description : use to return the mission index
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionIndex : MonoBehaviour {
	public int mission_Index = 0;					// Needed by the GameObject : "Manager_Game" on the Hierachy. <Important>  Must be different for each mission. 

	public int F_index () {
		return mission_Index;
	}
}
