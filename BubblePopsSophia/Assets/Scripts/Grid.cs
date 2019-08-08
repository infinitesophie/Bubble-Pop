using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public int ROWS = 20;

    public int COLUMNS = 14;

    public float TILE_SIZE = 0.68f;

    public float GRID_SPEED = 0.1f;

    public float changeTypeRate = 0.5f;

    public int emptyLines = 16;

    public GameObject gridBallGO;

    [HideInInspector]
    public float GRID_OFFSET_X = 0;


    [HideInInspector]
    public List<List<Bubble>> gridBalls;

    private List<Bubble> matchList;

    private List<Bubble.BALL_TYPE> typePool;

    private Bubble.BALL_TYPE lastType;

    private int bullets = 0;

    void Start()
    {

        matchList = new List<Bubble>();
        lastType = (Bubble.BALL_TYPE)Random.Range(0, 5);
        typePool = new List<Bubble.BALL_TYPE>();

        var i = 0;
        var total = 100000;
        while (i < total)
        {
            typePool.Add(GetBallType());
            i++;
        }

        Shuffle(typePool);

        BuildGrid();

    }


    void BuildGrid()
    {
        gridBalls = new List<List<Bubble>>();

        GRID_OFFSET_X = (COLUMNS * TILE_SIZE) * 0.5f;
        GRID_OFFSET_X -= TILE_SIZE * 0.5f;


        for (int row = 0; row < ROWS; row++)
        {

            var rowBalls = new List<Bubble>();

            for (int column = 0; column < COLUMNS; column++)
            {

                var item = Instantiate(gridBallGO) as GameObject;
                var ball = item.GetComponent<Bubble>();

                ball.SetBallPosition(this, column, row);
                ball.SetType(typePool[0]);
                typePool.RemoveAt(0);

                ball.transform.parent = gameObject.transform;
                rowBalls.Add(ball);

                if (gridBalls.Count < emptyLines)
                {
                    ball.gameObject.SetActive(false);
                }
            }

            gridBalls.Add(rowBalls);
        }

        var p = transform.position;
        p.y -= 4.7f;
        transform.position = p;

    }


    void AddLine()
    {


        ROWS++;

        var rowBalls = new List<Bubble>();

        for (int column = 0; column < COLUMNS; column++)
        {

            var item = Instantiate(gridBallGO) as GameObject;

            var ball = item.GetComponent<Bubble>();
            ball.transform.parent = gameObject.transform;
            ball.SetBallPosition(this, column, gridBalls.Count - 1);
            ball.SetType(typePool[0]);
            ball.connected = true;

            typePool.RemoveAt(0);

            rowBalls.Add(ball);
        }
        gridBalls.Add(rowBalls);

    }


    public void AddBall(Bubble collisionBall, Bullet bullet)
    {

        var neighbors = BallEmptyNeighbors(collisionBall);
        var minDistance = 10000.0f;
        Bubble minBall = null;
        foreach (var n in neighbors)
        {
            var d = Vector2.Distance(n.transform.position, bullet.transform.position);
            if (d < minDistance)
            {
                minDistance = d;
                minBall = n;
            }
        }
        bullet.gameObject.SetActive(false);
        minBall.SetType(bullet.type);
        minBall.gameObject.SetActive(true);

        CheckMatchesForBall(minBall);

    }


    public void CheckMatchesForBall(Bubble ball)
    {

        matchList.Clear();

        foreach (var r in gridBalls)
        {
            foreach (var b in r)
            {
                b.visited = false;
            }
        }


        //search for matches around ball
        var initialResult = GetMatches(ball);
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

                if (matchList.Count > 2)
                {

                    foreach (var b in matchList)
                    {
                        b.gameObject.SetActive(false);
                    }

                    CheckForDisconnected();

                    //remove disconnected balls
                    var i = gridBalls.Count - 1;
                    while (i >= 0)
                    {
                        foreach (var b in gridBalls[i])
                        {
                            if (!b.connected)
                            {
                                b.gameObject.SetActive(false);
                            }
                        }
                        i--;
                    }
                }
                return;
            }
        }
    }


    void CheckForDisconnected()
    {
        //set all balls as disconnected
        foreach (var r in gridBalls)
        {
            foreach (var b in r)
            {
                b.connected = false;
            }
        }
        //connect visible balls in last row 
        foreach (var b in gridBalls[ROWS - 1])
        {
            if (b.gameObject.activeSelf)
                b.connected = true;
        }


        //now set connect property on the rest of the balls
        var i = ROWS - 1;
        while (i >= 0)
        {
            foreach (var b in gridBalls[i])
            {
                if (b.gameObject.activeSelf)
                {
                    var neighbors = BallActiveNeighbors(b);
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
            i--;
        }
    }


    List<Bubble> GetMatches(Bubble ball)
    {
        ball.visited = true;
        var result = new List<Bubble>() { ball };
        var n = BallActiveNeighbors(ball);

        foreach (var b in n)
        {
            if (b.type == ball.type)
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



    Bubble.BALL_TYPE GetBallType()
    {
        var random = Random.Range(0.0f, 1.0f);
        if (random > changeTypeRate)
        {
            lastType = (Bubble.BALL_TYPE)Random.Range(0, 5);
        }
        return lastType;
    }



    List<Bubble> BallEmptyNeighbors(Bubble ball)
    {
        var result = new List<Bubble>();
        if (ball.column + 1 < COLUMNS)
        {
            if (!gridBalls[ball.row][ball.column + 1].gameObject.activeSelf)
                result.Add(gridBalls[ball.row][ball.column + 1]);
        }

        //left
        if (ball.column - 1 >= 0)
        {
            if (!gridBalls[ball.row][ball.column - 1].gameObject.activeSelf)
                result.Add(gridBalls[ball.row][ball.column - 1]);
        }
        //top
        if (ball.row - 1 >= 0)
        {
            if (!gridBalls[ball.row - 1][ball.column].gameObject.activeSelf)
                result.Add(gridBalls[ball.row - 1][ball.column]);
        }

        //bottom
        if (ball.row + 1 < gridBalls.Count)
        {
            if (!gridBalls[ball.row + 1][ball.column].gameObject.activeSelf)
                result.Add(gridBalls[ball.row + 1][ball.column]);
        }

        if (ball.column % 2 == 0)
        {

            //top-left
            if (ball.row - 1 >= 0 && ball.column - 1 >= 0)
            {
                if (!gridBalls[ball.row - 1][ball.column - 1].gameObject.activeSelf)
                    result.Add(gridBalls[ball.row - 1][ball.column - 1]);
            }

            //top-right
            if (ball.row - 1 >= 0 && ball.column + 1 < COLUMNS)
            {
                if (!gridBalls[ball.row - 1][ball.column + 1].gameObject.activeSelf)
                    result.Add(gridBalls[ball.row - 1][ball.column + 1]);
            }
        }
        else
        {
            //bottom-left
            if (ball.row + 1 < gridBalls.Count && ball.column - 1 >= 0)
            {
                if (!gridBalls[ball.row + 1][ball.column - 1].gameObject.activeSelf)
                    result.Add(gridBalls[ball.row + 1][ball.column - 1]);
            }

            //bottom-right
            if (ball.row + 1 < gridBalls.Count && ball.column + 1 < COLUMNS)
            {
                if (!gridBalls[ball.row + 1][ball.column + 1].gameObject.activeSelf)
                    result.Add(gridBalls[ball.row + 1][ball.column + 1]);
            }

        }


        return result;
    }

    List<Bubble> BallActiveNeighbors(Bubble ball)
    {

        var result = new List<Bubble>();
        //right
        if (ball.column + 1 < COLUMNS)
        {
            if (gridBalls[ball.row][ball.column + 1].gameObject.activeSelf)
                result.Add(gridBalls[ball.row][ball.column + 1]);
        }

        //left
        if (ball.column - 1 >= 0)
        {
            if (gridBalls[ball.row][ball.column - 1].gameObject.activeSelf)
                result.Add(gridBalls[ball.row][ball.column - 1]);
        }
        //bottom
        if (ball.row - 1 >= 0)
        {
            if (gridBalls[ball.row - 1][ball.column].gameObject.activeSelf)
                result.Add(gridBalls[ball.row - 1][ball.column]);
        }

        //top
        if (ball.row + 1 < gridBalls.Count)
        {
            if (gridBalls[ball.row + 1][ball.column].gameObject.activeSelf)
                result.Add(gridBalls[ball.row + 1][ball.column]);
        }


        if (ball.column % 2 == 0)
        {

            //top-left
            if (ball.row - 1 >= 0 && ball.column - 1 >= 0)
            {
                if (gridBalls[ball.row - 1][ball.column - 1].gameObject.activeSelf)
                    result.Add(gridBalls[ball.row - 1][ball.column - 1]);
            }

            //top-right
            if (ball.row - 1 >= 0 && ball.column + 1 < COLUMNS)
            {
                if (gridBalls[ball.row - 1][ball.column + 1].gameObject.activeSelf)
                    result.Add(gridBalls[ball.row - 1][ball.column + 1]);
            }
        }
        else
        {
            //bottom-left
            if (ball.row + 1 < gridBalls.Count && ball.column - 1 >= 0)
            {
                if (gridBalls[ball.row + 1][ball.column - 1].gameObject.activeSelf)
                    result.Add(gridBalls[ball.row + 1][ball.column - 1]);
            }

            //bottom-right
            if (ball.row + 1 < gridBalls.Count && ball.column + 1 < COLUMNS)
            {
                if (gridBalls[ball.row + 1][ball.column + 1].gameObject.activeSelf)
                    result.Add(gridBalls[ball.row + 1][ball.column + 1]);
            }

        }

        return result;
    }

    public Bubble BallCloseToPoint(Vector2 point)
    {

        point.y -= transform.position.y;

        int c = Mathf.FloorToInt((point.x + GRID_OFFSET_X + (TILE_SIZE * 0.5f)) / TILE_SIZE);
        if (c < 0)
            c = 0;
        if (c >= COLUMNS)
            c = COLUMNS - 1;

        int r = Mathf.FloorToInt(((TILE_SIZE * 0.5f) + point.y) / TILE_SIZE);
        if (r < 0) r = 0;
        if (r >= gridBalls.Count) r = gridBalls.Count - 1;



        return gridBalls[r][c];

    }


    void Update()
    {

        var p = transform.position;
        p.y -= Time.deltaTime * GRID_SPEED;
        transform.position = p;


        if (gridBalls[gridBalls.Count - 1][0].transform.position.y < 6)
        {

            //add new line
            AddLine();
        }

    }

    private static System.Random rng = new System.Random();
    public static void Shuffle<T>(IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }




}
