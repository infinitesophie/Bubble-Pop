using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBall : MonoBehaviour
{
    public GameObject[] bubblesGO;

    public Bubble.BUBBLE_TYPE type;

    public void SetType(Bubble.BUBBLE_TYPE type)
    {

        foreach (GameObject go in bubblesGO)
        {
            go.SetActive(false);
        }

        this.type = type;
        Debug.Log((int)type);

        bubblesGO[(int)type].SetActive(true);
    }

}
