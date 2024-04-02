using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyController : MonoBehaviour
{
    private Body[] bodies;
    private GameObject controller;
    public bool paused = false;
    
    public Camera cam;
    private Vector3 point;
    public Body selectedBody;

    public GameObject vectorObject;
    private GameObject[] vectorDiagram;

    private float G = (6.67f) * (Mathf.Pow(10, -11));


    // Start is called before the first frame update
    void Start()
    {
        controller = this.gameObject;

        bodies = new Body[controller.transform.childCount];

        for (int i = 0; i < controller.transform.childCount; i++) {
            GameObject child = controller.transform.GetChild(i).gameObject;
            bodies[i] = child.GetComponent<Body>();
        }
        //one for each body
        int numV = bodies.Length * (bodies.Length - 1);
        //one for manual aplication
        numV += 1;

        vectorDiagram = new GameObject[numV];
    }

    // Update is called once per frame
    void Update()
    {
        clearVecs();

        //logic for manual forces
        if ((Input.GetMouseButtonDown(0) && paused == false)) //on first click
        {
            //get the mouse position
            Vector3 currentMPos = Input.mousePosition;
            Vector3 point1 = cam.ScreenToWorldPoint(new Vector3(currentMPos.x, currentMPos.y, cam.nearClipPlane));
            //if clicking a body attempt to select and pause the movement updates
            for (int i = 0; i < bodies.Length; i++)
            {
                if (bodies[i].thisCollider.OverlapPoint(new Vector2(point1.x,point1.y)))
                {
                    selectedBody = bodies[i];
                    paused = true;
                    break;
                }
            }
        }//when holding
        else if (Input.GetMouseButton(0) && paused == true)
        {
            //update for the position of the force
            Vector3 mousePos2 = Input.mousePosition;
            point = cam.ScreenToWorldPoint(new Vector3(mousePos2.x, mousePos2.y, cam.nearClipPlane));
            //updateVecDiagram
            newVec(new Vector2((point.x - selectedBody.gameObject.transform.position.x), (point.y - selectedBody.gameObject.transform.position.y)), new Vector2(selectedBody.gameObject.transform.position.x, selectedBody.gameObject.transform.position.y), vectorDiagram.Length - 1);


        }//when let go off
        else if (paused == true) {
            //apply the new force
            float distX = (point.x - selectedBody.gameObject.transform.position.x);
            float distY = (point.y - selectedBody.gameObject.transform.position.y);

            Vector2 force = new Vector2(distX,distY);

            selectedBody.applyForce(force);

            for (int i = 0; i < bodies.Length; i++)
            {
                bodies[i].setIsRunning(true);
            }

            paused = false;
        }


        //logic for gravity
        if (paused == true) {
            for (int i = 0; i < bodies.Length; i++) {
                bodies[i].setIsRunning(false);
            }
        }
        else
        {
            
            //for the body i
            for (int i = 0; i < bodies.Length; i++)
            {

                //effect its trajectoy by all th other ones
                for (int j = 0; j < bodies.Length; j++)
                {
                    if (i != j)
                    {
                        float upperHalf = (G * bodies[i].mass * bodies[j].mass);
                        //divide by a thousand since each unit is a km
                        float distX = (bodies[i].gameObject.transform.position.x - bodies[j].gameObject.transform.position.x) / 1000;
                        float distY = (bodies[i].gameObject.transform.position.y - bodies[j].gameObject.transform.position.y) / 1000;

                        float distT = Mathf.Sqrt(Mathf.Pow(distX, 2) + Mathf.Pow(distY, 2));

                        float fGravity = upperHalf / Mathf.Pow(distT, 2);

                        float angle = findAngle(distY, distX);

                        float fGravityX = findSide(angle, fGravity, true) * -Mathf.Sign(distX);
                        float fGravityY = findSide(angle, fGravity, false) * -Mathf.Sign(distY);

                        bodies[i].applyForce(new Vector2(fGravityX, fGravityY));
                        
                    }

                }
            }
            
        }
        //clear all vecs

        
    }

    public float findAngle(float y, float x) {
        float angle;

        if (y == 0 || x == 0) {
            angle = 0;
        }
        else {
            x = Mathf.Abs(x);
            y = Mathf.Abs(y);
            angle = Mathf.Atan(y / x);
        }

        return angle;
    }

    public static float findAngle(float x, float y, float h) {
        float angle = 0;
        
        if (x > 0 && y > 0)
        {
            angle = Mathf.Atan(y / x);
        }
        else if (x < 0 && y > 0)
        {
            angle = Mathf.PI - Mathf.Asin(y / h);
        }
        else if (x < 0 && y < 0)
        {
            angle = Mathf.PI + Mathf.Atan(y / x);
        }
        else if (x > 0 && y < 0)
        {
            angle = 2.0f*Mathf.PI - Mathf.Acos(x / h);
        }
        else if (y == 0)
        {
            if (x > 0)
            {
                angle = 0;
            }
            else
            {
                angle = Mathf.PI;
            }
        }
        else if (x == 0) {
            if (y > 0)
            {
                angle = Mathf.PI/2.0f;
            }
            else {
                angle = (3.0f*Mathf.PI)/2.0f;
            }
        }
        return angle;
    }

    public float findSide(float angle, float r, bool isX) {
        float side;

        if (isX) {
            side = Mathf.Cos(angle) * r;
        }
        else { 
            side = Mathf.Sin(angle) * r;
        }

        return side;
    }

    public void newVec(Vector2 vec, Vector2 InitLocation,int index) {
        //do this nowers
        float hypotonuse = Mathf.Sqrt(Mathf.Pow(vec.x, 2) + Mathf.Pow(vec.y, 2));

        Vector3 point = new Vector3(InitLocation.x + (vec.x / 2.0f),InitLocation.y + (vec.y / 2.0f));
        vectorDiagram[index] = Instantiate(vectorObject,point, Quaternion.identity);
        vectorDiagram[index].transform.localScale = new Vector3(hypotonuse, 1, 1);
        vectorDiagram[index].transform.rotation = Quaternion.Euler(0,0, Mathf.Rad2Deg * findAngle(vec.x,vec.y,hypotonuse));

    }

    public void clearVecs() { 
        for (int i = 0; i < vectorDiagram.Length; i++)
        {
            if (vectorDiagram[i] != null)
            {
                Destroy(vectorDiagram[i]);
            }
        }
    }
}
