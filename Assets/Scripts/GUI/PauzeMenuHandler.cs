using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class PauzeMenuHandler : MonoBehaviour {

	public Button cont, quit, p2Quit, p1Quit;
    public GameObject pauzeScreen;

	Pauze pauze;

	void Start () {
        pauze = FindObjectOfType<Pauze>();
        cont.onClick.AddListener(unpauze);
        quit.onClick.AddListener(backToMenu);
		p2Quit.onClick.AddListener(backToMenu);
		p1Quit.onClick.AddListener(backToMenu);
	}

    void unpauze() {
        Time.timeScale = 1;
        pauzeScreen.SetActive(false);
        pauze.pauzed = false;
    }

	void backToMenu() {
        SceneManager.LoadSceneAsync(0);
    }
}
