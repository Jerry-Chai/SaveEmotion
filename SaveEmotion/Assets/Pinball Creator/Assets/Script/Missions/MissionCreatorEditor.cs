//Description : MissionCreatorEditor
#if (UNITY_EDITOR)
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;

[CustomEditor(typeof(MissionCreator))]
public class MissionCreatorEditor : Editor {
	SerializedProperty			SeeInspector;											// use to draw default Inspector

	SerializedProperty 			mechanicsPart1Type;
	SerializedProperty 			mechanicsPart2Type;
	SerializedProperty 			SpecificText;
	SerializedProperty 			speText;
	SerializedProperty 			targetPart1;
	SerializedProperty 			targetType;
	SerializedProperty 			targetPart2;
	SerializedProperty 			targetType2;
	SerializedProperty 			rolloverPart1;
	SerializedProperty 			HowManyRollover;
	SerializedProperty 			rolloverPart2;
	SerializedProperty 			HowManyRollover2;
	SerializedProperty 			bumperType;
	SerializedProperty 			bumperType2;
	SerializedProperty 			HowManyBumper;
	SerializedProperty 			HowManyBumper2;
	SerializedProperty 			uniqueMissionID;
	SerializedProperty 			HowManyTimeGrp_1;
	SerializedProperty 			HowManyTimeGrp_2;
	SerializedProperty 			addLedsWithPart1;
	SerializedProperty 			addLedsWithPart2;
	SerializedProperty 			InitMissionWhenBallLost;
	SerializedProperty 			b_PauseMissionMode;
	SerializedProperty 			KeepLedGrp1OnDuringMission;
	SerializedProperty 			Led_Part1_In_Progress;
	SerializedProperty 			Led_Mission_In_Progress;
	SerializedProperty 			Led_Mission_Complete;

	// Other Options 
	SerializedProperty 			displayOtherOptions;
	SerializedProperty 			b_DisplayText;
	SerializedProperty 			mission_Txt_Name;
	SerializedProperty 			missionText;
	SerializedProperty 			missionTextInfo;

	SerializedProperty 			mechanicsPart1Text;
	SerializedProperty 			mechanicsPart2Text;

	// Part 2 Options 
	SerializedProperty 			b_Mission_Timer;
	SerializedProperty 			b_Mission_Multi_Timer;
	SerializedProperty 			Mission_Time;

	// MultiBall Options
	SerializedProperty 			multiball;
	SerializedProperty 			numberOfBall;
	SerializedProperty 			jackpotPoints;

	// Options when mission is complete
	SerializedProperty 			points;

	// Bonus
	SerializedProperty 			enablekickback;
	SerializedProperty 			selectedBonus;
	SerializedProperty 			BallSaverDuration;
	SerializedProperty 			beginWithKickback;

	SerializedProperty 			kickbackGrp;

	public string[] 			tableMechanicsPart1 = new string[] {"Target","Rollover","Bumper","Spinner","Hole"};
	public string[] 			tableMechanicsPart2 = new string[] {"Empty","Target","Rollover","Bumper","Spinner","Hole"};

	public string[] 			tableTargetPart1 = new string[] {"No Order","Order"};
	public string[] 			tableTargetStylePart1 = new string[] {"Drop Target","Stationary"};


	public string[] 			tableRolloverPart1 = new string[] {"One Rollover","No Order","Order","Lane Change Type"};
	public string[] 			tableRolloverPart2 = new string[] {"One Rollover","No Order","Order"};

	public string[] 			tableBumperStylePart1 = new string[] {"Blue","Pink"};

	public string[] 			tableBonusList = new string[] {"No Bonus","Random","Extra Ball","Ball Saver","Multiplier","Kickback"};


	private Texture2D MakeTex(int width, int height, Color col) {						// use to change the GUIStyle
		Color[] pix = new Color[width * height];
		for (int i = 0; i < pix.Length; ++i) {
			pix[i] = col;
		}
		Texture2D result = new Texture2D(width, height);
		result.SetPixels(pix);
		result.Apply();
		return result;
	}

	private Texture2D 		Tex_01;														// use to define color in the inspector tab
	private Texture2D 		Tex_02;
	private Texture2D 		Tex_03;
	private Texture2D 		Tex_04;
	private Texture2D 		Tex_05;

	void OnEnable () {
		// Setup the SerializedProperties.
		SeeInspector 				= serializedObject.FindProperty ("SeeInspector");




		uniqueMissionID 			= serializedObject.FindProperty ("uniqueMissionID");


		if (!EditorPrefs.HasKey ("uniqueIndex")) {
			uniqueMissionID.intValue = 0;
			EditorPrefs.SetInt ("uniqueIndex",uniqueMissionID.intValue);
		} else {
			uniqueMissionID.intValue = EditorPrefs.GetInt ("uniqueIndex");
		}

		mechanicsPart1Type 			= serializedObject.FindProperty ("mechanicsPart1Type");
		mechanicsPart2Type 			= serializedObject.FindProperty ("mechanicsPart2Type");
		targetPart1 				= serializedObject.FindProperty ("targetPart1");
		targetType 					= serializedObject.FindProperty ("targetType");
		targetPart2 				= serializedObject.FindProperty ("targetPart2");
		targetType2 				= serializedObject.FindProperty ("targetType2");

		rolloverPart1 				= serializedObject.FindProperty ("rolloverPart1");
		SpecificText	 			=  serializedObject.FindProperty ("SpecificText");
		speText 					=  serializedObject.FindProperty ("speText");

		rolloverPart2		 		= serializedObject.FindProperty ("rolloverPart2"); 

		HowManyTimeGrp_1 			= serializedObject.FindProperty ("HowManyTimeGrp_1");
		HowManyRollover  			= serializedObject.FindProperty ("HowManyRollover");
		addLedsWithPart1 			=  serializedObject.FindProperty ("addLedsWithPart1");

		HowManyTimeGrp_2 			= serializedObject.FindProperty ("HowManyTimeGrp_2");
		HowManyRollover2 			= serializedObject.FindProperty ("HowManyRollover2");
		addLedsWithPart2 			=  serializedObject.FindProperty ("addLedsWithPart2");

		InitMissionWhenBallLost 	=  serializedObject.FindProperty ("InitMissionWhenBallLost");
		b_PauseMissionMode 			=  serializedObject.FindProperty ("b_PauseMissionMode");

		KeepLedGrp1OnDuringMission 	=  serializedObject.FindProperty ("KeepLedGrp1OnDuringMission");
		Led_Part1_In_Progress 		=  serializedObject.FindProperty ("Led_Part1_In_Progress");
		Led_Mission_In_Progress 	=  serializedObject.FindProperty ("Led_Mission_In_Progress");
		Led_Mission_Complete 		=  serializedObject.FindProperty ("Led_Mission_Complete");

		bumperType 					=  serializedObject.FindProperty ("bumperType");
		bumperType2 				=  serializedObject.FindProperty ("bumperType2");
		HowManyBumper 				=  serializedObject.FindProperty ("HowManyBumper");
		HowManyBumper2 				=  serializedObject.FindProperty ("HowManyBumper2");


		mechanicsPart1Text 			=  serializedObject.FindProperty ("mechanicsPart1Text");
		mechanicsPart2Text 			=  serializedObject.FindProperty ("mechanicsPart2Text");


		// Other Options 
		displayOtherOptions			=  serializedObject.FindProperty ("displayOtherOptions");
		b_DisplayText				=  serializedObject.FindProperty ("b_DisplayText");
		mission_Txt_Name			=  serializedObject.FindProperty ("mission_Txt_Name");
		missionText 				=  serializedObject.FindProperty ("missionText");
		missionTextInfo				=  serializedObject.FindProperty ("missionTextInfo");
			
		// Part 2 Options 
		b_Mission_Timer 			=  serializedObject.FindProperty ("b_Mission_Timer");
		b_Mission_Multi_Timer 		=  serializedObject.FindProperty ("b_Mission_Multi_Timer");
		Mission_Time 				=  serializedObject.FindProperty ("Mission_Time");

		// MultiBall Options
		multiball 					=  serializedObject.FindProperty ("multiball");
		numberOfBall 				=  serializedObject.FindProperty ("numberOfBall");
		jackpotPoints 				=  serializedObject.FindProperty ("jackpotPoints");
			
		// Options when mission is complete
		points 						=  serializedObject.FindProperty ("points");

		// Bonus
		enablekickback				=  serializedObject.FindProperty ("enablekickback");
		selectedBonus				=  serializedObject.FindProperty ("selectedBonus");
		BallSaverDuration 			=  serializedObject.FindProperty ("BallSaverDuration");
		beginWithKickback 			=  serializedObject.FindProperty ("beginWithKickback");

		kickbackGrp 				=  serializedObject.FindProperty ("kickbackGrp");



		Tex_01 = MakeTex(2, 2, new Color(1,.8f,0.2F,.4f)); 
		Tex_02 = MakeTex(2, 2, Color.yellow); 
		Tex_03 = MakeTex(2, 2, new Color(.3F,.9f,1,.5f));
		Tex_04 = MakeTex(2, 2, new Color(1,.3f,1,.3f)); 
		Tex_05 = MakeTex(2, 2, new Color(1,.5f,0.3F,.4f)); 
	}


