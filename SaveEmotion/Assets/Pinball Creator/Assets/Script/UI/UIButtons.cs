// Description UIButtons.cs : This script is used in association with UI buttons. USe to call functions when player press the button
// Unity Ads function too
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


#if UNITY_ADS
using UnityEngine.Advertisements;
#endif

public class UIButtons : MonoBehaviour {

	private MainMenu 					obj_MainMenu;										// gameobject ScrollMenu_Manager on the Hierarchy									
	private G_HubMenu					obj_G_Menu;											// gameObject G_Menu on the Hierarchy

	public bool							RewardsAds = false;

	public GameObject					Loading_Screen;										// Black screen when a table is loading
	public Text							LoadingText;

	public SpriteRenderer				S_CurrentBackground;								// Use to Display sprite for the background sprite

	public Text 						TextTableName;

	public bool 						b_lock = true;										// Check if it is possible to launch a scene


	void Awake(){
		Application.backgroundLoadingPriority = ThreadPriority.BelowNormal;
		GameObject tmpObj = GameObject.Find("ScrollMenu_Manager");							// Find GameObject that manage buttons used to load Scene	

		if(tmpObj)obj_MainMenu = tmpObj.GetComponent<MainMenu>();

		GameObject tmpButtonPlay = GameObject.Find ("Button_Play");
		if(obj_MainMenu.ButtonLock && PlayerPrefs.GetString(obj_MainMenu.SceneName[obj_MainMenu.CurrentScene] +"_Lock") == "Unlocked"){
			obj_MainMenu.ButtonLock.SetActive(false);

			if (tmpButtonPlay)
				tmpButtonPlay.GetComponent<UIButtons> ().b_lock = false;
		}
		else{
			if(obj_MainMenu.ButtonLock)obj_MainMenu.ButtonLock.SetActive(true);

			if (tmpButtonPlay)
				tmpButtonPlay.GetComponent<UIButtons> ().b_lock = true;
		}

	}


	public void LastScene(){										// --> Main Menu : Display the last table on screen
		if(obj_MainMenu){

		
			obj_MainMenu.CurrentScene --;

			if(obj_MainMenu.CurrentScene < 0)
				obj_MainMenu.CurrentScene = obj_MainMenu.SceneName.Count-1;

			if(obj_MainMenu.S_LastScene && obj_MainMenu.CurrentScene !=0)
				obj_MainMenu.S_LastScene.GetComponent<Image>().sprite = obj_MainMenu.TableSpriteList[(obj_MainMenu.CurrentScene-1)% obj_MainMenu.SceneName.Count];
			else if(obj_MainMenu.S_LastScene && obj_MainMenu.CurrentScene == 0)
				obj_MainMenu.S_LastScene.GetComponent<Image>().sprite = obj_MainMenu.TableSpriteList[obj_MainMenu.SceneName.Count-1];
			
			if(obj_MainMenu.S_CurrentScene)obj_MainMenu.S_CurrentScene.GetComponent<Image>().sprite = obj_MainMenu.TableSpriteList[obj_MainMenu.CurrentScene% obj_MainMenu.SceneName.Count];
			if(obj_MainMenu.S_NextScene)obj_MainMenu.S_NextScene.GetComponent<Image>().sprite = obj_MainMenu.TableSpriteList[(obj_MainMenu.CurrentScene+1)% obj_MainMenu.SceneName.Count];


			GameObject tmpButtonPlay = GameObject.Find ("Button_Play");

			if(obj_MainMenu.ButtonLock && PlayerPrefs.GetString(obj_MainMenu.SceneName[obj_MainMenu.CurrentScene] +"_Lock") == "Unlocked"){
				obj_MainMenu.ButtonLock.SetActive(false);

				if (tmpButtonPlay)
					tmpButtonPlay.GetComponent<UIButtons> ().b_lock = false;
			}
			else{
				if(obj_MainMenu.ButtonLock)obj_MainMenu.ButtonLock.SetActive(true);

				if (tmpButtonPlay)
					tmpButtonPlay.GetComponent<UIButtons> ().b_lock = true;
			}

			S_CurrentBackground.sprite = obj_MainMenu.TableBackgroundSpriteList[obj_MainMenu.CurrentScene];
			S_CurrentBackground.transform.localScale = new Vector3(obj_MainMenu.TableVector2SpriteList[obj_MainMenu.CurrentScene].x,obj_MainMenu.TableVector2SpriteList[obj_MainMenu.CurrentScene].y,1);
			if(TextTableName)TextTableName.text = obj_MainMenu.TableName[obj_MainMenu.CurrentScene];	

			PlayerPrefs.SetInt ("LastTableLoaded", obj_MainMenu.CurrentScene);
		}
	}

