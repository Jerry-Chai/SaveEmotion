// Pause_Mission : Desciption : 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause_Mission : MonoBehaviour {
	public GameObject[] 			Led ;				// Put her the lights used on the mission Manager
	private ChangeSpriteRenderer[]	Led_Renderer;
	private Manager_Led_Animation 	manager_Led_Animation;
	private bool 					Pause = false;
	//private GameObject 				Led_Mission_In_Progress;				
	//private ChangeSpriteRenderer 	led_Mission_In_Progress;
	//private GameObject 				Led_Mission_Part1;				
	//private ChangeSpriteRenderer 	led_Mission_Part1;


	void Start(){
		Led_Renderer = new ChangeSpriteRenderer[Led.Length];
		for(var i = 0;i<Led.Length;i++){
			Led_Renderer[i] = Led[i].GetComponent<ChangeSpriteRenderer>();		
		}

		manager_Led_Animation = GetComponent<Manager_Led_Animation>();		
	}


	/// The function is called by the object : "Manager_Game" in the hierachy
	public void Start_Pause_Mission(){
		Pause = true;
		SendMessage("Pause_Start");												// Call function "Pause_Start" on the Mission Script 

	}

	/// The function is called by the object : "Manager_Game" in the hierachy
	public void Stop_Pause_Mission() {
		Pause = false;
		SendMessage("Pause_Stop");												// Call function "Pause_Stop" on the Mission Script 
	}

	public bool Return_Pause(){
		return Pause;
	}


	////////// THE GAME IS IN PAUSE MODE


	public void Pause_Game(){
		manager_Led_Animation.Pause_Anim();
		SendMessage("Pause_Game_Mission");
	}

	public void Init_Obj_Pause_Mission(GameObject[] tmp_obj_Led){				// Automatitaly connect mission's object to this script
		if(Led.Length == 0){
			Led = new GameObject[tmp_obj_Led.Length];
			Led = tmp_obj_Led;
		}

		Led_Renderer = new ChangeSpriteRenderer[Led.Length];
		for(var k = 0;k<Led.Length;k++){
			if(Led[k] != null)Led_Renderer[k] = Led[k].GetComponent<ChangeSpriteRenderer>();		
		}
	}

	public void Init_led_Mission_In_Progress(GameObject tmp_obj_Led){				// Automaticaly connect mission's object to this script
		//Led_Mission_In_Progress = tmp_obj_Led;
		//led_Mission_In_Progress = Led_Mission_In_Progress.GetComponent<ChangeSpriteRenderer>();
	}

	public void Init_led_Part1_In_Progress(GameObject tmp_obj_Led){				// Automaticaly connect mission's object to this script
		//Led_Mission_Part1 = tmp_obj_Led;
		//led_Mission_Part1 = Led_Mission_Part1.GetComponent<ChangeSpriteRenderer>();
	}

}
