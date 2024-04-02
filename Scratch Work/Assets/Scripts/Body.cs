using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body : MonoBehaviour
{
    public float mass = 200.0f;

    public Vector2 velocity = new Vector2(0, 0);

    public Vector2 acceleration = new Vector2(0, 0);

    public Vector2 netForce = new Vector2(0, 0);

    private bool isRunning = true;

    public Collider2D thisCollider;

    // Start is called before the first frame update
    void Start()
    {
        thisCollider = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isRunning == true)
        {
            acceleration = netForce / mass;

            //update velocity in acordance with acceleration
            velocity += acceleration * Time.deltaTime;

            //finally, transform via rules
            transform.position += new Vector3(velocity.x, velocity.y, 0) * Time.deltaTime;
        }
    }

    public void Test() {
        Debug.Log(mass);
    }

    public void applyForce(Vector2 force) {
        netForce += force;
    }


    public void setIsRunning(bool isA) { 
        this.isRunning = isA;
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == "Body") {
            //apply normal force ig?
            //idk ask braithwaite

            //oke asked
            //now start with calculating the colission velociyies on the x

            Body body2 = collision.gameObject.GetComponent<Body>();

            float mass2 = body2.mass;
            Vector2 velocity2 = body2.velocity;

            //problem with this caluation is that energy is not conserved in both axis like momentem is, but these equations assume it is

            //find new solution somehow?
            //oke new solution found. it stupid.

            float velocityMag = Mathf.Sqrt(Mathf.Pow(velocity.x, 2) + Mathf.Pow(velocity.y, 2));
            float velocityMag2 = Mathf.Sqrt(Mathf.Pow(velocity2.x, 2) + Mathf.Pow(velocity2.y, 2));

            float theta1 = BodyController.findAngle(velocity.x,velocity.y, velocityMag);
            float theta2 = BodyController.findAngle(velocity2.x, velocity2.y, velocityMag2);

            //find the closest point on edge of colider since straight line between them

            Vector2 reboundPoint = thisCollider.ClosestPoint(collision.gameObject.transform.position);

            //find this point relitive to the original, the tangent will be perpendicular to this vector

            Vector2 reboundRelitive = reboundPoint - new Vector2(this.transform.position.x,this.transform.position.y);

            //the tangent runs perpendicular, so to find a point on that it can be a transofation of the point (x,y) --> (-y,x);

            //no clue why this is the tangent but itworks babyyyyyyyyyyy
            
            //not this anymore --> Vector2 impactTangentVec = new Vector2(-reboundRelitive.y, reboundRelitive.x);
            Vector2 impactTangentVec = reboundRelitive;

            //find ange phi 

            float phi = BodyController.findAngle(impactTangentVec.x, impactTangentVec.y, Mathf.Sqrt(Mathf.Pow(impactTangentVec.x, 2) + Mathf.Pow(impactTangentVec.y, 2)));

            //make the smol ange
            
            if (phi > Mathf.PI) {
                phi = phi - Mathf.PI;
            }
            
            //its stupid time
            float topFraction = velocityMag*Mathf.Cos(theta1 - phi)*(mass-mass2) + 2.0f*mass2*velocityMag2*Mathf.Cos(theta2-phi);
            float bottemFraction = mass + mass2;

            float totalFractionX = (topFraction/bottemFraction)*Mathf.Cos(phi);
            float totalFractionY = (topFraction / bottemFraction) * Mathf.Sin(phi);

            float additionX = velocityMag*Mathf.Sin(theta1-phi)*Mathf.Cos(phi + (Mathf.PI/2.0f));
            float additionY = velocityMag*Mathf.Sin(theta1 - phi)*Mathf.Sin(phi + (Mathf.PI / 2.0f));

            float newVelocityX = totalFractionX + additionX;
            float newVelocityY = totalFractionY + additionY;

            velocity = new Vector2 (newVelocityX, newVelocityY);

        }
    }

}
