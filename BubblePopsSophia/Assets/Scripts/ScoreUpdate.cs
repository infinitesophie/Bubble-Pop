using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScoreUpdate : MonoBehaviour
{
    public  float levelScoreNeeded;
    public Slider ScoreSlider;
    public AudioSource bubbleBurstSound;
    void Start()
    {
        PlayerPrefs.SetInt("playerScore", 0);
        ScoreSlider.value = PlayerPrefs.GetInt("playerScore", 0);
        ScoreSlider.maxValue = levelScoreNeeded;
    }

    void Update()
    {
        ScoreSlider.value = PlayerPrefs.GetInt("playerScore", 0);

        if (ScoreSlider.value == levelScoreNeeded)
        {
            bubbleBurstSound.Play();
            PlayerPrefs.SetInt("playerScore", 0);
            SceneManager.LoadScene("EndGame");
        }
    }
}
