// Blink : Description : Make the Led you want blinking . 
// Blink.js allows to keep synchronyzed Leds on the playfield.

// How it work : On the Inspector, check the box "B_Blinking" on every Led you want to blink. 
// Then when game start this script (Blink.js) automaticaly connects all the leds with B_Blinking = true; 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blink : MonoBehaviour {
	public float Blink_Time_ms = .2f;											// You could choose the blinking time
	private float Timer = 0;											// variable used to create a timer on the update function														
	private float target = 0;											// variable used to create a timer on the update function	

	private ChangeSpriteRenderer[] changeSpriteRenderer; 								// Used to access ChangeSpriteRenderer components from each Led object
	private bool b_Blink = false;										// true if changeSpriteRenderer.length>0

	private float TimeScale;												// used to access Time.timeScale;	

	public bool b_Pause_Blinking = false;										// Used when the game is on pause : Pause_Game.


	void Start () {
		TimeScale = Time.timeScale;																
		Blink_Time_ms *= TimeScale;																//  a second stay a second even if you change Time.timeScale


		GameObject[] gos = GameObject.FindGameObjectsWithTag("Blink"); 									// find the leds with the tag "Blink" that should blink
		changeSpriteRenderer = new ChangeSpriteRenderer[gos.Length];
		int tmp_count = 0;
		foreach (GameObject go in gos)  { 	
			changeSpriteRenderer[tmp_count] = go.GetComponent<ChangeSpriteRenderer>();			// accessing ChangeSpriteRenderer components from each Led object
			tmp_count++;
		}

		if(changeSpriteRenderer.Length > 0)b_Blink = true;
	}



	void Update () {
		if(b_Blink && !b_Pause_Blinking){
			Timer = Mathf.MoveTowards(Timer,target,Time.deltaTime);								// Here the timer to know if the leds must be On or Off
			if(Timer == target && Blink_Time_ms == target){										// On : 
				for(var i =0; i<changeSpriteRenderer.Length; i++){
					changeSpriteRenderer[i].F_ChangeSprite_On_Blink();							// Led On
				}
				target = 0;
			}
			else if(Timer == target && 0 == target){											// Off : 
				target = Blink_Time_ms;
				for(var j =0; j<changeSpriteRenderer.Length; j++){
					changeSpriteRenderer[j].F_ChangeSprite_Off_Blink();							// Led Off
				}
			}
		}
	}



	public void Pause_Blinking(){																	// This function is called by Manager_Game.js (line 355) when you want to pause the game
		if(b_Pause_Blinking) b_Pause_Blinking = false;											// Pause stop
		else b_Pause_Blinking = true;															// Pause start
	}
}
