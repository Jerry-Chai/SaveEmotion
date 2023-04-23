// Toy_Skull_Eye_LookAt_Ball : Descripton : Skull eyes follow the ball
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toy_Skull_Eye_LookAt_Ball : MonoBehaviour {

	public bool pos_X = true;
	public bool pos_Y = true;
	public bool pos_Z = true;

	public Transform Ball;
	private float Ball_Speed;

	public bool b_follow = true;

	public Transform target_Fixed;

	void Update () {
		if(b_follow){
			if(Ball != null && Ball.transform.localPosition.z < -13.3){		// Look at ball depending the ball position
				transform.LookAt(Ball);
			}
			else{
				var targetObj = GameObject.FindGameObjectWithTag("Ball");
				if(targetObj != null){
					Ball = targetObj.transform;
				}
			}
		}
		else{
			transform.LookAt(target_Fixed);
		}

	}

}
