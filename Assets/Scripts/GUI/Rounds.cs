using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Rounds : MonoBehaviour {

    public int rounds = 1;
    public Text text;
    public GameObject endGameScreen;

    private void Awake(){
        rounds = 1;
    }

	void Update () {
        text.text = "Round: " + rounds;
        if(rounds == 4){
            Time.timeScale = 0;
            endGameScreen.SetActive(true);
            if (Input.GetButton("start")) {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                Time.timeScale = 1;
            }
        }
	}
}