	public void NextScene(){				// --> Main Menu : Display the next table on screen
		if(obj_MainMenu){
			obj_MainMenu.CurrentScene ++;

			if(obj_MainMenu.S_LastScene)obj_MainMenu.S_LastScene.GetComponent<Image>().sprite = obj_MainMenu.TableSpriteList[(obj_MainMenu.CurrentScene-1)% obj_MainMenu.SceneName.Count];
			if(obj_MainMenu.S_CurrentScene)obj_MainMenu.S_CurrentScene.GetComponent<Image>().sprite = obj_MainMenu.TableSpriteList[obj_MainMenu.CurrentScene% obj_MainMenu.SceneName.Count];
			if(obj_MainMenu.S_NextScene)obj_MainMenu.S_NextScene.GetComponent<Image>().sprite = obj_MainMenu.TableSpriteList[(obj_MainMenu.CurrentScene+1)% obj_MainMenu.SceneName.Count];

			obj_MainMenu.CurrentScene = obj_MainMenu.CurrentScene % obj_MainMenu.SceneName.Count;

			GameObject tmpButtonPlay = GameObject.Find ("Button_Play");
			if(obj_MainMenu.ButtonLock && PlayerPrefs.GetString(obj_MainMenu.SceneName[obj_MainMenu.CurrentScene] +"_Lock") == "Unlocked"){
				obj_MainMenu.ButtonLock.SetActive(false);

				if (tmpButtonPlay)
					tmpButtonPlay.GetComponent<UIButtons> ().b_lock = false;
			}
			else{
				if(obj_MainMenu.ButtonLock)obj_MainMenu.ButtonLock.SetActive(true);

				if (tmpButtonPlay)
					tmpButtonPlay.GetComponent<UIButtons> ().b_lock = true;
			}

			S_CurrentBackground.sprite = obj_MainMenu.TableBackgroundSpriteList[obj_MainMenu.CurrentScene];
			S_CurrentBackground.transform.localScale = new Vector3(obj_MainMenu.TableVector2SpriteList[obj_MainMenu.CurrentScene].x,obj_MainMenu.TableVector2SpriteList[obj_MainMenu.CurrentScene].y,1);
			if(TextTableName)TextTableName.text = obj_MainMenu.TableName[obj_MainMenu.CurrentScene];	

			PlayerPrefs.SetInt ("LastTableLoaded", obj_MainMenu.CurrentScene);
		}
	}

	public void Unlock_A_Lock () {																					// --> Load a new scene that contain a new Scene game
		if(obj_MainMenu.TextTableName)
			obj_MainMenu.TextTableName.text = obj_MainMenu.TableName[obj_MainMenu.CurrentScene];						// Display the name of the next table

		if(obj_MainMenu.ButtonLock && PlayerPrefs.GetString(obj_MainMenu.SceneName[obj_MainMenu.CurrentScene] +"_Lock") == "Unlocked"){
			LoadASceneWhenPlayIsPressed_MainMenu();																		// Call function to Load a scene
		}
		else if(obj_MainMenu.ButtonLock){
			#if UNITY_ADS
				if (RewardsAds)
					ShowRewardedAd();
				else
					ShowDefaultAd();

			#else
				UnlockIfNo_UnityAds();
			#endif
		}
		else if(!obj_MainMenu.ButtonLock){
			LoadASceneWhenPlayIsPressed_MainMenu();																		// Call function to Load a scene
		}
	}


	public void LoadASceneWhenPlayIsPressed_MainMenu(){																	// Call when button Play is pressed on Main Menu
		if (!b_lock)
			StartCoroutine ("AsynchronousLoad");
		else
			Unlock_A_Lock();
	}


	IEnumerator AsynchronousLoad ()																					// --> Load a scene (Pinball Table)
	{
		if(Loading_Screen)Loading_Screen.gameObject.SetActive(true);													// Activate the loading screen
		yield return new WaitForEndOfFrame();

		AsyncOperation a = SceneManager.LoadSceneAsync(obj_MainMenu.SceneName[obj_MainMenu.CurrentScene]);				// Load a scene

		a.allowSceneActivation = false;																					// Do not activated the loading until allowSceneActivation = true 

		while (!a.isDone)
		{
			float progress = Mathf.Clamp01(a.progress / 0.9f);
			if(LoadingText)LoadingText.text = "Loading " + (progress * 100).ToString("n0") + "%";

			if (a.progress == 0.9f){																					// Loading completed
				a.allowSceneActivation = true;}

			yield return null;
		}
	}


		
	public void F_QuitGame(){																						// --> Quit the application
		Application.Quit();
	}



