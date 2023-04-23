// Target : Description : Manage the target mechanics
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour {
	[Header ("Drop target or stationary target")]
	public bool b_Drop_Target = true;			// if true : Drop target / false : stationary target

	[Header ("Infos to missions")]	
	public int index;						// choose a number. Used to create script mission.
	public GameObject[] Parent_Manager;				// Connect on the inspector the missions that use this object
	public string functionToCall = "Counter";		// Call a function when OnCollisionEnter -> true;

	[Header ("Force to drop the target")]	
	public float MinMagnitude = .5f;

	private bool b_MoveObject = false;
	private float target = 0;

	[Header ("Local Position if activate or deactivate")]
	public float ActivatePosY = .11f;				// local position when the target is activate
	public float DesactivatePosY = -.25f;				// local position when the target is deactivate
	public float MoveSpeed = 5;				// speed to reach the target

	[Header ("Sound fx")]
	public AudioClip Sfx_Hit;				// Sound when the target is hit
	public AudioClip Sfx_ActivateDesactivate;				// Sound when you activate or activate
	public float volume_Deactivate = .5f;
	public float volume_Activate = .5f;
	private AudioSource  sound_;				// Audio component

	[Header ("Points when Target is hit")]
	public int Points = 1000;				// Points you win when the object is hitting 
	private GameObject obj_Game_Manager;				// Use to connect the gameObject Manager_Game
	private Manager_Game gameManager;				// Manager_Game Component from obj_Game_Manager

	[Header ("Toy connected to the Target")]					// Connect a GameObject or paticule system with the script Toys.js attached
	public GameObject Toy;
	private Toys toy;
	public int AnimNum = 0;


	void Start() {																	// --> Init
		if(transform.localPosition.y == ActivatePosY)									// init variable target
			target = ActivatePosY;
		else
			target = DesactivatePosY;

		obj_Game_Manager = GameObject.Find("Manager_Game");								// Find the gameObject Manager_Game

		if(obj_Game_Manager!=null)
			gameManager = obj_Game_Manager.GetComponent<Manager_Game>();					// Access Manager_Game from obj_Game_Manager
		sound_ = GetComponent<AudioSource>();											// Access AudioSource Component
		if(Toy)toy = Toy.GetComponent<Toys>();											// access Toys component if needed
	}

	void Update () {																// --> Update
		if(b_MoveObject){																// Move the target



			float YPos = Mathf.MoveTowards(transform.localPosition.y, target, MoveSpeed*Time.deltaTime);

			transform.localPosition = new Vector3(
				transform.localPosition.x,
				YPos,
				transform.localPosition.z
			);

			if(transform.localPosition.y == target){
				b_MoveObject = false;
			}
		}
	}

	void OnCollisionEnter(Collision collision) {									// --> when the ball enter on collision with the targget
		if (collision.relativeVelocity.magnitude > MinMagnitude && !b_MoveObject){		// minimum magnitude et the Target don't move.	
			if(b_Drop_Target)	
				Desactivate_Object();													// Desactivate Object			

			for(var j = 0;j<Parent_Manager.Length;j++){
				Parent_Manager[j].SendMessage(functionToCall,index);					// Call Parents Mission script
			}

			if(!sound_.isPlaying && Sfx_Hit)sound_.PlayOneShot(Sfx_Hit,1);				// Play a sound if needed

			if(obj_Game_Manager!=null){
				if(gameManager)gameManager.F_Mode_BONUS_Counter();											// Send Message to the gameManager(Manager_Game.js) Add 1 to BONUS_Global_Hit_Counter
				if(gameManager)gameManager.Add_Score(Points);												// Send Message to the gameManager(Manager_Game.js) Add Points to Add_Score
			}
			if(Toy)toy.PlayAnimationNumber(AnimNum);									// Play toy animation if needed
		}
	}

	public void Desactivate_Object(){														// --> Desactivate the target
		if(!sound_.isPlaying && Sfx_ActivateDesactivate && target == ActivatePosY){
			sound_.PlayOneShot(Sfx_ActivateDesactivate,volume_Deactivate);}													
		target = DesactivatePosY;
		b_MoveObject = true;
	}

	public void  Activate_Object(){															// --> Activate the target
		if(!sound_.isPlaying && Sfx_ActivateDesactivate && target == DesactivatePosY){
			sound_.PlayOneShot(Sfx_ActivateDesactivate,volume_Activate);}
		target = ActivatePosY;
		b_MoveObject = true;
	}


	public int index_info(){																// return the index of the object. Use by missions
		return index;
	}
}
