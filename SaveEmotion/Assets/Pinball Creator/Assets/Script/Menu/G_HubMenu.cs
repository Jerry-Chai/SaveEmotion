//Description : G_HubMenu.cs : This script is used to access gameObject for Menu Buttons. Use by script UIButtons.cs
using UnityEngine;
using System.Collections;

public class G_HubMenu : MonoBehaviour {
	public GameObject 			SectionMenu;									// Connect gameObject Canvas_Menu
	public GameObject 			SectionGame;									// Connect gameObject Canvas_Game
	public GameObject 			SectionQuitSceneGame;								// Connect gameObject Quit_Scene_Game
	public GameObject 			SectionPauseSceneInGame;							// Connect gameObject Quit_Scene_InGame
	public GameObject 			ButtonPauseSceneGame;								// Connect gameObject Button_Pause
	public GameObject 			SectionLeaderBoard;								// Connect gameObject Grp_CanvasLeaderboard
	public LeaderboardSystem 	LeaderBoardSystem;								// Connect gameObject Grp_CanvasLeaderboard
	//public CameraMovement 		MainCamera;										// Connect gameObject MainCamera

	void Start(){
		Component[] objs_Search = GetComponentsInChildren( typeof(RectTransform),true );

		foreach (RectTransform _obj in objs_Search) {
			if(_obj.gameObject.name == "Quit_Game")SectionQuitSceneGame = _obj.gameObject;
			if(_obj.gameObject.name == "Quit_Scene_InGame")SectionPauseSceneInGame = _obj.gameObject;
			if(_obj.gameObject.name == "Button_Pause")ButtonPauseSceneGame = _obj.gameObject;
			if(_obj.gameObject.name == "Canvas_Menu")SectionMenu = _obj.gameObject;
			if(_obj.gameObject.name == "Canvas_Game")SectionGame = _obj.gameObject;
		}


		foreach(Transform child in gameObject.transform){
				if(child.name == "Grp_CanvasLeaderboard"){
				SectionLeaderBoard = child.gameObject;
			}
		}

		GameObject tmpObj = GameObject.Find("Camera_Main");
		//if(tmpObj)MainCamera = tmpObj.GetComponent<CameraMovement>();

		tmpObj = GameObject.Find("LeaderboardSystem");
		if(tmpObj)LeaderBoardSystem = tmpObj.GetComponent<LeaderboardSystem>();
	}
}
