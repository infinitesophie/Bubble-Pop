using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevel : MonoBehaviour
{
    public void ToGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void ToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
