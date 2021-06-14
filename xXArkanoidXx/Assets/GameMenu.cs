using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    public void StartCustomLevels()
    {
        SceneManager.LoadScene("Predefined");
    }

    public void StartGeneratedLevels()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
