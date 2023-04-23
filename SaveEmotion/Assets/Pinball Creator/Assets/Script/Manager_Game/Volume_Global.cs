// Volume_Global : Description : Use on AudioListener on the hierarchy to manage global scene volume
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Volume_Global : MonoBehaviour {
	
	public float volume_Global = 1;				// Set the vulume needed

	void Start () {
		if (PlayerPrefs.GetString ("SoundIs") == "Enabled" || !PlayerPrefs.HasKey ("SoundIs") ) {
			AudioListener.volume = volume_Global;
		}
		else {
			AudioListener.volume = 0;
		}
	}

	public void ChangeVolume(float vol) {		// Call this function if you want to change the global volume
		AudioListener.volume = vol;
	}

	public void MuteSound () {
		if (AudioListener.volume == 0) {
			AudioListener.volume = .5F;
			PlayerPrefs.SetString ("SoundIs", "Enabled");
		} 
		else {
			AudioListener.volume = 0;
			PlayerPrefs.SetString ("SoundIs", "Muted");
		}
	}




}