	public override void OnInspectorGUI()
	{
		if(SeeInspector.boolValue)							// If true Default Inspector is drawn on screen
			DrawDefaultInspector();

		serializedObject.Update ();

		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("See Inspector :", GUILayout.Width (120));
		EditorGUILayout.PropertyField(SeeInspector, new GUIContent (""), GUILayout.Width (30));
		EditorGUILayout.EndHorizontal ();

		GUIStyle style_Yellow_01 		= new GUIStyle(GUI.skin.box);	style_Yellow_01.normal.background 		= Tex_01; 
		GUIStyle style_Blue 			= new GUIStyle(GUI.skin.box);	style_Blue.normal.background 			= Tex_03;
		GUIStyle style_Purple 			= new GUIStyle(GUI.skin.box);	style_Purple.normal.background 			= Tex_04;
		GUIStyle style_Orange 			= new GUIStyle(GUI.skin.box);	style_Orange.normal.background 			= Tex_05; 
		GUIStyle style_Yellow_Strong 	= new GUIStyle(GUI.skin.box);	style_Yellow_Strong.normal.background 	= Tex_02;


		GUILayout.Label("");
		MissionCreator myScript = (MissionCreator)target; 

		// --> Display mechanics Part 1
		EditorGUILayout.BeginVertical(style_Yellow_Strong);
		GUILayout.Label("Unique Mission ID",EditorStyles.boldLabel);
		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("Mission ID :", GUILayout.Width (120));
		EditorGUILayout.PropertyField (uniqueMissionID, new GUIContent (""), GUILayout.Width (50));
		EditorGUILayout.EndHorizontal ();
		EditorGUILayout.EndVertical ();

		// --> Display mechanics Part 1
		EditorGUILayout.BeginVertical(style_Blue);
		GUILayout.Label("Mission Part 1",EditorStyles.boldLabel);
		Display_Part1_Options ();
		EditorGUILayout.EndVertical ();



		// --> Display mechanics Part 2
		EditorGUILayout.BeginVertical(style_Orange);
		GUILayout.Label("Mission Part 2",EditorStyles.boldLabel);
		Display_Part2_Options ();
		EditorGUILayout.EndVertical ();


		// --> Display other Options

	
		EditorGUILayout.BeginVertical (style_Yellow_01);

		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("Other Options :", GUILayout.Width (180));
		EditorGUILayout.PropertyField (displayOtherOptions, new GUIContent (""), GUILayout.Width (20));
		EditorGUILayout.EndHorizontal ();

		if (displayOtherOptions.boolValue) {
			
			Display_Other_Options ();
		}
			EditorGUILayout.EndVertical ();


		// --> Generate the mission
		EditorGUILayout.BeginVertical (style_Purple);
		GUILayout.Label("");
		if (GUILayout.Button ("Generate the Mission")) {
			GenerateMission(myScript);

			uniqueMissionID.intValue++;
			EditorPrefs.SetInt ("uniqueIndex",uniqueMissionID.intValue);
		}
		GUILayout.Label("");
		EditorGUILayout.EndVertical ();


		serializedObject.ApplyModifiedProperties ();
	}



