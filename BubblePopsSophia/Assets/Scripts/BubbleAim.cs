
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BubbleAim : MonoBehaviour
{
    public GameObject[] colorsGO;
    public GameObject dotPrefab;

    private bool mouseDown = false;
    private List<Vector2> dots;
    private List<GameObject> dotsPool;


    // Use this for initialization
    void Start()
    {

        dots = new List<Vector2>();
        dotsPool = new List<GameObject>();
        var i = 0;
        while (i < 100)
        {
            var d = Instantiate(dotPrefab) as GameObject;
            d.SetActive(false);
            dotsPool.Add(d);
            i++;
        }
    }


    void HandleTouchDown(Vector2 touch)
    {
    }

    void HandleTouchUp(Vector2 touch)
    {


        if (dots == null || dots.Count < 2)
            return;

        foreach (var d in dotsPool)
            d.SetActive(false);

    }

    void HandleTouchMove(Vector2 touch)
    {


        if (dots == null)
        {
            return;
        }

        dots.Clear();

        foreach (var d in dotsPool)
            d.SetActive(false);

        Vector2 point = Camera.main.ScreenToWorldPoint(touch);
        var direction = new Vector2(point.x - transform.position.x, point.y - transform.position.y);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction);
        if (hit.collider != null)
        {

            dots.Add(transform.position);

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

        dots.Add(previousHit.point);

        var normal = Mathf.Atan2(previousHit.normal.y, previousHit.normal.x);
        var newDirection = normal + (normal - Mathf.Atan2(directionIn.y, directionIn.x));
        var reflection = new Vector2(-Mathf.Cos(newDirection), -Mathf.Sin(newDirection));
        var newCastPoint = previousHit.point + (2 * reflection);

        //		directionIn.Normalize ();
        //		newCastPoint = new Vector2(previousHit.point.x + 2 * (-directionIn.x), previousHit.point.y + 2 * (directionIn.y));
        //		reflection = new Vector2 (-directionIn.x, directionIn.y);

        var hit2 = Physics2D.Raycast(newCastPoint, reflection);
        if (hit2.collider != null)
        {
            if (hit2.collider.tag == "Boundary")
            {
                //shoot another cast
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

        foreach (var d in dotsPool)
            d.SetActive(false);

        for (var i = 0; i < dots.Count; i++)
        {
            dotsPool[i].SetActive(true);
            dotsPool[i].transform.position = dots[i];
        }
    }


    // Update is called once per frame
    void Update()
    {

        if (dots == null)
            return;

        if (Input.touches.Length > 0)
        {

            Touch touch = Input.touches[0];

            if (touch.phase == TouchPhase.Began)
            {
                HandleTouchDown(touch.position);
            }
            else if (touch.phase == TouchPhase.Canceled || touch.phase == TouchPhase.Ended)
            {
                HandleTouchUp(touch.position);
            }
            else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
                HandleTouchMove(touch.position);
            }
            HandleTouchMove(touch.position);
            return;
        }
        else if (Input.GetMouseButtonDown(0))
        {
            mouseDown = true;
            HandleTouchDown(Input.mousePosition);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            mouseDown = false;
            HandleTouchUp(Input.mousePosition);
        }
        else if (mouseDown)
        {
            HandleTouchMove(Input.mousePosition);
        }
    }

}
