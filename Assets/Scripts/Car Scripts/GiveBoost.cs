using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveBoost : MonoBehaviour {
    void Awake() {
        foreach(GameObject boost in GameObject.FindGameObjectsWithTag("Boost")) {
      		if(!boost.GetComponent<Boost>()) boost.AddComponent<Boost>();
        }
    }
}
