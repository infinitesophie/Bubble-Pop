using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubblesGrid : MonoBehaviour
{
    
    [HideInInspector]
    public float GRID_OFFSET_X = 0;
    [HideInInspector]
    public float GRID_OFFSET_Y = 0;
    [HideInInspector]
    public List<List<Bubble>> gridBubbles;
    public int ROWS = 6;
    public int COLUMNS = 6;
    public float TILE_SIZE = 0.68f;
    public float changeTypeRate = 0.5f;
    public int lines = 5;
    public GameObject gridBubbleGO;
    public GameObject BubbleBurstEffect;

    private List<Bubble.BUBBLE_TYPE> bubbleTypePool;
    private Bubble.BUBBLE_TYPE lastType;
    private GameObject newBubbleGO;
    private List<Bubble> matchList;

    void Start()
    {
        matchList = new List<Bubble>();
        lastType = (Bubble.BUBBLE_TYPE)Random.Range(0, 7);
        bubbleTypePool = new List<Bubble.BUBBLE_TYPE>();
        //approximately creating a 10000 bubble type for the level
        int i = 0;
        int total = 10000;
        while (i < total)
        {
            bubbleTypePool.Add(GetBubbleType());
            i++;
        }
        
        Shuffle(bubbleTypePool);

        BuildGrid();
    }

    Bubble.BUBBLE_TYPE GetBubbleType()
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
        GRID_OFFSET_Y = 4.3f;

        GRID_OFFSET_X -= TILE_SIZE * 0.5f;
        GRID_OFFSET_Y -= TILE_SIZE * 0.5f;


        for (int row = 0; row < ROWS; row++)
        {

            List<Bubble> rowBubbles = new List<Bubble>();
            for (int column = 0; column < COLUMNS; column++)
            {
                //instantiate a GO from the pool
                GameObject item = Instantiate(gridBubbleGO) as GameObject;
                Bubble bubble = item.GetComponent<Bubble>();
                
                bubble.SetBubblePosition(this, column, row);
                bubble.SetType(bubbleTypePool[0]);
                bubbleTypePool.RemoveAt(0);

                bubble.transform.parent = gameObject.transform;
                rowBubbles.Add(bubble);
                //only a few lines are made visible rest all are hidden
                if (gridBubbles.Count > lines)
                {
                    bubble.gameObject.SetActive(false);
                }
            }

            gridBubbles.Add(rowBubbles);
        }
    }
    public void AddLine()
    {
        //does top line have visible bubbles
        bool emptyFirstRow = true;
        foreach (Bubble b in gridBubbles[0])
        {
            if (b.gameObject.activeSelf)
            {
                emptyFirstRow = false;
                break;
            }
        }
        
        if (!emptyFirstRow)
        {
            int rowCount = ROWS - 2;
            while (rowCount >= 0)
            {
                foreach (Bubble b in gridBubbles[rowCount])
                {
                    if (b.gameObject.activeSelf)
                    {
                        gridBubbles[rowCount + 1][b.column].gameObject.SetActive(true);
                        gridBubbles[rowCount + 1][b.column].SetType(b.type);
                    }
                    else
                    {
                        gridBubbles[rowCount + 1][b.column].gameObject.SetActive(false);
                    }
                }
                rowCount--;
            }
        }

        foreach (Bubble b in gridBubbles[0])
        {
            b.SetType(bubbleTypePool[0]);
            bubbleTypePool.RemoveAt(0);
            b.gameObject.SetActive(true);
        }
    }

    public void AddBubble(Bubble collisionBubble, ShotBubble shotBubble)
    {

        List<Bubble> neighbors = BubbleEmptyNeighbors(collisionBubble);
        float minDistance = 10000.0f;
        Bubble newBubble = null;

        foreach (Bubble n in neighbors)
        {
            float d = Vector2.Distance(n.transform.position, shotBubble.transform.position);
            if (d < minDistance)
            {
                minDistance = d;
                newBubble = n;
            }
        }
        shotBubble.gameObject.SetActive(false);
        newBubble.SetType(shotBubble.type);
        newBubble.gameObject.SetActive(true);

        CheckMatchesForBall(newBubble);
    }

    
    List<Bubble> BubbleEmptyNeighbors(Bubble bubble)
    {
        var result = new List<Bubble>();
        if (bubble.column + 1 < COLUMNS)
        {
            if (!gridBubbles[bubble.row][bubble.column + 1].gameObject.activeSelf)
                result.Add(gridBubbles[bubble.row][bubble.column + 1]);
        }

        //left
        if (bubble.column - 1 >= 0)
        {
            if (!gridBubbles[bubble.row][bubble.column - 1].gameObject.activeSelf)
                result.Add(gridBubbles[bubble.row][bubble.column - 1]);
        }
        //top
        if (bubble.row - 1 >= 0)
        {
            if (!gridBubbles[bubble.row - 1][bubble.column].gameObject.activeSelf)
                result.Add(gridBubbles[bubble.row - 1][bubble.column]);
        }
        //bottom
        if (bubble.row + 1 < ROWS)
        {
            if (!gridBubbles[bubble.row + 1][bubble.column].gameObject.activeSelf)
                result.Add(gridBubbles[bubble.row + 1][bubble.column]);
        }
        if (bubble.column % 2 == 0)
        {
            //bottom-left
            if (bubble.row + 1 < ROWS && bubble.column - 1 >= 0)
            {
                if (!gridBubbles[bubble.row + 1][bubble.column - 1].gameObject.activeSelf)
                    result.Add(gridBubbles[bubble.row + 1][bubble.column - 1]);
            }

            //bottom-right
            if (bubble.row + 1 < ROWS && bubble.column + 1 < COLUMNS)
            {
                if (!gridBubbles[bubble.row + 1][bubble.column + 1].gameObject.activeSelf)
                    result.Add(gridBubbles[bubble.row + 1][bubble.column + 1]);
            }
        }
        else
        {
            //top-left
            if (bubble.row - 1 >= 0 && bubble.column - 1 >= 0)
            {
                if (!gridBubbles[bubble.row - 1][bubble.column - 1].gameObject.activeSelf)
                    result.Add(gridBubbles[bubble.row - 1][bubble.column - 1]);
            }

            //top-right
            if (bubble.row - 1 >= 0 && bubble.column + 1 < COLUMNS)
            {
                if (!gridBubbles[bubble.row - 1][bubble.column + 1].gameObject.activeSelf)
                    result.Add(gridBubbles[bubble.row - 1][bubble.column + 1]);
            }
        }
        return result;
    }

    public void CheckMatchesForBall(Bubble bubble)
    {
        matchList.Clear();

        for (int row = 0; row < ROWS; row++)
        {
            for (int column = 0; column < COLUMNS; column++)
            {
                gridBubbles[row][column].visited = false;
            }
        }

        //search for matches around ball
        var initialResult = GetMatches(bubble);
        matchList.AddRange(initialResult);

        while (true)
        {

            var allVisited = true;
            for (var i = matchList.Count - 1; i >= 0; i--)
            {
                var b = matchList[i];
                if (!b.visited)
                {
                    AddMatches(GetMatches(b));
                    allVisited = false;
                }
            }

            if (allVisited)
            {
                if (matchList.Count > 1)
                {

                    foreach (var b in matchList)
                    {
                        b.gameObject.SetActive(false);
                        Instantiate(BubbleBurstEffect, b.gameObject.transform.position, Quaternion.identity);
                    }

                    CheckForDisconnected();

                    //remove disconnected balls
                    var i = 0;
                    while (i < ROWS)
                    {
                        foreach (var b in gridBubbles[i])
                        {
                            if (!b.connected && b.gameObject.activeSelf)
                            {
                                b.gameObject.SetActive(false);
                                Instantiate(BubbleBurstEffect, b.gameObject.transform.position, Quaternion.identity);
                            }
                        }
                        i++;
                    }
                }
                return;
            }
        }
    }
    List<Bubble> GetMatches(Bubble bubble)
    {
        bubble.visited = true;
        var result = new List<Bubble>() { bubble };
        var n = BubbleActiveNeighbors(bubble);

        foreach (var b in n)
        {
            if (b.type == bubble.type)
            {
                result.Add(b);
            }
        }

        return result;
    }

    void AddMatches(List<Bubble> matches)
    {
        foreach (var b in matches)
        {
            if (!matchList.Contains(b))
                matchList.Add(b);
        }
    }
    List<Bubble> BubbleActiveNeighbors(Bubble bubble)
    {
        var result = new List<Bubble>();
        if (bubble.column + 1 < COLUMNS)
        {
            if (gridBubbles[bubble.row][bubble.column + 1].gameObject.activeSelf)
                result.Add(gridBubbles[bubble.row][bubble.column + 1]);
        }

        //left
        if (bubble.column - 1 >= 0)
        {
            if (gridBubbles[bubble.row][bubble.column - 1].gameObject.activeSelf)
                result.Add(gridBubbles[bubble.row][bubble.column - 1]);
        }
        //top
        if (bubble.row - 1 >= 0)
        {
            if (gridBubbles[bubble.row - 1][bubble.column].gameObject.activeSelf)
                result.Add(gridBubbles[bubble.row - 1][bubble.column]);
        }

        //bottom
        if (bubble.row + 1 < ROWS)
        {
            if (gridBubbles[bubble.row + 1][bubble.column].gameObject.activeSelf)
                result.Add(gridBubbles[bubble.row + 1][bubble.column]);
        }
        if (bubble.column % 2 == 0)
        {
            //bottom-left
            if (bubble.row + 1 < ROWS && bubble.column - 1 >= 0)
            {
                if (gridBubbles[bubble.row + 1][bubble.column - 1].gameObject.activeSelf)
                    result.Add(gridBubbles[bubble.row + 1][bubble.column - 1]);
            }

            //bottom-right
            if (bubble.row + 1 < ROWS && bubble.column + 1 < COLUMNS)
            {
                if (gridBubbles[bubble.row + 1][bubble.column + 1].gameObject.activeSelf)
                    result.Add(gridBubbles[bubble.row + 1][bubble.column + 1]);
            }
        }
        else
        {
            //top-left
            if (bubble.row - 1 >= 0 && bubble.column - 1 >= 0)
            {
                if (gridBubbles[bubble.row - 1][bubble.column - 1].gameObject.activeSelf)
                    result.Add(gridBubbles[bubble.row - 1][bubble.column - 1]);
            }

            //top-right
            if (bubble.row - 1 >= 0 && bubble.column + 1 < COLUMNS)
            {
                if (gridBubbles[bubble.row - 1][bubble.column + 1].gameObject.activeSelf)
                    result.Add(gridBubbles[bubble.row - 1][bubble.column + 1]);
            }
        }

        return result;
    }

    public Bubble BallCloseToPoint(Vector2 point)
    {
        
        int c = Mathf.FloorToInt((point.x + GRID_OFFSET_X + (TILE_SIZE * 0.5f)) / TILE_SIZE);
        if (c < 0)
            c = 0;
        if (c >= COLUMNS)
            c = COLUMNS - 1;

        int r = Mathf.FloorToInt((GRID_OFFSET_Y + (TILE_SIZE * 0.5f) - point.y) / TILE_SIZE);
        if (r < 0) r = 0;
        if (r >= ROWS) r = ROWS - 1;

        return gridBubbles[r][c];

    }

    void CheckForDisconnected()
    {
        //set all balls as disconnected
        foreach (var r in gridBubbles)
        {
            foreach (var b in r)
            {
                b.connected = false;
            }
        }
        //connect visible balls in first row 
        foreach (var b in gridBubbles[0])
        {
            if (b.gameObject.activeSelf)
                b.connected = true;
        }

        //now set connect property on the rest of the balls
        var i = 1;
        while (i < ROWS)
        {
            foreach (var b in gridBubbles[i])
            {
                if (b.gameObject.activeSelf)
                {
                    var neighbors = BubbleActiveNeighbors(b);
                    var connected = false;

                    foreach (var n in neighbors)
                    {
                        if (n.connected)
                        {
                            connected = true;
                            break;
                        }
                    }

                    if (connected)
                    {
                        b.connected = true;
                        foreach (var n in neighbors)
                        {
                            if (n.gameObject.activeSelf)
                            {
                                n.connected = true;
                            }
                        }
                    }
                }
            }
            i++;
        }
    }
}
