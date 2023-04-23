// Manager_Grp_Leds.js : Description : Init a group of leds
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager_Grp_Leds : MonoBehaviour {


	public GameObject[] obj_Led;		// Connect Leds you want on this group

	void Start () {									// --> Init
		GetComponent<Manager_Led_Animation>().Init_Obj_Led_Animation(obj_Led);	// Init Script Manager_Led_Animation.js
	}



}
