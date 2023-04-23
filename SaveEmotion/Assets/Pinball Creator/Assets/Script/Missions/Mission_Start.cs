// Mission_Start : Description. Manage the mission
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mission_Start : MonoBehaviour {

	private bool _GetButton = false;					// true if you want input manage by Edit -> Project Settings -> Input
	private bool DownLeft = false;
	private bool DownRight = false;

	private GameObject obj_Game_Manager;						// Represent Object Manager_Game on the hierarchy
	private MissionIndex missionIndex;						// Used to Access Mission_Index component (You find Mission_Index.js on each Mission)
	private Manager_Game gameManager;						// Used to Access Manager_Game component (You find Manager_Game.js on Manager_Game object on the hierachy)
	private AudioSource sound_;
	private bool a_Sound_is_Playing_When_Pause_Game_Start = false;
	[Header ("|->See Documentation for more informations about Mission<-|", order = 0)	]
	[Header ("Initialize mission after ball lost", order = 1)]						// --> 
	public bool InitMissionWhenBallLost = true;					// InitMissionWhenBallLost = true. By default When the player lose a ball the mission is initialized. If false . Mission is not initialize if we are on Mission part 1

	[Header ("Allow a mission to be paused.")	]						// --> If false. Mission is not affected by the pause of other mission. And the mission could'nt pause other mission
	public bool b_PauseMissionMode = true;					// By default a mission is paused when another mission begins. But if you want that mission is never paused check b_PauseMissionMode = false, 
	private bool b_Pause = false;					// Used to pause the mission

	[Header ("--> Table Mechanics Gpr 1  : Type", order = 0)]			// --> 
	public int HowManyTime_Gpr1	= 3;
	[Header ("Type : Target", order = 1)]								// --> 
	public bool Grp_1_Target = true;
	public bool Target_No_Order_Grp_1 = true;					// Check if there is no particular order to shoot the target
	public bool Target_Order_Grp_1 = false;					// Check if there is particular order to shoot the target. The target order is the same as the  obj_Grp_1 array order
	private Target[] target_Grp_1;							// Used to Access Target component because in this mission obj_Grp_1 are targets
	private Target[] target_Grp_2;							// Used to Access Target component because in this mission obj_Grp_2 are targets

	[Header ("Type : Rollover")]											// --> 
	public bool Grp_1_Rollover	= false;
	public bool Rollover_No_Order_Grp_1 = false;					// Check if there is no particular order to shoot the target
	public bool Rollover_Order_Grp_1 = false;					// Check if there is particular order to shoot the target. The target order is the same as the  obj_Grp_1 array order
	public bool Rollover_Type3_Grp_1 = false;					// a group of rollover that could be change when the player press flippers buttons
	private Rollovers[] Rollover_Grp_1;						// Used to Access Rollovers component 
	private Rollovers[] Rollover_Grp_2;						// Used to Access Rollovers component 
	private bool Rollover_StopMoving = true;
	private int[] LedTmp;  
	public bool SpecificText = false;					// If you want a specific text appear letter by letter on LCD Screen. Use for rollover Grp1
	private string Input_name_Left = "m";						// Use with Rollover_Type3_Grp_1. 
	private string Input_name_Right = "q";						// Use with Rollover_Type3_Grp_1. 
	private Manager_Input_Setting gameManager_Input;			// access Manager_Input_Setting component from Manager_Game GameObject

	[Header ("Type : Bumper")]											// --> 
	public bool Grp_1_Bumper = false;
	private Bumper_js[] Bumper_Grp_1;						// Used to Access Bumper_js component 
	private Bumper_js[] Bumper_Grp_2;						// Used to Access Bumper_js component 

	[Header ("Type : Spinner")]											// --> 
	public bool Grp_1_Spinner = false;
	private Spinner_LapCounter[] Spinner_Grp_1;				// Used to Access Spinner_LapCounter component
	private Spinner_LapCounter[] Spinner_Grp_2;				// Used to Access Spinner_LapCounter component

	[Header ("Type : Hole")	]											// --> 
	public bool Grp_1_Hole = false;
	private Hole[] Hole_Grp_1;							// Used to Access Hole component
	private Hole[]  Hole_Grp_2;							// Used to Access Hole component

	[Header ("--> Put here your tables Mechanics")	]					// --> 
	public GameObject[] obj_Grp_1;						// Put here your tables Mechanics 

	[Header ("--> Leds corresponding to the tables Mechanics obj_Gpr_1. Same order as the obj_Grp_1")]// --> 
	public GameObject[] obj_Grp_1_Leds;						// Put here your Leds corresponding to the tables Mechanics obj_Gpr_1. For exemple connect obj_Grp_1[0] to obj_Grp_1_Leds[0]
	[Header ("--> During Mission : Keep Leds from Grp1 On")]
	public bool KeepLedGrp1OnDuringMission = false;

	[Header ("--> Table Mechanics Gpr 2  : Type", order = 0)]			// --> 
	public int HowManyTime_Gpr2 = 3;
	[Header ("Type : Target", order = 1)]								// --> 
	public bool Grp_2_Target = true;
	public bool Target_No_Order_Grp_2 = true;					// Check if there is no particular order to shoot the target
	public bool Target_Order_Grp_2	= false;					// Check if there is particular order to shoot the target. The target order is the same as the  obj_Grp_1 array order
	public bool Target_Type_Stationary	= false;

	[Header ("Type : Rollover")	]										// --> 
	public bool Grp_2_Rollover = false;
	public bool Rollover_No_Order_Grp_2 = false;					// Check if there is no particular order to shoot the target
	public bool Rollover_Order_Grp_2 = false;					// Check if there is particular order to shoot the target. The target order is the same as the  obj_Grp_1 array order

	[Header ("Type : Bumper")	]										// --> 
	public bool Grp_2_Bumper = false;
	[Header ("Type : Spinner")	]										// --> 
	public bool Grp_2_Spinner = false;
	[Header ("Type : Hole")		]										// --> 

	public bool Grp_2_Hole	= false;
	[Header ("--> Put here your tables Mechanics")	]					// --> 
	public GameObject[] obj_Grp_2;						// Same as obj_Grp_1. Sometime it could be easy to separate your tables Mechanics to script a mission. 
	[Header ("--> Leds corresponding to the tables Mechanics obj_Gpr_2. Same order as the obj_Grp_2")]// --> 
	public GameObject[] obj_Grp_2_Leds;						// Put here your Leds corresponding to the tables Mechanics obj_Gpr_2. For exemple connect obj_Grp_2[0] to obj_Grp_2_Leds[0]
	private GameObject[] obj_Led;						// It is obj_Grp_1_Leds + obj_Grp_1_Leds. Used to manage Manager_Led_Animation.js and Pause_Mission.js
	private ChangeSpriteRenderer[] Led_Renderer;			// Used to Access ChangeSpriteRenderer component from your Leds. (You find ChangeSpriteRenderer.js on each Leds object)
	//public int[] arr_led_State;  					// an array to manage the leds states (On or Off)
	public List<int> arr_led_State = new List<int>();

	[Header ("--> Led for Part1 in progress")]							// --> 
	public GameObject Led_Part1_InProgress;					// the led that could be used to switch on a led when a mission is in progress
	private ChangeSpriteRenderer led_Part1_InProgress_;			// This Led is not connected with the animation system of this mission. But you could add the led to a Group_Leds Prefab (Project->Prefabs->Grp_Leds->Group_Leds) 
	private int Led_Part1_InProgress_State = 0;

	[Header ("--> Led for Mission in progress")	]						// --> 
	public GameObject Led_Mission_InProgress;					// the led that could be used to switch on a led when a mission is in progress
	private ChangeSpriteRenderer led_Mission_InProgress_;			// This Led is not connected with the animation system of this mission. But you could add the led to a Group_Leds Prefab (Project->Prefabs->Grp_Leds->Group_Leds) 
	private int Led_Mission_InProgress_State = 0;

	[Header ("--> The led that switch On when the mission is complete")	]// --> 
	public GameObject Led_Mission_Complete;						// the led that switch On when the mission is complete
	private ChangeSpriteRenderer Led_Mission_Renderer;

	[Header ("--> Texts you want to display on LCD screen")]
	public string Mission_Txt_name = "";						// Use to display on LCD screen	
	public string[] Mission_Txt;							// An array to manage the text you want to write on LCD screen

	private int Step = 0;							// Used on function Counter(). Represent the different step to reach a mission 

	//public int[] arr_obj_Grp_1_State;						// an array to manage obj_Grp_1 states
	//public int[] arr_obj_Grp_2_State;  					// an array to manage obj_Grp_2 states
	public List<int> arr_obj_Grp_1_State = new List<int>();
	public List<int> arr_obj_Grp_2_State = new List<int>();

	[Header ("--> Options during mission",order=0)	]					// --> Options during mission
	[Header ("Timer",order=1)]											// --> Timer options during mission
	public bool b_Mission_Timer	= false;					// Only one timer during a mission
	public bool b_Mission_Multi_Timer = false;					// init timer after hitting object
	public int Mission_Time = 10;
	private bool b_MissionTimer = false;					// use for the mission timer
	private float missionTimer = 10;						// use for the mission timer
	private int lastTimerValue;								// use for the mission timer

	[Header ("Multi ball (only available for Rollover Gpr2)")	]											// --> Multi ball. The muti ball is activated when a mission start (Part 2 Start)
	public bool multiBall = false;					// if multi ball activate
	public int numberOfBall = 3;							// number of ball for the multi-ball
	public int JackpotPoints = 20000;						// Points win for a jackpot when multi ball is enable
	private int Step_MultiBall = 0;							// use when you need to pass through rollover Grp2 in a specific order. 

	[Header ("--> Options when Mission is Complete")]					// --> options that could win the player after a mission success. Choose only one option at a time
	public int Points = 20000;						// When a mission is complete the player win this points
	[Header ("Random Bonus between (ExtraBall,BallSaver,Multiplier,Points)")]// 
	public bool Random_Bonus = false;					// Random bonus between : ExtraBall, BallSaver, Multiplier, KickBack
	private int tmp_random;
	public bool ExtraBall = false;					// Player win an extra ball
	public bool BallSaver = false;					// Player win a ball Saver
	public int BallSaverDuration = 10;							// Choose the ball saver duration. if BallSaverDuration = -1 Ball Saver is enable until the player lose the ball
	public bool Multiplier = false;					// Increase the multiplier Bonus x2 x4 x6 x8 x10 Super Bonus (Choose the Multiplier_SuperBonus value On the gameObject Manger_Game on the hierarchy)
	public bool KickBack = false;					// When player win the mission one or more doors are open
	public bool BeginWithKickBack = false;					// If you want a mission start with the kickback open
	public GameObject[] obj_Door_Kickback;						// Put here the door you want to open
	private Target[] Kickback_Door;							// Accesse Target component from game objects obj_Door_Kickback
	public GameObject[] obj_Led_Kickback;						// Put here the led connected to the KickBack doors
	private ChangeSpriteRenderer[] Kickback_Led;			// Access ChangeSpriteRenderer component from game objects obj_Led_Kickback



	[Header ("--> Skillshot Mission")]									// --> Skillshot Mission . Skillshot work in association with the Plunger(Spring object) and Manager_Game on the hierarchy 
	public bool b_SkillShot = false;					// true if it is the skillshot mission
	public int Skillshot_Target_num = 1;							// Index number of the object you want to choose to be the target
	public GameObject Led_SkillShot;						// The led connected to the Skillshot mission
	private ChangeSpriteRenderer led_SkillShot;				// access ChangeSpriteRenderer component
	public int SkillshotDuration = 5;							// Skillshot Duration (seconds)
	public int Skillshot_Points	= 1000000;					// Points win if skillshot mission is complete
	public AudioClip sfx_Skillshot;
	public AudioClip sfx_Skillshot_Fail;
	private bool b_Mission_SkillShot = false;					// use to enable or disable skillshot mission
	private bool b_SkillShot_Timer = false;					// use to enable or disable skillshot timer
	private bool b_Skillshot_OnAir = false;					// Use to prevent the skillshot timer restart if the ball is not correctly ejected from the plunger 

	[Header ("--> Choose a led animation (or not) for each mission part")]// --> Animation that could be played for PART 1(before), PART 2(the mission Start), PART 3 (during the Mission), Mission Complete or fail
	// Next section refer to the animations created on script Manager_Game.js (Global Manager Leds pattern)						
	public int LED_Anim_Num_Part1 = 0;							// if LED_Anim_Num_Part1 = -1 no animation is playing/
	public int LED_Anim_Num_Part2 = 0;							// if LED_Anim_Num_Part2 = -1 no animation is playing/
	public int LED_Anim_Num_Part3 = 0;							// if LED_Anim_Num_Part3 = -1 no animation is playing/
	public int LED_Anim_Num_Complete = 0;							// if LED_Anim_Num_Complete = -1 no animation is playing/
	public int LED_Anim_Num_Fail = 0;							// if AniNumberFail = -1 no animation is playing/

	[Header ("--> Choose Toy animation (or not) for each mission part")]// --> Animated Object on the playfield that could be played for PART 1(before), PART 2(the mission Start), PART 3 (during the Mission), Mission Complete or fail
	public GameObject PlayfieldAnimation;						// Attached here an animated object with the script Toys.js
	private Toys playfieldAnimation;

	[Header ("Connected More than One Animation to the bumper")	]				// Connect a GameObject or paticule system with the script Toys.js attached

	public int PF_AnimNumPart1	= 0;							// if PF_AnimNumPart1 = -1 no animation is playing/
	public int PF_AnimNumPart2	= 0;							// if PF_AnimNumPart2 = -1 no animation is playing/
	public int PF_AnimNumPart3	= 0;							// if PF_AnimNumPart3 = -1 no animation is playing/
	public int PF_AnimNumComplete = 0;							// if PF_AnimNumComplete = -1 no animation is playing/
	public int PF_AnimNumFail	 = 0;							// if PF_AnimNumFail = -1 no animation is playing/

	[Header ("Multiple animation (only available when mission is complete)")]						// Connect a GameObject or paticule system with the script Toys.js attached
	public Toys[] Toys;
	public int[] AnimNums;


	[Header ("--> Choose animation (or not) to display on LCD screen for each mission part")]// --> 	
	public GameObject[] obj_Anim_On_Led_Display;						// Put here animation you want to play on the LCD sceen		
	public int LCD_AnimNumPart1 = 0;							// if LCD_AnimNumPart1 = -1 no animation is playing/
	public int LCD_AnimNumPart2	= 0;							// if LCD_AnimNumPart2 = -1 no animation is playing/
	public int LCD_AnimNumPart3	= 0;							// if LCD_AnimNumPart3 = -1 no animation is playing/
	public int LCD_AnimNumComplete = 0;							// if LCD_AnimNumComplete = -1 no animation is playing/
	public int LCD_AnimNumFail	= 0;							// if LCD_AnimNumFail = -1 no animation is playing/

	[Header ("--> Choose an sound fx (or not) for each mission part")]// --> 	
	public AudioClip sfx_Part1;						// Put here a sound fx
	public AudioClip sfx_Part2;						// Put here a sound fx
	public AudioClip sfx_Part3;						// Put here a sound fx
	public AudioClip sfx_Complete;						// Put here a sound fx
	public AudioClip sfx_Fail;						// Put here a sound fx

	[Header ("--> Debug elements")]// --> 	
	public GameObject obj_Gui_Debug;

	void Start(){																			// --> 1) INIT
		sound_ = GetComponent<AudioSource>();													// access AudioSource component
		first();																				// first() initialize obj_Grp_1[], obj_Grp_2[] and obj_Led[]
		//yield WaitForEndOfFrame();																
		//Mission_Intialisation();																// Init mission
	}

	////////// COUNTER : START /////////////
	// Object connected to obj_Grp_1[] et obj_Grp_2[] send message to this mission when an object is touched by the ball.
	public void Counter(int num){																// --> Mission Progression
		if(!b_Pause){																			// USE when the game PAUSE MODE or When you want to pause a mission IN GAME
			if(Grp_1_Target){			Part_1_Type_Target_Gpr1(num);}							// Part 1 : Pre mission
			else if(Grp_1_Rollover){	Part_1_Type_Rollover_Gpr1(num);}
			else if(Grp_1_Bumper){		Part_1_Type_Bumper_Gpr1(num);}
			else if(Grp_1_Spinner){		Part_1_Type_Spinner_Gpr1(num);}
			else if(Grp_1_Hole){		Part_1_Type_Hole_Gpr1(num);}

			if(Grp_2_Target){			Part_2_Type_Target_Gpr2(num);}							// Part 2 : Mission start
			else if(Grp_2_Rollover){	Part_2_Type_Rollover_Gpr2(num);}
			else if(Grp_2_Bumper){		Part_2_Type_Bumper_Gpr2(num);}
			else if(Grp_2_Spinner){		Part_2_Type_Spinner_Gpr2(num);}
			else if(Grp_2_Hole){		Part_2_Type_Hole_Gpr2(num);}
			else if(obj_Grp_2.Length==0 || HowManyTime_Gpr2 == 0){								// If no object on part 2.
				if(Step == HowManyTime_Gpr1)	Step++;
			}

			if(Grp_2_Target){			Part_3_Type_Target_Gpr2(num);}							// Part 3 : Mission				
			else if(Grp_2_Rollover){	Part_3_Type_Rollover_Gpr2(num);}
			else if(Grp_2_Bumper){		Part_3_Type_Bumper_Gpr2(num);}
			else if(Grp_2_Spinner){		Part_3_Type_Spinner_Gpr2(num);}
			else if(Grp_2_Hole){		Part_3_Type_Hole_Gpr2(num);}


			if(Step == (HowManyTime_Gpr1+HowManyTime_Gpr2+1)){									// Part 4 : Mission Complete
				if(Led_Mission_InProgress){
					Led_Mission_InProgress_State = 0;
					led_Mission_InProgress_.led_Part_InProgress_State(0);
					led_Mission_InProgress_.F_ChangeSprite_Off();	

				}

				if(Random_Bonus){
					F_Random_Bonus();
				}
				else{
					if(ExtraBall){																// Options when the mission is complete	
						gameManager.F_Mode_ExtraBall();
						Mission_Complete();	
					}
					else if(BallSaver){
						gameManager.F_Mode_Ball_Saver_On(BallSaverDuration);
						Mission_Complete();	
					}
					else if(Multiplier){
						gameManager.F_Multiplier();
						Mission_Complete();	
					}
					else if(KickBack){
						for(var i =0;i<obj_Door_Kickback.Length;i++){
							//Debug.Log ("Here");
							Kickback_Door[i].Desactivate_Object();	
						}
						for(var i =0;i<obj_Led_Kickback.Length;i++){
							Kickback_Led[i].F_ChangeSprite_On();	
						}
						Mission_Complete();	
					}
					else{
						Mission_Complete();	
					}
				}
			}
		}
	} 
	////////// COUNTER : END /////////////

	//////// FUNCTION WHEN A MISSION IS FINISHED ////////
	public void Mission_Complete(){														// --> Mission Complete
		Bonus_text();

		gameManager.Add_Score(Points);														// Add Score
		if(Led_Mission_Complete)
			Led_Mission_Renderer.Led_Mission_Complete("On");								// Switch On the led that indicate the mission is complete	

		Play_LedAnim_ObjAnim_LCDAnim_Complete();											// Play led animation; object animation or lcd animation																												
		Mission_Intialisation();															// Init Mission
	}

	//////// FUNCTION WHEN A MISSION IS FAILED ////////
	public void Mission_Fail(){															// --> Mission Fail		
		if(Mission_Txt.Length > 1
			&& Mission_Txt[1]!="")gameManager.Add_Info_To_Array(Mission_Txt_name + "\n" + Mission_Txt[1], 3);// Write a text on LCD Screen
		Play_LedAnim_ObjAnim_LCDAnim_Fail();												// Play led animation; object animation or lcd animation	
		Mission_Intialisation();															// Init Mission
	}

	//////// FUNCTION Mission_Intialisation_AfterBallLost ////////
	public void Mission_Intialisation_AfterBallLost(){										// --> initialisation after a ball lost
		if(InitMissionWhenBallLost || Step > obj_Grp_1.Length){								// Mission is initialize if : 1) var InitMissionWhenBallLost = true or 2) if the mission is on part 3					
			Mission_Intialisation();														// Init the mission
		}
		if(!BeginWithKickBack)Init_KickBack_WhenPlayerLoseBall(); 							// Init kickback when the ball is lost
	}


	//////// Mission_Intialisation : START ////////
	private bool init_Ended = false;
	public void Mission_Intialisation(){													// --> Function to initialize a mission
		init_Ended = false;
		if(Grp_1_Target){			Mission_Intialisation_Target_Gp1();}
		else if(Grp_1_Rollover){	Mission_Intialisation_Rollover_Gp1();}
		else if(Grp_1_Bumper){		Mission_Intialisation_Bumper_Gp1();}
		else if(Grp_1_Spinner){		Mission_Intialisation_Spinner_Gp1();}
		else if(Grp_1_Hole){		Mission_Intialisation_Hole_Gp1();}

		if(Grp_2_Target){			Mission_Intialisation_Target_Gp2();}
		else if(Grp_2_Rollover){	Mission_Intialisation_Rollover_Gp2();}
		else if(Grp_2_Bumper){		Mission_Intialisation_Bumper_Gp2();}
		else if(Grp_2_Spinner){		Mission_Intialisation_Spinner_Gp2();}
		else if(Grp_2_Hole){		Mission_Intialisation_Hole_Gp2();}

		if (BeginWithKickBack) {															// init kickback
			for (var i = 0; i < obj_Door_Kickback.Length; i++) {
				Kickback_Door [i].Desactivate_Object ();	
			}
			for (var i = 0; i < obj_Led_Kickback.Length; i++) {
				Kickback_Led [i].F_ChangeSprite_On ();	
			}
		} 


		if(b_PauseMissionMode)gameManager.Stop_Pause_Mode();							// Stop_Mission_Pause_Mode
		Step=0;																			// init mission step
		Step_MultiBall = HowManyTime_Gpr1+1;
		InitMissionTimer();																// init the mission timer
		init_Ended = true;
	}

	public void Mission_Intialisation_StartGame(){											// --> Function to initialize a mission when a game is starting. Call by Manager_Game.js
		init_Ended = false;
		if(Grp_1_Target){			Mission_Intialisation_Target_Gp1();}
		else if(Grp_1_Rollover){	Mission_Intialisation_Rollover_Gp1();}
		else if(Grp_1_Bumper){		Mission_Intialisation_Bumper_Gp1();}
		else if(Grp_1_Spinner){		Mission_Intialisation_Spinner_Gp1();}
		else if(Grp_1_Hole){		Mission_Intialisation_Hole_Gp1();}

		if(Grp_2_Target){			Mission_Intialisation_Target_Gp2();}
		else if(Grp_2_Rollover){	Mission_Intialisation_Rollover_Gp2();}
		else if(Grp_2_Bumper){		Mission_Intialisation_Bumper_Gp2();}
		else if(Grp_2_Spinner){		Mission_Intialisation_Spinner_Gp2();}
		else if(Grp_2_Hole){		Mission_Intialisation_Hole_Gp2();}

		if(BeginWithKickBack){															// init kickback
			for(var i =0;i<obj_Door_Kickback.Length;i++){
				Kickback_Door[i].Desactivate_Object();	
			}
			for(var i =0;i<obj_Led_Kickback.Length;i++){
				Kickback_Led[i].F_ChangeSprite_On();
			}
		}
		else{															// init kickback
			for(var i =0;i<obj_Door_Kickback.Length;i++){
				Kickback_Door[i].Activate_Object();	
			}
			for(var i =0;i<obj_Led_Kickback.Length;i++){
				Kickback_Led[i].F_ChangeSprite_Off();	
			}
		}

		Step=0;																			// init mission step
		Step_MultiBall = HowManyTime_Gpr1+1;
		InitMissionTimer();																// init the mission timer
		init_Ended = true;
	}

	//////// Mission_Intialisation : END ////////


	public void Init_KickBack_WhenPlayerLoseBall(){									// -> Init kickback when the ball is lost
		//Debug.Log("Here 3");
		if(BeginWithKickBack){															// init kickback
			for(var i =0;i<obj_Door_Kickback.Length;i++){
				Kickback_Door[i].Desactivate_Object();	
			}
			for(var i =0;i<obj_Led_Kickback.Length;i++){
				Kickback_Led[i].F_ChangeSprite_On();	
			}
		}
		else{															// init kickback
			for(var i =0;i<obj_Door_Kickback.Length;i++){
				Kickback_Door[i].Activate_Object();	
			}
			for(var i =0;i<obj_Led_Kickback.Length;i++){
				Kickback_Led[i].F_ChangeSprite_Off();	
			}
		}

	} 

	public int F_Led_Gpr1_num(int num){
		return num;
	}
	public int F_Led_Gpr2_num(int num){
		int tmp = 0;
		if(obj_Grp_1.Length != obj_Grp_1_Leds.Length){
			tmp = obj_Grp_1.Length - obj_Grp_1_Leds.Length;
		}

		num += obj_Grp_1.Length - tmp;
		return num;
	}


	//// PAUSE the mission (game continue but this mission is on pause) START ///
	public void Pause_Start(){																// What To Do When Pause START. Call Pause_Mission.js on mission's game object. All the led's missions are switch off
		if(b_PauseMissionMode){										
			b_Pause= true;
			if(obj_Gui_Debug)
				obj_Gui_Debug.GetComponent<Image>().color = new Color(1,0,0);

			for(var i = 0;i<Led_Renderer.Length;i++){
				Led_Renderer[i].F_ChangeSprite_Off();				
			}
			if(Led_Mission_InProgress){
				led_Mission_InProgress_.F_ChangeSprite_Off();
			}
			if(Led_Part1_InProgress){
				led_Part1_InProgress_.F_ChangeSprite_Off();
			}
		}
	}	

	public void Pause_Stop() {																// Waht To Do When pause STOP. Call Pause_Mission.js on mission's game object
		if(b_PauseMissionMode){
			if(obj_Gui_Debug)
				obj_Gui_Debug.GetComponent<Image>().color = new Color(1,1,1);
			b_Pause= false;
			// ---> 6 bis) WRITE HERE what you need to init after Pause
			if(Grp_1_Target){			Pause_Stop_Target_Gpr1();}	
			else if(Grp_1_Rollover){	Pause_Stop_Rollover_Gpr1();}
			else if(Grp_1_Bumper){		Pause_Stop_Bumper_Gpr1();}
			else if(Grp_1_Spinner){		Pause_Stop_Spinner_Gpr1();}
			else if(Grp_1_Hole){		Pause_Stop_Hole_Gpr1();}

			if(Grp_2_Target){			Pause_Stop_Target_Gpr2();}
			else if(Grp_2_Rollover){	Pause_Stop_Rollover_Gpr2();}
			else if(Grp_2_Bumper){		Pause_Stop_Bumper_Gpr2();}
			else if(Grp_2_Spinner){		Pause_Stop_Spinner_Gpr2();}
			else if(Grp_2_Hole){		Pause_Stop_Hole_Gpr2();}

			if(Led_Mission_InProgress){
				if(Led_Mission_InProgress_State == 0)
					led_Mission_InProgress_.F_ChangeSprite_Off();	
				else{
					Led_Mission_InProgress_State = 1;
					led_Mission_InProgress_.led_Part_InProgress_State(1);
					led_Mission_InProgress_.F_ChangeSprite_On();
				}
			}
		}
	}
	//// PAUSE the mission END ///

	//////// FUNCTION WHEN multi-ball id ENDED ////////
	// This function is used to initialize a mission that uses the multi-ball
	// Multi ball is manage by the manager_Game Object on the hierarchy (see script Manager_Game.js).
	// How to start a multi ball :
	// 1) call gameManager.F_Mission_MultiBall(missionIndex.F_index(), nbr_of_Ball : int) to start multi ball
	// 2) When a multi-ball end the mission receved a message (Mode_MultiBall_Ended()) from manager_Game (search Mission Multi Ball stop on  Manager_Game.js). 
	public void Mode_MultiBall_Ended(){													// --> Call by manager_Game on the hierachy
		Play_LedAnim_ObjAnim_LCDAnim_Fail();											// Play led animation; object animation or lcd animation							
		if(Mission_Txt.Length > 1
			&& Mission_Txt[1]!="")gameManager.Add_Info_To_Array(Mission_Txt_name + "\n" + Mission_Txt[1], 3);// Write Text on LCD Screen

		if(Led_Mission_Complete)
			Led_Mission_Renderer.Led_Mission_Complete("On");								// Switch On the led that indicate the mission is complete	

		Mission_Intialisation();
		Init_Leds_State();																// init Leds states		

       // Debug.Log("Ici");
        gameManager.InitObjAfterMultiball();
	}


	////////// UPDATE FUNCTION START /////////
	void Update(){																	// --> Update function
		if(!b_Pause){

			if(!_GetButton){																// true if you want input manage by Edit -> Project Settings -> Input
				if(Step < HowManyTime_Gpr1 && Rollover_Type3_Grp_1 && init_Ended){			// Use when Rollover_Type3_Grp_1 is chosen. init_Ended prevent bug when the mission is initialized
					if(Input.GetKeyDown(Input_name_Right) && Rollover_StopMoving){			
						Move_Leds_To_Right();}												// Move the led to the right 
					if(Input.GetKeyDown(Input_name_Left) && Rollover_StopMoving){
						Move_Leds_To_Left ();}												// Move the led to the left 
				}
			}
			else{
				if(Step < HowManyTime_Gpr1 && Rollover_Type3_Grp_1 && init_Ended){			// Use when Rollover_Type3_Grp_1 is chosen. init_Ended prevent bug when the mission is initialized
					if(Input.GetAxisRaw(Input_name_Right) > 0 && !DownRight && Rollover_StopMoving){			
						Move_Leds_To_Right();DownRight = true;}									// Move the led to the right 
					if(Input.GetAxisRaw(Input_name_Left) > 0 && !DownLeft && Rollover_StopMoving){
						Move_Leds_To_Left ();DownLeft = true;}									// Move the led to the left 

					if(Input.GetAxisRaw(Input_name_Right) == 0 && DownRight){					// Right Move led position only once
						DownRight = false;}
					if(Input.GetAxisRaw(Input_name_Left)  == 0 && DownLeft){					// Left Move led position only once
						DownLeft = false;}

				}
			}

			if(Rollover_Type3_Grp_1){
				for (var i = 0; i < Input.touchCount; ++i) {									// --> Touch Screen part
					if(Input.GetTouch(i).position.x > Screen.width*.5
						&& Input.GetTouch(i).position.y < Screen.height*.5){						// know which part of the screen is touched by the player
						if (Input.GetTouch(i).phase == TouchPhase.Began && Rollover_StopMoving){// if touch is detect 		
							Move_Leds_To_Right();												// (Lane Change Mission) Move the led to the right 											
						}
					}
					if(Input.GetTouch(i).position.x < Screen.width*.5
						&& Input.GetTouch(i).position.y < Screen.height*.5){						// know which part of the screen is touched by the player
						if (Input.GetTouch(i).phase == TouchPhase.Began && Rollover_StopMoving){// if touch is detect 	
							Move_Leds_To_Left ();												// (Lane Change Mission) Move the led to the left 		
						}
					}
				}
			}



			if(b_MissionTimer){															// MISSION TIMER
				missionTimer = Mathf.MoveTowards(missionTimer,0,						// Timer progression
					Time.deltaTime);

				var tmp_Timer = Mathf.Round(missionTimer);

				if(tmp_Timer != lastTimerValue)
					gameManager.Add_Info_To_Timer(tmp_Timer.ToString());				// Use 2 for second param to prevent mistake on LCD Screen

				if(missionTimer == 0){													// if missionTimer == 0 the mission is failed
					b_MissionTimer = false;												// stop the timer
					Mission_Fail();														// Mission fail
				}
				lastTimerValue = (int)tmp_Timer;	
			}

			if(b_SkillShot_Timer){														// Skillshot MISSION TIMER
				missionTimer = Mathf.MoveTowards(missionTimer,0,						// Timer progression
					Time.deltaTime);

				var tmp_Timer = Mathf.Round(missionTimer);

				if(tmp_Timer != lastTimerValue)
					gameManager.Add_Info_To_Timer(tmp_Timer.ToString());				// Use 2 for second param to prevent mistake on LCD Screen

				if(missionTimer == 0){													// if missionTimer == 0 the mission is failed
					b_SkillShot_Timer = false;											// stop the timer
					Skillshot_Mission_Fail();											// Mission Skillshot fail
				}
				lastTimerValue = (int)tmp_Timer;	
			}

		}
	}
	////////// UPDATE FUNCTION END /////////


	public void Pause_Game_Mission(){														// Mission : PAUSE (the scene is on Pause)
		if(!b_Pause){
			b_Pause=true;																// Pause mission : on
			if(sound_.isPlaying){														// If a sound is playing when pause start
				sound_.Pause();															// Pause sound
				a_Sound_is_Playing_When_Pause_Game_Start = true;						// Change state of a_Sound_is_Playing_When_Pause_Game_Start to true
			}
			else{																		// If no sound is playing
				a_Sound_is_Playing_When_Pause_Game_Start = false;						// Change state of a_Sound_is_Playing_When_Pause_Game_Start to true
			}
		}
		else{
			b_Pause=false;																// Pause mission : false
			if(a_Sound_is_Playing_When_Pause_Game_Start){								// a_Sound_is_Playing_When_Pause_Game_Start = true
				sound_.Play();															// Stop to pause the sound
			}
		}
	}

	public void MissionTimer(int seconds){												// --> Call this function to start the timer.
		missionTimer = seconds;															// Choose the timer duration with seconds : int 
		b_MissionTimer = true;															// Info : When a timer end, the mission is failed
	}

	public void InitMissionTimer(){														// Call this function to initialize the timer
		gameManager.Add_Info_To_Timer("");
		b_MissionTimer = false;
	}


	public void Enable_Skillshot_Mission(){												// --> Call by the script Game_Manager.js on object Game_Manger on the hierarchy. Send when the player start a new ball
		b_Mission_SkillShot = true;														// enable skillshot mission
		if(Led_SkillShot)																// if a led is connected to the mission
			led_SkillShot.F_ChangeSprite_On();											// Switch on the led
		b_Skillshot_OnAir = false;														// Use to prevent the skillshot timer restart if the ball is not correctly ejected from the plunger 
	}

	public void Disable_Skillshot_Mission(){												// --> Call by the script Game_Manager.js on object Game_Manger on the hierarchy. Send when the player lose a new ball
		b_Mission_SkillShot = false;													// Disable skillshot mission
		if(Led_SkillShot)																// if a led is connected to the mission
			led_SkillShot.F_ChangeSprite_Off();											// Switch off the led
		b_Skillshot_OnAir = true;														// Use to prevent the skillshot timer restart if the ball is not correctly ejected from the plunger 
		b_SkillShot_Timer = false;	
		gameManager.Add_Info_To_Timer("");												// init text
		missionTimer = 0;															
	}

	public void Skillshot_Mission(){														// --> Call by the script Spring_Launcher.js on the object Spring on the hierarchy.
		if(b_Mission_SkillShot && !b_Skillshot_OnAir){									// Use to prevent the skillshot timer restart if the ball is not correctly ejected from the plunger				
			F_SkillshotTimer(SkillshotDuration);										// start the skillshot timer
			b_Skillshot_OnAir = true;													// The timer can not start twice
		}
	}

	public void Skillshot_Mission_Complete(){												// --> Skillshot mission is complete
		if(sfx_Skillshot)sound_.PlayOneShot(sfx_Skillshot);
		b_Mission_SkillShot = false;													// disable skillshot mission
		gameManager.Add_Info_To_Array("Skillshot Complete", 3);							// Write a text on LCD Screen
		gameManager.Add_Score(Skillshot_Points);										// Add Points
		if(Led_SkillShot)																// if a led is connected to the mission
			led_SkillShot.F_ChangeSprite_Off();											// Switch off the led
		b_SkillShot_Timer = false;														// stop the timer
		gameManager.Add_Info_To_Timer("");												// init text
		missionTimer =0;																// init
	}

	public void Skillshot_Mission_Fail(){													// --> Skillshot mission is failed
		if(sfx_Skillshot_Fail)sound_.PlayOneShot(sfx_Skillshot_Fail);
		b_Mission_SkillShot = false;													// disable skillshot mission
		gameManager.Add_Info_To_Array("Skillshot Fail", 3);								// Write a text on LCD Screen
		if(Led_SkillShot)																// if a led is connected to the mission
			led_SkillShot.F_ChangeSprite_Off();											// Switch off the led
		gameManager.Add_Info_To_Timer("");												// init text
	}

	public void F_SkillshotTimer(int seconds){											// --> Init and start the skillshot timer											
		missionTimer = seconds;															
		b_SkillShot_Timer = true;															
	}


	public void Bonus_text(){															// --> Text on LCD screen when mission is complete
		if(ExtraBall){
			if(Mission_Txt.Length > 10 && Mission_Txt[0]!="" && Mission_Txt[7]!="")
				gameManager.Add_Info_To_Array(Mission_Txt_name + "\n" + "<size= 20>" 
					+ Mission_Txt[0] + "\n" 											// text : Mission Complete
					+  Mission_Txt[7]+"</size>", 3);									// text : Extra Ball
		}
		else if(BallSaver){
			if(Mission_Txt.Length > 10 && Mission_Txt[0]!="" && Mission_Txt[8]!="")
				gameManager.Add_Info_To_Array(Mission_Txt_name + "\n" + "<size= 20>" 
					+ Mission_Txt[0] + "\n" 											// text : Mission Complete
					+  Mission_Txt[8]+"</size>", 3);									// text : Ball Saver
		}
		else if(Multiplier){
			if(gameManager.F_return_multiplier() <= 10){
				if(Mission_Txt.Length > 2 && Mission_Txt[0]!="" && Mission_Txt[2]!="")
					gameManager.Add_Info_To_Array(Mission_Txt_name + "\n" + "<size= 20>" 
						+ Mission_Txt[0] + "\n"										// text : Mission Complete
						+  Mission_Txt[2]  											// text : Multiplier x 
						+ gameManager.F_return_multiplier().ToString()				// text : x
						+"</size>", 3);
			}
			else {																// SUper Bonus if multiplier > 10
				if(Mission_Txt.Length > 3 && Mission_Txt[0]!="" && Mission_Txt[3]!=""){
					gameManager.Add_Info_To_Array(Mission_Txt_name + "\n" + "<size= 20>" 
						+ Mission_Txt[0] + "\n"										// text : Mission Complete 
						+ Mission_Txt[3]  + " "									// text : Super Bonus
						+ gameManager.F_return_Mulitplier_SuperBonus().ToString()		
						+"</size>", 3);

				}
			}

		}
		else if(KickBack){
			if(Mission_Txt.Length > 10 && Mission_Txt[0]!="" && Mission_Txt[10]!="")
				gameManager.Add_Info_To_Array(Mission_Txt_name + "\n" + "<size= 20>" 
					+ Mission_Txt[0] + "\n" 											// text : Mission Complete
					+  Mission_Txt[10]+"</size>", 3);									// text : Kickback open
		}
		else {
			if(Mission_Txt.Length > 10 && Mission_Txt[0]!="" && Mission_Txt[9]!="")
				gameManager.Add_Info_To_Array(Mission_Txt_name + "\n" + "<size= 20>" 
					+ Mission_Txt[0] + "\n" + Points									// text : Mission Complete
					+  Mission_Txt[9]+"</size>", 3);									// text : Points
		}
	}

	public void F_Random_Bonus(){															// --> if Random_Bonus = true. The player win a random Bonus (ExtraBall,BallSaver,Multiplier,Points Only)
		tmp_random = Random.Range(1,5);

		switch (tmp_random) {
		case 1:																		// -> ExtraBall	
			gameManager.F_Mode_ExtraBall();
			Mission_Complete();		
			if(Mission_Txt.Length > 10 && Mission_Txt[0]!="" && Mission_Txt[6]!="" && Mission_Txt[7]!="")
				gameManager.Add_Info_To_Array(Mission_Txt_name + "\n" + "<size= 20>" 
					+ Mission_Txt[0] + "\n" 											// text : Mission Complete
					+  Mission_Txt[6]+ "\n" 											// text : Random Bonus 
					+  Mission_Txt[7]+"</size>", 3);									// text : Extra Ball
			break;
		case 2:																		// -> BallSaver	
			gameManager.F_Mode_Ball_Saver_On(BallSaverDuration);
			Mission_Complete();		
			if(Mission_Txt.Length > 10 && Mission_Txt[0]!="" && Mission_Txt[6]!="" && Mission_Txt[8]!="")
				gameManager.Add_Info_To_Array(Mission_Txt_name + "\n" + "<size= 20>" 
					+ Mission_Txt[0] + "\n"  											// text : Mission Complete
					+  Mission_Txt[6]+ "\n" 											// text : Random Bonus  
					+  Mission_Txt[8]+"</size>", 3);									// text : Ball Saver
			break;
		case 3:																		// -> Multiplier	
			gameManager.F_Multiplier();
			Mission_Complete();			
			if(gameManager.F_return_multiplier() <= 10 && Mission_Txt[0]!="" && Mission_Txt[6]!="" && Mission_Txt[2]!="")
			if(Mission_Txt.Length > 2)
				gameManager.Add_Info_To_Array(Mission_Txt_name + "\n" + "<size= 20>" 
					+ Mission_Txt[0] + "\n"											// text : Mission Complete
					+  Mission_Txt[6]		 										// text : Random Bonus 
					+ "\n" + Mission_Txt[2]  										// text : Multiplier x 
					+ gameManager.F_return_multiplier().ToString()					// text : x
					+"</size>", 3);
			else if(gameManager.F_return_multiplier() >= 12)
			if(Mission_Txt.Length > 3 && Mission_Txt[0]!="" && Mission_Txt[6]!="" && Mission_Txt[3]!="")
				gameManager.Add_Info_To_Array(Mission_Txt_name + "\n" + "<size= 20>" 
					+ Mission_Txt[0] + "\n"											// text : Mission Complete 
					+  Mission_Txt[6]	+ "\n"	 									// text : Random Bonus 
					+ Mission_Txt[3]  												// text : Super Bonus
					+ gameManager.F_return_Mulitplier_SuperBonus().ToString()		
					+"</size>", 3);
			break;
		case 4:																		// -> Points Only	
			Mission_Complete();											
			if(Mission_Txt.Length > 10 && Mission_Txt[0]!="" && Mission_Txt[6]!="" && Mission_Txt[9]!="")
				gameManager.Add_Info_To_Array(Mission_Txt_name + "\n" + "<size= 20>" 
					+ Mission_Txt[0] + "\n" 											// text : Mission Complete
					+  Mission_Txt[6]+ "\n" + Points			 						// text : Random Bonus 
					+  Points + Mission_Txt[9]+"</size>", 3);							// text : x Points
			break;
		}
	}


	public void newTxtArray(){																// --> Init Mission Text : if(Mission_Txt.Length == 0) Create text for the mission 
		Mission_Txt = new string[14];
		Mission_Txt[0] = "Mission Complete";
		Mission_Txt[1] = "Mission Failed";
		Mission_Txt[2] = "Multiplier x";
		Mission_Txt[3] = "Super Bonus";
		Mission_Txt[4] = "More x ";
		Mission_Txt[5] = "More x ";
		Mission_Txt[6] = "Random Bonus";
		Mission_Txt[7] = "Extra Ball";
		Mission_Txt[8] = "Ball Saver";
		Mission_Txt[9] = " Points";
		Mission_Txt[10]= "Kickback open";
		Mission_Txt[11]= "Word";
		Mission_Txt[12]= "Jackpot";
		Mission_Txt[13]= "Mission Start";
	}



	//////// FUNCTION FIRST : START /////////
	public void first(){																	// --> Initialisation when scene start
		if (obj_Game_Manager == null)														// Connect the Mission to the gameObject : "Manager_Game"
			obj_Game_Manager = GameObject.Find("Manager_Game");

		obj_Led = new GameObject[obj_Grp_1_Leds.Length + obj_Grp_2_Leds.Length];		// --> Put obj_Grp_1_Leds + obj_Grp_2_Leds inside obj_Led
		for(var l =0;l<obj_Grp_1_Leds.Length;l++){															
			obj_Led[l] = obj_Grp_1_Leds[l];
		}	
		for(var l =0;l<obj_Grp_2_Leds.Length;l++){																								
			obj_Led[l+obj_Grp_1_Leds.Length] = obj_Grp_2_Leds[l];}		 

		Led_Renderer = new ChangeSpriteRenderer[obj_Led.Length];						// --> Connect the Mission to obj_Led[i]<ChangeSpriteRenderer>() component. 
		for(var k =0;k<obj_Led.Length;k++){															
			Led_Renderer[k] = obj_Led[k].GetComponent<ChangeSpriteRenderer>();				
		}

		missionIndex = GetComponent<MissionIndex>();									// --> Connect the Mission to <MissionIndex>() component. 
		gameManager = obj_Game_Manager.GetComponent<Manager_Game>();					// --> Connect the Mission to <Manager_Game>() component. 


		arr_led_State.Clear ();
		arr_obj_Grp_1_State.Clear ();
		arr_obj_Grp_2_State.Clear ();

		for(var i =0;i<obj_Led.Length;i++){
			arr_led_State.Add(0);														// Create Array to record the Leds state from obj_Led
		}

		for(var i =0;i<obj_Grp_1.Length;i++){												// Create Array to record the state of the gameObject inside obj_Grp_1
			arr_obj_Grp_1_State.Add(0);
		}

		for(var i =0;i<obj_Grp_2.Length;i++){												// Create Array ro record the state of the gameObject inside obj_Grp_2
			arr_obj_Grp_2_State.Add(0);
		}

		if(Led_Mission_Complete)														// Connect the Mission to Led_Mission_Complete <ChangeSpriteRenderer>() component
			Led_Mission_Renderer = Led_Mission_Complete.GetComponent< ChangeSpriteRenderer>();

		GetComponent<Manager_Led_Animation>().Init_Obj_Led_Animation(obj_Led);			// Connect automaticaly the leds to the script Manager_Led_Animation on this Mission
		GetComponent<Pause_Mission>().Init_Obj_Pause_Mission(obj_Led);					// Connect automaticaly the leds to the script Pause_Mission on this Mission

		if(b_SkillShot){																// --> if b_SkillShot = true. This mission is used for the Skillshot.
			GameObject[] gos;			
			gos = GameObject.FindGameObjectsWithTag("Plunger"); 						 
			foreach (GameObject go in gos)  { 
				go.SendMessage("Connect_Plunger_To_Skillshot_Mission",this.gameObject);	// you need to tell the PLUNGER that is the skillshot mission	Spring_Launcher.js	on gameObject Spring_Launcher on the hierarchy
			} 

			gameManager.F_Init_Skillshot_Mission(this.gameObject);						// you need to tell the Manager_Game that is the skillshot mission	 

			if(Led_SkillShot)															// if a led is connected to the mission
				led_SkillShot= Led_SkillShot.GetComponent<ChangeSpriteRenderer>();		// Access ChangeSpriteRenderer component from Led_SkillShot GameObject
		}

		if(obj_Door_Kickback.Length > 0){												// Init kickback if object is connected to obj_Door_Kickback
			Kickback_Door = new Target[obj_Door_Kickback.Length];
			for(var i =0;i<obj_Door_Kickback.Length;i++){
				Kickback_Door[i] = obj_Door_Kickback[i].GetComponent<Target>();		// Access Target  component
			}
			Kickback_Led = new ChangeSpriteRenderer[obj_Led_Kickback.Length];			
			for(var i =0;i<obj_Led_Kickback.Length;i++){
				Kickback_Led[i] = obj_Led_Kickback[i].GetComponent<ChangeSpriteRenderer>();// Access Target  ChangeSpriteRenderer
			}

		}

		if(PlayfieldAnimation)playfieldAnimation = PlayfieldAnimation.GetComponent<Toys>();// Init if GameObject is connected

		// --> initialize GameObject mechanics depending on the type of object
		if(Grp_1_Target){																// Grp1 : type : Target														
			F_First_Target_Grp1();
		}
		else if(Grp_1_Rollover){														// Grp1 : type : Rollover	
			Rollover_Grp_1 = new Rollovers[obj_Grp_1.Length];
			for(var i =0;i<obj_Grp_1.Length;i++){															
				Rollover_Grp_1[i] = obj_Grp_1[i].GetComponent<Rollovers>();								
			}

			if(Rollover_Type3_Grp_1){
				HowManyTime_Gpr1 = obj_Grp_1.Length;
			}
		}
		else if(Grp_1_Bumper){															// Grp1 : type : Bumper
			F_First_Bumper_Grp1();
		}
		else if(Grp_1_Spinner){															// Grp1 : type : Spinner	
			F_First_Spinner_Grp1();
		}
		else if(Grp_1_Hole){															// Grp1 : type : Hole
			F_First_Hole_Grp1();
		}

		if(Grp_2_Target){																// Grp2 : type : Target	
			F_First_Target_Grp2();
		}
		else if(Grp_2_Rollover){														// Grp2 : type : Rollover	
			Rollover_Grp_2 = new Rollovers[obj_Grp_2.Length];
			for(var i =0;i<obj_Grp_2.Length;i++){															
				Rollover_Grp_2[i] = obj_Grp_2[i].GetComponent<Rollovers>();								
			}												
		}	
		else if(Grp_2_Bumper){															// Grp2 : type : Bumper
			F_First_Bumper_Grp2();
		}
		else if(Grp_2_Spinner){															// Grp2 : type : Spinner
			F_First_Spinner_Grp2();
		}
		else if(Grp_2_Hole){															// Grp2 : type : Hole
			F_First_Hole_Grp2();
		}

		if(Led_Mission_InProgress){														// Init the led : Led_Mission_InProgress
			led_Mission_InProgress_ = Led_Mission_InProgress.GetComponent<ChangeSpriteRenderer>();
			GetComponent<Pause_Mission>().Init_led_Mission_In_Progress(Led_Mission_InProgress);	
			Led_Mission_InProgress_State = 0;
			led_Mission_InProgress_.led_Part_InProgress_State(0);
		}
		if(Led_Part1_InProgress){														// Init the led : Led_Part1_InProgress
			led_Part1_InProgress_ = Led_Part1_InProgress.GetComponent<ChangeSpriteRenderer>();
			GetComponent<Pause_Mission>().Init_led_Part1_In_Progress(Led_Part1_InProgress);
			led_Part1_InProgress_.led_Part_InProgress_State(0);	
			Led_Part1_InProgress_State = 0;
		}

		gameManager_Input = obj_Game_Manager.GetComponent<Manager_Input_Setting>();	// Access Manager_Input_Setting component from object Manager_Game on the hierarchy 
		Input_name_Left = gameManager_Input.F_flipper_Left();							
		Input_name_Right = gameManager_Input.F_flipper_Right();

		if(Mission_Txt.Length == 0){newTxtArray();}										// if(Mission_Txt.Length == 0) Create text for the mission 

	}


	//////// FUNCTION FIRST : END /////////


	//////// USED WHEN A LED ANIMATION IS FINISH TO RESTORE THE LAST LED STATE //////// 
	public void Init_Leds_State(){															// USED WHEN A LED ANIMATION IS FINISH TO RESTORE THE LAST LED STATE
		for(var i =0;i<arr_led_State.Count;i++){
			if(GetComponent<Pause_Mission>().Return_Pause() == false){					// If the mission is not on Pause
				if(arr_led_State[i]==1)Led_Renderer[i].F_ChangeSprite_On();				// Led is on
				else Led_Renderer[i].F_ChangeSprite_Off();								// Led is off
			}
			else{																		// If the Mission is in Pause
				Led_Renderer[i].F_ChangeSprite_Off();									// All the light must be turn off after a Led animation
			}
		}
	}

	public void InitLedMission(){															// Init led that indicate that mission is complete
		if(Led_Mission_Complete)														// This function is called by the Manager_Game.js when player is Game Over
			Led_Mission_Renderer.Led_Mission_Complete("Off");
	}




	// Here the function for each type of mechanics : Target, rollover, bumper, spinner and hole
	// for each type of mechanics there are those function : (exemple target)
	// Part_1_Type_Target_Gpr1()
	// F_First_Target_Grp1()
	// F_First_Target_Grp2()
	// Mission_Intialisation_Target_Gp1()
	// Pause_Stop_Target_Gpr1()
	// Part_2_Type_Target_Gpr2()
	// Part_3_Type_Target_Gpr2()
	// Mission_Intialisation_Target_Gp2()
	// Pause_Stop_Target_Gpr2()

	////////////   TARGET SECTION : START //////////////
	public void Part_1_Type_Target_Gpr1(int num){	
		if(Step < HowManyTime_Gpr1){															// --> PART 1 : Shoot target Gpr_1 : Steps to be performed by the player before the mission begins
			for(var i =0;i<HowManyTime_Gpr1;i++){												
				if(target_Grp_1[i].index_info() == num  && arr_obj_Grp_1_State[i] == 0){		// Target from target_Grp_1 and the target has not been touched
					arr_obj_Grp_1_State[i] = 1;													// Update the state of target from Gpr_1


					if(arr_led_State.Count > F_Led_Gpr1_num(i)){								// if the taget as a led attached
						arr_led_State[F_Led_Gpr1_num(i)]= 0;									// You want switch off the leds
						Led_Renderer[F_Led_Gpr1_num(i)].F_ChangeSprite_Off();					// Led : Off	
					}
					Step++;																		// Next step

					if(Target_Order_Grp_1 && Step < HowManyTime_Gpr1){	
						arr_obj_Grp_1_State[Step]= 0;														
						target_Grp_1[Step].Activate_Object();									// Activate the target. Nothing happens if it's a stationary target
						if(obj_Grp_1_Leds.Length>Step){											// If a led is attached to the target
							arr_led_State[F_Led_Gpr1_num(Step)]= 1;								// Switch On the led
							Led_Renderer[F_Led_Gpr1_num(Step)].F_ChangeSprite_On();				// Led : On
						}
					}

					if(Mission_Txt.Length > 4 && Mission_Txt[4]!=""){							// Write a text on LCD Screen
						gameManager.Add_Info_To_Array(Mission_Txt_name + "\n" + Mission_Txt[4] + (HowManyTime_Gpr1 - Step).ToString() , 3);
					}


					if(Step < HowManyTime_Gpr1)Play_LedAnim_ObjAnim_LCDAnim_Part1();

					if(b_Mission_SkillShot && target_Grp_1[i].index_info() 						// -> If the skill shot is enabled end the player has shot the good target
						== Skillshot_Target_num){				
						Skillshot_Mission_Complete();											// Skill Shot Complete
					}
					break;
				}			
			}	
		}
	}

	public void F_First_Target_Grp1(){	
		target_Grp_1 = new Target[obj_Grp_1.Length];
		for(var i  =0;i<obj_Grp_1.Length;i++){															
			target_Grp_1[i] = obj_Grp_1[i].GetComponent<Target>();								// Connect the Mission to obj_Grp_1[i].GetComponent<Target>();	 component
		}
		HowManyTime_Gpr1 = obj_Grp_1.Length;													// Init the number of target the player need to hit
		if(Target_Type_Stationary){
			Target_Order_Grp_2 = false;
			Target_No_Order_Grp_2 = true;
		}
	}

	public void F_First_Target_Grp2(){	
		target_Grp_2 = new Target[obj_Grp_2.Length];
		for(var i  =0;i<obj_Grp_2.Length;i++){															
			target_Grp_2[i] = obj_Grp_2[i].GetComponent<Target>();								// Connect the Mission to obj_Grp_1[i].GetComponent<Target>();	 component
		}
		HowManyTime_Gpr2 = obj_Grp_2.Length;													// Init the number of target the player need to hit
	}

	public void Mission_Intialisation_Target_Gp1(){
		for(var j =0;j<obj_Grp_1.Length;j++){												// Init Gpr_1
			if(Target_No_Order_Grp_1){															// -> Target_No_Order_Grp_1 
				arr_obj_Grp_1_State[j]= 0;															
				target_Grp_1[j].Activate_Object();	
			}	
			else if(Target_Order_Grp_1){														// -> Target_Order_Grp_1
				if(j==0){
					arr_obj_Grp_1_State[j]= 0;													// state : 0 (Activate)		
					target_Grp_1[j].Activate_Object();	
				}
				else{
					arr_obj_Grp_1_State[j]= 1;													// state : 1 (Desactivate)		
					target_Grp_1[j].Desactivate_Object();	
				}
			}							
		}
		if(Target_No_Order_Grp_1){																// -> Target_No_Order_Grp_1 	
			for(var i  = 0;i < obj_Grp_1_Leds.Length;i++){									// init obj_Grp_1_Leds state 
				arr_led_State[F_Led_Gpr1_num(i)]= 1;												// Switch on the leds
				if(arr_led_State[F_Led_Gpr1_num(i)] == 1) Led_Renderer[F_Led_Gpr1_num(i)].F_ChangeSprite_On();	// Led : On
				else Led_Renderer[F_Led_Gpr1_num(i)].F_ChangeSprite_Off();										// Led : Off
			}
		}	
		else if(Target_Order_Grp_1){															// -> Target_Order_Grp_1
			for(var i = 0;i < obj_Grp_1_Leds.Length;i++){											// init obj_Grp_1_Leds state 
				if(i == 0){
					arr_led_State[F_Led_Gpr1_num(0)]= 1;
					Led_Renderer[F_Led_Gpr1_num(0)].F_ChangeSprite_On();						// Led : On
				}
				else{
					arr_led_State[F_Led_Gpr1_num(i)]= 0;										
					Led_Renderer[F_Led_Gpr1_num(i)].F_ChangeSprite_Off();						// Led : Off
				}										
			}
		}

		if(Led_Part1_InProgress){
			Led_Part1_InProgress_State = 1;
			led_Part1_InProgress_.F_ChangeSprite_On();
			led_Part1_InProgress_.led_Part_InProgress_State(1);
		}
	}

	public void Pause_Stop_Target_Gpr1(){															// --> 
		for(var j  = 0;j < obj_Grp_1.Length;j++){											// Init Gpr_1
			if(arr_obj_Grp_1_State[j] == 1)	target_Grp_1[j].Desactivate_Object();
			else target_Grp_1[j].Activate_Object();										
		}
		for(var i  = 0;i < obj_Grp_1_Leds.Length;i++){										// init obj_Grp_1_Leds state 
			if(arr_led_State[F_Led_Gpr1_num(i)] == 1) Led_Renderer[F_Led_Gpr1_num(i)].F_ChangeSprite_On();	// Led : On
			else Led_Renderer[F_Led_Gpr1_num(i)].F_ChangeSprite_Off();										// Led : Off
		}
		if(Led_Part1_InProgress){																// init Led_Part1_InProgress
			if(Led_Part1_InProgress_State == 0){
				led_Part1_InProgress_.F_ChangeSprite_Off();
			}
			else{
				Led_Part1_InProgress_State = 1;
				led_Part1_InProgress_.led_Part_InProgress_State(1);
				led_Part1_InProgress_.F_ChangeSprite_On();
			}
		}
	}

	// PART 2
	public void Part_2_Type_Target_Gpr2(int num){
		if(Step == HowManyTime_Gpr1){		
			if(b_PauseMissionMode)gameManager.Start_Pause_Mode(missionIndex.F_index());
			for(var i  = 0;i < obj_Grp_1_Leds.Length;i++){							// init obj_Grp_1_Leds state 
				if(KeepLedGrp1OnDuringMission){											// init Led
					arr_led_State[F_Led_Gpr1_num(i)]= 1;										
					Led_Renderer[F_Led_Gpr1_num(i)].F_ChangeSprite_On();
				}
				else{
					arr_led_State[F_Led_Gpr1_num(i)]= 0;										
					Led_Renderer[F_Led_Gpr1_num(i)].F_ChangeSprite_Off();
				}
			}

			if(Led_Part1_InProgress){													// init Led_Part1_InProgress
				Led_Part1_InProgress_State = 0;
				led_Part1_InProgress_.led_Part_InProgress_State(0);
				led_Part1_InProgress_.F_ChangeSprite_Off();
			}
			if(Led_Mission_InProgress){													// init Led_Mission_InProgress
				Led_Mission_InProgress_State = 1;
				led_Mission_InProgress_.led_Part_InProgress_State(1);
				led_Mission_InProgress_.F_ChangeSprite_On();	
			}
			if(b_Mission_Timer || b_Mission_Multi_Timer){MissionTimer(Mission_Time);}	// Init Timer with a value						

			if(multiBall){																// Start multiball if multiBall = true									
				gameManager.F_Mission_MultiBall(missionIndex.F_index(),numberOfBall);
			}

			if(Mission_Txt.Length > 13													// text on display
				&& Mission_Txt[13]!="")gameManager.Add_Info_To_Array(Mission_Txt_name + "\n" + Mission_Txt[13],3);

			if(Target_No_Order_Grp_2 || Target_Type_Stationary){						// -> Target_No_Order_Grp_2 || 	Target_Type_Stationary	
				for(var j  =0;j<obj_Grp_2.Length;j++){								// Update the state of target from Gpr_2										
					arr_obj_Grp_2_State[j]= 0;											// Activate the Gpr_2 targets	
					target_Grp_2[j].Activate_Object();	
					if(arr_led_State.Count > F_Led_Gpr2_num(j)){
						arr_led_State[F_Led_Gpr2_num(j)]= 1;							// You want switch on the leds
						Led_Renderer[F_Led_Gpr2_num(j)].F_ChangeSprite_On();			// Led : On	
					}									
				}						
			}
			if(Target_Order_Grp_2){														// -> Target_Order_Grp_2
				arr_obj_Grp_2_State[0]= 0;												// state : 1 (Activate)		
				target_Grp_2[0].Activate_Object();	
				if(arr_led_State.Count > obj_Grp_1_Leds.Length){
					arr_led_State[obj_Grp_1_Leds.Length]= 1;							// You want switch on the leds
					Led_Renderer[obj_Grp_1_Leds.Length].F_ChangeSprite_On();			// Led : On	
				}	
			}	

			Step++;
			Play_LedAnim_ObjAnim_LCDAnim_Part2();										// Play led animation, object animation or lcd animation
		}
	}

	// PART 3
	public void Part_3_Type_Target_Gpr2(int num){										
		if(Step > HowManyTime_Gpr1 && Step < (HowManyTime_Gpr1 + HowManyTime_Gpr2+1)){
			for(var i  = 0;i<obj_Grp_2.Length;i++){													
				if(target_Grp_2[i].index_info() == num  && arr_obj_Grp_2_State[i] == 0){
					if(b_Mission_Multi_Timer){MissionTimer(Mission_Time);}				// Init Timer with a value

					arr_obj_Grp_2_State[i] = 1;											// Update the state of target from Gpr_2	

					if((Target_No_Order_Grp_2 || Target_Order_Grp_2 || Target_Type_Stationary) && obj_Grp_2_Leds.Length > 0){
						if(obj_Grp_1_Leds.Length > 0 && arr_led_State.Count >= HowManyTime_Gpr1%obj_Grp_1_Leds.Length+HowManyTime_Gpr2%obj_Grp_2_Leds.Length){					
							arr_led_State[obj_Grp_1_Leds.Length+i]= 0;					// You want switch On the leds
							Led_Renderer[obj_Grp_1_Leds.Length+i].F_ChangeSprite_Off();	// Led : On	
						}
						else if(obj_Grp_1_Leds.Length == 0 && arr_led_State.Count >= HowManyTime_Gpr2%obj_Grp_2_Leds.Length){								
							arr_led_State[i]= 0;									// You want switch On the leds
							Led_Renderer[i].F_ChangeSprite_Off();					// Led : On	
						}
					}

					Step++;

					if(Target_Order_Grp_2 && Step < (HowManyTime_Gpr1 + obj_Grp_2.Length)+1){	// -> if Target_Order_Grp_2 is checked
						arr_obj_Grp_2_State[Step-HowManyTime_Gpr1-1]= 0;														
						target_Grp_2[Step-HowManyTime_Gpr1-1].Activate_Object();				// Activate the target. Nothing happens if it's a stationary target
						if(obj_Grp_2_Leds.Length>Step-HowManyTime_Gpr1-1){						// If a led is attached to the target
							arr_led_State[F_Led_Gpr2_num(Step-HowManyTime_Gpr1-1)]= 1;			// Switch On the led
							Led_Renderer[F_Led_Gpr2_num(Step-HowManyTime_Gpr1-1)].F_ChangeSprite_On();	// Led : On
						}
					}

					if(Mission_Txt.Length > 5 && Mission_Txt[5]!=""){							// Write a text on LCD Screen
						gameManager.Add_Info_To_Array(Mission_Txt_name + "\n" + Mission_Txt[5] + (HowManyTime_Gpr1+obj_Grp_2.Length+1 - Step).ToString() , 3);
					}
					if(Step < (HowManyTime_Gpr1 + HowManyTime_Gpr2))
						Play_LedAnim_ObjAnim_LCDAnim_Part3();										// Play led animation, object animation or lcd animation
					break;
				}							
			}					
		}	
	}

	// PART INITIALISATION
	public void Mission_Intialisation_Target_Gp2(){
		if(Led_Mission_InProgress){															// init Led_Mission_InProgress
			led_Mission_InProgress_.F_ChangeSprite_Off();
			Led_Mission_InProgress_State = 0;
			led_Mission_InProgress_.led_Part_InProgress_State(0);
		}

		if(Target_Type_Stationary){															// -> if Target_Type_Stationary
			for(var j  = 0;j<obj_Grp_2.Length;j++){									// Init Gpr_2	
				arr_obj_Grp_2_State[j]= 0;													// state : 1 (desactivate)													
				target_Grp_2[j].Activate_Object();										
			}
		}
		else{																				// -> if Target_No_Order_Grp_2,Target_Order_Grp_2
			for(var j = 0;j<obj_Grp_2.Length;j++){												// Init Gpr_2	
				arr_obj_Grp_2_State[j]= 1;													// state : 1 (desactivate)													
				target_Grp_2[j].Desactivate_Object();										
			}
		}

		for(var j =obj_Grp_1_Leds.Length;j<obj_Grp_1_Leds.Length+obj_Grp_2_Leds.Length;j++){	// Init Gpr_1
			arr_led_State[j]= 0;															// You want switch Off the leds
			Led_Renderer[j].F_ChangeSprite_Off();											// Led : Off	
		}
	}


	public void Pause_Stop_Target_Gpr2(){														// --> 
		if(!Target_Type_Stationary){														// -> if  Target_No_Order_Grp_2,Target_Order_Grp_2
			for(var j = 0;j < obj_Grp_2.Length;j++){									// Init Gpr_2
				if(arr_obj_Grp_2_State[j] == 1)	{target_Grp_2[j].Desactivate_Object();}
				else target_Grp_2[j].Activate_Object();										
			}
		}
		for(var i = 0;i < obj_Grp_2_Leds.Length;i++){									// init obj_Grp_1_Leds state 
			if(arr_led_State[i+obj_Grp_1_Leds.Length] == 1) Led_Renderer[i+obj_Grp_1_Leds.Length].F_ChangeSprite_On();	// Led : On
			else Led_Renderer[i+obj_Grp_1_Leds.Length].F_ChangeSprite_Off();										// Led : Off
		}

		if(Led_Mission_InProgress){															// init Led_Mission_InProgress
			if(Led_Mission_InProgress_State == 0){
				led_Mission_InProgress_.F_ChangeSprite_Off();
			}
			else{
				Led_Mission_InProgress_State = 1;
				led_Mission_InProgress_.led_Part_InProgress_State(1);
				led_Mission_InProgress_.F_ChangeSprite_On();
			}
		}
	}		
	////////////   TARGET SECTION : END //////////////



	////////////   ROLLOVER SECTION : START //////////////
	public void Move_Leds_To_Right(){															// --> use for Rollover_type_3. Leds move from Left to the right when the player press Left Flipper 
		Rollover_StopMoving = false;
		LedTmp = new int[obj_Grp_1.Length];
		for(var i =0;i<obj_Grp_1.Length;i++){							    			// Record the new possition of the leds.
			LedTmp[i] = arr_led_State[(i+obj_Grp_1.Length-1)%obj_Grp_1.Length];
		}

		for(var i =0;i<LedTmp.Length;i++){														// Save the leds state on the array				    
			arr_led_State[i] = LedTmp[i];
		}

		F_LedState();																		// Switch On/Off the leds
	}
	public void Move_Leds_To_Left(){															// --> use for Rollover_type_3. Leds move from Right to the Left when the player press Right Flipper 
		Rollover_StopMoving = false;
		LedTmp = new int[obj_Grp_1.Length];

		for(var i =0;i<obj_Grp_1.Length;i++){							    			// Record the new possition of the leds.
			LedTmp[i] = arr_led_State[(i+1)%obj_Grp_1.Length];
		}

		for(var i=0;i<LedTmp.Length;i++){														// Save the leds state on the array	
			arr_led_State[i] = LedTmp[i];
		}

		F_LedState();																		// Switch On/Off the leds
	}
	public void F_LedState(){																	// --> Use to switch on or switch Off the leds from arr_led_State
		for(var i =0;i<obj_Grp_1.Length;i++){
			if(arr_led_State[i] == 0){
				Led_Renderer[i].F_ChangeSprite_Off();										// Led : Off							
			}
			else{
				Led_Renderer[i].F_ChangeSprite_On();										// Led : On			
			}
		}
		Rollover_StopMoving = true;
	}
	public void Txt_Rollover_type_3(){															// --> use for Rollover on obj_Gpr1. Display Mission_Txt[11] on LCD screen 
		if(Mission_Txt.Length > 11 && obj_Grp_1.Length>1 && Mission_Txt[11]!=""){			// -> use for Rollover_type_3.			
			string tmp_Txt = "";
			for(var i =0;i<obj_Grp_1.Length;i++){							
				if(arr_led_State[i] == 0)													// if led state == 0 the letter is display with this color 
					tmp_Txt += 	"<color=#FF640078>" 
						+ Mission_Txt[11][i] + "</color>";					
				else																		// if led state == 1 the letter is display with this color 
					tmp_Txt += 	"<color=#FF6400FF>" 
						+ Mission_Txt[11][i] + "</color>";		
			}
			gameManager.Add_Info_To_Array(tmp_Txt, 3); 										// Write a text on LCD Screen	
		}	
		else if(Mission_Txt.Length > 11 && obj_Grp_1.Length==1){							// -> use for Rollover_No_Order_Grp_1 and Rollover_Order_Grp_1.
			string tmp_Txt = "";
			for(var i = 0;i<obj_Grp_1_Leds.Length;i++){
				if(arr_led_State[i] == 0)													// if led state == 0 the letter is display with this color 
					tmp_Txt += 	"<color=#FF640078>" 
						+ Mission_Txt[11][i] + "</color>";					
				else																		// if led state == 1 the letter is display with this color 
					tmp_Txt += 	"<color=#FF6400FF>" 
						+ Mission_Txt[11][i] + "</color>";		
			}
			gameManager.Add_Info_To_Array(tmp_Txt, 3); 										// Write a text on LCD Screen	
		}		
	}


	// PART 1
	public void Part_1_Type_Rollover_Gpr1(int num){											// --> Part 1
		if(Step < HowManyTime_Gpr1 ){
			for(var i =0;i<obj_Grp_1.Length;i++){	
				if(Rollover_Grp_1.Length == 1 && obj_Grp_1_Leds.Length > 0 					// -> Case 1
					&& Rollover_No_Order_Grp_1 &&  Rollover_Grp_1[i].index_info() == num){ 	
					if(arr_led_State.Count > F_Led_Gpr1_num(Step%obj_Grp_1.Length)){		// if the rollover as a led attached
						arr_led_State[F_Led_Gpr1_num(Step)]= 1;								// You want switch off the leds
						Led_Renderer[F_Led_Gpr1_num(Step)].F_ChangeSprite_On();				// Led : Off	
					}
					Step++;																	// Next step	

					if(SpecificText)Txt_Rollover_type_3();									// Write a text on LCD Screen
					else if(Mission_Txt.Length > 4 && Mission_Txt[4]!=""){					
						gameManager.Add_Info_To_Array(Mission_Txt_name + "\n" 
							+ Mission_Txt[4] + (HowManyTime_Gpr1 - Step).ToString() , 3);
					}

					if(Step < HowManyTime_Gpr1)Play_LedAnim_ObjAnim_LCDAnim_Part1();		// Play led animation, toy animation or lcd animation

					if(b_Mission_SkillShot && Rollover_Grp_1[i].index_info() 				// -> If the skill shot is enabled end the player has shot the good target
						== Skillshot_Target_num){				
						Skillshot_Mission_Complete();									// Skill Shot Complete
					}	
					break;
				}		
				else if(Rollover_No_Order_Grp_1 &&  Rollover_Grp_1[i].index_info() == num){	// -> Case 2
					Step++;																	// Next step	

					if(SpecificText)Txt_Rollover_type_3();									// Write a text on LCD Screen
					else if(Mission_Txt.Length > 4 && Mission_Txt[4]!=""){							
						gameManager.Add_Info_To_Array(Mission_Txt_name + "\n" + Mission_Txt[4] + (HowManyTime_Gpr1 - Step).ToString() , 3);
					}


					if(Step < HowManyTime_Gpr1)Play_LedAnim_ObjAnim_LCDAnim_Part1();		// Play led animation, toy animation or lcd animation

					if(b_Mission_SkillShot && Rollover_Grp_1[i].index_info() 				// -> If the skill shot is enabled end the player has shot the good target
						== Skillshot_Target_num){				
						Skillshot_Mission_Complete();									// Skill Shot Complete
					}	
					break;
				}										
				else if(Rollover_Order_Grp_1 												// -> Case 3 
					&&  Rollover_Grp_1[Step%obj_Grp_1.Length].index_info() == num){		
					arr_obj_Grp_1_State[i%obj_Grp_1.Length] = 1;							

					if(arr_led_State.Count > F_Led_Gpr1_num(Step%obj_Grp_1.Length)){		// if the rollover as a led attached
						arr_led_State[F_Led_Gpr1_num(Step%obj_Grp_1.Length)]= 0;			// You want switch off the leds
						Led_Renderer[F_Led_Gpr1_num(Step%obj_Grp_1.Length)].F_ChangeSprite_Off();// Led : Off	
					}
					Step++;																	// Next step

					if(SpecificText)Txt_Rollover_type_3();									// Write a text on LCD Screen
					else if(Mission_Txt.Length > 4 && Mission_Txt[4]!=""){	
						gameManager.Add_Info_To_Array(Mission_Txt_name + "\n" 
							+ Mission_Txt[4] + (HowManyTime_Gpr1 - Step).ToString() , 3);
					}
					if(Step < HowManyTime_Gpr1){	
						arr_obj_Grp_1_State[Step%obj_Grp_1.Length]= 0;														
						if(obj_Grp_1_Leds.Length>Step%obj_Grp_1.Length){					// If a led is attached to the target
							arr_led_State[F_Led_Gpr1_num(Step%obj_Grp_1.Length)]= 1;		// Switch On the led
							Led_Renderer[F_Led_Gpr1_num(Step%obj_Grp_1.Length)].F_ChangeSprite_On();// Led : On
						}
					}


					if(Step < HowManyTime_Gpr1)Play_LedAnim_ObjAnim_LCDAnim_Part1();		// Play led animation, toy animation or lcd animation

					if(b_Mission_SkillShot && Rollover_Grp_1[i].index_info() 				// -> If the skill shot is enabled end the player has shot the good target
						== Skillshot_Target_num){				
						Skillshot_Mission_Complete();									// Skill Shot Complete
					}
					break;
				}
				else if(Rollover_Type3_Grp_1 												// -> Case 4 
					&& Rollover_Grp_1[i].index_info() == num && arr_led_State[i]==0){
					if(arr_led_State.Count > F_Led_Gpr1_num(Step)){						// if the rollover as a led attached
						arr_led_State[i]= 1;												// You want switch off the leds
						Led_Renderer[i].F_ChangeSprite_On();								// Led : On	
					}
					Step++;																	// Next step	
					if(SpecificText)Txt_Rollover_type_3();									// Write a text on LCD Screen
					else if(Mission_Txt.Length > 4 && Mission_Txt[4]!=""){		
						gameManager.Add_Info_To_Array(Mission_Txt_name + "\n" 
							+ Mission_Txt[4] + (HowManyTime_Gpr1 - Step).ToString() , 3);
					}

					if(Step < HowManyTime_Gpr1)Play_LedAnim_ObjAnim_LCDAnim_Part1();		// Play led animation, toy animation or lcd animation

					if(b_Mission_SkillShot && Rollover_Grp_1[i].index_info() 				// -> If the skill shot is enabled end the player has shot the good target
						== Skillshot_Target_num){				
						Skillshot_Mission_Complete();									// Skill Shot Complete
					}	
					break;
				}																																																																																									// Next step
			}
		}
	}

	public void Mission_Intialisation_Rollover_Gp1(){											// --> Rollover : Init Gpr_1
		for(var j =0;j<obj_Grp_1_Leds.Length;j++){										
			arr_led_State[j]= 0;															// You want switch Off the leds
			Led_Renderer[j].F_ChangeSprite_Off();											// Led : Off	
		}
		if(Led_Part1_InProgress){
			Led_Part1_InProgress_State = 1;
			led_Part1_InProgress_.led_Part_InProgress_State(1);
			led_Part1_InProgress_.F_ChangeSprite_On();
		}

		if(Rollover_Grp_1.Length == 1 && Rollover_No_Order_Grp_1){							// --> Rollover_No_Order_Grp_1 	
			for(var i = 0;i < obj_Grp_1_Leds.Length;i++){								// init obj_Grp_1_Leds state 
				arr_led_State[F_Led_Gpr1_num(i)]= 0;										// You want switch on the leds
				Led_Renderer[F_Led_Gpr1_num(i)].F_ChangeSprite_Off();						// Led : Off
			}
		}	
		else if(Rollover_No_Order_Grp_1){													// --> Rollover_No_Order_Grp_1 	
			for(var i = 0;i < obj_Grp_1_Leds.Length;i++){										// init obj_Grp_1_Leds state 
				arr_led_State[F_Led_Gpr1_num(i)]= 1;										// You want switch on the leds
				if(arr_led_State[F_Led_Gpr1_num(i)] == 1) Led_Renderer[F_Led_Gpr1_num(i)].F_ChangeSprite_On();	// Led : On
				else Led_Renderer[F_Led_Gpr1_num(i)].F_ChangeSprite_Off();										// Led : Off
			}
		}	
		else if(Rollover_Order_Grp_1){														// --> Rollover_Order_Grp_1
			for(var i = 0;i < obj_Grp_1_Leds.Length;i++){										// init obj_Grp_1_Leds state 
				if(i == 0){
					arr_led_State[F_Led_Gpr1_num(0)]= 1;
					Led_Renderer[F_Led_Gpr1_num(0)].F_ChangeSprite_On();					// Led : On
				}
				else{
					arr_led_State[F_Led_Gpr1_num(i)]= 0;										
					Led_Renderer[F_Led_Gpr1_num(i)].F_ChangeSprite_Off();					// Led : Off
				}																				
			}
		}
		else if(Rollover_Type3_Grp_1){}
	}

	public void Pause_Stop_Rollover_Gpr1(){												// --> 
		for(var i = 0;i < obj_Grp_1_Leds.Length;i++){								// init obj_Grp_1_Leds state 
			if(arr_led_State[F_Led_Gpr1_num(i)] == 1) Led_Renderer[F_Led_Gpr1_num(i)].F_ChangeSprite_On();	// Led : On
			else Led_Renderer[F_Led_Gpr1_num(i)].F_ChangeSprite_Off();										// Led : Off
		}
		if(Led_Part1_InProgress){
			if(Led_Part1_InProgress_State == 0){
				led_Part1_InProgress_.F_ChangeSprite_Off();
			}
			else{
				Led_Part1_InProgress_State = 1;
				led_Part1_InProgress_.led_Part_InProgress_State(1);
				led_Part1_InProgress_.F_ChangeSprite_On();
			}
		}
	}


	// PART 2
	public void Part_2_Type_Rollover_Gpr2(int num){										// --> Rollover Part 2
		if(Step == HowManyTime_Gpr1){
			if(b_PauseMissionMode)gameManager.Start_Pause_Mode(missionIndex.F_index());
			for(var i = 0;i < obj_Grp_1_Leds.Length;i++){							// init obj_Grp_1_Leds state 
				if(KeepLedGrp1OnDuringMission){											// Keep Led On from obj_Grp1_Leds during mission progress
					arr_led_State[F_Led_Gpr1_num(i)]= 1;										
					Led_Renderer[F_Led_Gpr1_num(i)].F_ChangeSprite_On();
				}
				else{																	// Switch Off leds from obj_Grp1_Leds
					arr_led_State[F_Led_Gpr1_num(i)]= 0;										
					Led_Renderer[F_Led_Gpr1_num(i)].F_ChangeSprite_Off();
				}
			}

			if(Rollover_No_Order_Grp_2 && Rollover_Grp_2.Length == 1){					// -> Rollover_No_Order_Grp_2 : Case 1 :  init obj_Grp_2_Leds state 
				for(var i = 0;i < obj_Grp_2_Leds.Length;i++){								 
					arr_led_State[F_Led_Gpr2_num(i)]= 0;										
					Led_Renderer[F_Led_Gpr2_num(i)].F_ChangeSprite_Off();
				}
			}
			else if(Rollover_No_Order_Grp_2){											// -> Rollover_No_Order_Grp_2 : case 2 : init obj_Grp_2_Leds state 
				for(var i = 0;i < obj_Grp_2_Leds.Length;i++){								// init obj_Grp_1_Leds state 
					arr_led_State[F_Led_Gpr2_num(i)]= 1;										
					Led_Renderer[F_Led_Gpr2_num(i)].F_ChangeSprite_On();
				}
			}
			else if(Rollover_Order_Grp_2){												// -> Rollover_Order_Grp_2 : case 3 : init obj_Grp_2_Leds state 
				for(var i = 0;i < obj_Grp_2_Leds.Length;i++){								
					if(i==0){
						arr_led_State[F_Led_Gpr2_num(i)]= 1;										
						Led_Renderer[F_Led_Gpr2_num(i)].F_ChangeSprite_On();
					}
					else{
						arr_led_State[F_Led_Gpr2_num(i)]= 0;										
						Led_Renderer[F_Led_Gpr2_num(i)].F_ChangeSprite_Off();
					}
				}
			}


			if(Led_Part1_InProgress){													// -> init Led_Part1_InProgress
				Led_Part1_InProgress_State = 0;
				led_Part1_InProgress_.led_Part_InProgress_State(0);
				led_Part1_InProgress_.F_ChangeSprite_Off();
			}

			if(Led_Mission_InProgress){													// -> init Led_Mission_InProgress
				Led_Mission_InProgress_State = 1;
				led_Mission_InProgress_.led_Part_InProgress_State(1);
				led_Mission_InProgress_.F_ChangeSprite_On();	

			}
			if(b_Mission_Timer || b_Mission_Multi_Timer){MissionTimer(Mission_Time);}	// -> Init Timer with a value						

			if(multiBall){																// -> Start multiball if multiBall = true									
				gameManager.F_Mission_MultiBall(missionIndex.F_index(),numberOfBall);
				if(Rollover_No_Order_Grp_2){
					for(var i = 0;i < obj_Grp_2_Leds.Length;i++){							// init obj_Grp_1_Leds state 
						arr_led_State[F_Led_Gpr2_num(i)]= 1;										
						Led_Renderer[F_Led_Gpr2_num(i)].F_ChangeSprite_On();
					}
				}
				else{
					arr_led_State[F_Led_Gpr2_num(0)]= 1;										
					Led_Renderer[F_Led_Gpr2_num(0)].F_ChangeSprite_On();
				}
			}

			if(Mission_Txt.Length > 13 && Mission_Txt[13]!="")							// Write a text on LCD Screen
				gameManager.Add_Info_To_Array(Mission_Txt_name + "\n" + Mission_Txt[13],3);

			Step++;																		// Next Step
			Play_LedAnim_ObjAnim_LCDAnim_Part2();										// Play led animation, toy animation or lcd animation
		}
	}


	// PART 3
	public void Part_3_Type_Rollover_Gpr2(int num){										// Part 3 : Rollover
		for(var i = 0;i < obj_Grp_2.Length;i++){
			if(Step > HowManyTime_Gpr1 && Step <= HowManyTime_Gpr1+HowManyTime_Gpr2 ){


				if(multiBall){															// if mode multi ball true
					if(Rollover_No_Order_Grp_2 && Rollover_Grp_2[i].index_info() == num){// Case 1
						if(b_Mission_Multi_Timer){MissionTimer(Mission_Time);}			// Init Timer with a value
						if(Mission_Txt.Length > 12 && Mission_Txt[12]!="")				// Write a text on LCD Screen
							gameManager.Add_Info_To_Array(Mission_Txt_name 
								+ "\n" + Mission_Txt[12] + " : " + JackpotPoints, 2);
						gameManager.Add_Score(JackpotPoints);							// Add Points
						if(Step < HowManyTime_Gpr1+HowManyTime_Gpr2 )
							Play_LedAnim_ObjAnim_LCDAnim_Part3();							// Play led animation, toy animation or lcd animation
						break;			
					}	
					else if(Rollover_Order_Grp_2 && Rollover_Grp_2[(Step_MultiBall-1-HowManyTime_Gpr1)%obj_Grp_2.Length].index_info() == num){	// Case 2
						if(b_Mission_Multi_Timer){MissionTimer(Mission_Time);}																	// Init Timer with a value	
						if(obj_Grp_2_Leds.Length>(Step_MultiBall-1-HowManyTime_Gpr1)%obj_Grp_2.Length){											// if the rollover as a led attached
							arr_led_State[obj_Grp_1_Leds.Length+(Step_MultiBall-1-HowManyTime_Gpr1)%obj_Grp_2.Length]= 0;						// You want switch off the leds
							Led_Renderer[obj_Grp_1_Leds.Length+(Step_MultiBall-1-HowManyTime_Gpr1)%obj_Grp_2.Length].F_ChangeSprite_Off();		// Led : Off	
						}
						Step_MultiBall++;												// Next step
						if(Mission_Txt.Length > 12 && Mission_Txt[12]!="")				// Write a text on LCD Screen
							gameManager.Add_Info_To_Array(Mission_Txt_name 
								+ "\n" + Mission_Txt[12] + " : " + JackpotPoints, 2);
						gameManager.Add_Score(JackpotPoints);							// Add Points

						if(obj_Grp_2_Leds.Length>(Step_MultiBall-1-HowManyTime_Gpr1)%obj_Grp_2.Length){
							arr_led_State[obj_Grp_1_Leds.Length+(Step_MultiBall-1-HowManyTime_Gpr1)%obj_Grp_2.Length]= 1;						// Switch On the led
							Led_Renderer[obj_Grp_1_Leds.Length+(Step_MultiBall-1-HowManyTime_Gpr1)%obj_Grp_2.Length].F_ChangeSprite_On();		// Led : On
						}

						if(Step < HowManyTime_Gpr1+HowManyTime_Gpr2 )
							Play_LedAnim_ObjAnim_LCDAnim_Part3();							// Play led animation, toy animation or lcd animation
						break;	
					}
				}
				else{																	// --> if mode multi ball = false
					if(Rollover_Grp_2.Length == 1 && Rollover_No_Order_Grp_2 			// -> case 1
						&&  Rollover_Grp_2[(Step-1-HowManyTime_Gpr1)%obj_Grp_2.Length].index_info() == num){	// -> if there is only one rollover on Rollover_Grp_2. 
						if(b_Mission_Multi_Timer){MissionTimer(Mission_Time);}									// Init Timer with a value
						if(obj_Grp_2_Leds.Length>(Step-1-HowManyTime_Gpr1)){									// if the rollover as a led attached
							arr_led_State[obj_Grp_1_Leds.Length+(Step-1-HowManyTime_Gpr1)]= 1;					// You want switch off the leds
							Led_Renderer[obj_Grp_1_Leds.Length+(Step-1-HowManyTime_Gpr1)].F_ChangeSprite_On();	// Led : Off	
						}
						Step++;															// Next step

						if(Mission_Txt.Length > 5 && Mission_Txt[5]!=""){				// Write a text on LCD Screen
							gameManager.Add_Info_To_Array(Mission_Txt_name + "\n" 
								+ Mission_Txt[5] + (HowManyTime_Gpr1+HowManyTime_Gpr2+1 - Step).ToString() , 3);
						}

						if(Step < HowManyTime_Gpr1+HowManyTime_Gpr2 )
							Play_LedAnim_ObjAnim_LCDAnim_Part3();							// Play led animation, toy animation or lcd animation
						break;	
					}	
					else if(Rollover_No_Order_Grp_2 && Rollover_Grp_2[i].index_info() == num){	// -> case 2
						if(b_Mission_Multi_Timer){MissionTimer(Mission_Time);}					// Init Timer with a value
						Step++;																	// Next step

						if(Mission_Txt.Length > 5 && Mission_Txt[5]!=""){						// Write a text on LCD Screen
							gameManager.Add_Info_To_Array(Mission_Txt_name + "\n" 
								+ Mission_Txt[5] + (HowManyTime_Gpr1+HowManyTime_Gpr2+1 - Step).ToString() , 3);
						}

						if(Step < HowManyTime_Gpr1+HowManyTime_Gpr2 )
							Play_LedAnim_ObjAnim_LCDAnim_Part3();							// Play led animation, toy animation or lcd animation
						break;			
					}										
					else if(Rollover_Order_Grp_2 &&  Rollover_Grp_2[(Step-1-HowManyTime_Gpr1)%obj_Grp_2.Length].index_info() == num){	// -> case 3	
						if(b_Mission_Multi_Timer){MissionTimer(Mission_Time);}															// Init Timer with a value	
						if(obj_Grp_2_Leds.Length>(Step-1-HowManyTime_Gpr1)%obj_Grp_2.Length){											// if the rollover as a led attached
							arr_led_State[obj_Grp_1_Leds.Length+(Step-1-HowManyTime_Gpr1)%obj_Grp_2.Length]= 0;							// You want switch off the leds
							Led_Renderer[obj_Grp_1_Leds.Length+(Step-1-HowManyTime_Gpr1)%obj_Grp_2.Length].F_ChangeSprite_Off();		// Led : Off	
						}
						Step++;															// Next step

						if(Mission_Txt.Length > 5 && Mission_Txt[5]!=""){				// Write a text on LCD Screen
							gameManager.Add_Info_To_Array(Mission_Txt_name + "\n" 
								+ Mission_Txt[5] + (HowManyTime_Gpr1+HowManyTime_Gpr2+1 - Step).ToString() , 3);
						}
						// Play led animation, toy animation or lcd animation

						if(obj_Grp_2_Leds.Length>(Step-1-HowManyTime_Gpr1)%obj_Grp_2.Length){
							arr_led_State[obj_Grp_1_Leds.Length+(Step-1-HowManyTime_Gpr1)%obj_Grp_2.Length]= 1;					// Switch On the led
							Led_Renderer[obj_Grp_1_Leds.Length+(Step-1-HowManyTime_Gpr1)%obj_Grp_2.Length].F_ChangeSprite_On();	// Led : On
						}

						if(Step < HowManyTime_Gpr1+HowManyTime_Gpr2 )
							Play_LedAnim_ObjAnim_LCDAnim_Part3();							// Play led animation, toy animation or lcd animation
						break;	
					}
				}
			}
		}
	}


	// PART INITIALISATION
	public void Mission_Intialisation_Rollover_Gp2(){										// --> Init Rollover Grp2
		if(Led_Mission_InProgress){														// init Led_Mission_InProgress
			led_Mission_InProgress_.F_ChangeSprite_Off();
			led_Mission_InProgress_.led_Part_InProgress_State(0);
			Led_Mission_InProgress_State = 0;
		}

		for(var j =obj_Grp_1_Leds.Length;j<obj_Grp_1_Leds.Length+obj_Grp_2_Leds.Length;j++){	
			arr_led_State[j]= 0;														// You want switch Off the leds
			Led_Renderer[j].F_ChangeSprite_Off();										// Led : Off	
		}
	}


	public void Pause_Stop_Rollover_Gpr2(){												// -->
		for(var i = 0;i < obj_Grp_2_Leds.Length;i++){								
			if(arr_led_State[F_Led_Gpr2_num(i)] == 1) Led_Renderer[F_Led_Gpr2_num(i)].F_ChangeSprite_On();	// Led : On
			else Led_Renderer[F_Led_Gpr2_num(i)].F_ChangeSprite_Off();										// Led : Off
		}
		if(Led_Mission_InProgress){														// init Led_Mission_InProgress
			if(Led_Mission_InProgress_State == 0){
				led_Mission_InProgress_.F_ChangeSprite_Off();
			}
			else{
				Led_Mission_InProgress_State = 1;
				led_Mission_InProgress_.led_Part_InProgress_State(1);
				led_Mission_InProgress_.F_ChangeSprite_On();
			}
		}	
	}
	////////////   ROLLOVER SECTION : END //////////////



	////////////   BUMPER SECTION : START //////////////

	// PART 1
	public void Part_1_Type_Bumper_Gpr1(int num){													// --> Bumper Section Part 1
		for(var i = 0;i < obj_Grp_1.Length;i++){			
			if(Bumper_Grp_1[i].index_info() == num){
				if(Step < HowManyTime_Gpr1){
					if(arr_led_State.Count >= HowManyTime_Gpr1 && obj_Grp_1_Leds.Length > 0 ){		// if the Bumper as leds attached
						arr_led_State[Step]= 1;														// You want switch On the leds
						Led_Renderer[Step].F_ChangeSprite_On();										// Led : On	
					}

					Step++;																			// Next step

					if(Mission_Txt.Length > 4 && Mission_Txt[4]!=""){								// Write a text on LCD Screen
						gameManager.Add_Info_To_Array(Mission_Txt_name + "\n" + Mission_Txt[4] + (HowManyTime_Gpr1 - Step).ToString() , 3);
					}

					if(Step < HowManyTime_Gpr1)Play_LedAnim_ObjAnim_LCDAnim_Part1();				// Play led animation, toy animation or lcd animation

					if(b_Mission_SkillShot && Bumper_Grp_1[i].index_info() 							// -> If the skill shot is enabled end the player has shot the good target
						== Skillshot_Target_num){
						Skillshot_Mission_Complete();	
					}
				}
			}
		}
	}

	public void F_First_Bumper_Grp1(){																	// --> Init Bumper Grp1
		Bumper_Grp_1 = new Bumper_js[obj_Grp_1.Length];
		for(var i =0;i<obj_Grp_1.Length;i++){															
			Bumper_Grp_1[i] = obj_Grp_1[i].GetComponent<Bumper_js>();								// Connect the Mission to obj_Grp_1[i].GetComponent<Bumper_js>();	 component
		}
	}
	public void F_First_Bumper_Grp2(){																	// --> Init Bumper Grp2
		Bumper_Grp_2 = new Bumper_js[obj_Grp_2.Length];
		for(var i =0;i<obj_Grp_2.Length;i++){															
			Bumper_Grp_2[i] = obj_Grp_2[i].GetComponent<Bumper_js>();								// Connect the Mission to obj_Grp_1[i].GetComponent<Bumper_js>();	 component
		}
	}

	public void Mission_Intialisation_Bumper_Gp1(){													// Init Led Mission
		for(var j =0;j<obj_Grp_1_Leds.Length;j++){												// Init Gpr_1
			arr_led_State[j]= 0;																	// You want switch Off the leds
			Led_Renderer[j].F_ChangeSprite_Off();													// Led : Off	
		}
		if(Led_Part1_InProgress){																	// init Led_Part1_InProgress
			Led_Part1_InProgress_State = 1;
			led_Part1_InProgress_.led_Part_InProgress_State(1);
			led_Part1_InProgress_.F_ChangeSprite_On();
		}
	}

	public void Pause_Stop_Bumper_Gpr1(){																// --> 
		for(var i = 0;i < obj_Grp_1_Leds.Length;i++){											// init obj_Grp_1_Leds state 
			if(arr_led_State[F_Led_Gpr1_num(i)] == 1) Led_Renderer[F_Led_Gpr1_num(i)].F_ChangeSprite_On();	// Led : On
			else Led_Renderer[F_Led_Gpr1_num(i)].F_ChangeSprite_Off();										// Led : Off
		}
		if(Led_Part1_InProgress){																	// init Led_Part1_InProgress
			if(Led_Part1_InProgress_State == 0){
				led_Part1_InProgress_.F_ChangeSprite_Off();
			}
			else{
				Led_Part1_InProgress_State = 1;
				led_Part1_InProgress_.led_Part_InProgress_State(1);
				led_Part1_InProgress_.F_ChangeSprite_On();
			}
		}
	}

	// PART 2
	public void Part_2_Type_Bumper_Gpr2(int num){													// --> Bumper Part 2
		if(Step == HowManyTime_Gpr1){
			if(b_PauseMissionMode)gameManager.Start_Pause_Mode(missionIndex.F_index());				// Pause Mission
			for(var i = 0;i < obj_Grp_1_Leds.Length;i++){										// init obj_Grp_1_Leds state 
				if(KeepLedGrp1OnDuringMission){														// Keep Led Grp1 On During Mission if true
					arr_led_State[F_Led_Gpr1_num(i)]= 1;										
					Led_Renderer[F_Led_Gpr1_num(i)].F_ChangeSprite_On();
				}
				else{
					arr_led_State[F_Led_Gpr1_num(i)]= 0;										
					Led_Renderer[F_Led_Gpr1_num(i)].F_ChangeSprite_Off();
				}
			}

			if(Led_Part1_InProgress){																// init Led_Part1_InProgress
				Led_Part1_InProgress_State = 0;
				led_Part1_InProgress_.led_Part_InProgress_State(0);
				led_Part1_InProgress_.F_ChangeSprite_Off();
			}
			if(Led_Mission_InProgress){																// init Led_Mission_InProgress
				Led_Mission_InProgress_State = 1;
				led_Mission_InProgress_.led_Part_InProgress_State(1);
				led_Mission_InProgress_.F_ChangeSprite_On();	
			}
			if(b_Mission_Timer || b_Mission_Multi_Timer){MissionTimer(Mission_Time);}				// Init Timer with a value						

			if(multiBall){																			// Start multiball if multiBall = true									
				gameManager.F_Mission_MultiBall(missionIndex.F_index(),numberOfBall);
			}

			if(Mission_Txt.Length > 13																// Write a text on LCD Screen
				&& Mission_Txt[13]!="")gameManager.Add_Info_To_Array(Mission_Txt_name + "\n" + Mission_Txt[13],3);

			Step++;
			Play_LedAnim_ObjAnim_LCDAnim_Part2();
		}
	}


	// PART 3
	public void Part_3_Type_Bumper_Gpr2(int num){													// Bumper Part 3
		for(var i = 0;i < obj_Grp_2.Length;i++){
			if(Bumper_Grp_2[i].index_info() == num){
				if(Step > HowManyTime_Gpr1 && Step < (HowManyTime_Gpr1 + HowManyTime_Gpr2+1)){	
					if(b_Mission_Multi_Timer){MissionTimer(Mission_Time);}								// Init Timer with a value
					if(obj_Grp_2_Leds.Length>(Step-1-HowManyTime_Gpr1)){							// if the rollover as a led attached
						arr_led_State[obj_Grp_1_Leds.Length+(Step-1-HowManyTime_Gpr1)]= 1;			// You want switch off the leds
						Led_Renderer[obj_Grp_1_Leds.Length+(Step-1-HowManyTime_Gpr1)].F_ChangeSprite_On();	// Led : Off	
					}

					Step++;																			// Next step

					if(Mission_Txt.Length > 5 && Mission_Txt[5]!=""){								// Write a text on LCD Screen
						gameManager.Add_Info_To_Array(Mission_Txt_name + "\n" + Mission_Txt[5] + (HowManyTime_Gpr1+HowManyTime_Gpr2+1 - Step).ToString() , 3);
					}

					if(Step < (HowManyTime_Gpr1 + HowManyTime_Gpr2))
						Play_LedAnim_ObjAnim_LCDAnim_Part3();											// Play led animation, toy animation or lcd animation

					break;
				}
			}
		}
	}

	// PART INITIALISATION
	public void Mission_Intialisation_Bumper_Gp2(){													// --> Init Leds Mission Bumper
		if(Led_Mission_InProgress){																	// init Led_Mission_InProgress
			led_Mission_InProgress_.F_ChangeSprite_Off();
			Led_Mission_InProgress_State = 0;
			led_Mission_InProgress_.led_Part_InProgress_State(0);
		}

		for(var j =obj_Grp_1_Leds.Length;j<obj_Grp_1_Leds.Length+obj_Grp_2_Leds.Length;j++){	// init Led
			arr_led_State[j]= 0;																	// You want switch Off the leds
			Led_Renderer[j].F_ChangeSprite_Off();													// Led : Off	
		}
	}


	public void Pause_Stop_Bumper_Gpr2(){																// --> 
		for(var i = 0;i < obj_Grp_2_Leds.Length;i++){											// init obj_Grp_1_Leds state 
			if(arr_led_State[F_Led_Gpr2_num(i)] == 1) Led_Renderer[F_Led_Gpr2_num(i)].F_ChangeSprite_On();	// Led : On
			else Led_Renderer[F_Led_Gpr2_num(i)].F_ChangeSprite_Off();										// Led : Off
		}
		if(Led_Mission_InProgress){																	// init Led_Mission_InProgress
			if(Led_Mission_InProgress_State == 0){
				led_Mission_InProgress_.F_ChangeSprite_Off();
			}
			else{																					 
				Led_Mission_InProgress_State = 1;
				led_Mission_InProgress_.led_Part_InProgress_State(1);
				led_Mission_InProgress_.F_ChangeSprite_On();
			}
		}	
	}



	////////////   SPINNER SECTION : START //////////////
	// PART 1
	public void Part_1_Type_Spinner_Gpr1(int num){												// --> Spinner Part 1

		for(var i = 0;i < obj_Grp_1.Length;i++){			
			if(Spinner_Grp_1[i].index_info() == num){
				if(Step < HowManyTime_Gpr1 ){
					if(arr_led_State.Count >= HowManyTime_Gpr1 && obj_Grp_1_Leds.Length > 0 ){	// if the Spinner as leds attached
						arr_led_State[Step]= 1;													// You want switch On the leds
						Led_Renderer[Step].F_ChangeSprite_On();						// Led : On	
					}
					Step++;																		// Next step

					if(Mission_Txt.Length > 4 && Mission_Txt[4]!=""){							// Write a text on LCD Screen
						gameManager.Add_Info_To_Array(Mission_Txt_name + "\n" 
							+ Mission_Txt[4] + (HowManyTime_Gpr1 - Step).ToString() , 3);
					}

					if(Step < HowManyTime_Gpr1)Play_LedAnim_ObjAnim_LCDAnim_Part1();			// Play led animation, toy animation or lcd animation

					if(b_Mission_SkillShot && Spinner_Grp_1[i].index_info() 					// -> If the skill shot is enabled end the player has shot the good target
						== Skillshot_Target_num){
						Skillshot_Mission_Complete();	
					}
				}
			}
		}
	}

	public void F_First_Spinner_Grp1(){															// init Spinner Grp1
		Spinner_Grp_1 = new Spinner_LapCounter[obj_Grp_1.Length];
		for(var i =0;i<obj_Grp_1.Length;i++){															
			Spinner_Grp_1[i] = obj_Grp_1[i].GetComponent<Spinner_LapCounter>();				// Connect the Mission to obj_Grp_1[i].GetComponent<Target>();	 component
		}
	}	
	public void F_First_Spinner_Grp2(){															// init Spinner Grp2
		Spinner_Grp_2 = new Spinner_LapCounter[obj_Grp_2.Length];
		for(var i =0;i<obj_Grp_2.Length;i++){															
			Spinner_Grp_2[i] = obj_Grp_2[i].GetComponent<Spinner_LapCounter>();				// Connect the Mission to obj_Grp_1[i].GetComponent<Target>();	 component
		}
	}

	public void Mission_Intialisation_Spinner_Gp1(){												// Init Mission Led Grp1
		for(var j =0;j<obj_Grp_1_Leds.Length;j++){											// Init Gpr_1
			arr_led_State[j]= 0;																// You want switch Off the leds
			Led_Renderer[j].F_ChangeSprite_Off();												// Led : Off	
		}
		if(Led_Part1_InProgress){																// init Led_Part1_InProgress
			Led_Part1_InProgress_State = 1;
			led_Part1_InProgress_.led_Part_InProgress_State(1);
			led_Part1_InProgress_.F_ChangeSprite_On();
		}
	}

	public void Pause_Stop_Spinner_Gpr1(){															// --> 
		for(var i  = 0;i < obj_Grp_1_Leds.Length;i++){										// init obj_Grp_1_Leds state 
			if(arr_led_State[F_Led_Gpr1_num(i)] == 1) Led_Renderer[F_Led_Gpr1_num(i)].F_ChangeSprite_On();	// Led : On
			else Led_Renderer[F_Led_Gpr1_num(i)].F_ChangeSprite_Off();										// Led : Off
		}
		if(Led_Part1_InProgress){																// init Led_Part1_InProgress
			if(Led_Part1_InProgress_State == 0){
				led_Part1_InProgress_.F_ChangeSprite_Off();
			}
			else{
				Led_Part1_InProgress_State = 1;
				led_Part1_InProgress_.led_Part_InProgress_State(1);
				led_Part1_InProgress_.F_ChangeSprite_On();
			}
		}
	}

	// PART 2
	public void Part_2_Type_Spinner_Gpr2(int num){												// --> Part 2
		if(Step == HowManyTime_Gpr1){
			if(b_PauseMissionMode)gameManager.Start_Pause_Mode(missionIndex.F_index());
			for(var i = 0;i < obj_Grp_1_Leds.Length;i++){									// init obj_Grp_1_Leds state 
				if(KeepLedGrp1OnDuringMission){													// Keep Led Grp1 On During Mission if true
					arr_led_State[F_Led_Gpr1_num(i)]= 1;										
					Led_Renderer[F_Led_Gpr1_num(i)].F_ChangeSprite_On();
				}
				else{
					arr_led_State[F_Led_Gpr1_num(i)]= 0;										
					Led_Renderer[F_Led_Gpr1_num(i)].F_ChangeSprite_Off();
				}
			}

			if(Led_Part1_InProgress){															// init Led_Part1_InProgress
				Led_Part1_InProgress_State = 0;
				led_Part1_InProgress_.led_Part_InProgress_State(0);
				led_Part1_InProgress_.F_ChangeSprite_Off();
			}
			if(Led_Mission_InProgress){															// init Led_Mission_InProgress
				Led_Mission_InProgress_State = 1;
				led_Mission_InProgress_.led_Part_InProgress_State(1);
				led_Mission_InProgress_.F_ChangeSprite_On();	
			}
			if(b_Mission_Timer || b_Mission_Multi_Timer){MissionTimer(Mission_Time);}			// Init Timer with a value						

			if(multiBall){																		// Start multiball if multiBall = true									
				gameManager.F_Mission_MultiBall(missionIndex.F_index(),numberOfBall);
			}

			if(Mission_Txt.Length > 13 && Mission_Txt[13]!="")									// Write a text on LCD Screen
				gameManager.Add_Info_To_Array(Mission_Txt_name + "\n" 
					+ Mission_Txt[13],3);

			Step++;																				// Next step
			Play_LedAnim_ObjAnim_LCDAnim_Part2();												// Play led animation, toy animation or lcd animation
		}
	}



	// PART 3
	public void Part_3_Type_Spinner_Gpr2(int num){												// Part 3

		for(var i = 0;i < obj_Grp_2.Length;i++){
			if(Spinner_Grp_2[i].index_info() == num){
				//Debug.Log(Spinner_Grp_2[i].index_info() + " : " + num);
				if(Step > HowManyTime_Gpr1 && Step < (HowManyTime_Gpr1 + HowManyTime_Gpr2+1)){	
					if(b_Mission_Multi_Timer){MissionTimer(Mission_Time);}						// Init Timer with a value
					if(obj_Grp_2_Leds.Length>(Step-1-HowManyTime_Gpr1)){						// if the spinner as a led attached
						arr_led_State[obj_Grp_1_Leds.Length+(Step-1-HowManyTime_Gpr1)]= 1;		// You want switch off the leds
						Led_Renderer[obj_Grp_1_Leds.Length+(Step-1-HowManyTime_Gpr1)].F_ChangeSprite_On();// Led : On	
					}

					Step++;																		// Next step

					if(Mission_Txt.Length > 5 && Mission_Txt[5]!=""){							// Write a text on LCD Screen
						gameManager.Add_Info_To_Array(Mission_Txt_name + "\n" 
							+ Mission_Txt[5] + (HowManyTime_Gpr1+HowManyTime_Gpr2+1 - Step).ToString() , 3);
					}

					if(Step < (HowManyTime_Gpr1 + HowManyTime_Gpr2))
						Play_LedAnim_ObjAnim_LCDAnim_Part3();										// Play led animation, toy animation or lcd animation

					break;
				}
			}
		}
	}

	// PART INITIALISATION	
	public void Mission_Intialisation_Spinner_Gp2(){												// --> Init Leds Mission Grp2
		if(Led_Mission_InProgress){																// init Led_Mission_InProgress
			led_Mission_InProgress_.F_ChangeSprite_Off();
			Led_Mission_InProgress_State = 0;
			led_Mission_InProgress_.led_Part_InProgress_State(0);
		}

		for(var j =obj_Grp_1_Leds.Length;j<obj_Grp_1_Leds.Length+obj_Grp_2_Leds.Length;j++){// Init 
			arr_led_State[j]= 0;																// You want switch Off the leds
			Led_Renderer[j].F_ChangeSprite_Off();												// Led : Off	
		}
	}


	public void Pause_Stop_Spinner_Gpr2(){															// --> 
		for(var i  = 0;i < obj_Grp_2_Leds.Length;i++){										// init obj_Grp_1_Leds state 
			if(arr_led_State[F_Led_Gpr2_num(i)] == 1) Led_Renderer[F_Led_Gpr2_num(i)].F_ChangeSprite_On();	// Led : On
			else Led_Renderer[F_Led_Gpr2_num(i)].F_ChangeSprite_Off();										// Led : Off
		}
		if(Led_Mission_InProgress){																// init Led_Mission_InProgress
			if(Led_Mission_InProgress_State == 0){
				led_Mission_InProgress_.F_ChangeSprite_Off();
			}
			else{
				Led_Mission_InProgress_State = 1;
				led_Mission_InProgress_.led_Part_InProgress_State(1);
				led_Mission_InProgress_.F_ChangeSprite_On();
			}
		}	
	}

	////////////   HOLE SECTION : START //////////////
	// PART 1
	public void Part_1_Type_Hole_Gpr1(int num){
		for(var i = 0;i < obj_Grp_1.Length;i++){			
			if(Hole_Grp_1[i].index_info() == num){
				if(Step < HowManyTime_Gpr1 ){
					if(arr_led_State.Count >= HowManyTime_Gpr1 && obj_Grp_1_Leds.Length > 0 ){								// if the Rollover as leds attached
						arr_led_State[Step]= 1;									// You want switch On the leds
						Led_Renderer[Step].F_ChangeSprite_On();					// Led : On	
					}
					Step++;																		// Next step

					if(Mission_Txt.Length > 4 && Mission_Txt[4]!=""){							// Write a text on LCD Screen
						gameManager.Add_Info_To_Array(Mission_Txt_name + "\n" + Mission_Txt[4] + (HowManyTime_Gpr1 - Step).ToString() , 3);
					}


					if(Step < HowManyTime_Gpr1)Play_LedAnim_ObjAnim_LCDAnim_Part1();

					if(b_Mission_SkillShot && Hole_Grp_1[i].index_info() 						// -> If the skill shot is enabled end the player has shot the good target
						== Skillshot_Target_num){
						Skillshot_Mission_Complete();	
					}
				}
			}
		}
	}

	public void F_First_Hole_Grp1(){
		Hole_Grp_1 = new Hole[obj_Grp_1.Length];
		for(var i =0;i<obj_Grp_1.Length;i++){															
			Hole_Grp_1[i] = obj_Grp_1[i].GetComponent<Hole>();								// Connect the Mission to obj_Grp_1[i].GetComponent<Target>();	 component
		}
	}
	public void F_First_Hole_Grp2(){
		Hole_Grp_2 = new Hole[obj_Grp_2.Length];
		for(var i =0;i<obj_Grp_2.Length;i++){															
			Hole_Grp_2[i] = obj_Grp_2[i].GetComponent<Hole>();								// Connect the Mission to obj_Grp_1[i].GetComponent<Target>();	 component
		}
	}

	public void Mission_Intialisation_Hole_Gp1(){
		for(var j =0;j<obj_Grp_1_Leds.Length;j++){										// Init Gpr_1
			arr_led_State[j]= 0;									// You want switch Off the leds
			Led_Renderer[j].F_ChangeSprite_Off();					// Led : Off	
		}
		if(Led_Part1_InProgress){
			Led_Part1_InProgress_State = 1;
			led_Part1_InProgress_.led_Part_InProgress_State(1);
			led_Part1_InProgress_.F_ChangeSprite_On();
		}
	}

	public void Pause_Stop_Hole_Gpr1(){												// --> 
		for(var i = 0;i < obj_Grp_1_Leds.Length;i++){								// init obj_Grp_1_Leds state 
			if(arr_led_State[F_Led_Gpr1_num(i)] == 1) Led_Renderer[F_Led_Gpr1_num(i)].F_ChangeSprite_On();	// Led : On
			else Led_Renderer[F_Led_Gpr1_num(i)].F_ChangeSprite_Off();										// Led : Off
		}
		if(Led_Part1_InProgress){
			if(Led_Part1_InProgress_State == 0){
				led_Part1_InProgress_.F_ChangeSprite_Off();
			}
			else{
				Led_Part1_InProgress_State = 1;
				led_Part1_InProgress_.led_Part_InProgress_State(1);
				led_Part1_InProgress_.F_ChangeSprite_On();
			}
		}
	}

	// PART 2
	public void Part_2_Type_Hole_Gpr2(int num){
		if(Step == HowManyTime_Gpr1){
			if(b_PauseMissionMode)gameManager.Start_Pause_Mode(missionIndex.F_index());

			for(var i = 0;i < obj_Grp_1_Leds.Length;i++){								// init obj_Grp_1_Leds state 
				if(KeepLedGrp1OnDuringMission){
					arr_led_State[F_Led_Gpr1_num(i)]= 1;										
					Led_Renderer[F_Led_Gpr1_num(i)].F_ChangeSprite_On();
				}
				else{
					arr_led_State[F_Led_Gpr1_num(i)]= 0;										
					Led_Renderer[F_Led_Gpr1_num(i)].F_ChangeSprite_Off();
				}
			}

			if(Led_Part1_InProgress){
				Led_Part1_InProgress_State = 0;
				led_Part1_InProgress_.led_Part_InProgress_State(0);
				led_Part1_InProgress_.F_ChangeSprite_Off();
			}

			if(Led_Mission_InProgress){
				Led_Mission_InProgress_State = 1;
				led_Mission_InProgress_.led_Part_InProgress_State(1);
				led_Mission_InProgress_.F_ChangeSprite_On();	
			}
			if(b_Mission_Timer || b_Mission_Multi_Timer){MissionTimer(Mission_Time);}	// Init Timer with a value						

			if(multiBall){																// Start multiball if multiBall = true									
				gameManager.F_Mission_MultiBall(missionIndex.F_index(),numberOfBall);
			}

			if(Mission_Txt.Length > 13
				&& Mission_Txt[13]!="")gameManager.Add_Info_To_Array(Mission_Txt_name + "\n" + Mission_Txt[13],3);

			Step++;
			Play_LedAnim_ObjAnim_LCDAnim_Part2();
		}
	}


	// PART 3
	public void Part_3_Type_Hole_Gpr2(int num){
		for(var i  = 0;i < obj_Grp_2.Length;i++){
			if(Hole_Grp_2[i].index_info() == num){
				if(Step > HowManyTime_Gpr1 && Step < (HowManyTime_Gpr1 + HowManyTime_Gpr2+1)){	
					if(b_Mission_Multi_Timer){MissionTimer(Mission_Time);}				// Init Timer with a value

					if(obj_Grp_2_Leds.Length>(Step-1-HowManyTime_Gpr1)){											// if the rollover as a led attached
						arr_led_State[obj_Grp_1_Leds.Length+(Step-1-HowManyTime_Gpr1)]= 1;							// You want switch off the leds
						Led_Renderer[obj_Grp_1_Leds.Length+(Step-1-HowManyTime_Gpr1)].F_ChangeSprite_On();			// Led : Off	
					}

					Step++;																		// Next step

					if(Mission_Txt.Length > 5 && Mission_Txt[5]!=""){							// Write a text on LCD Screen
						gameManager.Add_Info_To_Array(Mission_Txt_name + "\n" + Mission_Txt[5] + (HowManyTime_Gpr1+HowManyTime_Gpr2+1 - Step).ToString() , 3);
					}

					if(Step < (HowManyTime_Gpr1 + HowManyTime_Gpr2))
						Play_LedAnim_ObjAnim_LCDAnim_Part3();
					break;
				}
			}
		}
	}

	// PART INITIALISATION
	public void Mission_Intialisation_Hole_Gp2(){
		if(Led_Mission_InProgress){
			led_Mission_InProgress_.F_ChangeSprite_Off();
			Led_Mission_InProgress_State = 0;
			led_Mission_InProgress_.led_Part_InProgress_State(0);
		}

		for(var j =obj_Grp_1_Leds.Length;j<obj_Grp_1_Leds.Length+obj_Grp_2_Leds.Length;j++){										// Init Gpr_1
			arr_led_State[j]= 0;									// You want switch Off the leds
			Led_Renderer[j].F_ChangeSprite_Off();					// Led : Off	
		}
	}


	public void Pause_Stop_Hole_Gpr2(){												// --> 
		for(var i  = 0;i < obj_Grp_2_Leds.Length;i++){										// init obj_Grp_1_Leds state 
			if(arr_led_State[F_Led_Gpr2_num(i)] == 1) Led_Renderer[F_Led_Gpr2_num(i)].F_ChangeSprite_On();	// Led : On
			else Led_Renderer[F_Led_Gpr2_num(i)].F_ChangeSprite_Off();										// Led : Off
		}
		if(Led_Mission_InProgress){
			if(Led_Mission_InProgress_State == 0){
				led_Mission_InProgress_.F_ChangeSprite_Off();
			}
			else{
				Led_Mission_InProgress_State = 1;
				led_Mission_InProgress_.led_Part_InProgress_State(1);
				led_Mission_InProgress_.F_ChangeSprite_On();
			}
		}	
	}



	//////////////////////////// HOLE SCTION : END


	//////// FUNCTION TO PLAY AN ANIMATION ON THE LCD SCREEN ////////
	public void Play_LCD_Screen_Animation(int num){
		GameObject[] gos;			
		gos = GameObject.FindGameObjectsWithTag("Led_animation"); 						// Find all game objects with tag Led_animation
		foreach (GameObject go in gos)  { 
			Destroy(go);																// You could play only one animation
		} 
		Instantiate(obj_Anim_On_Led_Display[num]);										// Play animation
	}


	// Play Led animation, Object Animation and LCD animation for each part of the mission
	public void Play_LedAnim_ObjAnim_LCDAnim_Part1(){
		if(LCD_AnimNumPart1 != -1 && obj_Anim_On_Led_Display.Length > LCD_AnimNumPart1)										// Play animation if an animation is affected
			Play_LCD_Screen_Animation(LCD_AnimNumPart1); 
		if(LED_Anim_Num_Part1!=-1)gameManager.PlayMultiLeds(LED_Anim_Num_Part1);	
		if(PlayfieldAnimation && PF_AnimNumPart1!=-1)playfieldAnimation.PlayAnimationNumber(PF_AnimNumPart1);
		if(sfx_Part1){sound_.clip = sfx_Part1;sound_.Play();}
	}
	public void Play_LedAnim_ObjAnim_LCDAnim_Part2(){
		if(LCD_AnimNumPart2 != -1 && obj_Anim_On_Led_Display.Length > LCD_AnimNumPart2)										// Play animation if an animation is affected
			Play_LCD_Screen_Animation(LCD_AnimNumPart2); 
		if(LED_Anim_Num_Part2!=-1)gameManager.PlayMultiLeds(LED_Anim_Num_Part2);	
		if(PlayfieldAnimation && PF_AnimNumPart2!=-1)playfieldAnimation.PlayAnimationNumber(PF_AnimNumPart2);
		if(sfx_Part2){sound_.clip = sfx_Part2;sound_.Play();}
	}
	public void Play_LedAnim_ObjAnim_LCDAnim_Part3(){
		if(LCD_AnimNumPart3 != -1 && obj_Anim_On_Led_Display.Length > LCD_AnimNumPart3)										// Play animation if an animation is affected
			Play_LCD_Screen_Animation(LCD_AnimNumPart3); 
		if(LED_Anim_Num_Part3!=-1)gameManager.PlayMultiLeds(LED_Anim_Num_Part3);	
		if(PlayfieldAnimation && PF_AnimNumPart3!=-1)playfieldAnimation.PlayAnimationNumber(PF_AnimNumPart3);
		if(sfx_Part3){sound_.clip = sfx_Part3;sound_.Play();}
	}
	public void Play_LedAnim_ObjAnim_LCDAnim_Complete(){
		if(LCD_AnimNumComplete != -1 && obj_Anim_On_Led_Display.Length > LCD_AnimNumComplete)								// Play animation if an animation is affected
			Play_LCD_Screen_Animation(LCD_AnimNumComplete); 
		if(LED_Anim_Num_Complete!=-1)gameManager.PlayMultiLeds(LED_Anim_Num_Complete);	
		if(PlayfieldAnimation && PF_AnimNumComplete!=-1)playfieldAnimation.PlayAnimationNumber(PF_AnimNumComplete);

		if(Toys.Length > 0){																	// Play more than One animation
			for(var i = 0;i<Toys.Length;i++){
				Toys[i].PlayAnimationNumber(AnimNums[i]);								
			}
		}

		if(sfx_Complete){sound_.clip = sfx_Complete;sound_.Play();}
	}
	public void Play_LedAnim_ObjAnim_LCDAnim_Fail(){
		if(LCD_AnimNumFail != -1 && obj_Anim_On_Led_Display.Length > LCD_AnimNumFail)										// Play animation if an animation is affected
			Play_LCD_Screen_Animation(LCD_AnimNumFail); 
		if(LED_Anim_Num_Fail!=-1)gameManager.PlayMultiLeds(LED_Anim_Num_Fail);	
		if(PlayfieldAnimation && PF_AnimNumFail!=-1)playfieldAnimation.PlayAnimationNumber(PF_AnimNumFail);
		if(sfx_Fail){sound_.clip = sfx_Fail;sound_.Play();}
	}


	public void F_InputGetButton(){														// use Edit -> Project Settings -> Input for Flippers
		_GetButton = true;
	}

}
