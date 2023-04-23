// switch : Description : Manage Switch table mechanics
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class switchMech : MonoBehaviour {
	public int index;
	public GameObject[] Parent_Manager;
	public string functionToCall = "Counter";	// Call a function when OnCollisionEnter -> true;

	public AudioClip Sfx_Hit;
	private AudioSource  sound_;

	public  int Points = 1000;			// Points you win when the object is hitting 
	private GameObject obj_Game_Manager;
	private Manager_Game gameManager;

	void Awake(){
		Physics.IgnoreLayerCollision(8,12, true);
	}
	void Start(){
		obj_Game_Manager = GameObject.Find("Manager_Game");
		gameManager = obj_Game_Manager.GetComponent<Manager_Game>();	
		sound_ = GetComponent<AudioSource>();
	}


	void OnCollisionEnter(Collision collision) {
		if(collision.transform.tag == "Ball"){
			for(var j = 0;j<Parent_Manager.Length;j++){
				Parent_Manager[j].SendMessage(functionToCall,index);			// Call Parents Mission script
			}

			if(!sound_.isPlaying && Sfx_Hit)sound_.PlayOneShot(Sfx_Hit);		// Play a sound

			if(gameManager)gameManager.F_Mode_BONUS_Counter();									// Add Points to bonus counter
			if(gameManager)gameManager.Add_Score(Points);										// Add point to score
		}
	}


}
