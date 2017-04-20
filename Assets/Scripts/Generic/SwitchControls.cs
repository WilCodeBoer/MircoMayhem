using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchControls : MonoBehaviour {

	CarController car;
	public List<string> controllers = new List<string> ();

	void Awake(){
		car = this.gameObject.GetComponent<CarController> ();

		foreach(string controller in Input.GetJoystickNames()){
			if (controller != "") {
				controllers.Add (controller);
			}
		}
			
		if (controllers.Count < 2) {
			car.Controls = CarController.CurrentInput.Keyboard;
		} else {
			if (car.gameObject.tag == "Player") {
				car.Controls = CarController.CurrentInput.Controller1;
			} else if (car.gameObject.tag == "player2") {
				car.Controls = CarController.CurrentInput.Controller2;
			}
		}
	}

	void Update () {
		if (Input.GetButton ("ChangeControls") && car.Controls == CarController.CurrentInput.Controller2) {
			car.Controls = CarController.CurrentInput.Keyboard;
		} else if (Input.GetKey (KeyCode.Tab) && car.Controls == CarController.CurrentInput.Keyboard) {
			car.Controls = CarController.CurrentInput.Controller2;
		}
	}
}
