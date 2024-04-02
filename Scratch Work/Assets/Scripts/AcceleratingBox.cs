using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcceleratingBox : MonoBehaviour
{
    public Vector2 velocity = new Vector2(0, 0);

    public Vector2 acceleration = new Vector2(0, 0);

    public GameObject pointObj;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("createPoint", 0f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        //update velocity in acordance with acceleration
        velocity += acceleration * Time.deltaTime;

        //finally, transform via rules
        transform.position += new Vector3(velocity.x, velocity.y, 0) * Time.deltaTime;
    }

    void createPoint() {
        GameObject instance = Instantiate(pointObj,this.transform.position, Quaternion.identity);
    }
}
