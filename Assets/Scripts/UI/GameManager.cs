using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager: MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        
        if (Input.GetKeyDown(KeyCode.K))
        {
            SceneManager.LoadScene("Level1_Kitchen", LoadSceneMode.Single);
        }
        
        if (Input.GetKeyDown(KeyCode.L))
        {
            SceneManager.LoadScene("Level2_City", LoadSceneMode.Single);
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
        }
    }
}
