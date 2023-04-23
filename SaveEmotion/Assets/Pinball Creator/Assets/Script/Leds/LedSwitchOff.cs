// LedSwitchOff Description : Use to switch off leds on bumpers and singshot
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedSwitchOff : MonoBehaviour {

	private ChangeSpriteRenderer Led;
	private float tmp_Time = 0;
	public float Timer 	= .2f;
	private bool b_Led_On_With_Timer = true;

	void Start () {
		Led = GetComponent<ChangeSpriteRenderer>();
	}

	public void Timer_Led(){
		b_Led_On_With_Timer = false;
	}

	void Update(){											
		if(!b_Led_On_With_Timer){										// Used with the function Led_On_With_Timer(value : float)							
			tmp_Time = Mathf.MoveTowards(tmp_Time,Timer,
				Time.deltaTime);
			if(tmp_Time == Timer){										// if time is finish we init the time an switch off the Led 						
				b_Led_On_With_Timer = true;
				tmp_Time = 0;
				Led.F_ChangeSprite_Off();
			}
		}
	}
}
