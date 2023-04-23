// Manager_Led_Animation : Description : 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Manager_Led_Animation : MonoBehaviour {

	private GameObject obj_Game_Manager;
	private Manager_Game gameManager;

	public  GameObject[] obj_Led;
	private ChangeSpriteRenderer[] Led_Renderer;

	[System.Serializable]
	public class List_Led_Pattern
	{
		public String[] pattern;
	}
	public List_Led_Pattern[] list_Led_Pattern  = new List_Led_Pattern[1];

	private bool Led_Anim_isPlaying = false;
	public float timeBetweenTwoLight = .1f;
	private float Timer;
	private float target = 0;
	private int count = 0;
	private string tmp_Last_State;  
	private int isPlaying_Pattern;
	[Header ("Led Animation associated with a mission")	]
	public bool b_Mission  = true;									// If it is a mission = true	//
	[Header ("Led Animation associated with a Group_Led")]	
	public bool b_Leds_Grp = false;									// If it is a animation = true // 
	[Header ("Led Animation associated only with the Group_Leds_Missions")]	
	public bool b_Mission_Leds_Grp = false;							// Use only for the led animation that indicate if missions are complete or not
	[Header ("Led Animation associated only with Mission Led_Mission_InProgress and Led_Part1_InProgress")]	
	public bool b_Mission_Leds_Mission_Part = false;		
	[Header ("Led Animation associated only with the Group_Leds_ExtraBall_BallSaver")]	
	public bool b_extraBall_or_BallSaver = false;					// Special condition for extraBall and BallSaver Leds. ExtraBall and ballSaver are manage by the Manager_Game.js script
	[Header ("Led Animation associated only with the Group_Leds_Multiplier")]
	public bool b_Multiplier = false;								// Special condition for Multiplier. Multipiler is manage by the Manager_Game.js script


	private bool b_Pause = false; 
	private float TimeScale;

	void Start () {
		if (obj_Game_Manager == null)																		// Connection with the gameObject : "Manager_Game"
			obj_Game_Manager = GameObject.Find("Manager_Game");

		if(obj_Game_Manager!=null)
			gameManager = obj_Game_Manager.GetComponent<Manager_Game>();	

		TimeScale = Time.timeScale;
		target += timeBetweenTwoLight*TimeScale;
	}

	void Update () {
		if(Led_Anim_isPlaying && !b_Pause){
			Timer = Mathf.MoveTowards(Timer,target,Time.deltaTime);
			if(Timer == target && count < list_Led_Pattern[isPlaying_Pattern].pattern.Length){				// PLay anmtion Pattern : If pause game the animation could be paused
				Animation_Leds(isPlaying_Pattern);
				count ++;
				target += timeBetweenTwoLight*TimeScale;
			}
			if(count == list_Led_Pattern[isPlaying_Pattern].pattern.Length){								// Play the last element of the pattern
				if(Timer == target){
					count ++;
				}
				if(count == (list_Led_Pattern[isPlaying_Pattern].pattern.Length+1)){						// Init Animation
					//Debug.Log("End of the animation : " + this.name);
					if(obj_Game_Manager!=null)gameManager.checkGlobalAnimationEnded();
					count = 0;target = 0;Timer = 0;Led_Anim_isPlaying = false;								// Init script
					if(b_Mission){
						SendMessage("Init_Leds_State");														// Init Led Object with the mission's scripts
					}
					else if(b_extraBall_or_BallSaver && obj_Game_Manager!=null){														// Special Condition to initialize the BallSaver and Extraball leds after a pattern. 	
						if(gameManager.b_ExtraBall)Led_Renderer[0].F_ChangeSprite_On();						// We check BallSaver and ExtraBall states directly from Manager_Game.js script
						else Led_Renderer[0].F_ChangeSprite_Off();											

						if(gameManager.b_Ball_Saver && obj_Game_Manager!=null)Led_Renderer[1].F_ChangeSprite_On();
						else Led_Renderer[1].F_ChangeSprite_Off();
					}
					else if(b_Multiplier){	
						init_Multiplier_Leds();																// Special Condition to initialize the Multiplier leds  
					}
					else if(b_Leds_Grp){
						for(var j = 0;j<obj_Led.Length;j++){

							//Led_Renderer[j].F_ChangeSprite_Off();											// Switch off the leds
						}
					}
					else if(b_Mission_Leds_Grp){
						for(var j = 0;j<obj_Led.Length;j++){
							if(Led_Renderer[j].Led_Mission_State())
								Led_Renderer[j].F_ChangeSprite_On();										// Switch on the leds
							else
								Led_Renderer[j].F_ChangeSprite_Off();										// Switch off the leds	
						}
					}
					else if(b_Mission_Leds_Mission_Part){
						for(var j = 0;j<obj_Led.Length;j++){
							if(Led_Renderer[j].F_led_Part_InProgress_State() == 1)
								Led_Renderer[j].F_ChangeSprite_On();										// Switch on the leds
							else
								Led_Renderer[j].F_ChangeSprite_Off();										// Switch off the leds	
						}
					}


					for(var i = 0;i<obj_Led.Length;i++){
						Led_Renderer[i].F_On_Blink_Switch();												// Start Blinking the light
					}

				}
			}


		}
	}


	public void Animation_Leds(int num){
		Led_Anim_isPlaying = false;
		isPlaying_Pattern = num;

		for(var j = 0;j< obj_Led.Length;j++){
			var tmp = list_Led_Pattern[num].pattern[count][j];
			if(tmp.ToString() == "1")
				Led_Renderer[j].F_ChangeSprite_On();
			else
				Led_Renderer[j].F_ChangeSprite_Off();
		}
		Led_Anim_isPlaying = true;
	}




	public void Play_New_Pattern(int num){														// CALL THIS FUNCTION TO PLAY A NEW PATTERN
		if(!b_Pause){
			for(var i = 0;i<obj_Led.Length;i++){
				Led_Renderer[i].F_Off_Blink_Switch();												// Stop Blinking each light
			}

			count = 0;target = 0;Timer = 0;Led_Anim_isPlaying = false;	
			Animation_Leds(num);
		}
	}


	public void Pause_Anim(){
		if(!b_Pause)Start_Pause_Anim();
		else Stop_Pause_Anim();
	}

	public void Stop_Pause_Anim(){b_Pause=false;}
	public void Start_Pause_Anim(){b_Pause=true;}

	public void Init_Obj_Led_Animation(GameObject[] tmp_obj_Led){
		if(obj_Led.Length == 0){
			obj_Led = new GameObject[tmp_obj_Led.Length];
			obj_Led = tmp_obj_Led;
		}

		Led_Renderer = new ChangeSpriteRenderer[obj_Led.Length];
		for(var k = 0;k<obj_Led.Length;k++){
			if(obj_Led[k] != null)Led_Renderer[k] = obj_Led[k].GetComponent<ChangeSpriteRenderer>();		
		}


		if(list_Led_Pattern[0].pattern.Length == 0){
			list_Led_Pattern[0].pattern = new String[obj_Led.Length*2+1];

			for(var j = 0;j< obj_Led.Length*2+1;j++){
				string temp_string = ""; 
				for(var i = 0;i< obj_Led.Length;i++){
					if(j< obj_Led.Length*2){
						if(j%obj_Led.Length == i)
							temp_string += "1";
						else
							temp_string += "0";
					}
					else{
						temp_string += "0";
					}
				}
				list_Led_Pattern[0].pattern[j] = temp_string;
			}

		}
	}


	public void init_Multiplier_Leds(){																		// Special Condition to initialize the Multiplier leds  
		if(obj_Game_Manager!=null){
			int tmp_multi  = gameManager.multiplier;														// We check Multiplier states directly from Manager_Game.js script 
			tmp_multi = tmp_multi/2;
			if(tmp_multi < 1){																					// multiplier = 1 so all the leds are switch off
				for(var i = 0;i< obj_Led.Length;i++){
					Led_Renderer[i].F_ChangeSprite_Off();	
				}
			}
			else{
				for(var j = 0;j < obj_Led.Length;j++){													// if multiplier is greater than 1 
					if(j < tmp_multi)																			// if tmp_multi = 1, switch on the first led, tmp_multi = 2, switch on the first two leds ...,
						Led_Renderer[j].F_ChangeSprite_On();	
					else
						Led_Renderer[j].F_ChangeSprite_Off();	
				}
			}
		}
	}

	public int HowManyAnimation(){
		return list_Led_Pattern.Length;
	}																												
}
