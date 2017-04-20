using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MultiPlayerController : MonoBehaviour {

	public GameObject splitScreen, singleScreen, firstUI, secondUI, player,secondPlayer;

    public enum GameMode { SinglePlayer, MultiPlayer };
    public GameMode gameMode;

    void Update() {
        switch (gameMode) {
            case GameMode.SinglePlayer:
                singleScreen.SetActive(true);
                secondPlayer.SetActive(false);
                splitScreen.SetActive(false);
                secondUI.SetActive(false);
                if(SceneManager.GetActiveScene().name != "SinglePlayer") {
                    SceneManager.LoadScene(1);
                }
                break;
            case GameMode.MultiPlayer:
                splitScreen.SetActive(true);
                secondUI.SetActive(true);
                secondPlayer.SetActive(true);
                singleScreen.SetActive(false);
                if (SceneManager.GetActiveScene().name != "MultiPlayer") {
                    SceneManager.LoadScene(0);
                }
                break;
            default:
                break;
        }
    }
}
