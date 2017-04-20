using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pauze : MonoBehaviour {

    public bool pauzed = true;
    public GameObject pauseScreen;

	void Start () {
        pauseScreen.SetActive(true);
        Time.timeScale = 0;
	}

	void Update () {
        if (Input.GetButtonUp("start") && !pauzed) {
            Time.timeScale = 0;
            pauzed = true;
            pauseScreen.SetActive(true);
        }
    }
}