	private void Display_Part1_Options(){
		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("Mechanics :", GUILayout.Width (120));
		mechanicsPart1Type.intValue = EditorGUILayout.Popup ("", mechanicsPart1Type.intValue, tableMechanicsPart1, EditorStyles.popup);
		EditorGUILayout.EndHorizontal ();


// --> Target Section
		if (mechanicsPart1Type.intValue == 0) {			

			EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField ("Type :", GUILayout.Width (120));
				targetPart1.intValue = EditorGUILayout.Popup ("", targetPart1.intValue, tableTargetPart1, EditorStyles.popup);
			EditorGUILayout.EndHorizontal ();


			EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField ("Sub type :", GUILayout.Width (120));
				targetType.intValue = EditorGUILayout.Popup ("", targetType.intValue, tableTargetStylePart1, EditorStyles.popup);
			EditorGUILayout.EndHorizontal ();

			//EditorGUILayout.LabelField ("");

			EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField ("How many targets:", GUILayout.Width (120));
				EditorGUILayout.PropertyField (HowManyTimeGrp_1, new GUIContent (""), GUILayout.Width (20));
			EditorGUILayout.EndHorizontal ();

			//EditorGUILayout.LabelField ("");

			EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField ("Add Leds :", GUILayout.Width (120));
				EditorGUILayout.PropertyField (addLedsWithPart1, new GUIContent (""), GUILayout.Width (20));
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("text :", GUILayout.Width (120));
			EditorGUILayout.PropertyField (mechanicsPart1Text.GetArrayElementAtIndex(0), new GUIContent (""), GUILayout.Width (100));
			EditorGUILayout.EndHorizontal ();


		}

// Rolover Section
		if (mechanicsPart1Type.intValue == 1) {			// Rollover 

			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Type:", GUILayout.Width (120));
			rolloverPart1.intValue = EditorGUILayout.Popup ("", rolloverPart1.intValue, tableRolloverPart1, EditorStyles.popup);
			EditorGUILayout.EndHorizontal ();


			if(rolloverPart1.intValue != 0  && rolloverPart1.intValue != 3){
				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField ("How many Rollover:", GUILayout.Width (120));
				EditorGUILayout.PropertyField (HowManyRollover, new GUIContent (""), GUILayout.Width (20));
				EditorGUILayout.EndHorizontal ();

				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField ("How many time:", GUILayout.Width (120));
				EditorGUILayout.PropertyField (HowManyTimeGrp_1, new GUIContent (""), GUILayout.Width (20));
				EditorGUILayout.EndHorizontal ();
			}
			else if (rolloverPart1.intValue != 3 && SpecificText.boolValue) {
				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField ("How many time:", GUILayout.Width (120));
				EditorGUILayout.PropertyField (HowManyTimeGrp_1, new GUIContent (""), GUILayout.Width (20));
				EditorGUILayout.EndHorizontal ();
			} else {
				HowManyTimeGrp_1.intValue = speText.stringValue.Length;
			}
// --> Only One Rollover
			if (rolloverPart1.intValue == 0) {				
				//EditorGUILayout.LabelField ("");

				if (SpecificText.boolValue) {
					EditorGUILayout.BeginHorizontal ();
					EditorGUILayout.LabelField ("Text :", GUILayout.Width (120));
					EditorGUILayout.PropertyField (SpecificText, new GUIContent (""), GUILayout.Width (20));
					EditorGUILayout.PropertyField (mechanicsPart1Text.GetArrayElementAtIndex(1) , new GUIContent (""), GUILayout.Width (120));
					EditorGUILayout.EndHorizontal ();
				} else {
					EditorGUILayout.BeginHorizontal ();
					EditorGUILayout.LabelField ("One by one Letter :", GUILayout.Width (120));
					EditorGUILayout.PropertyField (SpecificText, new GUIContent (""), GUILayout.Width (20));
					EditorGUILayout.PropertyField (missionText.GetArrayElementAtIndex(11), new GUIContent (""), GUILayout.Width (120));
					EditorGUILayout.EndHorizontal ();
				}
			

			}

// --> More than One Rollover
			if (rolloverPart1.intValue == 1 || rolloverPart1.intValue == 2) {				
				//EditorGUILayout.LabelField ("");

				SpecificText.boolValue = false;
				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField ("text :", GUILayout.Width (120));
				EditorGUILayout.PropertyField (mechanicsPart1Text.GetArrayElementAtIndex(1), new GUIContent (""), GUILayout.Width (100));
				EditorGUILayout.EndHorizontal ();

			}
			// --> Lane Change (Type 3)
			if (rolloverPart1.intValue == 3) {				
				//EditorGUILayout.LabelField ("");

				SpecificText.boolValue = false;
				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField ("Text :", GUILayout.Width (120));
				EditorGUILayout.PropertyField (missionText.GetArrayElementAtIndex(11), new GUIContent (""), GUILayout.Width (120));
				EditorGUILayout.EndHorizontal ();


			}


			//EditorGUILayout.LabelField ("");

			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Add Leds :", GUILayout.Width (120));
			EditorGUILayout.PropertyField (addLedsWithPart1, new GUIContent (""), GUILayout.Width (20));
			EditorGUILayout.EndHorizontal ();

		}

// --> Bumper
		if (mechanicsPart1Type.intValue == 2) {			
			EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField ("Type :", GUILayout.Width (120));
				bumperType.intValue = EditorGUILayout.Popup ("", bumperType.intValue, tableBumperStylePart1, EditorStyles.popup);
			EditorGUILayout.EndHorizontal ();


			EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField ("How many Bumper:", GUILayout.Width (120));
				EditorGUILayout.PropertyField (HowManyBumper, new GUIContent (""), GUILayout.Width (20));
			EditorGUILayout.EndHorizontal ();


			EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField ("How many time:", GUILayout.Width (120));
				EditorGUILayout.PropertyField (HowManyTimeGrp_1, new GUIContent (""), GUILayout.Width (20));
			EditorGUILayout.EndHorizontal ();

			addLedsWithPart1.boolValue = false;

			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("text :", GUILayout.Width (120));
			EditorGUILayout.PropertyField (mechanicsPart1Text.GetArrayElementAtIndex(2), new GUIContent (""), GUILayout.Width (100));
			EditorGUILayout.EndHorizontal ();
		}

// --> Spinner
		if (mechanicsPart1Type.intValue == 3) {			
			
			EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField ("How many time:", GUILayout.Width (120));
				EditorGUILayout.PropertyField (HowManyTimeGrp_1, new GUIContent (""), GUILayout.Width (20));
			EditorGUILayout.EndHorizontal ();

			//EditorGUILayout.LabelField ("");

			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Add Leds :", GUILayout.Width (120));
			EditorGUILayout.PropertyField (addLedsWithPart1, new GUIContent (""), GUILayout.Width (20));
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("text :", GUILayout.Width (120));
			EditorGUILayout.PropertyField (mechanicsPart1Text.GetArrayElementAtIndex(3), new GUIContent (""), GUILayout.Width (100));
			EditorGUILayout.EndHorizontal ();
		}

		if (mechanicsPart1Type.intValue == 4) {			// Hole
			
			EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField ("How many time:", GUILayout.Width (120));
				EditorGUILayout.PropertyField (HowManyTimeGrp_1, new GUIContent (""), GUILayout.Width (20));
			EditorGUILayout.EndHorizontal ();

			//EditorGUILayout.LabelField ("");

			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Add Leds :", GUILayout.Width (120));
			EditorGUILayout.PropertyField (addLedsWithPart1, new GUIContent (""), GUILayout.Width (20));
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("text :", GUILayout.Width (120));
			EditorGUILayout.PropertyField (mechanicsPart1Text.GetArrayElementAtIndex(4), new GUIContent (""), GUILayout.Width (100));
			EditorGUILayout.EndHorizontal ();
		}
	}

	private void Display_Part2_Options(){
		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("Mechanics :", GUILayout.Width (120));
		mechanicsPart2Type.intValue = EditorGUILayout.Popup ("", mechanicsPart2Type.intValue, tableMechanicsPart2, EditorStyles.popup);
		EditorGUILayout.EndHorizontal ();

		if (mechanicsPart2Type.intValue == 0) {		
			if (HowManyTimeGrp_2.intValue != 0) {
				HowManyTimeGrp_2.intValue = 0;
			}
		}


		// --> Target Section
		if (mechanicsPart2Type.intValue == 1) {			

			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Type :", GUILayout.Width (120));
			targetPart2.intValue = EditorGUILayout.Popup ("", targetPart2.intValue, tableTargetPart1, EditorStyles.popup);
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Sub type :", GUILayout.Width (120));
			targetType2.intValue = EditorGUILayout.Popup ("", targetType2.intValue, tableTargetStylePart1, EditorStyles.popup);
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("How many targets:", GUILayout.Width (120));
			EditorGUILayout.PropertyField (HowManyTimeGrp_2, new GUIContent (""), GUILayout.Width (20));
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Add Leds :", GUILayout.Width (120));
			EditorGUILayout.PropertyField (addLedsWithPart2, new GUIContent (""), GUILayout.Width (20));
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("text :", GUILayout.Width (120));
			EditorGUILayout.PropertyField (mechanicsPart2Text.GetArrayElementAtIndex(0), new GUIContent (""), GUILayout.Width (100));
			EditorGUILayout.EndHorizontal ();


		}

		// Rolover Section
		if (mechanicsPart2Type.intValue == 2) {			// Rollover 

			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Type:", GUILayout.Width (120));
			rolloverPart2.intValue = EditorGUILayout.Popup ("", rolloverPart2.intValue, tableRolloverPart2, EditorStyles.popup);
			EditorGUILayout.EndHorizontal ();

			if(rolloverPart2.intValue != 0){
				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField ("How many Rollover:", GUILayout.Width (120));
				EditorGUILayout.PropertyField (HowManyRollover2, new GUIContent (""), GUILayout.Width (20));
				EditorGUILayout.EndHorizontal ();
			}

			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("How many time:", GUILayout.Width (120));
			EditorGUILayout.PropertyField (HowManyTimeGrp_2, new GUIContent (""), GUILayout.Width (20));
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Add Leds :", GUILayout.Width (120));
			EditorGUILayout.PropertyField (addLedsWithPart2, new GUIContent (""), GUILayout.Width (20));
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("text :", GUILayout.Width (120));
			EditorGUILayout.PropertyField (mechanicsPart2Text.GetArrayElementAtIndex(1), new GUIContent (""), GUILayout.Width (100));
			EditorGUILayout.EndHorizontal ();

		}

		// --> Bumper
		if (mechanicsPart2Type.intValue == 3) {			
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Type :", GUILayout.Width (120));
			bumperType2.intValue = EditorGUILayout.Popup ("", bumperType2.intValue, tableBumperStylePart1, EditorStyles.popup);
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("How many Bumper:", GUILayout.Width (120));
			EditorGUILayout.PropertyField (HowManyBumper2, new GUIContent (""), GUILayout.Width (20));
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("How many time:", GUILayout.Width (120));
			EditorGUILayout.PropertyField (HowManyTimeGrp_2, new GUIContent (""), GUILayout.Width (20));
			EditorGUILayout.EndHorizontal ();

			addLedsWithPart2.boolValue = false;

			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("text :", GUILayout.Width (120));
			EditorGUILayout.PropertyField (mechanicsPart2Text.GetArrayElementAtIndex(2), new GUIContent (""), GUILayout.Width (100));
			EditorGUILayout.EndHorizontal ();
		}

		// --> Spinner
		if (mechanicsPart2Type.intValue == 4) {			

			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("How many time:", GUILayout.Width (120));
			EditorGUILayout.PropertyField (HowManyTimeGrp_2, new GUIContent (""), GUILayout.Width (20));
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Add Leds :", GUILayout.Width (120));
			EditorGUILayout.PropertyField (addLedsWithPart2, new GUIContent (""), GUILayout.Width (20));
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("text :", GUILayout.Width (120));
			EditorGUILayout.PropertyField (mechanicsPart2Text.GetArrayElementAtIndex(3), new GUIContent (""), GUILayout.Width (100));
			EditorGUILayout.EndHorizontal ();
		}

		if (mechanicsPart2Type.intValue == 5) {			// Hole

			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("How many time:", GUILayout.Width (120));
			EditorGUILayout.PropertyField (HowManyTimeGrp_2, new GUIContent (""), GUILayout.Width (20));
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Add Leds :", GUILayout.Width (120));
			EditorGUILayout.PropertyField (addLedsWithPart2, new GUIContent (""), GUILayout.Width (20));
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("text :", GUILayout.Width (120));
			EditorGUILayout.PropertyField (mechanicsPart2Text.GetArrayElementAtIndex(4), new GUIContent (""), GUILayout.Width (100));
			EditorGUILayout.EndHorizontal ();
		}
	}


