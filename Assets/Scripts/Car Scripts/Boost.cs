using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boost : MonoBehaviour {
    void OnTriggerEnter(Collider other) {
        if(other.tag == "Player") {
            Debug.Log(other);
            other.GetComponent<CarController>().ownRB.AddRelativeForce(other.gameObject.GetComponent<CarController>().moveDirection * 75 * Time.deltaTime * 5);
        }
    }
}
