using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class TimePassed : MonoBehaviour {

    Text timePassed;
    float minutesPassed, secondsPassed;

	void Start () {
        timePassed = GetComponent<Text>() as Text;
	}
	
	// Update is called once per frame
	void Update () {
		minutesPassed = (int)(Time.timeSinceLevelLoad/60f);
		secondsPassed = (int)(Time.timeSinceLevelLoad % 60f);
        timePassed.text = "Time Passed: " + minutesPassed.ToString("00") + ":" + secondsPassed.ToString("00");      
	}
}