	private void GenerateMissionPart2(MissionCreator myScript,GameObject newMission){
		//GameObject newMission = Instantiate (myScript.p_MissionInit, myScript.transform);


		Undo.RegisterCreatedObjectUndo (newMission, newMission.name);


		newMission.name =   RenameMissionPart2 (newMission.name);
		GameObject tmpObj = new GameObject ();


		GameObject newInstantiatePosition = Instantiate (tmpObj,newMission.transform);	
		DestroyImmediate (tmpObj);
		newInstantiatePosition.name = "Part2";

		// Setup Missionindex Script
		MissionIndex missionIndex =  newMission.GetComponent<MissionIndex>();
		missionIndex.mission_Index = myScript.uniqueMissionID;

		// Setup Mission_Start Script
		Mission_Start missionStart =  newMission.GetComponent<Mission_Start>();
		missionStart.HowManyTime_Gpr2 = myScript.HowManyTimeGrp_2;

		if (multiball.boolValue && mechanicsPart2Type.intValue == 2) {
			missionStart.HowManyTime_Gpr2 = 1;
		}

		float ledOffset = 0;
		int HowMechanics = 0;
		// rollover
		if (rolloverPart2.intValue == 0 && mechanicsPart2Type.intValue == 2) {		
			HowMechanics = 1;
			if (addLedsWithPart2.boolValue)
				missionStart.obj_Grp_2_Leds = new GameObject[HowManyTimeGrp_2.intValue];
		}
		else if ((rolloverPart2.intValue == 1 || rolloverPart2.intValue == 2) && mechanicsPart2Type.intValue == 2) {		
			HowMechanics = HowManyRollover2.intValue;
			if (addLedsWithPart2.boolValue)
				missionStart.obj_Grp_2_Leds = new GameObject[HowManyRollover2.intValue];
		}

		// Bumper
		else if (mechanicsPart2Type.intValue == 3) {	
			
			HowMechanics = HowManyBumper2.intValue;
		}
		// Spinner
		else if (mechanicsPart2Type.intValue == 4) {										
			HowMechanics = 1;
		}
		// Hole
		else if (mechanicsPart2Type.intValue == 5) {										
			HowMechanics = 1;
		}
		else
			HowMechanics = HowManyTimeGrp_2.intValue;

		// Target
		if(mechanicsPart2Type.intValue == 1){

			missionStart.Grp_2_Target = true;

			missionStart.Mission_Txt [5] = mechanicsPart2Text.GetArrayElementAtIndex (0).stringValue;

			if (targetPart2.intValue == 0)
				missionStart.Target_No_Order_Grp_2 = true;
			else
				missionStart.Target_Order_Grp_2 = true;


			missionStart.obj_Grp_2 = new GameObject[HowMechanics];

			if (addLedsWithPart2.boolValue)
				missionStart.obj_Grp_2_Leds = new GameObject[HowManyTimeGrp_2.intValue];

		}
		// Rollover
		if(mechanicsPart2Type.intValue == 2){	

			missionStart.Grp_2_Rollover = true;

			missionStart.Mission_Txt [5] = mechanicsPart2Text.GetArrayElementAtIndex (1).stringValue;

			if (rolloverPart2.intValue == 0 || rolloverPart2.intValue == 1 )
				missionStart.Rollover_No_Order_Grp_2 = true;
			else if (rolloverPart2.intValue == 2)
				missionStart.Rollover_Order_Grp_2 = true;
			


			missionStart.obj_Grp_2 = new GameObject[HowMechanics];

			//if (addLedsWithPart2.boolValue)
			//	missionStart.obj_Grp_2_Leds = new GameObject[HowManyTimeGrp_2.intValue];

		}

		// Bumper
		if(mechanicsPart2Type.intValue == 3){	

			missionStart.Grp_2_Bumper = true;

			missionStart.Mission_Txt [5] = mechanicsPart2Text.GetArrayElementAtIndex (2).stringValue;

			missionStart.obj_Grp_2 = new GameObject[HowMechanics];

		}

		// Spinner
		if(mechanicsPart2Type.intValue == 4){	

			missionStart.Grp_2_Spinner = true;

			missionStart.Mission_Txt [5] = mechanicsPart2Text.GetArrayElementAtIndex (3).stringValue;

			missionStart.obj_Grp_2 = new GameObject[HowMechanics];


			if (addLedsWithPart2.boolValue)
				missionStart.obj_Grp_2_Leds = new GameObject[HowManyTimeGrp_2.intValue];
		}

		// Hole
		if(mechanicsPart2Type.intValue == 5){	

			missionStart.Grp_2_Hole = true;

			missionStart.Mission_Txt [5] = mechanicsPart2Text.GetArrayElementAtIndex (4).stringValue;

			missionStart.obj_Grp_2 = new GameObject[HowMechanics];


			if (addLedsWithPart2.boolValue)
				missionStart.obj_Grp_2_Leds = new GameObject[HowManyTimeGrp_2.intValue];
		}



		List<GameObject> objList = new List<GameObject> ();
		for (var i = 0; i < HowMechanics; i++) {

			GameObject newMechanic;

			//Empty
			if (mechanicsPart2Type.intValue == 0) {
				newMechanic = null;
			}
			// Create Targets
			else if (mechanicsPart2Type.intValue == 1) {
				if (targetType2.intValue == 0){
					newMechanic = Instantiate (myScript.dropTarget, newMission.transform);
				}
				else {
					newMechanic = Instantiate (myScript.stationaryTarget, newMission.transform); 
					missionStart.Target_Type_Stationary = true;
				}

				SetupTarget (myScript, newMechanic, missionStart, i, newMission.transform,.2f,"Part2");
				ledOffset = .03f;
			}
			// Create Rollovers
			else if (mechanicsPart2Type.intValue == 2) {
				newMechanic = Instantiate (myScript.objRollover, newMission.transform); 

				SetupRollover (myScript, newMechanic, missionStart, i, newMission.transform,.2f,"Part2");
				ledOffset = .06f;
			} 
			// Create Bumpers
			else if (mechanicsPart2Type.intValue == 3) {
				if(bumperType2.intValue == 0)
					newMechanic = Instantiate (myScript.objBumper_01, newMission.transform); 
				else
					newMechanic = Instantiate (myScript.objBumper_02, newMission.transform); 	

				SetupBumper (myScript, newMechanic, missionStart, i, newMission.transform,.2f,"Part2");

			} 			
			// Create Spinner
			else if (mechanicsPart2Type.intValue == 4) {
				newMechanic = Instantiate (myScript.objSpinner, newMission.transform); 
				SetupSpinner (myScript, newMechanic, missionStart, i, newMission.transform,.2f,"Part2");
				ledOffset = 0f;
			} 
			// Create Hole
			else if (mechanicsPart2Type.intValue == 5) {
				newMechanic = Instantiate (myScript.objHole, newMission.transform); 
				SetupHole (myScript, newMechanic, missionStart, i, newMission.transform,.2f,"Part2");
				ledOffset = .06f;

			}
			else {

				newMechanic =null; 
				ledOffset = 0f;
			}	




			if (newMechanic != null) {
				newMechanic.transform.SetParent (newInstantiatePosition.transform);
				objList.Add (newMechanic);
			}
		}

		if (addLedsWithPart2.boolValue && objList.Count > 0) {
			if (mechanicsPart2Type.intValue != 2) {
				for (var i = 0; i < HowManyTimeGrp_2.intValue; i++) {
					// Create Led
					GameObject newLed = Instantiate (myScript.objLed, newMission.transform); 

					if (objList.Count == 1) {
						SetupLed (newLed, objList [0], missionStart, i, ledOffset + .03f * i, true,"Part2");
					} else {
						SetupLed (newLed, objList [i], missionStart, i, ledOffset, false,"Part2");
					}
				}
			}
			else {			// Rollover Case
				
				int tmpHowManyLeds = 0;
				if (rolloverPart2.intValue == 0) {
					tmpHowManyLeds = HowManyTimeGrp_2.intValue;

				}
				else if (rolloverPart2.intValue == 1 
					|| rolloverPart2.intValue == 2) {
					tmpHowManyLeds = HowManyRollover2.intValue;
				}


				for (var i = 0; i < tmpHowManyLeds; i++) {
					// Create Led
					GameObject newLed = Instantiate (myScript.objLed, newMission.transform); 

					if (objList.Count == 1) {
						SetupLed (newLed, objList [0], missionStart, i, ledOffset + .03f * i, true, "Part2");
					} else {
						SetupLed (newLed, objList [i], missionStart, i, ledOffset, false, "Part2");
					}
				}
			}
		}

		//DestroyImmediate (newInstantiatePosition);
	}


