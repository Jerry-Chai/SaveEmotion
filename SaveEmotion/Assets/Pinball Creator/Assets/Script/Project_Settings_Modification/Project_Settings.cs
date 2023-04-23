// Project_Settings : Description : Use to init the global Time paramaters and Physics parameters of the game
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Project_Settings : MonoBehaviour {

	[Header ("Edit->Project Settings->Time")]	
	public float init_FixedTimestep = 0.002f;
	public float init_MaximumAllowedTime = 0.03333333f;
	public float init_TimeScale = 1;

	[Header ("Edit->Project Settings->Physics")]
	public Vector3 init_Gravity = new Vector3(0,-9.81f,0);
	public float init_BounceTreshold = 0.05f;
	public float init_SleepThreshold = 0.01f;
	public float init_DefaultContactOffset = .0025f;
	public int init_SolverIterationCount = 7;

	void Awake () {
		// init Time param
		Time.fixedDeltaTime 		= init_FixedTimestep;
		Time.maximumDeltaTime		= init_MaximumAllowedTime;
		Time.timeScale 				= init_TimeScale;
		// init Physics param
		Physics.gravity				= init_Gravity;
		Physics.bounceThreshold 	= init_BounceTreshold;
		Physics.sleepThreshold		= init_SleepThreshold;
		Physics.defaultContactOffset= init_DefaultContactOffset;
		Physics.defaultSolverIterations= init_SolverIterationCount;
	}

}
