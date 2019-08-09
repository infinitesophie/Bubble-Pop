using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubblesGrid : MonoBehaviour
{
    public int ROWS = 6;

    public int COLUMNS = 6;

    public float TILE_SIZE = 0.68f;

    public float changeTypeRate = 0.5f;

    public int lines = 5;

    public GameObject gridBallGO;

    [HideInInspector]
    public float GRID_OFFSET_X = 0;

    [HideInInspector]
    public float GRID_OFFSET_Y = 0;

    [HideInInspector]
    public List<List<Bubble>> gridBubbles;

    private List<Bubble.BUBBLE_TYPE> bubbleTypePool;

    private Bubble.BUBBLE_TYPE lastType;




    void Start()
    {
        lastType = (Bubble.BUBBLE_TYPE)Random.Range(0, 5);
        bubbleTypePool = new List<Bubble.BUBBLE_TYPE>();
        //approximately creating a 10000 bubble type for the level
        int i = 0;
        int total = 10000;
        while (i < total)
        {
            bubbleTypePool.Add(GetBallType());
            i++;
        }
        
        Shuffle(bubbleTypePool);

        BuildGrid();
    }

    Bubble.BUBBLE_TYPE GetBallType()
    {
        float random = Random.Range(0.0f, 1.0f);
        //changeTypeRate ensures difficulty of level if its small value then the bubbles will be more of similar colours
        if (random > changeTypeRate)
        {
            lastType = (Bubble.BUBBLE_TYPE)Random.Range(0, 7);  
            //randomizing only to 7(which is 128) because I dont want bigger numbers to appear on grid 
        }
        return lastType;
    }

    private static System.Random randomNumberGenerator = new System.Random();


    public static void Shuffle<T>(IList<T> list)
    {
        //this method just shuffles and Reassigns a colour type to all 10000 bubbleTypes two at a time. Kind of like a mixing bowl
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = randomNumberGenerator.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    void BuildGrid()
    {
        gridBubbles = new List<List<Bubble>>();
        //the offset X and Y are points from where the grid starts
        GRID_OFFSET_X = (COLUMNS * TILE_SIZE) * 0.5f;
        GRID_OFFSET_Y = (ROWS * TILE_SIZE) * 0.5f;

        GRID_OFFSET_X -= TILE_SIZE * 0.5f;
        GRID_OFFSET_Y -= TILE_SIZE * 0.5f;


        for (int row = 0; row < ROWS; row++)
        {

            List<Bubble> rowBalls = new List<Bubble>();

            for (int column = 0; column < COLUMNS; column++)
            {

                var item = Instantiate(gridBallGO) as GameObject;
                var Bubble = item.GetComponent<Bubble>();

                Bubble.SetBubblePosition(this, column, row);
                Bubble.SetType(bubbleTypePool[0]);
                bubbleTypePool.RemoveAt(0);

                Bubble.transform.parent = gameObject.transform;
                rowBalls.Add(Bubble);

                if (gridBubbles.Count > lines)
                {
                    Bubble.gameObject.SetActive(false);
                }
            }

            gridBubbles.Add(rowBalls);
        }
    }
    public void AddLine()
    {
        //does top line have visible bubbles
        var emptyFirstRow = true;
        foreach (var b in gridBubbles[0])
        {
            if (b.gameObject.activeSelf)
            {
                emptyFirstRow = false;
                break;
            }
        }

        if (!emptyFirstRow)
        {
            var r = ROWS - 2;
            while (r >= 0)
            {
                foreach (var b in gridBubbles[r])
                {
                    if (b.gameObject.activeSelf)
                    {
                        gridBubbles[r + 1][b.column].gameObject.SetActive(true);
                        gridBubbles[r + 1][b.column].SetType(b.type);
                    }
                    else
                    {
                        gridBubbles[r + 1][b.column].gameObject.SetActive(false);
                    }
                }
                r--;
            }
        }

        foreach (var b in gridBubbles[0])
        {
            b.SetType(bubbleTypePool[0]);
            bubbleTypePool.RemoveAt(0);
            b.gameObject.SetActive(true);
        }
    }
    
    

}
