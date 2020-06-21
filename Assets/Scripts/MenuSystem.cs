using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuSystem : MonoBehaviour {

    public Text scoreText;

    public void Update() {
        UpdateUi();
    }

    void UpdateUi() {
        scoreText.text = Game.currentScore.ToString();
    }
    
    public void PlayAgain() {
        SceneManager.LoadScene("Level");
    }
}