	private void GenerateMission(MissionCreator myScript){
		GameObject newMission = Instantiate (myScript.p_MissionInit, myScript.transform);


		Undo.RegisterCreatedObjectUndo (newMission, newMission.name);


		newMission.name = RenameMission (newMission.name);

		GameObject tmpObj = new GameObject ();

		GameObject newInstantiatePosition = Instantiate (tmpObj,newMission.transform);
		DestroyImmediate (tmpObj);
		newInstantiatePosition.name = "Part1";


// Setup Missionindex Script
		MissionIndex missionIndex =  newMission.GetComponent<MissionIndex>();
		missionIndex.mission_Index = myScript.uniqueMissionID;



// Setup Mission_Start Script
		Mission_Start missionStart =  newMission.GetComponent<Mission_Start>();

		missionStart.InitMissionWhenBallLost = myScript.InitMissionWhenBallLost;
		missionStart.b_PauseMissionMode = myScript.b_PauseMissionMode;
		missionStart.HowManyTime_Gpr1 = myScript.HowManyTimeGrp_1;
		missionStart.KeepLedGrp1OnDuringMission = KeepLedGrp1OnDuringMission.boolValue;

// Setup Text

		for (var i = 0; i < missionText.arraySize; i++) {
			if (i == 4 || i == 5 || i == 11) {
			} else {
				missionStart.Mission_Txt [i] = missionText.GetArrayElementAtIndex (i).stringValue;
			}
		}

// Setup Progression Led
		generateProgressionLed(myScript,missionStart);

// Option Timer
		if (b_Mission_Timer.boolValue) {
			missionStart.b_Mission_Timer = b_Mission_Timer.boolValue;
			missionStart.b_Mission_Multi_Timer = b_Mission_Multi_Timer.boolValue;
			missionStart.Mission_Time = Mission_Time.intValue;
		}

// Option Multiball
		if (multiball.boolValue) {
			if (mechanicsPart2Type.intValue == 2) {
				missionStart.multiBall = multiball.boolValue;
				missionStart.numberOfBall = numberOfBall.intValue;
				missionStart.JackpotPoints = jackpotPoints.intValue;
			}
		}

// Options when mission is complete
		missionStart.Points = points.intValue;

		if (!multiball.boolValue) {
			if (enablekickback.boolValue) {
				missionStart.KickBack = enablekickback.boolValue;
				missionStart.BeginWithKickBack = beginWithKickback.boolValue;

				generateKickbackTargetAndLed (myScript, missionStart);
			}

			if(selectedBonus.intValue == 1)	missionStart.Random_Bonus = true;
			if(selectedBonus.intValue == 2)missionStart.ExtraBall = true;
			if(selectedBonus.intValue == 3) {
				missionStart.BallSaver = true;
				missionStart.BallSaverDuration = BallSaverDuration.intValue;
			}
			if(selectedBonus.intValue == 4)missionStart.Multiplier = true;
			if(selectedBonus.intValue == 5)missionStart.KickBack = true;

		}
		float ledOffset = 0;
		int HowMechanics = 0;
// rollover
		if (rolloverPart1.intValue == 0 && mechanicsPart1Type.intValue == 1) {		
			HowMechanics = 1;
		} else if (rolloverPart1.intValue == 1 && mechanicsPart1Type.intValue == 1
		         || rolloverPart1.intValue == 2 && mechanicsPart1Type.intValue == 1) {	
			HowMechanics = HowManyRollover.intValue;

			if (SpecificText.boolValue)
				SpecificText.boolValue = false;
			//Debug.Log ("Here : " + HowMechanics);
		}
		else if (rolloverPart1.intValue == 3 && mechanicsPart1Type.intValue == 1) {		
			HowMechanics = missionText.GetArrayElementAtIndex (11).stringValue.Length;
		} 			
// Bumper
		else if (mechanicsPart1Type.intValue == 2) {										
			HowMechanics = HowManyBumper.intValue;
		}
// Spinner
		else if (mechanicsPart1Type.intValue == 3) {										
			HowMechanics = 1;
		}
// Hole
		else if (mechanicsPart1Type.intValue == 4) {										
			HowMechanics = 1;
		}
		else
			HowMechanics = HowManyTimeGrp_1.intValue;

// Target
		if(mechanicsPart1Type.intValue == 0){
			missionStart.Mission_Txt [4] = mechanicsPart1Text.GetArrayElementAtIndex (0).stringValue;
			missionStart.Grp_1_Target = true;

			if (targetPart1.intValue == 0)
				missionStart.Target_No_Order_Grp_1 = true;
			else
				missionStart.Target_Order_Grp_1 = true;


			missionStart.obj_Grp_1 = new GameObject[HowMechanics];

			if (addLedsWithPart1.boolValue)
				missionStart.obj_Grp_1_Leds = new GameObject[HowManyTimeGrp_1.intValue];
			
		}
// Rollover
		if(mechanicsPart1Type.intValue == 1){	

			missionStart.Grp_1_Rollover = true;

			if (rolloverPart1.intValue == 0 || rolloverPart1.intValue == 1 )
				missionStart.Rollover_No_Order_Grp_1 = true;
			else if (rolloverPart1.intValue == 2)
				missionStart.Rollover_Order_Grp_1 = true;
			else if (rolloverPart1.intValue == 3)
				missionStart.Rollover_Type3_Grp_1 = true;

			missionStart.Mission_Txt [4] = mechanicsPart1Text.GetArrayElementAtIndex (1).stringValue;

			if (!SpecificText.boolValue && rolloverPart1.intValue == 0 || rolloverPart1.intValue == 3) {
				missionStart.SpecificText = true;
				missionStart.Mission_Txt [11] = missionText.GetArrayElementAtIndex (11).stringValue;

			} 


			missionStart.obj_Grp_1 = new GameObject[HowMechanics];

			if (addLedsWithPart1.boolValue) {
				if (rolloverPart1.intValue == 0) {
					if (!SpecificText.boolValue)
						missionStart.obj_Grp_1_Leds = new GameObject[missionText.GetArrayElementAtIndex (11).stringValue.Length];
					else
						missionStart.obj_Grp_1_Leds = new GameObject[HowManyTimeGrp_1.intValue];
				} else if (rolloverPart1.intValue == 1 || rolloverPart1.intValue == 2) {
					missionStart.obj_Grp_1_Leds = new GameObject[HowManyRollover.intValue];
				} else if (rolloverPart1.intValue == 3) {
					missionStart.obj_Grp_1_Leds = new GameObject[missionText.GetArrayElementAtIndex (11).stringValue.Length];
				}
			}

		}

// Bumper
		if(mechanicsPart1Type.intValue == 2){	

			missionStart.Grp_1_Bumper = true;
			missionStart.obj_Grp_1 = new GameObject[HowMechanics];
			missionStart.Mission_Txt [4] = mechanicsPart1Text.GetArrayElementAtIndex (2).stringValue;
		}
			
// Spinner
		if(mechanicsPart1Type.intValue == 3){	

			missionStart.Grp_1_Spinner = true;
			missionStart.obj_Grp_1 = new GameObject[HowMechanics];

			missionStart.Mission_Txt [4] = mechanicsPart1Text.GetArrayElementAtIndex (3).stringValue;

			if (addLedsWithPart1.boolValue)
				missionStart.obj_Grp_1_Leds = new GameObject[HowManyTimeGrp_1.intValue];
		}

// Hole
		if(mechanicsPart1Type.intValue == 4){	

			missionStart.Grp_1_Hole = true;
			missionStart.obj_Grp_1 = new GameObject[HowMechanics];

			missionStart.Mission_Txt [4] = mechanicsPart1Text.GetArrayElementAtIndex (4).stringValue;

			if (addLedsWithPart1.boolValue)
				missionStart.obj_Grp_1_Leds = new GameObject[HowManyTimeGrp_1.intValue];
		}



		List<GameObject> objList = new List<GameObject> ();
		for (var i = 0; i < HowMechanics; i++) {

			GameObject newMechanic;

// Create Targets
			if (mechanicsPart1Type.intValue == 0) {
				if(targetType.intValue == 0) 
					newMechanic = Instantiate (myScript.dropTarget,newMission.transform); 
				else
					newMechanic = Instantiate (myScript.stationaryTarget,newMission.transform); 

				SetupTarget (myScript, newMechanic, missionStart, i, newMission.transform,0f,"Part1");
				ledOffset = .03f;
			}
// Create Rollovers
			else if (mechanicsPart1Type.intValue == 1) {
				newMechanic = Instantiate (myScript.objRollover, newMission.transform); 

				SetupRollover (myScript, newMechanic, missionStart, i, newMission.transform,0f,"Part1");
				ledOffset = .06f;
			} 
// Create Bumpers
			else if (mechanicsPart1Type.intValue == 2) {
				if(bumperType.intValue == 0)
					newMechanic = Instantiate (myScript.objBumper_01, newMission.transform); 
				else
					newMechanic = Instantiate (myScript.objBumper_02, newMission.transform); 	

				SetupBumper (myScript, newMechanic, missionStart, i, newMission.transform,0f,"Part1");

			} 			
// Create Spinner
			else if (mechanicsPart1Type.intValue == 3) {
				newMechanic = Instantiate (myScript.objSpinner, newMission.transform); 
				SetupSpinner (myScript, newMechanic, missionStart, i, newMission.transform,0f,"Part1");
				ledOffset = 0f;
			} 
// Create Hole
			else if (mechanicsPart1Type.intValue == 4) {
				newMechanic = Instantiate (myScript.objHole, newMission.transform); 
				SetupHole (myScript, newMechanic, missionStart, i, newMission.transform,0f,"Part1");
				ledOffset = .06f;

			} else {

				newMechanic = null; 
				ledOffset = 0f;
			}	


			newMechanic.transform.SetParent (newInstantiatePosition.transform);
			objList.Add (newMechanic);
		}


		if (addLedsWithPart1.boolValue) {
			if (mechanicsPart1Type.intValue != 1) {
				for (var i = 0; i < HowManyTimeGrp_1.intValue; i++) {
					// Create Led
					GameObject newLed = Instantiate (myScript.objLed, newMission.transform); 

					if (objList.Count == 1) {
						SetupLed (newLed, objList [0], missionStart, i, ledOffset + .03f * i, true, "Part1");
					} else {
						SetupLed (newLed, objList [i], missionStart, i, ledOffset, false, "Part1");
					}
				}
			}
			else {			// Rollover Case
				int tmpHowManyLeds = 0;
				if (rolloverPart1.intValue == 0) {
					if (!SpecificText.boolValue)
						tmpHowManyLeds = missionText.GetArrayElementAtIndex (11).stringValue.Length;
					else
						tmpHowManyLeds = HowManyTimeGrp_1.intValue;
				}
				else if (rolloverPart1.intValue == 1 
					|| rolloverPart1.intValue == 2) {
					tmpHowManyLeds = HowManyRollover.intValue;
				}
				else if (rolloverPart1.intValue == 3) {
					tmpHowManyLeds = missionText.GetArrayElementAtIndex (11).stringValue.Length;
				}



				for (var i = 0; i < tmpHowManyLeds; i++) {
					// Create Led
					GameObject newLed = Instantiate (myScript.objLed, newMission.transform); 

					if (objList.Count == 1) {
						SetupLed (newLed, objList [0], missionStart, i, ledOffset + .03f * i, true, "Part1");
					} else {
						SetupLed (newLed, objList [i], missionStart, i, ledOffset, false, "Part1");
					}
				}
			}
		}

		//
		GenerateMissionPart2(myScript,newMission);


	}
		
