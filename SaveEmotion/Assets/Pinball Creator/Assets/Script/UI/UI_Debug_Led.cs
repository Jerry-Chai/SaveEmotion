// UI_Debug_Led : Description : use to test leds patterns
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Debug_Led : MonoBehaviour {

	public Text Gui_Txt_Timer;						// Connect a UI.Text

	private GameObject obj_Game_Manager;				// Represent Object Manager_Game on the hierarchy
	private MissionIndex missionIndex;					// Used to Access Mission_Index component (You find Mission_Index.js on each Mission)
	private Manager_Game gameManager;					// Used to Access Manager_Game component (You find Manager_Game.js on Manager_Game object on the hierachy)

	private int anim;
	[Header ("-> Leds Group (Mission, group of leds)")]
	public bool Group_Led = false;				// true if you want to test a group leds.
	public GameObject Obj_Grp;					// Connect the group of leds
	private Manager_Led_Animation obj_Grp;
	[Header ("-> Global Pattern (Manage by Manager_Game)")]
	public bool Global_Led = false;				// true if you want to test global pattern using more than one group of leds

	private int cmpt = 0;
	private int HowManyAnim;



	void Start () {
		if(Global_Led){
			if (obj_Game_Manager == null)									// Connect the Mission to the gameObject : "Manager_Game"
				obj_Game_Manager = GameObject.Find("Manager_Game");
			gameManager = obj_Game_Manager.GetComponent<Manager_Game>();	// --> Connect the Mission to <Manager_Game>() component. 
			HowManyAnim = gameManager.HowManyAnimation();
		}
		if(Group_Led){
			obj_Grp = Obj_Grp.GetComponent<Manager_Led_Animation>();
			HowManyAnim = obj_Grp.HowManyAnimation();
		}

		Gui_Txt_Timer.text = cmpt.ToString();
	}

	public void PlayLedAnim () {
		if(Global_Led)
			gameManager.PlayMultiLeds(cmpt);
		if(Group_Led)
			obj_Grp.Play_New_Pattern(cmpt);
	}

	public void  _PressButton() {
		cmpt++;
		cmpt = cmpt%HowManyAnim;
		Gui_Txt_Timer.text = cmpt.ToString();
	}

}
