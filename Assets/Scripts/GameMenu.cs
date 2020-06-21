using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMenu : MonoBehaviour {
    public Text levelText;
    public Text highScoreText;

    private void Start() {
        highScoreText.text = PlayerPrefs.GetInt("tetris.highscore").ToString();
    }

    public void PlayGame() {
        SceneManager.LoadScene("Level");
    }

    public void ChangedValue(float value) {
        Game.startingLevel = (int)value;
        levelText.text = value.ToString();
    }
    
    
}