// Pinball_TriggerForBall : Description : Detect when a ball is lost : Use by Out_Hole_TriggerDestroyBall gameObject on the hierarchy

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pinball_TriggerForBall : MonoBehaviour {

	private GameObject obj_Game_Manager;											// Manager_Game GameObject
	private Manager_Game gameManager;											// access Manager_Game component from Manager_Game GameObject on the hierarchy

	void Start(){																	// --> Function Start
		if (obj_Game_Manager == null)													// Connect the Mission to the gameObject : "Manager_Game"
			obj_Game_Manager = GameObject.Find("Manager_Game");

		gameManager = obj_Game_Manager.GetComponent<Manager_Game>();					// Access Manager_Game gameComponent from obj_Game_Manager
	}

	void OnTriggerEnter (Collider other) {										// --> Function OnTriggerEnter
		if(other.transform.tag == "Ball"){												// If it's a ball 
			gameManager.gamePlay(other.gameObject);										// Send Message to the obj_Game_Manager.  
		}
	}

}
