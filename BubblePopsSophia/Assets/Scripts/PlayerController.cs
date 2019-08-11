
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    public GameObject[] bubblesGO;
    public GameObject[] nextBubblesGO;
    public GameObject aimDotPrefab;
    public ShotBubble shotBubble;
    public BubblesGrid grid;

    private List<Vector2> dots;
    private List<GameObject> dottedLine;
    private bool mouseDown = false;
    private int dotsLength = 30;
    private float dotGap = 0.3f;

    private float shotBubbleProgress = 0.0f;
    private float shotBubbleInc = 0.0f;
    private int CurrentbubbleColorType = 0;
    private int nextbubbleColorType = 0;
    private int bullets = 0;

    void Start()
    {
        dots = new List<Vector2>();
        dottedLine = new List<GameObject>();
        float alpha = 1.0f / dotsLength;
        float startAlpha = 1;
        int i = 0;
        
        //Instantiate dots
        while (i < dotsLength)
        {
            GameObject dot = Instantiate(aimDotPrefab) as GameObject;
            SpriteRenderer sp = dot.GetComponent<SpriteRenderer>();
            Color c = sp.color;
            c.a = startAlpha - alpha;
            startAlpha -= alpha;
            sp.color = c;
            //seting the alpha of the dots to reduce over length of the cast
            dot.SetActive(false);
            dottedLine.Add(dot);
            i++;
        }

        SetNextBubbleColor();
    }

    void Update()
    {

        if (dots == null)
            return;

        if (Input.touches.Length > 0)
        {
            Touch touch = Input.touches[0];
            if (touch.phase == TouchPhase.Began)
            {MouseDown(touch.position);}

            else if (touch.phase == TouchPhase.Canceled || touch.phase == TouchPhase.Ended)
            {MouseUp(touch.position);}

            else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {MouseMove(touch.position);}

            MouseMove(touch.position);
            return;
        }
        else if (Input.GetMouseButtonDown(0))
        {
            mouseDown = true;
            MouseDown(Input.mousePosition);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            mouseDown = false;
            MouseUp(Input.mousePosition);
        }
        else if (mouseDown)
        {   MouseMove(Input.mousePosition);}
        //The above code determines when a touch press or drag or touch up has happened

        if (shotBubble.gameObject.activeSelf)
        {
            shotBubbleProgress += shotBubbleInc;
            if (shotBubbleProgress  > 1)
            {
                dots.RemoveAt(0);   //removing the dots list 
                //At the end of the path making the shot bubble disappear
                if (dots.Count < 2)
                {
                    shotBubble.gameObject.SetActive(false);
                    return;
                }
                else
                {
                    InitPath();
                }
            }
            //determining the next move transform for the shot bubble similar to the dots
            float px = dots[0].x + shotBubbleProgress * (dots[1].x - dots[0].x);
            float py = dots[0].y + shotBubbleProgress * (dots[1].y - dots[0].y);

            shotBubble.transform.position = new Vector2(px, py);
            return;
        }
    }   

    void MouseDown(Vector2 touch)
    {
    }

    void MouseUp(Vector2 touch)
    {   
        if (dots == null || dots.Count < 2)
            return;
        // When Mouse is up the dots will disappear
        foreach (GameObject x in dottedLine)
            x.SetActive(false);
        shotBubbleProgress = 0.0f;
       //Here the currentBubbleColor is shot from the shooter 
        shotBubble.SetType((Bubble.BUBBLE_TYPE)CurrentbubbleColorType);
        shotBubble.gameObject.SetActive(true);
        shotBubble.transform.position = transform.position;
        InitPath();
        //This Sets the color of next bubble to be shot
        SetNextBubbleColor();
    }

    void MouseMove(Vector2 touch)
    {
        if (shotBubble.gameObject.activeSelf)
            return;

        if (dots == null)
        {
            return;
        }
        //First clearing out the existing dots
        dots.Clear();

        foreach (GameObject x in dottedLine)
            x.SetActive(false);
        
        //Setting the transform of the dots at the position of touchDirection
        Vector2 point = Camera.main.ScreenToWorldPoint(touch);
        Vector2 direction = new Vector2(point.x - transform.position.x, point.y - transform.position.y);
        Vector3 newPostion = Vector3.MoveTowards(shotBubble.transform.position, point, 10f * Time.deltaTime);

        //Raycasting
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction);
        if (hit.collider != null)
        {
            dots.Add(transform.position);
            //Check for boundary hits and draw path from boundary
            if (hit.collider.tag == "Boundary")
            {
                DoRayCast(hit, direction);
            }
            else
            {
                dots.Add(hit.point);
                DrawPaths();
            }
        }
    }

    void DoRayCast(RaycastHit2D previousHit, Vector2 directionIn)
    {
        //Calculating the reflected new point to hit after a boundary is hit
        dots.Add(previousHit.point);
        float normal = Mathf.Atan2(previousHit.normal.y, previousHit.normal.x);
        float newDirection = normal + (normal - Mathf.Atan2(directionIn.y, directionIn.x));
        Vector2 reflection = new Vector2(-Mathf.Cos(newDirection), -Mathf.Sin(newDirection));
        Vector2 newCastPoint = previousHit.point + (2 * reflection);

        Vector3 newPostion = Vector3.MoveTowards(shotBubble.transform.position, newCastPoint, 10f * Time.deltaTime);
        RaycastHit2D hit2 = Physics2D.Raycast(newCastPoint, reflection);
        if (hit2.collider != null)
        {
            if (hit2.collider.tag == "Boundary")
            {
                //If again a boundary is met then recursively cast a ray
                DoRayCast(hit2, reflection);
            }
            else
            {
                dots.Add(hit2.point);
                DrawPaths();
            }
        }
        else
        {
            DrawPaths();
        }
    }

    void DrawPaths()
    {

        foreach (GameObject d in dottedLine)
            d.SetActive(false);

        //Making all points visible
        int index = 0;
        for (int i = 1; i < dots.Count; i++)
        {
            DrawSubPath(i - 1, i, ref index);
        }
        
    }
    
    void DrawSubPath(int start, int end, ref int index)
    {   
        //Start and end point between two main dots
        float pathLength = Vector2.Distance(dots[start], dots[end]);
        //Find the count of dots to fill the above distance
        int numDots = Mathf.RoundToInt((float)pathLength / dotGap);
        float dotProgress = 1.0f / numDots;     //dotprogrss is like a percentage of the doted path. value 1 means 100 percent finished.

        float p = 0.0f;

        while (p < 1)
        {   
            //each point p is marked by calculating the x and Y progress along the path calculated above
            float px = dots[start].x + p * (dots[end].x - dots[start].x);
            float py = dots[start].y + p * (dots[end].y - dots[start].y);

            if (index < dotsLength)
            {
                //Here it is all about setting active the exact dots at exact place
                GameObject d = dottedLine[index];
                d.transform.position = new Vector2(px, py);
                d.SetActive(true);
                index++;
            }

            p += dotProgress;   //increement the value for the loop
        }
    }

    void SetNextBubbleColor()
    {
        int count=0;
        foreach (GameObject go in bubblesGO)
        {
            go.SetActive(false);
        }
        foreach (GameObject nextgo in nextBubblesGO)
        {
            if (nextgo.activeSelf)
            {
                //set the current bubble equal to the nextPlayerBubble color
                bubblesGO[count].SetActive(true);
                nextgo.SetActive(false);
                CurrentbubbleColorType = count;
            }
            count++;
            if (count > 10)  //Check for arrayIndexOut of bounds
               count = 0;
            //print("CurrentbubbleColorType" + CurrentbubbleColorType);
        }

        nextbubbleColorType = Random.Range(0, 7);   
        //randomizing till 7(128 bubbletype) because I dont want bigger numbers 
        nextBubblesGO[nextbubbleColorType].SetActive(true);

        bullets++;
        if (bullets > 5)
        {
            bullets = 0;
            grid.AddLine();
        }
    }

    void InitPath()
    {
        //this method moves the shot bubble in the path of the dotted line
        Vector2 start = dots[0];
        Vector2 end = dots[1];
        float length = Vector2.Distance(start, end);
        float iterations = length / 0.15f;
        shotBubbleProgress = 0.0f;
        shotBubbleInc = 1.0f / iterations;
    }
}
