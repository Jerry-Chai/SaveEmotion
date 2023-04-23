// UI_Call_A_Function: Description : Use by the UI button to connect Manager_Game to the button and to call a function

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_Call_A_Function : MonoBehaviour {

	public Manager_Game manager_Game;
	public GameObject obj_UI;
	public GameObject obj_UI2;
	public GameObject obj_PauseMobile;


	public Camera_Movement camera_Movement;
	public GameObject BlackScreen;


	void Start () {
		GameObject tmp = GameObject.Find("Manager_Game");
		if(tmp!=null)manager_Game = tmp.GetComponent<Manager_Game>();


		tmp = GameObject.Find("G_UI_Game_Interface_Mobile");
		if(tmp!=null)obj_UI = tmp;

		tmp = GameObject.Find("G_UI_Game_Interface_Mobile_Part2");
		if(tmp!=null)obj_UI2 = tmp;


		tmp = GameObject.Find("Main Camera");


		if(tmp!=null){
			camera_Movement = tmp.GetComponent<Camera_Movement>();
		}

		tmp = GameObject.Find("PauseAndView");


		if(tmp!=null){
			Transform[] children = tmp.GetComponentsInChildren<Transform>(true);
			foreach (Transform child in children) {
				if(child.name == "btn_Mobile_Pause")obj_PauseMobile = child.gameObject;
			}
		}
	}

	public void Debug_ChangeCam(){				camera_Movement.Selected_Cam();}	

	public void Debug_NewBall(){				manager_Game.F_NewBall();}	
	public void Debug_Ball_Saver_On(){			manager_Game.F_Ball_Saver_On();}	
	public void Debug_Ball_Saver_Off(){		manager_Game.F_Ball_Saver_Off();}	
	public void Debug_ExtraBall(){				manager_Game.F_ExtraBall();}	
	public void Debug_MultiBall(){				manager_Game.F_MultiBall();}	

	public void Debug_PlayMultiLeds(){			manager_Game.F_PlayMultiLeds();}	
	public void Debug_Init_All_Mission(){		manager_Game.F_Init_All_Mission();}	
	public void Debug_Start_Pause_Mode(){		manager_Game.F_Start_Pause_Mode();}	
	public void Debug_Stop_Pause_Mode(){		manager_Game.F_Stop_Pause_Mode();}	



	public void Debug_InsertCoin_GameStart(){	manager_Game.F_InsertCoin_GameStart();}	
	public void Debug_Pause_Game(){			manager_Game.F_Pause_Game();}	
	public void F_Quit_No(){					manager_Game.F_Quit_No();}	
	public void F_Quit_Yes(){					manager_Game.F_Quit_Yes();}	
	public void F_Exit_Game(){	Exit_Game();}	

	public void Exit_Game(){
		StartCoroutine("I_Exit_Game");
	}
	IEnumerator I_Exit_Game(){
		yield return new WaitForEndOfFrame();
		Application.Quit();
	}

	public void F_GoToMAinMenu(){	GoToMAinMenu();}
	public void GoToMAinMenu() {
		if(BlackScreen)BlackScreen.SetActive(true);

		StartCoroutine("I_F_GoToMAinMenu");
	}
	IEnumerator I_F_GoToMAinMenu(){
		yield return new WaitForEndOfFrame();
		SceneManager.LoadScene(0);				// Load the Main Menu
	}


	public void DeactivateEventSystem(){									// Deactivate UIs when you don't need them
		if(obj_PauseMobile)obj_PauseMobile.SetActive(true);
		if(obj_UI)obj_UI.SetActive(false);
		if(obj_UI2)obj_UI2.SetActive(false);
	}

	public void ActivateCameraViewButton(){

	}

	public void ActivateButtonPausemobile(){
		if(obj_PauseMobile)obj_PauseMobile.SetActive(true);
	}
}
