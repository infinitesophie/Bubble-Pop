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
    public int BubbleValue;
    [HideInInspector]
    public bool visited;
    [HideInInspector]
    public bool connected;

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
        if (column % 2 == 0)
        {
            bubblePosition.y -= grid.TILE_SIZE * 0.5f;
        }
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
        this.BubbleValue = GetValue((int)type);
        if (type == BUBBLE_TYPE.NONE)
            return;

        colorsGO[(int)type].SetActive(true);
    }

    
    public void SetNextType(BUBBLE_TYPE type, int matches)
    {

        foreach (GameObject go in colorsGO)
        {
            go.SetActive(false);
        }
       
        int newType = GetBubbleTypeFromValue(matches * GetValue((int)type));
        //this logic will multiply all similar bubbles and give the value of the new sum
        this.type = (BUBBLE_TYPE)newType;
        this.BubbleValue = GetValue(newType);
        if (type == BUBBLE_TYPE.NONE)
            return;

        colorsGO[newType].SetActive(true);
    }

    public int GetValue(int BubbleType)
    {
        switch (BubbleType)
        { case 0:
                return 2;
            case 1:
                return 4;
            case 2:
                return 8;
            case 3:
                return 16;
            case 4:
                return 32;
            case 5:
                return 64;
            case 6:
                return 128;
            case 7:
                return 256;
            case 8:
                return 512;
            case 9:
                return 1024;
        }
        return 2;
    }

    public int GetBubbleTypeFromValue(int BubbleTypeValue)
    {
       
            if(BubbleTypeValue>0 && BubbleTypeValue<=2)
                return 0;
            else if(BubbleTypeValue > 2 && BubbleTypeValue <=4)
                return 1;
            else if (BubbleTypeValue > 4 && BubbleTypeValue <= 8)
            return 2;
            else if (BubbleTypeValue > 8 && BubbleTypeValue <= 16)
            return 3;
            else if (BubbleTypeValue > 16 && BubbleTypeValue <= 32)
            return 4;
            else if (BubbleTypeValue > 32 && BubbleTypeValue <= 64)
            return 5;
            else if (BubbleTypeValue > 64 && BubbleTypeValue <= 128)
            return 6;
            else if (BubbleTypeValue > 128 && BubbleTypeValue <= 256)
            return 7;
            else if (BubbleTypeValue > 256 && BubbleTypeValue <= 512)
            return 8;
            else if (BubbleTypeValue > 512 && BubbleTypeValue <= 1024)
            return 9;
        else
        return 0;
    }
}
