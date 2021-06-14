using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    private int scoreValue = 0;

    private Text scoreText;

    public GameObject highscore;
    
    public void Start()
    {
        scoreText = gameObject.GetComponent<Text>();
    }

    public int ScoreValue
    {
        get => scoreValue;
        /// sets the current score and updates the highscore if necessary
        set {
            scoreValue = value;
            highscore.GetComponent<HighScore>().CompareAndSetHighscore(value);
            scoreText.text = scoreValue.ToString();
        }
    }
}
