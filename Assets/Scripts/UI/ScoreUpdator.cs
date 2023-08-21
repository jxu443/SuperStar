using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class ScoreUpdator : MonoBehaviour
{
    public Text scoreText;
    public Text finishLabel;
    public int totalScore = 10;

    private int curScore = 0;
    
    
    public int CurScore
    {
        get => curScore;
        set => curScore = value;
    }
    

    void Update()
    {
        scoreText.text = "晶体收集: " + curScore + "/" + totalScore;
        if (curScore == totalScore)
        {
            finishLabel.text = "You've done it! Awesome!";
            finishLabel.color = new Color(0.2f, 0.8f, 0.2f);
            
            finishLabel.gameObject.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("Level1_Maze", LoadSceneMode.Single);
        }
       
    }
}
