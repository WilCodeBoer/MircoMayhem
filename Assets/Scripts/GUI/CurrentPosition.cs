using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrentPosition : MonoBehaviour {

    public CarController MainPlayer;
    public CarController ExtraPlayer;

    public Text position; 

	void Update () {
		if(MainPlayer.distanceTraveled > ExtraPlayer.distanceTraveled) {
            position.text = "Currently in 1st place";
        } else if (MainPlayer.distanceTraveled < ExtraPlayer.distanceTraveled) {
            position.text = "Currently in 2nd place";
        } else if(MainPlayer.distanceTraveled == ExtraPlayer.distanceTraveled) {
            position.text = "Currently Tied";
        }
	}
}
