using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class HandleMenu : MonoBehaviour {

    public Button single, multi, quit;
    public Image loader, loading;
	
	void Start () {
        single.onClick.AddListener(loadSingleScene);
        multi.onClick.AddListener(loadMultiScene);
        quit.onClick.AddListener(quitScene);
	}

    void loadSingleScene() {
	    single.gameObject.SetActive(false);
	    multi.gameObject.SetActive(false);
	    quit.gameObject.SetActive(false);
	    loading.gameObject.SetActive(true);
	    loader.gameObject.SetActive(true);
	    SceneManager.LoadSceneAsync(1);
    }

    void loadMultiScene() {
        single.gameObject.SetActive(false);
        multi.gameObject.SetActive(false);
        quit.gameObject.SetActive(false);
        loading.gameObject.SetActive(true);
        loader.gameObject.SetActive(true);
        SceneManager.LoadSceneAsync(2);
    }

    void quitScene() {
        Application.Quit();
    } 
}
