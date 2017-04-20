using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarFollow : MonoBehaviour {

    public Transform target;
    public float smoothRotateSpeed = 0.125f, smoothSpeed = 1f;
    public CarController car;

	void LateUpdate () {
	    if (!car.isFalling) {
	        transform.position = Vector3.Lerp(transform.position, target.position, smoothSpeed);
	        if (car.isGrounded) {             
	            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(new Vector3(0,target.eulerAngles.y,0)), smoothRotateSpeed);
	        }
	    }        
    }
}
