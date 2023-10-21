// Ball.js : Description : this script manage the ball (sound, collision, trail)
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Ball : MonoBehaviour {
	private Rigidbody 			rb;									// Ball RigidBody Component
	private TrailRenderer 		trail;                              // Ball TrailRenderer Component

	public Vector2 ballDir;  // urrent direction of the ball
	[Header ("The maximum speed of the ball")]
	//public float 				maxSpeed = 25;						// change the ball Max speed on table. If you change the cabinet scale you probably must increase the Max speed
	public float 				speed = 15;						// we dont expet ball speed change

	[Header ("Trail")]														
	public float 				Speed_To_Activate_Trail = .5f;		// The Trail become active when ball speed is superior to Speed_To_Activate_Trail
	private bool 				b_trail = true;

	private Vector3 			tmp_vel;							// Used with the function Ball_Pause()



	[Header ("Roll Sound")]	
	private AudioSource 		roll_audio	;						// roll AudioSource Component
	private bool 				once = false;						// Boolean used to play roll sound only if the sound is not playing
	public float 				min_Mag_roll_audio = 1;				// The minimum speed to play the roll sound
	private float 				tmp_Save_Min_Mag = 0;

	public bool 				b_Shake = false;
	public float 				Shake_Force = 2;

	private bool 				b_OnHole = false;					// Use to know if ball is on a hole or not


	void Start() {													// --> function Start			
		rb = GetComponent<Rigidbody>();									// Access <Rigidbody>() Component;	
		trail = GetComponent<TrailRenderer>();							// Access <TrailRenderer>() Component;	


		roll_audio = GetComponent<AudioSource>();						// Access <AudioSource>() Component; if roll sound is selected on the inspector

	}

	public void Ball_Shake(Vector3 Direction){
		if(rb)rb.AddForce(Direction*Shake_Force, ForceMode.VelocityChange);	
	}


	void FixedUpdate()															// --> Fixed Update : FixedUpdate is used to deal with Physics
	{
		//if(rb.velocity.magnitude > maxSpeed)										// Limit ball speed.
		//{
		//	rb.velocity 			
		//	= rb.velocity.normalized * maxSpeed;
		//}

        if (rb.velocity.magnitude != speed)                                       // Limit ball speed.
        {
            rb.velocity
            = rb.velocity.normalized * speed;
        }

        if (b_trail && rb.velocity.magnitude > Speed_To_Activate_Trail)				// Enable ball trail.
		{
			trail.enabled=true; 
			b_trail = false;
		}
		if(!b_trail && rb.velocity.magnitude < Speed_To_Activate_Trail)				// Desable ball trail.
		{
			trail.enabled=false; 
			b_trail = true;
		}

		if(rb.velocity.magnitude > min_Mag_roll_audio && once)						// Play the roll sound.
		{
			roll_audio.Play();
			once = false;
		}
		else if(rb.velocity.magnitude <= min_Mag_roll_audio && !once)				// Stop the roll sound.
		{
			roll_audio.Stop();
			roll_audio.pitch = 1;
			once = true;
		}

		if(!once && tmp_Save_Min_Mag == 0){
			roll_audio.pitch = rb.velocity.magnitude/2.5f;							// When ball accelerate the pitch increase. 
		}

        ballDir = new Vector2(rb.velocity.x, rb.velocity.z);						// Know the direction of the ball
		ballDir = ballDir.normalized;                                               // Normalize the direction of the ball


		
    }


	public void Ball_Pause(){													// --> Function Call When the game is on pause
		if(!b_OnHole){
			if(!rb.isKinematic){													// Start Pause
				tmp_vel = rb.velocity;
				rb.isKinematic = true;
			}
			else{																	// Stop Pause
				rb.isKinematic = false;
				rb.velocity = tmp_vel;
			}	
		}	
	}

	public void OnHole(){														// --> Know if ball is on a hole. Prevent bug when a ball enter a hole and player press pause.
		b_OnHole = true;
	}

	public void OutsideHole(){															
		b_OnHole = false;
	}


	public void OnTriggerEnter(Collider other){
		//Debug.Log("Collide");
		//if (other.tag == "Bricks") 
		//{
		//	other.gameObject.SetActive(false);
		//}
		if(other.transform.tag == "Ramp_Sound" && tmp_Save_Min_Mag == 0){
			Debug.Log("Ramp SOund Start");
			tmp_Save_Min_Mag = min_Mag_roll_audio;
			min_Mag_roll_audio = 0;
		}
		else if(other.transform.tag == "Ramp_Sound"){
			Debug.Log("Ramp SOund Stop ");
			min_Mag_roll_audio = tmp_Save_Min_Mag;
			tmp_Save_Min_Mag = 0;
		}
	}

	public void Shoot(Vector3 dir, float Speed) 
	{
        this.GetComponent<SphereCollider>().enabled = true;
        Rigidbody ballRB = this.GetComponent<Rigidbody>();
        ballRB.velocity = dir * Speed;
    }
}
