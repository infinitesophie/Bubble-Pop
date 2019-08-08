using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject[] colorsGO;

    public Bubble.BALL_TYPE type;

    public void SetType(Bubble.BALL_TYPE type)
    {

        foreach (var go in colorsGO)
        {
            go.SetActive(false);
        }

        this.type = type;
        Debug.Log((int)type);

        colorsGO[(int)type].SetActive(true);
    }

}
