using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public float camMoveSpeed = 10.0f;

    public float camScaleSpeed = 10.0f;

    public Camera camComponent;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalAxis = Input.GetAxis("Horizontal");
        float verticalAxis = Input.GetAxis("Vertical");

        float zoom = Input.GetAxis("zoom");
        if (camComponent.orthographicSize < 0f) {
            camComponent.orthographicSize = 0;
        }
        else if (zoom == 1f)
        {
            camComponent.orthographicSize -= camScaleSpeed * Time.deltaTime;
        }
        else if (zoom == -1f) {
            camComponent.orthographicSize += camScaleSpeed * Time.deltaTime;
        }

        transform.Translate(new Vector3(horizontalAxis,verticalAxis)*camMoveSpeed*Time.deltaTime);
    }
}
