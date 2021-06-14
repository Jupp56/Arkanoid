using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HighScore : MonoBehaviour
{
    private Text highscoreText;
    private int score;

    private string saveFilePath = Path.Combine(
                        System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments),
                        "My Games",
                        "Arkanoid for Windows",
                        "savegame.save");

    private HighscoreSave highScoreSave;

    private string activeScene;

    public int Score
    {
        get => score;
        set
        {
            score = value;
            highscoreText.text = "Highscore: " + score.ToString();
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        activeScene = SceneManager.GetActiveScene().name;

        highscoreText = GetComponent<Text>();
        try
        {
            BinaryFormatter b = new BinaryFormatter();
            highScoreSave = (HighscoreSave)b.Deserialize(File.OpenRead(saveFilePath));

            if (activeScene == "Predefined")
            {
                Score = highScoreSave.predefinedScore;

            }
            else
            {
                Score = highScoreSave.generatedScore;
            }
        }
        catch (System.Exception ex)
        {
            Score = 0;
            highScoreSave = new HighscoreSave();
        }
    }


    ///Sets the highscore to score if score is higher than the current highscore.
    public void CompareAndSetHighscore(int score)
    {
        if (score > Score)
        {
            Score = score;
        }
    }

    public void SaveScore()
    {
        if (activeScene == "Predefined")
        {
            highScoreSave.predefinedScore = score;
        }
        else
        {
            highScoreSave.generatedScore = score;
        }

        BinaryFormatter b = new BinaryFormatter();
        Directory.CreateDirectory(
           Path.GetDirectoryName(saveFilePath)
            ); 
        FileStream fs = new FileStream(saveFilePath, FileMode.Create);

        b.Serialize(fs, highScoreSave);

        fs.Close();
    }

    // Update is called once per frame
    void Update()
    {

    }
}

[System.Serializable]
public class HighscoreSave
{
    public int generatedScore;
    public int predefinedScore;
}
