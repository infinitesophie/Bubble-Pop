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
    public Text Score;
    void Start()
    {
        PlayerPrefs.SetInt("playerScore", 0);
        ScoreSlider.value = PlayerPrefs.GetInt("playerScore", 0);
        ScoreSlider.maxValue = levelScoreNeeded;
    }

    void Update()
    {
        
        ScoreSlider.value = PlayerPrefs.GetInt("playerScore", 0);
        Score.text = ScoreSlider.value.ToString();
        if (ScoreSlider.value == levelScoreNeeded)
        {
            bubbleBurstSound.Play();
            PlayerPrefs.SetInt("playerScore", 0);
            StartCoroutine(WaitASec(4));
            SceneManager.LoadScene("EndGame");
        }
        
    }
    IEnumerator WaitASec(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        foreach (GameObject o in Object.FindObjectsOfType<GameObject>())
        {
            Destroy(o);
        }

    }
}
