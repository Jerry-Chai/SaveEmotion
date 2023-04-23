using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionCreator : MonoBehaviour {
	public bool SeeInspector = false;


	//public int uniqueMissionIndex 	= 0;

	public string missionName = "M_";
	public int uniqueMissionID = 0;
	public GameObject p_MissionInit;
	public GameObject dropTarget;
	public GameObject stationaryTarget;
	public GameObject objLed;

	public bool InitMissionWhenBallLost = true;
	public bool b_PauseMissionMode = true;

	public int mechanicsPart1Type  = 0;
	public int mechanicsPart2Type  = 0;
	public string speText = "Word";

	public List<string> mechanicsPart1Text = new List<string> ();
	public List<string> mechanicsPart2Text = new List<string> ();
	public string mechTextPart1 = "";
	public string mechTextPart2 = "";

	// target Type Part 1
	public int targetPart1 = 0;
	public int targetType	= 0;

	// target Type Part 2
	public int targetPart2 = 0;
	public int targetType2	= 0;


	// Roloover Type Part 1
	public int HowManyRollover = 1;
	public GameObject objRollover;
	public int rolloverPart1 = 0;


	// Roloover Type Part 2
	public int HowManyRollover2 = 1;
	public int rolloverPart2 = 0;


	//Specific Text
	public bool SpecificText = false;
	public string rolloverSpeTextFalse = " Bumper hits";


	// Bumper
	public int bumperType	= 0;
	public int HowManyBumper = 3;
	public int bumperType2	= 0;
	public int HowManyBumper2 = 3;
	public GameObject objBumper_01;
	public GameObject objBumper_02;

	// Spinner
	public GameObject objSpinner;

	// Hole
	public GameObject objHole;




	public int HowManyTimeGrp_1 = 2;

	public bool addLedsWithPart1 = true;
	public int HowManyTimeGrp_2 = 2;

	public bool addLedsWithPart2 = true;




	// Other Options 
	public bool displayOtherOptions = false;
	public bool KeepLedGrp1OnDuringMission = false;
	public bool Led_Part1_In_Progress = false;
	public bool Led_Mission_In_Progress= false;
	public bool Led_Mission_Complete= false;
	public GameObject Led_Progress;


	public bool b_DisplayText = false;
	public string mission_Txt_Name = "-> Mission <-";
	public List<string> missionTextInfo = new List<string> ();
	public List<string> missionText = new List<string> ();

	// Part 2 Options 
	public bool b_Mission_Timer = false;
	public bool b_Mission_Multi_Timer = false;
	public int Mission_Time = 10;

	// MultiBall Options
	public bool multiball = false;
	public int numberOfBall = 3;
	public int jackpotPoints = 20000;

	// Options when mission is complete
	public int points = 20000;

	// Bonus
	public bool enablekickback = false;
	public int howManyKickback = 2;

	public int selectedBonus = 0;
	//public bool randomBonus = false;
	//public bool extraball = false;
	//public bool ballsaver = false;
	public int BallSaverDuration = 10;
	//public bool multiplier = false;

	//public bool kickback = false;
	public bool beginWithKickback = false;
	public GameObject kickbackGrp;





	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
