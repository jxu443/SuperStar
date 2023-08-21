using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LifeUpdater : MonoBehaviour
{
    public Text LifeText;
    public int DeathLimit = 0;
    public PhaseChange phaseChange;
    
    private int life = 100;
    private float duration;

    void Update()
    {
        if (!phaseChange.IsFluid)
        {
            LifeText.text = "软体模式, 不再蒸发水分";
            return;
        }
         
        duration += Time.deltaTime;
        if (duration >= 1f)
        {
            duration -= 1f;
            life -= 1;
            LifeText.text = life + "% 水含量";
        }
        
        if (life <= DeathLimit)
        {
            Time.timeScale = 0f; 
            LifeText.text = "死亡，按R键重来";
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            Time.timeScale = 1f;
            life = 100;
            SceneManager.LoadScene("Level1_Kitchen", LoadSceneMode.Single);
        }
    }
    
    public void AddLife(int delta)
    {
        life += delta;
        if (life > 100)
        {
            life = 100;
        }
    }
    

}
