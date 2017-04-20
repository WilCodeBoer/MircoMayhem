using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayRandomMusic : MonoBehaviour {

	AudioSource[] music;
	public AudioSource currentClip;

	void Awake () {
		music = gameObject.GetComponents<AudioSource> ();
		currentClip = music [Random.Range (0, music.Length)];
	}

	void Start(){
		currentClip.Play();
	}
}
