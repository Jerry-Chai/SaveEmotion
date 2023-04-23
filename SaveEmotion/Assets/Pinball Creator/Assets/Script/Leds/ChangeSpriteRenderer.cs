// ChangeSpriteRenderer : Description : Used to change the renderer of a led
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSpriteRenderer : MonoBehaviour {

	public bool On = false;
	public bool b_Blinking = false;

	private Renderer rend;

	//private float Timer = 0;				// Variable used when you call Function Led_On_With_Timer(value : float
	//private float tmp_Time = 0;
	//private bool b_Led_On_With_Timer = true;


	[Header ("Led Emission")]
	public Color Emission_Off_ = new Color(0,0,0);
	public Color Emission_On = new Color(1,1,1);


	[Header ("Connect point Light to Led")	]
	public Light obj_Light ;
	private Light lightComp;

	private bool b_Led_Mission		= false;			// Use to know the state of Mission's leds.
	private int b_Led_Part_InProgress = 0;			// Use to know the state of Mission Led_Part1_InProgress.

	private LedSwitchOff ledSwitchOff;			// Use For Slingshot and bumper to switch of the light.Call LedSwitchOff.js on the same gameObject


	void Awake(){
		if(b_Blinking)this.transform.tag = "Blink"; 					// this tag is use by the script Blink.js on Manager_Game on the hierachy. 
	}

	void Start(){													// --> Init Led

		ledSwitchOff = GetComponent<LedSwitchOff>(); 

		rend = GetComponent<Renderer>();								// access component

		if(obj_Light)lightComp = obj_Light.GetComponent<Light>();		// access component

		if(On){	
			if(obj_Light)lightComp.enabled = true;
			rend.material.SetColor ("_EmissionColor",Emission_On);
		}
		else {
			if(obj_Light)lightComp.enabled = false;
			rend.material.SetColor ("_EmissionColor",Emission_Off_);
		}
	}



	public void F_ChangeSprite_On() {										//--> Switch On the led
		On = true;
		if(obj_Light)lightComp.enabled = true;
		rend.material.SetColor ("_EmissionColor",Emission_On);
	}

	public void F_ChangeSprite_Off() {										//--> Switch Off the led
		On = false;
		if(obj_Light)lightComp.enabled = false;
		rend.material.SetColor ("_EmissionColor",Emission_Off_);
	}


	public void F_ChangeSprite_On_Blink() {								// --> Led Blinking
		if(On && b_Blinking){
			if(obj_Light)lightComp.enabled = true;
			rend.material.SetColor ("_EmissionColor",Emission_On);
		}
	}

	public void F_ChangeSprite_Off_Blink() {								// --> Led Blinking
		if(On && b_Blinking){
			if(obj_Light)lightComp.enabled = false;
			rend.material.SetColor ("_EmissionColor",Emission_Off_);
		}
	}

	public bool F_On_or_Off(){return On;}									// return if led is switch On or off

	public void F_On_Blink_Switch(){b_Blinking = true;}					//	used in Manager_Led_Animation become you don't want the light blinking when we play a Led animation pattern

	public void F_Off_Blink_Switch(){b_Blinking = false;}					//	used in Manager_Led_Animation become you don't want the light blinking when we play a Led animation pattern



	public void Led_On_With_Timer(float value){							// Function call to enable the led during a few time
		//Timer = value;													// Init the timer
		if(ledSwitchOff != null)
			ledSwitchOff.Timer_Led();
		//else
		//	b_Led_On_With_Timer = false;									// Start the timer

		F_ChangeSprite_On();											// Switch On the led

	}

	public void Led_Mission_Complete(string switch_){					// This function is only called by a mission after the player lose a ball. If the mission is complete the led will be switch On
		if(switch_ == "On"){
			F_ChangeSprite_On();
			b_Led_Mission = true;										// Use to know the state of Mission's leds. The mission corresponding to this led is complete.
		}
		else{
			F_ChangeSprite_Off();
			b_Led_Mission = false;										// Use to know the state of Mission's leds. The mission corresponding to this led is not complete.
		}
	}

	public bool Led_Mission_State(){										// Call by the script Manager_Led_Animation.js
		return b_Led_Mission;
	}

	public void led_Part_InProgress_State(int value){
		b_Led_Part_InProgress = value;
	}

	public int F_led_Part_InProgress_State(){
		return b_Led_Part_InProgress;
	}
}