	public void UIButtonActivateLeaderboard(){																		// --> Display leaderboard on screen
		if(!obj_G_Menu){while(!Find_GrpMenu()){}}

		if(obj_G_Menu)obj_G_Menu.SectionLeaderBoard.SetActive(true);
	}

	public void UIButtonDeactivateLeaderboard(){																	// --> Deactivate the leaderboard
		if(!obj_G_Menu){while(!Find_GrpMenu()){}}

		if(obj_G_Menu)obj_G_Menu.SectionLeaderBoard.SetActive(false);
	}

	public void UIButtonUpdateLeaderboardSystem(){																	// --> Update leaderboard values
		if(!obj_G_Menu){while(!Find_GrpMenu()){}}

		if(obj_G_Menu)obj_G_Menu.LeaderBoardSystem.UpdateLeaderboard();
	}

	public void UIButtonActivateQuitGameMenu(){																		// --> activate the gameObject Quit_Scene_Game
		if(!obj_G_Menu){while(!Find_GrpMenu()){}}

		if(obj_G_Menu)obj_G_Menu.SectionQuitSceneGame.SetActive(true);
	}

	public void UIButtonDeactivateQuitGameMenu(){																	// --> deactivate the gameObject Quit_Scene_Game
		if(!obj_G_Menu){while(!Find_GrpMenu()){}}

		if(obj_G_Menu)obj_G_Menu.SectionQuitSceneGame.SetActive(false);
	}
		

	public void UIButtonInitPauseGameButtonSection(){																// --> Use to go back to Main menu to init the gameObject Quit_Scene_Game and Button_Pause
		if(!obj_G_Menu){while(!Find_GrpMenu()){}}

		if(obj_G_Menu){
			obj_G_Menu.ButtonPauseSceneGame.SetActive(true);
			obj_G_Menu.SectionPauseSceneInGame.SetActive(false);
		}
	}


	bool Find_GrpMenu(){
		GameObject tmpObj = GameObject.Find("G_Simple_Menu");					// Find GameObject that manage buttons used to load Scene	
		if(tmpObj){obj_G_Menu = tmpObj.GetComponent<G_HubMenu>();}
		else{
			tmpObj = GameObject.Find("G_Menu");									// Find GameObject that manage buttons used to load Scene	
			if(tmpObj)obj_G_Menu = tmpObj.GetComponent<G_HubMenu>();
		}
		return true;
	}


	/// <summary>
	/// Unity Ad Section
	/// </summary>
	public void ShowDefaultAd()
	{
		#if UNITY_ADS
		if (!Advertisement.IsReady())
		{
			Debug.Log("Ads not ready for default placement");
			return;
		}

		Advertisement.Show();
		#endif
	}

	public void ShowRewardedAd()
	{
		const string RewardedPlacementId = "rewardedVideo";

		#if UNITY_ADS
		if (!Advertisement.IsReady(RewardedPlacementId))
		{
			Debug.Log(string.Format("Ads not ready for placement '{0}'", RewardedPlacementId));
			return;
		}

		var options = new ShowOptions { resultCallback = HandleShowResult };
		Advertisement.Show(RewardedPlacementId, options);
		#endif
	}

	#if UNITY_ADS
	private void HandleShowResult(ShowResult result)
	{
		switch (result)
		{
			case ShowResult.Finished:
				Debug.Log("The ad was successfully shown.");
				//
				
				F_Reward();															// Call the function that unlock a Scene Game corresponding to the button pressed by the player 
				break;
			case ShowResult.Skipped:
				Debug.Log("The ad was skipped before reaching the end.");
				break;
			case ShowResult.Failed:
				Debug.LogError("The ad failed to be shown.");
				break;
		}
	}
	#endif

	void UnlockIfNo_UnityAds(){
		// YOUR CODE TO Unlock THE GAME
		//F_Reward();																// Call the function that unlock a Scene Game corresponding to the button pressed by the player 
		Debug.Log("Script your code to unlock this Scene");
	}

	void F_Reward(){																							// --> This function allow to unlock a Scene Game

		// YOUR CODE TO REWARD THE GAMER

		PlayerPrefs.SetString(obj_MainMenu.SceneName[obj_MainMenu.CurrentScene] +"_Lock","Unlocked");			// Unlock Scene Game
		b_lock = false;
		if(obj_MainMenu)obj_MainMenu.ButtonLock.SetActive(false);
		if(obj_MainMenu)obj_MainMenu.UnlockOrLock [obj_MainMenu.CurrentScene] = true;
	}



}
