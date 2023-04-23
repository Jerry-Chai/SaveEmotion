// Description : MainMenu.cs : Manage buttons to load multiple Scene game
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class MainMenu : MonoBehaviour {

	public List<string> 	TableName 	= new List<string>(); 					// The name of the Table you wanted to load
	public List<string> 	SceneName 	= new List<string>(); 					// The name of the scene you wanted to load
	public List<bool> 		UnlockOrLock 	= new List<bool>(); 				// Know If the Scene is unlocked or locked
	public List<Sprite> 	TableSpriteList 	= new List<Sprite>(); 			// List of sprite for Scene same order as SceneName List
	public List<Sprite> 	TableBackgroundSpriteList 	= new List<Sprite>(); 	// List of sprite for Scene same order as SceneName List
	public List<Vector2> 	TableVector2SpriteList 	= new List<Vector2>(); 		// List of sprite for Scene same order as SceneName List



	[System.Serializable]
	public class _LeadName{
		public List<string> 	Names 	= new List<string>(); 					// Use to create name list for the leaderboard	// Use on custom editor
		public List<int> 	Score 	= new List<int>(); 							// Use to create score list for the leaderboard // Use on custom editor
	}


	public List<_LeadName> 	LeadName 	= new List<_LeadName>(); 					// Use to create name list for the leaderboard. Use on custom editor
	public List<bool> 		ShowLeaderboard 	= new List<bool>(); 				// Use on custom editor


	public bool 			ShowParts 	= false; 								// Used to show these next variable on Inspector
	public bool 			ShowPlayerPrefsInfos = false;						// Used to show PlayerPrefs Infos on Inspector

	public int 				CurrentScene = 0;									// Know which Scene is display on screen 
	public GameObject 		ButtonLock;											// Activate if the Selected Scene is locked
	public GameObject 		S_LastScene;										// Use to Display sprite for the preview Scene 
	public GameObject 		S_CurrentScene;										// Use to Display sprite for the current	
	public GameObject 		S_NextScene;										// Use to Display sprite for the next Scene

	public SpriteRenderer	S_CurrentBackground;								// Use to Display sprite for the background sprite

	public Text 			TextTableName;										// Use to Display Table name on screen	

	public int 				CheckModification;									// use to visualize info on the custom Editor

	// Use this for initialization		
	void Awake(){																					// --> Initialisation
		int LastTableLoaded = 0;
		if (PlayerPrefs.HasKey ("LastTableLoaded")) {
			LastTableLoaded = PlayerPrefs.GetInt ("LastTableLoaded");
		}
		else{ 
			PlayerPrefs.SetInt ("LastTableLoaded", 0);
			LastTableLoaded = 0;
			Debug.Log ("Start for the first Time");								// Init Leadeboard the first time the game start


			for (int i = 0; i < LeadName.Count; i++) {							// Generate leaderboards
				string tmpListNameAndScore = "";
			
				for (int j = 0; j < LeadName [i].Names.Count; j++) {
					tmpListNameAndScore += LeadName[i].Names[j] + ",";
					tmpListNameAndScore += LeadName[i].Score[j].ToString () + ",";
					tmpListNameAndScore += "" + ",";
					tmpListNameAndScore += LeadName[i].Score[j] + ",";
				}


				PlayerPrefs.SetString (SceneName [i] + "_Lead", tmpListNameAndScore);	// Score (int)

			}

		}

		CurrentScene = LastTableLoaded;

		for (int i = 0;i< UnlockOrLock.Count;i++){
			if(UnlockOrLock[i] == true){										 
				PlayerPrefs.SetString(SceneName[i] +"_Lock","Unlocked");								// If true : Unlock this Scene
			}
		}
			
		if(ButtonLock && PlayerPrefs.GetString(SceneName[LastTableLoaded] +"_Lock") == "Unlocked"){					// Check if the first Scene Game is locked or unlocked
			ButtonLock.SetActive(false);}
		else{
			if(ButtonLock)ButtonLock.SetActive(true);}

		if(TextTableName)TextTableName.text = TableName[LastTableLoaded];													// Display the name of the first table
		S_CurrentBackground.sprite = TableBackgroundSpriteList[LastTableLoaded];
		S_CurrentBackground.transform.localScale = new Vector3(TableVector2SpriteList[LastTableLoaded].x,TableVector2SpriteList[LastTableLoaded].y,1);

		//if(S_LastScene)S_LastScene.GetComponent<Image>().sprite = TableSpriteList[TableSpriteList.Count-1];				// Display sprite for the preview Scene on the list
		if(S_CurrentScene)S_CurrentScene.GetComponent<Image>().sprite = TableSpriteList[LastTableLoaded];				// Display sprite for the current Scene on the list		
		//if(S_NextScene && TableSpriteList.Count>1)S_NextScene.GetComponent<Image>().sprite = TableSpriteList[1];		// Display sprite for the next Scene on the list

	}
		
}