	private void generateKickbackTargetAndLed(MissionCreator myScript,Mission_Start missionStart){
		missionStart.obj_Door_Kickback = new GameObject[2];
		missionStart.obj_Led_Kickback = new GameObject[2];

			GameObject objParent = Instantiate((GameObject) kickbackGrp.objectReferenceValue,missionStart.transform);
			objParent.name = "Kickback_grp";

			Transform[] children = objParent.GetComponentsInChildren<Transform> ();

			foreach(Transform child in children) {
				if(child.name == "sc_Drop_Target_2_Left")
					missionStart.obj_Door_Kickback[0] = child.gameObject;
				if(child.name == "sc_Plane_Led_17_Left_zl")
					missionStart.obj_Led_Kickback[0] = child.gameObject;
				if(child.name == "sc_Drop_Target_1_Right")
					missionStart.obj_Door_Kickback[1] = child.gameObject;
				if(child.name == "sc_Plane_Led_17_Right_zl")
					missionStart.obj_Led_Kickback[1] = child.gameObject;
			}
	}

	private void generateProgressionLed(MissionCreator myScript,Mission_Start missionStart){
		GameObject newLed;
		if (Led_Part1_In_Progress.boolValue) {
			newLed = Instantiate (myScript.Led_Progress, missionStart.transform); 

			newLed.transform.localPosition = new Vector3 (
				newLed.transform.localPosition.x + .05f,
				newLed.transform.localPosition.y,
				newLed.transform.localPosition.z + .2f);

			newLed.name = newLed.name.Replace ("(Clone)", "");
			newLed.name = "Led_Part1_InProgress";
			newLed.transform.SetParent (missionStart.transform);
		
			ChangeSpriteRenderer[] children2 = newLed.GetComponentsInChildren<ChangeSpriteRenderer> ();

			foreach (ChangeSpriteRenderer child in children2) {
				missionStart.Led_Part1_InProgress = child.gameObject;
			}


		}
		if (Led_Mission_In_Progress.boolValue) {
			newLed = Instantiate (myScript.Led_Progress, missionStart.transform); 

			newLed.transform.localPosition = new Vector3 (
				newLed.transform.localPosition.x + .05f + 0.2f,
				newLed.transform.localPosition.y,
				newLed.transform.localPosition.z + .2f);

			newLed.name = newLed.name.Replace ("(Clone)", "");
			newLed.name = "Led_Part2_InProgress";
			newLed.transform.SetParent (missionStart.transform);

			ChangeSpriteRenderer[] children2 = newLed.GetComponentsInChildren<ChangeSpriteRenderer> ();

			foreach (ChangeSpriteRenderer child in children2) {
				missionStart.Led_Mission_InProgress = child.gameObject;
			}
		}
		if (Led_Mission_Complete.boolValue) {
			newLed = Instantiate (myScript.Led_Progress, missionStart.transform); 

			newLed.transform.localPosition = new Vector3 (
				newLed.transform.localPosition.x - .3f,
				newLed.transform.localPosition.y,
				newLed.transform.localPosition.z);

			newLed.name = newLed.name.Replace ("(Clone)", "");
			newLed.name = "Led_Mission_Complete";
			newLed.transform.SetParent (missionStart.transform);

			ChangeSpriteRenderer[] children2 = newLed.GetComponentsInChildren<ChangeSpriteRenderer> ();

			foreach (ChangeSpriteRenderer child in children2) {
				missionStart.Led_Mission_Complete = child.gameObject;
			}
		}

	}

