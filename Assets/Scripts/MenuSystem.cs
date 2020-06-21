using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuSystem : MonoBehaviour {

    public Text scoreText;
    private bool updated = false;

    public void Update() {
        UpdateUi();
        updateHighScore();
    }

    void UpdateUi() {
        scoreText.text = Game.currentScore.ToString();
    }
    
    public void PlayAgain() {
        SceneManager.LoadScene("Level");
    }

    void updateHighScore() {
        if (Game.gameOver && !updated) {
            string p_scores = PlayerPrefs.GetString("tetris.highscore");
            string[] scores = p_scores.Split(';');
            int s1 = scores[0] != null ? Int32.Parse(scores[0]) : 0;
            int s2 = scores[1] != null ? Int32.Parse(scores[1]) : 0;
            int s3 = scores[2] != null ? Int32.Parse(scores[2]) : 0;
            List<int> listScores = new List<int>();
            listScores.Add(s1);
            listScores.Add(s2);
            listScores.Add(s3);
            listScores.Add(Game.currentScore);
            listScores.Sort();
            listScores.Reverse();

            s1 = listScores[0];
            s2 = listScores[1];
            s3 = listScores[2];
        

            string new_highscores = s1 + ";" + s2 + ";" + s3;
            if (new_highscores != p_scores) PlayerPrefs.SetString("tetris.highscore", new_highscores);
            updated = true;
        }
    }
}