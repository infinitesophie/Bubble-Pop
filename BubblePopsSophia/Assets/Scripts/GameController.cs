using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public Slider levelProgressBar;

    private void Update()
    {
        levelProgressBar.value = 20f;
    }
}