	private void SetupHole (MissionCreator myScript, GameObject newMechanic,Mission_Start missionStart,int i,Transform newMission,float startOffset,string s_Part){
		newMechanic.transform.SetParent (newMission.transform);

		newMechanic.transform.localPosition = new Vector3 (
			newMechanic.transform.localPosition.x + startOffset,
			newMechanic.transform.localPosition.y,
			newMechanic.transform.localPosition.z- 0.08f*i + 0.2f);

		newMechanic.name = newMechanic.name.Replace ("(Clone)", "");
		newMechanic.name = i + "_" + newMechanic.name;

		Hole[] children = newMechanic.GetComponentsInChildren<Hole> ();

		foreach (Hole child in children) {
			child.name = i + "_" + child.name;


			Hole objChild = child.GetComponent<Hole> ();
			if(s_Part == "Part1")
				objChild.index = i;
			else if(s_Part == "Part2")
				objChild.index = i + missionStart.obj_Grp_1.Length;
			
			objChild.Parent_Manager = new GameObject[1];
			objChild.Parent_Manager [0] = newMission.gameObject;

			if(s_Part == "Part1")
				missionStart.obj_Grp_1 [i] = child.gameObject;
			else if(s_Part == "Part2")
				missionStart.obj_Grp_2 [i] = child.gameObject;
		}
	}

	private void SetupSpinner (MissionCreator myScript, GameObject newMechanic,Mission_Start missionStart,int i,Transform newMission,float startOffset,string s_Part){
		newMechanic.transform.SetParent (newMission.transform);

		newMechanic.transform.localPosition = new Vector3 (
			newMechanic.transform.localPosition.x + startOffset,
			newMechanic.transform.localPosition.y,
			newMechanic.transform.localPosition.z- 0.08f*i + 0.2f);

		newMechanic.name = newMechanic.name.Replace ("(Clone)", "");
		newMechanic.name = i + "_" + newMechanic.name;

		Spinner_LapCounter[] children = newMechanic.GetComponentsInChildren<Spinner_LapCounter> ();

		foreach (Spinner_LapCounter child in children) {
			child.name = i + "_" + child.name;


			Spinner_LapCounter objChild = child.GetComponent<Spinner_LapCounter> ();

			if(s_Part == "Part1")
				objChild.index = i;
			else if(s_Part == "Part2")
				objChild.index = i + missionStart.obj_Grp_1.Length;
			
			objChild.Parent_Manager = new GameObject[1];
			objChild.Parent_Manager [0] = newMission.gameObject;

			if(s_Part == "Part1")
				missionStart.obj_Grp_1 [i] = child.gameObject;
			else if(s_Part == "Part2")
				missionStart.obj_Grp_2 [i] = child.gameObject;
		}
	}

	private void SetupBumper (MissionCreator myScript, GameObject newMechanic,Mission_Start missionStart,int i,Transform newMission,float startOffset,string s_Part){
		newMechanic.transform.SetParent (newMission.transform);

		newMechanic.transform.localPosition = new Vector3 (
			newMechanic.transform.localPosition.x + startOffset,
			newMechanic.transform.localPosition.y,
			newMechanic.transform.localPosition.z- 0.08f*i + 0.2f);

		newMechanic.name = newMechanic.name.Replace ("(Clone)", "");
		newMechanic.name = i + "_" + newMechanic.name;

		Bumper_js[] children = newMechanic.GetComponentsInChildren<Bumper_js> ();

		foreach (Bumper_js child in children) {
			child.name = i + "_" + child.name;


			Bumper_js objChild = child.GetComponent<Bumper_js> ();

			if(s_Part == "Part1")
				objChild.index = i;
			else if(s_Part == "Part2")
				objChild.index = i + missionStart.obj_Grp_1.Length;

			objChild.Parent_Manager = new GameObject[1];
			objChild.Parent_Manager [0] = newMission.gameObject;

			if(s_Part == "Part1")
				missionStart.obj_Grp_1 [i] = child.gameObject;
			else if(s_Part == "Part2")
				missionStart.obj_Grp_2 [i] = child.gameObject;
		}
	}


	private void SetupRollover (MissionCreator myScript, GameObject newMechanic,Mission_Start missionStart,int i,Transform newMission,float startOffset,string s_Part){
		newMechanic.transform.SetParent (newMission.transform);

		newMechanic.transform.localPosition = new Vector3 (
			newMechanic.transform.localPosition.x + startOffset,
			newMechanic.transform.localPosition.y,
			newMechanic.transform.localPosition.z- 0.05f*i + 0.2f);

		newMechanic.name = newMechanic.name.Replace ("(Clone)", "");
		newMechanic.name = i + "_" + newMechanic.name;

		Rollovers[] children = newMechanic.GetComponentsInChildren<Rollovers> ();

		foreach (Rollovers child in children) {
			child.name = i + "_" + child.name;


			Rollovers objChild = child.GetComponent<Rollovers> ();

			if(s_Part == "Part1")
				objChild.index = i;
			else if(s_Part == "Part2")
				objChild.index = i + missionStart.obj_Grp_1.Length;

			objChild.Parent_Manager = new GameObject[1];
			objChild.Parent_Manager [0] = newMission.gameObject;

			if(s_Part == "Part1")
				missionStart.obj_Grp_1 [i] = child.gameObject;
			else if(s_Part == "Part2")
				missionStart.obj_Grp_2 [i] = child.gameObject;
		}
	}


	private void SetupTarget (MissionCreator myScript, GameObject newMechanic,Mission_Start missionStart,int i,Transform newMission,float startOffset,string s_Part){
		newMechanic.transform.SetParent (newMission.transform);

		newMechanic.transform.localPosition = new Vector3 (
			newMechanic.transform.localPosition.x + startOffset,
			newMechanic.transform.localPosition.y,
			newMechanic.transform.localPosition.z- 0.05f*i + .2f);

		newMechanic.name = newMechanic.name.Replace ("(Clone)", "");
		newMechanic.name = i + "_" + newMechanic.name;

		Target[] children = newMechanic.GetComponentsInChildren<Target> ();

		foreach (Target child in children) {
			child.name = i + "_" + child.name;


			Target objChild = child.GetComponent<Target> ();

			if(s_Part == "Part1")
				objChild.index = i;
			else if(s_Part == "Part2")
				objChild.index = i + missionStart.obj_Grp_1.Length;

			objChild.Parent_Manager = new GameObject[1];
			objChild.Parent_Manager [0] = newMission.gameObject;

			if(s_Part == "Part1")
				missionStart.obj_Grp_1 [i] = child.gameObject;
			else if(s_Part == "Part2")
				missionStart.obj_Grp_2 [i] = child.gameObject;
		}
	}

	private void SetupLed (GameObject newLed, GameObject newMechanic,Mission_Start missionStart,int i,float posOffset,bool OneMechanics,string s_Part){
		newLed.transform.position = newMechanic.transform.position;

		newLed.transform.localPosition = new Vector3 (
			newLed.transform.localPosition.x - posOffset,
			newLed.transform.localPosition.y,
			newLed.transform.localPosition.z);

		newLed.name = newLed.name.Replace ("(Clone)", "");
		newLed.transform.SetParent (newMechanic.transform);
		if (OneMechanics) {
			newLed.transform.localScale = new Vector3 (.3f, .3f, .3f);
		}

		ChangeSpriteRenderer[] children2 = newLed.GetComponentsInChildren<ChangeSpriteRenderer> ();

		foreach (ChangeSpriteRenderer child in children2) {
			child.name = i + "_" + child.name;
		
			if(s_Part == "Part1")
				missionStart.obj_Grp_1_Leds [i] = child.gameObject;
			else if(s_Part == "Part2")
				missionStart.obj_Grp_2_Leds [i] = child.gameObject;
		}

	}

