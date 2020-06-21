using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMenu : MonoBehaviour {
    public Text levelText;
    public Text highScoreText;
    public Text highScoreText2;
    public Text highScoreText3;

    private void Start() {
        // PlayerPrefs.SetString("tetris.highscore", "0;0;0");
        string p_scores = PlayerPrefs.GetString("tetris.highscore");
        string[] scores = p_scores.Split(';');
        int s1 = scores[0] != null ? Int32.Parse(scores[0]) : 0;
        int s2 = scores[1] != null ? Int32.Parse(scores[1]) : 0;
        int s3 = scores[2] != null ? Int32.Parse(scores[2]) : 0;
        highScoreText.text = s1.ToString();
        highScoreText2.text = s2.ToString();
        highScoreText3.text = s3.ToString();
    }

    public void PlayGame() {
        SceneManager.LoadScene("Level");
    }

    public void ChangedValue(float value) {
        Game.startingLevel = (int)value;
        levelText.text = value.ToString();
        
    }
    
    
}