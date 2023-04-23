// Manager_Input_Setting : Description 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager_Input_Setting : MonoBehaviour {
	[Header ("Game Input")]
	public bool _Input_GetButton 			= false;		// true if you want input manage by Edit -> Project Settings -> Input
	public string Flipper_Left				= "left shift";
	public string Flipper_Right				= "right shift";
	public string Plunger					= "return";
	public string Pause_Game				= "e";
	public string Change_Camera				= "c";
	public string Shake_Left				= "r";
	public string Shake_Right				= "t";
	public string Shake_Up					= "f";

	[Header ("Debug Shortcuts")]
	private bool PinballDebugMode 			= false;

	//private string PlayMultiLeds			= "g";

	//private string Ball_Saver_and_ExtraBall = "i";
	//private string Mode_Ball_Saver_Off		 = "o";


	void Start(){
		if(_Input_GetButton){	
			GameObject[] gos = GameObject.FindGameObjectsWithTag("Flipper"); 						// Find all game objects with tag Flipper
			foreach (GameObject go  in gos)  { 
				go.GetComponent<Flippers>().F_InputGetButton();						// use Edit -> Project Settings -> Input for Flippers
			} 

			GetComponent<Manager_Game>().F_InputGetButton();							// Access UI and Shake buttons

			gos = GameObject.FindGameObjectsWithTag("Plunger"); 						// Find all game objects with tag Plunger
			foreach (GameObject go  in gos)  { 
				go.GetComponent<Spring_Launcher>().F_InputGetButton();					// use Edit -> Project Settings -> Input for Plunger
			}

			gos = GameObject.FindGameObjectsWithTag("Missions"); 						// Find all game objects with tag Missions
			foreach (GameObject go  in gos)  { 
				go.GetComponent<Mission_Start>().F_InputGetButton();					// use Edit -> Project Settings -> Input for Plunger
			}
		}
	}


	public string F_flipper_Left(){			return Flipper_Left;}
	public string F_flipper_Right(){		return Flipper_Right;}
	public string F_Plunger(){				return Plunger;}
	public string F_Pause_Game(){			return Pause_Game;}
	public string F_Change_Camera(){		return Change_Camera;}
	public string F_Shake_Left(){			return Shake_Left;}
	public string F_Shake_Right(){			return Shake_Right;}
	public string F_Shake_Up(){				return Shake_Up;}



	public bool F_Debug_Game(){				return PinballDebugMode;}

}
