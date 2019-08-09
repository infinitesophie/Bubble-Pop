using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    public enum BUBBLE_TYPE
    {
        NONE = -1,
        TYPE_1,
        TYPE_2,
        TYPE_3,
        TYPE_4,
        TYPE_5,
        TYPE_6,
        TYPE_7,
        TYPE_8,
        TYPE_9,
        TYPE_10
    }
    public GameObject[] colorsGO;

    [HideInInspector]
    public int row;

    [HideInInspector]
    public int column;

    [HideInInspector]
    public BUBBLE_TYPE type;

    [HideInInspector]
    public bool visited;

    private Vector3 bubblePosition;

    private BubblesGrid grid;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "ShotBubble")
        {
            ShotBubble b = other.gameObject.GetComponent<ShotBubble>();
            grid.AddBubble(this, b);
        }

    }

    public void SetBubblePosition(BubblesGrid grid, int column, int row)
    {

        this.grid = grid;
        this.column = column;
        this.row = row;

        bubblePosition = new Vector3((column * grid.TILE_SIZE) - grid.GRID_OFFSET_X, grid.GRID_OFFSET_Y + (-row * grid.TILE_SIZE), 0);

        transform.localPosition = bubblePosition;

        foreach (GameObject go in colorsGO)
        {
            go.SetActive(false);
        }
    }


    public void SetType(BUBBLE_TYPE type)
    {

        foreach (GameObject go in colorsGO)
        {
            go.SetActive(false);
        }

        this.type = type;

        if (type == BUBBLE_TYPE.NONE)
            return;

        colorsGO[(int)type].SetActive(true);
    }
}