	private string RenameMission(string newName){
		newName = uniqueMissionID.intValue + "_";
		if (mechanicsPart1Type.intValue == 0)
			newName += "Targ" + HowManyTimeGrp_1.intValue;
		if (mechanicsPart1Type.intValue == 1)
			newName += "Roll" + HowManyTimeGrp_1.intValue;
		if (mechanicsPart1Type.intValue == 2)
			newName += "Bump" + HowManyTimeGrp_1.intValue;
		if (mechanicsPart1Type.intValue == 3)
			newName += "Spin" + HowManyTimeGrp_1.intValue;
		if (mechanicsPart1Type.intValue == 4)
			newName += "Hole" + HowManyTimeGrp_1.intValue;

		return newName;
	}

	private string RenameMissionPart2(string newName){
		newName += "_";
		if (mechanicsPart2Type.intValue == 1)
			newName += "Targ" + HowManyTimeGrp_2.intValue;
		if (mechanicsPart2Type.intValue == 2)
			newName += "Roll" + HowManyTimeGrp_2.intValue;
		if (mechanicsPart2Type.intValue == 3)
			newName += "Bump" + HowManyTimeGrp_2.intValue;
		if (mechanicsPart2Type.intValue == 4)
			newName += "Spin" + HowManyTimeGrp_2.intValue;
		if (mechanicsPart2Type.intValue == 5)
			newName += "Hole" + HowManyTimeGrp_2.intValue;

		return newName;
	}

	private void Display_Other_Options (){
		// Part 1 Options 
		EditorGUILayout.LabelField ("Global Mission Parameters",EditorStyles.boldLabel);

		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("Init Mission When ball Lost:", GUILayout.Width (180));
		EditorGUILayout.PropertyField (InitMissionWhenBallLost, new GUIContent (""), GUILayout.Width (20));
		EditorGUILayout.EndHorizontal ();
		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("Allow Mission to be paused:", GUILayout.Width (180));
		EditorGUILayout.PropertyField (b_PauseMissionMode, new GUIContent (""), GUILayout.Width (20));
		EditorGUILayout.EndHorizontal ();





		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("Access Mission Text", GUILayout.Width (180));
		EditorGUILayout.PropertyField (b_DisplayText, new GUIContent (""), GUILayout.Width (120));
		EditorGUILayout.EndHorizontal ();
		if (b_DisplayText.boolValue) {
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Mission Name", GUILayout.Width (180));
			EditorGUILayout.PropertyField (mission_Txt_Name, new GUIContent (""), GUILayout.Width (120));
			EditorGUILayout.EndHorizontal ();

			for (var i = 0; i < missionText.arraySize; i++) {
				if (i == 4 || i == 5 || i == 11) {
				} else {
					EditorGUILayout.BeginHorizontal ();
					EditorGUILayout.LabelField (missionTextInfo.GetArrayElementAtIndex (i).stringValue, GUILayout.Width (180));
					EditorGUILayout.PropertyField (missionText.GetArrayElementAtIndex (i), new GUIContent (""), GUILayout.Width (120));
					EditorGUILayout.EndHorizontal ();
				}
			}
		}
		EditorGUILayout.LabelField ("");



		EditorGUILayout.LabelField ("Options During Mission Part 1",EditorStyles.boldLabel);
		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("Keep Leds from Part 1 On:", GUILayout.Width (180));
		EditorGUILayout.PropertyField (KeepLedGrp1OnDuringMission, new GUIContent (""), GUILayout.Width (20));
		EditorGUILayout.EndHorizontal ();
		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("Led Part 1 in progress:", GUILayout.Width (180));
		EditorGUILayout.PropertyField (Led_Part1_In_Progress, new GUIContent (""), GUILayout.Width (120));
		EditorGUILayout.EndHorizontal ();
		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("Led Part 2 in progress:", GUILayout.Width (180));
		EditorGUILayout.PropertyField (Led_Mission_In_Progress, new GUIContent (""), GUILayout.Width (120));
		EditorGUILayout.EndHorizontal ();
		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("Led Mission complete:", GUILayout.Width (180));
		EditorGUILayout.PropertyField (Led_Mission_Complete, new GUIContent (""), GUILayout.Width (120));
		EditorGUILayout.EndHorizontal ();

		EditorGUILayout.LabelField ("");
		// Other Options 




		// Part 2 Options 
		EditorGUILayout.LabelField ("Options During Mission Part 2",EditorStyles.boldLabel);

		EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Timer :", GUILayout.Width (180));
			EditorGUILayout.PropertyField (b_Mission_Timer, new GUIContent (""), GUILayout.Width (120));
		EditorGUILayout.EndHorizontal ();

// Timer
		if (b_Mission_Timer.boolValue) {
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Reset Timer after Hit :", GUILayout.Width (180));
			EditorGUILayout.PropertyField (b_Mission_Multi_Timer, new GUIContent (""), GUILayout.Width (120));
			EditorGUILayout.EndHorizontal ();
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Timer Duration :", GUILayout.Width (180));
			EditorGUILayout.PropertyField (Mission_Time, new GUIContent (""), GUILayout.Width (120));
			EditorGUILayout.EndHorizontal ();
		}
		else {
			if (b_Mission_Multi_Timer.boolValue)
				b_Mission_Multi_Timer.boolValue = false;
		}

		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("Multiball :", GUILayout.Width (180));
		EditorGUILayout.PropertyField (multiball, new GUIContent (""), GUILayout.Width (120));
		EditorGUILayout.EndHorizontal ();

// Multiball
		if (multiball.boolValue) {
			if (mechanicsPart2Type.intValue == 2) {
				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField ("Number of balls :", GUILayout.Width (180));
				EditorGUILayout.PropertyField (numberOfBall, new GUIContent (""), GUILayout.Width (120));
				EditorGUILayout.EndHorizontal ();
				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField ("Jackpot points by hit :", GUILayout.Width (180));
				EditorGUILayout.PropertyField (jackpotPoints, new GUIContent (""), GUILayout.Width (120));
				EditorGUILayout.EndHorizontal ();
			} else {
				EditorGUILayout.HelpBox ("Multiball could be use only with Rollovers setup in Mission Part 2", MessageType.Warning);
			}
		}

		EditorGUILayout.LabelField ("");
		EditorGUILayout.LabelField ("Options When Mission is complete",EditorStyles.boldLabel);
// Points
		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("Points when mission is complete :", GUILayout.Width (180));
		EditorGUILayout.PropertyField (points, new GUIContent (""), GUILayout.Width (120));
		EditorGUILayout.EndHorizontal ();


		if (!multiball.boolValue) {
// Kickback
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Add cabinet Kickback :", GUILayout.Width (180));
			EditorGUILayout.PropertyField (enablekickback, new GUIContent (""), GUILayout.Width (120));
			EditorGUILayout.EndHorizontal ();

			if (enablekickback.boolValue) {			
				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField ("Begin ball with Kickback On:", GUILayout.Width (180));
				EditorGUILayout.PropertyField (beginWithKickback, new GUIContent (""), GUILayout.Width (120));
				EditorGUILayout.EndHorizontal ();
			}



// Bonus 
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Add Bonus :", GUILayout.Width (180));
			selectedBonus.intValue = EditorGUILayout.Popup ("", selectedBonus.intValue, tableBonusList, EditorStyles.popup);
			EditorGUILayout.EndHorizontal ();

			if (selectedBonus.intValue == 3) {			// Ball Saver
			
				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField ("Duration :", GUILayout.Width (180));
				EditorGUILayout.PropertyField (BallSaverDuration, new GUIContent (""), GUILayout.Width (120));
				EditorGUILayout.EndHorizontal ();
			}
		}
	}


	void OnSceneGUI( )
	{
	}
}
#endif